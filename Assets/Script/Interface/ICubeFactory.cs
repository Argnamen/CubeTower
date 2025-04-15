using UnityEngine;

public interface ICubeFactory
{
    CubeHierechy CreateCube(Transform parent, Color32 color);
}
