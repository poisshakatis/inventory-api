namespace Helpers;

public static class FileHelpers
{
    public static string GetImagePath(string imageName)
    {
        var dir = Directory.GetParent(Environment.CurrentDirectory)!.FullName;
        var uploadsFolder = Path.Combine(dir, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        return Path.Combine(uploadsFolder, imageName);
    }
}