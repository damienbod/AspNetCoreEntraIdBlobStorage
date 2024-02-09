using DelegatedReadAppWriteBlobStorage.FilesProvider.AzureStorageAccess;
using DelegatedReadAppWriteBlobStorage.FilesProvider.SqlDataAccess;
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
var env = builder.Environment;

services.AddScoped<BlobApplicationUploadProvider>();
services.AddScoped<BlobDelegatedDownloadProvider>();
services.AddSingleton<ClientSecretCredentialProvider>();

services.AddTransient<DelegatedTokenAcquisitionTokenCredential>();

services.AddDbContext<FileContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
services.AddTransient<FileDescriptionProvider>();

services.AddHttpClient();
services.AddOptions();

string[]? initialScopes = configuration.GetValue<string>("AzureStorage:ScopeForAccessToken")?.Split(' ');

services.AddMicrosoftIdentityWebAppAuthentication(configuration)
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddInMemoryTokenCaches();

services.AddAuthorization(options =>
{
    options.AddPolicy("blob-two-read-policy", policyBlobOneRead =>
    {
        policyBlobOneRead.RequireClaim("roles", ["blobtworeadrole", "blobtwowriterole"]);
    });
    options.AddPolicy("blob-two-write-policy", policyBlobOneRead =>
    {
        policyBlobOneRead.RequireClaim("roles", ["blobtwowriterole"]);
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

app.MapRazorPages();
app.MapControllers();

app.Run();
