using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreAzureStorage.FilesProvider.AzureStorageAccess;
using AspNetCoreAzureStorage.FilesProvider.SqlDataAccess;
using AspNetCoreAzureStorage.FilesProvider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace AspNetCoreAzureStorage.Pages
{
    [AuthorizeForScopes(Scopes = new string[] { "https://storage.azure.com/user_impersonation", "user.read","Directory.Read.All","User.ReadBasic.All" })]
    public class ListFilesModel : PageModel
    {
        private readonly AzureStorageProvider _azureStorageService;
        private readonly FileDescriptionProvider _fileDescriptionProvider;
        private readonly GraphApiClientService _graphApiClientService;

        [BindProperty]
        public IEnumerable<FileDescriptionDto> FileDescriptions { get; set; }

        [BindProperty]
        public string FileName { get; set; }

        public ListFilesModel(AzureStorageProvider azureStorageService,
            FileDescriptionProvider fileDescriptionProvider,
            GraphApiClientService graphApiClientService)
        {
            _azureStorageService = azureStorageService;
            _fileDescriptionProvider = fileDescriptionProvider;
            _graphApiClientService = graphApiClientService;
        }

        public async Task OnGetAsync()
        {
            await _graphApiClientService.GetGraphApiUser();
            FileDescriptions = _fileDescriptionProvider.GetAllFiles();
        }

        public async Task<ActionResult> OnGetDownloadFile(string fileName)
        {
            var file = await _azureStorageService.DownloadFile(fileName);

            return File(file.Value.Content, file.Value.ContentType, fileName);
        }

    }
}
