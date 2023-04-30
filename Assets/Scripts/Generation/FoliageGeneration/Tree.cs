using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Вид дерева, не зависящий от биома
/// </summary>
[CreateAssetMenu(fileName = "New Tree", menuName = "Procedural World/Tree", order = 51)]
public class Tree : ScriptableObject
{
    [SerializeField]
    private GameObject[] treePrefabs;
    /// <summary>
    /// Дерево представлено различными вариантами моделей
    /// </summary>
    public GameObject[] TreePrefabs { get => treePrefabs; set => treePrefabs = value; }
}
