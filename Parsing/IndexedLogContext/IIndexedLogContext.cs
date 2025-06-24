using Microsoft.EntityFrameworkCore;

namespace SerilogViewer.Service.IndexedLogContext;

public interface IIndexedLogContext
{
	DbSet<ExceptionTemplate> ExceptionTemplates { get; set; }
	DbSet<ExceptionInstance> ExceptionInstances { get; set; }
	DbSet<SerilogTableMarker> SerilogTableMarkers { get; set; }
}
