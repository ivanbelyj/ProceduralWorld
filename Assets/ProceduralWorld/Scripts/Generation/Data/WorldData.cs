using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Данные о бесконечном процедурно генерируемом мире
/// </summary>
[System.Serializable]
public class WorldData
{
    [SerializeField] private int seed;
    /// <summary>
    /// Каждому ключу генерации сопоставляется уникальный мир,
    /// который можно по нему однозначно восстановить
    /// </summary>
    public int Seed { get => seed; private set => seed = value; }

    [SerializeField] private NoiseData noiseData;
    /// <summary>
    /// Параметры шума, лежащего в основе ландшафта
    /// </summary>
    public NoiseData NoiseData => noiseData;

    [SerializeField] private int chunkSize = 1000;
    public int ChunkSize { get => chunkSize; private set => chunkSize = value; }
    [SerializeField] private int chunkHeight = 600;
    public int ChunkHeight { get => chunkHeight; private set => chunkHeight = value; }

    [SerializeField] private float scale = 100f;
    public float Scale { get => scale; private set => scale = value; }

    public WorldData(int seed) {
        this.seed = seed;
    }
}
