using UnityEngine;
using UnityEngine.UI;

public class CubeFactory : ICubeFactory
{
    private readonly GameObject _cubePrefab;

    public CubeFactory(GameObject cubePrefab)
    {
        _cubePrefab = cubePrefab;
    }

    public CubeHierechy CreateCube(Transform parent, Color32 color)
    {
        GameObject cube = Object.Instantiate(_cubePrefab, parent);
        cube.GetComponent<Image>().color = color;
        return cube.GetComponent<CubeHierechy>();
    }
}
