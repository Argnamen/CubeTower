using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config/Game")]
public class GameConfig : ScriptableObject, IGameConfig
{
    public int cubeCount = 20;
    public Color32[] cubeColors;

    int IGameConfig.CubeCount => cubeCount;
    Color32[] IGameConfig.CubeColors => cubeColors;
}

[CreateAssetMenu(fileName = "LocalizationConfig", menuName = "Game/Config/Localization")]
public class LocalizationConfig : ScriptableObject
{
    public List<Localization> Localization = new List<Localization>();
}

[Serializable]
public class Localization
{
    public string Key;
    public string ENG;
    public string RUS;
}
