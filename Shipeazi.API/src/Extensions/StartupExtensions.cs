using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Orchestrix.Mediator;
using Shipeazi.Application.src.Commands;
using Shipeazi.Application.src.Repositories;
using Shipeazi.Application.src.Services;
using Shipeazi.Infrastructure.src.Identity;
using Shipeazi.Infrastructure.src.Persistence;
using Shipeazi.Infrastructure.src.Persistence.Repositories;
using Shipeazi.Infrastructure.src.Services;

namespace Shipeazi.API.src.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection RegisterStartupExtensions(this IServiceCollection services, IConfiguration config)
        {
            services.AddDatabaseContext(config);
            services.AddRepositories();
            services.AddIdentityServices(config);
            services.AddJwtAuthentication(config);
            services.AddApiVersioningConfig();
            services.AddThirdPartyServices();

            return services;
        }

        // add repositories
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IOtpVerificationRepository, OtpVerificationRepository>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IHttpContextService, HttpContextService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddHttpContextAccessor();
            return services;
        }

        // add database context
        private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }

        // add api versoning config
        private static IServiceCollection AddApiVersioningConfig(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            return services;
        }

        private static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        private static IServiceCollection AddThirdPartyServices(this IServiceCollection services)
        {
            services.AddOrchestrix(cfg =>
            {
               // Register handlers from the Application assembly where the command handlers are
               cfg.RegisterHandlersFromAssemblies(
                   typeof(Program).Assembly,
                   typeof(AuthenticateCommand).Assembly
               ); 
            });

            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            });

            return services;
        }
    }
}