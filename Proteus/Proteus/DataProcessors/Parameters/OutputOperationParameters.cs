////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes an operation type in addition to the base operation parameters.
    /// 
    /// Used for operations that don't require any specific arguments.
    /// </summary>
    public class OutputOperationParameters<TOperationType> : BaseOutputParameters
    {
        /// <summary>
        /// The type of the operation that should be performed.
        /// </summary>
        public TOperationType OperationType { get; protected set; }

        public OutputOperationParameters(
            string outputFilePath,
            TOperationType operationType)
            : base(outputFilePath)
        {
            this.OperationType = operationType;
        }
    }
}
