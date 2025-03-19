using UnityEngine;

public interface ITowerManager
{
    public int TowerCubesCount { get; }
    public void AddToTower(GameObject cube);
    public void RemoveFromTower(GameObject cube);
    public bool IsCubeOnTower(Vector2 dropPosition, int lastCubeNumber);

    public void SaveTower();
    public GameObject[] LoadTower(Transform parent, ICubeFactory cubeFactory);
}
