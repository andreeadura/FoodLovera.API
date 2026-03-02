using System.Security.Cryptography;
using FoodLovera.Core.Contracts;
using FoodLovera.Core.Repositories;
using FoodLovera.Models.Entities;
using FoodLovera.Models.Enums;

namespace FoodLovera.Core.Services;

public sealed class SessionService : ISessionService
{
    private readonly ISessionRepository _sessions;
    private readonly ISessionParticipantRepository _participants;

    public SessionService(ISessionRepository sessions, ISessionParticipantRepository participants)
    {
        _sessions = sessions;
        _participants = participants;
    }

    public async Task<CreateSessionResponse> CreateAsync(CreateSessionRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Session name is required.", nameof(request));

        var joinCode = await GenerateUniqueJoinCodeAsync(ct);

        var session = new Session
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            JoinCode = joinCode,
            CreatedAt = DateTime.UtcNow,

            IsActive = true,

            Status = SessionStatus.Active,
            CompletedReason = null,
            CompletedAt = null
        };

        await _sessions.AddAsync(session, ct);
        await _sessions.SaveChangesAsync(ct);

        return new CreateSessionResponse
        {
            SessionId = session.Id,
            JoinCode = session.JoinCode,
            Name = session.Name
        };
    }

    public async Task<JoinSessionResponse> JoinAsync(string joinCode, JoinSessionRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(joinCode))
            throw new ArgumentException("Join code is required.", nameof(joinCode));

        if (string.IsNullOrWhiteSpace(request.DisplayName))
            throw new ArgumentException("Display name is required.", nameof(request));

        var session = await _sessions.GetByJoinCodeAsync(joinCode.Trim(), ct);
        if (session is null)
            throw new InvalidOperationException("Session not found.");

        if (!session.IsActive || session.Status == SessionStatus.Completed)
            throw new InvalidOperationException("Session is not active.");

        var participant = new SessionParticipant
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            DisplayName = request.DisplayName.Trim(),
            UserId = null,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _participants.AddAsync(participant, ct);
        await _participants.SaveChangesAsync(ct);

        return new JoinSessionResponse
        {
            SessionId = session.Id,
            ParticipantId = participant.Id,
            DisplayName = participant.DisplayName
        };
    }

    private async Task<string> GenerateUniqueJoinCodeAsync(CancellationToken ct)
    {
        // cod din 6 caractere
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ23456789";
        var bytes = new byte[6];

        for (var attempt = 0; attempt < 20; attempt++)
        {
            RandomNumberGenerator.Fill(bytes); // random securizat criptografic

            var chars = new char[6];
            for (int i = 0; i < chars.Length; i++)
                chars[i] = alphabet[bytes[i] % alphabet.Length];

            var code = new string(chars);

            if (!await _sessions.JoinCodeExistsAsync(code, ct))
                return code;
        }

        throw new InvalidOperationException("Could not generate a unique join code. Try again.");
    }
}