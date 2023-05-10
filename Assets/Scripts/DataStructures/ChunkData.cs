using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    public TerrainData TerrainData { get; set; }
    public ChunkPosition ChunkPosition { get; set; }
    public float[,] Temperature { get; set; }
    public float[,] Moisture { get; set; }
    public float[,] Radiation { get; set; }
    public float[,] Variety { get; set; }
    public uint[,] BiomeIds { get; set; }

    /// <summary>
    /// Маски биомов чанка по id биомов. Используется, например, для текстурирования биомов
    /// </summary>
    public Dictionary<uint, float[,]> BiomeMasksById { get; set; }

    public Terrain Terrain { get; set; }

    public ChunkData() {
        
    }
}
