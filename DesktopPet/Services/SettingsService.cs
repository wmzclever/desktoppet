using DesktopPet.Models;
using System;
using System.IO;
using System.Text.Json;

namespace DesktopPet.Services;

public sealed class SettingsService
{
    private static readonly string ConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DesktopPet");
    private static readonly string ConfigPath = Path.Combine(ConfigDir, "settings.json");

    public PetSettings Load()
    {
        try
        {
            if (!File.Exists(ConfigPath)) return new PetSettings();
            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<PetSettings>(json) ?? new PetSettings();
        }
        catch
        {
            return new PetSettings();
        }
    }

    public void Save(PetSettings settings)
    {
        Directory.CreateDirectory(ConfigDir);
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
}
