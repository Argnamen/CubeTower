using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GameObject cubePrefab;
    public GameConfig gameConfig;
    public UIView uI;

    public override void InstallBindings()
    {
        Container.Bind<IGameConfig>().FromInstance(gameConfig).AsSingle();
        Container.Bind<ICubeFactory>().To<CubeFactory>().AsSingle().WithArguments(cubePrefab);
        Container.Bind<StateManager>().AsSingle();
        Container.Bind<SaveLoadManager>().AsSingle();
        Container.Bind<ITowerManager>().To<TowerManager>().AsSingle().WithArguments(cubePrefab.GetComponent<RectTransform>().rect.height);

        Container.Bind<UIView>().FromInstance(uI).AsSingle();
        Container.Bind<ILocalizationManager>().To<LocalizationManager>().AsSingle();
    }
}
