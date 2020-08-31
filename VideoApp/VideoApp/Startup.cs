using System;
using AutoMapper;
using FFmpegUtilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VideoApp.Web.Database;
using VideoApp.Web.Services;
using VideoApp.Web.TaskRunner;

namespace VideoApp
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
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<VideoInformationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("VideoConverterDatabase")),
                ServiceLifetime.Singleton);
            

            services.AddScoped<IVideoConverterService, VideoConverterService>();
            //services.AddScoped<IJobRunnerQueue, JobRunnerQueue>();
            //services.AddScoped<ICommandExecuter, CommandExecuter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
