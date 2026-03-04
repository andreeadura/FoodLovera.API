#nullable enable
namespace FoodLovera.Core.Exceptions;

public sealed class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message) { }
}