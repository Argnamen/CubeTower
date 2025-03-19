using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject
{
    public int cubeCount = 20;
    public Color32[] cubeColors;

    public List<Localization> Localization = new List<Localization>();
}

[Serializable]
public class Localization
{
    public string Key;
    public string ENG;
    public string RUS;
}
