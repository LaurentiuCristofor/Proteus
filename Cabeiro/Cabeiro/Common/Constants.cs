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

            public const string EditLines = "el";

            public const string EditColumnValues = "ecv";

            public const string SelectLinesHavingColumnValue = "slhcv";

            public const string SelectLinesByLineNumber = "slbln";

            public const string SelectColumnsByColumnNumber = "scbcn";

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

                public const string LineNumberSelectionFirst = "first";
                public const string LineNumberSelectionNotFirst = "nfirst";
                public const string LineNumberSelectionLast = "last";
                public const string LineNumberSelectionNotLast = "nlast";
                public const string LineNumberSelectionBetween = "btwn";
                public const string LineNumberSelectionNotBetween = "nbtwn";
                public const string LineNumberSelectionEach = "each";
                public const string LineNumberSelectionNotEach = "neach";

                public const string ColumnNumberSelectionBetween = "btwn";
                public const string ColumnNumberSelectionNotBetween = "nbtwn";

                public const string DataTypeString = "s";
                public const string DataTypeInteger = "i";
                public const string DataTypeUnsignedInteger = "ui";
                public const string DataTypeFloatingPoint = "f";
                public const string DataTypeDateTime = "dt";

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
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonLessThan}' = less than"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonLessThanOrEqual}' = less than or equal"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonEqual}' = equal"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonGreaterThanOrEqual}' = greater than or equal"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonGreaterThan}' = greater than"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonNotEqual}' = not equal"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonBetween}' = between"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonStrictlyBetween}' = strictly between"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonNotBetween}' = not between"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonNotStrictlyBetween}' = not strictly between"
                        ;

                    public const string SelectionType = "<selection_type>";

                    public static readonly string LineNumberSelectionTypeText = $"\n\n{Constants.Commands.Arguments.Descriptions.SelectionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.LineNumberSelectionFirst}' = first"
                        + $"\n\t- '{Constants.Commands.Arguments.LineNumberSelectionNotFirst}' = not first"
                        + $"\n\t- '{Constants.Commands.Arguments.LineNumberSelectionLast}' = last"
                        + $"\n\t- '{Constants.Commands.Arguments.LineNumberSelectionNotLast}' = not last"
                        + $"\n\t- '{Constants.Commands.Arguments.LineNumberSelectionBetween}' = between"
                        + $"\n\t- '{Constants.Commands.Arguments.LineNumberSelectionNotBetween}' = not between"
                        + $"\n\t- '{Constants.Commands.Arguments.LineNumberSelectionEach}' = each"
                        + $"\n\t- '{Constants.Commands.Arguments.LineNumberSelectionNotEach}' = not each"
                        ;

                    public static readonly string ColumnNumberSelectionTypeText = $"\n\n{Constants.Commands.Arguments.Descriptions.SelectionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.ColumnNumberSelectionBetween}' = between"
                        + $"\n\t- '{Constants.Commands.Arguments.ColumnNumberSelectionNotBetween}' = not between"
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
