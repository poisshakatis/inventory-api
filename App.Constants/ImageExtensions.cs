namespace App.Constants;

using System.Collections.Generic;

public static class ImageExtensions
{
    private static readonly HashSet<string> ValidImageExtensions =
        [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg", ".webp"];

    public static bool IsValidImageExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension) || !extension.StartsWith('.')) return false;

        return ValidImageExtensions.Contains(extension.ToLower());
    }
}