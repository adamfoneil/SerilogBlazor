using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SerilogViewer.Abstractions.SourceContextView;

/// <summary>
/// sets visibility of different source contexts in the log view UI
/// </summary>
public class SourceContextView
{
	public int Id { get; set; }
	public string UserName { get; set; } = default!;
	public string SourceContext { get; set; } = default!;
	public string Level { get; set; } = default!;
	public bool IsVisible { get; set; } = true;
}

public class SourceContextViewConfiguration : IEntityTypeConfiguration<SourceContextView>
{
	public void Configure(EntityTypeBuilder<SourceContextView> builder)
	{		
		builder.HasIndex(e => new { e.SourceContext, e.Level, e.UserName }).IsUnique();		
		builder.Property(e => e.SourceContext).HasMaxLength(255).IsRequired();
		builder.Property(e => e.Level).HasMaxLength(10).IsRequired();
		builder.Property(e => e.UserName).HasMaxLength(100).IsRequired();
	}
}