////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that sorts the input lines.
    /// </summary>
    public class SortProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, string>
    {
        protected BaseOutputParameters Parameters { get; set; }

        /// <summary>
        /// Data structure used for loading the lines before sorting them.
        /// </summary>
        protected List<string> Lines { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            this.Lines = new List<string>();

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            this.Lines.Add(line);

            return true;
        }

        public override void CompleteExecution()
        {
            if (this.Lines == null)
            {
                throw new ProteusException("Internal error: An expected data structure has not been initialized!");
            }

            Timer timer = new Timer($"\n{Constants.Strings.SortingStart}", Constants.Strings.SortingEnd);
            this.Lines.Sort();
            timer.StopAndReport();

            foreach (string line in this.Lines)
            {
                this.OutputWriter.WriteLine(line);
            }

            base.CompleteExecution();
        }
    }
}
