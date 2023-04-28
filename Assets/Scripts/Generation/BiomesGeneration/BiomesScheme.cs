using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Имеет доступ к списку существующих биомов и классифицирующей схеме, позволяет
/// классифицировать тип биома по передаваемым параметрам
/// </summary>
public class BiomesScheme : MonoBehaviour
{
    [SerializeField]
    private Texture2D biomeSchemeImage;  // Изображение с матрицей классификации биомов
    [SerializeField]
    private Biome[] biomes;
    
    private Dictionary<Color, BiomesGroup> biomeGroupByColor;  // Соответствия цветов и биомов
    private Dictionary<uint, Biome> biomeById;

    private Color[] biomeMapColors;  // Массив цветов изображения с матрицей биомов
    private int width;  // Ширина матрицы биомов
    private int height;  // Высота матрицы биомов

    public void Initialize()
    {
        // Загрузка цветов изображения-схемы биомов
        biomeMapColors = biomeSchemeImage.GetPixels();
        width = biomeSchemeImage.width;
        height = biomeSchemeImage.height;

        biomeGroupByColor = new Dictionary<Color, BiomesGroup>();
        biomeById = new Dictionary<uint, Biome>();
        foreach (Biome biome in biomes) {
            if (!biomeGroupByColor.ContainsKey(biome.GroupColor)) {
                biomeGroupByColor.Add(biome.GroupColor, new BiomesGroup());
            }
            biomeGroupByColor[biome.GroupColor].Biomes.Add(biome);

            biomeById.Add(biome.BiomeId, biome);
        }
    }

    public Biome GetBiomeById(uint id) {
        return biomeById[id];
    }

    public uint GetBiomeId(float moisture, float temperature)
    {
        // Определение позиции в матрице биомов на основе влажности и температуры
        int x = Mathf.FloorToInt(moisture * (width - 1));
        int y = Mathf.FloorToInt(temperature * (height - 1));

        // Определение цвета в позиции x, y в матрице биомов
        Color color = biomeMapColors[y * width + x];

        // Определение типа биома на основе цвета
        if (biomeGroupByColor.ContainsKey(color))
        {
            // Todo: учет дополнительных параметров, например, радиации
            return biomeGroupByColor[color].Biomes[0].BiomeId;
        }
        else
        {
            Debug.LogError("Unknown biome color: " + color);
            return 0;
        }
    }
}

