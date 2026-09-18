using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using TaskFlow.Lambda.Handlers;
using TaskFlow.Lambda.Helpers;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TaskFlow.Lambda;

public class Function
{
    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(
        APIGatewayHttpApiV2ProxyRequest request,
        ILambdaContext context)
    {
        var method = request.RequestContext?.Http?.Method?.ToUpper() ?? "GET";
        var path = request.RawPath ?? "/";

        context.Logger.LogInformation($"→ {method} {path}");

        try
        {
            // Route table
            if (method == "GET" && path == "/health")
                return ApiResponse.Ok(new { status = "healthy", time = DateTime.UtcNow });

            if (method == "GET" && path == "/tasks")
                return await TaskHandlers.GetAll(context);

            if (method == "POST" && path == "/tasks")
                return await TaskHandlers.Create(request, context);

            // Path parameters: /tasks/{id}
            if (path.StartsWith("/tasks/"))
            {
                var idPart = path.Substring("/tasks/".Length);
                if (!int.TryParse(idPart, out var id))
                    return ApiResponse.BadRequest("Invalid task ID");

                return method switch
                {
                    "GET" => await TaskHandlers.GetById(id, context),
                    "PUT" => await TaskHandlers.Update(id, request, context),
                    "DELETE" => await TaskHandlers.Delete(id, context),
                    _ => ApiResponse.BadRequest($"Method {method} not allowed")
                };
            }

            return ApiResponse.NotFound();
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Unhandled error: {ex.Message}");
            return ApiResponse.ServerError(ex.Message);
        }
    }
}