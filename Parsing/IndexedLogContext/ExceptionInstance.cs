using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Service.IndexedLogContext;

public class ExceptionInstance
{
	public int Id { get; set; }
	public int ExceptionTemplateId { get; set; }
	public DateTime Timestamp { get; set; }
	public string Message { get; set; } = default!;
	/// <summary>
	/// source Id in Serilog table
	/// </summary>
	public int LogId { get; set; }

	public ExceptionTemplate ExceptionTemplate { get; set; } = default!;
}

public class ExceptionInstanceConfiguration : IEntityTypeConfiguration<ExceptionInstance>
{
	public void Configure(EntityTypeBuilder<ExceptionInstance> builder)
	{
		builder.HasOne(e => e.ExceptionTemplate).WithMany(e => e.Instances).HasForeignKey(e => e.ExceptionTemplateId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
