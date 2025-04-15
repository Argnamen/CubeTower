using UnityEngine;

public interface ITowerManager
{
    public int TowerCubesCount { get; }
    public void AddToTower(CubeHierechy cube);
    public void RemoveFromTower(CubeHierechy cube);
    public bool IsCubeOnTower(Vector2 dropPosition, int lastCubeNumber);

    public void SaveTower();
    public CubeHierechy[] LoadTower(Transform parent, ICubeFactory cubeFactory);
}
