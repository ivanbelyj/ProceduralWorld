using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Точка присутствия, вокруг которой активируются чанки в определенном радиусе.
/// В начале игры инициирует создание области чанков 
/// </summary>
public class PresentInChunk : MonoBehaviour
{
    [SerializeField]
    private ChunkWorld chunkWorld;

    [SerializeField]
    [Tooltip("Позиция чанка, в котором изначально находится точка присутствия")]
    private ChunkPosition initialChunkPosition;

    /// <summary>
    /// Последняя позиция присутствия, для которой были созданы / скрыты чанки
    /// </summary>
    private ChunkPosition lastChunkPosition;

    private void Start() {
        chunkWorld.AddPointOfPresence(initialChunkPosition);
        // Перемещение в центр чанка
        transform.position = chunkWorld.PosInActiveChunkToWorldPos(initialChunkPosition);
    }

    private void OnDestroy() {
        chunkWorld.RemovePointOfPresence(GetCurrentChunkPosition());
    }

    private ChunkPosition GetCurrentChunkPosition() {
        return chunkWorld.WorldPosToChunkPos(transform.position);
    }

    private void Update() {
        ChunkPosition currentPos = GetCurrentChunkPosition();
        if (currentPos != lastChunkPosition) {
            chunkWorld.UpdatePointOfPresence(lastChunkPosition, currentPos);
        }
        lastChunkPosition = currentPos;
    }
}
