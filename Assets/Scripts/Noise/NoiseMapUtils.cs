using UnityEngine;

/// <summary>
/// Класс, предоставляющий методы, генерирующие и обрабатывающие карту шума
/// </summary>
public static class NoiseMapUtils
{
    public static float[,] GenerateNoiseMap(
        NoiseData noiseData, int seed,
        int width, int height, Vector2 offset, float scaleMultiplier = 1f) {
        FastNoiseLite simplex = Simplex(noiseData, seed);
        // FastNoiseLite ridged = Ridged(noiseData, seed);

        float[,] noiseMap = new float[height, width];

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        // Проход по точкам карты высот
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Координаты для получения значения из шума
                float noiseX = (x - halfWidth + offset.x) * noiseData.Scale * scaleMultiplier;
                float noiseY = (y - halfHeight + offset.y) * noiseData.Scale * scaleMultiplier;

                float noiseVal = To01(simplex.GetNoise(noiseX, noiseY));
                noiseMap[y, x] = noiseVal * noiseVal;
            }
        }

        return noiseMap;
    }

    private static FastNoiseLite Cellular(NoiseData noiseData, int seed) {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        noise.SetSeed(seed);
        // noise.SetFrequency(noiseData.Scale * scaleMultiplier);
        noise.SetFrequency(0.01f);

        noise.SetFractalType(FastNoiseLite.FractalType.PingPong);
        noise.SetFractalOctaves(4);
        noise.SetFractalLacunarity(2);
        noise.SetFractalGain(0.9f);
        noise.SetFractalWeightedStrength(0.7f);
        noise.SetFractalPingPongStrength(3f);

        noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
        noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Add);
        noise.SetCellularJitter(1f);

        return noise;
    }

    private static FastNoiseLite Simplex(NoiseData noiseData, int seed) {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetSeed(seed);
        // noise.SetFrequency(noiseData.Scale * scaleMultiplier);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFractalOctaves(noiseData.Octaves);
        noise.SetFractalLacunarity(noiseData.Lacunarity);
        noise.SetFractalGain(noiseData.Persistence);
        return noise;
    }

    private static FastNoiseLite Ridged(NoiseData noiseData, int seed) {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetSeed(seed);
        // noise.SetFrequency(noiseData.Scale * scaleMultiplier);
        noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
        noise.SetFractalOctaves(noiseData.Octaves);
        noise.SetFractalLacunarity(noiseData.Lacunarity);
        noise.SetFractalGain(noiseData.Persistence);
        return noise;
    }

    private static FastNoiseLite SimplexLow(NoiseData noiseData, int seed) {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetSeed(seed);
        noise.SetFrequency(0.008f);
        // noise.SetFrequency(noiseData.Scale * scaleMultiplier);
        noise.SetFractalType(FastNoiseLite.FractalType.None);
        noise.SetFractalOctaves(1);
        noise.SetFractalLacunarity(noiseData.Lacunarity);
        noise.SetFractalGain(noiseData.Persistence);
        return noise;
    }

    // [-1; 1] -> [0; 1]
    private static float To01(float val) {
        return (val + 1) / 2;
    } 

    /// <summary>
    /// Генерирует карту шума с заданными параметрами
    /// </summary>
    public static float[,] GenerateNoiseMapOld(
        NoiseData noiseData, int seed,
        int width, int height, Vector2 offset, float scaleMultiplier = 1f)
    {
        int octaves = noiseData.Octaves;
        // Массив данных о вершинах
        float[,] noiseMap = new float[height, width];

        // Порождающий элемент
        System.Random rand = new System.Random(seed);

        // Сдвиги для каждой октавы
        Vector2[] octavesOffset = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            // Учитываем внешний сдвиг положения
            float xOffset = rand.Next(-100000, 100000) / width + offset.x;
            float yOffset = rand.Next(-100000, 100000) / height + offset.y;
            // octavesOffset[i] = new Vector2(xOffset / width, yOffset / height);
            octavesOffset[i] = new Vector2(xOffset, yOffset);
        }

        // if (scale < 0)
        // {
        //     scale = 0.0001f;
        // }

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        // Проход по точкам карты высот
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Значения для первой октавы
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                float superpositionCompensation = 0;

                // Обработка наложения октав
                for (int i = 0; i < octaves; i++)
                {
                    // Рассчитываем координаты для получения значения из шума
                    float xResult = (x - halfWidth) * frequency + octavesOffset[i].x * frequency;
                    float yResult = (y - halfHeight) * frequency + octavesOffset[i].y * frequency;

                    // Получение высоты из ГСПЧ
                    float generatedValue = 0;
                    switch (noiseData.NoiseType) {
                        case NoiseType.Perlin:
                            generatedValue = PerlinNoise.GetNoise(seed, xResult,
                                yResult, noiseData.Scale * scaleMultiplier);
                        break;
                        case NoiseType.Simplex:
                            generatedValue = SimplexNoise.GetNoise(xResult, yResult,
                                noiseData.Scale * scaleMultiplier);
                        break;
                    }
                    // Перераспределение
                    generatedValue = Mathf.Pow(generatedValue, 3f);

                    // Наложение октав
                    noiseHeight += generatedValue * amplitude;
                    // Компенсируем наложение октав, чтобы остаться в границах диапазона [0,1]
                    noiseHeight -= superpositionCompensation;

                    // Расчёт амплитуды, частоты и компенсации для следующей октавы
                    amplitude *= noiseData.Persistence;
                    frequency *= noiseData.Lacunarity;
                    superpositionCompensation = amplitude / 2;
                }

                // Сохраняем точку для карты высот
                // Из-за наложения октав есть вероятность выхода за границы диапазона [0,1]
                noiseMap[y, x] = Mathf.Clamp01(noiseHeight);
            }
        }

        return noiseMap;
    }
    
    // public static float[] GenerateNoiseMap(int width, int height, int seed, float scale,
    //     int octaves, float persistence, float lacunarity, Vector2 offset) {
    //     float[] noise = new float[width * height];
    //     for (int y = 0; y < height; y++)
    //     {
    //         for (int x = 0; x < width; x++)
    //         {
    //             noise[y * width + x] = PerlinNoise.GetNoise(seed, x + offset.x, y + offset.y, scale);
    //             // noise[y * width + x] = SimplexNoise.GetNoise(x + offset.x, y + offset.y, scale);
    //         }
    //     }
    //     return noise;
    // }
}
