using AspNetCoreAzureStorage.AzureStorageAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage.Pages
{
    public class AzStorageFilesModel : PageModel
    {
        private readonly AzureStorageService _azureStorageService;

        [BindProperty]
        public FileDescriptionShort FileDescriptionShort { get; set; }

        public AzStorageFilesModel(AzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public void OnGet()
        {
            FileDescriptionShort = new FileDescriptionShort
            {
                Description = "enter description"
            };
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