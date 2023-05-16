using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Предоставляет возможности генерации чанков пользователю
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    private bool showLogMessages;

    [SerializeField]
    [Tooltip("Максимальное количество миллисекунд, которое считается допустимым для генерации этапа. "
        + " При превышении во время генерации будет выводиться предупреждение")]
    private float maxNormalMsPerStage = 200;

    [SerializeField]
    private Dispatcher dispatcher;

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

    private WorldGenerationData worldData;
    private List<IGenerationStage> generationStages;

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

        if (showLogMessages)
            Debug.Log("Initializing WorldGenerator...");
        float startTime = GetTime();

        foreach(var stage in generationStages) {
            stage.Initialize(worldData, dispatcher);
        }
        if (showLogMessages)
            Debug.Log($"Initialized. Elapsed: { GetTime() - startTime} ms");
    }

    /// <summary>
    /// Генерирует данные чанка, расположенного по заданной позиции
    /// </summary>
    public async Task<ChunkData> CreateChunkAsync(ChunkPosition chunkPos) {
        if (generationStages.Count == 0)
            throw new System.InvalidOperationException(
                "Generation stages must be set before chunk generation");
        
        TerrainData terrainData = await dispatcher.Execute(
            () => CreateInitialTerrainData());
        ChunkData initialChunkData = new ChunkData() {
            ChunkPosition = chunkPos,
            TerrainData = terrainData
        };

        if (showLogMessages)
            Debug.Log($"Creating chunk {chunkPos}...");
        float totalStartTime = GetTime();

        ChunkData lastProcessed = initialChunkData;
        foreach (var stage in generationStages) {
            if (stage.IncludeInGeneration) {
                if (showLogMessages)
                    Debug.Log($"Stage {stage.StageName}");
                
                float startTime = GetTime();

                // lastProcessed = await Task.Run(() => stage.ProcessChunk(lastProcessed));
                lastProcessed = await stage.ProcessChunkAsync(lastProcessed);

                float elapsedMs = GetTime() - startTime;
                if (showLogMessages)
                    Debug.Log($"Stage {stage.StageName} completed. Elapsed: { elapsedMs } ms");

                if (elapsedMs > maxNormalMsPerStage) {
                    Debug.LogWarning($"Max normal time ({maxNormalMsPerStage})" + 
                        $" per generation stage is exceeded ({elapsedMs}) by { stage.StageName }");
                }
            }
        }

        if (showLogMessages)
            Debug.Log($"Chunk {chunkPos} created. Elapsed: { GetTime() - totalStartTime } ms");
            
        return lastProcessed;
    }

    // public async Task<ChunkData> CreateChunkAsync(ChunkPosition chunkPos) {
    //     return await Task.Run(() => CreateChunk(chunkPos));
    // }

    private float GetTime() {
        return Time.realtimeSinceStartup * 1000;
    }

    /// <summary>
    /// Создание изначальных данных ландшафта, которые далее будут обрабатываться
    /// </summary>
    private TerrainData CreateInitialTerrainData() {
        return new TerrainData();
    }
}
