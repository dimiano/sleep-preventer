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

        // 1. settings file valid
        using var stream = File.OpenRead(_settingsFilePath);
        var settings = JsonSerializer.Deserialize<AppSettings>(stream, _jsonOptions);
        if (IsValid(settings))
            return NormalizeSettings(settings);

        // 2. settings file invalid
        Save(defaults);
        //catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException or ArgumentException)

        return defaults;
    }

    public void Save(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        using var stream = new FileStream(_settingsFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        JsonSerializer.Serialize(stream, settings, _jsonOptions);
    }

    private static AppSettings NormalizeSettings(AppSettings settings)
    {
        const string httpsPrefix = "https://";

        if (!string.IsNullOrWhiteSpace(settings.Url) && 
            !settings.Url.StartsWith(httpsPrefix, StringComparison.OrdinalIgnoreCase) &&
            !settings.Url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            settings.Url = httpsPrefix + settings.Url;
        }

        return settings;
    }

    private static bool IsValid(AppSettings settings)
    {
        if(settings is null) return false;

        var isIntervalValid = AppSettings.IsIntervalValid(settings.Interval);
        var isKeyValid = Enum.IsDefined(settings.Key);
        return isIntervalValid && isKeyValid;
    }
}
