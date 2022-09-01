﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A class that performs specific string edit operations.
    /// </summary>
    public class StringEditor
    {
        /// <summary>
        /// The type of edit operation.
        /// </summary>
        protected StringEditType EditType { get; set; }

        /// <summary>
        /// First operation argument.
        /// </summary>
        protected string FirstArgument { get; set; }

        /// <summary>
        /// Second operation argument.
        /// </summary>
        protected string SecondArgument { get; set; }

        /// <summary>
        /// First operation argument, as an integer value (set only if the operation expects an integer argument).
        /// </summary>
        protected int FirstArgumentAsInt { get; set; }

        /// <summary>
        /// Second operation argument, as an integer value (set only if the operation expects an integer argument).
        /// </summary>
        protected int SecondArgumentAsInt { get; set; }

        /// <summary>
        /// Initializes the edit operation parameters.
        /// </summary>
        /// <param name="editType">Type of operation.</param>
        /// <param name="firstArgument">First operation argument.</param>
        /// <param name="secondArgument">Second operation argument.</param>
        public void Initialize(StringEditType editType, string firstArgument, string secondArgument)
        {
            this.EditType = editType;
            this.FirstArgument = firstArgument;
            this.SecondArgument = secondArgument;

            // Validate arguments for the operation.
            switch (editType)
            {
                case StringEditType.Rewrite:
                case StringEditType.Uppercase:
                case StringEditType.Lowercase:
                case StringEditType.Invert:
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
                    ArgumentChecker.CheckPresenceAndNotEmpty(firstArgument);
                    break;

                case StringEditType.ReplaceContent:
                    ArgumentChecker.CheckPresenceAndNotEmpty(firstArgument);
                    ArgumentChecker.CheckPresence(secondArgument);
                    break;

                case StringEditType.InsertContentBeforeMarker:
                case StringEditType.InsertContentAfterMarker:
                case StringEditType.DeleteContentBetweenMarkers:
                case StringEditType.KeepContentBetweenMarkers:
                case StringEditType.KeepContentOutsideMarkers:
                    ArgumentChecker.CheckPresenceAndNotEmpty(firstArgument);
                    ArgumentChecker.CheckPresenceAndNotEmpty(secondArgument);
                    break;

                case StringEditType.DeleteFirstCharacters:
                case StringEditType.DeleteLastCharacters:
                case StringEditType.KeepFirstCharacters:
                case StringEditType.KeepLastCharacters:
                    ArgumentChecker.CheckPresence(firstArgument);

                    this.FirstArgumentAsInt = int.Parse(firstArgument);

                    ArgumentChecker.CheckNotNegative(this.FirstArgumentAsInt);
                    break;

                case StringEditType.DeleteContentAtIndex:
                case StringEditType.KeepContentAtIndex:
                    ArgumentChecker.CheckPresence(firstArgument);
                    ArgumentChecker.CheckPresence(secondArgument);

                    this.FirstArgumentAsInt = int.Parse(firstArgument);
                    this.SecondArgumentAsInt = int.Parse(secondArgument);

                    ArgumentChecker.CheckNotNegative(this.FirstArgumentAsInt);
                    ArgumentChecker.CheckPositive(this.SecondArgumentAsInt);
                    break;

                case StringEditType.InsertContentAtIndex:
                    ArgumentChecker.CheckPresenceAndNotEmpty(firstArgument);
                    ArgumentChecker.CheckPresence(secondArgument);

                    this.SecondArgumentAsInt = int.Parse(secondArgument);

                    ArgumentChecker.CheckNotNegative(this.FirstArgumentAsInt);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string edit type '{editType}'!");
            }
        }

        /// <summary>
        /// Perform an edit operation of a string using the initializationg parameters.
        /// </summary>
        /// <param name="data">The data to edit.</param>
        /// <param name="lineNumber">The line number of the data. It is produced internally.</param>
        /// <returns></returns>
        public string Edit(string data, ulong lineNumber)
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
                    break;

                case StringEditType.Uppercase:
                    editedData = data.ToUpperInvariant();
                    break;

                case StringEditType.Lowercase:
                    editedData = data.ToLowerInvariant();
                    break;

                case StringEditType.Invert:
                    editedData = InvertString(data);
                    break;

                case StringEditType.PrefixLineNumbers:
                    editedData = $"{lineNumber}{this.FirstArgument}{data}";
                    break;

                case StringEditType.AddPrefix:
                    editedData = $"{this.FirstArgument}{data}";
                    break;

                case StringEditType.AddSuffix:
                    editedData = $"{data}{this.FirstArgument}";
                    break;

                case StringEditType.DeletePrefix:
                    if (data.StartsWith(this.FirstArgument))
                    {
                        int prefixLength = this.FirstArgument.Length;
                        editedData = data.Substring(prefixLength);
                    }
                    break;

                case StringEditType.DeleteSuffix:
                    if (data.EndsWith(this.FirstArgument))
                    {
                        int suffixLength = this.FirstArgument.Length;
                        editedData = data.Substring(0, data.Length - suffixLength);
                    }
                    break;

                case StringEditType.DeleteFirstCharacters:
                    editedData = RemoveCharacters(data, this.FirstArgumentAsInt, deleteFromStart: true);
                    break;

                case StringEditType.DeleteLastCharacters:
                    editedData = RemoveCharacters(data, this.FirstArgumentAsInt, deleteFromStart: false);
                    break;

                case StringEditType.KeepFirstCharacters:
                    editedData = KeepCharacters(data, this.FirstArgumentAsInt, keepFromStart: true);
                    break;

                case StringEditType.KeepLastCharacters:
                    editedData = KeepCharacters(data, this.FirstArgumentAsInt, keepFromStart: false);
                    break;

                case StringEditType.DeleteContentAtIndex:
                    editedData = DeleteContent(data, this.FirstArgumentAsInt, this.SecondArgumentAsInt);
                    break;

                case StringEditType.KeepContentAtIndex:
                    editedData = KeepContent(data, this.FirstArgumentAsInt, this.SecondArgumentAsInt);
                    break;

                case StringEditType.InsertContentAtIndex:
                    {
                        int index = this.SecondArgumentAsInt;
                        if (index <= data.Length)
                        {
                            editedData = data.Insert(index, this.FirstArgument);
                        }
                        break;
                    }

                case StringEditType.ReplaceContent:
                    editedData = data.Replace(this.FirstArgument, this.SecondArgument);
                    break;

                case StringEditType.DeleteContentBeforeMarker:
                    {
                        int index = data.IndexOf(this.FirstArgument);
                        if (index != -1)
                        {
                            editedData = data.Substring(index);
                        }
                        break;
                    }

                case StringEditType.DeleteContentAfterMarker:
                    {
                        int index = data.IndexOf(this.FirstArgument);
                        if (index != -1)
                        {
                            index += this.FirstArgument.Length;
                            editedData = data.Substring(0, index);
                        }
                        break;
                    }

                case StringEditType.KeepContentBeforeMarker:
                    {
                        int index = data.IndexOf(this.FirstArgument);
                        if (index != -1)
                        {
                            editedData = data.Substring(0, index);
                        }
                        break;
                    }

                case StringEditType.KeepContentAfterMarker:
                    {
                        int index = data.IndexOf(this.FirstArgument);
                        if (index != -1)
                        {
                            index += this.FirstArgument.Length;
                            editedData = data.Substring(index);
                        }
                        break;
                    }

                case StringEditType.InsertContentBeforeMarker:
                    {
                        int index = data.IndexOf(this.SecondArgument);
                        if (index != -1)
                        {
                            editedData = data.Insert(index, this.FirstArgument);
                        }
                        break;
                    }

                case StringEditType.InsertContentAfterMarker:
                    {
                        int index = data.IndexOf(this.SecondArgument);
                        if (index != -1)
                        {
                            index += this.SecondArgument.Length;
                            editedData = data.Insert(index, this.FirstArgument);
                        }
                        break;
                    }

                case StringEditType.DeleteContentBetweenMarkers:
                    {
                        int indexFirstMarker;
                        int indexSecondMarker;
                        if (FindMarkers(data, this.FirstArgument, this.SecondArgument, out indexFirstMarker, out indexSecondMarker))
                        {
                            string dataPrefix = data.Substring(0, indexFirstMarker + this.FirstArgument.Length);
                            string dataSuffix = data.Substring(indexSecondMarker, data.Length - indexSecondMarker);
                            editedData = $"{dataPrefix}{dataSuffix}";
                        }
                        break;

                    }

                case StringEditType.KeepContentBetweenMarkers:
                    {
                        int indexFirstMarker;
                        int indexSecondMarker;
                        if (FindMarkers(data, this.FirstArgument, this.SecondArgument, out indexFirstMarker, out indexSecondMarker))
                        {
                            editedData = data.Substring(indexFirstMarker + this.FirstArgument.Length, indexSecondMarker - indexFirstMarker - this.FirstArgument.Length);
                        }
                        break;
                    }

                case StringEditType.KeepContentOutsideMarkers:
                    {
                        int indexFirstMarker;
                        int indexSecondMarker;
                        if (FindMarkers(data, this.FirstArgument, this.SecondArgument, out indexFirstMarker, out indexSecondMarker))
                        {
                            string dataPrefix = data.Substring(0, indexFirstMarker);
                            string dataSuffix = data.Substring(indexSecondMarker + this.SecondArgument.Length, data.Length - indexSecondMarker - this.SecondArgument.Length);
                            editedData = $"{dataPrefix}{dataSuffix}";
                        }
                        break;

                    }

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string edit type '{this.EditType}'");
            }

            return editedData;
        }

        /// <summary>
        /// Invert the character sequence of a string value.
        /// </summary>
        /// <param name="data">The string value to invert.</param>
        /// <returns>The inverted string value.</returns>
        public static string InvertString(string data)
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
        /// Remove the specified number of characters from the start or end of string.
        /// </summary>
        /// <param name="data">The string value from which to remove the characters.</param>
        /// <param name="countCharacters">The count of characters to remove.</param>
        /// <param name="deleteFromStart">Whether to delete from the start or from the end of the string.</param>
        /// <returns>If the string has enough characters, it returns the truncated string; otherwise, it returns an empty string.</returns>
        public static string RemoveCharacters(string data, int countCharacters, bool deleteFromStart)
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
        /// Remove all except the specified number of characters from the start or end of string.
        /// </summary>
        /// <param name="data">The string value from which to remove characters.</param>
        /// <param name="countCharacters">The count of characters to keep.</param>
        /// <param name="keepFromStart">Whether to keep the characters from the start or from the end of the string.</param>
        /// <returns>If the string has less characters, it returns the original string; otherwise, it returns the string truncated to the specified number of characters.</returns>
        public static string KeepCharacters(string data, int countCharacters, bool keepFromStart)
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
        /// Remove a substring identified by index and length.
        /// </summary>
        /// <param name="data">The string value from which to remove the substring.</param>
        /// <param name="contentStartIndex">The index at which the substring begins.</param>
        /// <param name="contentLength">The length of the substring to remove.</param>
        /// <returns>Returns the portion of the original string that falls outside the bounds of the specified interval.</returns>
        public static string DeleteContent(string data, int contentStartIndex, int contentLength)
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
        /// Keep a substring identified by index and length.
        /// </summary>
        /// <param name="data">The string value from which to keep the substring.</param>
        /// <param name="contentStartIndex">The index at which the substring begins.</param>
        /// <param name="contentLength">The length of the substring to keep.</param>
        /// <returns>Returns the portion of the original string that falls inside the bounds of the specified interval.</returns>
        public static string KeepContent(string data, int contentStartIndex, int contentLength)
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
    }
}