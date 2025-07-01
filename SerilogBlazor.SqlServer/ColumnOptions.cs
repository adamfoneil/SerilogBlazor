using Serilog.Sinks.MSSqlServer;
using System.Data;

namespace SerilogBlazor.SqlServer;

public static class SqlServerColumnOptions
{
	public static ColumnOptions Default => new()
	{
		AdditionalColumns =
		[
			new SqlColumn("SourceContext", SqlDbType.NVarChar, allowNull: true, dataLength: 256),
			new SqlColumn("RequestId", SqlDbType.NVarChar, allowNull: true, dataLength: 64),
			new SqlColumn("UserName", SqlDbType.NVarChar, allowNull: true, dataLength: 100),
		]
	};
}
