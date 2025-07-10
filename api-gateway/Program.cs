using System.Reflection;
using System.Text;
using ApiGateway.Middleware;
using ApiGateway.SwaggerSchemas;
using DataTransferLib;
using DataTransferLib.CacheServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerGen;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// === CORS ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(
                "https://testapp.24cases.ru",
                "https://24cases.ru",
                "http://localhost:3000",
                "https://front.24cases.ru",
                "https://stardrop.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// === DI ===
builder.Services.AddCommunicationServices();
builder.Services.AddSingleton<RabbitMqCacheService>();

// === Controllers + JSON ===
builder.Services.AddControllers().AddNewtonsoftJson();

// === JWT ===
var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? string.Empty);
var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// === Token lifespan ===
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});

// === Swagger ===
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EnumSchemaFilter>();

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api Stardrop",
        Version = "v1",
        Description = "API использует `HttpOnly` куки для авторизации. Пожалуйста, убедитесь, что вы залогинились, чтобы получить необходимые куки перед использованием защищенных методов."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    c.DocInclusionPredicate((docName, apiDesc) => true);

    c.TagActionsBy(apiDesc =>
    {
        if (apiDesc.TryGetMethodInfo(out var methodInfo))
        {
            var groupName = methodInfo.GetCustomAttributes(true)
                .OfType<ApiExplorerSettingsAttribute>()
                .FirstOrDefault()?.GroupName;
            if (!string.IsNullOrEmpty(groupName))
                return new[] { groupName };
        }

        return new[] { "Default" };
    });
});

// === OpenAPI for Scalar ===
builder.Services.AddOpenApi();

var app = builder.Build();

// === Middleware ===
app.UseMiddleware<JwtFromCookieMiddleware>();

// === Swagger UI ===
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Stardrop");
    options.RoutePrefix = "swagger";
});

// === Scalar UI ===
app.MapOpenApi(); // доступно по /openapi/v1.json
app.MapScalarApiReference(options =>
{
    options
        .WithTheme(ScalarTheme.Kepler)
        .WithDarkModeToggle(true)
        .WithClientButton(true)
        .WithOpenApiRoutePattern("/swagger/{documentName}.json")
        .AddPreferredSecuritySchemes("Bearer");
});

// === HTTP pipeline ===
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMetricServer();

app.Run();
