////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Algorithms;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that edits a string.
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// StringParameters[0] - first string (optional)
    /// StringParameters[1] - second string (optional)
    /// IntParameters[0] - first char count (optional)
    /// IntParameters[1] - second char count (optional)
    /// </summary>
    public class EditStringProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<StringEditType>, OneExtractedValue>
    {
        protected const int FirstStringIndex = 0;
        protected const int SecondStringIndex = 1;
        protected const int FirstCharCountIndex = 0;
        protected const int SecondCharCountIndex = 1;

        /// <summary>
        /// The type of edit operation.
        /// </summary>
        protected StringEditType EditType { get; set; }

        /// <summary>
        /// First string argument, if expected.
        /// </summary>
        protected string FirstString { get; set; }

        /// <summary>
        /// Second string argument, if expected.
        /// </summary>
        protected string SecondString { get; set; }

        /// <summary>
        /// First char count argument, if expected.
        /// </summary>
        protected int FirstCharCount { get; set; }

        /// <summary>
        /// Second char count argument, if expected.
        /// </summary>
        protected int SecondCharCount { get; set; }

        /// <summary>
        /// First string argument, as a char[] value (set only if the operation expects a char[] argument).
        /// </summary>
        protected char[] FirstStringAsCharArray { get; set; }

        /// <summary>
        /// First string argument, as a char value (set only if the operation expects a char argument).
        /// </summary>
        protected char FirstStringAsChar { get; set; }

        public void Initialize(OutputExtraOperationParameters<StringEditType> processingParameters)
        {
            this.EditType = processingParameters.OperationType;

            // Validate the arguments for each operation type.
            //
            switch (this.EditType)
            {
                case StringEditType.Rewrite:
                case StringEditType.Invert:
                case StringEditType.Uppercase:
                case StringEditType.Lowercase:
                case StringEditType.TrimStart:
                case StringEditType.TrimEnd:
                case StringEditType.Trim:
                    break;

                case StringEditType.TrimCharsStart:
                case StringEditType.TrimCharsEnd:
                case StringEditType.TrimChars:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckNotNullAndNotEmpty(processingParameters.StringParameters[FirstStringIndex]);
                    this.FirstStringAsCharArray = processingParameters.StringParameters[FirstStringIndex].ToCharArray();
                    break;

                case StringEditType.PadLeft:
                case StringEditType.PadRight:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);
                    ArgumentChecker.CheckOneCharacter(processingParameters.StringParameters[FirstStringIndex]);

                    this.FirstStringAsChar = processingParameters.StringParameters[FirstStringIndex].ToCharArray()[0];
                    this.FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];

                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.FirstCharCount, 1);
                    break;

                case StringEditType.PrefixLineNumbers:
                case StringEditType.AddPrefix:
                case StringEditType.AddSuffix:
                case StringEditType.DeletePrefix:
                case StringEditType.DeleteSuffix:
                case StringEditType.DeleteContentBeforeMarker:
                case StringEditType.DeleteContentAfterMarker:
                case StringEditType.KeepContentBeforeMarker:
                case StringEditType.KeepContentAfterMarker:
                case StringEditType.DeleteContentBeforeLastMarker:
                case StringEditType.DeleteContentAfterLastMarker:
                case StringEditType.KeepContentBeforeLastMarker:
                case StringEditType.KeepContentAfterLastMarker:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    this.FirstString = processingParameters.StringParameters[FirstStringIndex];
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.FirstString);
                    break;

                case StringEditType.DeleteFirstCharacters:
                case StringEditType.DeleteLastCharacters:
                case StringEditType.KeepFirstCharacters:
                case StringEditType.KeepLastCharacters:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);
                    this.FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];
                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.FirstCharCount, 1);
                    break;

                case StringEditType.DeleteContentAtIndex:
                case StringEditType.KeepContentAtIndex:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, SecondCharCountIndex);

                    this.FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];
                    this.FirstCharCount = processingParameters.IntParameters[SecondCharCountIndex];

                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.FirstCharCount, 0);
                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.SecondCharCount, 1);
                    break;

                case StringEditType.InsertContentAtIndex:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);

                    this.FirstString = processingParameters.StringParameters[FirstStringIndex];
                    this.FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];

                    ArgumentChecker.CheckNotNullAndNotEmpty(this.FirstString);
                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.FirstCharCount, 0);
                    break;

                case StringEditType.ReplaceContent:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, SecondStringIndex);

                    this.FirstString = processingParameters.StringParameters[FirstStringIndex];
                    this.SecondString = processingParameters.StringParameters[SecondStringIndex];

                    ArgumentChecker.CheckNotNullAndNotEmpty(this.FirstString);
                    ArgumentChecker.CheckNotNull(this.SecondString);
                    break;

                case StringEditType.InsertContentBeforeMarker:
                case StringEditType.InsertContentAfterMarker:
                case StringEditType.InsertContentBeforeLastMarker:
                case StringEditType.InsertContentAfterLastMarker:
                case StringEditType.DeleteContentBetweenMarkers:
                case StringEditType.KeepContentBetweenMarkers:
                case StringEditType.KeepContentOutsideMarkers:
                case StringEditType.DeleteContentBetweenLastMarkers:
                case StringEditType.KeepContentBetweenLastMarkers:
                case StringEditType.KeepContentOutsideLastMarkers:
                case StringEditType.DeleteContentBetweenInnermostMarkers:
                case StringEditType.KeepContentBetweenInnermostMarkers:
                case StringEditType.KeepContentOutsideInnermostMarkers:
                case StringEditType.DeleteContentBetweenOutermostMarkers:
                case StringEditType.KeepContentBetweenOutermostMarkers:
                case StringEditType.KeepContentOutsideOutermostMarkers:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, SecondStringIndex);

                    this.FirstString = processingParameters.StringParameters[FirstStringIndex];
                    this.SecondString = processingParameters.StringParameters[SecondStringIndex];

                    ArgumentChecker.CheckNotNullAndNotEmpty(this.FirstString);
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.SecondString);
                    break;

                case StringEditType.Set:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    this.FirstString = processingParameters.StringParameters[FirstStringIndex];
                    ArgumentChecker.CheckNotNull(this.FirstString);
                    break;

                case StringEditType.SetIfEquals:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, SecondStringIndex);

                    this.FirstString = processingParameters.StringParameters[FirstStringIndex];
                    this.SecondString = processingParameters.StringParameters[SecondStringIndex];

                    ArgumentChecker.CheckNotNull(this.FirstString);
                    ArgumentChecker.CheckNotNull(this.SecondString);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string edit type '{this.EditType}'!");
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            string data = lineData.ExtractedData.ToString();
            string editedData = this.Edit(data, lineNumber);
            string editedLine = editedData;

            // Check if we need to reconstruct a line from column parts;
            // this is the case when we've been editing a column value.
            //
            if (lineData.ExtractedColumnNumber != 0)
            {
                // Replace the column data with the edited one and join all the columns into a line.
                //
                int columnIndex = lineData.ExtractedColumnNumber - 1;
                lineData.Columns[columnIndex] = editedData;
                editedLine = string.Join(lineData.ColumnSeparator, lineData.Columns);
            }

            this.OutputWriter.WriteLine(editedLine);

            return true;
        }

        /// <summary>
        /// Performs an edit operation of a string using the initializationg parameters.
        /// </summary>
        /// <param name="data">The data to edit.</param>
        /// <param name="lineNumber">The line number of the data. It is produced internally.</param>
        /// <returns></returns>
        protected string Edit(string data, ulong lineNumber)
        {
            if (data == null)
            {
                throw new ProteusException("StringEditor has been called on null data!");
            }

            string editedData = data;

            switch (this.EditType)
            {
                case StringEditType.Rewrite:
                    // Nothing to do, keep the data unchanged.
                    //
                    break;

                case StringEditType.Invert:
                    editedData = StringMethods.InvertString(data);
                    break;

                case StringEditType.Uppercase:
                    editedData = data.ToUpperInvariant();
                    break;

                case StringEditType.Lowercase:
                    editedData = data.ToLowerInvariant();
                    break;

                case StringEditType.TrimStart:
                    editedData = data.TrimStart();
                    break;

                case StringEditType.TrimEnd:
                    editedData = data.TrimEnd();
                    break;

                case StringEditType.Trim:
                    editedData = data.Trim();
                    break;

                case StringEditType.TrimCharsStart:
                    editedData = data.TrimStart(this.FirstStringAsCharArray);
                    break;

                case StringEditType.TrimCharsEnd:
                    editedData = data.TrimEnd(this.FirstStringAsCharArray);
                    break;

                case StringEditType.TrimChars:
                    editedData = data.Trim(this.FirstStringAsCharArray);
                    break;

                case StringEditType.PadLeft:
                    editedData = data.PadLeft(this.FirstCharCount, this.FirstStringAsChar);
                    break;

                case StringEditType.PadRight:
                    editedData = data.PadRight(this.FirstCharCount, this.FirstStringAsChar);
                    break;

                case StringEditType.PrefixLineNumbers:
                    editedData = $"{lineNumber}{this.FirstString}{data}";
                    break;

                case StringEditType.AddPrefix:
                    editedData = $"{this.FirstString}{data}";
                    break;

                case StringEditType.AddSuffix:
                    editedData = $"{data}{this.FirstString}";
                    break;

                case StringEditType.DeletePrefix:
                    if (data.StartsWith(this.FirstString))
                    {
                        int prefixLength = this.FirstString.Length;
                        editedData = data.Substring(prefixLength);
                    }
                    break;

                case StringEditType.DeleteSuffix:
                    if (data.EndsWith(this.FirstString))
                    {
                        int suffixLength = this.FirstString.Length;
                        editedData = data.Substring(0, data.Length - suffixLength);
                    }
                    break;

                case StringEditType.DeleteFirstCharacters:
                    editedData = StringMethods.RemoveCharacters(data, this.FirstCharCount, deleteFromStart: true);
                    break;

                case StringEditType.DeleteLastCharacters:
                    editedData = StringMethods.RemoveCharacters(data, this.FirstCharCount, deleteFromStart: false);
                    break;

                case StringEditType.KeepFirstCharacters:
                    editedData = StringMethods.KeepCharacters(data, this.FirstCharCount, keepFromStart: true);
                    break;

                case StringEditType.KeepLastCharacters:
                    editedData = StringMethods.KeepCharacters(data, this.FirstCharCount, keepFromStart: false);
                    break;

                case StringEditType.DeleteContentAtIndex:
                    editedData = StringMethods.DeleteContent(data, this.FirstCharCount, this.SecondCharCount);
                    break;

                case StringEditType.KeepContentAtIndex:
                    editedData = StringMethods.KeepContent(data, this.FirstCharCount, this.SecondCharCount);
                    break;

                case StringEditType.InsertContentAtIndex:
                    {
                        int index = this.FirstCharCount;
                        if (index <= data.Length)
                        {
                            editedData = data.Insert(index, this.FirstString);
                        }
                        break;
                    }

                case StringEditType.ReplaceContent:
                    editedData = data.Replace(this.FirstString, this.SecondString);
                    break;

                case StringEditType.DeleteContentBeforeMarker:
                    {
                        int index = data.IndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(index);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBeforeLastMarker:
                    {
                        int index = data.LastIndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(index);
                        }
                        break;
                    }

                case StringEditType.DeleteContentAfterMarker:
                    {
                        int index = data.IndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(0, index + this.FirstString.Length);
                        }
                        break;
                    }

                case StringEditType.DeleteContentAfterLastMarker:
                    {
                        int index = data.LastIndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(0, index + this.FirstString.Length);
                        }
                        break;
                    }

                case StringEditType.KeepContentBeforeMarker:
                    {
                        int index = data.IndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(0, index);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentBeforeLastMarker:
                    {
                        int index = data.LastIndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(0, index);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentAfterMarker:
                    {
                        int index = data.IndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(index + this.FirstString.Length);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentAfterLastMarker:
                    {
                        int index = data.LastIndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(index + this.FirstString.Length);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.InsertContentBeforeMarker:
                    {
                        int index = data.IndexOf(this.SecondString);
                        if (index != -1)
                        {
                            editedData = data.Insert(index, this.FirstString);
                        }
                        break;
                    }

                case StringEditType.InsertContentBeforeLastMarker:
                    {
                        int index = data.LastIndexOf(this.SecondString);
                        if (index != -1)
                        {
                            editedData = data.Insert(index, this.FirstString);
                        }
                        break;
                    }

                case StringEditType.InsertContentAfterMarker:
                    {
                        int index = data.IndexOf(this.SecondString);
                        if (index != -1)
                        {
                            editedData = data.Insert(index + this.SecondString.Length, this.FirstString);
                        }
                        break;
                    }

                case StringEditType.InsertContentAfterLastMarker:
                    {
                        int index = data.LastIndexOf(this.SecondString);
                        if (index != -1)
                        {
                            editedData = data.Insert(index + this.SecondString.Length, this.FirstString);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenMarkers:
                    {
                        if (StringMethods.FindMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.DeleteContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenLastMarkers:
                    {
                        if (StringMethods.FindLastMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.DeleteContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenInnermostMarkers:
                    {
                        if (StringMethods.FindInnermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.DeleteContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenOutermostMarkers:
                    {
                        if (StringMethods.FindOutermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.DeleteContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenMarkers:
                    {
                        if (StringMethods.FindMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.KeepContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenLastMarkers:
                    {
                        if (StringMethods.FindLastMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.KeepContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenInnermostMarkers:
                    {
                        if (StringMethods.FindInnermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.KeepContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenOutermostMarkers:
                    {
                        if (StringMethods.FindOutermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.KeepContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideMarkers:
                    {
                        if (StringMethods.FindMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.KeepContentOutsideMarkers(data, this.SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideLastMarkers:
                    {
                        if (StringMethods.FindLastMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.KeepContentOutsideMarkers(data, this.SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideInnermostMarkers:
                    {
                        if (StringMethods.FindInnermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.KeepContentOutsideMarkers(data, this.SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideOutermostMarkers:
                    {
                        if (StringMethods.FindOutermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringMethods.KeepContentOutsideMarkers(data, this.SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.Set:
                    editedData = this.FirstString;
                    break;

                case StringEditType.SetIfEquals:
                    if (data.Equals(this.FirstString))
                    {
                        editedData = this.SecondString;
                    }
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string edit type '{this.EditType}'");
            }

            return editedData;
        }
    }
}
