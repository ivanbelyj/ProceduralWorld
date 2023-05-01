using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailsGeneration : GenerationStage
{
    private const int detailDensityMultiplier = 1;

    [SerializeField]
    private Texture2D grassTexture;

    public override ChunkData ProcessChunk(ChunkData chunkData)
    {
        chunkData.TerrainData.SetDetailResolution(worldData.ChunkSize * detailDensityMultiplier,
            worldData.ChunkSize * detailDensityMultiplier);
        CreateDetails(chunkData);
        return chunkData;
    }

    private void CreateDetails(ChunkData chunkData) {
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

        for (int y = 0; y < detailResolution; y++) {
            for (int x = 0; x < detailResolution; x++) {
                detailValues[y, x] = 1;
            }
        }

        terrainData.SetDetailLayer(0, 0, 0, detailValues);
    }
}
