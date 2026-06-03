using DDS_Tz_Helper.Api.Middleware;
using DDS_Tz_Helper.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var corsPolicyName = "ConfiguredCorsPolicy";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var allowCredentials = builder.Configuration.GetValue<bool>("Cors:AllowCredentials");
var logDirectory = builder.Configuration.GetValue<string>("Serilog:LogDirectory") ?? @"C:\DDS_Logs";

Directory.CreateDirectory(logDirectory);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(item => item.Value?.Errors.Count > 0)
                .SelectMany(item => item.Value!.Errors.Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid request value." : error.ErrorMessage))
                .ToArray();

            return new BadRequestObjectResult(new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Message = errors.Length == 0 ? "The request is invalid." : string.Join(" ", errors),
                TraceId = context.HttpContext.TraceIdentifier
            });
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod();

        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }

        if (allowCredentials)
        {
            policy.AllowCredentials();
        }
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
    {
        await next();
    }
});
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} -> {StatusCode} ({Elapsed:0.000} ms)";
    options.GetLevel = (context, _, exception) =>
    {
        if (exception is not null || context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            return LogEventLevel.Error;
        }

        if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
        {
            return LogEventLevel.Warning;
        }

        return LogEventLevel.Information;
    };
});
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);
app.UseAuthorization();
app.MapControllers();
app.Run();
