using UnityEngine;

/// <summary>
/// Данные для генерации шума
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

    [SerializeField] private NoiseType noiseType;
    public NoiseType NoiseType => noiseType;

    [SerializeField] private int octaves;
    public int Octaves => octaves;
    [SerializeField] private float persistence;
    public float Persistence => persistence;
    [SerializeField] private float lacunarity;
    public float Lacunarity => lacunarity;

    [SerializeField] private float scale;
    public float Scale { get => scale; private set => scale = value; }

    // public float[] ToNoiseMapArray() {
    //     return NoiseMapUtils.GenerateNoiseMap(width, height, seed, scale, octaves,
    //         persistence, lacunarity, offset);
    // }
}
