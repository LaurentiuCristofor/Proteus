////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// Includes parameters for performing an operation specified by a type.
    /// </summary>
    public class OperationTypeParameters<TOperationType> : BaseOutputParameters
    {
        /// <summary>
        /// The type of the operation that should be performed.
        /// </summary>
        public TOperationType OperationType { get; set; }

        /// <summary>
        /// The first argument of the operation.
        /// </summary>
        public string FirstArgument { get; set; }

        /// <summary>
        /// The second argument of the operation.
        /// </summary>
        public string SecondArgument { get; set; }

        public OperationTypeParameters(
            string outputFilePath,
            TOperationType operationType,
            string firstArgument = null,
            string secondArgument = null)
            : base(outputFilePath)
        {
            this.OperationType = operationType;
            this.FirstArgument = firstArgument;
            this.SecondArgument = secondArgument;
        }
    }
}
