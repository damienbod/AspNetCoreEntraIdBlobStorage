using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MultiClientBlobStorage.Providers;

namespace MultiClientBlobStorage.Pages;

[Authorize(Policy = "blob-admin-policy")]
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
            var blobContainer = await _clientBlobContainerProvider
                .CreateBlobContainerClient(ClientName);

            if(blobContainer != null)
            {
                // 1. Create new security group for client users
                var groupBlobOneRead = "efa3647e-f334-4cab-8c0e-87b042fc9d30";

                await _clientBlobContainerProvider.ApplyReaderGroupToBlobContainer(blobContainer, 
                    groupBlobOneRead);
            }
        }

        return Page();
    }
}