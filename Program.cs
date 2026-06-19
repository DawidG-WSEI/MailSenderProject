using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MailSender.Configuration;
using MailSender.Application.Interfaces;
using MailSender.Application.Services;
using MailSender.Infrastructure.MailProviders;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<MessageFormatter>();
builder.Services.AddHttpClient<IMailSenderProvider, MailtrapMailProvider>();

builder.Services.AddControllers();
// KOD WYGENEROWANY BY WALCZYĆ Z CORSEM wygenerowany na podstawie zdjęcia (model GPT Codex 5.2)

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDevCors", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:5500",
                "http://localhost:8080",
                "http://127.0.0.1:3000",
                "http://127.0.0.1:5173",
                "http://127.0.0.1:5500",
                "http://127.0.0.1:8080"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// KONIEC KODU GENEROWANEGO PRZECIW CORSOWI
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var secretKey = jwtSettings!.SecretKey;

// kod z -> https://medium.com/@solomongetachew112/jwt-authentication-in-net-8-a-complete-guide-for-secure-and-scalable-applications-6281e5e8667c
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // walidacja tokenów
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,       // Sprawdza czy token nie wygasł
        ValidateIssuerSigningKey = true, // Sprawdza czy podpis się zgadza
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        // Konwersja hasła na bajty wymagane przez algorytm
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});


// kod z -> https://stackoverflow.com/questions/58179180/jwt-authentication-and-swagger-with-net-core-3-0 
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "MailSender API", Version = "v1" });

    // Token przekazywany w nagłówku HTTP (Header)
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter a valid token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Wymuszamy, aby Swagger dołączał ten token do każdego żądania
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>() // pusta list scope'ów, bo tak trzeba
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Kod przeciw CORS
app.UseCors("LocalDevCors");
// Koniec kodu przeciw CORS

// UWAGA: Kolejność tych linii jest BARDZO WAŻNA!
app.UseAuthentication(); // Sprawdza kim jesteś (Authentication)...
app.UseAuthorization(); // ...czy masz prawo tu wejść (Authorization).

app.MapControllers();

app.Run();