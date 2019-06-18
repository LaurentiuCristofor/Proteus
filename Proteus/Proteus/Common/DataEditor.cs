////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A class that performs specific edit operations on input data.
    /// </summary>
    public abstract class DataEditor
    {
        /// <summary>
        /// Perform an edit operation of a string using the provided parameters.
        /// </summary>
        /// <param name="data">The data to edit.</param>
        /// <param name="editType">The type of edit operation to perform.</param>
        /// <param name="lineNumber">The line number of the data. It is produced internally.</param>
        /// <param name="firstArgument">A first argument of the edit operation. It comes from an external source.</param>
        /// <param name="secondArgument">A second argument of the edit operation. It comes for an external source.</param>
        /// <returns></returns>
        public static string Edit(string data, StringEditType editType, ulong lineNumber, string firstArgument, string secondArgument)
        {
            string editedData = data;

            switch (editType)
            {
                case StringEditType.Rewrite:
                    // Nothin to do, keep the data unchanged.
                    break;

                case StringEditType.Invert:
                    editedData = InvertString(data);
                    break;

                case StringEditType.PrefixLineNumbers:
                    ArgumentChecker.CheckPresence(firstArgument);

                    editedData = $"{lineNumber}{firstArgument}{data}";
                    break;

                case StringEditType.AddPrefix:
                    ArgumentChecker.CheckPresence(firstArgument);

                    editedData = $"{firstArgument}{data}";
                    break;

                case StringEditType.AddSuffix:
                    ArgumentChecker.CheckPresence(firstArgument);

                    editedData = $"{data}{firstArgument}";
                    break;

                case StringEditType.DeletePrefix:
                    ArgumentChecker.CheckPresence(firstArgument);

                    if (data.StartsWith(firstArgument))
                    {
                        int prefixLength = firstArgument.Length;
                        editedData = data.Substring(prefixLength);
                    }
                    break;

                case StringEditType.DeleteSuffix:
                    ArgumentChecker.CheckPresence(firstArgument);

                    if (data.EndsWith(firstArgument))
                    {
                        int suffixLength = firstArgument.Length;
                        editedData = data.Substring(0, data.Length - suffixLength);
                    }
                    break;

                case StringEditType.DeleteContent:
                    {
                        ArgumentChecker.CheckPresence(firstArgument);
                        ArgumentChecker.CheckPresence(secondArgument);

                        int contentStartIndex = int.Parse(firstArgument);
                        int contentLength = int.Parse(secondArgument);

                        editedData = RemoveContent(data, contentStartIndex, contentLength);
                        break;
                    }

                case StringEditType.DeleteContentBeforeMarker:
                    {
                        ArgumentChecker.CheckPresence(firstArgument);

                        int index = data.IndexOf(firstArgument);
                        if (index != -1)
                        {
                            editedData = data.Substring(index);
                        }
                        break;
                    }

                case StringEditType.DeleteContentAfterMarker:
                    {
                        ArgumentChecker.CheckPresence(firstArgument);

                        int index = data.LastIndexOf(firstArgument);
                        if (index != -1)
                        {
                            index += firstArgument.Length;
                            editedData = data.Substring(0, index);
                        }
                        break;
                    }

                case StringEditType.InsertContentBeforeMarker:
                    {
                        ArgumentChecker.CheckPresence(firstArgument);
                        ArgumentChecker.CheckPresence(secondArgument);

                        int index = data.IndexOf(secondArgument);
                        if (index != -1)
                        {
                            editedData = data.Insert(index, firstArgument);
                        }
                        break;
                    }

                case StringEditType.InsertContentAfterMarker:
                    {
                        ArgumentChecker.CheckPresence(firstArgument);
                        ArgumentChecker.CheckPresence(secondArgument);

                        int index = data.LastIndexOf(secondArgument);
                        if (index != -1)
                        {
                            index += secondArgument.Length;
                            editedData = data.Insert(index, firstArgument);
                        }
                        break;
                    }

                case StringEditType.InsertContentAtIndex:
                    {
                        ArgumentChecker.CheckPresence(firstArgument);
                        ArgumentChecker.CheckPresence(secondArgument);

                        int index = int.Parse(secondArgument);
                        if (index <= data.Length)
                        {
                            editedData = data.Insert(index, firstArgument);
                        }
                        break;
                    }

                case StringEditType.ReplaceContent:
                    ArgumentChecker.CheckPresence(firstArgument);
                    ArgumentChecker.CheckPresence(secondArgument);

                    editedData = data.Replace(firstArgument, secondArgument);
                    break;

                case StringEditType.Uppercase:
                    editedData = data.ToUpperInvariant();
                    break;

                case StringEditType.Lowercase:
                    editedData = data.ToLowerInvariant();
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data operation type '{editType}'");
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
            if (data == null)
            {
                return null;
            }

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
        /// Remove a substring identified by index and length.
        /// </summary>
        /// <param name="data">The string value from which to remove the substring.</param>
        /// <param name="contentStartIndex">The index at which the substring begins.</param>
        /// <param name="contentLength">The length of the substring to remove.</param>
        /// <returns></returns>
        public static string RemoveContent(string data, int contentStartIndex, int contentLength)
        {
            if (data == null)
            {
                return null;
            }

            int contentEndIndex = contentStartIndex + contentLength - 1;

            if (contentEndIndex < data.Length)
            {
                string prefix = data.Substring(0, contentStartIndex);
                string suffix = data.Substring(contentEndIndex + 1);

                return prefix + suffix;
            }

            return data;
        }
    }
}
