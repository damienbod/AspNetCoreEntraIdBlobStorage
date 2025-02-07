using DelegatedEntraIDBlobStorage.FilesProvider.AzureStorageAccess;
using DelegatedEntraIDBlobStorage.FilesProvider.SqlDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;

services.AddScoped<BlobDelegatedUploadProvider>();
services.AddScoped<BlobDelegatedDownloadProvider>();
services.AddTransient<DelegatedTokenAcquisitionTokenCredential>();

services.AddDbContext<FileContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
services.AddTransient<FileDescriptionProvider>();

services.AddHttpClient();
services.AddOptions();

string[]? initialScopes = configuration.GetValue<string>("AzureStorage:ScopeForAccessToken")?.Split(' ');

builder.Services.AddDistributedMemoryCache();

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "EntraID",
        subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true)
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddDistributedTokenCaches();


services.AddAuthorization(options =>
{
    options.AddPolicy("blob-one-read-policy", policyBlobOneRead =>
    {
        policyBlobOneRead.RequireClaim("roles", ["blobonereadrole", "blobonewriterole"]);
    });
    options.AddPolicy("blob-one-write-policy", policyBlobOneRead =>
    {
        policyBlobOneRead.RequireClaim("roles", ["blobonewriterole"]);
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

IdentityModelEventSource.ShowPII = true;
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

if (app.Environment.IsDevelopment())
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

app.MapRazorPages();
app.MapControllers();

app.Run();
