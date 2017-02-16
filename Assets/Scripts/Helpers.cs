using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helpers {

    public static IEnumerable<float> Range(float min, float max, float step = 1)
    {
        for (var i = 0; i < Int32.MaxValue; i++)
        {
            var value = min + step * i;
            if (value >= max)
                break;
            yield return value;
        }
    }

    public static IEnumerable<Vector3> Surrounding(Vector3 pos)
    {
        var offset = new List<float> { -1, 0, 1 };
        return from x in offset from y in offset select new Vector3(x, y, 0) + pos;
    }

    public static Vector3 Round(Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), 0);
    }
}
