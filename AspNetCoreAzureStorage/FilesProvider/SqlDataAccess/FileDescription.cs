namespace AspNetCoreAzureStorage.FilesProvider.SqlDataAccess;

public class FileDescription
{
    public int Id { get; set; }
    public string? FileName { get; set; }
    public string? Description { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime CreatedTimestamp { get; set; }
    public DateTime UpdatedTimestamp { get; set; }
    public string? ContentType { get; set; }
}
