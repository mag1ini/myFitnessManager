using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyFitnessManager.Db;
using MyFitnessManager.Db.Repositories;

namespace MyFitnessManager
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var connectionString = Configuration["DbConnectionString"];
            services.AddDbContext<FitnessDbContext>
                    (builder => builder.UseSqlServer(connectionString));

            services.AddScoped<IHallRepository, HallRepository>();
            services.AddScoped<ICoachRepository,CoachRepository>();
            services.AddScoped<ITrainingRepository, TrainingRepository>();

            services.AddAutoMapper(GetType());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            EnsureDbCreated(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void EnsureDbCreated(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>().CreateScope();

            var context = serviceScope
                .ServiceProvider
                .GetRequiredService<FitnessDbContext>();

            context.Database.EnsureCreated();

        }
    }
}
