////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
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
                    editedData = InvertString(data);
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
                    editedData = RemoveCharacters(data, this.FirstCharCount, deleteFromStart: true);
                    break;

                case StringEditType.DeleteLastCharacters:
                    editedData = RemoveCharacters(data, this.FirstCharCount, deleteFromStart: false);
                    break;

                case StringEditType.KeepFirstCharacters:
                    editedData = KeepCharacters(data, this.FirstCharCount, keepFromStart: true);
                    break;

                case StringEditType.KeepLastCharacters:
                    editedData = KeepCharacters(data, this.FirstCharCount, keepFromStart: false);
                    break;

                case StringEditType.DeleteContentAtIndex:
                    editedData = DeleteContent(data, this.FirstCharCount, this.SecondCharCount);
                    break;

                case StringEditType.KeepContentAtIndex:
                    editedData = KeepContent(data, this.FirstCharCount, this.SecondCharCount);
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
                            editedData = DeleteContentAfterMarker(data, this.FirstString, index);
                        }
                        break;
                    }

                case StringEditType.DeleteContentAfterLastMarker:
                    {
                        int index = data.LastIndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = DeleteContentAfterMarker(data, this.FirstString, index);
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
                        break;
                    }

                case StringEditType.KeepContentBeforeLastMarker:
                    {
                        int index = data.LastIndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = data.Substring(0, index);
                        }
                        break;
                    }

                case StringEditType.KeepContentAfterMarker:
                    {
                        int index = data.IndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = KeepContentAfterMarker(data, this.FirstString, index);
                        }
                        break;
                    }

                case StringEditType.KeepContentAfterLastMarker:
                    {
                        int index = data.LastIndexOf(this.FirstString);
                        if (index != -1)
                        {
                            editedData = KeepContentAfterMarker(data, this.FirstString, index);
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
                            editedData = InsertContentAfterMarker(data, this.SecondString, index, this.FirstString);
                        }
                        break;
                    }

                case StringEditType.InsertContentAfterLastMarker:
                    {
                        int index = data.LastIndexOf(this.SecondString);
                        if (index != -1)
                        {
                            editedData = InsertContentAfterMarker(data, this.SecondString, index, this.FirstString);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenMarkers:
                    {
                        if (FindMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = DeleteContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenLastMarkers:
                    {
                        if (FindLastMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = DeleteContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenInnermostMarkers:
                    {
                        if (FindInnermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = DeleteContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenOutermostMarkers:
                    {
                        if (FindOutermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = DeleteContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenMarkers:
                    {
                        if (FindMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = KeepContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenLastMarkers:
                    {
                        if (FindLastMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = KeepContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenInnermostMarkers:
                    {
                        if (FindInnermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = KeepContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentBetweenOutermostMarkers:
                    {
                        if (FindOutermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = KeepContentBetweenMarkers(data, this.FirstString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideMarkers:
                    {
                        if (FindMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = KeepContentOutsideMarkers(data, this.SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideLastMarkers:
                    {
                        if (FindLastMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = KeepContentOutsideMarkers(data, this.SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideInnermostMarkers:
                    {
                        if (FindInnermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = KeepContentOutsideMarkers(data, this.SecondString, indexFirstMarker, indexSecondMarker);
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideOutermostMarkers:
                    {
                        if (FindOutermostMarkers(data, this.FirstString, this.SecondString, out int indexFirstMarker, out int indexSecondMarker))
                        {
                            editedData = KeepContentOutsideMarkers(data, this.SecondString, indexFirstMarker, indexSecondMarker);
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

        /// <summary>
        /// Inverts the character sequence of a string value.
        /// </summary>
        /// <param name="data">The string value to invert.</param>
        /// <returns>The inverted string value.</returns>
        protected static string InvertString(string data)
        {
            char[] valueChars = data.ToCharArray();
            char[] invertedChars = new char[valueChars.Length];

            for (int i = 0; i < valueChars.Length; i++)
            {
                invertedChars[valueChars.Length - 1 - i] = valueChars[i];
            }

            string invertedData = new string(invertedChars);

            return invertedData;
        }

        /// <summary>
        /// Removes the specified number of characters from the start or end of string.
        /// </summary>
        /// <param name="data">The string value from which to remove the characters.</param>
        /// <param name="countCharacters">The count of characters to remove.</param>
        /// <param name="deleteFromStart">Whether to delete from the start or from the end of the string.</param>
        /// <returns>If the string has enough characters, it returns the truncated string; otherwise, it returns an empty string.</returns>
        protected static string RemoveCharacters(string data, int countCharacters, bool deleteFromStart)
        {
            if (countCharacters >= data.Length)
            {
                return string.Empty;
            }

            if (deleteFromStart)
            {
                return data.Substring(countCharacters, data.Length - countCharacters);
            }
            else
            {
                return data.Substring(0, data.Length - countCharacters);
            }
        }

        /// <summary>
        /// Removes all except the specified number of characters from the start or end of string.
        /// </summary>
        /// <param name="data">The string value from which to remove characters.</param>
        /// <param name="countCharacters">The count of characters to keep.</param>
        /// <param name="keepFromStart">Whether to keep the characters from the start or from the end of the string.</param>
        /// <returns>If the string has less characters, it returns the original string; otherwise, it returns the string truncated to the specified number of characters.</returns>
        protected static string KeepCharacters(string data, int countCharacters, bool keepFromStart)
        {
            if (countCharacters == 0)
            {
                return string.Empty;
            }
            else if (countCharacters >= data.Length)
            {
                return data;
            }

            if (keepFromStart)
            {
                return data.Substring(0, countCharacters);
            }
            else
            {
                return data.Substring(data.Length - countCharacters, countCharacters);
            }
        }

        /// <summary>
        /// Removes a substring identified by index and length.
        /// </summary>
        /// <param name="data">The string value from which to remove the substring.</param>
        /// <param name="contentStartIndex">The index at which the substring begins.</param>
        /// <param name="contentLength">The length of the substring to remove.</param>
        /// <returns>Returns the portion of the original string that falls outside the bounds of the specified interval.</returns>
        protected static string DeleteContent(string data, int contentStartIndex, int contentLength)
        {
            if (contentStartIndex >= data.Length)
            {
                return data;
            }

            int contentEndIndex = contentStartIndex + contentLength - 1;

            if (contentEndIndex > data.Length - 1)
            {
                contentEndIndex = data.Length - 1;
            }

            string prefix = data.Substring(0, contentStartIndex);
            string suffix = data.Substring(contentEndIndex + 1);

            return prefix + suffix;
        }

        /// <summary>
        /// Keeps a substring identified by index and length.
        /// </summary>
        /// <param name="data">The string value from which to keep the substring.</param>
        /// <param name="contentStartIndex">The index at which the substring begins.</param>
        /// <param name="contentLength">The length of the substring to keep.</param>
        /// <returns>Returns the portion of the original string that falls inside the bounds of the specified interval.</returns>
        protected static string KeepContent(string data, int contentStartIndex, int contentLength)
        {
            if (contentStartIndex >= data.Length)
            {
                return string.Empty;
            }

            if (contentStartIndex + contentLength > data.Length)
            {
                contentLength = data.Length - contentStartIndex;
            }

            return data.Substring(contentStartIndex, contentLength);
        }

        /// <summary>
        /// Finds the indexes of two string markers; the second marker is expected to be found after the first.
        /// This method finds the first occurrence of each marker in the input string.
        /// </summary>
        /// <param name="data">The string in which to search for the markers.</param>
        /// <param name="firstMarker">The first marker.</param>
        /// <param name="secondMarker">The second marker.</param>
        /// <param name="indexFirstMarker">The output parameter into which to write the index at which the first marker was found.</param>
        /// <param name="indexSecondMarker">The output parameter into which to write the index at which the second marker was found.</param>
        /// <returns>True if both markers were found and false otherwise.</returns>
        protected static bool FindMarkers(string data, string firstMarker, string secondMarker, out int indexFirstMarker, out int indexSecondMarker)
        {
            indexSecondMarker = -1;

            indexFirstMarker = data.IndexOf(firstMarker);
            if (indexFirstMarker == -1 || indexFirstMarker + firstMarker.Length > data.Length - secondMarker.Length)
            {
                return false;
            }

            indexSecondMarker = data.IndexOf(secondMarker, indexFirstMarker + firstMarker.Length);
            if (indexSecondMarker == -1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finds the indexes of two string markers; the second marker is expected to be found after the first.
        /// This method finds the last occurrence of each marker in the input string.
        /// </summary>
        /// <param name="data">The string in which to search for the markers.</param>
        /// <param name="firstMarker">The first marker.</param>
        /// <param name="secondMarker">The second marker.</param>
        /// <param name="indexFirstMarker">The output parameter into which to write the index at which the first marker was found.</param>
        /// <param name="indexSecondMarker">The output parameter into which to write the index at which the second marker was found.</param>
        /// <returns>True if both markers were found and false otherwise.</returns>
        protected static bool FindLastMarkers(string data, string firstMarker, string secondMarker, out int indexFirstMarker, out int indexSecondMarker)
        {
            indexSecondMarker = -1;

            indexFirstMarker = data.LastIndexOf(firstMarker);
            if (indexFirstMarker == -1 || indexFirstMarker + firstMarker.Length > data.Length - secondMarker.Length)
            {
                return false;
            }

            indexSecondMarker = data.LastIndexOf(secondMarker, data.Length - 1, data.Length - indexFirstMarker - firstMarker.Length);
            if (indexSecondMarker == -1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finds the indexes of two string markers; the second marker is expected to be found after the first.
        /// This method finds the last occurrence of the first marker and the first occurrence of the second marker (after the first marker) in the input string.
        /// </summary>
        /// <param name="data">The string in which to search for the markers.</param>
        /// <param name="firstMarker">The first marker.</param>
        /// <param name="secondMarker">The second marker.</param>
        /// <param name="indexFirstMarker">The output parameter into which to write the index at which the first marker was found.</param>
        /// <param name="indexSecondMarker">The output parameter into which to write the index at which the second marker was found.</param>
        /// <returns>True if both markers were found and false otherwise.</returns>
        protected static bool FindInnermostMarkers(string data, string firstMarker, string secondMarker, out int indexFirstMarker, out int indexSecondMarker)
        {
            indexSecondMarker = -1;

            indexFirstMarker = data.LastIndexOf(firstMarker);
            if (indexFirstMarker == -1 || indexFirstMarker + firstMarker.Length > data.Length - secondMarker.Length)
            {
                return false;
            }

            indexSecondMarker = data.IndexOf(secondMarker, indexFirstMarker + firstMarker.Length);
            if (indexSecondMarker == -1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finds the indexes of two string markers; the second marker is expected to be found after the first.
        /// This method finds the first occurrence of the first marker and the last occurrence of the second marker in the input string.
        /// </summary>
        /// <param name="data">The string in which to search for the markers.</param>
        /// <param name="firstMarker">The first marker.</param>
        /// <param name="secondMarker">The second marker.</param>
        /// <param name="indexFirstMarker">The output parameter into which to write the index at which the first marker was found.</param>
        /// <param name="indexSecondMarker">The output parameter into which to write the index at which the second marker was found.</param>
        /// <returns>True if both markers were found and false otherwise.</returns>
        protected static bool FindOutermostMarkers(string data, string firstMarker, string secondMarker, out int indexFirstMarker, out int indexSecondMarker)
        {
            indexSecondMarker = -1;

            indexFirstMarker = data.IndexOf(firstMarker);
            if (indexFirstMarker == -1 || indexFirstMarker + firstMarker.Length > data.Length - secondMarker.Length)
            {
                return false;
            }

            indexSecondMarker = data.LastIndexOf(secondMarker, data.Length - 1, data.Length - indexFirstMarker - firstMarker.Length);
            if (indexSecondMarker == -1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes content after a marker string.
        /// </summary>
        /// <param name="data">The input string data.</param>
        /// <param name="marker">The marker string.</param>
        /// <param name="indexMarker">The index where the marker string was found in the input data string.</param>
        /// <returns>Returns the edited string.</returns>
        protected static string DeleteContentAfterMarker(string data, string marker, int indexMarker)
        {
            indexMarker += marker.Length;
            return data.Substring(0, indexMarker);
        }

        /// <summary>
        /// Keeps the content after a marker string.
        /// </summary>
        /// <param name="data">The input string data.</param>
        /// <param name="marker">The marker string.</param>
        /// <param name="indexMarker">The index where the marker string was found in the input data string.</param>
        /// <returns>Returns the edited string.</returns>
        protected static string KeepContentAfterMarker(string data, string marker, int indexMarker)
        {
            indexMarker += marker.Length;
            return data.Substring(indexMarker);
        }

        /// <summary>
        /// Inserts a string after a marker string.
        /// </summary>
        /// <param name="data">The input string data.</param>
        /// <param name="marker">The marker string.</param>
        /// <param name="indexMarker">The index where the marker string was found in the input data string.</param>
        /// <param name="content">The string to insert.</param>
        /// <returns>Returns the edited string.</returns>
        protected static string InsertContentAfterMarker(string data, string marker, int indexMarker, string content)
        {
            indexMarker += marker.Length;
            return data.Insert(indexMarker, content);
        }

        /// <summary>
        /// Deletes the content found between two string markers.
        /// </summary>
        /// <param name="data">The input string data.</param>
        /// <param name="firstMarker">The first marker string</param>
        /// <param name="indexFirstMarker">The index where the first marker string was found in the input data string.</param>
        /// <param name="indexSecondMarker">The index where the second marker string was found in the input data string.</param>
        /// <returns>Returns the edited string.</returns>
        protected static string DeleteContentBetweenMarkers(string data, string firstMarker, int indexFirstMarker, int indexSecondMarker)
        {
            string dataPrefix = data.Substring(0, indexFirstMarker + firstMarker.Length);
            string dataSuffix = data.Substring(indexSecondMarker, data.Length - indexSecondMarker);
            return $"{dataPrefix}{dataSuffix}";
        }

        /// <summary>
        /// Keeps the content between two string markers.
        /// </summary>
        /// <param name="data">The input string data.</param>
        /// <param name="firstMarker">The first marker string</param>
        /// <param name="indexFirstMarker">The index where the first marker string was found in the input data string.</param>
        /// <param name="indexSecondMarker">The index where the second marker string was found in the input data string.</param>
        /// <returns>Returns the edited string.</returns>
        protected static string KeepContentBetweenMarkers(string data, string firstMarker, int indexFirstMarker, int indexSecondMarker)
        {
            return data.Substring(indexFirstMarker + firstMarker.Length, indexSecondMarker - indexFirstMarker - firstMarker.Length);
        }

        /// <summary>
        /// Keeps the content outside two string markers.
        /// </summary>
        /// <param name="data">The input string data.</param>
        /// <param name="firstMarker">The first marker string</param>
        /// <param name="indexFirstMarker">The index where the first marker string was found in the input data string.</param>
        /// <param name="indexSecondMarker">The index where the second marker string was found in the input data string.</param>
        /// <returns>Returns the edited string.</returns>
        protected static string KeepContentOutsideMarkers(string data, string secondMarker, int indexFirstMarker, int indexSecondMarker)
        {
            string dataPrefix = data.Substring(0, indexFirstMarker);
            string dataSuffix = data.Substring(indexSecondMarker + secondMarker.Length, data.Length - indexSecondMarker - secondMarker.Length);
            return $"{dataPrefix}{dataSuffix}";
        }
    }
}
