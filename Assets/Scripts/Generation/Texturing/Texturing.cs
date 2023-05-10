using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texturing : GenerationStage
{
    [SerializeField]
    private TerrainPainterController terrainPainterController;

    public override ChunkData ProcessChunk(ChunkData chunkData)
    {
        chunkData = base.ProcessChunk(chunkData);

        // TerrainPainter может быть выключен в редакторе,
        // чтобы после остановки игры не появлялась ошибка
        // MissingReferenceException самого ассета
        bool togglePainter = !terrainPainterController.gameObject.activeSelf;
        if (togglePainter)
            terrainPainterController.gameObject.SetActive(true);
        
        PaintTerrain(chunkData.Terrain);

        if (togglePainter)
            terrainPainterController.gameObject.SetActive(false);
        
        return chunkData;
    }

    private void PaintTerrain(Terrain terrain) {
        terrainPainterController.AssignActiveTerrains();
        terrainPainterController.Repaint(terrain);
    }
}
