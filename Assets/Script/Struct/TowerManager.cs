using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TowerManager : ITowerManager
{
    private List<GameObject> _towerCubes = new List<GameObject>();

    private readonly float _cubeSize;
    private readonly float _screenTopY;
    private readonly UIView _uiView;
    private readonly StateManager _stateManager;
    private readonly SaveLoadManager _saveLoadManager;

    public int TowerCubesCount => _towerCubes.Count;

    public TowerManager(UIView uIView, StateManager stateManager, SaveLoadManager saveLoadManager, float cubeSize)
    {
        _uiView = uIView;
        _stateManager = stateManager;
        _cubeSize = cubeSize;
        _saveLoadManager = saveLoadManager;
        _screenTopY = uIView.TowerArea.rect.height;
    }

    public void SaveTower()
    {
        _saveLoadManager.Save(_towerCubes.ToArray());
    }

    public GameObject[] LoadTower(Transform parent, ICubeFactory cubeFactory)
    {
        GameObject[] loadCubes = _saveLoadManager.Load(parent, cubeFactory);

        if (loadCubes != null)
        {
            _towerCubes = loadCubes.ToList();

            for(int i = 0; i < _towerCubes.Count; i++)
            {
                _towerCubes[i].GetComponent<CubeHierechy>().Color = loadCubes[i].GetComponent<CubeHierechy>().Color;
                _towerCubes[i].GetComponent<CubeHierechy>().NextPosition = loadCubes[i].GetComponent<CubeHierechy>().NextPosition;
            }

            return _towerCubes.ToArray();
        }
        else
            return null;
    }

    public void AddToTower(GameObject cube)
    {
        Vector3 position = _towerCubes.Count == 0 ? _uiView.TowerArea.position : _towerCubes[_towerCubes.Count - 1].transform.position;
        position += Vector3.up * _cubeSize;
        position += Vector3.right * Random.Range(-_cubeSize / 2, _cubeSize / 2);
        cube.GetComponent<CubeHierechy>().NextPosition = position;

        _towerCubes.Add(cube);

        cube.transform.DOJump(position, 20f, 1, 0.5f).SetEase(Ease.OutQuad);
        _stateManager.UpdateState(TowerState.InTower);

        SaveTower();
    }

    public void RemoveFromTower(GameObject cube)
    {
        int index = _towerCubes.IndexOf(cube);

        Sequence animation = DOTween.Sequence();

        _stateManager.UpdateState(TowerState.InHole);

        if (index != -1)
        {
            _towerCubes.RemoveAt(index);

            animation.Append(cube.transform.DOMove(_uiView.HolePoint.position, 0.3f).SetEase(Ease.Flash));
            animation.Append(cube.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Object.Destroy(cube)));

            for (int i = index; i < _towerCubes.Count; i++)
            {
                Vector3 targetPosition = new Vector3(
                    _towerCubes[i].transform.position.x,
                    index > 0 ? _towerCubes[index - 1].transform.position.y + (_cubeSize * (i - index + 1)) : _uiView.TowerArea.position.y + (_cubeSize * (i - index + 1)),
                    _towerCubes[i].transform.position.z);

                if (IsCubeOnTower(targetPosition, i - 1) && index > 0)
                {
                    _towerCubes[i].transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);

                    _towerCubes[i].GetComponent<CubeHierechy>().NextPosition = targetPosition;

                    SaveTower();
                }
                else
                {
                    RemoveFromTower(_towerCubes[i]);
                    break;
                }
            }
        }
        else
        {
            cube.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Object.Destroy(cube));
        }

        SaveTower();
    }

    public bool IsCubeOnTower(Vector2 dropPosition, int lastCubeNumber)
    {
        if (lastCubeNumber < 0)
            lastCubeNumber = _towerCubes.Count - 1;

        if (RectTransformUtility.RectangleContainsScreenPoint(_uiView.TowerArea, dropPosition))
        {
            if (lastCubeNumber < 0 && _towerCubes.Count != 0)
                return true;

            if (_towerCubes.Count != 0)
                dropPosition = new Vector2(
                    dropPosition.x,
                    _towerCubes[lastCubeNumber].transform.position.y);

            if (_towerCubes.Count == 0 || RectTransformUtility.RectangleContainsScreenPoint(_towerCubes[lastCubeNumber].GetComponent<RectTransform>(), dropPosition))
            {
                if (CheckTowerHeight(_towerCubes.Count))
                {
                    return true;
                }
                else
                {
                    _stateManager.UpdateState(TowerState.TowerHeight);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool CheckTowerHeight(int towerHeight)
    {
        float totalHeight = (towerHeight + 1) * _cubeSize;
        return totalHeight < _screenTopY;
    }
}
