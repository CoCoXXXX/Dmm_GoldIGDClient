using System.Collections.Generic;

namespace Dmm.PokerLogic
{
    public class ListUtil
    {
        public static void QuickSort<T>(List<T> list, IComparer<T> comparer)
        {
            QuickSort0(list, 0, list.Count - 1, comparer);
        }

        private static void QuickSort0<T>(List<T> list, int low, int high, IComparer<T> comparer)
        {
            if (low >= high) return;

            T key = list[low];
            int i = low;
            int j = high;

            while (i < j)
            {
                while (i < j && (comparer.Compare(list[j], key) >= 0))
                {
                    j--;
                }
                list[i] = list[j];
                while (i < j && (comparer.Compare(list[i], key) <= 0))
                {
                    i++;
                }
                list[j] = list[i];
            }
            list[i] = key;

            QuickSort0<T>(list, low, i - 1, comparer);
            QuickSort0<T>(list, i + 1, high, comparer);
        }
    }
}