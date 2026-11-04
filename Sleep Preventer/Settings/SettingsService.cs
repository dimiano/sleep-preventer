using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SleepPreventer.Settings;

internal class SettingsService
{
    private readonly string _settingsFilePath = "settings.json";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public AppSettings Get()
    {
        var defaults = new AppSettings();

        if (!File.Exists(_settingsFilePath))
        {
            Save(defaults);
            return defaults;
        }

        using var stream = File.OpenRead(_settingsFilePath);
        var settings = JsonSerializer.Deserialize<AppSettings>(stream, _jsonOptions);
        if (IsValid(settings))
            return settings;

        Save(defaults);

        //catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException or ArgumentException)

        return defaults;
    }

    public void Save(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        using (var stream = new FileStream(_settingsFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            JsonSerializer.Serialize(stream, settings, _jsonOptions);
        }
    }

    private static bool IsValid(AppSettings settings)
    {
        return settings is not null && settings.Interval > 0 && Enum.IsDefined(settings.Key);
    }
}
