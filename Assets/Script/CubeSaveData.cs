using System.Collections.Generic;

[System.Serializable]
public class CubeSaveData
{
    public float PositionX;
    public float PositionY;
    public byte ColorR;
    public byte ColorG;
    public byte ColorB;
    public byte ColorA;
}

[System.Serializable]
public class TowerSaveData
{
    public List<CubeSaveData> Cubes = new List<CubeSaveData>();
}
