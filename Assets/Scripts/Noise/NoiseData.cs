using UnityEngine;

/// <summary>
/// Данные, позволяющие генерировать бесконечную карту шума
/// </summary>
[System.Serializable]
public struct NoiseData
{
    // [SerializeField] private int width;
    // public int Width => width;
    // [SerializeField] private int height;
    // public int Height => height;
    // [SerializeField] private float scale;
    // public float Scale => scale;
    // [SerializeField] private Vector2 offset;
    // public Vector2 Offset => offset;

    [SerializeField] private int octaves;
    public int Octaves => octaves;
    [SerializeField] private float persistence;
    public float Persistence => persistence;
    [SerializeField] private float lacunarity;
    public float Lacunarity => lacunarity;

    // [SerializeField] private int seed;
    // public int Seed => seed;

    // public float[] ToNoiseMapArray() {
    //     return NoiseMapUtils.GenerateNoiseMap(width, height, seed, scale, octaves,
    //         persistence, lacunarity, offset);
    // }
}
