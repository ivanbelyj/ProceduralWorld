using System.Collections;
using System.Collections.Generic;
using System;

public static class RandomExtensions
{
    public static float Range(this Random random, float minInclusive, float maxInclusive)
    {
        double range = (double)maxInclusive - (double)minInclusive;
        double sample = random.NextDouble() * range + (double)minInclusive;
        return (float)sample;
    }
}

