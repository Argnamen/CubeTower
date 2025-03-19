using System.Collections.Generic;
using UnityEngine;

public interface IGameConfig
{
    int CubeCount { get; }
    Color32[] CubeColors { get; }
}
