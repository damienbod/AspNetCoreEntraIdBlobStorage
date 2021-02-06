using System;
using System.Collections.Generic;

namespace AspNetCoreAzureStorage.FilesProvider.SqlDataAccess
{
    public class UploadedFileResult
    {
        public List<(string FileName, string ContentType)> FileInfos { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
       
    }
}