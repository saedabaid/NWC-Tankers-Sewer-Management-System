using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.Hangfire.Jobs;

namespace NWC.Hangfire
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            services.AddControllers();

            services.AddSingleton<ILoggedInUserService, LoggedInUserService>();
            services.AddSingleton<IWorkOrderService, WorkOrderService>();
            services.AddSingleton<IWorkOrderVehicleService, WorkOrderVehicleService>();
            services.AddSingleton<Authentication>();
            services.AddSingleton<RecurringJobs>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Authentication authentication, RecurringJobs recurringJobs)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            var authenticateResp = authentication.AuthenticateUser();
            if (!authenticateResp.IsErrorState && authenticateResp.Value != null)
            {
                recurringJobs.Register();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
