using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreAzureStorage.FilesProvider.AzureStorageAccess;
using AspNetCoreAzureStorage.FilesProvider.SqlDataAccess;
using AspNetCoreAzureStorage.FilesProvider.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace AspNetCoreAzureStorage.Pages
{
    [Authorize(Policy = "StorageBlobDataReaderPolicy")]
    [AuthorizeForScopes(Scopes = new string[] { "https://storage.azure.com/user_impersonation" })]
    public class ListFilesModel : PageModel
    {
        private readonly AzureStorageProvider _azureStorageService;
        private readonly FileDescriptionProvider _fileDescriptionProvider;

        [BindProperty]
        public IEnumerable<FileDescriptionDto> FileDescriptions { get; set; } = new List<FileDescriptionDto>();

        [BindProperty]
        public string? FileName { get; set; }

        public ListFilesModel(AzureStorageProvider azureStorageService,
            FileDescriptionProvider fileDescriptionProvider)
        {
            _azureStorageService = azureStorageService;
            _fileDescriptionProvider = fileDescriptionProvider;
        }

        public void OnGetAsync()
        {
            // should only return this dat if authz is good.
            FileDescriptions = _fileDescriptionProvider.GetAllFiles();
        }

        public async Task<ActionResult> OnGetDownloadFile(string fileName)
        {
            var file = await _azureStorageService.DownloadFile(fileName);

            return File(file.Value.Content, file.Value.ContentType, fileName);
        }

    }
}
