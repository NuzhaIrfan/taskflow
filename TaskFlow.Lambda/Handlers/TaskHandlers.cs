using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskFlow.Data.Models;
using TaskFlow.Lambda.Helpers;
using TaskFlow.Lambda.Models;

namespace TaskFlow.Lambda.Handlers;

public static class TaskHandlers
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<APIGatewayHttpApiV2ProxyResponse> GetAll(
        APIGatewayHttpApiV2ProxyRequest request, int userId, ILambdaContext context)
    {
        using var db = DbContextFactory.Create();

        var tasks = await db.Tasks
            .Include(t => t.User)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsDone = t.IsDone,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UserEmail = t.User.Email
            })
            .ToListAsync();

        context.Logger.LogInformation($"User {userId} — returned {tasks.Count} tasks");
        return ApiResponse.Ok(tasks);
    }

    public static async Task<APIGatewayHttpApiV2ProxyResponse> GetById(
        int id, int userId, ILambdaContext context)
    {
        using var db = DbContextFactory.Create();

        var task = await db.Tasks
            .Include(t => t.User)
            .Where(t => t.Id == id && t.UserId == userId)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsDone = t.IsDone,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UserEmail = t.User.Email
            })
            .FirstOrDefaultAsync();

        if (task is null) return ApiResponse.NotFound($"Task {id} not found");

        return ApiResponse.Ok(task);
    }

    public static async Task<APIGatewayHttpApiV2ProxyResponse> Create(
        APIGatewayHttpApiV2ProxyRequest request, int userId, ILambdaContext context)
    {
        var body = JsonSerializer.Deserialize<CreateTaskRequest>(request.Body ?? "{}", JsonOptions);

        if (body is null || string.IsNullOrWhiteSpace(body.Title))
            return ApiResponse.BadRequest("Title is required");

        using var db = DbContextFactory.Create();

        var task = new TaskItem
        {
            UserId = userId,
            Title = body.Title,
            Description = body.Description,
            Priority = body.Priority,
            DueDate = body.DueDate
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        context.Logger.LogInformation($"Created task {task.Id} for user {userId}");
        return ApiResponse.Created(new { id = task.Id, title = task.Title });
    }

    public static async Task<APIGatewayHttpApiV2ProxyResponse> Update(
        int id, APIGatewayHttpApiV2ProxyRequest request, int userId, ILambdaContext context)
    {
        var body = JsonSerializer.Deserialize<UpdateTaskRequest>(request.Body ?? "{}", JsonOptions);
        if (body is null) return ApiResponse.BadRequest("Invalid body");

        using var db = DbContextFactory.Create();

        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task is null) return ApiResponse.NotFound($"Task {id} not found");

        if (body.Title is not null) task.Title = body.Title;
        if (body.Description is not null) task.Description = body.Description;
        if (body.IsDone.HasValue) task.IsDone = body.IsDone.Value;
        if (body.Priority.HasValue) task.Priority = body.Priority.Value;
        if (body.DueDate.HasValue) task.DueDate = body.DueDate.Value;

        await db.SaveChangesAsync();
        context.Logger.LogInformation($"Updated task {id}");

        return ApiResponse.Ok(new { id = task.Id, title = task.Title, isDone = task.IsDone });
    }

    public static async Task<APIGatewayHttpApiV2ProxyResponse> Delete(
        int id, int userId, ILambdaContext context)
    {
        using var db = DbContextFactory.Create();

        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task is null) return ApiResponse.NotFound($"Task {id} not found");

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();

        context.Logger.LogInformation($"Deleted task {id}");
        return ApiResponse.NoContent();
    }
}