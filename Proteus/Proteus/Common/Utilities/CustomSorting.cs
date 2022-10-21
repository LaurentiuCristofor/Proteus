////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common.Algorithms;
using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.Utilities
{
    public abstract class CustomSorting
    {
        /// <summary>
        /// Sorts a list using a custom sorting algorithm.
        /// </summary>
        /// <typeparam name="T">The type of the list elements.</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="algorithmType">The custom sorting algorithm to use.</param>
        public static void Sort<T>(List<T> list, SortingAlgorithmType algorithmType)
            where T : IComparable
        {
            switch (algorithmType)
            {
                case SortingAlgorithmType.Insertion:
                    SortingOperations.InsertionSort<T>(list);
                    break;

                case SortingAlgorithmType.Shell:
                    SortingOperations.ShellSort<T>(list);
                    break;

                case SortingAlgorithmType.Merge:
                    SortingOperations.MergeSort<T>(list);
                    break;

                case SortingAlgorithmType.Quicksort:
                    SortingOperations.Quicksort<T>(list);
                    break;

                case SortingAlgorithmType.Heap:
                    SortingOperations.HeapSort<T>(list);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling sorting algorithm type '{algorithmType}'!");
            }
        }
    }
}
