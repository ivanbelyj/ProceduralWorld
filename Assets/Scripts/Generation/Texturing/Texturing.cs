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
        
        PaintTerrain(chunkData.Terrain);
        
        return chunkData;
    }

    private void PaintTerrain(Terrain terrain) {
        terrainPainterController.AssignActiveTerrains();
        terrainPainterController.Repaint(terrain);
    }
}
