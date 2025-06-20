using AdminPanel;
using AdminPanel.Constants;
using AdminPanel.Interfaces;
using AdminPanel.Models;
using AdminPanel.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Регистрация сервисов с интерфейсами
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("JwtSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRateService, RateService>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicies.AllowAll, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Сваггер (можно вынести в отдельный класс)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Admin Dashboard API", Version = "v1" });

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(CorsPolicies.AllowAll);
app.UseAuthentication();
app.UseAuthorization();

// Инициализация бд
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        logger.LogInformation("Устанавливаем миграции...");
        db.Database.Migrate();
        SeedData.Initialize(db);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Произошла ошибка во время инициализации бд");
        throw;
    }
}

// Аутентификация
app.MapPost("/auth/login", async ([FromBody] LoginRequestBody model, IAuthService authService) =>
{
    var result = await authService.LoginAsync(model);
    return result != null ? Results.Ok(result) : Results.Unauthorized();
});

app.MapPost("/auth/refresh", async ([FromBody] RefreshTokenRequestBody model, IAuthService authService) =>
{
    var result = await authService.RefreshTokenAsync(model.RefreshToken);
    return result != null ? Results.Ok(result) : Results.Unauthorized();
});

// CRUD клиентов
app.MapGet("/clients", async (IClientService clientService) =>
{
    return await clientService.GetAllClientsAsync();
}).RequireAuthorization();

app.MapGet("/clients/{id}", async (int id, IClientService clientService) =>
{
    var client = await clientService.GetClientByIdAsync(id);
    return client != null ? Results.Ok(client) : Results.NotFound();
}).RequireAuthorization();

app.MapPost("/clients", async ([FromBody] Client client, IClientService clientService) =>
{
    var createdClient = await clientService.CreateClientAsync(client);
    return Results.Created($"/clients/{createdClient.Id}", createdClient);
}).RequireAuthorization();

app.MapPut("/clients/{id}", async (int id, [FromBody] Client inputClient, IClientService clientService) =>
{
    var updatedClient = await clientService.UpdateClientAsync(id, inputClient);
    return updatedClient == null ? Results.NotFound() : Results.Ok(updatedClient);
}).RequireAuthorization();

app.MapDelete("/clients/{id}", async (int id, IClientService clientService) =>
{
    var success = await clientService.DeleteClientAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Платежи
app.MapGet("/payments", async (int take, IPaymentService paymentService) =>
{
    var payments = await paymentService.GetRecentPaymentsAsync(take);
    return Results.Ok(payments);
}).RequireAuthorization();

// Курс
app.MapGet("/rate", async (IRateService rateService) =>
{
    var rate = await rateService.GetCurrentRateAsync();
    return rate != null ? Results.Ok(rate) : Results.NotFound();
}).RequireAuthorization();

app.MapPost("/rate", async ([FromBody] Rate newRate, IRateService rateService) =>
{
    var rate = await rateService.UpdateRateAsync(newRate);
    return Results.Ok(rate);
}).RequireAuthorization();

// CRUD меток
app.MapGet("/tags", async (ITagService tagService) =>
{
    var tags = await tagService.GetAllTagsAsync();
    return Results.Ok(tags);
}).RequireAuthorization();

app.MapPost("/tags", async ([FromBody] Tag tag, ITagService tagService) =>
{
    var createdTag = await tagService.CreateTagAsync(tag);
    return Results.Created($"/tags/{createdTag.Id}", createdTag);
}).RequireAuthorization();

app.MapDelete("/tags/{id}", async (int id, ITagService tagService) =>
{
    var success = await tagService.DeleteTagAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Метки клиента
app.MapGet("/clients/{id}/tags", async (int id, IClientService clientService) =>
{
    var tags = await clientService.GetClientTagsAsync(id);
    return Results.Ok(tags);
}).RequireAuthorization();

app.MapPost("/clients/{id}/tags", async (int id, [FromBody] int tagId, IClientService clientService) =>
{
    var success = await clientService.AddTagToClientAsync(id, tagId);
    return success ? Results.Ok(success) : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/clients/{id}/tags/{tagId}", async (int id, int tagId, IClientService clientService) =>
{
    var success = await clientService.RemoveTagFromClientAsync(id, tagId);
    return success ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.Run("http://0.0.0.0:5000");