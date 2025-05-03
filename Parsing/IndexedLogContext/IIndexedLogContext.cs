using Microsoft.EntityFrameworkCore;

namespace Service.IndexedLogContext;

public interface IIndexedLogContext
{
	DbSet<ExceptionTemplate> ExceptionTemplates { get; set; }
	DbSet<ExceptionInstance> ExceptionInstances { get; set; }
}
