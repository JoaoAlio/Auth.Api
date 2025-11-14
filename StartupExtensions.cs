using Auth.Api.Data;
using Auth.Api.Interfaces;
using Auth.Api.Repository;
using Auth.Api.Security.Tokens;
using Auth.Api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Auth.Api;

public static class StartupExtensions
{
    public static void AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }

    public static void AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        var key = config["Settings:Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado");
        var issuer = config["Settings:Jwt:Issuer"] ?? "your-issuer";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        });
    }

    public static void AddSwaggerService(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Auth Api",
                Version = "v1"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Jwt Authorization header using the bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\nExample: \"Bearer 12345abcdef\" "
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] { }
                }
            });
        });
    }

    public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var expirationTimeMinutes = uint.Parse(configuration["Settings:Jwt:ExpirationTimeMinutes"]);
        var signingkey = configuration["Settings:Jwt:Key"];

        services.AddScoped<IAcessTokenGenerator>(sp => new JwtTokenGenerator(expirationTimeMinutes, signingkey));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailValidator, EmailValidator>();
        services.AddScoped<IPasswordHash, PasswordHash>();
        services.AddScoped<IGoogleService, GoogleService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    }

    public static void AddGoogleAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var googleClientId = configuration["Settings:Google:ClientId"];
        var googleClientSecret = configuration["Settings:Google:ClientSecret"];

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie()
        .AddGoogle(googleOption =>
        {
            googleOption.ClientId = googleClientId;
            googleOption.ClientSecret = googleClientSecret;
        });
    }
}
