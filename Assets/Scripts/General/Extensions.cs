using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void DeleteChildren(this Transform transform)
    {
        foreach (Transform child in transform) Object.Destroy(child.gameObject);
    }

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

    public static void RemoveNulls<T>(this List<T> list)
    {
        List<T> newList = new List<T>();
        for(int i =  0; i < list.Count; i++)
        {
            if (list[i] != null)
            {
                newList.Add(list[i]);
            }
        }

        list = newList;
    }
}
