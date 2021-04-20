using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.MessageQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PersonService.DBContexts;
using PersonService.Repository;
using static Common.DependencyInjection.Utilities;
using Microsoft.OpenApi.Models;

namespace PersonService
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
            IServiceCollection servicesCollection = new ServiceCollection();
            var serviceClientSettingsConfig = Configuration.GetSection("RabbitMq");
            servicesCollection.AddScoped<IMessageQueue, RabbitMQHandler>(r => {
                return new RabbitMQHandler(serviceClientSettingsConfig.GetValue<string>("HostName"),
                                            serviceClientSettingsConfig.GetValue<int>("Port"),
                                            serviceClientSettingsConfig.GetValue<string>("UserName"),
                                            serviceClientSettingsConfig.GetValue<string>("Password"));
            });
            DependencyInjection.AddServices(servicesCollection);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Person Service", Version = "v1" });
            });
            services.AddDbContext<PersonServiceContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PersonServiceDB")));
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IEventRepository, EventRepository>();           
            services.AddHostedService<MainService>();
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
                c.SwaggerEndpoint("v1/swagger.json", "Person Service");
            });
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
