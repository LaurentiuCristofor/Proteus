////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
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

            public const string Url = "https://github.com/LaurentiuCristofor/Proteus";

            public const string License = "This software is made available under the MIT license. Do not use it if you have not received an associated LICENSE file.";

            public static readonly string Description = $"{Constants.Program.Copyright}\n{Constants.Program.Url}\n\n{Constants.Program.License}"
                + $"\n\nGeneral command syntax:\n\n\t{Constants.Program.Name} <command_name> [<command_arguments>]"
                + $"\n\nUseful notes:\n\n"
                + $"\t(1) Command names and arguments are case-insensitive.\n"
                + $"\t(2) Errors, warnings, and progress messages are always printed to standard error (you can redirect these using '2>').\n"
                + $"\t(3) The output file will be overwritten if it already exists.\n"
                + $"\t(4) A '.' is printed for each 1,000 rows, an 'o' for each 100,000, and an 'O' for each 1,000,000.\n"
                + $"\t(5) The string 'tab' can be used on the command line if you need to indicate a tab character as column separator.\n"
                + $"\t(6) Row and column numbers start from 1.\n";

            public static readonly string GetStartedTip = $"\nTo get started, execute '{Constants.Program.Name} help help'.";
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
            public const string CustomSort = "cs";
            public const string CustomSortByColumnValue = "csbcv";
            public const string Shuffle = "sh";
            public const string OrderColumns = "oc";

            public const string EditLines = "el";
            public const string EditColumnStrings = "ecs";
            public const string EditColumnValues = "ecv";
            public const string InsertLine = "il";
            public const string JoinLines = "jl";
            public const string ConcatenateLines = "cl";
            public const string TransformLines = "tl";
            public const string TransformColumns = "tc";

            public const string SelectLinesByColumnValue = "slbcv";
            public const string SelectLinesByNumber = "slbn";
            public const string SelectColumnsByNumber = "scbn";
            public const string SelectLinesByLineString = "slbls";
            public const string SelectLinesByColumnString = "slbcs";
            public const string SelectLinesByColumnCount = "slbcc";
            public const string SelectLinesByLineStringRelativeToOtherLines = "slblsr";
            public const string SelectLinesByColumnValueRelativeToOtherLines = "slbcvr";
            public const string SelectLinesByLookupInFile = "slblif";
            public const string SelectLinesByColumnValueLookupInFile = "slbcvlif";
            public const string SelectLinesSample = "sls";

            public const string SplitLineRanges = "splr";
            public const string SplitColumns = "spc";
            public const string SplitColumnValues = "spcv";
            public const string SplitLinesIntoRandomSets = "splirs";

            public const string SortBySecondColumnValue = "sb2cv";
            public const string MergeLines = "ml";
            public const string MergeLinesByColumnValue = "mlbcv";
            public const string SelectLinesPostSortingByLineStringRelativeToOtherLines = "slpsblsr";
            public const string SelectLinesPostSortingByColumnValueRelativeToOtherLines = "slpsbcvr";
            public const string SelectLinesPostSortingByLookupInFile = "slpsblif";
            public const string SelectLinesPostSortingByColumnValueLookupInFile = "slpsbcvlif";
            public const string JoinLinesPostSorting = "jlps";
            public const string FindStateTransitions = "fst";

            public const string GenerateDistribution = "gend";
            public const string GenerateSample = "gens";

            // Command descriptions.
            //
            public abstract class Descriptions
            {
                public const string Invert = "Inverts the order of the lines of the input file, making the last line become the first one.";

                public const string AnalyzeLines = "Produces a report about the line values.";
                public const string AnalyzeColumnValues = "Produces a report about a specified column's values.";

                public const string SelectLinesByColumnValue = "Selects lines by comparing a column's value against specified arguments.";
                public const string SelectLinesByLineString = "Selects lines based on their string value matching specified properties.";
                public const string SelectLinesByColumnString = "Selects lines based on a column's string value matching specified properties.";
                public const string SelectLinesByLineStringRelativeToOtherLines = "Selects lines, based on their relationship to other lines.";
                public const string SelectLinesByColumnValueRelativeToOtherLines = "Selects lines, based on a column value's relationship to those of other lines.";
                public const string SelectLinesByLookupInFile = "Selects lines based on whether they appear in another file or not.";
                public const string SelectLinesByColumnValueLookupInFile = "Selects lines based on whether their specified column's value appears in another file or not.";

                public const string SplitLineRanges = "Splits a file into multiple files, each containing a range of lines of specified size. Last file will likely have fewer lines than the rest.";
                public const string SplitColumns = "Splits a file into multiple files, one for each column. Resulting files may have different size than the original file, if the original file contained lines with varying number of columns.";
                public const string SplitColumnValues = "Splits a file into multiple files, one for each different value present in a specified column. Lines that do not have the specified column present are ignored.";
                public const string SplitLinesIntoRandomSets = "Randomly splits the lines of a file into the specified number of sets. If the number of sets is larger than the number of lines, some of the sets will be empty.";

                public const string MergeLines = "Merges lines from two sorted files. Does not verify the correct sorting of the input files.";
                public const string MergeLinesByColumnValue = "Merges lines from two files sorted on a specific column. Does not verify the correct sorting of the input files.";
                public const string SelectLinesPostSortingByLineStringRelativeToOtherLines = "Selects lines from a sorted file, based on their relationship to other lines.";
                public const string SelectLinesPostSortingByColumnValueRelativeToOtherLines = "Selects lines from a column-sorted file, based on that column value's relationship to those of other lines.";
                public const string SelectLinesPostSortingByLookupInFile = "Selects lines from a sorted file based on whether they appear in another sorted file or not. Does not verify the correct sorting of the input files.";
                public const string SelectLinesPostSortingByColumnValueLookupInFile = "Selects lines from a column-sorted file based on whether their sorted column's value appears in the first column of another sorted file or not. Does not verify the correct sorting of the input files.";
                public const string JoinLinesPostSorting = "Joins the lines of two column-sorted files. Does not verify the correct sorting of the input files.";
                public const string FindStateTransitions = "Selects pairs of lines from a column-sorted file, which match on the sorted column's values, but differ on a second column's 'state' values. Two consecutive transitions will result in duplicate lines being output; for example, ABA will output AB and BA.";

                public const string GenerateDistribution = "Generates a number of values with the specified distribution.";
                public const string GenerateSample = "Generates a sample of values out of values ranging from 1 to a specified total count.";
            }

            // Command notes.
            //
            public abstract class Notes
            {
                public const string ConsoleOutput = "This command outputs to the console instead of writing to a file.\n";

                public const string MemoryRequirementConstant = "Memory requirement: O(1).";
                public const string MemoryRequirementConstantLine = "Memory requirement: For regular data files: O(1); more specifically: O(N), where N is the size of a line.";
                public const string MemoryRequirementConstantColumns = "Memory requirement: For regular data files: O(1); more specifically: O(N), where N is the maximum count of columns in a line.";
                public const string MemoryRequirementLinearUnique = "Memory requirement: O(N), where N is the total size of unique data values found.";
                public const string MemoryRequirementLinearTotal = "Memory requirement: O(N), where N is the total size of the input data file.";
                public const string MemoryRequirementLinearSample = "Memory requirement: O(N), where N is the size of the requested sample.";
                public const string MemoryRequirementLinearSets = "Memory requirement: O(N), where N is the number of requested sets.";
                public const string MemoryRequirementLinearTotalSecond = "Memory requirement: O(N), where N is the total size of the second input data file.";
                public const string MemoryRequirementLinearUniqueLookup = "Memory requirement: O(N), where N is the total size of unique data found in the lookup file.";
                public const string MemoryRequirementLinearPrimaryColumnRepetitions = "Memory requirement: O(N), where N is the largest number of lines having the same primary column value.";
                public const string MemoryRequirementLinearJoinKeyRepetitions = "Memory requirement: O(N), where N is the largest number of lines having the same join column value in the second input data file.";
                public const string MemoryRequirementExceptionLast = "Exception: `last` and `nlast` require O(N) where N is the argument to these commands.";
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

                public const string SortingAlgorithmTypeInsertion = "i";
                public const string SortingAlgorithmTypeShell = "sh";
                public const string SortingAlgorithmTypeMerge = "m";
                public const string SortingAlgorithmTypeQuicksort = "qs";
                public const string SortingAlgorithmTypeHeap = "h";

                public const string StringEditTypeRewrite = "rw";
                public const string StringEditTypeInvert = "i";
                public const string StringEditTypeUppercase = "uc";
                public const string StringEditTypeLowercase = "lc";
                public const string StringEditTypeTrimStart = "ts";
                public const string StringEditTypeTrimEnd = "te";
                public const string StringEditTypeTrim = "t";
                public const string StringEditTypeTrimCharsStart = "tcs";
                public const string StringEditTypeTrimCharsEnd = "tce";
                public const string StringEditTypeTrimChars = "tc";
                public const string StringEditTypePadLeft = "pl";
                public const string StringEditTypePadRight = "pr";
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
                public const string StringEditTypeDeleteContentBeforeLastMarker = "dcblm";
                public const string StringEditTypeDeleteContentAfterLastMarker = "dcalm";
                public const string StringEditTypeKeepContentBeforeLastMarker = "kcblm";
                public const string StringEditTypeKeepContentAfterLastMarker = "kcalm";
                public const string StringEditTypeInsertContentBeforeLastMarker = "icblm";
                public const string StringEditTypeInsertContentAfterLastMarker = "icalm";
                public const string StringEditTypeDeleteContentBetweenMarkers = "dcb2m";
                public const string StringEditTypeKeepContentBetweenMarkers = "kcb2m";
                public const string StringEditTypeKeepContentOutsideMarkers = "kco2m";
                public const string StringEditTypeDeleteContentBetweenLastMarkers = "dcb2lm";
                public const string StringEditTypeKeepContentBetweenLastMarkers = "kcb2lm";
                public const string StringEditTypeKeepContentOutsideLastMarkers = "kco2lm";
                public const string StringEditTypeDeleteContentBetweenInnermostMarkers = "dcb2im";
                public const string StringEditTypeKeepContentBetweenInnermostMarkers = "kcb2im";
                public const string StringEditTypeKeepContentOutsideInnermostMarkers = "kco2im";
                public const string StringEditTypeDeleteContentBetweenOutermostMarkers = "dcb2om";
                public const string StringEditTypeKeepContentBetweenOutermostMarkers = "kcb2om";
                public const string StringEditTypeKeepContentOutsideOutermostMarkers = "kco2om";
                public const string StringEditTypeSet = "s";
                public const string StringEditTypeSetIfEquals = "sie";

                public const string ValueEditTypeRewrite = "rw";
                public const string ValueEditTypeAdd = "add";
                public const string ValueEditTypeSubtract = "sub";
                public const string ValueEditTypeMultiply = "mul";
                public const string ValueEditTypeDivide = "div";

                public const string PositionInsertionTypePosition = "pos";
                public const string PositionInsertionTypeEach = "each";
                public const string PositionInsertionTypeLast = "last";

                public const string JoinTypeInner = "in";
                public const string JoinTypeLeftOuter = "lo";

                public const string LineTransformationTypeBreak = "break";
                public const string LineTransformationTypeUnite = "unite";

                public const string ColumnTransformationTypePack = "pack";
                public const string ColumnTransformationTypeUnpack = "unpack";

                public const string ComparisonTypeLessThan = "lt";
                public const string ComparisonTypeLessThanOrEqual = "lte";
                public const string ComparisonTypeEqual = "eq";
                public const string ComparisonTypeGreaterThanOrEqual = "gte";
                public const string ComparisonTypeGreaterThan = "gt";
                public const string ComparisonTypeNotEqual = "neq";
                public const string ComparisonTypeBetween = "btwn";
                public const string ComparisonTypeStrictlyBetween = "sbtwn";
                public const string ComparisonTypeNotBetween = "nbtwn";
                public const string ComparisonTypeNotStrictlyBetween = "nsbtwn";

                public const string PositionSelectionTypeLast = "last";
                public const string PositionSelectionTypeNotLast = "nlast";
                public const string PositionSelectionTypeBetween = "btwn";
                public const string PositionSelectionTypeNotBetween = "nbtwn";
                public const string PositionSelectionTypeEach = "each";
                public const string PositionSelectionTypeNotEach = "neach";

                public const string StringSelectionTypeHasLengthBetween = "btwn";
                public const string StringSelectionTypeHasLengthNotBetween = "nbtwn";
                public const string StringSelectionTypeIncludes = "incl";
                public const string StringSelectionTypeNotIncludes = "nincl";
                public const string StringSelectionTypeStartsWith = "start";
                public const string StringSelectionTypeNotStartsWith = "nstart";
                public const string StringSelectionTypeEndsWith = "end";
                public const string StringSelectionTypeNotEndsWith = "nend";
                public const string StringSelectionTypeStartsAndEndsWith = "startend";
                public const string StringSelectionTypeNotStartsAndEndsWith = "nstartend";
                public const string StringSelectionTypeEquals = "eq";
                public const string StringSelectionTypeNotEquals = "neq";

                public const string RelativeValueSelectionFirst = "first";
                public const string RelativeValueSelectionNotFirst = "nfirst";
                public const string RelativeValueSelectionLast = "last";
                public const string RelativeValueSelectionNotLast = "nlast";

                public const string LookupTypeIncluded = "incl";
                public const string LookupTypeNotIncluded = "nincl";

                public const string DataDistributionTypeUniform = "u";
                public const string DataDistributionTypeNormal = "n";
                public const string DataDistributionTypeExponential = "e";
                public const string DataDistributionTypePoisson = "p";

                // Command argument description strings.
                //
                public abstract class Descriptions
                {
                    public const string CommandName = "<command_name>";
                    public const string CategoryName = "<category_name>";

                    public const string InputFilePath = "<input_file_path>";
                    public const string FirstInputFilePath = "<first_input_file_path>";
                    public const string SecondInputFilePath = "<second_input_file_path>";
                    public const string DataFilePath = "<data_file_path>";
                    public const string LookupFilePath = "<lookup_file_path>";
                    public const string OutputFilePath = "<output_file_path>";

                    public const string ColumnNumber = "<column_number>";
                    public const string FirstFileColumnNumber = "<first_file_column_number>";
                    public const string SecondFileColumnNumber = "<second_file_column_number>";
                    public const string DataFileColumnNumber = "<data_file_column_number>";
                    public const string LookupFileColumnNumber = "<lookup_file_column_number>";
                    public const string SortedColumnNumber = "<sorted_column_number>";
                    public const string SortedColumnDataType = "<sorted_column_data_type>";

                    public const string ColumnSeparator = "<column_separator>";

                    public const string FirstArgument = "<first_argument>";
                    public const string SecondArgument = "<second_argument>";
                    public const string ThirdArgument = "<third_argument>";

                    public const string LineValue = "<line_value>";
                    public const string RangeSize = "<range_size>";
                    public const string SampleSize = "<sample_size>";
                    public const string SetsCount = "<sets_count>";
                    public const string SeedValue = "<seed_value>";
                    public const string GenerationCount = "<generation_count>";
                    public const string TotalCount = "<total_count>";
                    public const string DistributionMean = "<distribution_mean>";

                    public static readonly string HelpCategoriesText = $"{CommandName} will produce detailed information on the specified command."
                        + $"\n\n'{Commands.Arguments.All}' will list all available commands."
                        + $"\n\n'{Commands.Arguments.Categories}' will list all command categories."
                        + $"\n\n'{Commands.Arguments.Category}' will list all commands in the {Commands.Arguments.Descriptions.CategoryName} category."
                        ;

                    public const string LimitValues = "<limit_values>";

                    public static readonly string LimitValuesText = $"{Constants.Commands.Arguments.Descriptions.LimitValues} specifies how many values should be presented from the most frequent and least frequent sets of values; set this value to 0 if you want the command to produce a dump of all unique values encountered.";

                    public const string DataType = "<data_type>";

                    public static readonly string DataTypeText = $"{Constants.Commands.Arguments.Descriptions.DataType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeString}' = string"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeInteger}' = (signed) integer"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeUnsignedInteger}' = unsigned integer"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeFloatingPoint}' = floating point"
                        + $"\n\t- '{Constants.Commands.Arguments.DataTypeDateTime}' = datetime"
                        ;

                    public const string AlgorithmType = "<algorithm_type>";

                    // List less performant algorithms last.
                    //
                    public static readonly string SortingAlgorithmTypeText = $"{Constants.Commands.Arguments.Descriptions.AlgorithmType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.SortingAlgorithmTypeMerge}' = merge sorting (stable)"
                        + $"\n\t- '{Constants.Commands.Arguments.SortingAlgorithmTypeQuicksort}' = quicksort (in-place)"
                        + $"\n\t- '{Constants.Commands.Arguments.SortingAlgorithmTypeHeap}' = heap sorting (in-place)"
                        + $"\n\t- '{Constants.Commands.Arguments.SortingAlgorithmTypeShell}' = Shell sorting (in-place)"
                        + $"\n\t- '{Constants.Commands.Arguments.SortingAlgorithmTypeInsertion}' = insertion sorting (quadratic!, stable, in-place)"
                        ;

                    public const string NewFirstColumnsList = "<new_first_columns_list>";

                    public static readonly string NewFirstColumnsListText = $"{Constants.Commands.Arguments.Descriptions.NewFirstColumnsList} is a comma-separated list of column numbers representing the new order of columns requested. Unspecified columns will appear after the specified ones, in their initial order.";

                    public const string EditType = "<edit_type>";

                    public static readonly string StringEditTypeText = $"{Constants.Commands.Arguments.Descriptions.EditType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeRewrite}' = rewrite"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeInvert}' = invert"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeUppercase}' = uppercase"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeLowercase}' = lowercase"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeTrimStart}' = trim whitespace at start"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeTrimEnd}' = trim whitespace at end"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeTrim}' = trim whitespace"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeTrimCharsStart}' = trim characters at start; arguments: <string_of_characters_to_trim>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeTrimCharsEnd}' = trim characters at end; arguments: <string_of_characters_to_trim>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeTrimChars}' = trim characters; arguments: <string_of_characters_to_trim>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypePadLeft}' = pad left; arguments: <padding_size> <padding_character>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypePadRight}' = pad right; arguments: <padding_size> <padding_character>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypePrefixLineNumbers}' = prefix line numbers; arguments: <separator_value>"
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
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentBeforeLastMarker}' = delete content before last occurrence of marker; arguments: <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentAfterLastMarker}' = delete content after last occurrence of marker; arguments: <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentBeforeLastMarker}' = keep content before last occurrence of marker; arguments: <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentAfterLastMarker}' = keep content after last occurrence of marker; arguments: <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeInsertContentBeforeLastMarker}' = insert content before last occurrence of marker; arguments: <content> <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeInsertContentAfterLastMarker}' = insert content after last occurrence of marker; arguments: <content> <marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenMarkers}' = delete content between markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentBetweenMarkers}' = keep content between markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentOutsideMarkers}' = keep content outside markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenLastMarkers}' = delete content between last occurrences of markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentBetweenLastMarkers}' = keep content between last occurrences of markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentOutsideLastMarkers}' = keep content outside last occurrences of markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenInnermostMarkers}' = delete content between innermost occurrences of markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentBetweenInnermostMarkers}' = keep content between innermost occurrences of markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentOutsideInnermostMarkers}' = keep content outside innermost occurrences of markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenOutermostMarkers}' = delete content between outermost occurrences of markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentBetweenOutermostMarkers}' = keep content between outermost occurrences of markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeKeepContentOutsideOutermostMarkers}' = keep content outside outermost occurrences of markers; arguments: <first_marker> <second_marker>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeSet}' = set; arguments: <replacement>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringEditTypeSetIfEquals}' = set if equals value; arguments: <value_to_match> <replacement>"
                        ;

                    public static readonly string ValueEditTypeText = $"{Constants.Commands.Arguments.Descriptions.EditType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.ValueEditTypeRewrite}' = rewrite"
                        + $"\n\t- '{Constants.Commands.Arguments.ValueEditTypeAdd}' = add; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ValueEditTypeSubtract}' = subtract; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ValueEditTypeMultiply}' = multiply; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ValueEditTypeDivide}' = divide; arguments: <value>"
                        + "\nSome edit types are not supported by some data types.";

                    public const string InsertionType = "<insertion_type>";

                    public static readonly string PositionInsertionTypeText = $"{Constants.Commands.Arguments.Descriptions.InsertionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.PositionInsertionTypePosition}' = position; arguments: <line_number>"
                        + $"\n\t- '{Constants.Commands.Arguments.PositionInsertionTypeEach}' = each; arguments: <each_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.PositionInsertionTypeLast}' = last"
                        ;

                    public const string JoinType = "<join_type>";

                    public static readonly string JoinTypeText = $"{Constants.Commands.Arguments.Descriptions.JoinType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.JoinTypeInner}' = inner - omit lines not matching second file"
                        + $"\n\t- '{Constants.Commands.Arguments.JoinTypeLeftOuter}' = left outer - include lines not matching second file"
                        ;

                    public const string TransformationType = "<transformation_type>";

                    public static readonly string LineTransformationTypeText = $"{Constants.Commands.Arguments.Descriptions.TransformationType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.LineTransformationTypeBreak}' = break a line into a line for each column"
                        + $"\n\t- '{Constants.Commands.Arguments.LineTransformationTypeUnite}' = unite a sequence of lines into a single line; arguments <sequence_size>"
                        ;

                    public static readonly string LineTransformationTypeUniteText = $"'{Constants.Commands.Arguments.LineTransformationTypeUnite}' will skip the last <input_file_size> % <sequence_size> lines.";

                    public static readonly string ColumnTransformationTypeText = $"{Constants.Commands.Arguments.Descriptions.TransformationType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.ColumnTransformationTypePack}' = pack column range; arguments: <start_column_number> <end_column_number> <packing_separator>"
                        + $"\n\t- '{Constants.Commands.Arguments.ColumnTransformationTypeUnpack}' = unpack column; arguments: <column_number> <packing_separator>"
                        ;

                    public const string ComparisonType = "<comparison_type>";

                    public static readonly string ComparisonTypeText = $"{Constants.Commands.Arguments.Descriptions.ComparisonType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeLessThan}' = less than; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeLessThanOrEqual}' = less than or equal; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeEqual}' = equal; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeGreaterThanOrEqual}' = greater than or equal; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeGreaterThan}' = greater than; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeNotEqual}' = not equal; arguments: <value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeBetween}' = between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeStrictlyBetween}' = strictly between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeNotBetween}' = not between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.ComparisonTypeNotStrictlyBetween}' = not strictly between; arguments: <start_value> <end_value>"
                        ;

                    public const string SelectionType = "<selection_type>";

                    public static readonly string PositionSelectionTypeText = $"{Constants.Commands.Arguments.Descriptions.SelectionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.PositionSelectionTypeBetween}' = between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.PositionSelectionTypeNotBetween}' = not between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.PositionSelectionTypeLast}' = last; arguments: <last_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.PositionSelectionTypeNotLast}' = not last; arguments: <last_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.PositionSelectionTypeEach}' = each; arguments: <each_count>"
                        + $"\n\t- '{Constants.Commands.Arguments.PositionSelectionTypeNotEach}' = not each; arguments: <each_count>"
                        ;

                    public static readonly string StringSelectionTypeText = $"{Constants.Commands.Arguments.Descriptions.SelectionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeHasLengthBetween}' = has length between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeHasLengthNotBetween}' = has length not between; arguments: <start_value> <end_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeIncludes }' = includes; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeNotIncludes}' = not includes; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeStartsWith}' = starts with; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeNotStartsWith}' = not starts with; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeEndsWith}' = ends with; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeNotEndsWith}' = not ends with; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeStartsAndEndsWith}' = starts and ends with; arguments: <starting_value> <ending_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeNotStartsAndEndsWith}' = not starts and ends with; arguments: <starting_value> <ending_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeEquals}' = equals; arguments: <string_value>"
                        + $"\n\t- '{Constants.Commands.Arguments.StringSelectionTypeNotEquals}' = not equals; arguments: <string_value>"
                        ;

                    public static readonly string RelativeLineSelectionTypeText = $"{Constants.Commands.Arguments.Descriptions.SelectionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.RelativeValueSelectionFirst}' = first occurrence"
                        + $"\n\t- '{Constants.Commands.Arguments.RelativeValueSelectionNotFirst}' = not first occurrence"
                        ;

                    public static readonly string RelativeValueSelectionTypeText = $"{Constants.Commands.Arguments.Descriptions.SelectionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.RelativeValueSelectionFirst}' = first occurrence"
                        + $"\n\t- '{Constants.Commands.Arguments.RelativeValueSelectionNotFirst}' = not first occurrence"
                        + $"\n\t- '{Constants.Commands.Arguments.RelativeValueSelectionLast}' = last occurrence"
                        + $"\n\t- '{Constants.Commands.Arguments.RelativeValueSelectionNotLast}' = not last occurrence"
                        ;

                    public const string LookupType = "<lookup_type>";

                    public static readonly string LookupTypeText = $"{Constants.Commands.Arguments.Descriptions.LookupType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.LookupTypeIncluded}' = included - include lines matching lookup file content"
                        + $"\n\t- '{Constants.Commands.Arguments.LookupTypeNotIncluded}' = not included - include lines not matching lookup file content"
                        ;

                    public static readonly string SeedValueText = $"{Constants.Commands.Arguments.Descriptions.SeedValue} is an integer value that can be used to initialize the RNG for repeatable results. To not use a seed, just set this parameter to any negative value.";

                    public const string DataDistributionType = "<data_distribution_type>";

                    public static readonly string DataDistributionTypeText = $"{Constants.Commands.Arguments.Descriptions.DataDistributionType} can take the values:"
                        + $"\n\t- '{Constants.Commands.Arguments.DataDistributionTypeUniform}' = uniform"
                        + $"\n\t- '{Constants.Commands.Arguments.DataDistributionTypeNormal}' = normal"
                        + $"\n\t- '{Constants.Commands.Arguments.DataDistributionTypeExponential}' = exponential; arguments: <distribution_mean> (double)"
                        + $"\n\t- '{Constants.Commands.Arguments.DataDistributionTypePoisson}' = Poisson; arguments: <distribution_mean> (ulong)"
                        ;
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
