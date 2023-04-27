using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoliageGeneration : MonoBehaviour, IGenerationStage
{
    [SerializeField]
    private GameObject[] treePrefabs;

    public void Initialize() {
        
    }

    public ChunkData ProcessChunk(WorldData worldData, ChunkData chunkData)
    {
        var terrainData = chunkData.TerrainData;

        terrainData.treePrototypes = treePrefabs.Select(go => new TreePrototype() {
            prefab = go
        }).ToArray();

        var trees = CreateTreeInstances(terrainData);
        terrainData.SetTreeInstances(trees.ToArray(), true);
        
        return chunkData;
    }

    private List<TreeInstance> CreateTreeInstances(TerrainData terrainData) {
        var res = new List<TreeInstance>();
        
        for (float x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (float z = 0; z < terrainData.heightmapResolution; z++)
            {
                int r = UnityEngine.Random.Range(0, 500);
                if (r == 0)
                {
                    TreeInstance tree = new TreeInstance();

                    // Позиция локальная и находится в диапазоне [0, 1]
                    tree.position = new Vector3(x / terrainData.heightmapResolution,
                        0, z / terrainData.heightmapResolution);

                    tree.prototypeIndex = 0;
                    tree.widthScale = 1f;
                    tree.heightScale = 1f;
                    tree.color = Color.white;
                    tree.lightmapColor = Color.white;
                    res.Add(tree);
                }
            }
        }

        return res;
    }
}
