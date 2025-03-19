using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadManager
{
    private List<GameObject> _towerCubes = new List<GameObject>();
    public void Save(GameObject[] towerCubes)
    {
        TowerSaveData saveData = new TowerSaveData();

        _towerCubes = towerCubes.ToList();

        foreach (var cube in _towerCubes)
        {
            CubeHierechy cubeHierechy = cube.GetComponent<CubeHierechy>();
            CubeSaveData cubeData = new CubeSaveData
            {
                PositionX = cubeHierechy.NextPosition.x,
                PositionY = cubeHierechy.NextPosition.y,
                ColorR = cubeHierechy.Color.r,
                ColorG = cubeHierechy.Color.g,
                ColorB = cubeHierechy.Color.b,
                ColorA = cubeHierechy.Color.a
            };

            Debug.Log(cubeData.PositionX + " " + cubeData.PositionY);

            saveData.Cubes.Add(cubeData);
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("TowerSaveData", json);
        PlayerPrefs.Save();
    }

    public GameObject[] Load(Transform parent, ICubeFactory cubeFactory)
    {
        if (PlayerPrefs.HasKey("TowerSaveData"))
        {
            string json = PlayerPrefs.GetString("TowerSaveData");
            TowerSaveData saveData = JsonUtility.FromJson<TowerSaveData>(json);

            foreach (var cubeData in saveData.Cubes)
            {
                Color32 color = new Color32(cubeData.ColorR, cubeData.ColorG, cubeData.ColorB, cubeData.ColorA);
                GameObject cube = cubeFactory.CreateCube(parent, color);
                cube.transform.position = new Vector3(cubeData.PositionX, cubeData.PositionY, 0);

                cube.GetComponent<DragHandler>().OnTower = true;
                cube.GetComponent<CubeHierechy>().Color = color;
                cube.GetComponent<CubeHierechy>().NextPosition = new Vector2(cubeData.PositionX, cubeData.PositionY);

                _towerCubes.Add(cube);
            }

            return _towerCubes.ToArray();
        }
        else
            return null;
    }

    // Метод для очистки сохранений
    public void ClearSave()
    {
        PlayerPrefs.DeleteKey("TowerSaveData");
        PlayerPrefs.Save();
    }
}
