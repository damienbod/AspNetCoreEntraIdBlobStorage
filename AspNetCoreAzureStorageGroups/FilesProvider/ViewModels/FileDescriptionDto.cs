namespace AspNetCoreAzureStorageGroups.FilesProvider.ViewModels
{
    public class FileDescriptionDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string UploadedBy { get; set; }
    }
}