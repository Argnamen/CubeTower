using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class CubeController : MonoBehaviour
{
    public GameConfig gameConfig;

    public GameObject cubePrefab;
    public Transform bottomPanel;
    public RectTransform towerArea;
    public RectTransform holeArea;
    public TMP_Text commentText;

    private int _cubeCount = 20;
    private Color32[] _cubeColors;
    private TowerState _towerState;

    private List<GameObject> towerCubes = new List<GameObject>();
    private float cubeSize;
    private float screenTopY;

    private void Start()
    {
        _cubeCount = gameConfig.cubeCount;
        _cubeColors = gameConfig.cubeColors;
        cubeSize = cubePrefab.GetComponent<RectTransform>().rect.height; // Высота кубика
        screenTopY = towerArea.rect.height; // Верхняя граница экрана
        GenerateCubes();
    }

    private void GenerateCubes()
    {
        for (int i = 0; i < _cubeCount; i++)
        {
            GameObject cube = Instantiate(cubePrefab, bottomPanel);
            cube.GetComponent<Image>().color = _cubeColors[i];
            cube.GetComponent<DragHandler>().OnDrop += OnCubeDropped;

            cube.GetComponent<CubeHierechy>().Color = _cubeColors[i];
        }
    }

    private void OnCubeDropped(GameObject cube, Vector2 dropPosition)
    {
        var cloneCube = cube;

        _towerState = TowerState.Null;

        if (!cube.GetComponent<DragHandler>().OnTower)
        {
            cloneCube = Instantiate(cube, bottomPanel.root);
            cloneCube.GetComponent<DragHandler>().OnTower = true;
            cloneCube.GetComponent<DragHandler>().OnDrop += OnCubeDropped;
        }
        else if (!RectTransformUtility.RectangleContainsScreenPoint(holeArea, dropPosition))
        {
            cube.transform.position = cube.GetComponent<DragHandler>().Hierechy.StartPosition;
            return;
        }

        if (isCubeOnTower(dropPosition, towerCubes.Count - 1))
        {
            AddToTower(cloneCube);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(holeArea, dropPosition))
        {
            RemoveFromTower(cloneCube);
        }
        else
        {
            MissedCube(cloneCube);
        }

        StateInText();
    }

    private bool isCubeOnTower(Vector2 dropPosition, int lastCubeNumber)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(towerArea, dropPosition))
        {
            if (lastCubeNumber < 0 && towerCubes.Count != 0)
                return true;

            if (towerCubes.Count != 0)
                dropPosition = new Vector2(
                    dropPosition.x,
                    towerCubes[lastCubeNumber].transform.position.y);

            if (towerCubes.Count == 0 || RectTransformUtility.RectangleContainsScreenPoint(towerCubes[lastCubeNumber].GetComponent<RectTransform>(), dropPosition))
            {
                if (CheckTowerHeight(towerCubes.Count))
                {
                    return true;
                }
                else
                {
                    UpdateTowerState(TowerState.TowerHeight);
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
        float totalHeight = (towerHeight + 1) * cubeSize;
        return totalHeight < screenTopY;
    }

    private void AddToTower(GameObject cube)
    {
        Vector3 position = towerCubes.Count == 0 ? towerArea.position : towerCubes[towerCubes.Count - 1].transform.position;
        position += Vector3.up * cubeSize;
        position += Vector3.right * Random.Range(-cubeSize / 2, cubeSize / 2); // Случайное смещение по горизонтали
        cube.transform.position = position;
        towerCubes.Add(cube);

        // Анимация подпрыгивания с DOTween
        cube.transform.DOJump(position, 20f, 1, 0.5f).SetEase(Ease.OutQuad);

        UpdateTowerState(TowerState.InTower);
    }

    private void RemoveFromTower(GameObject cube)
    {
        int index = towerCubes.IndexOf(cube);
        if (index != -1)
        {
            towerCubes.RemoveAt(index);
            cube.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Destroy(cube)); // Анимация исчезновения

            UpdateTowerState(TowerState.InHole);

            for (int i = index; i < towerCubes.Count; i++)
            {
                Vector3 targetPosition = new Vector3(
                        towerCubes[i].transform.position.x,
                        index > 0 ? towerCubes[index - 1].transform.position.y + (cubeSize * (i - index + 1)) : towerArea.position.y + (cubeSize * (i - index + 1)),
                        towerCubes[i].transform.position.z);

                if (isCubeOnTower(targetPosition, i - 1))
                {
                    towerCubes[i].transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);

                    Debug.Log("Cube number: " + i);
                }
                else
                {
                    RemoveFromTower(towerCubes[i]);
                    break;
                }
            }
        }
        else
        {
            cube.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Destroy(cube));
        }
    }

    private void MissedCube(GameObject cube)
    {
        // Анимация исчезновения с DOTween
        cube.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Destroy(cube));

        UpdateTowerState(TowerState.Missed);
    }

    private void UpdateTowerState(TowerState towerState)
    {
        if(_towerState == TowerState.Null)
        {
            _towerState = towerState;
        }
    }

    private void StateInText()
    {
        switch (_towerState) 
        {
            case TowerState.InTower:
                commentText.text = gameConfig.Localization.Find(x => x.Key == "IN_TOWER").RUS;
                break;
            case TowerState.InHole:
                commentText.text = gameConfig.Localization.Find(x => x.Key == "IN_HOLE").RUS;
                break;
            case TowerState.TowerHeight:
                commentText.text = gameConfig.Localization.Find(x => x.Key == "TOWER_HEIGHT").RUS;
                break;
            case TowerState.Missed:
                commentText.text = gameConfig.Localization.Find(x => x.Key == "MISSED").RUS;
                break;
        }
    }
}

public enum TowerState
{
    InTower, InHole, TowerHeight, Missed, Null
}