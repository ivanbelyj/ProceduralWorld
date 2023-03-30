using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Создает тестовый ландшафт из нескольких чанков для демонстрации системы генерации
/// </summary>
public class TestChunkWorldBuilder : MonoBehaviour
{
    // Генерирует данные чанков
    private WorldGenerator worldGenerator;

    // Создает игровые объекты чанков на сцене
    [SerializeField]
    private WorldBuilder worldBuilder;

    // Это поле сериализуется, а значит, может задаваться в окне inspector
    [SerializeField]
    private WorldData worldData;

    // [SerializeField] private TerrainData initialTerrainData;

    [SerializeField] private int chunksRadius = 0;
    
    private void Awake()
    {
        worldGenerator = new WorldGenerator();

        worldGenerator.Initialize(worldData);
        worldBuilder.Initialize(worldData);
        
        // Создание мира n x n
        int n = chunksRadius - 1;
        for (int x = -n; x <= n; x++) {
            for (int y = -n; y <= n; y++) {
                GenerateAndCreateChunkGO(new ChunkPosition(x, y));
            }
        }
    }

    /// <summary>
    /// Генерирует чанк и создает на сцене GameObject
    /// </summary>
    private void GenerateAndCreateChunkGO(ChunkPosition pos) {
        ChunkData chunkData = worldGenerator.GenerateChunk(pos);
        worldBuilder.CreateChunkGO(chunkData);
    }
}
