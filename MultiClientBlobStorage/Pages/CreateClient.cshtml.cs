using MultiClientBlobStorage.FilesProvider.AzureStorageAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MultiClientBlobStorage.Pages;

[Authorize(Policy = "blob-two-write-policy")]
public class CreateClientModel : PageModel
{
    private readonly BlobApplicationUploadProvider _blobUploadProvider;

    [BindProperty]
    public string ClientName { get; set; } = string.Empty;

    public CreateClientModel(BlobApplicationUploadProvider blobUploadProvider)
    {
        _blobUploadProvider = blobUploadProvider;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
           
        }


        return Page();
    }
}