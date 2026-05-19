using ClearFX.Domain.Entities;

namespace ClearFX.Application.Common;

public interface IJwtService
{
    (string AccessToken, string RefreshToken) GenerateTokens(User user);
}