using FitHub.Endpoints.ServiceConfigurations;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.IdentityModel.Tokens.Jwt;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("FitnessApi") // Your service name
            .AddAttributes(new Dictionary<string, object>
                {
                    { "deployment.environment", builder.Environment.EnvironmentName }
                }))
            .AddAspNetCoreInstrumentation() // Captures incoming API requests
            .AddHttpClientInstrumentation() // Captures outgoing HTTP requests
            .AddSqlClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options =>{
                options.SetDbStatementForText = true;
            })// Captures SQL Server queries
            .AddJaegerExporter(opts =>
            {
                opts.AgentHost = "jaeger"; // Change if Jaeger is hosted elsewhere
                opts.AgentPort = 6831;
            });
    });

var app = builder.Build();

app.UseCors("AllowFrontend");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
app.UseAuthorization();

app.MapControllers();

app.Run();
