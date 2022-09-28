////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors;
using LaurentiuCristofor.Proteus.DataProcessors.Dual;
using LaurentiuCristofor.Proteus.DataProcessors.Lookup;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileProcessors;

using LaurentiuCristofor.Cabeiro.Common;
using LaurentiuCristofor.Cabeiro.OnlineHelp;

using CabeiroConstants = LaurentiuCristofor.Cabeiro.Common.Constants;

namespace LaurentiuCristofor.Cabeiro
{
    /// <summary>
    /// Entry point class.
    /// </summary>
    public abstract class Cabeiro
    {
        /// <summary>
        /// Entry point method.
        /// </summary>
        /// <param name="arguments">Our command line arguments.</param>
        public static void Main(string[] arguments)
        {
            try
            {
                // Validate version of Proteus library.
                //
                ValidateProteusVersion();

                // Initialize the command registry that handles the online help.
                //
                CommandRegistry.Initialize();

                // Interpret arguments
                //
                ParseAndExecuteArguments(arguments);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"\nAn error has occurred during the execution of {CabeiroConstants.Program.Name}:\n{e.Message}");
                Console.Error.WriteLine($"\nFull exception information:\n\n{e}");
            }
        }

        /// <summary>
        /// Checks that Cabeiro is used with the correct Proteus version.
        /// </summary>
        private static void ValidateProteusVersion()
        {
            const int expectedProteusMajorVersion = 0;
            const int expectedProteusMinorVersion = 9;

            AssemblyName proteusInfo = ProteusInfo.GetAssemblyInfo();
            AssemblyName cabeiroInfo = CabeiroInfo.GetAssemblyInfo();

            if (proteusInfo.Version.Major < expectedProteusMajorVersion
                || (proteusInfo.Version.Major == expectedProteusMajorVersion && proteusInfo.Version.Minor < expectedProteusMinorVersion))
            {
                CommandRegistry.DisplayProgramVersion();
                throw new CabeiroException($"Cabeiro version {cabeiroInfo.Version.Major}.{cabeiroInfo.Version.Minor} expects Proteus version {expectedProteusMajorVersion}.{expectedProteusMinorVersion} but found version {proteusInfo.Version.Major}.{proteusInfo.Version.Minor} instead!");
            }
        }

        /// <summary>
        /// A simple command parser.
        /// </summary>
        /// <param name="arguments">The arguments to parse.</param>
        private static void ParseAndExecuteArguments(string[] arguments)
        {
            // Parse arguments and invoke specific commands or display help.
            //
            if (arguments.Length == 0)
            {
                CommandRegistry.DisplayProgramDescription();
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.Help))
            {
                if (arguments.Length > 1)
                {
                    if (arguments[1].Equals(CabeiroConstants.Commands.Arguments.All))
                    {
                        CommandRegistry.DisplayAllCommands();
                        return;
                    }
                    else if (arguments[1].Equals(CabeiroConstants.Commands.Arguments.Categories))
                    {
                        CommandRegistry.DisplayAllCommandCategories();
                        return;
                    }
                    else if (arguments[1].Equals(CabeiroConstants.Commands.Arguments.Category)
                        && arguments.Length > 2)
                    {
                        CommandRegistry.DisplayCategoryCommands(arguments[2]);
                        return;
                    }
                    else
                    {
                        CommandRegistry.DisplayCommandDescription(arguments[1]);
                        return;
                    }
                }

                // No other arguments will default to 'all' option.
                //
                CommandRegistry.DisplayAllCommands();
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.CountLines))
            {
                const int expectedArgumentNumber = 2;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, expectedArgumentNumber))
                {
                    string inputFilePath = arguments[1];

                    CountLines(inputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.AnalyzeLines))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 3;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int valuesLimit = ArgumentParser.GetPositiveInteger(arguments[2]);

                    AnalyzeLines(
                        inputFilePath,
                        valuesLimit);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.AnalyzeColumnValues))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 6;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    int valuesLimit = ArgumentParser.GetPositiveInteger(arguments[5]);

                    AnalyzeColumnValues(
                        inputFilePath,
                        columnNumber,
                        columnSeparator,
                        dataType,
                        valuesLimit);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.Invert))
            {
                const int minimumArgumentNumber = 2;
                const int maximumArgumentNumber = 3;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    ArgumentParser.ExtractLastArguments(0, 2, arguments, out _, out string outputFilePath);

                    InvertFile(
                        inputFilePath,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.Sort))
            {
                const int minimumArgumentNumber = 2;
                const int maximumArgumentNumber = 3;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    ArgumentParser.ExtractLastArguments(0, 2, arguments, out _, out string outputFilePath);

                    SortFile(
                        inputFilePath,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SortByColumnValue))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 8;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    ArgumentParser.ExtractLastArguments(0, 5, arguments, out _, out string outputFilePath);

                    SortFileByColumnValue(
                        inputFilePath,
                        columnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.OrderColumns))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                    string newFirstColumnsList = arguments[3];
                    ArgumentParser.ExtractLastArguments(0, 4, arguments, out _, out string outputFilePath);

                    OrderColumns(
                        inputFilePath,
                        columnSeparator,
                        newFirstColumnsList,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.EditLines))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 6;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    Tuple<StringEditType, int> operationInfo = ArgumentParser.ParseStringEditType(arguments[2]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 3, arguments, out string[] operationArguments, out string outputFilePath);

                    EditLines(
                        inputFilePath,
                        operationInfo.Item1, arguments[2],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.EditColumnStrings))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 8;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    Tuple<StringEditType, int> operationInfo = ArgumentParser.ParseStringEditType(arguments[4]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 5, arguments, out string[] operationArguments, out string outputFilePath);

                    EditColumnStrings(
                        inputFilePath,
                        columnNumber,
                        columnSeparator,
                        operationInfo.Item1, arguments[4],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.EditColumnValues))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 8;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    Tuple<ValueEditType, int> operationInfo = ArgumentParser.ParseValueEditType(arguments[5]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 6, arguments, out string[] operationArguments, out string outputFilePath);

                    EditColumnValues(
                        inputFilePath,
                        columnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        operationInfo.Item1, arguments[5],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.InsertLine))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 6;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    string lineValue = arguments[2];
                    Tuple<PositionInsertionType, int> operationInfo = ArgumentParser.ParsePositionInsertionType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out string[] operationArguments, out string outputFilePath);

                    InsertLine(
                        inputFilePath,
                        lineValue,
                        operationInfo.Item1, arguments[3],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.JoinLines)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.JoinLinesPostSorting))
            {
                const int minimumArgumentNumber = 8;
                const int maximumArgumentNumber = 10;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string firstFilePath = arguments[1];
                    int firstFileColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    string secondFilePath = arguments[5];
                    int secondFileColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[6]);
                    Tuple<JoinType, int> operationInfo = ArgumentParser.ParseJoinType(arguments[7]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 8, arguments, out string[] operationArguments, out string outputFilePath);

                    bool isSorted = ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.JoinLinesPostSorting);

                    JoinLines(
                        isSorted,
                        firstFilePath,
                        firstFileColumnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        secondFilePath,
                        secondFileColumnNumber,
                        operationInfo.Item1, arguments[6],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.ConcatenateLines))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string firstFilePath = arguments[1];
                    string secondFilePath = arguments[2];
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    ArgumentParser.ExtractLastArguments(0, 4, arguments, out _, out string outputFilePath);

                    ConcatenateLines(
                        firstFilePath,
                        secondFilePath,
                        columnSeparator,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.TransformColumns))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 8;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                    Tuple<ColumnTransformationType, int> operationInfo = ArgumentParser.ParseColumnTransformationType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out string[] operationArguments, out string outputFilePath);

                    TransformColumns(
                        inputFilePath,
                        columnSeparator,
                        operationInfo.Item1, arguments[3],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnValue))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 9;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    Tuple<ComparisonType, int> operationInfo = ArgumentParser.ParseComparisonType(arguments[5]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 6, arguments, out string[] operationArguments, out string outputFilePath);

                    SelectLinesByColumnValue(
                        inputFilePath,
                        columnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        operationInfo.Item1, arguments[5],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByNumber))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 6;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    Tuple<PositionSelectionType, int> operationInfo = ArgumentParser.ParsePositionSelectionType(arguments[2]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 3, arguments, out string[] operationArguments, out string outputFilePath);

                    SelectLinesByNumber(
                        inputFilePath,
                        operationInfo.Item1, arguments[2],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectColumnsByNumber))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 7;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                    Tuple<PositionSelectionType, int> operationInfo = ArgumentParser.ParsePositionSelectionType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out string[] operationArguments, out string outputFilePath);

                    SelectColumnsByNumber(
                        inputFilePath,
                        columnSeparator,
                        operationInfo.Item1, arguments[3],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByLineString))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 6;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    Tuple<StringSelectionType, int> operationInfo = ArgumentParser.ParseStringSelectionType(arguments[2]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 3, arguments, out string[] operationArguments, out string outputFilePath);

                    SelectLinesByLineString(
                        inputFilePath,
                        operationInfo.Item1, arguments[2],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnString))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 8;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    Tuple<StringSelectionType, int> operationInfo = ArgumentParser.ParseStringSelectionType(arguments[4]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 5, arguments, out string[] operationArguments, out string outputFilePath);

                    SelectLinesByColumnString(
                        inputFilePath,
                        columnNumber,
                        columnSeparator,
                        operationInfo.Item1, arguments[4],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnCount))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 7;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                    Tuple<ComparisonType, int> operationInfo = ArgumentParser.ParseComparisonType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out string[] operationArguments, out string outputFilePath);

                    SelectLinesByColumnCount(
                        inputFilePath,
                        columnSeparator,
                        operationInfo.Item1, arguments[3],
                        operationArguments,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesHandlingRepeatedLines)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingHandlingRepeatedLines))
            {
                const int minimumArgumentNumber = 2;
                const int maximumArgumentNumber = 3;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    Tuple<RepetitionHandlingType, int> operationInfo = ArgumentParser.ParseRepetitionHandlingType(arguments[2]);
                    ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out string outputFilePath);

                    bool isSorted = ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingHandlingRepeatedLines);

                    SelectLinesHandlingRepeatedLines(
                        isSorted,
                        inputFilePath,
                        operationInfo.Item1, arguments[2],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesHandlingRepeatedColumnValues)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingHandlingRepeatedColumnValues))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 6;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    Tuple<RepetitionHandlingType, int> operationInfo = ArgumentParser.ParseRepetitionHandlingType(arguments[5]);
                    ArgumentParser.ExtractLastArguments(0, 6, arguments, out _, out string outputFilePath);

                    bool isSorted = ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingHandlingRepeatedColumnValues);

                    SelectLinesHandlingRepeatedColumnValues(
                        isSorted,
                        inputFilePath,
                        columnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        operationInfo.Item1, arguments[4],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByLookupInFile)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByLookupInFile))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string dataFilePath = arguments[1];
                    string lookupFilePath = arguments[2];
                    Tuple<LookupType, int> operationInfo = ArgumentParser.ParseLookupType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out _, out string outputFilePath);

                    bool isSorted = ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByLookupInFile);

                    SelectLinesByLookupInFile(
                        isSorted,
                        dataFilePath,
                        lookupFilePath,
                        operationInfo.Item1, arguments[3],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnValueLookupInFile)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueLookupInFile))
            {
                const int minimumArgumentNumber = 8;
                const int maximumArgumentNumber = 9;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string dataFilePath = arguments[1];
                    int dataFileColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    string lookupFilePath = arguments[5];
                    int lookupFileColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[6]);
                    Tuple<LookupType, int> operationInfo = ArgumentParser.ParseLookupType(arguments[7]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 8, arguments, out _, out string outputFilePath);

                    bool isSorted = ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueLookupInFile);

                    SelectLinesByColumnValueLookupInFile(
                        isSorted,
                        dataFilePath,
                        dataFileColumnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        lookupFilePath,
                        lookupFileColumnNumber,
                        operationInfo.Item1, arguments[5],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SplitLineRanges))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 4;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    ulong rangeSize = ArgumentParser.GetUnsignedLongInteger(arguments[2]);
                    ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out string outputFilePath);

                    SplitLineRanges(
                        inputFilePath,
                        rangeSize,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SplitColumns))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 4;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                    ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out string outputFilePath);

                    SplitColumns(
                        inputFilePath,
                        columnSeparator,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SplitColumnValues))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 6;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    ArgumentParser.ExtractLastArguments(0, 5, arguments, out _, out string outputFilePath);

                    SplitColumnValues(
                        inputFilePath,
                        columnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SortBySecondColumnValue))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 10;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int secondColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType secondDataType = ArgumentParser.ParseDataType(arguments[4]);
                    int firstColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[5]);
                    DataType firstDataType = ArgumentParser.ParseDataType(arguments[6]);
                    ArgumentParser.ExtractLastArguments(0, 7, arguments, out _, out string outputFilePath);

                    SortFileBySecondColumnValue(
                        inputFilePath,
                        secondColumnNumber,
                        columnSeparator,
                        secondDataType, arguments[4],
                        firstColumnNumber,
                        firstDataType,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.MergeLines))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 4;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string firstFilePath = arguments[1];
                    string secondFilePath = arguments[2];
                    ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out string outputFilePath);

                    MergeLines(
                        firstFilePath,
                        secondFilePath,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.MergeLinesByColumnValue))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 8;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string firstFilePath = arguments[1];
                    int firstFileColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    string secondFilePath = arguments[5];
                    int secondFileColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[6]);
                    ArgumentParser.ExtractLastArguments(0, 7, arguments, out _, out string outputFilePath);

                    MergeLinesByColumnValue(
                        firstFilePath,
                        firstFileColumnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        secondFilePath,
                        secondFileColumnNumber,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.FindStateTransitions))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 10;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string inputFilePath = arguments[1];
                    int secondColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType secondDataType = ArgumentParser.ParseDataType(arguments[4]);
                    int firstColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[5]);
                    DataType firstDataType = ArgumentParser.ParseDataType(arguments[6]);
                    ArgumentParser.ExtractLastArguments(0, 7, arguments, out _, out string outputFilePath);

                    FindStateTransitions(
                        inputFilePath,
                        secondColumnNumber,
                        columnSeparator,
                        secondDataType, arguments[4],
                        firstColumnNumber,
                        firstDataType,
                        outputFilePath);
                    return;
                }
            }

            // If we reached this point, the user command did not match any existing command.
            // Display the program description as a reminder.
            //
            CommandRegistry.DisplayProgramDescription();
        }

        private static void CountLines(string inputFilePath)
        {
            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SinkProcessor, Unused>(
                    inputFilePath,
                    extractionParameters: null,
                    processingParameters: null);

            fileProcessor.ProcessFile();
        }

        private static void AnalyzeLines(string inputFilePath, int valuesLimit)
        {
            AnalyzeParameters processingParameters = new AnalyzeParameters(DataType.String, valuesLimit);

            var fileProcessor
                = new FileProcessor<LineAsParsedLineExtractor, Unused, ParsedLine, AnalyzeProcessor, AnalyzeParameters>(
                    inputFilePath,
                    extractionParameters: null,
                    processingParameters: processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void AnalyzeColumnValues(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType,
            int valuesLimit)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            AnalyzeParameters processingParameters = new AnalyzeParameters(dataType, valuesLimit);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, AnalyzeProcessor, AnalyzeParameters>(
                    inputFilePath,
                    extractionParameters: extractionParameters,
                    processingParameters: processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void InvertFile(
            string inputFilePath,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.Invert}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, FileInvertProcessor, BaseOutputParameters>(
                    inputFilePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SortFile(
            string inputFilePath,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.Sort}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SortProcessor, BaseOutputParameters>(
                    inputFilePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SortFileByColumnValue(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.Sort}.{columnNumber}.{dataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SortByColumnValueProcessor, BaseOutputParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void OrderColumns(
            string inputFilePath,
            string columnSeparator,
            string newFirstColumnsList,
            string outputFilePath)
        {
            // We need a successful column extraction to extract all columns,
            // so we'll just ask to extract the first column value, which must always exist if there is any data in the line.
            //
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber: 1,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.OrderColumns}.{newFirstColumnsList}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputStringParameters processingParameters = new OutputStringParameters(
                outputFilePath,
                newFirstColumnsList);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, OrderColumnsProcessor, OutputStringParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void EditLines(
            string inputFilePath,
            StringEditType editType, string editTypeString,
            string[] editArguments,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.EditLines}.{editTypeString.ToLower()}";
            var filePathBuilder = new EditOperationFilePathBuilder(editType, inputFilePath, outputFileExtension, editArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<StringEditType> processingParameters = new OutputOperationParameters<StringEditType>(
                outputFilePath,
                editType,
                editArguments);

            var fileProcessor
                = new FileProcessor<LineAsParsedLineExtractor, Unused, ParsedLine, EditStringProcessor, OutputOperationParameters<StringEditType>>(
                    inputFilePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void EditColumnStrings(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            StringEditType editType, string editTypeString,
            string[] editArguments,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.EditColumnStrings}.{columnNumber}.{editTypeString.ToLower()}";
            var filePathBuilder = new EditOperationFilePathBuilder(editType, inputFilePath, outputFileExtension, editArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<StringEditType> processingParameters = new OutputOperationParameters<StringEditType>(
                outputFilePath,
                editType,
                editArguments);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, EditStringProcessor, OutputOperationParameters<StringEditType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void EditColumnValues(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            ValueEditType editType, string editTypeString,
            string[] editArguments,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.EditColumnValues}.{columnNumber}.{dataTypeString.ToLower()}.{editTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, editArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            // Construct IDataHolder argument, if one is provided.
            //
            IDataHolder argument = null;
            if (editArguments.Length > 0)
            {
                argument = DataHolderOperations.BuildDataHolder(dataType, editArguments[0]);
            }

            OutputValueEditParameters processingParameters = new OutputValueEditParameters(
                outputFilePath,
                editType,
                argument);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, EditColumnValueProcessor, OutputValueEditParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void InsertLine(
            string inputFilePath,
            string lineValue,
            PositionInsertionType insertionType, string insertionTypeString,
            string[] insertionArguments,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.InsertLine}.{insertionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, insertionArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            // We pass the line value as the first operation argument and the first argument becomes the second.
            //
            OutputOperationParameters<PositionInsertionType> processingParameters = new OutputOperationParameters<PositionInsertionType>(
                outputFilePath,
                insertionType,
                new string[] { lineValue, insertionArguments[0] });

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, InsertLineProcessor, OutputOperationParameters<PositionInsertionType>>(
                    inputFilePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void JoinLines(
            bool isSorted,
            string firstFilePath,
            int firstFileColumnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            string secondFilePath,
            int secondFileColumnNumber,
            JoinType joinType, string joinTypeString,
            string[] joinArguments,
            string outputFilePath)
        {
            ColumnExtractionParameters firstExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                firstFileColumnNumber,
                dataType);

            ColumnExtractionParameters secondExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                secondFileColumnNumber,
                dataType);

            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.JoinLinesPostSorting}.{firstFileColumnNumber}.{dataTypeString.ToLower()}.{joinTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.JoinLines}.{firstFileColumnNumber}.{dataTypeString.ToLower()}.{joinTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, joinArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<JoinType> processingParameters = new OutputOperationParameters<JoinType>(
                outputFilePath,
                joinType,
                joinArguments);

            if (isSorted)
            {
                var dualFileProcessor
                    = new DualFileProcessor<
                        ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                        JoinPostSortingProcessor, OutputOperationParameters<JoinType>>(
                        firstFilePath,
                        firstExtractionParameters,
                        secondFilePath,
                        secondExtractionParameters,
                        processingParameters);

                dualFileProcessor.ProcessFiles();
            }
            else
            {
                var lookupFileProcessor
                    = new LookupFileProcessor<
                        ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                        ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                        JoinBuilder, Dictionary<IDataHolder, List<string>>,
                        JoinProcessor, OutputOperationParameters<JoinType>>(
                        firstFilePath,
                        firstExtractionParameters,
                        secondFilePath,
                        secondExtractionParameters,
                        processingParameters);

                lookupFileProcessor.ProcessFiles();
            }
        }

        private static void ConcatenateLines(
            string firstFilePath,
            string secondFilePath,
            string columnSeparator,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.ConcatenateLines}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputStringParameters processingParameters = new OutputStringParameters(
                outputFilePath,
                columnSeparator);

            var dualFileProcessor
                = new DualFileProcessor<
                    LineExtractor, Unused, string,
                    ConcatenateProcessor, OutputStringParameters>(
                    firstFilePath,
                    firstExtractionParameters: null,
                    secondFilePath,
                    secondExtractionParameters: null,
                    processingParameters);

            dualFileProcessor.ProcessFiles();
        }

        private static void TransformColumns(
            string inputFilePath,
            string columnSeparator,
            ColumnTransformationType transformationType, string transformationTypeString,
            string[] transformationArguments,
            string outputFilePath)
        {
            // We need a successful column extraction to extract all columns,
            // so we'll just ask to extract the first column value, which must always exist if there is any data in the line.
            //
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                1,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.TransformColumns}.{transformationTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, transformationArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<ColumnTransformationType> processingParameters = new OutputOperationParameters<ColumnTransformationType>(
                outputFilePath,
                transformationType,
                transformationArguments);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, TransformColumnsProcessor, OutputOperationParameters<ColumnTransformationType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByColumnValue(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            ComparisonType comparisonType, string comparisonTypeString,
            string[] comparisonArguments,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}.{comparisonTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, comparisonArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<ComparisonType> processingParameters = new OutputOperationParameters<ComparisonType>(
                outputFilePath,
                comparisonType,
                comparisonArguments);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectLineByColumnValueProcessor, OutputOperationParameters<ComparisonType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByNumber(
            string inputFilePath,
            PositionSelectionType selectionType, string selectionTypeString,
            string[] selectionArguments,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, selectionArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<PositionSelectionType> processingParameters = new OutputOperationParameters<PositionSelectionType>(
                outputFilePath,
                selectionType,
                selectionArguments);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SelectLineByNumberProcessor, OutputOperationParameters<PositionSelectionType>>(
                    inputFilePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectColumnsByNumber(
            string inputFilePath,
            string columnSeparator,
            PositionSelectionType selectionType, string selectionTypeString,
            string[] selectionArguments,
            string outputFilePath)
        {
            // We need a successful column extraction to extract all columns,
            // so we'll just ask to extract the first column value, which must always exist if there is any data in the line.
            //
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber: 1,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectColumnsByNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, selectionArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<PositionSelectionType> processingParameters = new OutputOperationParameters<PositionSelectionType>(
                outputFilePath,
                selectionType,
                selectionArguments);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectColumnByNumberProcessor, OutputOperationParameters<PositionSelectionType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByLineString(
            string inputFilePath,
            StringSelectionType selectionType, string selectionTypeString,
            string[] selectionArguments,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByLineString}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, selectionArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<StringSelectionType> processingParameters = new OutputOperationParameters<StringSelectionType>(
                outputFilePath,
                selectionType,
                selectionArguments);

            var fileProcessor
                = new FileProcessor<LineAsParsedLineExtractor, Unused, ParsedLine, SelectLineByStringProcessor, OutputOperationParameters<StringSelectionType>>(
                    inputFilePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByColumnString(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            StringSelectionType selectionType, string selectionTypeString,
            string[] selectionArguments,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnString}.{columnNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, selectionArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<StringSelectionType> processingParameters = new OutputOperationParameters<StringSelectionType>(
                outputFilePath,
                selectionType,
                selectionArguments);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectLineByStringProcessor, OutputOperationParameters<StringSelectionType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByColumnCount(
            string inputFilePath,
            string columnSeparator,
            ComparisonType comparisonType, string comparisonTypeString,
            string[] comparisonArguments,
            string outputFilePath)
        {
            // We need a successful column extraction to extract all columns,
            // so we'll just ask to extract the first column value, which must always exist if there is any data in the line.
            //
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber: 1,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnCount}.{comparisonTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, comparisonArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<ComparisonType> processingParameters = new OutputOperationParameters<ComparisonType>(
                outputFilePath,
                comparisonType,
                comparisonArguments);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectLineByColumnCountProcessor, OutputOperationParameters<ComparisonType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesHandlingRepeatedLines(
            bool isSorted,
            string inputFilePath,
            RepetitionHandlingType handlingType, string handlingTypeString,
            string outputFilePath)
        {
            string outputFileExtension
                = isSorted 
                ? $".{CabeiroConstants.Commands.SelectLinesPostSortingHandlingRepeatedLines}.{handlingTypeString}"
                : $".{CabeiroConstants.Commands.SelectLinesHandlingRepeatedLines}.{handlingTypeString}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<RepetitionHandlingType> processingParameters = new OutputOperationParameters<RepetitionHandlingType>(
                outputFilePath,
                handlingType);

            if (isSorted)
            {
                var fileProcessor
                    = new FileProcessor<LineAsParsedLineExtractor, Unused, ParsedLine, SelectLinePostSortingHandlingRepeteadValuesProcessor, OutputOperationParameters<RepetitionHandlingType>>(
                        inputFilePath,
                        extractionParameters: null,
                        processingParameters);

                fileProcessor.ProcessFile();
            }
            else
            {
                var fileProcessor
                    = new FileProcessor<LineAsParsedLineExtractor, Unused, ParsedLine, SelectLineHandlingRepeteadValuesProcessor, OutputOperationParameters<RepetitionHandlingType>>(
                        inputFilePath,
                        extractionParameters: null,
                        processingParameters);

                fileProcessor.ProcessFile();
            }
        }

        private static void SelectLinesHandlingRepeatedColumnValues(
            bool isSorted,
            string inputFilePath, 
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            RepetitionHandlingType handlingType, string handlingTypeString,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.SelectLinesPostSortingHandlingRepeatedColumnValues}.{columnNumber}.{dataTypeString.ToLower()}.{handlingTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.SelectLinesHandlingRepeatedColumnValues}.{columnNumber}.{dataTypeString.ToLower()}.{handlingTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<RepetitionHandlingType> processingParameters = new OutputOperationParameters<RepetitionHandlingType>(
                outputFilePath,
                handlingType);

            if (isSorted)
            {
                var fileProcessor
                    = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectLinePostSortingHandlingRepeteadValuesProcessor, OutputOperationParameters<RepetitionHandlingType>>(
                        inputFilePath,
                        extractionParameters,
                        processingParameters);

                fileProcessor.ProcessFile();
            }
            else
            {
                var fileProcessor
                    = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectLineHandlingRepeteadValuesProcessor, OutputOperationParameters<RepetitionHandlingType>>(
                        inputFilePath,
                        extractionParameters,
                        processingParameters);

                fileProcessor.ProcessFile();
            }
        }

        private static void SelectLinesByLookupInFile(
            bool isSorted,
            string dataFilePath,
            string lookupFilePath,
            LookupType lookupType, string lookupTypeString,
            string outputFilePath)
        {
            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.SelectLinesPostSortingByLookupInFile}.{lookupTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.SelectLinesByLookupInFile}.{lookupTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(dataFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<LookupType> processingParameters = new OutputOperationParameters<LookupType>(
                outputFilePath,
                lookupType);

            if (isSorted)
            {
                var dualFileProcessor
                    = new DualFileProcessor<
                        LineAsParsedLineExtractor, Unused, ParsedLine,
                        LookupPostSortingProcessor, OutputOperationParameters<LookupType>>(
                        dataFilePath,
                        firstExtractionParameters: null,
                        lookupFilePath,
                        secondExtractionParameters: null,
                        processingParameters);

                dualFileProcessor.ProcessFiles();
            }
            else
            {
                var lookupFileProcessor
                    = new LookupFileProcessor<
                        LineAsParsedLineExtractor, Unused, ParsedLine,
                        LineAsParsedLineExtractor, Unused, ParsedLine,
                        LookupBuilder, HashSet<IDataHolder>,
                        LookupProcessor, OutputOperationParameters<LookupType>>(
                        dataFilePath,
                        dataFileExtractionParameters: null,
                        lookupFilePath,
                        lookupFileExtractionParameters: null,
                        processingParameters);

                lookupFileProcessor.ProcessFiles();
            }
        }

        private static void SelectLinesByColumnValueLookupInFile(
            bool isSorted,
            string dataFilePath,
            int dataFileColumnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            string lookupFilePath,
            int lookupFileColumnNumber,
            LookupType lookupType, string lookupTypeString,
            string outputFilePath)
        {
            ColumnExtractionParameters dataFileExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                dataFileColumnNumber,
                dataType);

            ColumnExtractionParameters lookupFileExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                lookupFileColumnNumber,
                dataType);

            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueLookupInFile}.{dataFileColumnNumber}.{dataTypeString.ToLower()}.{lookupTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.SelectLinesByColumnValueLookupInFile}.{dataFileColumnNumber}.{dataTypeString.ToLower()}.{lookupTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(dataFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<LookupType> processingParameters = new OutputOperationParameters<LookupType>(
                outputFilePath,
                lookupType);

            if (isSorted)
            {
                var dualFileProcessor
                    = new DualFileProcessor<
                        ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                        LookupPostSortingProcessor, OutputOperationParameters<LookupType>>(
                        dataFilePath,
                        dataFileExtractionParameters,
                        lookupFilePath,
                        lookupFileExtractionParameters,
                        processingParameters);

                dualFileProcessor.ProcessFiles();
            }
            else
            {
                var lookupFileProcessor
                    = new LookupFileProcessor<
                        ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                        ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                        LookupBuilder, HashSet<IDataHolder>,
                        LookupProcessor, OutputOperationParameters<LookupType>>(
                        dataFilePath,
                        dataFileExtractionParameters,
                        lookupFilePath,
                        lookupFileExtractionParameters,
                        processingParameters);

                lookupFileProcessor.ProcessFiles();
            }
        }

        private static void SplitLineRanges(
            string inputFilePath,
            ulong rangeSize,
            string outputFilePath)
        {
            // The file name will be bundled by the processor with the start line number of each line range, so we exclude the text extension from it;
            // it will get appended by the processor internally.
            //
            string outputFileExtension = $".{CabeiroConstants.Commands.SplitLineRanges}.{rangeSize}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            OutputStringAndULongParameters processingParameters = new OutputStringAndULongParameters(
                outputFilePath,
                CabeiroConstants.Files.Extensions.Txt,
                rangeSize);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SplitLineRangesProcessor, OutputStringAndULongParameters>(
                    inputFilePath,
                    null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SplitColumns(
            string inputFilePath,
            string columnSeparator,
            string outputFilePath)
        {
            // We need a successful column extraction to extract all columns,
            // so we'll just ask to extract the first column value, which must always exist if there is any data in the line.
            //
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber: 1,
                DataType.String);

            // The file name will be bundled by the processor with the number of each column, so we exclude the text extension from it;
            // it will get appended by the processor internally.
            //
            string outputFileExtension = $".{CabeiroConstants.Commands.SplitColumns}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            OutputStringParameters processingParameters = new OutputStringParameters(
                outputFilePath,
                CabeiroConstants.Files.Extensions.Txt);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SplitColumnsProcessor, OutputStringParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SplitColumnValues(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            string outputFilePath)
        {
            // We need a successful column extraction to extract all columns,
            // so we'll just ask to extract the first column value, which must always exist if there is any data in the line.
            //
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            // The file name will be bundled by the processor with the number of each column, so we exclude the text extension from it;
            // it will get appended by the processor internally.
            //
            string outputFileExtension = $".{CabeiroConstants.Commands.SplitColumnValues}.{columnNumber}.{dataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            OutputStringParameters processingParameters = new OutputStringParameters(
                outputFilePath,
                CabeiroConstants.Files.Extensions.Txt);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SplitColumnValuesProcessor, OutputStringParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SortFileBySecondColumnValue(
            string inputFilePath,
            int secondColumnNumber,
            string columnSeparator,
            DataType secondDataType, string secondDataTypeString,
            int firstColumnNumber,
            DataType firstDataType,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                firstColumnNumber,
                firstDataType,
                secondColumnNumber,
                secondDataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SortBySecondColumnValue}.{secondColumnNumber}.{secondDataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SortBySecondColumnValueProcessor, BaseOutputParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void MergeLines(
            string firstFilePath,
            string secondFilePath,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.MergeLines}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var dualFileProcessor
                = new DualFileProcessor<
                    LineAsParsedLineExtractor, Unused, ParsedLine,
                    MergeProcessor, BaseOutputParameters>(
                    firstFilePath,
                    firstExtractionParameters: null,
                    secondFilePath,
                    secondExtractionParameters: null,
                    processingParameters);

            dualFileProcessor.ProcessFiles();
        }

        private static void MergeLinesByColumnValue(
            string firstFilePath,
            int firstFileColumnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            string secondFilePath,
            int secondFileColumnNumber,
            string outputFilePath)
        {
            ColumnExtractionParameters firstExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                firstFileColumnNumber,
                dataType);

            ColumnExtractionParameters secondExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                secondFileColumnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.MergeLinesByColumnValue}.{firstFileColumnNumber}.{dataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var dualFileProcessor
                = new DualFileProcessor<
                    ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                    MergeProcessor, BaseOutputParameters>(
                    firstFilePath,
                    firstExtractionParameters,
                    secondFilePath,
                    secondExtractionParameters,
                    processingParameters);

            dualFileProcessor.ProcessFiles();
        }

        private static void FindStateTransitions(
            string inputFilePath,
            int secondColumnNumber,
            string columnSeparator,
            DataType secondDataType, string secondDataTypeString,
            int firstColumnNumber,
            DataType firstDataType,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                firstColumnNumber,
                firstDataType,
                secondColumnNumber,
                secondDataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.FindStateTransitions}.{secondColumnNumber}.{secondDataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, operationArguments: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, FindStateTransitionsProcessor, BaseOutputParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }
    }
}
