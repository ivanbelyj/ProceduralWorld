using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Данные о виде деревьев в рамках биома
/// </summary>
[System.Serializable]
public class BiomeTree
{
    [SerializeField]
    private GameObject treePrefab;
    public GameObject TreePrefab { get => treePrefab; set => treePrefab = value; }

    [SerializeField]
    private float prevalence;
    /// <summary>
    /// Распространенность дерева, учитывающаяся при выборе дерева, которое
    /// будет расположено в биоме на данном шаге. Допустимы любые положительные значения,
    /// чем больше, тем больше вероятность выбора данного дерева
    /// </summary>
    public float Prevalence { get => prevalence; set => prevalence = value; }
}
