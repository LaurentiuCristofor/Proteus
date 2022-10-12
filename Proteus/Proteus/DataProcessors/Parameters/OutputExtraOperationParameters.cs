////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes extra parameters for performing an output operation specified by a type.
    /// 
    /// For most operations, the type of the parameters varies depending on the type of the operation,
    /// so we collect them as strings and parse them as a specific type when we actually perform the operation.
    /// </summary>
    public class OutputExtraOperationParameters<TOperationType> : OutputExtraParameters
    {
        /// <summary>
        /// The type of the operation that should be performed.
        /// </summary>
        public TOperationType OperationType { get; protected set; }

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

        public OutputExtraOperationParameters(
            string outputFilePath,
            TOperationType operationType,
            string[] arguments = null)
            : base(outputFilePath, arguments)
        {
            this.OperationType = operationType;

            // Make arguments accessible directly without dereferencing the arguments array.
            //
            if (this.StringParameters != null)
            {
                if (this.StringParameters.Length > 0)
                {
                    this.FirstArgument = this.StringParameters[0];
                }
                if (this.StringParameters.Length > 1)
                {
                    this.SecondArgument = this.StringParameters[1];
                }
                if (this.StringParameters.Length > 2)
                {
                    this.ThirdArgument = this.StringParameters[2];
                }
            }
        }
    }
}
