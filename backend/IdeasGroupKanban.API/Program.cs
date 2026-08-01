using IdeasGroupKanban.Domain.Interfaces;
using IdeasGroupKanban.Infrastructure.Data;
using IdeasGroupKanban.Infrastructure.Repositories;
using IdeasGroupKanban.Application.Services;
using IdeasGroupKanban.Application.Reports;
using IdeasGroupKanban.Infrastructure.Services;
using IdeasGroupKanban.Infrastructure.Reports;
using IdeasGroupKanban.API.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure EF Core with PostgreSQL
// We use environment variables to override the connection string if provided (e.g. from docker-compose)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var envConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
if (!string.IsNullOrEmpty(envConnectionString))
{
    connectionString = envConnectionString;
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IColumnRepository, ColumnRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Register Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IColumnService, ColumnService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

// Reports
builder.Services.AddScoped<IReportFactory, ReportFactory>();
builder.Services.AddScoped<IReportStrategy, PdfReportStrategy>();
builder.Services.AddScoped<IReportStrategy, ExcelReportStrategy>();

builder.Services.AddSignalR()
    .AddJsonProtocol(options => 
    {
        options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Swagger OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
var secret = builder.Configuration["JwtSettings:Secret"];
var issuer = builder.Configuration["JwtSettings:Issuer"];
var audience = builder.Configuration["JwtSettings:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/kanban"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Setup CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular default port
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // needed for SignalR
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
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Auto-apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.MapControllers();
app.MapHub<KanbanHub>("/hubs/kanban");

app.Run();
