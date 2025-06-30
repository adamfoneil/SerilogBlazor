using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SerilogViewer.Abstractions.SavedSearches;

public interface ISerilogSavedSearches
{
	DbSet<SerilogSavedSearch> SerilogSavedSearches { get; set; }
}

public class SerilogSavedSearch
{
	public int Id { get; set; }
	public string UserName { get; set; } = string.Empty;
	public string SearchName { get; set; } = default!;
	public string Expression { get; set; } = default!;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }
}

public class SerilogSavedSearchConfiguration : IEntityTypeConfiguration<SerilogSavedSearch>
{
	public void Configure(EntityTypeBuilder<SerilogSavedSearch> builder)
	{
		builder.HasIndex(e => new { e.UserName, e.SearchName }).IsUnique();
		builder.Property(e => e.UserName).IsRequired().HasMaxLength(50);
		builder.Property(e => e.SearchName).IsRequired().HasMaxLength(100);
		builder.Property(e => e.Expression).IsRequired().HasMaxLength(255);
	}
}