using Common.Exceptions;
using Entities.DTOs.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GenandoAPI.Middlewares;

/// <summary>
/// Middleware for handling exceptions and returning appropriate JSON responses.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions and return JSON responses.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            ApiResponse errorResponse = HandleException(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorResponse.StatusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse,
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })
            );
        }
    }

    private static ApiResponse HandleException(Exception error)
    {
        Console.WriteLine(error.Message);
        Console.WriteLine(error.StackTrace);

        ApiResponse errorResponse = new()
        {
            Message = error.InnerException?.Message ?? error.Message,
            StatusCode = error switch
            {
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ModelValidationException => StatusCodes.Status400BadRequest,
                InvalidModelStateException => StatusCodes.Status400BadRequest,
                ForbiddenException => StatusCodes.Status403Forbidden,
                ResourceNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError,
            }
        };

        if (error is ModelValidationException e)
        {
            errorResponse.Errors = e.Errors;
        }
        return errorResponse;
    }
}