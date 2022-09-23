////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Cabeiro.Common
{
    /// <summary>
    /// A file path builder utility that encapsulates common operations required by Cabeiro.
    /// </summary>
    public class FilePathBuilder
    {
        /// <summary>
        /// The input file path that will be used to generate an output file path, if one was not provided.
        /// </summary>
        protected string InputFilePath { get; set; }

        /// <summary>
        /// An output file extension requested by caller. This may be augmented by the builder.
        /// </summary>
        protected string OutputFileExtension { get; set; }

        /// <summary>
        /// The arguments of the operation - they usually get written into the extension.
        /// </summary>
        protected string[] OperationArguments { get; set; }

        /// <summary>
        /// Building this path if it was not provided is the purpose of the builder.
        /// </summary>
        protected string OutputFilePath { get; set; }

        protected bool HasProcessedArguments { get; set; }

        public FilePathBuilder(string inputFilePath, string outputFileExtension, string[] operationArguments, string outputFilePath)
        {
            this.InputFilePath = inputFilePath;
            this.OutputFileExtension = outputFileExtension;
            this.OperationArguments = operationArguments;
            this.OutputFilePath = outputFilePath;
            this.HasProcessedArguments = false;
        }

        /// <summary>
        /// A stub for performing additional argument processing, if appropriate.
        /// </summary>
        protected virtual void ProcessArguments()
        {
            this.HasProcessedArguments = true;
        }

        /// <summary>
        /// Adds the arguments to the output file extension, to make it more descriptive of the performed operation.
        /// </summary>
        protected virtual void AppendArgumentsToExtension()
        {
            if (this.OperationArguments == null)
            {
                return;
            }

            // Append arguments to extension, if they're available.
            //
            for (int i = 0; i < this.OperationArguments.Length; ++i)
            {
                if (this.OperationArguments[i] != null)
                {
                    this.OutputFileExtension += $".{this.OperationArguments[i]}";
                }
            }
        }

        /// <summary>
        /// Builds the output file path and returns it.
        /// </summary>
        /// <returns>The output file path.</returns>
        public virtual string BuildOutputFilePath(bool excludeTextExtension = false)
        {
            // If we already have a path, return it.
            //
            if (this.OutputFilePath != null)
            {
                return this.OutputFilePath;
            }

            // Just in case we get called repeatedly,
            // check to avoid redundant processing.
            //
            if (!this.HasProcessedArguments)
            {
                this.ProcessArguments();
            }

            // Append the arguments to the extension.
            //
            this.AppendArgumentsToExtension();

            // Append final extension.
            //
            if (!excludeTextExtension)
            {
                this.OutputFileExtension += Constants.Files.Extensions.Txt;
            }

            // Build the output path.
            //
            this.OutputFilePath = BuildFilePath(this.InputFilePath, this.OutputFileExtension);

            // Return the path.
            //
            return this.OutputFilePath;
        }

        /// <summary>
        /// Builds a new file path from an existing file path by appending a specified extension.
        /// 
        /// Additionally:
        ///  - if the original file path had a txt extension, that will be removed before appending the new extension.
        ///  - the new extension is cleared of any characters that cannot be part of a filename.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="additionalExtension">The extension to add to the input file path.</param>
        /// <returns>The new file path.</returns>
        public static string BuildFilePath(string inputFilePath, string additionalExtension)
        {
            string path = RemoveFileExtension(inputFilePath);
            string sanitizedAdditionalExtension = SanitizeExtension(additionalExtension);

            string newPath = path + sanitizedAdditionalExtension;

            return newPath;
        }

        /// <summary>
        /// Removes special extensions from a path.
        /// 
        /// The special extensions are hardcoded here.
        /// </summary>
        /// <param name="filePath">The path to process.</param>
        /// <returns>A path without the special extensions.</returns>
        private static string RemoveFileExtension(string filePath)
        {
            if (filePath.EndsWith(Constants.Files.Extensions.Txt))
            {
                int extensionLength = Constants.Files.Extensions.Txt.Length;
                return filePath.Substring(0, filePath.Length - extensionLength);
            }

            return filePath;
        }

        /// <summary>
        /// Removes invalid characters from an extension to form a valid extension.
        /// </summary>
        /// <param name="extension">The extension to process.</param>
        /// <returns>A valid extension that does not include any restricted characters.</returns>
        private static string SanitizeExtension(string extension)
        {
            // Gather all restricted characters in an array.
            //
            string[] restrictedCharacters = new string[] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };

            // Use the restricted characters as separators to obtain the valid parts of the extension.
            //
            string[] validParts = extension.Split(restrictedCharacters, StringSplitOptions.RemoveEmptyEntries);

            // Put back together all the valid parts, separated by a #, to form a valid extension.
            //
            string sanitizedExtension = string.Join('#', validParts);

            return sanitizedExtension;
        }
    }
}
