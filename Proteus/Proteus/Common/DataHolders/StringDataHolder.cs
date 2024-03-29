﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.DataHolders
{
    public class StringDataHolder : IDataHolder
    {
        protected string Data { get; set; }

        public StringDataHolder(string data)
        {
            Data = data;
        }

        public DataType GetDataType()
        {
            return DataType.String;
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
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Data;
        }

        public IDataHolder Add(IDataHolder otherData)
        {
            Data += otherData.ToString();
            return this;
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

            return Data.CompareTo(otherData.ToString());
        }

        public int CompareTo(object otherObject)
        {
            StringDataHolder otherData = (StringDataHolder)otherObject;
            return CompareTo(otherData);
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
                comparison = CompareTo(otherObject);
            }
            catch (ProteusException)
            {
                return false;
            }

            return comparison == 0;
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }
}
