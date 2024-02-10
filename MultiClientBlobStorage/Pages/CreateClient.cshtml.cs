using MultiClientBlobStorage.FilesProvider.AzureStorageAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MultiClientBlobStorage.Pages;

[Authorize(Policy = "blob-two-write-policy")]
public class CreateClientModel : PageModel
{
    private readonly ClientBlobContainerProvider _clientBlobContainerProvider;

    [BindProperty]
    public string ClientName { get; set; } = string.Empty;

    public CreateClientModel(ClientBlobContainerProvider clientBlobContainerProvider)
    {
        _clientBlobContainerProvider = clientBlobContainerProvider;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            await _clientBlobContainerProvider.CreateClient(ClientName);  
        }

        return Page();
    }
}