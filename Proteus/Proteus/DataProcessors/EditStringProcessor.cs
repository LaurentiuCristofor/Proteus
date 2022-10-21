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
    /// StringParameters[0] - first string (if required)
    /// StringParameters[1] - second string (if required)
    /// IntParameters[0] - first char count (if required)
    /// IntParameters[1] - second char count (if required)
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
            EditType = processingParameters.OperationType;

            // Validate the arguments for each operation type.
            //
            switch (EditType)
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
                    FirstStringAsCharArray = processingParameters.StringParameters[FirstStringIndex].ToCharArray();
                    break;

                case StringEditType.PadLeft:
                case StringEditType.PadRight:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);
                    ArgumentChecker.CheckOneCharacter(processingParameters.StringParameters[FirstStringIndex]);

                    FirstStringAsChar = processingParameters.StringParameters[FirstStringIndex].ToCharArray()[0];
                    FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];

                    ArgumentChecker.CheckGreaterThanOrEqualTo(FirstCharCount, 1);
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
                    FirstString = processingParameters.StringParameters[FirstStringIndex];
                    ArgumentChecker.CheckNotNullAndNotEmpty(FirstString);
                    break;

                case StringEditType.DeleteFirstCharacters:
                case StringEditType.DeleteLastCharacters:
                case StringEditType.KeepFirstCharacters:
                case StringEditType.KeepLastCharacters:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);
                    FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];
                    ArgumentChecker.CheckGreaterThanOrEqualTo(FirstCharCount, 1);
                    break;

                case StringEditType.DeleteContentAtIndex:
                case StringEditType.KeepContentAtIndex:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, SecondCharCountIndex);

                    FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];
                    FirstCharCount = processingParameters.IntParameters[SecondCharCountIndex];

                    ArgumentChecker.CheckGreaterThanOrEqualTo(FirstCharCount, 0);
                    ArgumentChecker.CheckGreaterThanOrEqualTo(SecondCharCount, 1);
                    break;

                case StringEditType.InsertContentAtIndex:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);

                    FirstString = processingParameters.StringParameters[FirstStringIndex];
                    FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];

                    ArgumentChecker.CheckNotNullAndNotEmpty(FirstString);
                    ArgumentChecker.CheckGreaterThanOrEqualTo(FirstCharCount, 0);
                    break;

                case StringEditType.ReplaceContent:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, SecondStringIndex);

                    FirstString = processingParameters.StringParameters[FirstStringIndex];
                    SecondString = processingParameters.StringParameters[SecondStringIndex];

                    ArgumentChecker.CheckNotNullAndNotEmpty(FirstString);
                    ArgumentChecker.CheckNotNull(SecondString);
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

                    FirstString = processingParameters.StringParameters[FirstStringIndex];
                    SecondString = processingParameters.StringParameters[SecondStringIndex];

                    ArgumentChecker.CheckNotNullAndNotEmpty(FirstString);
                    ArgumentChecker.CheckNotNullAndNotEmpty(SecondString);
                    break;

                case StringEditType.Set:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    FirstString = processingParameters.StringParameters[FirstStringIndex];
                    ArgumentChecker.CheckNotNull(FirstString);
                    break;

                case StringEditType.SetIfEquals:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, SecondStringIndex);

                    FirstString = processingParameters.StringParameters[FirstStringIndex];
                    SecondString = processingParameters.StringParameters[SecondStringIndex];

                    ArgumentChecker.CheckNotNull(FirstString);
                    ArgumentChecker.CheckNotNull(SecondString);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string edit type '{EditType}'!");
            }

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            string data = lineData.ExtractedData.ToString();
            string editedData = Edit(data, lineNumber);
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

            OutputWriter.WriteLine(editedLine);

            return true;
        }

        /// <summary>
        /// Performs an edit operation of a string using the initialization parameters.
        /// </summary>
        /// <param name="data">The data to edit.</param>
        /// <param name="lineNumber">The line number of the data. It is produced internally.</param>
        /// <returns>The edited string value.</returns>
        protected string Edit(string data, ulong lineNumber)
        {
            if (data == null)
            {
                throw new ProteusException("Editing has been requested for null data!");
            }

            string editedData = data;

            switch (EditType)
            {
                case StringEditType.Rewrite:
                    // Nothing to do, keep the data unchanged.
                    //
                    break;

                case StringEditType.Invert:
                    editedData = StringOperations.InvertString(data);
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
                    editedData = data.TrimStart(FirstStringAsCharArray);
                    break;

                case StringEditType.TrimCharsEnd:
                    editedData = data.TrimEnd(FirstStringAsCharArray);
                    break;

                case StringEditType.TrimChars:
                    editedData = data.Trim(FirstStringAsCharArray);
                    break;

                case StringEditType.PadLeft:
                    editedData = data.PadLeft(FirstCharCount, FirstStringAsChar);
                    break;

                case StringEditType.PadRight:
                    editedData = data.PadRight(FirstCharCount, FirstStringAsChar);
                    break;

                case StringEditType.PrefixLineNumbers:
                    editedData = $"{lineNumber}{FirstString}{data}";
                    break;

                case StringEditType.AddPrefix:
                    editedData = $"{FirstString}{data}";
                    break;

                case StringEditType.AddSuffix:
                    editedData = $"{data}{FirstString}";
                    break;

                case StringEditType.DeletePrefix:
                    if (data.StartsWith(FirstString))
                    {
                        int prefixLength = FirstString.Length;
                        editedData = data.Substring(prefixLength);
                    }
                    break;

                case StringEditType.DeleteSuffix:
                    if (data.EndsWith(FirstString))
                    {
                        int suffixLength = FirstString.Length;
                        editedData = data.Substring(0, data.Length - suffixLength);
                    }
                    break;

                case StringEditType.DeleteFirstCharacters:
                    editedData = StringOperations.RemoveCharacters(data, FirstCharCount, deleteFromStart: true);
                    break;

                case StringEditType.DeleteLastCharacters:
                    editedData = StringOperations.RemoveCharacters(data, FirstCharCount, deleteFromStart: false);
                    break;

                case StringEditType.KeepFirstCharacters:
                    editedData = StringOperations.KeepCharacters(data, FirstCharCount, keepFromStart: true);
                    break;

                case StringEditType.KeepLastCharacters:
                    editedData = StringOperations.KeepCharacters(data, FirstCharCount, keepFromStart: false);
                    break;

                case StringEditType.DeleteContentAtIndex:
                    editedData = StringOperations.DeleteContent(data, FirstCharCount, SecondCharCount);
                    break;

                case StringEditType.KeepContentAtIndex:
                    editedData = StringOperations.KeepContent(data, FirstCharCount, SecondCharCount);
                    break;

                case StringEditType.InsertContentAtIndex:
                    {
                        int index = FirstCharCount;
                        if (index <= data.Length)
                        {
                            editedData = data.Insert(index, FirstString);
                        }
                        break;
                    }

                case StringEditType.ReplaceContent:
                    editedData = data.Replace(FirstString, SecondString);
                    break;

                case StringEditType.DeleteContentBeforeMarker:
                    {
                        int index = data.IndexOf(FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(index);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBeforeLastMarker:
                    {
                        int index = data.LastIndexOf(FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(index);
                        }
                        break;
                    }

                case StringEditType.DeleteContentAfterMarker:
                    {
                        int index = data.IndexOf(FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(0, index + FirstString.Length);
                        }
                        break;
                    }

                case StringEditType.DeleteContentAfterLastMarker:
                    {
                        int index = data.LastIndexOf(FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(0, index + FirstString.Length);
                        }
                        break;
                    }

                case StringEditType.KeepContentBeforeMarker:
                    {
                        int index = data.IndexOf(FirstString);
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
                        int index = data.LastIndexOf(FirstString);
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
                        int index = data.IndexOf(FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(index + FirstString.Length);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentAfterLastMarker:
                    {
                        int index = data.LastIndexOf(FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(index + FirstString.Length);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.InsertContentBeforeMarker:
                    {
                        int index = data.IndexOf(SecondString);
                        if (index != -1)
                        {
                            editedData = data.Insert(index, FirstString);
                        }
                        break;
                    }

                case StringEditType.InsertContentBeforeLastMarker:
                    {
                        int index = data.LastIndexOf(SecondString);
                        if (index != -1)
                        {
                            editedData = data.Insert(index, FirstString);
                        }
                        break;
                    }

                case StringEditType.InsertContentAfterMarker:
                    {
                        int index = data.IndexOf(SecondString);
                        if (index != -1)
                        {
                            editedData = data.Insert(index + SecondString.Length, FirstString);
                        }
                        break;
                    }

                case StringEditType.InsertContentAfterLastMarker:
                    {
                        int index = data.LastIndexOf(SecondString);
                        if (index != -1)
                        {
                            editedData = data.Insert(index + SecondString.Length, FirstString);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenMarkers:
                    {
                        if (StringOperations.FindMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.DeleteContentBetweenMarkers(data, FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenLastMarkers:
                    {
                        if (StringOperations.FindLastMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.DeleteContentBetweenMarkers(data, FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenInnermostMarkers:
                    {
                        if (StringOperations.FindInnermostMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.DeleteContentBetweenMarkers(data, FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenOutermostMarkers:
                    {
                        if (StringOperations.FindOutermostMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.DeleteContentBetweenMarkers(data, FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenMarkers:
                    {
                        if (StringOperations.FindMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.KeepContentBetweenMarkers(data, FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenLastMarkers:
                    {
                        if (StringOperations.FindLastMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.KeepContentBetweenMarkers(data, FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenInnermostMarkers:
                    {
                        if (StringOperations.FindInnermostMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.KeepContentBetweenMarkers(data, FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenOutermostMarkers:
                    {
                        if (StringOperations.FindOutermostMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.KeepContentBetweenMarkers(data, FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideMarkers:
                    {
                        if (StringOperations.FindMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.KeepContentOutsideMarkers(data, SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideLastMarkers:
                    {
                        if (StringOperations.FindLastMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.KeepContentOutsideMarkers(data, SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideInnermostMarkers:
                    {
                        if (StringOperations.FindInnermostMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.KeepContentOutsideMarkers(data, SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideOutermostMarkers:
                    {
                        if (StringOperations.FindOutermostMarkers(data, FirstString, SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = StringOperations.KeepContentOutsideMarkers(data, SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        else
                        {
                            editedData = string.Empty;
                        }
                        break;
                    }

                case StringEditType.Set:
                    editedData = FirstString;
                    break;

                case StringEditType.SetIfEquals:
                    if (data.Equals(FirstString))
                    {
                        editedData = SecondString;
                    }
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string edit type '{EditType}'");
            }

            return editedData;
        }
    }
}
