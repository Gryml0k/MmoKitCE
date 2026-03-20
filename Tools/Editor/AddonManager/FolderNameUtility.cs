/**
 * FolderNameUtility
 * Author: Denarii Games
 * Version: 1.0-rc1
 */

using System;
using System.IO;
using System.Text.RegularExpressions;

public static class FolderNameUtility
{
	// Regex to remove invalid filename characters
	private static readonly Regex InvalidCharsRegex = new Regex(
		$"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]",
		RegexOptions.Compiled);

	// Reserved Windows names (case-insensitive)
	private static readonly string[] ReservedNames = {
		"CON", "PRN", "AUX", "NUL",
		"COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
		"LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
	};

	/// <summary>
	/// Converts any string into a safe folder name.
	/// </summary>
	public static string MakeSafeFolderName(string name, string fallback = "Addon")
	{
		if (string.IsNullOrWhiteSpace(name))
			return fallback;

		// Trim whitespace
		string safeName = name.Trim();

		// Remove invalid characters
		safeName = InvalidCharsRegex.Replace(safeName, "_");

		// Replace multiple underscores with single
		safeName = Regex.Replace(safeName, @"_+", "_");

		// Trim underscores from start/end
		safeName = safeName.Trim('_');

		// Handle reserved names
		string upper = safeName.ToUpperInvariant();
		if (Array.Exists(ReservedNames, reserved => upper == reserved || upper.StartsWith(reserved + ".")))
		{
			safeName = "_" + safeName;
		}

		// Final fallback if name is empty after cleaning
		if (string.IsNullOrEmpty(safeName))
			safeName = fallback;

		// Limit length (Windows max path is 260, but folder names should be shorter)
		if (safeName.Length > 100)
			safeName = safeName.Substring(0, 100);

		return safeName;
	}
}