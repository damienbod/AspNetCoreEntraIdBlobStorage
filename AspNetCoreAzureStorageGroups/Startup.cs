using AspNetCoreAzureStorageGroups.FilesProvider.AzureStorageAccess;
using AspNetCoreAzureStorageGroups.FilesProvider.SqlDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace AspNetCoreAzureStorageGroups
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
            services.AddScoped<AzureStorageProvider>();
            services.AddTransient<LocalTokenAcquisitionTokenCredential>();
            services.AddDbContext<FileContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<FileDescriptionProvider>();


            services.AddSingleton<IAuthorizationHandler, StorageBlobDataContributorRoleHandler>();
            services.AddSingleton<IAuthorizationHandler, StorageBlobDataReaderRoleHandler>();

            services.AddHttpClient();
            services.AddOptions();

            string[] initialScopes = Configuration.GetValue<string>("AzureStorage:ScopeForAccessToken")?.Split(' ');

            services.AddMicrosoftIdentityWebAppAuthentication(Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                .AddInMemoryTokenCaches();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("StorageBlobDataContributorPolicy", policyIsAdminRequirement =>
                {
                    policyIsAdminRequirement.Requirements.Add(new StorageBlobDataContributorRoleRequirement());
                });
                options.AddPolicy("StorageBlobDataReaderPolicy", policyIsAdminRequirement =>
                {
                    policyIsAdminRequirement.Requirements.Add(new StorageBlobDataReaderRoleRequirement());
                });
            });

            services.AddRazorPages().AddMvcOptions(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
