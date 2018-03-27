using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Employee_Mg_Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Employee_Mg_Core
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
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession();
            //services.AddDbContext<EmployeeMgContext>(options =>options.UseSqlServer(Configuration.GetConnectionString("EmployeeMgContext")));
            // var connection = @"Server=(localdb)\mssqllocaldb;Database=EmployeeManagement;Trusted_Connection=True;;MultipleActiveResultSets=true";
            var connection = @"Data Source = (localdb)\mssqllocaldb; Initial Catalog = EmployeeManagement; Integrated Security = True";
            services.AddDbContext<EmployeeMgContext>(options => options.UseSqlServer(connection));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            // app.UseSession();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Registrations}/{action=LogIn}/{id?}");
            });
        }
    }
}
