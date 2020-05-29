using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BackOfficeBase.Application;
using BackOfficeBase.Application.Email;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.AppConstants.Configuration;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Domain.Localization.Resources;
using BackOfficeBase.Web.Core.ActionFilters;
using BackOfficeBase.Web.Core.Authorization;
using BackOfficeBase.Web.Core.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;

namespace BackOfficeBase.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BackOfficeBaseDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(AppConfig.DefaultConnection))
                    .UseLazyLoadingProxies());

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<BackOfficeBaseDbContext>()
                .AddDefaultTokenProviders();

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration[AppConfig.Authentication_JwtBearer_SecurityKey]));
            var jwtTokenConfiguration = new JwtTokenConfiguration
            {
                Issuer = Configuration[AppConfig.Authentication_JwtBearer_Issuer],
                Audience = Configuration[AppConfig.Authentication_JwtBearer_Audience],
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(60),
            };

            services.Configure<JwtTokenConfiguration>(config =>
            {
                config.Audience = jwtTokenConfiguration.Audience;
                config.EndDate = jwtTokenConfiguration.EndDate;
                config.Issuer = jwtTokenConfiguration.Issuer;
                config.StartDate = jwtTokenConfiguration.StartDate;
                config.SigningCredentials = jwtTokenConfiguration.SigningCredentials;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtTokenConfiguration.Issuer,
                    ValidAudience = jwtTokenConfiguration.Audience,
                    IssuerSigningKey = signingKey
                };
            });

            services.Configure<EmailSettings>(Configuration.GetSection(AppConfig.Email_Smtp));
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddHttpContextAccessor();
            services.ConfigureApplicationService();

            AddAuthorizationConfiguration(services);
            AddMvcConfiguration(services);
            AddSwaggerConfiguration(services);
            AddLocalizationConfiguration(services);
        }

        private void AddAuthorizationConfiguration(IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddAuthorization(options =>
            {
                foreach (var permission in AppPermissions.GetAll())
                {
                    options.AddPolicy(permission,
                        policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });

            services.AddCors(options =>
            {
                options.AddPolicy(Configuration[AppConfig.App_CorsOriginPolicyName],
                    builder =>
                        builder.WithOrigins(Configuration[AppConfig.App_CorsOrigins]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries))
                            .AllowAnyHeader()
                            .AllowAnyMethod());
            });
        }

        private static void AddMvcConfiguration(IServiceCollection services)
        {
            services.AddScoped<UnitOfWorkActionFilter>();
            var mvcBuilder = services.AddControllers(options =>
                {
                    options.Filters.AddService<UnitOfWorkActionFilter>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider =
                        (type, factory) => factory.Create(typeof(Translations));
                });
            LoadModules(mvcBuilder);
        }

        private static void AddSwaggerConfiguration(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiBestPractices", Version = "v1" });
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }

        private static void AddLocalizationConfiguration(IServiceCollection services)
        {
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                CultureInfo[] supportedCultures =
                {
                    new CultureInfo("en"),
                    new CultureInfo("tr")
                };

                options.DefaultRequestCulture = new RequestCulture("tr");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider(),
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Back Office Base Application API V1");
            });

            app.UseCors(Configuration[AppConfig.App_CorsOriginPolicyName]);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // TODO: This is the easiest way to load assemblies. But this can be more generic.
        // NOTE: Module projects (DLLs) should be referenced manually. 
        private static void LoadModules(IMvcBuilder mvcBuilder)
        {
            var moduleAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name.Contains("BackOfficeBase.Modules"));

            foreach (var moduleAssembly in moduleAssemblies)
            {
                mvcBuilder.AddApplicationPart(moduleAssembly);
            }
        }
    }
}
