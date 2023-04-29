using UnityEngine;

/// <summary>
/// Данные для генерации шума
/// </summary>
[System.Serializable]
public struct NoiseData
{
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
}
