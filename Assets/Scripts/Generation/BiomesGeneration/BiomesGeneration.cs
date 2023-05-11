using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomesGeneration : GenerationStage
{
    private const int moistureSeedC = 1233;
    private const int temperatureSeedC = 7733;
    private const int radiationSeedC = 131377;
    private const int varietySeedC = 881231;

    /// <summary>
    /// Значение, на которое опускается температура на максимальной высоте
    /// (не учитывая изначальную карту температур без высот)
    /// </summary>
    private const float temperatureDecreasePerHeight = 0.4f;

    /// <summary>
    /// Коэффициент, определяющий то, насколько радиация будет рассеиваться в сухих биомах.
    /// 1, чтобы радиация развеивалась прямо пропорционально сухости
    /// (максимальная сухость развеивает максимальную радиацию)
    /// </summary>
    private const float radiationDissipationByDryness = 0.5f;

    [SerializeField]
    private BiomesManager biomesManager;

    [SerializeField]
    private NoiseData moistureNoise;

    [SerializeField]
    private NoiseData temperatureNoise;

    [SerializeField]
    private NoiseData radiationNoise;

    [SerializeField]
    private NoiseData varietyNoise;

    public override void Initialize(WorldGenerationData worldGenerationData)
    {
        base.Initialize(worldGenerationData);
        biomesManager.Initialize();
    }

    public override ChunkData ProcessChunk(ChunkData chunkData)
    {
        chunkData = base.ProcessChunk(chunkData);
        int heightsSize = worldData.ChunkResolution;

        var noiseOffset = new Vector2(chunkData.ChunkPosition.X * worldData.ChunkSize,
            chunkData.ChunkPosition.Z * worldData.ChunkSize);

        float[,] moisture = chunkData.Moisture = NoiseMapUtils.GenerateNoiseMap(moistureNoise,
            worldData.Seed * moistureSeedC,
            heightsSize, heightsSize, noiseOffset,
            worldData.WorldScale);

        // Temperature
        float[,] temperatureRandom = NoiseMapUtils.GenerateNoiseMap(temperatureNoise,
            worldData.Seed * temperatureSeedC,
            heightsSize, heightsSize, noiseOffset,
            worldData.WorldScale);
        float[,] temperatureOnHeights = chunkData.Temperature = TemperatureOnHeights(temperatureRandom,
            chunkData.TerrainData.GetHeights(0, 0, heightsSize, heightsSize));

        // Radiation
        float[,] radiationNotDissipated = NoiseMapUtils.GenerateNoiseMap(radiationNoise,
            worldData.Seed * radiationSeedC,
            heightsSize, heightsSize, noiseOffset,
            worldData.WorldScale);
        float[,] radiation = chunkData.Radiation
            = RadiationDissipatedByDryness(moisture, radiationNotDissipated);

        float[,] variety = chunkData.Variety = NoiseMapUtils.GenerateNoiseMap(varietyNoise,
            worldData.Seed * varietySeedC,
            heightsSize, heightsSize, noiseOffset,
            worldData.WorldScale);
        
        // id биомов, расположенных в соответствии с позициями чанка
        uint[,] biomes = new uint[heightsSize, heightsSize];
        for (int y = 0; y < heightsSize; y++) {
            for (int x = 0; x < heightsSize; x++) {
                biomes[y, x] = biomesManager.GetBiomeId(moisture[y, x], temperatureOnHeights[y, x],
                    radiation[y, x], variety[y, x]);
            }
        }

        chunkData.BiomeIds = biomes;

        return chunkData;
    }

    /// <summary>
    /// Карта температур, учитывающая также высоты
    /// </summary>
    private float[,] TemperatureOnHeights(float[,] temperature, float[,] heights) {
        int width = temperature.GetLength(1);
        int height = temperature.GetLength(0);
        float[,] res = new float[height, width];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                res[y, x] = temperature[y, x] - heights[y, x] * temperatureDecreasePerHeight;
                if (res[y, x] < 0)
                    res[y, x] = 0;
            }
        }
        return res;
    }

    /// <summary>
    /// Карта радиации, учитывающая, что радиация рассеивается в более сухих биомах
    /// </summary>
    private float[,] RadiationDissipatedByDryness(float[,] moisture, float[,] radiation) {
        int width = moisture.GetLength(1);
        int height = moisture.GetLength(0);
        float[,] res = new float[height, width];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float dryness = 1 - moisture[y, x];
                res[y, x] = radiation[y, x] - radiationDissipationByDryness * dryness;
                if (res[y, x] < 0)
                    res[y, x] = 0;
            }
        }
        return res;
    }
}
