////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.DataHolders
{
    public class DateTimeDataHolder : IDataHolder
    {
        protected DateTime Data { get; set; }

        public DateTimeDataHolder(DateTime data)
        {
            this.Data = data;
        }

        public DataType GetDataType()
        {
            return DataType.DateTime;
        }

        public bool IsNumerical()
        {
            return false;
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
            throw new NotImplementedException();
        }

        public DateTime GetDateTimeValue()
        {
            return this.Data;
        }

        public override string ToString()
        {
            return this.Data.ToString();
        }

        public IDataHolder Add(IDataHolder otherData)
        {
            throw new NotImplementedException();
        }

        public IDataHolder Subtract(IDataHolder otherData)
        {
            throw new NotImplementedException();
        }

        public IDataHolder Multiply(IDataHolder otherData)
        {
            throw new NotImplementedException();
        }

        public IDataHolder Divide(IDataHolder otherData)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDataHolder otherData)
        {
            if (otherData == null)
            {
                throw new ProteusException("Attempting to compare with null!");
            }

            return this.Data.CompareTo(otherData.GetDateTimeValue());
        }

        public int CompareTo(object otherObject)
        {
            DateTimeDataHolder otherData = (DateTimeDataHolder)otherObject;
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
