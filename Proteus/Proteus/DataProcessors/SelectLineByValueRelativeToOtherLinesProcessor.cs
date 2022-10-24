////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks a value's relationship to that of other lines,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLineByValueRelativeToOtherLinesProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<RelativeValueSelectionType>, OneExtractedValue>
    {
        protected RelativeValueSelectionType SelectionType { get; set; }

        /// <summary>
        /// Set of values seen so far.
        /// </summary>
        protected HashSet<IDataHolder> SetValues { get; set; }

        /// <summary>
        /// Map of values seen so far to the last lines that we saw them in.
        /// </summary>
        protected Dictionary<IDataHolder, string> ValuesToLastLines { get; set; }

        public void Initialize(OutputOperationParameters<RelativeValueSelectionType> processingParameters)
        {
            SelectionType = processingParameters.OperationType;

            switch (SelectionType)
            {
                case RelativeValueSelectionType.First:
                case RelativeValueSelectionType.NotFirst:
                    SetValues = new HashSet<IDataHolder>();
                    break;

                case RelativeValueSelectionType.Last:
                case RelativeValueSelectionType.NotLast:
                    ValuesToLastLines = new Dictionary<IDataHolder, string>();
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling relative value selection type '{SelectionType}'!");
            }

            OutputWriter = new FileWriter(processingParameters.OutputFilePath, trackProgress: (SelectionType == RelativeValueSelectionType.Last));
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            IDataHolder data = lineData.ExtractedData;

            // Determine whether to output the line based on the handling type.
            //
            bool shouldOutputLine = false;
            string lineToOutput = lineData.OriginalLine;
            switch (SelectionType)
            {
                case RelativeValueSelectionType.First:
                    // Lookup data in our set;
                    //
                    if (!SetValues.Contains(data))
                    {
                        shouldOutputLine = true;
                        SetValues.Add(data);
                    }
                    break;

                case RelativeValueSelectionType.NotFirst:
                    // Lookup data in our set;
                    //
                    if (SetValues.Contains(data))
                    {
                        shouldOutputLine = true;
                    }
                    else
                    {
                        SetValues.Add(data);
                    }
                    break;

                case RelativeValueSelectionType.Last:
                    if (ValuesToLastLines.ContainsKey(data))
                    {
                        ValuesToLastLines[data] = lineData.OriginalLine;
                    }
                    else
                    {
                        ValuesToLastLines.Add(data, lineData.OriginalLine);
                    }
                    break;

                case RelativeValueSelectionType.NotLast:
                    if (ValuesToLastLines.ContainsKey(data))
                    {
                        // The line we saw before with this data value is not the last, so we can output it.
                        //
                        lineToOutput = ValuesToLastLines[data];
                        shouldOutputLine = true;

                        // Update last line seen for this data value.
                        //
                        ValuesToLastLines[data] = lineData.OriginalLine;
                    }
                    else
                    {
                        ValuesToLastLines.Add(data, lineData.OriginalLine);
                    }
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling relative value selection type '{SelectionType}'!");
            }

            if (shouldOutputLine)
            {
                OutputWriter.WriteLine(lineToOutput);
            }

            return true;
        }

        public override void CompleteExecution()
        {
            if (SelectionType == RelativeValueSelectionType.Last)
            {
                if (ValuesToLastLines == null)
                {
                    throw new ProteusException("Internal error: An expected data structure has not been initialized!");
                }

                foreach (string line in ValuesToLastLines.Values)
                {
                    OutputWriter.WriteLine(line);
                }
            }

            base.CompleteExecution();
        }
    }
}
