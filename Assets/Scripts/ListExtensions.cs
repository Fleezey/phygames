using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PhyGames
{
    public static class ListExtension
    {
        public static void Shuffle<T>(this IList<T> tList)
        {
            int count = tList.Count;
            int last = count - 1;

            for (int i = 0; i < last; ++i)
            {
                int r = Random.Range(i, count);
                T temp = tList[i];
                tList[i] = tList[r];
                tList[r] = temp;
            }
        }
    }
}