using DelegatedEntraIDBlobStorage.FilesProvider.AzureStorageAccess;
using DelegatedEntraIDBlobStorage.FilesProvider.SqlDataAccess;
using DelegatedEntraIDBlobStorage.FilesProvider.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace DelegatedEntraIDBlobStorage.Pages;

[Authorize(Policy = "blob-one-write-policy")]
[AuthorizeForScopes(Scopes = ["https://storage.azure.com/user_impersonation"])]
public class AzStorageFilesModel : PageModel
{
    private readonly BlobDelegatedUploadProvider _blobUploadProvider;
    private readonly FileDescriptionProvider _fileDescriptionProvider;

    [BindProperty]
    public FileDescriptionUpload FileDescriptionShort { get; set; } = new FileDescriptionUpload();

    public AzStorageFilesModel(BlobDelegatedUploadProvider blobUploadProvider,
        FileDescriptionProvider fileDescriptionProvider)
    {
        _blobUploadProvider = blobUploadProvider;
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
            if (HttpContext.Request.ContentType == null || !IsMultipartContentType(HttpContext.Request.ContentType))
            {
                ModelState.AddModelError("FileDescriptionShort.File", "not a MultipartContentType");
                return Page();
            }

            foreach (var file in FileDescriptionShort.File)
            {
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.ToString().Trim('"');
                    var userName = HttpContext.User.Identity?.Name;

                    if (fileName != null && userName != null)
                    {
                        fileInfos.Add((fileName, file.ContentType));

                        await _blobUploadProvider.AddNewFile(new BlobFileUploadModel
                        {
                            Name = fileName,
                            Description = FileDescriptionShort.Description,
                            UploadedBy = userName
                        }, file);
                    }
                }
            }
        }

        var files = new UploadedFileResult
        {
            FileInfos = fileInfos,
            Description = FileDescriptionShort.Description,
            UploadedBy = HttpContext.User.Identity?.Name,
            CreatedTimestamp = DateTime.UtcNow,
            UpdatedTimestamp = DateTime.UtcNow,
        };

        await _fileDescriptionProvider.AddFileDescriptionsAsync(files);

        return Page();
    }

    private static bool IsMultipartContentType(string contentType)
    {
        return !string.IsNullOrEmpty(contentType) && contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
    }
}