using System.Net;
using System.Text.Json;

namespace DDS_Tz_Helper.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var title = GetTitle(statusCode);

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            StatusCode = statusCode,
            Title = title,
            Message = GetMessage(exception, statusCode),
            TraceId = context.TraceIdentifier,
            Detail = _environment.IsDevelopment() ? exception.ToString() : null
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status404NotFound => "Not Found",
            _ => "Internal Server Error"
        };
    }

    private static string GetMessage(Exception exception, int statusCode)
    {
        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            return "An unexpected error occurred while processing your request.";
        }

        return string.IsNullOrWhiteSpace(exception.Message) ? GetTitle(statusCode) : exception.Message;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string TraceId { get; set; }
    public string Detail { get; set; }
}
