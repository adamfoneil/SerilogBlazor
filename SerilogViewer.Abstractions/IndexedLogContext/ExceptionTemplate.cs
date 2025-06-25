using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SerilogViewer.Service.IndexedLogContext;

public class ExceptionTemplate
{
	public int Id { get; set; }
	public string SourceContext { get; set; } = default!;
	/// <summary>
	/// md5 hash of error location
	/// </summary>
	public string ErrorId { get; set; } = default!;
	/// <summary>
	/// exception type
	/// </summary>
	public string Type { get; set; } = default!;
	/// <summary>
	/// first message of this ErrorId
	/// </summary>
	public string Message { get; set; } = default!;
	/// <summary>
	/// json parse of stack trace
	/// </summary>
	public string StackTraceData { get; set; } = default!;

	public ICollection<ExceptionInstance> Instances { get; set; } = [];
}

public class ExceptionTemplateConfiguration : IEntityTypeConfiguration<ExceptionTemplate>
{
	public void Configure(EntityTypeBuilder<ExceptionTemplate> builder)
	{
		builder.Property(e => e.SourceContext).HasMaxLength(255).IsRequired();
		builder.Property(e => e.ErrorId).HasMaxLength(32).IsRequired();
		builder.Property(e => e.Type).HasMaxLength(255).IsRequired();
		builder.Property(e => e.Message).IsRequired();
		builder.Property(e => e.StackTraceData).IsRequired();
	}
}