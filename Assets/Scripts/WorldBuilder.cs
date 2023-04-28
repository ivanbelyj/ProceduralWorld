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
        CreateSpriteMap(chunkData);

        return terrainGO;
    }

    private void CreateSpriteMap(ChunkData chunkData) {
        Color[] colorMap = NoiseMapToTextureUtils.NoiseMapToColorMap(
            chunkData.TerrainData.GetHeights(0, 0, worldData.HeightsSize, worldData.HeightsSize));
        Texture2D noiseTexture = NoiseMapToTextureUtils.ColorMapToTexture(
            worldData.HeightsSize, worldData.HeightsSize, colorMap);

        int mapSize = worldData.HeightsSize;
        
        // Магическое число... почему-то работает
        Vector3 pos = chunkData.ChunkPosition.ToVector2() * 0.65f;
        GameObject spriteGO = Instantiate(mapPrefab, pos, Quaternion.identity);
        spriteGO.transform.SetParent(noiseMapsParent.transform);

        var noiseRenderer = spriteGO.GetComponent<NoiseMapRenderer>();
        
        noiseRenderer.RenderMap(mapSize, mapSize,
            chunkData.TerrainData.GetHeights(0, 0, mapSize, mapSize),
            NoiseMapRenderer.MapType.Noise);
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
                Debug.Log("\tОбновление соседей для соседа. " + neighbor);
                UpdateNeighbors(neighbor, setForNeighbors: false);
            }
        }
    }

    private Terrain GetTerrainByPos(ChunkPosition cPos)
        => createdTerrains.ContainsKey(cPos) ? createdTerrains[cPos] : null;
}
