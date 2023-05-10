using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Предоставляет возможности генерации чанков пользователю
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    private WorldGenerationData worldData;
    private List<IGenerationStage> generationStages;

    [SerializeField]
    private BaseTerrainGeneration baseTerrainGeneration;

    [SerializeField]
    private BiomesGeneration biomesGeneration;

    [SerializeField]
    private BiomesMasksGeneration biomesMasksGeneration;

    [SerializeField]
    private TreesGeneration treesGeneration;

    [SerializeField]
    private DetailsGeneration detailsGeneration;

    // Создает игровые объекты чанков на сцене
    [SerializeField]
    private WorldBuilder worldBuilder;

    [SerializeField]
    private Texturing texturing;

    [SerializeField]
    private DebugSpritesBuilder debugSpritesBuilder;

    /// <summary>
    /// Устанавливает исходные данные о мире перед тем, как генерировать чанки
    /// </summary>
    public void Initialize(WorldGenerationData wordData) {
        this.worldData = wordData;

        AddGenerationStages(worldData.Seed);
    }

    /// <summary>
    /// Добавление объектов, осуществляющих генерацию
    /// </summary>
    private void AddGenerationStages(int seed) {
        generationStages = new List<IGenerationStage>();

        generationStages.Add(baseTerrainGeneration);
        generationStages.Add(biomesGeneration);
        generationStages.Add(biomesMasksGeneration);
        generationStages.Add(treesGeneration);
        generationStages.Add(detailsGeneration);

        generationStages.Add(worldBuilder);
        generationStages.Add(texturing);
        generationStages.Add(debugSpritesBuilder);

        foreach(var stage in generationStages) {
            stage.Initialize(worldData);
        }
    }

    /// <summary>
    /// Генерирует данные чанка, расположенного по заданной позиции
    /// </summary>
    public ChunkData CreateChunk(ChunkPosition chunkPos) {
        if (generationStages.Count == 0)
            throw new System.InvalidOperationException(
                "Generation stages must be set before chunk generation");
        
        var terrainData = CreateInitialTerrainData();
        ChunkData initialChunkData = new ChunkData() {
            ChunkPosition = chunkPos,
            TerrainData = terrainData
        };

        ChunkData lastProcessed = generationStages[0].ProcessChunk(initialChunkData);
        foreach (var stage in generationStages) {
            if (stage.IncludeInGeneration)
                lastProcessed = stage.ProcessChunk(lastProcessed);
        }
        return lastProcessed;
    }

    /// <summary>
    /// Создание изначальных данных ландшафта, которые далее будут обрабатываться
    /// </summary>
    private TerrainData CreateInitialTerrainData() {
        return new TerrainData();
    }
}
