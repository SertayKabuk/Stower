using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stower;
using System;
using WebApplication1.Application;
using WebApplication1.Domain;

namespace WebApplication1
{
    public class Startup
    {
        public IConfiguration _configuration { get; }
        public IWebHostEnvironment _environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _environment = env;
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
            .AddControllersAsServices();
            services.AddSwaggerGen();

            services.AddStower(options =>
            {
                options.AddStack<WeatherData>(Convert.ToInt32(_configuration["Stower:MaxStackLenght"]), Convert.ToInt32(_configuration["Stower:MaxWaitInSecond"]));
                options.AddStack<PositionData>(Convert.ToInt32(_configuration["Stower:MaxStackLenght"]), Convert.ToInt32(_configuration["Stower:MaxWaitInSecond"]));
            }, typeof(WeatherDataToppleHandler).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
