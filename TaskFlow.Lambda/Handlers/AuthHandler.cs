using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskFlow.Data.Models;
using TaskFlow.Lambda.Helpers;
using TaskFlow.Lambda.Models;

namespace TaskFlow.Lambda.Handlers;

public static class AuthHandlers
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<APIGatewayHttpApiV2ProxyResponse> Register(
        APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        var body = JsonSerializer.Deserialize<RegisterRequest>(request.Body ?? "{}", JsonOptions);

        if (body is null || string.IsNullOrWhiteSpace(body.Email) || string.IsNullOrWhiteSpace(body.Password))
            return ApiResponse.BadRequest("Email and password are required");

        if (body.Password.Length < 6)
            return ApiResponse.BadRequest("Password must be at least 6 characters");

        using var db = DbContextFactory.Create();

        var exists = await db.Users.AnyAsync(u => u.Email == body.Email);
        if (exists) return ApiResponse.BadRequest("Email already registered");

        var user = new User
        {
            Email = body.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(body.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = JwtHelper.GenerateToken(user);
        context.Logger.LogInformation($"Registered user {user.Id}");

        return ApiResponse.Created(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            UserId = user.Id
        });
    }

    public static async Task<APIGatewayHttpApiV2ProxyResponse> Login(
        APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        var body = JsonSerializer.Deserialize<LoginRequest>(request.Body ?? "{}", JsonOptions);

        if (body is null || string.IsNullOrWhiteSpace(body.Email) || string.IsNullOrWhiteSpace(body.Password))
            return ApiResponse.BadRequest("Email and password are required");

        using var db = DbContextFactory.Create();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == body.Email);
        if (user is null) return ApiResponse.Unauthorized("Invalid credentials");

        var ok = BCrypt.Net.BCrypt.Verify(body.Password, user.PasswordHash);
        if (!ok) return ApiResponse.Unauthorized("Invalid credentials");

        var token = JwtHelper.GenerateToken(user);
        context.Logger.LogInformation($"Login user {user.Id}");

        return ApiResponse.Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            UserId = user.Id
        });
    }
}