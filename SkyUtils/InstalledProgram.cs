using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

public class InstalledProgram
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ExecutablePath { get; set; }

    private string _iconPath;

    [NotMapped]
    public string DisplayIcon
    {
        get
        {
            if (string.IsNullOrEmpty(_iconPath) && !string.IsNullOrEmpty(ExecutablePath))
            {
                _iconPath = ExtractIcon(ExecutablePath);
            }
            return _iconPath;
        }
    }

    private string ExtractIcon(string exePath)
    {
        try
        {
            Icon icon = Icon.ExtractAssociatedIcon(exePath);
            if (icon != null)
            {
                string tempPath = Path.Combine(Path.GetTempPath(), $"{Path.GetFileNameWithoutExtension(exePath)}.png");
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    icon.ToBitmap().Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                }
                return tempPath;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при извлечении иконки: {ex.Message}");
        }
        return string.Empty;
    }
}