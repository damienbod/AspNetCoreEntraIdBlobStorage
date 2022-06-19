using AspNetCoreAzureStorage;
using AspNetCoreAzureStorage.FilesProvider.AzureStorageAccess;
using AspNetCoreAzureStorage.FilesProvider.SqlDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

services.AddTransient<AzureStorageProvider>();
services.AddTransient<LocalTokenAcquisitionTokenCredential>();
services.AddDbContext<FileContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
services.AddTransient<FileDescriptionProvider>();

services.AddTransient<AzureManagementFluentService>();

services.AddSingleton<IAuthorizationHandler, StorageBlobDataContributorRoleHandler>();
services.AddSingleton<IAuthorizationHandler, StorageBlobDataReaderRoleHandler>();

services.AddHttpClient();
services.AddOptions();

string[]? initialScopes = configuration.GetValue<string>("AzureStorage:ScopeForAccessToken")?.Split(' ');

services.AddMicrosoftIdentityWebAppAuthentication(configuration)
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



var app = builder.Build();

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

app.Run();
