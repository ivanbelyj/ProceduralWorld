using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomesMasksGeneration : GenerationStage
{
    public override ChunkData ProcessChunk(ChunkData chunkData)
    {
        chunkData = base.ProcessChunk(chunkData);
        chunkData.BiomeMaskById = GetBiomeMasksForIds(chunkData.BiomeIds);
        return chunkData;
    }

    private Dictionary<uint, float[,]> GetBiomeMasksForIds(uint[,] biomeIds) {
        var res = new Dictionary<uint, float[,]>();

        int height = biomeIds.GetLength(0);
        int width = biomeIds.GetLength(1);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                uint biomeId = biomeIds[y, x];

                float[,] biomeMask;
                // Если карты биомов еще нет, добавляем
                if (!res.ContainsKey(biomeId)) {
                    biomeMask = new float[height, width];
                    res.Add(biomeId, biomeMask);
                } else {
                    // Иначе просто получаем карту, которую уже добавили
                    biomeMask = res[biomeId];
                }

                biomeMask[y, x] = 1f;
            }
        }
        return res;
    }
}
