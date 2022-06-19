namespace AspNetCoreAzureStorageGroups.FilesProvider.SqlDataAccess;

public class UploadedFileResult
{
    public List<(string FileName, string ContentType)>? FileInfos { get; set; }
    public string? Description { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime CreatedTimestamp { get; set; }
    public DateTime UpdatedTimestamp { get; set; }

}