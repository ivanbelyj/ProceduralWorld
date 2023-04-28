using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomesGenerationStage : MonoBehaviour, IGenerationStage
{
    [SerializeField]
    private NoiseData moistureNoise;

    [SerializeField]
    private NoiseData temperatureNoise;
    public void Initialize()
    {
        
    }

    public ChunkData ProcessChunk(WorldData worldData, ChunkData chunkData)
    {
        return chunkData;
    }
}
