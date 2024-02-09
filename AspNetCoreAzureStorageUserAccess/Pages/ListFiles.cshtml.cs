using AspNetCoreAzureStorageUserAccess.FilesProvider.AzureStorageAccess;
using AspNetCoreAzureStorageUserAccess.FilesProvider.SqlDataAccess;
using AspNetCoreAzureStorageUserAccess.FilesProvider.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace AspNetCoreAzureStorageUserAccess.Pages;

[Authorize(Policy = "blob-one-read-policy")]
[AuthorizeForScopes(Scopes = ["https://storage.azure.com/user_impersonation"])]
public class ListFilesModel : PageModel
{
    private readonly AzureBlobStorageProvider _azureStorageService;
    private readonly FileDescriptionProvider _fileDescriptionProvider;
    private readonly ITokenAcquisition _tokenAcquisition;

    [BindProperty]
    public IEnumerable<FileDescriptionDto> FileDescriptions { get; set; } 
        = new List<FileDescriptionDto>();

    [BindProperty]
    public string? FileName { get; set; }

    public ListFilesModel(AzureBlobStorageProvider azureStorageService,
        ITokenAcquisition tokenAcquisition,
        FileDescriptionProvider fileDescriptionProvider)
    {
        _azureStorageService = azureStorageService;
        _fileDescriptionProvider = fileDescriptionProvider;
        _tokenAcquisition = tokenAcquisition;
    }

    public async Task OnGetAsync()
    {
        // gets a user access token
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://storage.azure.com/user_impersonation" });
        // should only return this dat if authz is good.
        FileDescriptions = _fileDescriptionProvider.GetAllFiles();
    }

    public async Task<ActionResult> OnGetDownloadFile(string fileName)
    {
        var file = await _azureStorageService.DownloadFile(fileName);

        return File(file.Value.Content, file.Value.ContentType, fileName);
    }
}