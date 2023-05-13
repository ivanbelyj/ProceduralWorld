using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using sc.terrain.proceduralpainter;
using UnityEngine;

/// <summary>
/// Использует сторонний компонент ProceduralTerrainPainter и данные биомов,
/// чтобы текстурировать чанки соответственно биомам
/// </summary>
public class Texturing : GenerationStage
{
    [SerializeField]
    [Tooltip("Во сколько раз будет увеличена маска биомов для получения текстуры. "
        + "Чем больше, тем более плавными будут границы биомов, но тем ресурсозатратнее вычисления")]
    private int scaleFactor;

    [SerializeField]
    private TerrainPainter terrainPainter;

    [SerializeField]
    private BiomesManager biomesManager;

    private int layersAddedForCurrentChunk;

    public async override Task<ChunkData> ProcessChunk(ChunkData chunkData)
    {
        chunkData = await base.ProcessChunk(chunkData);

        // Добавление слоев, специфичных для биомов чанка
        foreach (var biomeIdAndMask in chunkData.BiomeMaskById) {
            Biome biome = biomesManager.GetBiomeById(biomeIdAndMask.Key);
            if (biome == null)
                Debug.LogError("Unknown biome");
            if (biome.LayerSettings == null || biome.LayerSettings.Length == 0)
                continue;

            float[,] interpolatedMask = InterpolateBiomeMask(biomeIdAndMask.Value);
            chunkData.InterpolatedBiomeMask = interpolatedMask;
            
            await AddBiomeLayerSettings(biome, await BiomeMaskToTexture2D(interpolatedMask));    
        }

        PaintTerrain(chunkData.Terrain);

        // К следующим чанкам специфичные слои биома не должны применяться
        if (layersAddedForCurrentChunk > 0)
            RemoveLayerSettings();
        
        return chunkData;
    }

    // Интерполяция и сглаживание маски биома для нормального вида в игре
    private float[,] InterpolateBiomeMask(float[,] biomeMask) {
        float[,] interpolatedMask = MatrixProcessingUtils.InterpolateBilinear(
            biomeMask, scaleFactor);
        // float[,] smoothedMask = MatrixProcessingUtils.BlurLinear(
        //     interpolatedMask, fadeWidth
        // );

        return biomeMask;
    }

    // Создание текстуры маски биома
    private async Task<Texture2D> BiomeMaskToTexture2D(float[,] biomeMask) {
        int biomeMaskHeight = biomeMask.GetLength(0);
        int biomeMaskWidth = biomeMask.GetLength(1);

        Color[] biomeMaskColors = NoiseMapToTextureUtils.NoiseMapToColorMap(biomeMask);
        Texture2D biomeMaskTexture = await dispatcher.Enqueue(() =>
            NoiseMapToTextureUtils.ColorMapToTexture(biomeMaskWidth, biomeMaskHeight, biomeMaskColors));

        return biomeMaskTexture;
    }

    private void PaintTerrain(Terrain terrain) {
        dispatcher.Enqueue(() => {
            terrainPainter.SetTargetTerrains(new Terrain[] { terrain });
            terrainPainter.RepaintAll();
        });
    }

    private async Task AddBiomeLayerSettings(Biome biome, Texture2D biomeMaskTexture) {
        foreach (LayerSettings layerSettings in biome.LayerSettings) {
            // Исходный объект не должен изменяться, поэтому создается новый
            LayerSettings newLayerSettings = new LayerSettings() {
                enabled = layerSettings.enabled,
                layer = layerSettings.layer,
                modifierStack = new List<Modifier>(layerSettings.modifierStack)
            };
            TextureMask textureMask = await dispatcher.Enqueue(() =>
                (TextureMask)ScriptableObject.CreateInstance("TextureMask"));
            textureMask.enabled = true;
            textureMask.texture = biomeMaskTexture;

            // В новый LayerSettings добавляется модификатор со сгенерированной в Runtime текстурой
            newLayerSettings.modifierStack.Add(textureMask);

            terrainPainter.layerSettings.Insert(0, newLayerSettings);
            layersAddedForCurrentChunk++;
        }
    }

    private void RemoveLayerSettings() {
        for (; layersAddedForCurrentChunk > 0; layersAddedForCurrentChunk--) {
            terrainPainter.layerSettings.RemoveAt(0);
        }
    }
}
