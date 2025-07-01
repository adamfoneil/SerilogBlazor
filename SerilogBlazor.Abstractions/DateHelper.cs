namespace SerilogBlazor.Abstractions;

public static class DateHelper
{
	public static string ParseAgeText(int ageMinutes)
	{
		if (ageMinutes < 1)
			return "just now";

		var ts = TimeSpan.FromMinutes(ageMinutes);

		var parts = new List<string>();

		if (ts.Days > 0)
			parts.Add($"{ts.Days}d");
		if (ts.Hours > 0)
			parts.Add($"{ts.Hours}h");
		if (ts.Minutes > 0 && ts.Days == 0) // Only show minutes if less than a day
			parts.Add($"{ts.Minutes}m");

		return string.Join(", ", parts);
	}
}
