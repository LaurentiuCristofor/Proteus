////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Cabeiro.Common
{
    public class EditOperationFilePathBuilder : FilePathBuilder
    {
        protected StringEditType EditType { get; set; }

        public EditOperationFilePathBuilder(StringEditType editType, string inputFilePath, string outputFileExtension, string firstArgument, string secondArgument, string outputFilePath)
            : base(inputFilePath, outputFileExtension, firstArgument, secondArgument, outputFilePath)
        {
            this.InputFilePath = inputFilePath;
            this.OutputFileExtension = outputFileExtension;
            this.FirstArgument = firstArgument;
            this.SecondArgument = secondArgument;
            this.OutputFilePath = outputFilePath;
        }

        protected override void ProcessArguments()
        {
            // PLN requires additional special processing of its separator argument.
            //
            if (this.EditType == StringEditType.PrefixLineNumbers)
            {
                this.FirstArgument = ArgumentParser.ParseSeparator(this.FirstArgument);
            }
        }

        protected override void AppendArgumentsToExtension()
        {
            // For PLN we don't need to append its argument to the output file extension.
            //
            if (this.EditType == StringEditType.PrefixLineNumbers)
            {
                return;
            }

            base.AppendArgumentsToExtension();
        }
    }
}
