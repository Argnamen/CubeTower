using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadManager
{
    private List<CubeHierechy> _towerCubes = new List<CubeHierechy>();
    public void Save(CubeHierechy[] towerCubes)
    {
        TowerSaveData saveData = new TowerSaveData();

        _towerCubes = towerCubes.ToList();

        foreach (var cube in _towerCubes)
        {
            CubeSaveData cubeData = new CubeSaveData
            {
                PositionX = cube.NextPosition.x,
                PositionY = cube.NextPosition.y,
                ColorR = cube.Color.r,
                ColorG = cube.Color.g,
                ColorB = cube.Color.b,
                ColorA = cube.Color.a
            };

            Debug.Log(cubeData.PositionX + " " + cubeData.PositionY);

            saveData.Cubes.Add(cubeData);
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("TowerSaveData", json);
        PlayerPrefs.Save();
    }

    public CubeHierechy[] Load(Transform parent, ICubeFactory cubeFactory)
    {
        if (PlayerPrefs.HasKey("TowerSaveData"))
        {
            string json = PlayerPrefs.GetString("TowerSaveData");
            TowerSaveData saveData = JsonUtility.FromJson<TowerSaveData>(json);

            foreach (var cubeData in saveData.Cubes)
            {
                Color32 color = new Color32(cubeData.ColorR, cubeData.ColorG, cubeData.ColorB, cubeData.ColorA);
                CubeHierechy cube = cubeFactory.CreateCube(parent, color);
                cube.transform.position = new Vector3(cubeData.PositionX, cubeData.PositionY, 0);

                cube.OnTower = true;
                cube.Color = color;
                cube.NextPosition = new Vector2(cubeData.PositionX, cubeData.PositionY);

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
