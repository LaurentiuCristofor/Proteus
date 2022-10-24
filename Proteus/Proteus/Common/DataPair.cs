////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A container for storing a pair of data.
    /// 
    /// Unlike System.Tuple, comparisons of DataPair instances will only use the first data of the pair.
    /// 
    /// Effectively, a DataPair is just a container of the first data instance
    /// that just happens to also provide a reference to a second piece of data,
    /// but that second piece of data does not influence the identity of the pair.
    /// </summary>
    public class DataPair<T1, T2> : IComparable
        where T1 : IComparable
    {
        /// <summary>
        /// The first data of the pair.
        /// </summary>
        public T1 FirstData { get; set; }

        /// <summary>
        /// The second data of the pair.
        /// </summary>
        public T2 SecondData { get; set; }

        /// <summary>
        /// Creates a pair of data.
        /// </summary>
        /// <param name="firstData">First data of the pair.</param>
        /// <param name="secondData">Second data of the pair.</param>
        public DataPair(T1 firstData, T2 secondData)
        {
            if (firstData == null)
            {
                throw new ProteusException("First data of a DataPair cannot be null!");
            }

            FirstData = firstData;
            SecondData = secondData;
        }

        public int CompareTo(object otherObject)
        {
            DataPair<T1, T2> otherDataPair = (DataPair<T1, T2>)otherObject;
            return FirstData.CompareTo(otherDataPair.FirstData);
        }

        public override bool Equals(Object otherObject)
        {
            throw new ProteusException("DataPair instances do not support Equals(); they only support CompareTo()!");
        }

        public override int GetHashCode()
        {
            throw new ProteusException("DataPair instances do not support GetHashCode(); they only support CompareTo()!");
        }
    }
}
