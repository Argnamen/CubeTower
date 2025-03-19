using UnityEngine;

public interface ICubeFactory
{
    GameObject CreateCube(Transform parent, Color32 color);
}
