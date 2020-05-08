using System;
using System.Linq;
using System.Text;
using BackOfficeBase.Application;
using BackOfficeBase.Application.Email;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.AppConstants.Configuration;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Web.Core.ActionFilters;
using BackOfficeBase.Web.Core.Authorization;
using BackOfficeBase.Web.Core.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
    // TODO: Make this project open-source when .net 5 is released!
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

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("JwtTokenBasedAuthExample_8CFB2EC534E14D56"));
            var jwtTokenConfiguration = new JwtTokenConfiguration
            {
                Issuer = "JwtTokenBasedAuthExample",
                Audience = "http://localhost:44341",
                SigningCredentials = new SigningCredentials(
                    signingKey
                    , SecurityAlgorithms.HmacSha256),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(60),
            };
            Configuration.Bind(jwtTokenConfiguration);

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
                options.AddPolicy(AppConfig.App_CorsOriginPolicyName,
                    builder =>
                        builder.WithOrigins(AppConfig.App_CorsOrigins.Split(",", StringSplitOptions.RemoveEmptyEntries))
                            .AllowAnyHeader()
                            .AllowAnyMethod());
            });

            services.AddScoped<UnitOfWorkActionFilter>();
            var mvcBuilder = services.AddControllers(options =>
            {
                options.Filters.AddService<UnitOfWorkActionFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

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

            LoadModules(mvcBuilder);
            services.Configure<EmailSettings>(Configuration.GetSection(AppConfig.Email_Smtp));
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddHttpContextAccessor();
            services.ConfigureApplicationService();
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

            app.UseCors(AppConfig.App_CorsOriginPolicyName);

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
