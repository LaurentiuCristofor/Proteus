////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    public abstract class Constants
    {
        public abstract class Strings
        {
            public const string NameValueSeparator = "=";
            public const string ListSeparator = ",";

            public const string Count = "Count";
            public const string UniqueCount = "UniqueCount";
            public const string MinimumValue = "MinimumValue";
            public const string MaximumValue = "MaximumValue";
            public const string AverageValue = "AverageValue";
            public const string ShortestStringRepresentation = "ShortestStringRepresentation";
            public const string LongestStringRepresentation = "LongestStringRepresentation";
            public const string AverageStringRepresentationLength = "AverageStringRepresentationLength";
            public const string Entropy = "Entropy";

            public const string Bottom = "Bottom";
            public const string Top = "Top";
        }

        public abstract class Messages
        {
            public const string LineTooShortForColumnExtraction = "\nLine {0} is too short for extracting column number {1}!";
            public const string InvalidValueForColumnExtraction = "\nFound an invalid value for column {0} of type {1} in line {2}: '{3}'!";

            public const string NoDataFoundForAnalysis = "No data was found to analyze!";

            public const string AnalysisPostProcessingStart = "Post-processing analyzed data...";
            public const string AnalysisPostProcessingEnd = "done! Post-processing time: ";

            public const string SortingStart = "Sorting...";
            public const string SortingEnd = "done! Sorting time: ";

            public const string LinesReadFromFile = "lines were read from file";
            public const string LinesWrittenToFile = "lines were written to file";
            public const string LinesReadFromFirstFile = "lines were read from first file";
            public const string LinesReadFromSecondFile = "lines were read from second file";
            public const string LinesReadFromLookupFile = "lines were read from lookup file";
            public const string LinesReadFromDataFile = "lines were read from data file";

            public const string TotalProcessingTime = "Total processing time: ";
        }
    }
}
