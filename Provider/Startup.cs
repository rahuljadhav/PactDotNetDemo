using Microsoft.AspNetCore.Builder;
using Provider.Repositories;

namespace Provider
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
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddControllers()
                        .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();

            }
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseEndpoints(e => e.MapControllers());
        }
    }
}
