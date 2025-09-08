namespace SleepPreventer.Settings;

internal class SettingsService //: ISettingsService
{
    private readonly AppSettings _settings;
    //public SettingsService(IOptions<AppSettings> settings)
    //{
    //    _settings = settings.Value;
    //}

    public AppSettings Get()
    {
        //var interval = _settings.Interval;
        //var settingsAreOk = Enum.TryParse<KeyCode>(_settings.Key, ignoreCase: true, out var keyCode);
        return _settings;
    }
}
