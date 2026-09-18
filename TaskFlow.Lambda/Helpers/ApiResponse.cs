using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

namespace TaskFlow.Lambda.Helpers;

public static class ApiResponse
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly Dictionary<string, string> JsonHeaders = new()
    {
        ["Content-Type"] = "application/json"
    };

    public static APIGatewayHttpApiV2ProxyResponse Ok(object body) => new()
    {
        StatusCode = 200,
        Body = JsonSerializer.Serialize(body, JsonOptions),
        Headers = JsonHeaders
    };

    public static APIGatewayHttpApiV2ProxyResponse Created(object body) => new()
    {
        StatusCode = 201,
        Body = JsonSerializer.Serialize(body, JsonOptions),
        Headers = JsonHeaders
    };

    public static APIGatewayHttpApiV2ProxyResponse NoContent() => new()
    {
        StatusCode = 204,
        Body = "",
        Headers = JsonHeaders
    };

    public static APIGatewayHttpApiV2ProxyResponse BadRequest(string message) => new()
    {
        StatusCode = 400,
        Body = JsonSerializer.Serialize(new { error = message }, JsonOptions),
        Headers = JsonHeaders
    };

    public static APIGatewayHttpApiV2ProxyResponse NotFound(string message = "Not found") => new()
    {
        StatusCode = 404,
        Body = JsonSerializer.Serialize(new { error = message }, JsonOptions),
        Headers = JsonHeaders
    };

    public static APIGatewayHttpApiV2ProxyResponse Unauthorized(string message = "Unauthorized") => new()
    {
        StatusCode = 401,
        Body = JsonSerializer.Serialize(new { error = message }, JsonOptions),
        Headers = JsonHeaders
    };

    public static APIGatewayHttpApiV2ProxyResponse ServerError(string message) => new()
    {
        StatusCode = 500,
        Body = JsonSerializer.Serialize(new { error = message }, JsonOptions),
        Headers = JsonHeaders
    };
}