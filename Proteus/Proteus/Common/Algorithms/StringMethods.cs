////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Algorithms
{
    public abstract class StringMethods
    {
        /// <summary>
        /// Inverts the character sequence of a string value.
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
        /// Removes the specified number of characters from the start or end of string.
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
        /// Removes all except the specified number of characters from the start or end of string.
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
        /// Removes a substring identified by index and length.
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
        /// Keeps a substring identified by index and length.
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
        /// This method finds the first occurrence of each marker in the input string.
        /// </summary>
        /// <param name="data">The string in which to search for the markers.</param>
        /// <param name="firstMarker">The first marker.</param>
        /// <param name="secondMarker">The second marker.</param>
        /// <param name="indexFirstMarker">The output parameter into which to write the index at which the first marker was found.</param>
        /// <param name="indexSecondMarker">The output parameter into which to write the index at which the second marker was found.</param>
        /// <returns>True if both markers were found and false otherwise.</returns>
        public static bool FindMarkers(string data, string firstMarker, string secondMarker, out int indexFirstMarker, out int indexSecondMarker)
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
        public static bool FindLastMarkers(string data, string firstMarker, string secondMarker, out int indexFirstMarker, out int indexSecondMarker)
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
        public static bool FindInnermostMarkers(string data, string firstMarker, string secondMarker, out int indexFirstMarker, out int indexSecondMarker)
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
        public static bool FindOutermostMarkers(string data, string firstMarker, string secondMarker, out int indexFirstMarker, out int indexSecondMarker)
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
        /// Deletes the content found between two string markers.
        /// </summary>
        /// <param name="data">The input string data.</param>
        /// <param name="firstMarker">The first marker string</param>
        /// <param name="indexFirstMarker">The index where the first marker string was found in the input data string.</param>
        /// <param name="indexSecondMarker">The index where the second marker string was found in the input data string.</param>
        /// <returns>Returns the edited string.</returns>
        internal static string DeleteContentBetweenMarkers(string data, string firstMarker, int indexFirstMarker, int indexSecondMarker)
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
        internal static string KeepContentBetweenMarkers(string data, string firstMarker, int indexFirstMarker, int indexSecondMarker)
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
        internal static string KeepContentOutsideMarkers(string data, string secondMarker, int indexFirstMarker, int indexSecondMarker)
        {
            string dataPrefix = data.Substring(0, indexFirstMarker);
            string dataSuffix = data.Substring(indexSecondMarker + secondMarker.Length, data.Length - indexSecondMarker - secondMarker.Length);
            return $"{dataPrefix}{dataSuffix}";
        }
    }
}
