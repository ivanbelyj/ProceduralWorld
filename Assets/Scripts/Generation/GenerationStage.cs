using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerationStage : MonoBehaviour, IGenerationStage
{
    protected WorldGenerationData worldData;
    public virtual void Initialize(WorldGenerationData worldGenerationData)
    {
        worldData = worldGenerationData;
    }

    public abstract ChunkData ProcessChunk(ChunkData chunkData);
}
