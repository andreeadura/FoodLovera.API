#nullable enable

using System.Security.Cryptography;
using FoodLovera.Core.Contracts;
using FoodLovera.Core.Exceptions;
using FoodLovera.Core.Helpers;
using FoodLovera.Models.Entities;
using FoodLovera.Models.Models;
using Microsoft.Extensions.Configuration;
using AuthenticationException = FoodLovera.Core.Exceptions.AuthenticationException;

namespace FoodLovera.Core.Services;

public sealed class AuthService : IAuthService
{
    private const int VerificationCodeExpiresMinutes = 15;
    private const int VerificationMaxAttempts = 10;

    private readonly IUserRepository _users;
    private readonly IEmailVerificationTokenRepository _emailTokens;
    private readonly IEmailSender _emailSender;
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwt;
    private readonly IConfiguration _configuration;
    private readonly IPasswordResetTokenRepository _passwordResetTokens;



    public AuthService(
        IUserRepository users,
        IEmailVerificationTokenRepository emailTokens,
        IEmailSender emailSender,
        IUnitOfWork uow,
        IJwtTokenService jwt,
        IConfiguration configuration,
        IPasswordResetTokenRepository passwordResetTokens)
    {
        _users = users;
        _emailTokens = emailTokens;
        _emailSender = emailSender;
        _uow = uow;
        _jwt = jwt;
        _configuration = configuration;
        _passwordResetTokens = passwordResetTokens;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.", nameof(request.Email));
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required.", nameof(request.Password));

        var email = request.Email.Trim().ToLowerInvariant();

        var existing = await _users.GetByEmailAsync(email, ct);
        if (existing is not null)
            throw new ConflictException("Email already registered.");

        var pwErrors = PasswordStrengthValidator.Validate(request.Password, minLength: 8);
        if (pwErrors.Count > 0)
            throw new ArgumentException("Weak password:\n- " + string.Join("\n- ", pwErrors), nameof(request.Password));

        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow,
            IsEmailVerified = false
        };

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct); 

        await UpsertAndSendVerificationCodeAsync(user, ct);

        return new AuthResponse(null, true);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.", nameof(request.Email));
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required.", nameof(request.Password));

        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            throw new AuthenticationException("Invalid credentials.");

        if (!user.IsEmailVerified)
            throw new ConflictException("Email not verified. Please verify your email before logging in.");

        return new AuthResponse(_jwt.CreateAccessToken(user), false);
    }

    public async Task VerifyEmailAsync(VerifyEmailRequestDTO request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.", nameof(request.Email));
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ArgumentException("Code is required.", nameof(request.Code));

        var email = request.Email.Trim().ToLowerInvariant();
        var code = request.Code.Trim();

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.IsEmailVerified)
            return; 

        var token = await _emailTokens.GetByUserIdAsync(user.Id, ct);
        if (token is null)
            throw new ConflictException("No verification code found. Please request a new code.");

        if (token.ExpiresAtUtc <= DateTime.UtcNow)
            throw new ConflictException("Verification code expired. Please request a new code.");

        if (token.AttemptCount >= VerificationMaxAttempts)
            throw new ConflictException("Too many attempts. Please request a new code.");

        token.AttemptCount++;
        token.LastAttemptAtUtc = DateTime.UtcNow;

        var pepper = GetPepperOrThrow();
        var expectedHash = VerificationCode.Hash(code + pepper);

        var ok = FixedTimeEqualsHex(token.CodeHash, expectedHash);
        if (!ok)
        {
            await _uow.SaveChangesAsync(ct);
            throw new ArgumentException("Invalid verification code.", nameof(request.Code));
        }

        user.IsEmailVerified = true;

        _emailTokens.Remove(token);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task ResendVerificationCodeAsync(string email, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        var normalized = email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(normalized, ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.IsEmailVerified)
            return;

        await UpsertAndSendVerificationCodeAsync(user, ct);
    }

    private async Task UpsertAndSendVerificationCodeAsync(User user, CancellationToken ct)
    {
        var code = VerificationCode.Generate6Digits();

        var pepper = GetPepperOrThrow();
        var codeHash = VerificationCode.Hash(code + pepper);

        var token = await _emailTokens.GetByUserIdAsync(user.Id, ct);
        if (token is null)
        {
            token = new EmailVerificationToken
            {
                UserId = user.Id,
                CodeHash = codeHash,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(VerificationCodeExpiresMinutes),
                AttemptCount = 0,
                LastAttemptAtUtc = null
            };

            await _emailTokens.AddAsync(token, ct);
        }
        else
        {
            token.CodeHash = codeHash;
            token.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(VerificationCodeExpiresMinutes);
            token.AttemptCount = 0;
            token.LastAttemptAtUtc = null;
        }

        await _uow.SaveChangesAsync(ct);

        await _emailSender.SendAsync(
            toEmail: user.Email,
            subject: "FoodLovera - Verify your email",
            body: $"Your verification code is: {code}\nIt expires in {VerificationCodeExpiresMinutes} minutes.",
            ct: ct);
    }
    public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDTO request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (userId <= 0) throw new ArgumentException("Invalid user id.", nameof(userId));

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            throw new ArgumentException("CurrentPassword is required.", nameof(request.CurrentPassword));
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new ArgumentException("NewPassword is required.", nameof(request.NewPassword));

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        if (!user.IsEmailVerified)
            throw new ConflictException("Email not verified.");

        if (!PasswordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new AuthenticationException("Current password is incorrect.");

        var pwErrors = PasswordStrengthValidator.Validate(request.NewPassword, minLength: 8);
        if (pwErrors.Count > 0)
            throw new ArgumentException("Weak password:\n- " + string.Join("\n- ", pwErrors), nameof(request.NewPassword));

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);

        await _uow.SaveChangesAsync(ct);
    }
    private const int PasswordResetCodeExpiresMinutes = 15;
    private const int PasswordResetMaxAttempts = 10;

    public async Task ForgotPasswordAsync(ForgotPasswordRequestDTO request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.", nameof(request.Email));

        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
            return;

        await UpsertAndSendPasswordResetCodeAsync(user, ct);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDTO request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.", nameof(request.Email));
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ArgumentException("Code is required.", nameof(request.Code));
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new ArgumentException("NewPassword is required.", nameof(request.NewPassword));

        var email = request.Email.Trim().ToLowerInvariant();
        var code = request.Code.Trim();

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        var token = await _passwordResetTokens.GetByUserIdAsync(user.Id, ct);
        if (token is null)
            throw new ConflictException("No reset code found. Please request a new code.");

        if (token.ExpiresAtUtc <= DateTime.UtcNow)
            throw new ConflictException("Reset code expired. Please request a new code.");

        if (token.AttemptCount >= PasswordResetMaxAttempts)
            throw new ConflictException("Too many attempts. Please request a new code.");

        token.AttemptCount++;
        token.LastAttemptAtUtc = DateTime.UtcNow;

        var pepper = GetPasswordResetPepperOrThrow();
        var expectedHash = VerificationCode.Hash(code + pepper);

        var ok = FixedTimeEqualsHex(token.CodeHash, expectedHash);
        if (!ok)
        {
            await _uow.SaveChangesAsync(ct);
            throw new ArgumentException("Invalid reset code.", nameof(request.Code));
        }

        var pwErrors = PasswordStrengthValidator.Validate(request.NewPassword, minLength: 8);
        if (pwErrors.Count > 0)
            throw new ArgumentException("Weak password:\n- " + string.Join("\n- ", pwErrors), nameof(request.NewPassword));

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);

        _passwordResetTokens.Remove(token);

        await _uow.SaveChangesAsync(ct);
    }

    private async Task UpsertAndSendPasswordResetCodeAsync(User user, CancellationToken ct)
    {
        var code = VerificationCode.Generate6Digits();

        var pepper = GetPasswordResetPepperOrThrow();
        var codeHash = VerificationCode.Hash(code + pepper);

        var token = await _passwordResetTokens.GetByUserIdAsync(user.Id, ct);
        if (token is null)
        {
            token = new PasswordResetToken
            {
                UserId = user.Id,
                CodeHash = codeHash,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(PasswordResetCodeExpiresMinutes),
                AttemptCount = 0,
                LastAttemptAtUtc = null
            };

            await _passwordResetTokens.AddAsync(token, ct);
        }
        else
        {
            token.CodeHash = codeHash;
            token.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(PasswordResetCodeExpiresMinutes);
            token.AttemptCount = 0;
            token.LastAttemptAtUtc = null;
        }

        await _uow.SaveChangesAsync(ct);

        await _emailSender.SendAsync(
            toEmail: user.Email,
            subject: "FoodLovera - Reset password",
            body: $"Your password reset code is: {code}\nIt expires in {PasswordResetCodeExpiresMinutes} minutes.",
            ct: ct);
    }

    private string GetPasswordResetPepperOrThrow()
        => _configuration["PasswordReset:Pepper"]
           ?? throw new InvalidOperationException("PasswordReset:Pepper missing from configuration.");

    private string GetPepperOrThrow()
        => _configuration["EmailVerification:Pepper"]
           ?? throw new InvalidOperationException("EmailVerification:Pepper missing from configuration.");

    private static bool FixedTimeEqualsHex(string hexA, string hexB)
    {
        
        var a = Convert.FromHexString(hexA);
        var b = Convert.FromHexString(hexB);
        return CryptographicOperations.FixedTimeEquals(a, b);
    }
}