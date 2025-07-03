using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;

namespace SerilogBlazor.Postgres;

public static class PostgresColumnOptions
{
	public static IDictionary<string, ColumnWriterBase> Default => new Dictionary<string, ColumnWriterBase>
	{
		{ "timestamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
		{ "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
		{ "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
		{ "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
		{ "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
		{ "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
		{ "source_context", new SinglePropertyColumnWriter("SourceContext", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar, "l", 255) },
		{ "request_id", new SinglePropertyColumnWriter("RequestId", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar, "l", 50) },
		{ "user_name", new SinglePropertyColumnWriter("UserName", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar, "l", 50) }
	};
}