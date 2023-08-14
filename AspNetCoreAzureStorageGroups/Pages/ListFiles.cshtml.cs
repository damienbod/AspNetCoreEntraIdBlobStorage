using AspNetCoreAzureStorageGroups.FilesProvider.AzureStorageAccess;
using AspNetCoreAzureStorageGroups.FilesProvider.SqlDataAccess;
using AspNetCoreAzureStorageGroups.FilesProvider.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using static System.Formats.Asn1.AsnWriter;

namespace AspNetCoreAzureStorageGroups.Pages;

[Authorize(Policy = "StorageBlobDataReaderPolicy")]
[AuthorizeForScopes(Scopes = new string[] { "https://storage.azure.com/user_impersonation" })]
public class ListFilesModel : PageModel
{
    private readonly AzureStorageProvider _azureStorageService;
    private readonly FileDescriptionProvider _fileDescriptionProvider;
    private readonly ITokenAcquisition _tokenAcquisition;

    [BindProperty]
    public IEnumerable<FileDescriptionDto> FileDescriptions { get; set; } 
        = new List<FileDescriptionDto>();

    [BindProperty]
    public string? FileName { get; set; }

    public ListFilesModel(AzureStorageProvider azureStorageService,
        ITokenAcquisition tokenAcquisition,
        FileDescriptionProvider fileDescriptionProvider)
    {
        _azureStorageService = azureStorageService;
        _fileDescriptionProvider = fileDescriptionProvider;
        _tokenAcquisition = tokenAcquisition;
    }

    public async Task OnGetAsync()
    {
        // gets an access token
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
