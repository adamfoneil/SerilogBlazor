using System.Security.Cryptography;
using System.Text.Json;

namespace SerilogBlazor.ApiConnector;

internal static class HashHelper
{
	internal static string ToMd5(this string? input)
	{
		input ??= "empty";
		var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
		var hashBytes = MD5.HashData(inputBytes);
		return Convert.ToHexStringLower(hashBytes);
	}

	internal static string ToMd5<T>(this T input)
	{
		var json = JsonSerializer.Serialize(input); 
		return ToMd5(json);
	}
}
