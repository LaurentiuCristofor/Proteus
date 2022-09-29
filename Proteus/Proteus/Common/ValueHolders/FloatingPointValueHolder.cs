////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.ValueHolders
{
    public class FloatingPointValueHolder : IValueHolder
    {
        protected double Data { get; set; }

        public FloatingPointValueHolder(double data)
        {
            this.Data = data;
        }

        public DataType GetDataType()
        {
            return DataType.FloatingPoint;
        }

        public bool IsNumerical()
        {
            return true;
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

        public IValueHolder Add(IValueHolder otherData)
        {
            this.Data += otherData.GetFloatingPointValue();
            return this;
        }

        public IValueHolder Subtract(IValueHolder otherData)
        {
            this.Data -= otherData.GetFloatingPointValue();
            return this;
        }

        public IValueHolder Multiply(IValueHolder otherData)
        {
            this.Data *= otherData.GetFloatingPointValue();
            return this;
        }

        public IValueHolder Divide(IValueHolder otherData)
        {
            this.Data /= otherData.GetFloatingPointValue();
            return this;
        }

        public int CompareTo(IValueHolder otherData)
        {
            if (otherData == null)
            {
                throw new ProteusException("Attempting to compare with null!");
            }

            return this.Data.CompareTo(otherData.GetFloatingPointValue());
        }

        public int CompareTo(object otherObject)
        {
            FloatingPointValueHolder otherData = (FloatingPointValueHolder)otherObject;
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
