using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Data.Models;

namespace TaskFlow.Lambda.Helpers;

public static class JwtHelper
{
    public static string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: AppConfig.JwtIssuer,
            audience: AppConfig.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(AppConfig.JwtExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Returns user id from a Bearer token, or null if invalid.
    /// </summary>
    public static int? ValidateTokenAndGetUserId(string? authHeader)
    {
        if (string.IsNullOrWhiteSpace(authHeader)) return null;
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return null;

        var token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtSecret));
            var handler = new JwtSecurityTokenHandler();

            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = AppConfig.JwtIssuer,
                ValidAudience = AppConfig.JwtAudience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            }, out _);

            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(sub, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Extracts Authorization header from either v1 or v2 API Gateway payload.
    /// </summary>
    public static string? GetAuthHeader(Amazon.Lambda.APIGatewayEvents.APIGatewayHttpApiV2ProxyRequest request)
    {
        if (request.Headers is null) return null;

        foreach (var kv in request.Headers)
        {
            if (string.Equals(kv.Key, "authorization", StringComparison.OrdinalIgnoreCase))
                return kv.Value;
        }
        return null;
    }
}