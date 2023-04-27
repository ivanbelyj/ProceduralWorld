using UnityEngine;

/// <summary>
/// Класс, предоставляющий методы, генерирующие и обрабатывающие карту шума
/// </summary>
public static class NoiseMapUtils
{
    /// <summary>
    /// Генерирует карту шума с заданными параметрами
    /// </summary>
    public static float[] GenerateNoiseMap(int width, int height, int seed, float scale,
        int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        // Массив данных о вершинах
        float[] noiseMap = new float[width * height];

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
                    // Рассчитываем координаты для получения значения из Шума Перлина
                    float xResult = (x - halfWidth) * frequency + octavesOffset[i].x * frequency;
                    float yResult = (y - halfHeight) * frequency + octavesOffset[i].y * frequency;

                    // Получение высоты из ГСПЧ
                    float generatedValue = SimplexNoise.GetNoise(xResult, yResult, scale);
                    // float generatedValue = PerlinNoise.GetNoise(seed, xResult, yResult, scale);

                    // Наложение октав
                    noiseHeight += generatedValue * amplitude;
                    // Компенсируем наложение октав, чтобы остаться в границах диапазона [0,1]
                    noiseHeight -= superpositionCompensation;

                    // Расчёт амплитуды, частоты и компенсации для следующей октавы
                    amplitude *= persistence;
                    frequency *= lacunarity;
                    superpositionCompensation = amplitude / 2;
                }

                // Сохраняем точку для карты высот
                // Из-за наложения октав есть вероятность выхода за границы диапазона [0,1]
                noiseMap[y * width + x] = Mathf.Clamp01(noiseHeight);
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