using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DDS_Tz_Helper.Api.Middleware;

public class ApiRequestResponseLoggingMiddleware
{
    private const int MaxBodyLength = 8000;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ApiRequestResponseLoggingMiddleware> _logger;
    private readonly bool _includeBodies;

    public ApiRequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<ApiRequestResponseLoggingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _includeBodies = configuration.GetValue<bool>("ApiLogging:IncludeBodies");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!ShouldLog(context.Request))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestBody = _includeBodies ? await ReadRequestBodyAsync(context.Request) : string.Empty;
        var originalBody = context.Response.Body;

        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        _logger.LogInformation(
            "REQUEST {Method} {Path} Query={QueryString} BodyLogging={BodyLogging}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString.Value,
            GetBodyLoggingState(requestBody));

        try
        {
            await _next(context);
            stopwatch.Stop();

            var responseText = _includeBodies ? await ReadResponseBodyAsync(context.Response) : string.Empty;
            context.Response.Body = originalBody;
            await responseBody.CopyToAsync(originalBody);

            if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
            {
                _logger.LogError(
                    "FAILURE {Method} {Path} StatusCode={StatusCode} ElapsedMs={ElapsedMs} BodyLogging={BodyLogging}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    GetBodyLoggingState(responseText));
                return;
            }

            _logger.LogInformation(
                "RESPONSE {Method} {Path} StatusCode={StatusCode} ElapsedMs={ElapsedMs} BodyLogging={BodyLogging}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                GetBodyLoggingState(responseText));
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static bool ShouldLog(HttpRequest request)
    {
        return request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);
    }

    private string GetBodyLoggingState(string body)
    {
        if (!_includeBodies)
        {
            return "Disabled";
        }

        return string.IsNullOrWhiteSpace(body) ? "Empty" : body;
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength is null or 0 || !IsTextContent(request.ContentType))
        {
            return string.Empty;
        }

        request.EnableBuffering();

        using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return SanitizeAndTrim(body);
    }

    private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        if (!IsTextContent(response.ContentType))
        {
            return string.Empty;
        }

        response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(response.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return SanitizeAndTrim(body);
    }

    private static bool IsTextContent(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return true;
        }

        return contentType.Contains("json", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("text", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("xml", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("form", StringComparison.OrdinalIgnoreCase);
    }

    private static string SanitizeAndTrim(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;
        }

        var sanitized = TryRedactJson(body.Trim());
        return sanitized.Length <= MaxBodyLength ? sanitized : sanitized[..MaxBodyLength] + "...[truncated]";
    }

    private static string TryRedactJson(string body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            var redacted = RedactElement(document.RootElement);
            return JsonSerializer.Serialize(redacted, JsonOptions);
        }
        catch (JsonException)
        {
            return body;
        }
    }

    private static object RedactElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(
                property => property.Name,
                property => IsSensitive(property.Name) ? "***REDACTED***" : RedactElement(property.Value)),
            JsonValueKind.Array => element.EnumerateArray().Select(RedactElement).ToArray(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var number) ? number : element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }

    private static bool IsSensitive(string propertyName)
    {
        return propertyName.Contains("password", StringComparison.OrdinalIgnoreCase)
            || propertyName.Contains("token", StringComparison.OrdinalIgnoreCase)
            || propertyName.Contains("secret", StringComparison.OrdinalIgnoreCase);
    }
}
