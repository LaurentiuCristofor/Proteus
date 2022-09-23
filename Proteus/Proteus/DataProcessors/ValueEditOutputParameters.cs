////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// Includes parameters for performing a value edit operation.
    /// </summary>
    public class ValueEditOutputParameters : BaseOutputParameters
    {
        /// <summary>
        /// The type of the edit operation that should be performed.
        /// </summary>
        public ValueEditType EditType { get; protected set; }

        /// <summary>
        /// The argument of the operation.
        /// </summary>
        public DataTypeContainer Argument { get; protected set; }

        public ValueEditOutputParameters(
            string outputFilePath,
            ValueEditType editType,
            DataTypeContainer argument = null)
            : base(outputFilePath)
        {
            this.EditType = editType;
            this.Argument = argument;
        }
    }
}
