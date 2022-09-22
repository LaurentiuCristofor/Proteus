////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A class for providing validation methods for data processors.
    /// </summary>
    public static class DataProcessorValidation
    {
        public static void ValidateLine(string line)
        {
            if (String.IsNullOrEmpty(line))
            {
                throw new ProteusException("A data processor was called with a null or empty line!");
            }
        }

        public static void ValidateLineData(ParsedLine lineData)
        {
            if (lineData == null)
            {
                throw new ProteusException("A data processor was called with null line data!");
            }
        }

        public static void ValidateColumnInformation(ParsedLine lineData)
        {
            if (lineData.Columns == null)
            {
                throw new ProteusException($"A data processor was called without expected column information!");
            }
            else if (String.IsNullOrEmpty(lineData.ColumnSeparator))
            {
                throw new ProteusException($"A data processor was called without expected column separator information!");
            }
            else if (lineData.ExtractedColumnNumber == 0)
            {
                throw new ProteusException($"A data processor was called without expected column number information!");
            }
        }

        public static void ValidateExtractedDataIsString(ParsedLine lineData)
        {
            if (lineData.ExtractedData.DataType != DataType.String)
            {
                throw new ProteusException($"A data processor that expected a string input was called with an input of type '{lineData.ExtractedData.DataType}'!");
            }
        }

        public static void ValidateSecondExtractedData(ParsedLine lineData)
        {
            if (lineData.SecondExtractedData == null)
            {
                throw new ProteusException("A data processor was called without the expected second extracted data!");
            }
        }
    }
}
