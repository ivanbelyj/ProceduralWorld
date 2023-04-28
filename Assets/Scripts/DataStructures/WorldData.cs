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

    [SerializeField] private int chunkSize = 1000;
    public int ChunkSize { get => chunkSize; private set => chunkSize = value; }

    /// <summary>
    /// Размерность карты высот и зависимых от нее параметров (разрешение).
    /// Превосходит размер чанка на единицу, т.к. карта высот накладывается
    /// не на "квадраты", из которых состоят чанки, а на вершины сетки
    /// </summary>
    public int HeightsSize => ChunkSize + 1;
    [SerializeField] private int chunkHeight = 600;
    public int ChunkHeight { get => chunkHeight; private set => chunkHeight = value; }

    public WorldData(int seed) {
        this.seed = seed;
    }
}
