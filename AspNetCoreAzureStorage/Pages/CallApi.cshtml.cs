using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage.Pages
{
    public class CallApiModel : PageModel
    {
        private readonly AzureStorageService _azureStorageService;

        public JArray DataFromApi { get; set; }
        public CallApiModel(AzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public async Task OnGetAsync()
        {
            DataFromApi = await _azureStorageService.GetApiDataAsync();
        }
    }
}