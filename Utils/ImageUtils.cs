namespace Utils;

public class ImageUtils
{
    private static readonly HashSet<string> ValidImageExtensions =
        [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg", ".webp"];
    
    public static string GetImagePath(string imageName)
    {
        var dir = Directory.GetParent(Environment.CurrentDirectory)!.FullName;
        var uploadsFolder = Path.Combine(dir, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        return Path.Combine(uploadsFolder, imageName);
    }
    
    public static bool IsValidImageExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension) || !extension.StartsWith('.'))
        {
            return false;
        }

        return ValidImageExtensions.Contains(extension.ToLower());
    }
}