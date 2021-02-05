using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage.Pages
{
    public class AzStorageFilesModel : PageModel
    {
        private readonly AzureStorageService _azureStorageService;

        public string DataFromApi { get; set; }

        [BindProperty]
        public FileDescriptionShort FileDescriptionShort { get; set; }
        public AzStorageFilesModel(AzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public async Task OnGetAsync()
        {
            FileDescriptionShort = new FileDescriptionShort
            {
                Description = "enter description"
            };
            //DataFromApi = await _azureStorageService.AddNewFile();
        }

        //[HttpPost]
        //[ServiceFilter(typeof(ValidateMimeMultipartContentFilter))]
        public async Task<IActionResult> OnPostAsync()
        {
            var names = new List<string>();
            var contentTypes = new List<string>();
            if (ModelState.IsValid)
            {
                // http://www.mikesdotnetting.com/article/288/asp-net-5-uploading-files-with-asp-net-mvc-6
                // http://dotnetthoughts.net/file-upload-in-asp-net-5-and-mvc-6/
                foreach (var file in FileDescriptionShort.File)
                {
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Trim('"');
                        contentTypes.Add(file.ContentType);

                        names.Add(fileName);

                        // await file.SaveAsAsync(Path.Combine(_optionsApplicationConfiguration.Value.ServerUploadFolder, fileName));
                    }
                }
            }

            var files = new FileResult
            {
                FileNames = names,
                ContentTypes = contentTypes,
                Description = FileDescriptionShort.Description,
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
            };

            // Add to Azure storage

            return Page();
        }

    }

    public class FileResult
    {
        public List<string> FileNames { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
        public List<string> ContentTypes { get; set; }
    }

    public class FileDescriptionShort
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public ICollection<IFormFile> File { get; set; }
    }
}