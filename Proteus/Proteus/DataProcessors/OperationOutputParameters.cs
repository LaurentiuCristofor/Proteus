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
    /// Includes parameters for performing an output operation specified by a type.
    /// </summary>
    public class OperationOutputParameters<TOperationType> : BaseOutputParameters
    {
        /// <summary>
        /// The type of the operation that should be performed.
        /// </summary>
        public TOperationType OperationType { get; protected set; }

        /// <summary>
        /// The arguments of the operation.
        /// </summary>
        protected string[] Arguments { get; set; }

        /// <summary>
        /// The first argument of the operation.
        /// </summary>
        public string FirstArgument { get; protected set; }

        /// <summary>
        /// The second argument of the operation.
        /// </summary>
        public string SecondArgument { get; protected set; }

        /// <summary>
        /// The third argument of the operation.
        /// </summary>
        public string ThirdArgument { get; protected set; }

        public OperationOutputParameters(
            string outputFilePath,
            TOperationType operationType,
            string[] arguments = null)
            : base(outputFilePath)
        {
            this.OperationType = operationType;
            this.Arguments = arguments;

            // Make arguments accessible directly without dereferencing the arguments array.
            //
            if (this.Arguments != null)
            {
                if (this.Arguments.Length > 0)
                {
                    this.FirstArgument = this.Arguments[0];
                }
                if (this.Arguments.Length > 1)
                {
                    this.SecondArgument = this.Arguments[1];
                }
                if (this.Arguments.Length > 2)
                {
                    this.ThirdArgument = this.Arguments[2];
                }
            }
        }
    }
}
