////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
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
        public IDataHolder Argument { get; protected set; }

        public ValueEditOutputParameters(
            string outputFilePath,
            ValueEditType editType,
            IDataHolder argument = null)
            : base(outputFilePath)
        {
            this.EditType = editType;
            this.Argument = argument;
        }
    }
}
