////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Cabeiro.Common
{
    /// <summary>
    /// A special file path builder used for string edit operations.
    /// </summary>
    public class EditOperationFilePathBuilder : FilePathBuilder
    {
        protected StringEditType EditType { get; set; }

        public EditOperationFilePathBuilder(StringEditType editType, string inputFilePath, string outputFileExtension, string[] editArguments, string outputFilePath)
            : base(inputFilePath, outputFileExtension, editArguments, outputFilePath)
        {
            EditType = editType;
        }

        protected override void ProcessArguments()
        {
            // PLN requires additional special processing of its separator argument.
            //
            if (EditType == StringEditType.PrefixLineNumbers)
            {
                OperationArguments[0] = ArgumentParser.ParseSeparator(OperationArguments[0]);
            }
        }

        protected override void AppendArgumentsToExtension()
        {
            // For PLN we don't append its separator argument to the output file extension.
            //
            if (EditType == StringEditType.PrefixLineNumbers)
            {
                return;
            }

            base.AppendArgumentsToExtension();
        }
    }
}
