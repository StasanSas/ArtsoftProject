using System.Reflection;
using AuthService.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Application;
using NotificationService.WebApi.Clients;
using NotificationService.WebApi.Middlewares;
using TaskService.WebApi.Clients;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient(); 
builder.Services.AddCustomClients();
builder.Services.AddSingleton<JwtPublicKeyService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
DbInitializer.Initialize(connectionString);
builder.Services.AddDb(connectionString);

builder.Services.AddApplication();
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
            ValidateIssuerSigningKey = true
            // IssuerSigningKey позже создадим т.к. нужен сервис
        };
    });


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var publicKeyService = scope.ServiceProvider.GetRequiredService<JwtPublicKeyService>();
    var jwtBearerOptions = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
        .Get(JwtBearerDefaults.AuthenticationScheme);
    // IssuerSigningKey инициализирован (если всё прошло успешно)
    jwtBearerOptions.TokenValidationParameters.IssuerSigningKey = await publicKeyService.GetPublicKey();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();
app.UseRouting();
app.UseHttpsRedirection();
//app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();