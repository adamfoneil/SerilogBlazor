using SerilogViewer.Abstractions.SourceContextView;
using SerilogViewer.SqlServer;

namespace SampleApp.Data;

public partial class ApplicationDbContext
{
	public Task<SourceContextViewItem[]> GetSourceContextViewItemsAsync(string userName) => 
		this.QuerySourceContextViewItemsAsync(userName);


	public Task SetSourceContextVisibilityAsync(string userName, string sourceContext, string level, bool isVisible)
	{
		throw new NotImplementedException();
	}
}
