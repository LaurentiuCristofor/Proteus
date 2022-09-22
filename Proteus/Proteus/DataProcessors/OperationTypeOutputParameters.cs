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
    /// Includes parameters for performing an operation specified by a type.
    /// </summary>
    public class OperationTypeOutputParameters<TOperationType> : BaseOutputParameters
    {
        /// <summary>
        /// The type of the operation that should be performed.
        /// </summary>
        public TOperationType OperationType { get; protected set; }

        /// <summary>
        /// The arguments of the operation.
        /// </summary>
        public string[] Arguments { get; protected set; }

        /// <summary>
        /// Alias for the first argument of the operation.
        /// </summary>
        public string FirstArgument { get { return this.Arguments[0]; } }

        /// <summary>
        /// Alias for the second argument of the operation.
        /// </summary>
        public string SecondArgument { get { return this.Arguments[1]; } }

        /// <summary>
        /// Alias for the third argument of the operation.
        /// </summary>
        public string ThirdArgument { get { return this.Arguments[2]; } }

        public OperationTypeOutputParameters(
            string outputFilePath,
            TOperationType operationType,
            string[] arguments = null)
            : base(outputFilePath)
        {
            this.OperationType = operationType;
            this.Arguments = arguments;
        }

        public void CheckFirstArgumentIsAvailable()
        {
            if (this.Arguments == null || this.Arguments.Length < 1 || this.Arguments[0] == null)
            {
                throw new ProteusException($"The expected first argument of operation {this.OperationType} is not available!");
            }
        }

        public void CheckFirstArgumentIsAvailableAndNotEmpty()
        {
            if (this.Arguments == null || this.Arguments.Length < 1 || String.IsNullOrEmpty(this.Arguments[0]))
            {
                throw new ProteusException($"The expected first argument of operation {this.OperationType} is not available or is an empty string!");
            }
        }

        public void CheckSecondArgumentIsAvailable()
        {
            if (this.Arguments == null || this.Arguments.Length < 2 || this.Arguments[1] == null)
            {
                throw new ProteusException($"The expected second argument of operation {this.OperationType} is not available!");
            }
        }

        public void CheckSecondArgumentIsAvailableAndNotEmpty()
        {
            if (this.Arguments == null || this.Arguments.Length < 2 || String.IsNullOrEmpty(this.Arguments[1]))
            {
                throw new ProteusException($"The expected second argument of operation {this.OperationType} is not available or is an empty string!");
            }
        }

        public void CheckThirdArgumentIsAvailable()
        {
            if (this.Arguments == null || this.Arguments.Length < 3 || this.Arguments[2] == null)
            {
                throw new ProteusException($"The expected third argument of operation {this.OperationType} is not available!");
            }
        }

        public void CheckThirdArgumentIsAvailableAndNotEmpty()
        {
            if (this.Arguments == null || this.Arguments.Length < 3 || String.IsNullOrEmpty(this.Arguments[2]))
            {
                throw new ProteusException($"The expected third argument of operation {this.OperationType} is not available or is an empty string!");
            }
        }
    }
}
