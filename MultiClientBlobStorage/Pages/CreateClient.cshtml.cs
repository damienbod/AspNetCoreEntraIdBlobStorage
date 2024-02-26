using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MultiClientBlobStorage.Providers;
using MultiClientBlobStorage.Providers.GroupUserServices;

namespace MultiClientBlobStorage.Pages;

[Authorize(Policy = "blob-admin-policy")]
public class CreateClientModel : PageModel
{
    private readonly ClientBlobContainerProvider _clientBlobContainerProvider;
    private readonly ApplicationMsGraphService _applicationMsGraphService;

    [BindProperty]
    public string ClientName { get; set; } = string.Empty;

    public CreateClientModel(ClientBlobContainerProvider clientBlobContainerProvider,
        ApplicationMsGraphService applicationMsGraphService)
    {
        _clientBlobContainerProvider = clientBlobContainerProvider;
        _applicationMsGraphService = applicationMsGraphService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var group = await _applicationMsGraphService.CreateSecurityGroupAsync(ClientName);

            var blobContainer = await _clientBlobContainerProvider
                .CreateBlobContainerClient(ClientName);

            if(blobContainer != null && group != null && group.Id != null)
            {
                await _clientBlobContainerProvider
                    .ApplyReaderGroupToBlobContainer(blobContainer, group.Id);
            }
        }

        return Page();
    }
}