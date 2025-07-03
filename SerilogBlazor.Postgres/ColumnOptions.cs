using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;

namespace SerilogBlazor.Postgres;

public static class PostgresColumnOptions
{
	public static IDictionary<string, ColumnWriterBase> Default => new Dictionary<string, ColumnWriterBase>
	{
		{ "Timestamp", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
		{ "Level", new LevelColumnWriter(true, NpgsqlDbType.Integer) }, // Store as integer
		{ "Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
		{ "MessageTemplate", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
		{ "Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
		{ "Properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) }, // Store as JSONB
		{ "SourceContext", new SinglePropertyColumnWriter("SourceContext", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar, "l") },
		{ "RequestId", new SinglePropertyColumnWriter("RequestId", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar, "l") },
		{ "UserName", new SinglePropertyColumnWriter("UserName", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar, "l") }
	};

	/// <summary>
	/// Column options with computed SourceContext column
	/// Note: Postgres doesn't have computed columns like SQL Server, but we can use a view or trigger
	/// This is provided for compatibility - you may need to implement SourceContext extraction differently
	/// </summary>
	public static IDictionary<string, ColumnWriterBase> WithComputedSourceContext => new Dictionary<string, ColumnWriterBase>
	{
		{ "Timestamp", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
		{ "Level", new LevelColumnWriter(true, NpgsqlDbType.Integer) },
		{ "Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
		{ "MessageTemplate", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
		{ "Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
		{ "Properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
		// For computed SourceContext, you might need to extract it from Properties JSON
		// or use a database view/trigger - this is a placeholder
		{ "RequestId", new SinglePropertyColumnWriter("RequestId", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar, "l") },
		{ "UserName", new SinglePropertyColumnWriter("UserName", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar, "l") }
	};
}