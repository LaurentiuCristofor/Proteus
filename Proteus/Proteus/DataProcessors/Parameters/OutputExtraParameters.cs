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
    /// Includes extra parameters for performing an output operation.
    /// </summary>
    public class OutputExtraParameters : BaseOutputParameters
    {
        /// <summary>
        /// String-type parameters.
        /// </summary>
        public string[] StringParameters { get; set; }

        /// <summary>
        /// Int-type parameters.
        /// </summary>
        public int[] IntParameters { get; set; }

        /// <summary>
        /// Ulong-type parameters.
        /// </summary>
        public ulong[] UlongParameters { get; set; }

        /// <summary>
        /// Double-type parameters.
        /// </summary>
        public double[] DoubleParameters { get; set; }

        /// <summary>
        /// IDataHolder-type parameters.
        /// </summary>
        public IDataHolder[] DataHolderParameters { get; set; }

        public OutputExtraParameters(
            string outputFilePath,
            string[] stringParameters = null,
            int[] intParameters = null,
            ulong[] ulongParameters = null,
            double[] doubleParameters = null,
            IDataHolder[] dataHolderParameters = null)
            : base(outputFilePath)
        {
            this.StringParameters = stringParameters;
            this.IntParameters = intParameters;
            this.UlongParameters = ulongParameters;
            this.DoubleParameters = doubleParameters;
            this.DataHolderParameters = dataHolderParameters;
        }

        public OutputExtraParameters(
            string outputFilePath,
            string[] stringParameters,
            ulong[] ulongParameters)
            : base(outputFilePath)
        {
            this.StringParameters = stringParameters;
            this.IntParameters = null;
            this.UlongParameters = ulongParameters;
            this.DoubleParameters = null;
            this.DataHolderParameters = null;
        }

        public OutputExtraParameters(
            string outputFilePath,
            string[] stringParameters,
            double[] doubleParameters)
            : base(outputFilePath)
        {
            this.StringParameters = stringParameters;
            this.IntParameters = null;
            this.UlongParameters = null;
            this.DoubleParameters = doubleParameters;
            this.DataHolderParameters = null;
        }

        public OutputExtraParameters(
            string outputFilePath,
            string[] stringParameters,
            IDataHolder[] dataHolderParameters)
            : base(outputFilePath)
        {
            this.StringParameters = stringParameters;
            this.IntParameters = null;
            this.UlongParameters = null;
            this.DoubleParameters = null;
            this.DataHolderParameters = dataHolderParameters;
        }

        public OutputExtraParameters(
            string outputFilePath,
            int[] intParameters)
            : base(outputFilePath)
        {
            this.StringParameters = null;
            this.IntParameters = intParameters;
            this.UlongParameters = null;
            this.DoubleParameters = null;
            this.DataHolderParameters = null;
        }

        public OutputExtraParameters(
            string outputFilePath,
            ulong[] ulongParameters)
            : base(outputFilePath)
        {
            this.StringParameters = null;
            this.IntParameters = null;
            this.UlongParameters = ulongParameters;
            this.DoubleParameters = null;
            this.DataHolderParameters = null;
        }

        public OutputExtraParameters(
            string outputFilePath,
            double[] doubleParameters)
            : base(outputFilePath)
        {
            this.StringParameters = null;
            this.IntParameters = null;
            this.UlongParameters = null;
            this.DoubleParameters = doubleParameters;
            this.DataHolderParameters = null;
        }

        public OutputExtraParameters(
            string outputFilePath,
            IDataHolder[] dataHolderParameters)
            : base(outputFilePath)
        {
            this.StringParameters = null;
            this.IntParameters = null;
            this.UlongParameters = null;
            this.DoubleParameters = null;
            this.DataHolderParameters = dataHolderParameters;
        }
    }
}
