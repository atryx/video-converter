using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VideoApp.Web.Database;
using VideoApp.Web.JobQueue;
using VideoApp.Web.Services;
using VideoApp.Web.TaskRunner;
using VideoApp.Web.Utilities;

namespace VideoApp
{
    public class Startup
    {
        readonly string AngularAppOrigin = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<VideoInformationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("VideoConverterDatabase"))
                , ServiceLifetime.Singleton);

            services.AddCors(options =>
            {
                options.AddPolicy(name: AngularAppOrigin,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:4200")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });


            services.AddScoped<IVideoConverterService, VideoConverterService>();
            services.AddSingleton<IFFmpegWraperService, FFmpegWraperService>();
            services.AddScoped<IFileManagerService, FileManagerService>();
            services.AddScoped<IJobRunnerQueue, JobRunnerQueue>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(AngularAppOrigin);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
