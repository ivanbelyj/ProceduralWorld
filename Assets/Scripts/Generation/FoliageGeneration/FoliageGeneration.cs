using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoliageGeneration : MonoBehaviour, IGenerationStage
{
    [SerializeField]
    private GameObject[] treePrefabs;

    [SerializeField]
    private BiomesScheme biomesScheme;

    private WorldGenerationData worldData;

    public void Initialize(WorldGenerationData worldGenerationData) {
        worldData = worldGenerationData;
    }

    public ChunkData ProcessChunk(ChunkData chunkData)
    {
        var terrainData = chunkData.TerrainData;

        terrainData.treePrototypes = treePrefabs.Select(go => new TreePrototype() {
            prefab = go
        }).ToArray();

        var trees = CreateTreeInstancesHalton(terrainData);
                    // CreateTreeInstancesRandom(worldData, chunkData);
        terrainData.SetTreeInstances(trees.ToArray(), true);
        
        return chunkData;
    }

    private List<TreeInstance> CreateTreeInstancesHalton(TerrainData terrainData) {
        List<TreeInstance> res = new List<TreeInstance>();

        var haltonX = new HaltonSequence(2);
        var haltonZ = new HaltonSequence(3);

        for (int i = 0; i < 7; i++)
        {
            float x = (float)haltonX.Next();
            float z = (float)haltonZ.Next();
            Vector3 position = new Vector3(x, 0, z);
            res.Add(CreateTreeInstance(position));
        }
        return res;
    }

    /// <param name="position">Локальная позиция дерева на Terrain, в диапазоне [0, 1]</param>
    private TreeInstance CreateTreeInstance(Vector3 position) {
        TreeInstance tree = new TreeInstance();

        tree.position = position;

        tree.prototypeIndex = 0;
        tree.widthScale = 1f;
        tree.heightScale = 1f;
        tree.color = Color.white;
        tree.lightmapColor = Color.white;

        return tree;
    }

    private List<TreeInstance> CreateTreeInstancesRandom(WorldGenerationData worldData, ChunkData chunkData) {
        var terrainData = chunkData.TerrainData;
        var res = new List<TreeInstance>();

        System.Random rnd = new System.Random();
        
        for (int x = 0; x < worldData.ChunkResolution; x++)
        {
            for (int z = 0; z < worldData.ChunkResolution; z++)
            {
                double fixingValue = rnd.NextDouble();
                if (fixingValue < biomesScheme.GetBiomeById(chunkData.BiomeIds[z, x]).TreesDensity)
                {
                    var tree = CreateTreeInstance(new Vector3(x / terrainData.heightmapResolution,
                        0, z / terrainData.heightmapResolution));
                    res.Add(tree);
                }
                Debug.Log($"Foliage. x: {x}, z: {z}");
            }
        }

        return res;
    }
}
