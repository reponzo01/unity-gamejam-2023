using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    /// <summary>
    /// Powerup Enum
    /// </summary>
    public enum PowerupEnum
    {
        bomb,
        match,
        multiselect,
        none
    }
}
