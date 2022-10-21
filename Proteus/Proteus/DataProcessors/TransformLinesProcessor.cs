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
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// IntParameters[0] - count of lines to unite (if required)
    /// </summary>
    public class TransformLinesProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<LineTransformationType>, ExtractedColumnStrings>
    {
        protected const int CountOfLinesToUniteIndex = 0;

        protected LineTransformationType TransformationType { get; set; }

        /// <summary>
        /// The count of lines to unite, if expected.
        /// </summary>
        protected int CountOfLinesToUnite { get; set; }

        /// <summary>
        /// A list collecting the lines to unite.
        /// </summary>
        protected List<string> LinesToUnite { get; set; }

        public void Initialize(OutputExtraOperationParameters<LineTransformationType> processingParameters)
        {
            this.TransformationType = processingParameters.OperationType;

            switch (this.TransformationType)
            {
                case LineTransformationType.Break:
                    break;

                case LineTransformationType.Unite:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, CountOfLinesToUniteIndex);
                    this.CountOfLinesToUnite = processingParameters.IntParameters[CountOfLinesToUniteIndex];
                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.CountOfLinesToUnite, 2);

                    this.LinesToUnite = new List<string>();
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling line transformation type '{this.TransformationType}'!");
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            switch (this.TransformationType)
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
                        if (this.LinesToUnite.Count == this.CountOfLinesToUnite)
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
                    throw new ProteusException($"Internal error: Proteus is not handling line transformation type '{this.TransformationType}'!");
            }

            return true;
        }
    }
}
