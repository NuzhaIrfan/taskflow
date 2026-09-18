namespace TaskFlow.Lambda.Helpers;

public static class AppConfig
{
    // In production, this comes from Lambda environment variables
    public const string JwtSecret = "CHANGE-ME-IN-PRODUCTION-min-32-chars-long-secret-key";
    public const string JwtIssuer = "TaskFlow";
    public const string JwtAudience = "TaskFlowClient";
    public const int JwtExpiryMinutes = 60 * 24; // 24 hours
}