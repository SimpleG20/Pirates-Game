using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public static class Extensions
{
    public static T PickRandomArray<T>(this T[] array)
    {        
        int randIndex = Random.Range(0, array.Length);
        var obj = array[randIndex];
        return obj;
    }

    public static T PickRandomList<T>(this List<T> list, bool remove = false)
    {
        int randIndex = Random.Range(0, list.Count);
        var obj = list[randIndex];

        if (remove)
        {
            list.Remove(obj);
        }

        return obj;
    }

}
