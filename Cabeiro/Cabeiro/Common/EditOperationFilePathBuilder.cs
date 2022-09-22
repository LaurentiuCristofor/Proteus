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

        public EditOperationFilePathBuilder(StringEditType editType, string inputFilePath, string outputFileExtension, string[] editArguments, string outputFilePath)
            : base(inputFilePath, outputFileExtension, editArguments, outputFilePath)
        {
            this.EditType = editType;
        }

        protected override void ProcessArguments()
        {
            // PLN requires additional special processing of its separator argument.
            //
            if (this.EditType == StringEditType.PrefixLineNumbers)
            {
                this.OperationArguments[0] = ArgumentParser.ParseSeparator(this.OperationArguments[0]);
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
