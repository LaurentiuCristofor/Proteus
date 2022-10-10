////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace LaurentiuCristofor.Proteus.Common.Utilities
{
    public abstract class SortingMethods
    {
        /// <summary>
        /// Exchanges two elements in a list.
        /// Does not check the validity of the input.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list in which to exchange elements.</param>
        /// <param name="i">The index of the first element to exchange.</param>
        /// <param name="j">The index of the second element to exchange.</param>
        public static void Exchange<T>(List<T> list, int i, int j)
        {
            T savedValue = list[i];
            list[i] = list[j];
            list[j] = savedValue;
        }

        /// <summary>
        /// Returns the index in the list where value is found
        /// or where it should be inserted.
        /// Does not check the validity of the input.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to search in.</param>
        /// <param name="left">The start index of the search range.</param>
        /// <param name="right">The end index of the search range.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns></returns>
        public static int BinarySearch<T>(List<T> list, int left, int right, T value)
            where T : IComparable
        {
            while (left <= right)
            {
                int middle = (left + right) / 2;

                int comparison = value.CompareTo(list[middle]);
                if (comparison == 0)
                {
                    return middle;
                }
                else if (comparison < 0)
                {
                    right = middle - 1;
                }
                else // if (comparison > 0)
                {
                    left = middle + 1;
                }
            }

            return left;
        }

        /// <summary>
        /// Orders 3 different elements of a list.
        /// Does not check the validity of the input.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list in which to order the elements.</param>
        /// <param name="i">The index of the first element.</param>
        /// <param name="j">The index of the second element.</param>
        /// <param name="k">The index of the third element.</param>
        public static void Sort3<T>(List<T> list, int i, int j, int k)
            where T : IComparable
        {
            if (list[i].CompareTo(list[j]) > 0)
            {
                Exchange(list, i, j);
            }

            if (list[i].CompareTo(list[k]) > 0)
            {
                Exchange(list, i, k);
            }

            if (list[j].CompareTo(list[k]) > 0)
            {
                Exchange(list, j, k);
            }
        }

        /// <summary>
        /// Heap sift-down method.
        /// Does not check the validity of the input.
        /// </summary>
        /// <typeparam name="T">The heap element type.</typeparam>
        /// <param name="list">The heap list.</param>
        /// <param name="i">The index of the element to sift down.</param>
        /// <param name="n">The index of the last heap element.</param>
        public static void SiftDown<T>(List<T> list, int i, int n)
            where T : IComparable
        {
            T value = list[i];

            for (int j = 2 * i + 1; j <= n; i = j, j = 2 * i + 1)
            {
                // Determine which child is greater
                //
                if (j < n)
                {
                    if (list[j + 1].CompareTo(list[j]) > 0)
                    {
                        j++;
                    }
                }

                // Break if children are smaller.
                //
                if (list[j].CompareTo(value) <= 0)
                {
                    break;
                }

                // Move child up.
                //
                list[i] = list[j];
            }

            // Place value into its final position
            //
            list[i] = value;
        }

        /// <summary>
        /// An implementation of insertion sorting.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to sort.</param>
        public static void InsertionSort<T>(List<T> list)
            where T : IComparable
        {
            DeltaInsertionSort(list, 0, list.Count - 1, 1);
        }

        /// <summary>
        /// Implements insertion sorting of elements that are delta positions apart.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="left">The start index of the sorting range.</param>
        /// <param name="right">The end index of the sorting range.</param>
        /// <param name="delta">The delta spacing between elements that should be used.</param>
        public static void DeltaInsertionSort<T>(List<T> list, int left, int right, int delta)
            where T : IComparable
        {
            for (int i = left + delta; i <= right; i += delta)
            {
                T value = list[i];

                int j;
                for (j = i; j > left && value.CompareTo(list[j - delta]) < 0; j -= delta)
                {
                    list[j] = list[j - delta];
                }

                list[j] = value;
            }
        }

        /// <summary>
        /// Implementation of the Shell sorting algorithm (Donald L. Shell 1959).
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to sort.</param>
        public static void ShellSort<T>(List<T> list)
            where T : IComparable
        {
            ShellSort(list, 0, list.Count - 1);
        }

        /// <summary>
        /// Implementation of the Shell sorting algorithm (Donald L. Shell 1959).
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="left">The start index of the sorting range.</param>
        /// <param name="right">The end index of the sorting range.</param>
        public static void ShellSort<T>(List<T> list, int left, int right)
            where T : IComparable
        {
            int delta = list.Count;

            do
            {
                // Initially, we set delta to about one third of the number of elements in the list.
                //
                delta = delta / 3 + 1;

                for (int i = left; i < left + delta; ++i)
                {
                    DeltaInsertionSort(list, i, right, delta);
                }
            }
            while (delta > 1);
        }

        /// <summary>
        /// An implementation of merge sorting.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to sort.</param>
        public static void MergeSort<T>(List<T> list)
            where T : IComparable
        {
            List<T> copyList = new List<T>(list);

            MergeSort(list, copyList, 0, list.Count - 1);
        }

        /// <summary>
        /// An implementation of merge sorting.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="copyList">The list to use for copying the sublists to merge.</param>
        /// <param name="left">The start index of the sorting range.</param>
        /// <param name="right">The end index of the sorting range.</param>
        private static void MergeSort<T>(List<T> list, List<T> copyList, int left, int right)
            where T : IComparable
        {
            if (right > left)
            {
                int i, j;

                int middle = (left + right) / 2;

                MergeSort(list, copyList, left, middle);
                MergeSort(list, copyList, middle + 1, right);

                // Copy the elements of list into copyList.
                //
                // At the end of this loop i will equal left.
                //
                for (i = middle + 1; i > left; --i)
                {
                    copyList[i - 1] = list[i - 1];
                }

                // The second sublist is copied in reverse order.
                // At the end of this loop j will equal right.
                //
                for (j = middle; j < right; ++j)
                {
                    copyList[right + middle - j] = list[j + 1];
                }

                // Merge the two sublists into the original list.
                //
                for (int k = left; k <= right; ++k)
                {
                    // <= makes it stable.
                    //
                    if (copyList[i].CompareTo(copyList[j]) <= 0)
                    {
                        list[k] = copyList[i++];
                    }
                    else
                    {
                        list[k] = copyList[j--];
                    }
                }
            }
        }

        /// <summary>
        /// A quicksort partitioning method that picks the pivot as the median of three elements:
        /// left, middle, and right.
        /// 
        /// Based on Robert Sedgewick's code from his "Algorithms in C++".
        /// 
        /// This method expects to be called on list segments of at least 3 elements.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="left">The start index of the sorting range.</param>
        /// <param name="right">The end index of the sorting range.</param>
        /// <returns>The index of the pivot element after the partitioning was done.</returns>
        private static int MedianOfThreePartition<T>(List<T> list, int left, int right)
            where T : IComparable
        {
            int middle = (left + right) / 2;

            Sort3(list, left, middle, right);

            // Now left and right will serve as sentinels.
            // The partitioning element will be put in list[right - 1].
            //
            --right;

            Exchange(list, middle, right);

            int i, j;
            for (i = left, j = right; ;)
            {
                while (list[++i].CompareTo(list[right]) < 0)
                {
                }

                while (list[--j].CompareTo(list[right]) > 0)
                {
                }

                if (i >= j)
                {
                    break;
                }

                Exchange(list, i, j);
            }

            Exchange(list, i, right);

            return i;
        }

        /// <summary>
        /// An implementation of the quicksort algorithm (C. A. R. Hoare 1962)
        /// that uses the median-of-three partitioning scheme for sublists larger than 16 elements
        /// and completes the sorting using InsertionSort.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Quicksort<T>(List<T> list)
            where T : IComparable
        {
            Quicksort(list, 0, list.Count - 1);
            InsertionSort(list);
        }

        /// <summary>
        /// An implementation of the quicksort algorithm (C. A. R. Hoare 1962).
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="left">The start index of the sorting range.</param>
        /// <param name="right">The end index of the sorting range.</param>
        public static void Quicksort<T>(List<T> list, int left, int right)
            where T : IComparable
        {
            const int sorterSwitchSize = 15;

            if ((right - left) > sorterSwitchSize)
            {
                int p = MedianOfThreePartition(list, left, right);

                Quicksort(list, left, p - 1);
                Quicksort(list, p + 1, right);
            }
        }

        /// <summary>
        /// An implementation of heap sorting.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list to sort.</param>
        public static void HeapSort<T>(List<T> list)
            where T : IComparable
        {
            int last = list.Count - 1;

            // Heapify.
            //
            for (int i = (last - 1) / 2; i > 0; --i)
            {
                SiftDown(list, i, last);
            }

            // The first SiftDown call will finish the heapify step;
            // then we will extract the top of the heap,
            // exchange it with the last element of the heap,
            // decrease the size of the heap, and start over again
            //
            for (int i = last; i > 0; --i)
            {
                SiftDown(list, 0, i);
                Exchange(list, 0, i);
            }
        }
    }
}
