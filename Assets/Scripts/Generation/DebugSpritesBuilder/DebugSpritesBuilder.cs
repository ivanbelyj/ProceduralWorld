using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpritesBuilder : GenerationStage
{
    [SerializeField]
    private GameObject mapPrefab;

    [SerializeField]
    private BiomesScheme biomesScheme;

    [SerializeField]
    private uint biomeIdToDisplayMaskForTest;

    private GameObject noiseMapsParent;

    public override void Initialize(WorldGenerationData worldGenerationData)
    {
        base.Initialize(worldGenerationData);
        noiseMapsParent = new GameObject("NoiseMaps");
    }

    public override ChunkData ProcessChunk(ChunkData chunkData)
    {
        chunkData = base.ProcessChunk(chunkData);
        CreateSpriteMaps(chunkData);
        return chunkData;
    }

    private void CreateSpriteMaps(ChunkData chunkData) {
        List<(Color, float[,])> colorsAndMaps = new List<(Color, float[,])>() {
            (Color.white, chunkData.TerrainData
                .GetHeights(0, 0, worldData.ChunkResolution, worldData.ChunkResolution)),
            (Color.red, chunkData.Temperature),
            (Color.cyan, chunkData.Moisture),
            (Color.yellow, chunkData.Radiation),
            (Color.magenta, chunkData.Variety),
            
        };
        float offset = 0;
        const float OFFSET_STEP = 5f;

        // Отображение карты высот, температуры, влажности, радиации...
        foreach (var colorAndMap in colorsAndMaps) {
            Color[] colorMap = NoiseMapToTextureUtils.NoiseMapToColorMap(colorAndMap.Item2);
            CreateSpriteMap(chunkData, colorMap, NextOffset(), colorAndMap.Item1);
        }

        // Отображение карты биомов
        Color[] biomesColorMap = BiomesMapToColorMap(chunkData.BiomeIds, chunkData.Variety);
        CreateSpriteMap(chunkData, biomesColorMap, NextOffset());

        // Отображение маски одного из биомов (для теста)
        if (chunkData.BiomeMasksById.ContainsKey(biomeIdToDisplayMaskForTest)) {
            
            float[,] biomeMask = chunkData.BiomeMasksById[biomeIdToDisplayMaskForTest];
            
            Color[] biomeMaskColors = NoiseMapToTextureUtils.NoiseMapToColorMap(biomeMask);
            CreateSpriteMap(chunkData, biomeMaskColors, NextOffset(), Color.white);
        }

        float NextOffset() {
            return offset += OFFSET_STEP;
        }
    }

    private Color[] BiomesMapToColorMap(uint[,] biomesMap, float[,] variety) {
        int width = biomesMap.GetLength(1);
        int height = biomesMap.GetLength(0);
        Color[] res = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                Biome biome = biomesScheme.GetBiomeById(biomesMap[y, x]);
                // res[y * width + x] = Color.Lerp(Color.black, biome.GroupColor,
                //     variety[y, x]);
                res[y * width + x] = biome.GroupColor;
            }
        }
        return res;
    }

    private void CreateSpriteMap(ChunkData chunkData, Color[] colorMap, float zOffset = 0,
        Color color = default) {
        if (color == default) {
            color = Color.white;
        }
        
        Texture2D noiseTexture = NoiseMapToTextureUtils.ColorMapToTexture(
            worldData.ChunkResolution, worldData.ChunkResolution, colorMap);

        int mapSize = worldData.ChunkResolution;
        
        ChunkPosition cPos = chunkData.ChunkPosition;
        Vector3 pos = worldData.ChunkSize / 100f * new Vector3(cPos.X, cPos.Z, zOffset);
        
        GameObject spriteGO = Instantiate(mapPrefab, pos, Quaternion.identity);
        spriteGO.transform.SetParent(noiseMapsParent.transform);

        var noiseRenderer = spriteGO.GetComponent<NoiseMapRenderer>();
        
        noiseRenderer.RenderMap(mapSize, mapSize,
            colorMap);

        var spriteRenderer = spriteGO.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }
}
