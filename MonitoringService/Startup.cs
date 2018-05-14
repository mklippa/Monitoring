using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitoringService.Repositories;
using MonitoringService.Services;
using StructureMap;
using StructureMap.Pipeline;

namespace MonitoringService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<MonitoringContext>(options => options.UseSqlite(Configuration["ConnectionString"]));
            services.Configure<MonitoringSettings>(Configuration);

            var container = new Container();

            container.Configure(config =>
            {
                config.AddRegistry(new StructuremapRegistry());
                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            ApplyDbMigrations(app.ApplicationServices);
        }

        private static void ApplyDbMigrations(IServiceProvider applicationServices)
        {
            using (var serviceScope = applicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var db = serviceScope.ServiceProvider.GetService<MonitoringContext>())
                {
                    db.Database.Migrate();
                }
            }
        }
    }

    public class StructuremapRegistry : Registry
    {
        public StructuremapRegistry()
        {
            For<IUnitOfWork>().Use<UnitOfWork>();
            For<IAgentStateService>().Use<AgentStateService>();
            For<IAggregationService>().Use<AggregationService>();
            For<IHostedService>().LifecycleIs(Lifecycles.Singleton).Use<ReportManagerService>();
        }
    }
}