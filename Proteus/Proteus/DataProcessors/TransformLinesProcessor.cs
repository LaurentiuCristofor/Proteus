////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that transforms lines.
    /// </summary>
    public class TransformLinesProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<LineTransformationType>, ExtractedColumnStrings>
    {
        protected OutputOperationParameters<LineTransformationType> Parameters { get; set; }

        /// <summary>
        /// First argument, as an integer value, if expected.
        /// </summary>
        protected int FirstArgumentAsInt { get; set; }

        /// <summary>
        /// A list collecting the lines to unite.
        /// </summary>
        protected List<string> LinesToUnite { get; set; }

        public void Initialize(OutputOperationParameters<LineTransformationType> processingParameters)
        {
            this.Parameters = processingParameters;

            switch (this.Parameters.OperationType)
            {
                case LineTransformationType.Break:
                    break;

                case LineTransformationType.Unite:
                    ArgumentChecker.CheckNotNull(this.Parameters.FirstArgument);

                    this.FirstArgumentAsInt = int.Parse(this.Parameters.FirstArgument);

                    ArgumentChecker.CheckStrictlyPositive(this.FirstArgumentAsInt);

                    this.LinesToUnite = new List<string>();
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling line transformation type '{this.Parameters.OperationType}'!");
            }

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            switch (this.Parameters.OperationType)
            {
                case LineTransformationType.Break:
                    {
                        foreach (string line in lineData.Columns)
                        {
                            this.OutputWriter.WriteLine(line);
                        }
                        break;
                    }

                case LineTransformationType.Unite:
                    {
                        this.LinesToUnite.Add(lineData.OriginalLine);

                        // If we collected enough lines to unite,
                        // then concatenate them all together.
                        //
                        if (this.LinesToUnite.Count == this.FirstArgumentAsInt)
                        {
                            string unitedLine = string.Join(lineData.ColumnSeparator, this.LinesToUnite.ToArray());
                            this.OutputWriter.WriteLine(unitedLine);

                            // Clear list to start collecting a new set of lines.
                            //
                            this.LinesToUnite.Clear();
                        }

                        break;
                    }

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling line transformation type '{this.Parameters.OperationType}'!");
            }

            return true;
        }
    }
}
