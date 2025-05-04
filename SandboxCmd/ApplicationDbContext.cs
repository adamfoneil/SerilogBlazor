using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Service.IndexedLogContext;

namespace SandboxCmd;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IIndexedLogContext
{
	public DbSet<ExceptionTemplate> ExceptionTemplates { get; set; }
	public DbSet<ExceptionInstance> ExceptionInstances { get; set; }
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=SerilogViewerDemo;Integrated Security=true;TrustServerCertificate=True;";

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		optionsBuilder.UseSqlServer(ConnectionString);
		return new ApplicationDbContext(optionsBuilder.Options);
	}
}