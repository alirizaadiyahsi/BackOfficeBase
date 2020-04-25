using System;
using System.Linq;
using System.Text;
using BackOfficeBase.Application;
using BackOfficeBase.Application.Email;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.AppConsts.Authorization;
using BackOfficeBase.Domain.AppConsts.Configuration;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Web.Core.ActionFilters;
using BackOfficeBase.Web.Core.Authorization;
using BackOfficeBase.Web.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

            Configuration.Bind(new JwtTokenConfiguration
            {
                Issuer = AppConfig.Authentication_JwtBearer_Issuer,
                Audience = AppConfig.Authentication_JwtBearer_Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(AppConfig.Authentication_JwtBearer_SecurityKey))
                    , SecurityAlgorithms.HmacSha256),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(60),
            });

            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddAuthorization(options =>
            {
                foreach (var permission in Permissions.GetAll())
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
