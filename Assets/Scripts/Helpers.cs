using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Helpers : MonoBehaviour {

    public static IEnumerable<float> Range(float min, float max, float step = 1)
    {
        for (var i = 0; i < int.MaxValue; i++)
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
}
