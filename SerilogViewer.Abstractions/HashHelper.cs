namespace SerilogViewer.Abstractions;

public static class HashHelper
{
	public static string ToMd5(this string input)
	{
		var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
		var hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);
		return Convert.ToHexStringLower(hashBytes);
	}
}
