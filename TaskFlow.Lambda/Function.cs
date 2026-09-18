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
            // Public routes
            if (method == "GET" && path == "/health")
                return ApiResponse.Ok(new { status = "healthy", time = DateTime.UtcNow });

            if (method == "POST" && path == "/auth/register")
                return await AuthHandlers.Register(request, context);

            if (method == "POST" && path == "/auth/login")
                return await AuthHandlers.Login(request, context);

            // Protected routes — require JWT
            var userId = JwtHelper.ValidateTokenAndGetUserId(JwtHelper.GetAuthHeader(request));
            if (userId is null)
                return ApiResponse.Unauthorized("Missing or invalid token");

            var uid = userId.Value;

            if (method == "GET" && path == "/tasks")
                return await TaskHandlers.GetAll(request, uid, context);

            if (method == "POST" && path == "/tasks")
                return await TaskHandlers.Create(request, uid, context);

            if (path.StartsWith("/tasks/"))
            {
                var idPart = path.Substring("/tasks/".Length);
                if (!int.TryParse(idPart, out var id))
                    return ApiResponse.BadRequest("Invalid task ID");

                return method switch
                {
                    "GET" => await TaskHandlers.GetById(id, uid, context),
                    "PUT" => await TaskHandlers.Update(id, request, uid, context),
                    "DELETE" => await TaskHandlers.Delete(id, uid, context),
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