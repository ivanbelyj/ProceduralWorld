using UnityEngine;

/// <summary>
/// Данные, позволяющие сгенерировать карту шума
/// </summary>
[System.Serializable]
public struct NoiseMap
{
    [SerializeField] private int width;
    public int Width => width;
    [SerializeField] private int height;
    public int Height => height;
    [SerializeField] private float scale;

    [SerializeField] private int octaves;
    [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;

    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;
    

    public float[] ToNoiseMapArray() {
        return NoiseMapUtils.GenerateNoiseMap(width, height, seed, scale, octaves,
            persistence, lacunarity, offset);
    }
}
