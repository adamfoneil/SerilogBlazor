using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SerilogViewer.Abstractions.IndexedLogContext;

/// <summary>
/// Represents a Serilog entry in the database
/// </summary>
public class SerilogEntry
{
	public int Id { get; set; }
	public DateTime Timestamp { get; set; }
	public string? SourceContext { get; set; }
	public string? RequestId { get; set; }
	public string Level { get; set; } = default!;
	public string MessageTemplate { get; set; } = default!;
	public string Message { get; set; } = default!;
	public string? Exception { get; set; }
	public string? Properties { get; set; }
}

public class SerilogEntryConfiguration : IEntityTypeConfiguration<SerilogEntry>
{
	public void Configure(EntityTypeBuilder<SerilogEntry> builder)
	{
		builder.ToTable("Serilog", "log");
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Level).IsRequired();
		builder.Property(e => e.MessageTemplate).IsRequired();
		builder.Property(e => e.Message).IsRequired();
	}
}