using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент, отображающий карту заданного шума на старте
/// </summary>
[RequireComponent(typeof(NoiseMapRenderer))]
public class NoiseMapRendererOnStart : MonoBehaviour
{
    [SerializeField]
    private NoiseMap noiseMap;
    [SerializeField] private NoiseMapRenderer.MapType type = NoiseMapRenderer.MapType.Noise;
    private NoiseMapRenderer noiseMapRenderer;

    private void Start() {
        noiseMapRenderer = GetComponent<NoiseMapRenderer>();

        float[] noiseMapArr = this.noiseMap.ToNoiseMapArray();
        noiseMapRenderer.RenderMap(noiseMap.Width, noiseMap.Height, noiseMapArr, type);
    }
}
