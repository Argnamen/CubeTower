using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Zenject;

public class CubeController : MonoBehaviour
{
    [Inject] private readonly IGameConfig _gameConfig;
    [Inject] private readonly ICubeFactory _cubeFactory;
    [Inject] private readonly ITowerManager _towerManager;
    [Inject] private readonly StateManager _stateManager;
    [Inject] private readonly UIView _uIView;

    private int _cubeCount;
    private Color32[] _cubeColors;

    private void Start()
    {
        _cubeCount = _gameConfig.CubeCount;
        _cubeColors = _gameConfig.CubeColors;

        LoadGame();

        GenerateCubes();
    }

    private void LoadGame()
    {
        GameObject[] cubes = _towerManager.LoadTower(_uIView.TowerContainer, _cubeFactory);

        if (cubes != null)
        {
            foreach (GameObject cube in cubes)
            {
                cube.GetComponent<DragHandler>().OnDrop += OnCubeDropped;
            }
        }
    }

    private void GenerateCubes()
    {
        for (int i = 0; i < _cubeCount; i++)
        {
            GameObject cube = _cubeFactory.CreateCube(_uIView.BottomPanel, _cubeColors[i]);
            cube.GetComponent<DragHandler>().OnDrop += OnCubeDropped;
            cube.GetComponent<CubeHierechy>().Color = _cubeColors[i];
            cube.GetComponent<CubeHierechy>().ChildIndex = i;
        }
    }

    private void OnCubeDropped(GameObject cube, Vector2 dropPosition)
    {
        var cloneCube = cube;

        _stateManager.UpdateState(TowerState.Null);

        if (!cube.GetComponent<DragHandler>().OnTower)
        {
            cloneCube = _cubeFactory.CreateCube(_uIView.TowerContainer, cube.GetComponent<CubeHierechy>().Color);
            cloneCube.GetComponent<DragHandler>().OnTower = true;
            cloneCube.GetComponent<DragHandler>().OnDrop += OnCubeDropped;
            cloneCube.GetComponent<CubeHierechy>().Color = cube.GetComponent<CubeHierechy>().Color;
        }
        else if (!RectTransformUtility.RectangleContainsScreenPoint(_uIView.HoleArea, dropPosition))
        {
            cube.transform.position = cube.GetComponent<DragHandler>().Hierechy.StartPosition;
            return;
        }
        

        if (_towerManager.IsCubeOnTower(dropPosition, -1))
        {
            cloneCube.transform.position = dropPosition;
            _towerManager.AddToTower(cloneCube);
        }
        else
        {
            if (!cube.GetComponent<DragHandler>().OnTower)
            {
                cloneCube.transform.position = dropPosition;
                cloneCube.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Object.Destroy(cloneCube));
                _stateManager.UpdateState(TowerState.Missed);
            }
            else
                _towerManager.RemoveFromTower(cloneCube);
        }
    }
}

public enum TowerState
{
    InTower, InHole, TowerHeight, Missed, Null
}