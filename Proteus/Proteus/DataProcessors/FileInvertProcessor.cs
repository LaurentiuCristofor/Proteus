////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that inverts the order of the input lines.
    /// </summary>
    public class FileInvertProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, string>
    {
        /// <summary>
        /// Data structure used for loading the lines before outputting them in reverse order.
        /// </summary>
        protected Stack<string> LineStack { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            LineStack = new Stack<string>();

            OutputWriter = new FileWriter(processingParameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            LineStack.Push(line);

            return true;
        }

        public override void CompleteExecution()
        {
            if (LineStack == null)
            {
                throw new ProteusException("Internal error: An expected data structure has not been initialized!");
            }

            foreach (string line in LineStack)
            {
                OutputWriter.WriteLine(line);
            }

            base.CompleteExecution();
        }
    }
}
