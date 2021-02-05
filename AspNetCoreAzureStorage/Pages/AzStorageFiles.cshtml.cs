using AspNetCoreAzureStorage.AzureStorageAccess;
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

        //[ServiceFilter(typeof(ValidateMimeMultipartContentFilter))]
        public async Task<IActionResult> OnPostAsync()
        {
            var names = new List<string>();
            var contentTypes = new List<string>();
            if (ModelState.IsValid)
            {
                foreach (var file in FileDescriptionShort.File)
                {
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Trim('"');
                        contentTypes.Add(file.ContentType);

                        names.Add(fileName);

                        await _azureStorageService.AddNewFile(new BlobFileUpload
                        {
                            Name = fileName,
                            Description = FileDescriptionShort.Description,
                            UploadedBy = HttpContext.User.Identity.Name
                        },
                        file); 
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

            // Add to Azure Table storage

            return Page();
        }

    }
}