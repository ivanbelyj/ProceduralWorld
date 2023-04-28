using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Компонен, позволяющий создавать чанки на сцене
/// </summary>
public class WorldBuilder : MonoBehaviour
{
    private WorldData worldData;

    private Dictionary<ChunkPosition, Terrain> createdTerrains;

    [SerializeField]
    private TerrainPainterController terrainPainterController;

    [SerializeField]
    private GameObject mapPrefab;

    [SerializeField]
    private BiomesScheme biomesScheme;

    private GameObject chunksParent;
    private GameObject noiseMapsParent;

    public void Initialize(WorldData worldData) {
        this.worldData = worldData;
        createdTerrains = new Dictionary<ChunkPosition, Terrain>();

        chunksParent = new GameObject("Chunks");
        noiseMapsParent = new GameObject("NoiseMaps");
    }

    /// <summary>
    /// На основе ChunkData создает игровой объект чанка на сцене и возвращает его
    /// </summary>
    public GameObject CreateChunkGO(ChunkData chunkData) {
        GameObject terrainGO = Terrain.CreateTerrainGameObject(chunkData.TerrainData);
        terrainGO.transform.SetParent(chunksParent.transform);
        terrainGO.transform.position = new Vector3(worldData.ChunkSize * chunkData.ChunkPosition.X, 0,
            worldData.ChunkSize * chunkData.ChunkPosition.Z);

        var terrain = terrainGO.GetComponent<Terrain>();
        createdTerrains[chunkData.ChunkPosition] = terrain;
        
        terrain.treeBillboardDistance = 1000;
        UpdateNeighbors(chunkData.ChunkPosition);
        PaintTerrain(terrain);

        Color[] heightsColorMap = NoiseMapToTextureUtils.NoiseMapToColorMap(chunkData.TerrainData
            .GetHeights(0, 0, worldData.HeightsSize, worldData.HeightsSize));
        CreateSpriteMap(chunkData, heightsColorMap);

        Color[] biomesColorMap = BiomesMapToColorMap(chunkData.BiomeIds);
        CreateSpriteMap(chunkData, biomesColorMap, 5);

        return terrainGO;
    }

    private Color[] BiomesMapToColorMap(uint[,] biomesMap) {
        int width = biomesMap.GetLength(1);
        int height = biomesMap.GetLength(0);
        Color[] res = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                res[y * width + x] = biomesScheme.GetBiomeById(biomesMap[y, x]).GroupColor;
            }
        }
        return res;
    }

    private void CreateSpriteMap(ChunkData chunkData, Color[] colorMap, float zOffset = 0) {
        // Color[] colorMap = NoiseMapToTextureUtils.NoiseMapToColorMap(
        //     chunkData.TerrainData.GetHeights(0, 0, worldData.HeightsSize, worldData.HeightsSize));
        Texture2D noiseTexture = NoiseMapToTextureUtils.ColorMapToTexture(
            worldData.HeightsSize, worldData.HeightsSize, colorMap);

        int mapSize = worldData.HeightsSize;
        
        ChunkPosition cPos = chunkData.ChunkPosition;
        // Магическое число... почему-то работает
        Vector3 pos = 0.65f * new Vector3(cPos.X, cPos.Z, zOffset);
        
        GameObject spriteGO = Instantiate(mapPrefab, pos, Quaternion.identity);
        spriteGO.transform.SetParent(noiseMapsParent.transform);

        var noiseRenderer = spriteGO.GetComponent<NoiseMapRenderer>();
        
        noiseRenderer.RenderMap(mapSize, mapSize,
            colorMap);
    }

    private void PaintTerrain(Terrain terrain) {
        terrainPainterController.AssignActiveTerrains();
        terrainPainterController.Repaint(terrain);
    }

    /// <summary>
    /// Обновляет существующие соседние Terrain для созданного Terrain'а по заданной позиции.
    /// setForNeighbors: true, если требуется обновить соседей не только самого элемента,
    /// но и его соседей
    /// </summary>
    private void UpdateNeighbors(ChunkPosition cPos, bool setForNeighbors = true) {
        if (setForNeighbors)
            Debug.Log("Обновление соседей для " + cPos);
        // Установка соседей
        Terrain terrain = createdTerrains[cPos];
        
        Terrain top = GetTerrainByPos(cPos.Top);
        Terrain right = GetTerrainByPos(cPos.Right);
        Terrain bottom = GetTerrainByPos(cPos.Bottom);
        Terrain left = GetTerrainByPos(cPos.Left);

        terrain.SetNeighbors(left, top, right, bottom);

        if (setForNeighbors) {
            foreach (var neighbor in new [] { cPos.Top, cPos.Right, cPos.Bottom, cPos.Left }
                .Where(pos => createdTerrains.ContainsKey(pos)) ) {
                UpdateNeighbors(neighbor, setForNeighbors: false);
            }
        }
    }

    private Terrain GetTerrainByPos(ChunkPosition cPos)
        => createdTerrains.ContainsKey(cPos) ? createdTerrains[cPos] : null;
}
