using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskFlow.Data;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TaskFlow.Lambda;

public class Function
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static DbContextOptions<AppDbContext> BuildDbOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=127.0.0.1,1433;Database=TaskFlow;User Id=taskflowuser;Password=Password123!;TrustServerCertificate=True")
            .Options;
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(
        APIGatewayHttpApiV2ProxyRequest request,
        ILambdaContext context)
    {
        context.Logger.LogInformation($"Path: {request.RawPath}, Method: {request.RequestContext.Http.Method}");

        try
        {
            var method = request.RequestContext.Http.Method.ToUpper();
            var path = request.RawPath ?? "/";

            if (method == "GET" && path == "/tasks")
            {
                return await GetTasks(context);
            }

            if (method == "GET" && path == "/health")
            {
                return Ok(new { status = "healthy", time = DateTime.UtcNow });
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error: {ex.Message}");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new { error = ex.Message }, JsonOptions),
                Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
            };
        }
    }

    private static async Task<APIGatewayHttpApiV2ProxyResponse> GetTasks(ILambdaContext context)
    {
        using var db = new AppDbContext(BuildDbOptions());

        var tasks = await db.Tasks
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.IsDone,
                t.Priority,
                t.DueDate,
                t.CreatedAt,
                UserEmail = t.User.Email
            })
            .ToListAsync();

        context.Logger.LogInformation($"Returned {tasks.Count} tasks");
        return Ok(tasks);
    }

    private static APIGatewayHttpApiV2ProxyResponse Ok(object body) => new()
    {
        StatusCode = 200,
        Body = JsonSerializer.Serialize(body, JsonOptions),
        Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
    };

    private static APIGatewayHttpApiV2ProxyResponse NotFound() => new()
    {
        StatusCode = 404,
        Body = JsonSerializer.Serialize(new { error = "Not found" }),
        Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
    };
}