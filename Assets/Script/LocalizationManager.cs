public class LocalizationManager : ILocalizationManager
{
    private readonly IGameConfig _gameConfig;

    public LocalizationManager(IGameConfig gameConfig)
    {
        _gameConfig = gameConfig;
    }

    public string GetLocalizedText(string key)
    {
        var localization = _gameConfig.Localization.Find(x => x.Key == key);
        return localization?.RUS ?? string.Empty;
    }
}
