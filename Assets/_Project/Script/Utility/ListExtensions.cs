using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Utility
{
    public static class Extensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            var n = 0;
            while (n < list.Count)
            {
                var random = Random.Range(0, list.Count );
                (list[random], list[n]) = (list[n], list[random]);
                n++;
            }
        }
        
        public static void Shuffle<T>(this T[] array)
        {
            for (var i = array.Length - 1; i > 0; i--)
            {
                var random = Random.Range(0, i + 1);
                (array[random], array[i]) = (array[i], array[random]);
            }
        }

        public static void Shuffle<T>(this Queue<T> queue)
        {
            var n = 0;
            var list = queue.ToList();
            while (n < list.Count)
            {
                var random = Random.Range(0, list.Count - 1);
                (list[random], list[n]) = (list[n], list[random]);
                n++;
            }
            queue.Clear();
            foreach (var item in list)
                queue.Enqueue(item);
        }

        /// <summary>
        /// 중복이 없는 랜덤 반환
        /// </summary>
        public static List<T> NonDupRandomT<T>(this List<T> list, int count)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count < count)
                throw new ArgumentException("list가 count보다 작음");

            var shuffledList = new List<T>(list);
            shuffledList.Shuffle();

            return shuffledList.Take(count).ToList();
        }
    
        /// <summary>
        /// 중복이 있는 랜덤 반환
        /// </summary>
        public static List<T> DupRandomT<T>(this List<T> list, int count)
        {
            var randomList = new List<T>();
        
            while (randomList.Count < count)
            {
                var random = Random.Range(0, list.Count);
                randomList.Add(list[random]);
            }

            return randomList;
        }
        
        public static T GetRandomEnum<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }
    }
}
