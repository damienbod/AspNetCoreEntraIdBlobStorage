using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AspNetCoreAzureStorage.FilesProvider.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreAzureStorage.FilesProvider.SqlDataAccess
{
    public class FileDescriptionProvider
    {

        private readonly FileContext _context;

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public FileDescriptionProvider(FileContext context,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("FileRepository");
            _configuration = configuration;
        }

        public IEnumerable<FileDescriptionDto> GetAllFiles()
        {
            var storage = _configuration.GetValue<string>("AzureStorage:StorageAndContainerName");

            return _context.FileDescriptions.Select(t => new FileDescriptionDto
            {
                Name = t.FileName,
                FullName = $"{storage}{t.FileName}",
                Id = t.Id,
                UploadedBy = t.UploadedBy,
                Description = t.Description
            });
        }

        public FileDescription GetFileDescription(int id)
        {
            return _context.FileDescriptions.Single(t => t.Id == id);
        }

        public async Task AddFileDescriptionsAsync(UploadedFileResult uploadedFileResult)
        {
            foreach (var (FileName, ContentType) in uploadedFileResult.FileInfos)
            {
                _context.FileDescriptions.Add(new FileDescription
                {
                    FileName = FileName,
                    ContentType = ContentType,
                    Description = uploadedFileResult.Description,
                    UploadedBy = uploadedFileResult.UploadedBy,
                    CreatedTimestamp = uploadedFileResult.CreatedTimestamp,
                    UpdatedTimestamp = uploadedFileResult.UpdatedTimestamp
                });
            }

            await _context.SaveChangesAsync();

        }

    }
}

