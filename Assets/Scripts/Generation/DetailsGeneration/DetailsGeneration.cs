using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DetailsGeneration : GenerationStage
{
    [SerializeField]
    [Tooltip("При значении 1 разрешение карты деталей будет соответстовать карте высот, "
        + "единица которой - метр. Большие значения увеличивают разрешение "
        + "и максимальную возможную плотность деталей, но снижают производительность")]
    private int detailDensityMultiplier = 2;

    [SerializeField]
    [Tooltip("В Unity существуют ограничения на максимальное возможное количество деталей на batch, "
        + "поэтому с повышением разрешения карты деталей некоторые из них могут не отображаться. "
        + "В таком случае можно уменьшить максимальное значение "
        + " detailResolutionPerPatch (128) в указанное число раз, однако это снижает производительность")]
    private int detailResolutionPerPatchDivider = 2;
    private const int MAX_DETAIL_RESOLUTION_PER_PATCH = 128;

    [SerializeField]
    private BiomesManager biomesManager;

    [SerializeField]
    private Texture2D grassTexture;

    [SerializeField]
    private NoiseData detailNoiseData;

    // Todo: зависимость от радиации, влажности, высоты, температуры

    protected async override Task<ChunkData> ProcessChunk(ChunkData chunkData)
    {
        int detailRes = worldData.ChunkSize * detailDensityMultiplier;
        chunkData.TerrainData.SetDetailResolution(detailRes,
            MAX_DETAIL_RESOLUTION_PER_PATCH / detailResolutionPerPatchDivider);
        
        await CreateDetails(chunkData);
        return chunkData;
    }

    private async Task CreateDetails(ChunkData chunkData) {
        TerrainData terrainData = chunkData.TerrainData;

        // Сбор всех деталей, которые встречаются в чанке
        var chunkDetails = new List<BiomeDetail>();
        foreach (uint biomeId in chunkData.ChunkBiomes) {
            Biome biome = biomesManager.GetBiomeById(biomeId);
            chunkDetails.AddRange(biome.BiomeDetails);
        }

        // Создание слоев
        int detailRes = chunkData.TerrainData.detailResolution;
        var layersForPrototypes = await Task.Run(() => GenerateDetailLayers(chunkData,
            chunkDetails, detailRes));
        terrainData.detailPrototypes = layersForPrototypes.Select(x => x.Item1).ToArray();

        // Назначение слоев
        for (int i = 0; i < layersForPrototypes.Count; i++) {
            terrainData.SetDetailLayer(0, 0, i, layersForPrototypes[i].Item2);
        }
    }

    /// <summary>
    /// Создает DetailPrototype и генерирует слой для каждого переданного BiomeDetail
    /// </summary>
    private List<(DetailPrototype, int[,])> GenerateDetailLayers(ChunkData chunkData,
        List<BiomeDetail> chunkDetails, int detailRes) {
        int cRes = worldData.ChunkResolution;

        // Инициализация результата (изначально все слои пусты)
        var res = new List<(DetailPrototype, int[,])>(chunkDetails.Count);
        for (int i = 0; i < chunkDetails.Count; i++) {
            // Todo: seed
            res.Add((chunkDetails[i].ToDetailPrototype(0), new int[detailRes, detailRes]));
        }

        // Некоторые детали размещаются на основе шума, поэтому для них генерируются шумы
        var noiseMapByDetail = new Dictionary<BiomeDetail, float[,]>();
        foreach (BiomeDetail detail in chunkDetails) {
            if (detail.placingMode == DetailsPlacingMode.Noise
                || detail.placingMode == DetailsPlacingMode.NoiseAndRandom) {
                Vector2 noiseOffset = new Vector2(worldData.ChunkSize
                    * chunkData.ChunkPosition.X, worldData.ChunkHeight * chunkData.ChunkPosition.Z);
                noiseMapByDetail.Add(detail, NoiseMapUtils.GenerateNoiseMap(detailNoiseData,
                    unchecked(worldData.Seed), detailRes, detailRes,
                    noiseOffset, worldData.WorldScale));
            }
        }

        // Для расстановки деталей только в соотв. биомах потребуется быстрая проверка,
        // относится ли деталь к данному биому
        var biomeDetailsByChunkId = new Dictionary<uint, HashSet<BiomeDetail>>(
            chunkData.ChunkBiomes.Count);
        foreach (uint chunkId in chunkData.ChunkBiomes) {
            biomeDetailsByChunkId[chunkId] = new HashSet<BiomeDetail>();
            foreach (BiomeDetail detail in biomesManager.GetBiomeById(chunkId).BiomeDetails) {
                biomeDetailsByChunkId[chunkId].Add(detail);
            }
        }

        const float EPS = 1e-8f;

        // Какие детали будут расположены в данной позиции
        for (int i = 0; i < chunkDetails.Count; i++) {
            BiomeDetail detail = chunkDetails[i];
            int step = 1;
            int maxIter = detailRes;
            for (int y = 0; y < maxIter; y += step) {
                for (int x = 0; x < maxIter; x += step) {
                    // Карта биомов размера cRes, приведение к размеру
                    int xBiome = (int)((float)x / detailRes * cRes);
                    int yBiome = (int)((float)y / detailRes * cRes);
                    uint biomeId = chunkData.BiomeIds[yBiome, xBiome];

                    // Если данная деталь не встречается в биоме, пропускаем
                    if (!biomeDetailsByChunkId[biomeId].Contains(detail))
                        continue;

                    Biome biome = biomesManager.GetBiomeById(biomeId);
                    
                    float density = 0f;
                    if (detail.placingMode == DetailsPlacingMode.Noise) {
                        density = noiseMapByDetail[detail][y, x];
                    } else if (detail.placingMode == DetailsPlacingMode.Random) {
                        density = (float)randomForCurrentChunk.NextDouble();
                    } else if (detail.placingMode == DetailsPlacingMode.NoiseAndRandom) {
                        density = noiseMapByDetail[detail][y, x]
                            * (float)randomForCurrentChunk.NextDouble();
                    } else {
                        Debug.LogError("Unknown detail placing mode");
                    }

                    float threshold = detail.threshold;
                    if (detail.placingMode == DetailsPlacingMode.Random) {
                        threshold /= (int)detail.rarity;
                    }

                    if (density > threshold) {
                        density = 0f;
                    }

                    // Значение в [0; threshold] приводится в [0, 1], сохраняя градацию
                    density /= threshold;

                    int densityInPos = Mathf.Abs(density) < EPS ? 0
                        : Mathf.CeilToInt(density * detail.densityMultiplier);
                    res[i].Item2[y, x] = densityInPos;
                }
            }
        }

        return res;
    }
}
