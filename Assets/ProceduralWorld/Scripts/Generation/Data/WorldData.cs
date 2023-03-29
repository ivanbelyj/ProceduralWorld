using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Исходные данные, которые должны быть учтены при генерации
/// </summary>
[System.Serializable]
public struct WorldData
{
    /// <summary>
    /// По ключу генерации (seed) можно однозначно восстановить бесконечный мир
    /// </summary>
    public int Seed { get; private set; }
    public WorldData(int seed) {
        Seed = seed;
    }
}
