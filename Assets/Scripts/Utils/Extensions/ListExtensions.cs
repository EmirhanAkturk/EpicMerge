using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandomEnum<T>(this T _enum) where T : Enum
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            return values.OrderBy(_ => UnityEngine.Random.Range(0, 100f)).First();
        }

        public static T GetRandomEnum<T>(this List<T> _enumValues) where T : Enum
        {
            return _enumValues.OrderBy(_ => UnityEngine.Random.Range(0, 100f)).First();
        }

        public static T GetRandom<T>(this T[] arr) where T : class
        {
            if (arr.Length == 0) return null;
            return arr.OrderBy(_ => UnityEngine.Random.Range(0, 100f)).First();
        }
        public static T GetRandom<T>(this List<T> llist) where T : class
        {
            if (llist.Count == 0) return null;
            return llist.OrderBy(_ => UnityEngine.Random.Range(0, 100f)).First();
        }

        public static void Shuffle<T>(this List<T> list)
        {
            if (list.Count == 0) return;
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    
        public static List<T> ShuffleToList<T>(this List<T> list)
        {
            if (list.Count == 0) return list;
            List<T> newList = new List<T>(list);
            int n = newList.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                (newList[k], newList[n]) = (newList[n], newList[k]);
            }
            return newList;
        }
    }
}
