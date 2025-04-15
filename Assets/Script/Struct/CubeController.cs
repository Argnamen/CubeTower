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
        CubeHierechy[] cubes = _towerManager.LoadTower(_uIView.TowerContainer, _cubeFactory);

        if (cubes != null)
        {
            foreach (var cube in cubes)
            {
                cube.DragHandler.AddDropEvent(OnCubeDropped);
            }
        }
    }

    private void GenerateCubes()
    {
        for (int i = 0; i < _cubeCount; i++)
        {
            CubeHierechy cube = _cubeFactory.CreateCube(_uIView.BottomPanel, _cubeColors[i]);
            cube.DragHandler.AddDropEvent(OnCubeDropped);
            cube.Color = _cubeColors[i];
            cube.ChildIndex = i;
        }
    }

    private void OnCubeDropped(CubeHierechy cube, Vector2 dropPosition)
    {
        var cloneCube = cube;

        _stateManager.UpdateState(TowerState.Null);

        if (!cube.OnTower)
        {
            cloneCube = _cubeFactory.CreateCube(_uIView.TowerContainer, cube.Color);
            cloneCube.OnTower = true;
            cloneCube.DragHandler.AddDropEvent(OnCubeDropped);
            cloneCube.Color = cube.Color;
        }
        else if (!RectTransformUtility.RectangleContainsScreenPoint(_uIView.HoleArea, dropPosition))
        {
            cube.transform.position = cube.StartPosition;
            return;
        }
        

        if (_towerManager.IsCubeOnTower(dropPosition, -1))
        {
            cloneCube.transform.position = dropPosition;
            _towerManager.AddToTower(cloneCube);
        }
        else
        {
            if (!cube.OnTower)
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