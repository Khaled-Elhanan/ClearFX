using System.Text;
using ClearFX.API.Authorization;
using ClearFX.API.Middleware;
using ClearFX.Application.Behaviors;
using ClearFX.Application.Common;
using ClearFX.Application.Features.Auth.Commands;
using ClearFX.Domain.Enums;
using ClearFX.Domain.Interfaces;
using ClearFX.Infrastructure.Auth;
using ClearFX.Infrastructure.Persistence;
using ClearFX.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(15);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ClearFX API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.ApiKey,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            []
        }
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));
// validation pipleline
builder.Services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));




// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

// Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.CanDeposit,       p => p.RequireRole(
        nameof(UserRole.Admin), nameof(UserRole.Manager), nameof(UserRole.Teller)));

    options.AddPolicy(Policies.CanWithdraw,      p => p.RequireRole(
        nameof(UserRole.Admin), nameof(UserRole.Manager), nameof(UserRole.Teller)));

    options.AddPolicy(Policies.CanTransfer,      p => p.RequireRole(
        nameof(UserRole.Admin), nameof(UserRole.Manager), nameof(UserRole.Teller)));

    options.AddPolicy(Policies.CanExchange,      p => p.RequireRole(
        nameof(UserRole.Admin), nameof(UserRole.Manager), nameof(UserRole.Teller)));

    options.AddPolicy(Policies.CanManageRates,   p => p.RequireRole(
        nameof(UserRole.Admin), nameof(UserRole.Manager)));

    options.AddPolicy(Policies.CanViewAuditLogs, p => p.RequireRole(
        nameof(UserRole.Admin), nameof(UserRole.Manager), nameof(UserRole.Auditor)));

    options.AddPolicy(Policies.CanManageUsers,   p => p.RequireRole(
        nameof(UserRole.Admin)));
    
    options.AddPolicy("CanManageCustomers", p => p.RequireRole(
        nameof(UserRole.Admin), nameof(UserRole.Manager), nameof(UserRole.Teller)));
    
});

// Seed



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

// Middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
