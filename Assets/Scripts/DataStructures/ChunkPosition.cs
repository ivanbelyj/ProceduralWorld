using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Позиция чанка в бесконечном мире относительно начала мира (чанк (0, 0))
/// </summary>
public struct ChunkPosition
{

    public int X { get; private set; }
    public int Z { get; private set; }
    public ChunkPosition(int x, int z) {
        X = x;
        Z = z;
    }
    public override int GetHashCode()
    {
        return (X, Z).GetHashCode();
    }

    public ChunkPosition Top => new ChunkPosition(X, Z + 1);
    public ChunkPosition Right => new ChunkPosition(X + 1, Z);
    public ChunkPosition Bottom => new ChunkPosition(X, Z - 1);
    public ChunkPosition Left => new ChunkPosition(X - 1, Z);

    public override string ToString()
    {
        return $"{X}, {Z}";
    }
}
