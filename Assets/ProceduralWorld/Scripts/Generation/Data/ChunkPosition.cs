using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Позиция чанка в бесконечном мире относительно начала мира (чанк (0, 0))
/// </summary>
public struct ChunkPosition
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public ChunkPosition(int x, int y) {
        X = x;
        Y = y;
    }
}
