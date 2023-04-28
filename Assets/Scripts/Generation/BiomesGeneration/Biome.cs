using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Данные о биоме
/// </summary>
[CreateAssetMenu(fileName = "New Biome", menuName = "Biome/Biome", order = 51)]
public class Biome : ScriptableObject
{
    [SerializeField]
    private byte biomeId;
    public byte BiomeId { get => biomeId; set => biomeId = value; }

    [SerializeField]
    private Color groupColor;
    /// <summary>
    /// Цвет, которым на классифицирующей схеме обозначается группа,
    /// к которой относится данный биом
    /// </summary>
    public Color GroupColor { get => groupColor; set => groupColor = value; }
}
