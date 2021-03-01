using AspNetCoreAzureStorageGroups.FilesProvider.AzureStorageAccess;
using AspNetCoreAzureStorageGroups.FilesProvider.SqlDataAccess;
using AspNetCoreAzureStorageGroups.FilesProvider.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorageGroups.Pages
{
    [Authorize(Policy = "StorageBlobDataContributorPolicy")]
    [AuthorizeForScopes(Scopes = new string[] { "https://storage.azure.com/user_impersonation" })]
    public class AzStorageFilesModel : PageModel
    {
        private readonly AzureStorageProvider _azureStorageService;
        private readonly FileDescriptionProvider _fileDescriptionProvider;

        [BindProperty]
        public FileDescriptionUpload FileDescriptionShort { get; set; }

        public AzStorageFilesModel(AzureStorageProvider azureStorageService,
            FileDescriptionProvider fileDescriptionProvider)
        {
            _azureStorageService = azureStorageService;
            _fileDescriptionProvider = fileDescriptionProvider;
        }

        public void OnGet()
        {
            FileDescriptionShort = new FileDescriptionUpload
            {
                Description = "enter description"
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var fileInfos = new List<(string FileName, string ContentType)>();
            if (ModelState.IsValid)
            {
                if (!IsMultipartContentType(HttpContext.Request.ContentType))
                {
                    ModelState.AddModelError("FileDescriptionShort.File", "not a MultipartContentType");
                    return Page();
                }

                foreach (var file in FileDescriptionShort.File)
                {
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Trim('"');

                        fileInfos.Add((fileName, file.ContentType));

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

            var files = new UploadedFileResult
            {
                FileInfos = fileInfos,
                Description = FileDescriptionShort.Description,
                UploadedBy = HttpContext.User.Identity.Name,
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
            };

            await _fileDescriptionProvider.AddFileDescriptionsAsync(files);

            return Page();
        }


        private static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}