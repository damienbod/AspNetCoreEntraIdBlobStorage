using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AspNetCoreAzureStorage.FilesProvider.ViewModels;

namespace AspNetCoreAzureStorage.FilesProvider.SqlDataAccess
{
    public class FileDescriptionProvider
    {

        private readonly FileContext _context;

        private readonly ILogger _logger;


        public FileDescriptionProvider(FileContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("FileRepository");
        }

        public IEnumerable<FileDescriptionShort> GetAllFiles()
        {
            return _context.FileDescriptions.Select(
                    t => new FileDescriptionShort { Name = t.FileName, Id = t.Id, Description = t.Description });
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
                    CreatedTimestamp = uploadedFileResult.CreatedTimestamp,
                    UpdatedTimestamp = uploadedFileResult.UpdatedTimestamp
                });
            }

            await _context.SaveChangesAsync();

        }

    }
}

