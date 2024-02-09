using Microsoft.EntityFrameworkCore;

namespace DelegatedEntraIDBlobStorage.FilesProvider.SqlDataAccess;

public class FileContext : DbContext
{
    public FileContext(DbContextOptions<FileContext> options) : base(options)
    { }

    public DbSet<FileDescription> FileDescriptions => Set<FileDescription>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<FileDescription>().HasKey(m => m.Id);
        base.OnModelCreating(builder);
    }
}