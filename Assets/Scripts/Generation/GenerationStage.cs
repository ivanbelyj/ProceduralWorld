using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerationStage : MonoBehaviour, IGenerationStage
{
    [SerializeField]
    private bool includeInGeneration = true;
    public bool IncludeInGeneration {
        get => includeInGeneration;
        set => includeInGeneration = value;
    }

    protected WorldGenerationData worldData;
    /// <summary>
    /// Для обеспечения детерминированности генерации каждого чанка, для каждого из них
    /// используется собственный объект Random, детерминированно определяемый позицией
    /// и ключом генерации
    /// </summary>
    protected System.Random randomForCurrentChunk;
    
    public virtual void Initialize(WorldGenerationData worldGenerationData)
    {
        worldData = worldGenerationData;
    }

    public virtual ChunkData ProcessChunk(ChunkData chunkData) {
        ChunkPosition cPos = chunkData.ChunkPosition;
        int seedForChunk = unchecked(((cPos.X << 16) | cPos.Z) * worldData.Seed);
        randomForCurrentChunk = new System.Random(seedForChunk);
        Random.InitState(seedForChunk * 61);
        
        return chunkData;
    }
}
