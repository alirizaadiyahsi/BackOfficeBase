using System;
using System.Linq;
using BackOfficeBase.Application.Email;
using BackOfficeBase.Domain.AppConsts.Configuration;
using BackOfficeBase.Web.Core.ActionFilters;
using BackOfficeBase.Web.Core.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

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
            var mvcBuilder = services.AddControllers(options =>
            {
                options.Filters.AddService<UnitOfWorkActionFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            LoadModules(mvcBuilder);

            services.Configure<EmailSettings>(Configuration.GetSection(AppConfig.Email_Smtp));
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<UnitOfWorkActionFilter>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
