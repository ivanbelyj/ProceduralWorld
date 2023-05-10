using System.Collections;
using System.Collections.Generic;
using sc.terrain.proceduralpainter;
using UnityEngine;

public class Texturing : GenerationStage
{
    // Пока что код только для пробы
    [SerializeField]
    private uint testBiomeId;

    [SerializeField]
    private TerrainLayer testTerrainLayer;

    [SerializeField]
    private TerrainPainter terrainPainter;

    public override ChunkData ProcessChunk(ChunkData chunkData)
    {
        chunkData = base.ProcessChunk(chunkData);

        TerrainPainter newPainter = terrainPainter;
        
        // Если на чанке есть данный биом
        if (chunkData.BiomeMasksById.ContainsKey(testBiomeId)) {
            AddLayerSettings(newPainter, chunkData.BiomeMasksById[testBiomeId]);
        }
        
        PaintTerrain(newPainter, chunkData.Terrain);

        if (chunkData.BiomeMasksById.ContainsKey(testBiomeId)) {
            RemoveLayerSettings();
        }
        
        return chunkData;
    }

    private void PaintTerrain(TerrainPainter terrainPainter, Terrain terrain) {
        terrainPainter.SetTargetTerrains(new Terrain[] { terrain });
        terrainPainter.RepaintTerrain(terrain);
    }

    private void AddLayerSettings(TerrainPainter terrainPainter, float[,] biomeMask) {
        LayerSettings layerSettings = new LayerSettings() {
            enabled = true,
            layer = testTerrainLayer
        };

        // Создание текстуры маски биома
        int biomeMaskHeight = biomeMask.GetLength(0);
        int biomeMaskWidth = biomeMask.GetLength(1);

        Color[] biomeMaskColors = NoiseMapToTextureUtils.NoiseMapToColorMap(biomeMask);
        Texture2D biomeMaskTexture = NoiseMapToTextureUtils
            .ColorMapToTexture(biomeMaskWidth, biomeMaskHeight, biomeMaskColors);

        TextureMask textureMask = (TextureMask)ScriptableObject.CreateInstance("TextureMask");
        textureMask.enabled = true;
        textureMask.texture = biomeMaskTexture;

        // Добавление модификаторов
        layerSettings.modifierStack.Add(textureMask);

        terrainPainter.layerSettings.Insert(0, layerSettings);
    }

    private void RemoveLayerSettings() {
        terrainPainter.layerSettings.RemoveAt(0);
    }
}
