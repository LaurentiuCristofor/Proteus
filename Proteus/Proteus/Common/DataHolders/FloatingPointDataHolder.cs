////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.DataHolders
{
    /// <summary>
    /// An IDataType implementation that manages floating point values.
    /// </summary>
    public class FloatingPointDataHolder : IDataHolder
    {
        protected double Data { get; set; }

        public FloatingPointDataHolder(double data)
        {
            this.Data = data;
        }

        public DataType GetDataType()
        {
            return DataType.FloatingPoint;
        }

        public long GetIntegerValue()
        {
            throw new NotImplementedException();
        }

        public ulong GetUnsignedIntegerValue()
        {
            throw new NotImplementedException();
        }

        public double GetFloatingPointValue()
        {
            return this.Data;
        }

        public DateTime GetDateTimeValue()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.Data.ToString();
        }

        public IDataHolder Add(IDataHolder otherData)
        {
            this.Data += otherData.GetFloatingPointValue();
            return this;
        }

        public IDataHolder Subtract(IDataHolder otherData)
        {
            this.Data -= otherData.GetFloatingPointValue();
            return this;
        }

        public IDataHolder Multiply(IDataHolder otherData)
        {
            this.Data *= otherData.GetFloatingPointValue();
            return this;
        }

        public IDataHolder Divide(IDataHolder otherData)
        {
            this.Data /= otherData.GetFloatingPointValue();
            return this;
        }

        public int CompareTo(IDataHolder otherData)
        {
            if (otherData == null)
            {
                throw new ProteusException("Attempting to compare with null!");
            }

            return this.Data.CompareTo(otherData.GetFloatingPointValue());
        }

        public int CompareTo(object otherObject)
        {
            FloatingPointDataHolder otherData = (FloatingPointDataHolder)otherObject;
            return this.CompareTo(otherData);
        }

        public override bool Equals(Object otherObject)
        {
            if (otherObject == null)
            {
                return false;
            }

            int comparison;

            try
            {
                comparison = this.CompareTo(otherObject);
            }
            catch (ProteusException)
            {
                return false;
            }

            return comparison == 0;
        }

        public override int GetHashCode()
        {
            return this.Data.GetHashCode();
        }
    }
}
