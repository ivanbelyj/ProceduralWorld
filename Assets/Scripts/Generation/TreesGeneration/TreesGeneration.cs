using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreesGeneration : GenerationStage
{
    /// <summary>
    /// Значение, изменяющее сетку прохода по точкам чанка для расстановки деревьев.
    /// 1, чтобы проход осуществлялся по каждой точке чанка (ресурсозатратно)
    /// </summary>
    private const float treeDensityModifier = 0.4f;

    [SerializeField]
    private BiomesScheme biomesScheme;

    public override ChunkData ProcessChunk(ChunkData chunkData)
    {
        CreateTrees(chunkData);
        
        return chunkData;
    }

    private void CreateTrees(ChunkData chunkData) {
        TerrainData terrainData = chunkData.TerrainData;
        var prototypesAndInstances = CreateTreePrototypesAndInstances(worldData, chunkData);
                    // CreateTreeInstancesHalton(terrainData);
        terrainData.treePrototypes = prototypesAndInstances.Item1.ToArray();
        terrainData.SetTreeInstances(prototypesAndInstances.Item2.ToArray(), true);
    }

    // private List<TreeInstance> CreateTreeInstancesHalton(TerrainData terrainData) {
    //     List<TreeInstance> res = new List<TreeInstance>();

    //     var haltonX = new HaltonSequence(2);
    //     var haltonZ = new HaltonSequence(3);

    //     for (int i = 0; i < 7; i++)
    //     {
    //         float x = (float)haltonX.Next();
    //         float z = (float)haltonZ.Next();
    //         Vector3 position = new Vector3(x, 0, z);
    //         res.Add(CreateTreeInstance(position));
    //     }
    //     return res;
    // }

    private Tree SelectTree(Biome biome, float moisture, float radiation) {
        BiomeTree[] trees = biome.Trees;

        float totalPrevalence = trees.Sum(x => x.Prevalence);

        float randomNum = Random.Range(0, totalPrevalence);

        foreach (BiomeTree tree in trees) {
            randomNum -= tree.Prevalence;
            if (randomNum <= 0) {
                return tree.Tree;
            }
        }

        return null;
    }
    
    /// <param name="position">Локальная позиция дерева на Terrain, в диапазоне [0, 1]</param>
    private TreeInstance CreateTreeInstance(int prototypeIndex, Vector3 position,
        float worldScale) {
        TreeInstance treeInstance = new TreeInstance();

        treeInstance.position = position;

        treeInstance.prototypeIndex = prototypeIndex;
        treeInstance.widthScale = 1f / worldScale;
        treeInstance.heightScale = 1f / worldScale;
        treeInstance.color = Color.white;
        treeInstance.lightmapColor = Color.white;

        return treeInstance;
    }

    /// <summary>
    /// Вероятность возникновения дерева в точке заданного биома с заданной
    /// влажностью
    /// </summary>    
    private float TreeProbability(Biome biome, float moisture) {
        return Mathf.Clamp01(biome.TreesDensity * moisture);
        // Todo: разные виды деревьев и лесов по-разному воспринимают радиацию.
        // Кроме того, радиация может не только снижать количество деревьев, но,
        // например, влиять на распределение видов
    }

    private (List<TreePrototype>, List<TreeInstance>) CreateTreePrototypesAndInstances(
        WorldGenerationData worldData, ChunkData chunkData) {
        var instances = new List<TreeInstance>();
        var prototypes = new List<TreePrototype>();

        System.Random rnd = new System.Random();

        float chunkSize = worldData.ChunkResolution;

        // Проход осуществляется не по каждой точке чанка,
        // а по сетке со скорректированным масштабом

        // Половина ячейки сетки
        const float treeCellHalfSize = 1 / treeDensityModifier / 2;
        Debug.Log("Tree cell half size: " + treeCellHalfSize);

        // Деревья, которые были посажены в чанке и уже добавлены в результирующий
        // список прототипов, а также их будущие индексы прототипа (их много т.к. дерево
        // может быть представлено множеством моделей)
        Dictionary<Tree, int[]> treePrefabsInChunkAndProtIndexes
            = new Dictionary<Tree, int[]>();

        for (float x = 0; x < chunkSize; x += 1 / treeDensityModifier)
        {
            for (float z = 0; z < chunkSize; z += 1 / treeDensityModifier)
            {
                int biomeZ = Mathf.RoundToInt(z);
                int biomeX = Mathf.RoundToInt(x);
                Biome biome = biomesScheme.GetBiomeById(
                    chunkData.BiomeIds[biomeZ, biomeX]);

                float moisture = chunkData.Moisture[biomeZ, biomeX];
                float radiation = chunkData.Radiation[biomeZ, biomeX];
                
                float treeProbability = TreeProbability(biome, moisture);
                float fixingValue = (float)rnd.NextDouble();
                if (fixingValue < treeProbability)
                {
                    // === Вычисление позиции дерева ===
                    // Деревья размещаются в середине ячеек
                    Vector3 gridTreePos = new Vector3(x + treeCellHalfSize, 0,
                        z + treeCellHalfSize);

                    // Сетка обхода при сильной модификации выглядит слишком квадратно,
                    // поэтому каждое дерево смещается (максимум на длину одной ячейки)
                    Vector3 offset = new Vector3(Random.value, 0, Random.value).normalized
                        / treeDensityModifier;
                    Vector3 treePos = (gridTreePos + offset) / chunkSize;

                    // === Выбор дерева === 
                    Tree tree = SelectTree(biome, moisture, radiation);
                    // Но выбрать дерево недостаточно, нужно добавить прототип (если его еще не было)

                    // === Создание прототипа (если не создан) ===
                    if (tree != null && !treePrefabsInChunkAndProtIndexes.ContainsKey(tree)) {
                        int[] treeVariantsIndexes = new int[tree.TreePrefabs.Length];
                        
                        // Все варианты моделей дерева добавляются как прототипы
                        for (int i = 0; i < tree.TreePrefabs.Length; i++) {
                            // Добавление результата
                            prototypes.Add(new TreePrototype() {
                                prefab = tree.TreePrefabs[i]
                            });
                            int protIndex = prototypes.Count - 1;
                            // Индекс только что добавленного
                            treeVariantsIndexes[i] = protIndex;
                        }
                        // По Tree доступен список индексов прототипов его вариантов
                        treePrefabsInChunkAndProtIndexes.Add(tree, treeVariantsIndexes);
                    }

                    // === Создание TreeInstance ===
                    if (tree != null) {
                        int[] variantsProtIndexes = treePrefabsInChunkAndProtIndexes[tree];
                        int rndIndex = rnd.Next(variantsProtIndexes.Length);
                        var treeInstance = CreateTreeInstance(rndIndex, treePos, worldData.WorldScale);
                        instances.Add(treeInstance);
                    }
                }
                // Debug.Log($"Foliage. x: {x}, z: {z}");
            }
        }

        return (prototypes, instances);
    }
}
