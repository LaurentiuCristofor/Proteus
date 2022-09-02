////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Cabeiro.Common
{
    /// <summary>
    /// A repository of the constants used by Cabeiro.
    /// </summary>
    public abstract class Constants
    {
        // Program information strings.
        //
        public abstract class Program
        {
            public const string Name = "Cabeiro";

            public const string Copyright = "(c) Laurentiu Cristofor";

            public const string License = "This software is made available under the MIT license. Do not use it if you have not received an associated LICENSE file.";

            public static readonly string Description = $"{Constants.Program.Copyright}\n\n{Constants.Program.License}"
                + $"\n\nGeneral command syntax:\n\n\t{Constants.Program.Name} <command_name> [<command_arguments>]"
                + $"\n\nUseful notes:\n\n"
                + $"\t(1) Command names and arguments are case-insensitive.\n"
                + $"\t(2) Errors, warnings, and progress messages are always printed to standard error (you can redirect these using '2>').\n"
                + $"\t(3) The output file will be overwritten if it already exists.\n"
                + $"\t(4) A '.' is printed for each 1,000 rows, an 'o' for each 100,000, and an 'O' for each 1,000,000.\n"
                + $"\t(5) The string 'tab' can be used on the command line if you need to indicate a tab character.\n"
                + $"\t(6) Row and column numbers start from 1.\n";

            public static readonly string GetStartedTip = $"\n\nTo get started, execute '{Constants.Program.Name} help help'.";
        }

        // Formatting strings.
        //
        public abstract class Formatting
        {
            public const string TextEmphasisWings = "***";

            public const string TabSeparator = "\t";
        }

        // Command strings.
        // All of these should be lowercase.
        //
        public abstract class Commands
        {
            public const string Help = "help";

            public const string CountLines = "c";

            public const string AnalyzeLines = "al";

            public const string AnalyzeColumnValues = "acv";

            public const string Invert = "i";

            public const string Sort = "s";

            public const string SortByColumnValue = "sbcv";

            public const string EditLines = "el";

            public const string EditColumnValues = "ecv";

            public const string SelectLinesByColumnValue = "slbcv";

            public const string SelectLinesByLineNumber = "slbln";

            public const string SelectColumnsByColumnNumber = "scbcn";

            public const string SelectLinesByLineString = "slbls";

            public const string SelectLinesByColumnString = "slbcs";

            // Command notes.
            //
            public abstract class Notes
            {
                public const string ConsoleOutput = "\n\nThis command outputs to the console instead of writing to a file.";
            }

            // Command argument strings.
            //
            public abstract class Arguments
            {
                public const string Tab = "tab";
                public const string All = "all";

                public const string Categories = "categories";
                public const string Category = "category";

                public const string DataTypeString = "s";
                public const string DataTypeInteger = "i";
                public const string DataTypeUnsignedInteger = "ui";
                public const string DataTypeFloatingPoint = "f";
                public const string DataTypeDateTime = "dt";

                public const string ComparisonLessThan = "lt";
                public const string ComparisonLessThanOrEqual = "lte";
                public const string ComparisonEqual = "eq";
                public const string ComparisonGreaterThanOrEqual = "gte";
                public const string ComparisonGreaterThan = "gt";
                public const string ComparisonNotEqual = "neq";
                public const string ComparisonBetween = "btwn";
                public const string ComparisonStrictlyBetween = "sbtwn";
                public const string ComparisonNotBetween = "nbtwn";
                public const string ComparisonNotStrictlyBetween = "nsbtwn";

                public const string NumberSelectionLast = "last";
                public const string NumberSelectionNotLast = "nlast";
                public const string NumberSelectionBetween = "btwn";
                public const string NumberSelectionNotBetween = "nbtwn";
                public const string NumberSelectionEach = "each";
                public const string NumberSelectionNotEach = "neach";

                public const string StringSelectionHasLengthBetween = "btwn";
                public const string StringSelectionHasLengthNotBetween = "nbtwn";
                public const string StringSelectionIncludes= "incl";
                public const string StringSelectionNotIncludes = "nincl";
                public const string StringSelectionStartsWith = "start";
                public const string StringSelectionNotStartsWith = "nstart";
                public const string StringSelectionEndsWith = "end";
                public const string StringSelectionNotEndsWith = "nend";
                public const string StringSelectionIsDemarked = "mark";
                public const string StringSelectionIsNotDemarked = "nmark";
                public const string StringSelectionEquals = "eq";
                public const string StringSelectionNotEquals = "neq";

                public const string StringEditTypeRewrite = "rw";
                public const string StringEditTypeUppercase = "uc";
                public const string StringEditTypeLowercase = "lc";
                public const string StringEditTypeInvert = "i";
                public const string StringEditTypePrefixLineNumbers = "pln";
                public const string StringEditTypeAddPrefix = "ap";
                public const string StringEditTypeAddSuffix = "as";
                public const string StringEditTypeDeletePrefix = "dp";
                public const string StringEditTypeDeleteSuffix = "ds";
                public const string StringEditTypeDeleteFirstCharacters = "dfc";
                public const string StringEditTypeDeleteLastCharacters = "dlc";
                public const string StringEditTypeKeepFirstCharacters = "kfc";
                public const string StringEditTypeKeepLastCharacters = "klc";
                public const string StringEditTypeDeleteContentAtIndex = "dcai";
                public const string StringEditTypeKeepContentAtIndex = "kcai";
                public const string StringEditTypeInsertContentAtIndex = "icai";
                public const string StringEditTypeReplaceContent = "rc";
                public const string StringEditTypeDeleteContentBeforeMarker = "dcbm";
                public const string StringEditTypeDeleteContentAfterMarker = "dcam";
                public const string StringEditTypeKeepContentBeforeMarker = "kcbm";
                public const string StringEditTypeKeepContentAfterMarker = "kcam";
                public const string StringEditTypeInsertContentBeforeMarker = "icbm";
                public const string StringEditTypeInsertContentAfterMarker = "icam";
                public const string StringEditTypeDeleteContentBetweenMarkers = "dcb2m";
                public const string StringEditTypeKeepContentBetweenMarkers = "kcb2m";
                public const string StringEditTypeKeepContentOutsideMarkers= "kco2m";

                // Command argument description strings.
                //
                public class Descriptions
                {
                    public const string CommandName = "<command_name>";
                    public const string CategoryName = "<category_name>";
                    public const string InputFilePath = "<input_file_path>";
                    public const string OutputFilePath = "<output_file_path>";
                    public const string ColumnNumber = "<column_number>";
                    public const string ColumnSeparator = "<column_separator>";
                    public const string FirstArgument = "<first_argument>";
                    public const string SecondArgument = "<second_argument>";
                    public const string DataType = "<data_type>";

                    public static readonly string DataTypeText = $"\n\n{Constants.Commands.Arguments.Descriptions.DataType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeString}' = string"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeInteger}' = (signed) integer"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeUnsignedInteger}' = unsigned integer"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeFloatingPoint}' = floating point"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeDateTime}' = datetime"
                        ;

                    public const string ComparisonType = "<comparison_type>";

                    public static readonly string ComparisonTypeText = $"\n\n{Constants.Commands.Arguments.Descriptions.ComparisonType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonLessThan}' = less than; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonLessThanOrEqual}' = less than or equal; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonEqual}' = equal; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonGreaterThanOrEqual}' = greater than or equal; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonGreaterThan}' = greater than; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonNotEqual}' = not equal; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonBetween}' = between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonStrictlyBetween}' = strictly between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonNotBetween}' = not between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonNotStrictlyBetween}' = not strictly between; arguments: <start_value> <end_value>"
                        ;

                    public const string SelectionType = "<selection_type>";

                    public static readonly string NumberSelectionTypeText = $"\n\n{Constants.Commands.Arguments.Descriptions.SelectionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.NumberSelectionBetween}' = between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.NumberSelectionNotBetween}' = not between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.NumberSelectionLast}' = last; arguments: <last_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.NumberSelectionNotLast}' = not last; arguments: <last_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.NumberSelectionEach}' = each; arguments: <each_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.NumberSelectionNotEach}' = not each; arguments: <each_count>"
                        ;

                    public static readonly string StringSelectionTypeText = $"\n\n{Constants.Commands.Arguments.Descriptions.SelectionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionHasLengthBetween}' = has length between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionHasLengthNotBetween}' = has length not between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionIncludes}' = includes; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionNotIncludes}' = not includes; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionStartsWith}' = starts with; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionNotStartsWith}' = not starts with; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionEndsWith}' = ends with; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionNotEndsWith}' = not ends with; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionIsDemarked}' = is demarked; arguments: <prefix_value> <suffix_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionIsNotDemarked}' = is not demarked; arguments: <prefix_value> <suffix_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionEquals}' = equals; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionNotEquals}' = not equals; arguments: <string_value>"
                        ;

                    public const string EditType = "<edit_type>";

                    public static readonly string EditTypeText = $"\n\n{Constants.Commands.Arguments.Descriptions.EditType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeRewrite}' = rewrite"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeUppercase}' = uppercase"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeLowercase}' = lowercase"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeInvert}' = invert"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypePrefixLineNumbers}' = prefix line numbers"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeAddPrefix}' = add prefix; arguments: <prefix_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeAddSuffix}' = add suffix; arguments: <suffix_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeletePrefix}' = delete prefix; arguments: <prefix_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteSuffix}' = delete suffix; arguments: <suffix_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteFirstCharacters}' = delete first characters; arguments: <character_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteLastCharacters}' = delete last characters; arguments: <character_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepFirstCharacters}' = keep first characters; arguments: <character_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepLastCharacters}' = keep last characters; arguments: <character_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentAtIndex}' = delete content at index; arguments: <content_index> <content_length>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentAtIndex}' = keep content at index; arguments: <content_index> <content_length>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeInsertContentAtIndex}' = insert content at index; arguments: <content> <index>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeReplaceContent}' = replace content; arguments: <content> <replacement>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentBeforeMarker}' = delete content before marker; arguments: <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentAfterMarker}' = delete content after marker; arguments: <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentBeforeMarker}' = keep content before marker; arguments: <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentAfterMarker}' = keep content after marker; arguments: <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeInsertContentBeforeMarker}' = insert content before marker; arguments: <content> <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeInsertContentAfterMarker}' = insert content after marker; arguments: <content> <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenMarkers}' = delete content between markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentBetweenMarkers}' = keep content between markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentOutsideMarkers}' = keep content outside markers; arguments: <first_marker> <second_marker>"
                        ;

                    public const string LimitValues = "<limit_values>";

                    public static readonly string LimitValuesText = $"\n\n{Constants.Commands.Arguments.Descriptions.LimitValues} specifies how many values should be presented from the most frequent and least frequent sets of values; set this value to 0 if you want the command to produce a dump of all unique values encountered.";
                }
            }
        }

        // File strings.
        //
        public abstract class Files
        {
            // File extension strings.
            //
            public abstract class Extensions
            {
                public const string Txt = ".txt";
            }
        }
    }
}
