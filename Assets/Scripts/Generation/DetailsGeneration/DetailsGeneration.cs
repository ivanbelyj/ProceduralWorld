using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DetailsGeneration : GenerationStage
{
    private const int detailDensityMultiplier = 1;

    [SerializeField]
    private Texture2D grassTexture;

    protected async override Task<ChunkData> ProcessChunk(ChunkData chunkData)
    {
        chunkData.TerrainData.SetDetailResolution(worldData.ChunkSize * detailDensityMultiplier,
            worldData.ChunkSize * detailDensityMultiplier);
        
        await CreateDetails(chunkData);
        return chunkData;
    }

    private async Task CreateDetails(ChunkData chunkData) {
        TerrainData terrainData = chunkData.TerrainData;

        var grassDetailPrototype = new DetailPrototype() {
            prototypeTexture = grassTexture,
            
        };
        terrainData.detailPrototypes = new DetailPrototype[] {
            grassDetailPrototype
        };

        int detailResolution = terrainData.detailResolution;
        int detailLayers = terrainData.detailPrototypes.Length;

        int[,] detailValues = new int[detailResolution, detailResolution];

        await Task.Run(() => {
            // Todo: генерация растительности
            for (int y = 0; y < detailResolution; y++) {
                for (int x = 0; x < detailResolution; x++) {
                    detailValues[y, x] = 1;
                }
            }
        });

        terrainData.SetDetailLayer(0, 0, 0, detailValues);
    }
}
