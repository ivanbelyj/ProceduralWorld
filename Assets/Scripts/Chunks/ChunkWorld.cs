using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Создает тестовый ландшафт из нескольких чанков для демонстрации системы генерации
/// </summary>
public class ChunkWorld : MonoBehaviour
{
    // Генерирует данные чанков
    [SerializeField]
    private WorldGenerator worldGenerator;

    // Параметры генерации мира
    [SerializeField]
    private WorldGenerationData worldGenerationData;

    [SerializeField]
    [Tooltip("Радиус активных чанков вокруг точек присутствия. 1 - активен только тот чанк, "
        + "в котором находится точка присутствия")]
    private int activeChunksRadius = 1;

    /// <summary>
    /// Позволяет получать данные созданных чанков из стороннего кода
    /// </summary>
    [HideInInspector]
    public Dictionary<ChunkPosition, ChunkData> CreatedChunkByPosition { get; set; }

    /// <summary>
    /// Позиции, на которых чанки активированы
    /// </summary>
    [HideInInspector]
    public HashSet<ChunkPosition> ActiveChunks { get; set; }

    [HideInInspector]
    public event Action<ChunkData> ChunkCreated;

    private List<ChunkPosition> pointsOfPresence;
    
    private void Awake()
    {
        CreatedChunkByPosition = new Dictionary<ChunkPosition, ChunkData>();
        ActiveChunks = new HashSet<ChunkPosition>();
        pointsOfPresence = new List<ChunkPosition>();

        worldGenerator.Initialize(worldGenerationData);
    }

    /// <summary>
    /// По мировой координате определяет чанк, в котором находится данная позиция
    /// </summary>
    public ChunkPosition WorldPosToChunkPos(Vector3 pos) {
        // .. .v. .. .. |
        // -5.5 / 2 = -2.75 => ceil() => -2
        int x = GetChunkPos(pos.x);
        int z = GetChunkPos(pos.z);
        return new ChunkPosition(x, z);

        int GetChunkPos(float pos) {
            return (int)(Mathf.Ceil(Mathf.Abs(pos / worldGenerationData.ChunkSize))
                * Mathf.Sign(pos));
        }
    }

    /// <param name="offset">По умолчанию применяется отступ, перемещающий позицию в центр чанка</param>
    public Vector3 PosInActiveChunkToWorldPos(ChunkPosition chunkPos, Vector3 offset = default) {
        if (!ActiveChunks.Contains(chunkPos)) {
            Debug.LogError("Trying to convert pos in unactive chunk to world pos."
                + " Height can not be defined.");
        }

        if (offset == default) {
            float halfChunk = worldGenerationData.ChunkSize / 2f;
            offset = new Vector3(halfChunk, 0, halfChunk);
        }
        
        ChunkData chunkData = CreatedChunkByPosition[chunkPos];
        Vector3 worldPos = new Vector3(GetWorldPos(chunkPos.X) + offset.x,
            0, GetWorldPos(chunkPos.Z) + offset.z);
        worldPos.y = chunkData.Terrain.SampleHeight(worldPos);

        return worldPos;

        float GetWorldPos(int pos) {
            return pos * worldGenerationData.ChunkSize;
        }
    }

    private void SetChunkActive(ChunkPosition chunkPosition, bool isActive) {
        CreatedChunkByPosition[chunkPosition].Terrain.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Генерирует чанк и создает его на сцене
    /// </summary>
    private void GenerateAndCreateChunkGO(ChunkPosition pos) {
        ChunkData chunkData = worldGenerator.CreateChunk(pos);
        CreatedChunkByPosition.Add(chunkData.ChunkPosition, chunkData);
        ChunkCreated?.Invoke(chunkData);
    }

    #region Points Of Presence
    public void AddPointOfPresence(ChunkPosition chunkPos) {
        pointsOfPresence.Add(chunkPos);
        for (int x = chunkPos.X - activeChunksRadius + 1; x <= chunkPos.X + activeChunksRadius - 1; x++) {
            for (int z = chunkPos.Z - activeChunksRadius + 1; z <= chunkPos.Z + activeChunksRadius - 1; z++) {
                ChunkPosition pos = new ChunkPosition(x, z);
                // Если чанк еще не сгенерирован
                if (!CreatedChunkByPosition.ContainsKey(pos)) {
                    GenerateAndCreateChunkGO(pos);
                }
                // Если создан, но неактивен
                else if (!ActiveChunks.Contains(pos)) {
                    SetChunkActive(pos, true);
                }
                ActiveChunks.Add(pos);
            }
        }
    }
    public void RemovePointOfPresence(ChunkPosition chunkPos) {
        pointsOfPresence.Remove(chunkPos);
        HashSet<ChunkPosition> chunksToDeactivate = new HashSet<ChunkPosition>();
        for (int x = chunkPos.X - activeChunksRadius + 1; x <= chunkPos.X + activeChunksRadius - 1; x++) {
            for (int z = chunkPos.Z - activeChunksRadius + 1; z <= chunkPos.Z + activeChunksRadius - 1; z++) {
                ChunkPosition pos = new ChunkPosition(x, z);
                // Если чанк создан и активен
                if (CreatedChunkByPosition.ContainsKey(pos) && ActiveChunks.Contains(pos)
                    // Метод RemovePointOfPresence может быть вызван при уничтожении GameObject
                    // с компонентом PresentInChunk, а уничтожение может происходить в том числе
                    // при закрытии сцены. При закрытии сцены Terrain могут быть уже уничтожены,
                    // поэтому необходимо проверять, не уничтожены ли они, чтобы не пытаться
                    // отключать уничтоженный объект
                    && CreatedChunkByPosition[pos].Terrain != null) {
                    SetChunkActive(pos, false);
                    ActiveChunks.Remove(pos);
                    if (pointsOfPresence.All(p => Vector3.Distance(new Vector3(p.X, 0, p.Z),
                        new Vector3(pos.X, 0, pos.Z)) >= activeChunksRadius)) {
                        chunksToDeactivate.Add(pos);
                    }
                }
            }
        }
        foreach (ChunkPosition pos in chunksToDeactivate) {
            // CreatedChunkByPosition.Remove(pos);
            SetChunkActive(pos, false);
            ActiveChunks.Remove(pos);
            // Destroy(CreatedChunkByPosition[pos].Terrain.gameObject);
        }
    }
    public void UpdatePointOfPresence(ChunkPosition chunkPosOld, ChunkPosition chunkPosNew) {
        RemovePointOfPresence(chunkPosOld);
        AddPointOfPresence(chunkPosNew);
    }
    #endregion
}
