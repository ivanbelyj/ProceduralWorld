using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DetailsGeneration : GenerationStage
{
    private const int detailDensityMultiplier = 1;

    [SerializeField]
    private Texture2D grassTexture;

    public async override Task<ChunkData> ProcessChunk(ChunkData chunkData)
    {
        chunkData = await base.ProcessChunk(chunkData);
        
        await dispatcher.Execute(() => {
            chunkData.TerrainData.SetDetailResolution(worldData.ChunkSize * detailDensityMultiplier,
                worldData.ChunkSize * detailDensityMultiplier);
        });
        
        CreateDetails(chunkData);
        return chunkData;
    }

    private async void CreateDetails(ChunkData chunkData) {
        TerrainData terrainData = chunkData.TerrainData;

        var grassDetailPrototype = new DetailPrototype() {
            prototypeTexture = grassTexture,
            
        };
        await dispatcher.Execute(() => {
            terrainData.detailPrototypes = new DetailPrototype[] {
                grassDetailPrototype
            };
        });

        int detailResolution = await dispatcher.Execute(() => terrainData.detailResolution);
        int detailLayers = await dispatcher.Execute(() => terrainData.detailPrototypes.Length);

        int[,] detailValues = new int[detailResolution, detailResolution];

        for (int y = 0; y < detailResolution; y++) {
            for (int x = 0; x < detailResolution; x++) {
                detailValues[y, x] = 1;
            }
        }

        await dispatcher.Execute(() => {
            terrainData.SetDetailLayer(0, 0, 0, detailValues);
        });
    }
}
