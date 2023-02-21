using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T PickRandom<T>(this T[] array, bool remove = false)
    {        
        int randIndex = Random.Range(0, array.Length);
        var obj = array[randIndex];
        if (remove)
        {
            var newArray = new T[array.Length-1];
            for(int i = 0; i < newArray.Length; i++)
            {
                if (i == randIndex) newArray[i] = array[i+1];
                else newArray[i] = array[i];
            }

            array = newArray;
        }

        return obj;
    }

}
