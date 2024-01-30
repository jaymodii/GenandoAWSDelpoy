using BusinessAccessLayer.Abstraction;
using BusinessAccessLayer.Implementation;
using BusinessAccessLayer.Profiles;
using BusinessAccessLayer.Validators;
using Common.Constants;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using DataAccessLayer.Implementation;
using Entities.DTOs.Request;
using FluentValidation;
using GenandoAPI.ExtAuthorization;
using GenandoAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace GenandoAPI.Extensions;

public static class ApplicationConfiguration
{
    public static void ConnectDatabase(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString(SystemConstants.CONNECTION_STRING_NAME)!);
        });
    }

    public static void RegisterRepository(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClinicalDetailRepository, ClinicalDetailRepository>();
        services.AddScoped<IClinicalQuestionRepository, ClinicalQuestionRepository>();
        services.AddScoped<IClinicalProcessRepository, ClinicalProcessRepository>();
        services.AddScoped<ITestDetailRepository, TestDetailRepository>();
        services.AddScoped<ITestResultRepository, TestResultRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IFAQRepository, FAQRepository>();
        services.AddScoped<IRequestMoreInfoQuestionRepository, RequestMoreInfoQuestionRepository>();
        services.AddScoped<IClinicalProcessTestRepository, ClinicalProcessTestRepository>();
        services.AddScoped<IClinicalEnhancementAnsRepository, ClinicalEnhancementAnsRepository>();
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClinicalDetailService, ClinicalDetailService>();
        services.AddScoped<IClinicalQuestionService, ClinicalQuestionService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<IClinicalDetailService, ClinicalDetailService>();
        services.AddScoped<IClinicalProcessService, ClinicalProcessService>();
        services.AddScoped<IJwtManageService, JwtManageService>();
        services.AddScoped<ITestDetailService, TestDetailService>();
        services.AddScoped<ITestResultService, TestResultService>();
        services.AddScoped<IRequestMoreInfoQuestionService, RequestMoreInfoQuestionService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IFAQService, FAQService>();
        services.AddScoped<IClinicalEnhancementService, ClinicalEnhancementService>();
        services.AddScoped<IClinicalProcessTestService, ClinicalProcessTestService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISwipeActionSettingService, SwipeActionSettingService>();
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(SystemConstants.CorsPolicy,
                builder => builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true; // Enable detailed errors for debugging
        }).AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.PropertyNamingPolicy = null; // Customize JSON serialization if needed
        });
    }
    public static void SetRequestBodySize(this IServiceCollection services)
    {
        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = int.MaxValue;
        });
    }

    public static void RegisterFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginCredentialDtoValidationRule>(ServiceLifetime.Scoped);
        services.AddValidatorsFromAssemblyContaining<LoginOtpDtoValidationRule>(ServiceLifetime.Scoped);
        services.AddValidatorsFromAssemblyContaining<UserDetailsFormValidator>(ServiceLifetime.Scoped);
        services.AddScoped<ValidateModelAttribute>();
    }

    public static void RegisterAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfiles));
    }

    public static void RegisterMail(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MailSettingDto>(config.GetSection("MailSettings"));
        services.AddScoped<IMailService, MailService>();
    }

    public static void ConfigAuthentication(this IServiceCollection services, IConfiguration config)
    {

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Bearer Authentication with JWT Token",
                Type = SecuritySchemeType.Http
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                            }
                        },
                    new List < string > ()
                    }
                });
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                };
            });

        // custom auth filter
        services.AddScoped<ExtAuthorizeFilter>();


        // Register the ExtAuthorizeHandler before adding policies
        services.AddScoped<IAuthorizationHandler, ExtAuthorizeHandler>();

        services.AddAuthorization(config =>
        {
            config.AddPolicy(SystemConstants.DoctorPolicy, policy =>
            {
                policy.Requirements.Add(new ExtAuthorizeRequirement(SystemConstants.DoctorPolicy));
            });
            config.AddPolicy(SystemConstants.PatientPolicy, policy =>
            {
                policy.Requirements.Add(new ExtAuthorizeRequirement(SystemConstants.PatientPolicy));
            });
            config.AddPolicy(SystemConstants.LabUserPolicy, policy =>
            {
                policy.Requirements.Add(new ExtAuthorizeRequirement(SystemConstants.LabUserPolicy));
            });
            config.AddPolicy(SystemConstants.AllUserPolicy, policy =>
            {
                policy.Requirements.Add(new ExtAuthorizeRequirement(SystemConstants.AllUserPolicy));
            });
        });
    }
}
