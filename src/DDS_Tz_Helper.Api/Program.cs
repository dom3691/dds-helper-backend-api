using DDS_Tz_Helper.Api.Middleware;
using DDS_Tz_Helper.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

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
    options.AddPolicy("AngularDevClient", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:4200", "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularDevClient");
app.UseAuthorization();
app.MapControllers();
app.Run();
