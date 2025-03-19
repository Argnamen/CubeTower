using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GameObject Cube;
    public GameConfig Game;
    public LocalizationConfig Localization;
    public UIView uI;

    public override void InstallBindings()
    {
        Container.Bind<IGameConfig>().FromInstance(Game).AsSingle();
        Container.Bind<LocalizationConfig>().FromInstance(Localization).AsSingle();
        Container.Bind<ICubeFactory>().To<CubeFactory>().AsSingle().WithArguments(Cube);
        Container.Bind<StateManager>().AsSingle();
        Container.Bind<SaveLoadManager>().AsSingle();
        Container.Bind<ITowerManager>().To<TowerManager>().AsSingle().WithArguments(Cube.GetComponent<RectTransform>().rect.height);

        Container.Bind<UIView>().FromInstance(uI).AsSingle();
        Container.Bind<ILocalizationManager>().To<LocalizationManager>().AsSingle();
    }
}
