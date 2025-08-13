using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthService.Application;
using AuthService.Persistence;
using AuthService.WebApi.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var rsa = RSA.Create(2048);
var privateKey = new RsaSecurityKey(rsa);
var publicKey = new RsaSecurityKey(rsa.ExportParameters(false));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена в конфигурации.");
}
DbInitializer.Initialize(connectionString);
builder.Services.AddDb(connectionString);
builder.Services.AddApplication(privateKey, publicKey);
builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services
    .AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "AuthService",
            ValidateAudience = true,
            ValidAudience = "ApiService",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = publicKey // Используем публичный ключ
        };
    });
    
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AuthCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AuthCorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();