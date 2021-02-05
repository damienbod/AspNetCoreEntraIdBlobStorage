using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage.Pages
{
    public class AzStorageFilesModel : PageModel
    {
        private readonly AzureStorageService _azureStorageService;

        public JArray DataFromApi { get; set; }
        public AzStorageFilesModel(AzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public async Task OnGetAsync()
        {
            DataFromApi = await _azureStorageService.GetApiDataAsync();
        }
    }
}