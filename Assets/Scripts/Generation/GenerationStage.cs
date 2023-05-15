using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GenerationStage : MonoBehaviour, IGenerationStage
{
    [SerializeField]
    protected IDispatcher dispatcher;

    [SerializeField]
    private bool includeInGeneration = true;
    public bool IncludeInGeneration {
        get => includeInGeneration;
        set => includeInGeneration = value;
    }

    private string stageName;

    public string StageName {
        get {
            if (stageName == null) {
                stageName = GetType().Name;
            }
            return stageName;
        }
    }

    protected WorldGenerationData worldData;
    /// <summary>
    /// Для обеспечения детерминированности генерации каждого чанка, для каждого из них
    /// используется собственный объект Random, детерминированно определяемый позицией
    /// и ключом генерации
    /// </summary>
    protected System.Random randomForCurrentChunk;
    
    public virtual void Initialize(WorldGenerationData worldGenerationData, IDispatcher dispatcher)
    {
        worldData = worldGenerationData;
        this.dispatcher = dispatcher;
    }

    public async virtual Task<ChunkData> ProcessChunk(ChunkData chunkData) {
        ChunkPosition cPos = chunkData.ChunkPosition;
        int seedForChunk = unchecked(((cPos.X << 16) | cPos.Z) * worldData.Seed);
        randomForCurrentChunk = new System.Random(seedForChunk);

        await dispatcher.Execute(() => Random.InitState(seedForChunk * 61));
        
        return chunkData;
    }
}
