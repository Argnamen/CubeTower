public class LocalizationManager : ILocalizationManager
{
    private readonly LocalizationConfig _config;

    public LocalizationManager(LocalizationConfig config)
    {
        _config = config;
    }

    public string GetLocalizedText(string key)
    {
        var localization = _config.Localization.Find(x => x.Key == key);
        return localization?.RUS ?? string.Empty;
    }
}
