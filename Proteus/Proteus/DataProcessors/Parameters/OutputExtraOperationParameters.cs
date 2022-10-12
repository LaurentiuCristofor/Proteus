////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.DataHolders;

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes operation type besides extra output parameters.
    /// 
    /// For some operations, the type of their parameters varies depending on the type of the operation,
    /// so we collect them as strings and parse them as a specific type when we actually perform the operation.
    /// </summary>
    public class OutputExtraOperationParameters<TOperationType> : OutputExtraParameters
    {
        /// <summary>
        /// The type of the operation that should be performed.
        /// </summary>
        public TOperationType OperationType { get; protected set; }

        public OutputExtraOperationParameters(
            string outputFilePath,
            TOperationType operationType,
            string[] stringParameters = null,
            int[] intParameters = null,
            ulong[] ulongParameters = null,
            double[] doubleParameters = null,
            IDataHolder[] dataHolderParameters = null)
            : base(outputFilePath, stringParameters, intParameters, ulongParameters, doubleParameters, dataHolderParameters)
        {
            this.OperationType = operationType;
        }

        public OutputExtraOperationParameters(
            string outputFilePath,
            TOperationType operationType,
            string[] stringParameters,
            ulong[] ulongParameters)
            : base(outputFilePath, stringParameters, ulongParameters)
        {
            this.OperationType = operationType;
        }

        public OutputExtraOperationParameters(
            string outputFilePath,
            TOperationType operationType,
            string[] stringParameters,
            double[] doubleParameters)
            : base(outputFilePath, stringParameters, doubleParameters)
        {
            this.OperationType = operationType;
        }

        public OutputExtraOperationParameters(
            string outputFilePath,
            TOperationType operationType,
            string[] stringParameters,
            IDataHolder[] dataHolderParameters)
            : base(outputFilePath, stringParameters, dataHolderParameters)
        {
            this.OperationType = operationType;
        }

        public OutputExtraOperationParameters(
            string outputFilePath,
            TOperationType operationType,
            int[] intParameters)
            : base(outputFilePath, intParameters)
        {
            this.OperationType = operationType;
        }

        public OutputExtraOperationParameters(
            string outputFilePath,
            TOperationType operationType,
            ulong[] ulongParameters)
            : base(outputFilePath, ulongParameters)
        {
            this.OperationType = operationType;
        }

        public OutputExtraOperationParameters(
            string outputFilePath,
            TOperationType operationType,
            double[] doubleParameters)
            : base(outputFilePath, doubleParameters)
        {
            this.OperationType = operationType;
        }

        public OutputExtraOperationParameters(
            string outputFilePath,
            TOperationType operationType,
            IDataHolder[] dataHolderParameters)
            : base(outputFilePath, dataHolderParameters)
        {
            this.OperationType = operationType;
        }
    }
}
