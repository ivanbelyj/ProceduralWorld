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
    public override int GetHashCode()
    {
        return (X, Y).GetHashCode();
    }

    public ChunkPosition Top => new ChunkPosition(X, Y + 1);
    public ChunkPosition Right => new ChunkPosition(X + 1, Y);
    public ChunkPosition Bottom => new ChunkPosition(X, Y - 1);
    public ChunkPosition Left => new ChunkPosition(X - 1, Y);

    public override string ToString()
    {
        return $"{X}, {Y}";
    }
}
