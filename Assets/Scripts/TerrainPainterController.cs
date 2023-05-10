using System.Collections;
using System.Collections.Generic;
using sc.terrain.proceduralpainter;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(TerrainPainter))]
public class TerrainPainterController : MonoBehaviour
{
    private TerrainPainter terrainPainter;
    
    private void Awake() {
        terrainPainter = GetComponent<TerrainPainter>();
    }

    public void AssignActiveTerrains() {
        terrainPainter.AssignActiveTerrains();
    }

    public void Repaint(Terrain terrain) {
        Debug.Log("Repaint terrain " + terrain.name);
        terrainPainter.RepaintTerrain(terrain);
    }

    public void RepaintAll() {
        terrainPainter.RepaintAll();
    }

    
}
