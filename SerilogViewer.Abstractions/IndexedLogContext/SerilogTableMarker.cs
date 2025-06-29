using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SerilogViewer.Abstractions.IndexedLogContext;

/// <summary>
/// tracks the last log id that was queried for each serilog table (normally you have just one)
/// </summary>
public class SerilogTableMarker
{
	public int Id { get; set; }
	public string SchemaName { get; set; } = default!;
	public string TableName { get; set; } = default!;
	/// <summary>
	/// last minimum log id that was queried
	/// </summary>
	public int LogId { get; set; }	
}

public class SerilogTableMarkerConfiguration : IEntityTypeConfiguration<SerilogTableMarker>
{
	public void Configure(EntityTypeBuilder<SerilogTableMarker> builder)
	{
		builder.ToTable("SerilogTableMarker", "serilog");
		builder.Property(e => e.SchemaName).HasMaxLength(128).IsRequired();
		builder.Property(e => e.TableName).HasMaxLength(255).IsRequired();
		builder.HasIndex(builder => new { builder.SchemaName, builder.TableName }).IsUnique();
	}
}
