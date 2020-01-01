using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SitecoreHub
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
            // This is a brand new feature as of 12/30/2019
            // This blog post is helpful: https://edi.wang/post/2019/12/5/try-the-new-azure-net-sdk
            // The SDK is here:: https://github.com/Azure/azure-sdk-for-net

            services.AddAzureClients(builder =>
            {
                services.AddAzureClients(builder => builder.AddBlobServiceClient(Configuration.GetValue<string>("DefaultConnectionString")));
            });
            services.AddServerSideBlazor();
            services.AddRazorPages();
            services.AddHttpClient();
            services.Scan(scan => scan.FromCallingAssembly().AddClasses().AsMatchingInterface());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });        }
    }
}