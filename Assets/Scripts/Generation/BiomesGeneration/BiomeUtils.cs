using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeUtils
{
    public static BiomeType CalculateHoldridgeBiome(float precipitation, float temperature)
    {
        if (precipitation <= 0 || temperature <= 0)
        {
            throw new ArgumentException("Precipitation and temperature values must be greater than 0.");
        }

        // Calculate potential evapotranspiration
        double pet = 29.8 * Math.Exp(-0.147 * temperature) * Math.Pow(temperature + 8.86, 2) / 100.0;

        // Calculate moisture index
        double mi = Math.Log10(precipitation) + (pet / 10.0) - 2.67;

        // Calculate potential vegetation
        double pv = 0.0285 * Math.Pow(mi, 1.67);

        // Calculate biome type based on temperature and potential vegetation
        if (temperature < 0.0)
        {
            return BiomeType.PolarDesert;
        }
        else if (temperature >= 0.0 && temperature < 1.4)
        {
            if (pv < 0.05)
            {
                return BiomeType.Tundra;
            }
            else
            {
                return BiomeType.BorealForest;
            }
        }
        else if (temperature >= 1.4 && temperature < 6.0)
        {
            if (pv < 0.05)
            {
                return BiomeType.Tundra;
            }
            else if (pv >= 0.05 && pv < 0.1)
            {
                return BiomeType.Shrubland;
            }
            else if (pv >= 0.1 && pv < 0.5)
            {
                return BiomeType.TemperateForest;
            }
            else
            {
                return BiomeType.TemperateRainForest;
            }
        }
        else if (temperature >= 6.0 && temperature < 10.0)
        {
            if (pv < 0.05)
            {
                return BiomeType.Tundra;
            }
            else if (pv >= 0.05 && pv < 0.1)
            {
                return BiomeType.Shrubland;
            }
            else if (pv >= 0.1 && pv < 0.5)
            {
                return BiomeType.TemperateForest;
            }
            else if (pv >= 0.5 && pv < 0.75)
            {
                return BiomeType.TemperateRainForest;
            }
            else
            {
                return BiomeType.TropicalRainForest;
            }
        }
        else
        {
            return BiomeType.TropicalSeasonalForest;
        }
    }
}
