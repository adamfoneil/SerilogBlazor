using System.Text.RegularExpressions;

namespace Service;

public static class StackTraceParser
{
	public static StackTraceCore Parse(string stackTrace, string basePath, string myCodePrefix)
	{
		if (string.IsNullOrWhiteSpace(stackTrace))
			throw new ArgumentException("Stack trace cannot be null or empty.", nameof(stackTrace));

		string[] lines = stackTrace.Split(["\r", "\n", "     at "], StringSplitOptions.RemoveEmptyEntries);
		var codeLocations = new List<CodeLocation>();
		
		var firstLineMatch = Regex.Match(lines[0], @"^(?<exceptionType>[\w\.]+): (?<message>.+)");

		string exceptionType;
		string message;
		if (firstLineMatch.Success)
		{
			exceptionType = firstLineMatch.Groups["exceptionType"].Value.Trim();
			message = firstLineMatch.Groups["message"].Value.Trim();

			var innerMessageStart = message.IndexOf(" ---> ");
			if (innerMessageStart > -1) message = message[..innerMessageStart].Trim();
		}
		else
		{
			throw new InvalidOperationException("Could not parse exception type and message from stack trace.");
		}

		// Regex to match stack frames with file info
		var fileInfoRegex = new Regex(@"(?<method>[^\(]+)(?:\(.*?\))? in (?<file>.+):line (?<line>\d+)", RegexOptions.Compiled);

		foreach (var line in lines)
		{
			var match = fileInfoRegex.Match(line);
			if (match.Success)
			{
				var fullMethodName = match.Groups["method"].Value.Trim();
				var method = fullMethodName.Split('.').Last();

				var file = match.Groups["file"].Value.Trim();
				var lineNumber = int.Parse(match.Groups["line"].Value);

				if (fullMethodName.StartsWith(myCodePrefix, StringComparison.Ordinal))
				{
					// Remove basePath if it matches
					if (!string.IsNullOrEmpty(basePath) && file.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
					{
						file = file[basePath.Length..].TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
					}

					codeLocations.Add(new(method, file, lineNumber));
				}
			}
		}

		return new StackTraceCore(exceptionType, message, [.. codeLocations]);
	}
}
