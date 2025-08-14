using System.Reflection;
using AuthService.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Application;
using NotificationService.WebApi;
using NotificationService.WebApi.Clients;
using NotificationService.WebApi.Hubs;
using NotificationService.WebApi.Middlewares;
using Serilog;
using TaskService.WebApi.Clients;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpClient(); 
builder.Services.AddCustomClients();
builder.Services.AddSingleton<JwtPublicKeyService>();

builder.Services.AddSignalR();
builder.Services.AddWebSocketServices();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("������ ����������� 'DefaultConnection' �� ������� � ������������.");
}
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
            // IssuerSigningKey ����� �������� �.�. ����� ������
        };
    });


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

var app = builder.Build();
app.UseCors("CorsPolicy");
using (var scope = app.Services.CreateScope())
{
    var publicKeyService = scope.ServiceProvider.GetRequiredService<JwtPublicKeyService>();
    var jwtBearerOptions = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
        .Get(JwtBearerDefaults.AuthenticationScheme);
    // IssuerSigningKey ��������������� (���� �� ������ �������)
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
app.MapHub<NotificationHub>("/notificationHub");
app.Run();