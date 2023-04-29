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

    public ChunkData() {
        
    }
}
