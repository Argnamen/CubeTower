using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

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

    private List<GameObject> towerCubes = new List<GameObject>();
    private float cubeSize;
    private float screenTopY;

    private void Start()
    {
        _cubeCount = gameConfig.cubeCount;
        _cubeColors = gameConfig.cubeColors;
        cubeSize = cubePrefab.GetComponent<RectTransform>().rect.height; // Высота кубика
        screenTopY = Screen.height; // Верхняя граница экрана
        GenerateCubes();
    }

    private void GenerateCubes()
    {
        for (int i = 0; i < _cubeCount; i++)
        {
            GameObject cube = Instantiate(cubePrefab, bottomPanel);
            cube.GetComponent<Image>().color = _cubeColors[i];
            cube.GetComponent<DragHandler>().OnDrop += OnCubeDropped;
        }
    }

    private void OnCubeDropped(GameObject cube, Vector2 dropPosition)
    {
        if (!cube.GetComponent<DragHandler>().OnTower)
        {
            cube = Instantiate(cube, bottomPanel.root);
            cube.GetComponent<DragHandler>().OnTower = true;
            cube.GetComponent<DragHandler>().OnDrop += OnCubeDropped;
        }

        if (isCubeOnTower(dropPosition, towerCubes.Count - 1))
        {
            AddToTower(cube);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(holeArea, dropPosition))
        {
            RemoveFromTower(cube);
        }
        else
        {
            MissedCube(cube);
        }
    }

    private bool isCubeOnTower(Vector2 dropPosition, int lastCubeNumber)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(towerArea, dropPosition))
        {
            if (towerCubes.Count != 0)
                dropPosition = new Vector2(
                    dropPosition.x,
                    towerCubes[lastCubeNumber].transform.position.y);

            if (towerCubes.Count == 0 || RectTransformUtility.RectangleContainsScreenPoint(towerCubes[lastCubeNumber].GetComponent<RectTransform>(), dropPosition))
            {
                if (CheckTowerHeight(cubeSize))
                {
                    return true;
                }
                else
                {
                    commentText.text = "Башня слишком высокая!";
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

    private bool CheckTowerHeight(float newCubeHeight)
    {
        float totalHeight = towerCubes.Count * cubeSize + newCubeHeight;
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
        commentText.text = "Кубик добавлен в башню!";
    }

    private void RemoveFromTower(GameObject cube)
    {
        int index = towerCubes.IndexOf(cube);
        if (index != -1)
        {
            towerCubes.RemoveAt(index);
            cube.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Destroy(cube)); // Анимация исчезновения

            for (int i = index; i < towerCubes.Count; i++)
            {
                Debug.Log(index + " " + towerCubes.Count + " " + i);

                if (isCubeOnTower(towerCubes[i].transform.position, i - 1))
                {
                    Vector3 targetPosition = new Vector3(
                        towerCubes[i].transform.position.x,
                        i > 0 ? towerCubes[i - 1].transform.position.y + cubeSize : towerArea.position.y + cubeSize,
                        towerCubes[i].transform.position.z);
                    towerCubes[i].transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);
                }
                else
                {
                    Debug.Log("Cube number: " + i);
                    RemoveFromTower(towerCubes[i]);
                    break;
                }
            }
            commentText.text = "Кубик выброшен в дыру!";
        }
    }

    private void MissedCube(GameObject cube)
    {
        // Анимация исчезновения с DOTween
        cube.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Destroy(cube));

        commentText.text = "Кубик пропал!";
    }
}