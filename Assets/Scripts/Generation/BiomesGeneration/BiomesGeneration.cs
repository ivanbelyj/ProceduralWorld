using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomesGeneration : MonoBehaviour, IGenerationStage
{
    private int moistureSeedC = 1233;
    private int temperatureSeedC = 7733;

    [SerializeField]
    private BiomesScheme biomesScheme;

    [SerializeField]
    private NoiseData moistureNoise;

    [SerializeField]
    private NoiseData temperatureNoise;

    public void Initialize()
    {
        biomesScheme.Initialize();
    }

    public ChunkData ProcessChunk(WorldData worldData, ChunkData chunkData)
    {
        int heightsSize = worldData.HeightsSize;

        var noiseOffset = new Vector2(chunkData.ChunkPosition.X * worldData.ChunkSize,
            chunkData.ChunkPosition.Z * worldData.ChunkSize);

        float[,] moisture = NoiseMapUtils.GenerateNoiseMap(moistureNoise,
            worldData.Seed * moistureSeedC,
            heightsSize, heightsSize, noiseOffset);

        float[,] temperature = NoiseMapUtils.GenerateNoiseMap(temperatureNoise,
            worldData.Seed * temperatureSeedC,
            heightsSize, heightsSize, noiseOffset);
        
        // id биомов, расположенных в соответствии с позициями чанка
        uint[,] biomes = new uint[heightsSize, heightsSize];
        for (int y = 0; y < heightsSize; y++) {
            for (int x = 0; x < heightsSize; x++) {
                biomes[y, x] = biomesScheme.GetBiomeId(moisture[y, x], temperature[y, x]);
            }
        }
        chunkData.BiomeIds = biomes;
        
        return chunkData;
    }

}
