using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject, IGameConfig
{
    public int cubeCount = 20;
    public Color32[] cubeColors;
    public List<Localization> Localization = new List<Localization>();

    int IGameConfig.CubeCount => cubeCount;
    Color32[] IGameConfig.CubeColors => cubeColors;
    List<Localization> IGameConfig.Localization => Localization;
}

[Serializable]
public class Localization
{
    public string Key;
    public string ENG;
    public string RUS;
}
