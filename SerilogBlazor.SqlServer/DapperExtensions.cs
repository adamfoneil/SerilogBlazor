using Dapper;
using Microsoft.Extensions.Logging;

namespace SerilogBlazor.SqlServer;

internal static class DapperExtensions
{
	internal static void LogParameters<T>(this DynamicParameters dynamicParameters, ILogger<T> logger)
	{
		if (dynamicParameters == null || !dynamicParameters.ParameterNames.Any()) return;

		foreach (var paramName in dynamicParameters.ParameterNames)
		{
			var value = dynamicParameters.Get<object>(paramName);
			logger.LogDebug("Parameter: {ParamName} = {Value}", paramName, value);
		}
	}
}
