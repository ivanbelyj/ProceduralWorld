using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Исходные данные, на основе которых генерируется мир
/// </summary>
[System.Serializable]
public class WorldData
{
    [SerializeField] private int seed;
    /// <summary>
    /// По ключу генерации (seed) можно однозначно восстановить бесконечный мир
    /// </summary>
    public int Seed { get => seed; private set => seed = value; }

    [SerializeField] private float chunkWidth = 1000f;
    public float ChunkWidth { get => chunkWidth; private set => chunkWidth = value; }
    [SerializeField] private float chunkLength = 1000f;
    public float ChunkLength { get => chunkLength; private set => chunkLength = value; }
    [SerializeField] private float chunkHeight = 600f;
    public float ChunkHeight { get => chunkHeight; private set => chunkHeight = value; }

    public WorldData(int seed) {
        this.seed = seed;
    }
}
