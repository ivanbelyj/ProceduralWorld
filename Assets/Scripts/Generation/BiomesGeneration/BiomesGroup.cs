using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Группа биомов - набор биомов, которые совпадают в плане
/// классификации по температуре и влажности, и различающиеся
/// каким-то другим параметрам (например, радиации)
/// </summary>
public class BiomesGroup
{
    public List<Biome> Biomes { get; set; }

    public BiomesGroup() {
        Biomes = new List<Biome>();
    }
}
