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
        public static void Sort<T>(List<T> list, SortingAlgorithmType algorithmType)
            where T : IComparable
        {
            switch (algorithmType)
            {
                case SortingAlgorithmType.Insertion:
                    SortingMethods.InsertionSort<T>(list);
                    break;

                case SortingAlgorithmType.Shell:
                    SortingMethods.ShellSort<T>(list);
                    break;

                case SortingAlgorithmType.Merge:
                    SortingMethods.MergeSort<T>(list);
                    break;

                case SortingAlgorithmType.Quicksort:
                    SortingMethods.Quicksort<T>(list);
                    break;

                case SortingAlgorithmType.Heap:
                    SortingMethods.HeapSort<T>(list);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling sorting algorithm type '{algorithmType}'!");
            }
        }
    }
}
