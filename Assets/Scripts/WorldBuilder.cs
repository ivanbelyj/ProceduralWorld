using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Компонент, позволяющий создавать чанки на сцене
/// </summary>
public class WorldBuilder : MonoBehaviour
{
    private WorldGenerationData worldData;

    private Dictionary<ChunkPosition, Terrain> createdTerrains;

    [SerializeField]
    private TerrainPainterController terrainPainterController;

    [SerializeField]
    private GameObject mapPrefab;

    [SerializeField]
    private BiomesScheme biomesScheme;

    private GameObject chunksParent;
    private GameObject noiseMapsParent;

    public void Initialize(WorldGenerationData worldData) {
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

        CreateSpriteMaps(chunkData);

        return terrainGO;
    }

    private void CreateSpriteMaps(ChunkData chunkData) {
        List<(Color, float[,])> colorsAndMaps = new List<(Color, float[,])>() {
            (Color.white, chunkData.TerrainData
                .GetHeights(0, 0, worldData.ChunkResolution, worldData.ChunkResolution)),
            (Color.red, chunkData.Temperature),
            (Color.cyan, chunkData.Moisture),
            (Color.yellow, chunkData.Radiation),
            (Color.magenta, chunkData.Variety),
            
        };
        float offset = 0;
        const float OFFSET_STEP = 5f;
        foreach (var colorAndMap in colorsAndMaps) {
            Color[] colorMap = NoiseMapToTextureUtils.NoiseMapToColorMap(colorAndMap.Item2);
            CreateSpriteMap(chunkData, colorMap, offset, colorAndMap.Item1);
            offset += OFFSET_STEP;
        }

        Color[] biomesColorMap = BiomesMapToColorMap(chunkData.BiomeIds, chunkData.Variety);
        CreateSpriteMap(chunkData, biomesColorMap, offset + OFFSET_STEP);
    }

    private Color[] BiomesMapToColorMap(uint[,] biomesMap, float[,] variety) {
        int width = biomesMap.GetLength(1);
        int height = biomesMap.GetLength(0);
        Color[] res = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                Biome biome = biomesScheme.GetBiomeById(biomesMap[y, x]);
                res[y * width + x] = Color.Lerp(Color.black, biome.GroupColor,
                    variety[y, x]);
            }
        }
        return res;
    }

    private void CreateSpriteMap(ChunkData chunkData, Color[] colorMap, float zOffset = 0,
        Color color = default) {
        if (color == default) {
            color = Color.white;
        }
        
        Texture2D noiseTexture = NoiseMapToTextureUtils.ColorMapToTexture(
            worldData.ChunkResolution, worldData.ChunkResolution, colorMap);

        int mapSize = worldData.ChunkResolution;
        
        ChunkPosition cPos = chunkData.ChunkPosition;
        // Магическое число...
        Vector3 pos = 0.65f * new Vector3(cPos.X, cPos.Z, zOffset);
        
        GameObject spriteGO = Instantiate(mapPrefab, pos, Quaternion.identity);
        spriteGO.transform.SetParent(noiseMapsParent.transform);

        var noiseRenderer = spriteGO.GetComponent<NoiseMapRenderer>();
        
        noiseRenderer.RenderMap(mapSize, mapSize,
            colorMap);

        var spriteRenderer = spriteGO.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
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
