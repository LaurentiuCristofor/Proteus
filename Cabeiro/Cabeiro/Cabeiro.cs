﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors;
using LaurentiuCristofor.Proteus.DataProcessors.Dual;
using LaurentiuCristofor.Proteus.DataProcessors.Lookup;
using LaurentiuCristofor.Proteus.FileProcessors;

using LaurentiuCristofor.Cabeiro.Common;
using LaurentiuCristofor.Cabeiro.OnlineHelp;

using CabeiroConstants = LaurentiuCristofor.Cabeiro.Common.Constants;

namespace LaurentiuCristofor.Cabeiro
{
    /// <summary>
    /// Entry point class.
    /// </summary>
    public class Cabeiro
    {
        /// <summary>
        /// Entry point method.
        /// </summary>
        /// <param name="arguments">Our command line arguments.</param>
        static void Main(string[] arguments)
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
                Console.Error.WriteLine($"\nFull exception information:\n\n{e.ToString()}");
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
                throw new CabeiroException($"Cabeiro version {cabeiroInfo.Version.Major}.{cabeiroInfo.Version.Minor} expects Proteus version {expectedProteusMajorVersion}.{expectedProteusMinorVersion} but found version {proteusInfo.Version.Major}.{proteusInfo.Version.Minor}!");
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
                    CountLines(arguments[1]);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.AnalyzeLines))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 3;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    int valuesLimit = ArgumentParser.GetPositiveInteger(arguments[2], acceptZero: true);

                    AnalyzeLines(
                        arguments[1],
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
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    int valuesLimit = ArgumentParser.GetPositiveInteger(arguments[5], acceptZero: true);

                    AnalyzeColumnValues(
                        arguments[1],
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
                    ArgumentParser.ExtractLastArguments(0, 2, arguments, out _, out _, out string outputFilePath);

                    InvertFile(
                        arguments[1],
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
                    ArgumentParser.ExtractLastArguments(0, 2, arguments, out _, out _, out string outputFilePath);

                    SortFile(
                        arguments[1],
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
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    ArgumentParser.ExtractLastArguments(0, 5, arguments, out _, out _, out string outputFilePath);

                    SortFileByColumnValue(
                        arguments[1],
                        columnNumber,
                        columnSeparator,
                        dataType, arguments[4],
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
                    Tuple<StringEditType, int> operationInfo = ArgumentParser.ParseStringEditType(arguments[2]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 3, arguments, out string firstArgument, out string secondArgument, out string outputFilePath);

                    EditLines(
                        arguments[1],
                        operationInfo.Item1, arguments[2],
                        firstArgument,
                        secondArgument,
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
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    Tuple<StringEditType, int> operationInfo = ArgumentParser.ParseStringEditType(arguments[4]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 5, arguments, out string firstArgument, out string secondArgument, out string outputFilePath);

                    EditColumnStrings(
                        arguments[1],
                        columnNumber,
                        columnSeparator,
                        operationInfo.Item1, arguments[4],
                        firstArgument,
                        secondArgument,
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
                    string lineValue = arguments[2];
                    Tuple<PositionInsertionType, int> operationInfo = ArgumentParser.ParsePositionInsertionType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out string firstArgument, out _, out string outputFilePath);

                    InsertLine(
                        arguments[1],
                        lineValue,
                        operationInfo.Item1, arguments[3],
                        firstArgument,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.JoinLines))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 9;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string firstFilePath = arguments[1];
                    int firstFileColumnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = arguments[3];
                    string secondFilePath = arguments[4];
                    int secondFileColumnNumber = ArgumentParser.GetPositiveInteger(arguments[5]);
                    Tuple<JoinType, int> operationInfo = ArgumentParser.ParseJoinType(arguments[6]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 7, arguments, out string firstArgument, out _, out string outputFilePath);

                    JoinLines(
                        firstFilePath,
                        firstFileColumnNumber,
                        columnSeparator,
                        secondFilePath,
                        secondFileColumnNumber,
                        operationInfo.Item1, arguments[6],
                        firstArgument,
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
                    string columnSeparator = arguments[3];
                    ArgumentParser.ExtractLastArguments(0, 4, arguments, out _, out _, out string outputFilePath);

                    ConcatenateLines(
                        firstFilePath,
                        secondFilePath,
                        columnSeparator,
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
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    Tuple<ComparisonType, int> operationInfo = ArgumentParser.ParseComparisonType(arguments[5]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 6, arguments, out string firstArgument, out string secondArgument, out string outputFilePath);

                    SelectLinesByColumnValue(
                        arguments[1],
                        columnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        operationInfo.Item1, arguments[5],
                        firstArgument,
                        secondArgument,
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
                    Tuple<PositionSelectionType, int> operationInfo = ArgumentParser.ParsePositionSelectionType(arguments[2]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 3, arguments, out string firstArgument, out string secondArgument, out string outputFilePath);

                    SelectLinesByNumber(
                        arguments[1],
                        operationInfo.Item1, arguments[2],
                        firstArgument,
                        secondArgument,
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
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                    Tuple<PositionSelectionType, int> operationInfo = ArgumentParser.ParsePositionSelectionType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out string firstArgument, out string secondArgument, out string outputFilePath);

                    SelectColumnsByNumber(
                        arguments[1],
                        columnSeparator,
                        operationInfo.Item1, arguments[3],
                        firstArgument,
                        secondArgument,
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
                    Tuple<StringSelectionType, int> operationInfo = ArgumentParser.ParseStringSelectionType(arguments[2]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 3, arguments, out string firstArgument, out string secondArgument, out string outputFilePath);

                    SelectLinesByLineString(
                        arguments[1],
                        operationInfo.Item1, arguments[2],
                        firstArgument,
                        secondArgument,
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
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    Tuple<StringSelectionType, int> operationInfo = ArgumentParser.ParseStringSelectionType(arguments[4]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 5, arguments, out string firstArgument, out string secondArgument, out string outputFilePath);

                    SelectLinesByColumnString(
                        arguments[1],
                        columnNumber,
                        columnSeparator,
                        operationInfo.Item1, arguments[4],
                        firstArgument,
                        secondArgument,
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
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                    Tuple<ComparisonType, int> operationInfo = ArgumentParser.ParseComparisonType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out string firstArgument, out string secondArgument, out string outputFilePath);

                    SelectLinesByColumnCount(
                        arguments[1],
                        columnSeparator,
                        operationInfo.Item1, arguments[3],
                        firstArgument,
                        secondArgument,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesHandlingRepeatedLines))
            {
                const int minimumArgumentNumber = 2;
                const int maximumArgumentNumber = 3;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    Tuple<RepetitionHandlingType, int> operationInfo = ArgumentParser.ParseRepetitionHandlingType(arguments[2]);
                    ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out _, out string outputFilePath);

                    SelectLinesHandlingRepeatedLines(
                        arguments[1],
                        operationInfo.Item1, arguments[2],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesHandlingRepeatedColumnStrings))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    Tuple<RepetitionHandlingType, int> operationInfo = ArgumentParser.ParseRepetitionHandlingType(arguments[4]);
                    ArgumentParser.ExtractLastArguments(0, 5, arguments, out _, out _, out string outputFilePath);

                    SelectLinesHandlingRepeatedColumnStrings(
                        arguments[1],
                        columnNumber,
                        columnSeparator,
                        operationInfo.Item1, arguments[4],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByLookupInFile))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string dataFilePath = arguments[1];
                    string lookupFilePath = arguments[2];
                    Tuple<LookupType, int> operationInfo = ArgumentParser.ParseLookupType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out _, out _, out string outputFilePath);

                    SelectLinesByLookupInFile(
                        dataFilePath,
                        lookupFilePath,
                        operationInfo.Item1, arguments[3],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnStringLookupInFile))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 7;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string dataFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = arguments[3];
                    string lookupFilePath = arguments[4];
                    Tuple<LookupType, int> operationInfo = ArgumentParser.ParseLookupType(arguments[5]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 6, arguments, out _, out _, out string outputFilePath);

                    SelectLinesByColumnStringLookupInFile(
                        dataFilePath,
                        columnNumber,
                        columnSeparator,
                        lookupFilePath,
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
                    ulong rangeSize = ArgumentParser.GetUnsignedLongInteger(arguments[2]);
                    ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out _, out string outputFilePath);

                    SplitLineRanges(
                        arguments[1],
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
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                    ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out _, out string outputFilePath);

                    SplitColumns(
                        arguments[1],
                        columnSeparator,
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SplitColumnStrings))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    ArgumentParser.ExtractLastArguments(0, 4, arguments, out _, out _, out string outputFilePath);

                    SplitColumnStrings(
                        arguments[1],
                        columnNumber,
                        columnSeparator,
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
                    int secondColumnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType secondDataType = ArgumentParser.ParseDataType(arguments[4]);
                    int firstColumnNumber = ArgumentParser.GetPositiveInteger(arguments[5]);
                    DataType firstDataType = ArgumentParser.ParseDataType(arguments[6]);
                    ArgumentParser.ExtractLastArguments(0, 7, arguments, out _, out _, out string outputFilePath);

                    SortFileBySecondColumnValue(
                        arguments[1],
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
                    ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out _, out string outputFilePath);

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
                    int firstFileColumnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = arguments[3];
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    string secondFilePath = arguments[5];
                    int secondFileColumnNumber = ArgumentParser.GetPositiveInteger(arguments[6]);
                    ArgumentParser.ExtractLastArguments(0, 7, arguments, out _, out _, out string outputFilePath);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByLookupInFile))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string dataFilePath = arguments[1];
                    string lookupFilePath = arguments[2];
                    Tuple<LookupType, int> operationInfo = ArgumentParser.ParseLookupType(arguments[3]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out _, out _, out string outputFilePath);

                    SelectLinesPostSortingByLookupInFile(
                        dataFilePath,
                        lookupFilePath,
                        operationInfo.Item1, arguments[3],
                        outputFilePath);
                    return;
                }
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueLookupInFile))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 8;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    string dataFilePath = arguments[1];
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = arguments[3];
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    string lookupFilePath = arguments[5];
                    Tuple<LookupType, int> operationInfo = ArgumentParser.ParseLookupType(arguments[6]);
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 7, arguments, out _, out _, out string outputFilePath);

                    SelectLinesPostSortingByColumnValueLookupInFile(
                        dataFilePath,
                        columnNumber,
                        columnSeparator,
                        dataType, arguments[4],
                        lookupFilePath,
                        operationInfo.Item1, arguments[6],
                        outputFilePath);
                    return;
                }
            }

            // If we reached this point, the user command did not match any existing command.
            // Display the program description as a reminder.
            //
            CommandRegistry.DisplayProgramDescription();
        }

        private static void CountLines(string filePath)
        {
            var fileProcessor
                = new FileProcessor<LineExtractor, UnusedType, string, SinkProcessor, UnusedType>(
                    filePath,
                    extractionParameters: null,
                    processingParameters: null);

            fileProcessor.ProcessFile();
        }

        private static void AnalyzeLines(string filePath, int valuesLimit)
        {
            AnalyzeParameters processingParameters = new AnalyzeParameters(valuesLimit);

            var fileProcessor
                = new FileProcessor<LineAsParsedLineExtractor, UnusedType, ParsedLine, AnalyzeProcessor, AnalyzeParameters>(
                    filePath,
                    extractionParameters: null,
                    processingParameters: processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void AnalyzeColumnValues(
            string filePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType,
            int valuesLimit)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            AnalyzeParameters processingParameters = new AnalyzeParameters(valuesLimit);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, AnalyzeProcessor, AnalyzeParameters>(
                    filePath,
                    extractionParameters: extractionParameters,
                    processingParameters: processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void InvertFile(
            string filePath,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.Invert}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<LineExtractor, UnusedType, string, FileInvertProcessor, BaseOutputParameters>(
                    filePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SortFile(
            string filePath,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.Sort}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<LineExtractor, UnusedType, string, SortProcessor, BaseOutputParameters>(
                    filePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SortFileByColumnValue(
            string filePath,
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
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SortByColumnValueProcessor, BaseOutputParameters>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void EditLines(
            string filePath,
            StringEditType editType, string editTypeString,
            string firstArgument,
            string secondArgument,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.EditLines}.{editTypeString.ToLower()}";
            var filePathBuilder = new EditOperationFilePathBuilder(editType, filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<StringEditType> processingParameters = new OperationTypeOutputParameters<StringEditType>(
                outputFilePath,
                editType,
                firstArgument,
                secondArgument);

            var fileProcessor
                = new FileProcessor<LineAsParsedLineExtractor, UnusedType, ParsedLine, EditStringProcessor, OperationTypeOutputParameters<StringEditType>>(
                    filePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void EditColumnStrings(
            string filePath,
            int columnNumber,
            string columnSeparator,
            StringEditType editType, string editTypeString,
            string firstArgument,
            string secondArgument,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.EditColumnStrings}.{columnNumber}.{editTypeString.ToLower()}";
            var filePathBuilder = new EditOperationFilePathBuilder(editType, filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<StringEditType> processingParameters = new OperationTypeOutputParameters<StringEditType>(
                outputFilePath,
                editType,
                firstArgument,
                secondArgument);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, EditStringProcessor, OperationTypeOutputParameters<StringEditType>>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void InsertLine(
            string filePath,
            string lineValue,
            PositionInsertionType insertionType, string insertionTypeString,
            string firstArgument,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.InsertLine}.{insertionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            // We pass the line value as the first operation argument and the first argument becomes the second.
            //
            OperationTypeOutputParameters<PositionInsertionType> processingParameters = new OperationTypeOutputParameters<PositionInsertionType>(
                outputFilePath,
                insertionType,
                firstArgument: lineValue,
                secondArgument: firstArgument);

            var fileProcessor
                = new FileProcessor<LineExtractor, UnusedType, string, InsertLineProcessor, OperationTypeOutputParameters<PositionInsertionType>>(
                    filePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void JoinLines(
            string firstFilePath,
            int firstFileColumnNumber,
            string columnSeparator,
            string secondFilePath,
            int secondFileColumnNumber,
            JoinType joinType, string joinTypeString,
            string firstArgument,
            string outputFilePath)
        {
            ColumnExtractionParameters firstExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                firstFileColumnNumber,
                DataType.String);

            ColumnExtractionParameters secondExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                secondFileColumnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.JoinLines}.{firstFileColumnNumber}.{joinTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, firstArgument, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<JoinType> processingParameters = new OperationTypeOutputParameters<JoinType>(
                outputFilePath,
                joinType,
                firstArgument);

            var lookupFileProcessor
                = new LookupFileProcessor<
                    ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                    ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                    DictionaryBuilder, Dictionary<string, string>,
                    JoinProcessor, OperationTypeOutputParameters<JoinType>>(
                    firstFilePath,
                    firstExtractionParameters,
                    secondFilePath,
                    secondExtractionParameters,
                    processingParameters);

            lookupFileProcessor.ProcessFiles();
        }

        private static void ConcatenateLines(
            string firstFilePath,
            string secondFilePath,
            string columnSeparator,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.ConcatenateLines}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            StringOutputParameters processingParameters = new StringOutputParameters(
                outputFilePath,
                columnSeparator);

            var dualFileProcessor
                = new DualFileProcessor<
                    LineExtractor, UnusedType, string,
                    ConcatenateProcessor, StringOutputParameters>(
                    firstFilePath,
                    firstExtractionParameters: null,
                    secondFilePath,
                    secondExtractionParameters: null,
                    processingParameters);

            dualFileProcessor.ProcessFiles();
        }

        private static void SelectLinesByColumnValue(
            string filePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            ComparisonType comparisonType, string comparisonTypeString,
            string firstArgument,
            string secondArgument,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}.{comparisonTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<ComparisonType> processingParameters = new OperationTypeOutputParameters<ComparisonType>(
                outputFilePath,
                comparisonType,
                firstArgument,
                secondArgument);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectLineByColumnValueProcessor, OperationTypeOutputParameters<ComparisonType>>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByNumber(
            string filePath,
            PositionSelectionType selectionType, string selectionTypeString,
            string firstArgument,
            string secondArgument,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<PositionSelectionType> processingParameters = new OperationTypeOutputParameters<PositionSelectionType>(
                outputFilePath,
                selectionType,
                firstArgument,
                secondArgument);

            var fileProcessor
                = new FileProcessor<LineExtractor, UnusedType, string, SelectLineByNumberProcessor, OperationTypeOutputParameters<PositionSelectionType>>(
                    filePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectColumnsByNumber(
            string filePath,
            string columnSeparator,
            PositionSelectionType selectionType, string selectionTypeString,
            string firstArgument,
            string secondArgument,
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
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<PositionSelectionType> processingParameters = new OperationTypeOutputParameters<PositionSelectionType>(
                outputFilePath,
                selectionType,
                firstArgument,
                secondArgument);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectColumnByNumberProcessor, OperationTypeOutputParameters<PositionSelectionType>>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByLineString(
            string filePath,
            StringSelectionType selectionType, string selectionTypeString,
            string firstArgument,
            string secondArgument,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByLineString}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<StringSelectionType> processingParameters = new OperationTypeOutputParameters<StringSelectionType>(
                outputFilePath,
                selectionType,
                firstArgument,
                secondArgument);

            var fileProcessor
                = new FileProcessor<LineAsParsedLineExtractor, UnusedType, ParsedLine, SelectLineByStringProcessor, OperationTypeOutputParameters<StringSelectionType>>(
                    filePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByColumnString(
            string filePath,
            int columnNumber,
            string columnSeparator,
            StringSelectionType selectionType, string selectionTypeString,
            string firstArgument,
            string secondArgument,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnString}.{columnNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<StringSelectionType> processingParameters = new OperationTypeOutputParameters<StringSelectionType>(
                outputFilePath,
                selectionType,
                firstArgument,
                secondArgument);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectLineByStringProcessor, OperationTypeOutputParameters<StringSelectionType>>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByColumnCount(
            string filePath,
            string columnSeparator,
            ComparisonType comparisonType, string comparisonTypeString,
            string firstArgument,
            string secondArgument,
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
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<ComparisonType> processingParameters = new OperationTypeOutputParameters<ComparisonType>(
                outputFilePath,
                comparisonType,
                firstArgument,
                secondArgument);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectLineByColumnCountProcessor, OperationTypeOutputParameters<ComparisonType>>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesHandlingRepeatedLines(
            string filePath,
            RepetitionHandlingType handlingType, string handlingTypeString,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesHandlingRepeatedLines}.{handlingTypeString}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<RepetitionHandlingType> processingParameters = new OperationTypeOutputParameters<RepetitionHandlingType>(
                outputFilePath,
                handlingType);

            var fileProcessor
                = new FileProcessor<LineAsParsedLineExtractor, UnusedType, ParsedLine, SelectLineHandlingRepeteadStringsProcessor, OperationTypeOutputParameters<RepetitionHandlingType>>(
                    filePath,
                    extractionParameters: null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesHandlingRepeatedColumnStrings(
            string filePath,
            int columnNumber,
            string columnSeparator,
            RepetitionHandlingType handlingType, string handlingTypeString,
            string outputFilePath)
        {
            ColumnExtractionParameters extractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesHandlingRepeatedColumnStrings}.{columnNumber}.{handlingTypeString}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<RepetitionHandlingType> processingParameters = new OperationTypeOutputParameters<RepetitionHandlingType>(
                outputFilePath,
                handlingType);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SelectLineHandlingRepeteadStringsProcessor, OperationTypeOutputParameters<RepetitionHandlingType>>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByLookupInFile(
            string dataFilePath,
            string lookupFilePath,
            LookupType lookupType, string lookupTypeString,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByLookupInFile}.{lookupTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(dataFilePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<LookupType> processingParameters = new OperationTypeOutputParameters<LookupType>(
                outputFilePath,
                lookupType);

            var lookupFileProcessor
                = new LookupFileProcessor<
                    LineAsParsedLineExtractor, UnusedType, ParsedLine,
                    LineExtractor, UnusedType, string,
                    HashSetBuilder, HashSet<string>,
                    LookupStringProcessor, OperationTypeOutputParameters<LookupType>>(
                    dataFilePath,
                    dataFileExtractionParameters: null,
                    lookupFilePath,
                    lookupFileExtractionParameters: null,
                    processingParameters);

            lookupFileProcessor.ProcessFiles();
        }

        private static void SelectLinesByColumnStringLookupInFile(
            string dataFilePath,
            int columnNumber,
            string columnSeparator,
            string lookupFilePath,
            LookupType lookupType, string lookupTypeString,
            string outputFilePath)
        {
            ColumnExtractionParameters dataFileExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnStringLookupInFile}.{columnNumber}.{lookupTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(dataFilePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<LookupType> processingParameters = new OperationTypeOutputParameters<LookupType>(
                outputFilePath,
                lookupType);

            var lookupFileProcessor
                = new LookupFileProcessor<
                    ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                    LineExtractor, UnusedType, string,
                    HashSetBuilder, HashSet<string>,
                    LookupStringProcessor, OperationTypeOutputParameters<LookupType>>(
                    dataFilePath,
                    dataFileExtractionParameters,
                    lookupFilePath,
                    lookupFileExtractionParameters: null,
                    processingParameters);

            lookupFileProcessor.ProcessFiles();
        }

        private static void SplitLineRanges(
            string filePath,
            ulong rangeSize,
            string outputFilePath)
        {
            // The file name will be bundled by the processor with the start line number of each line range, so we exclude the text extension from it;
            // it will get appended by the processor internally.
            //
            string outputFileExtension = $".{CabeiroConstants.Commands.SplitLineRanges}.{rangeSize}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            StringAndUnsignedIntegerOutputParameters processingParameters = new StringAndUnsignedIntegerOutputParameters(
                outputFilePath,
                CabeiroConstants.Files.Extensions.Txt,
                rangeSize);

            var fileProcessor
                = new FileProcessor<LineExtractor, UnusedType, string, SplitLineRangesProcessor, StringAndUnsignedIntegerOutputParameters>(
                    filePath,
                    null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SplitColumns(
            string filePath,
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
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            StringOutputParameters processingParameters = new StringOutputParameters(
                outputFilePath,
                CabeiroConstants.Files.Extensions.Txt);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SplitColumnsProcessor, StringOutputParameters>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SplitColumnStrings(
            string filePath,
            int columnNumber,
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
            string outputFileExtension = $".{CabeiroConstants.Commands.SplitColumnStrings}.{columnNumber}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            StringAndIntegerOutputParameters processingParameters = new StringAndIntegerOutputParameters(
                outputFilePath,
                CabeiroConstants.Files.Extensions.Txt,
                columnNumber);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SplitColumnValuesProcessor, StringAndIntegerOutputParameters>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SortFileBySecondColumnValue(
            string filePath,
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
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<ColumnExtractor, ColumnExtractionParameters, ParsedLine, SortBySecondColumnValueProcessor, BaseOutputParameters>(
                    filePath,
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
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var dualFileProcessor
                = new DualFileProcessor<
                    LineAsParsedLineExtractor, UnusedType, ParsedLine,
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
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
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

        private static void SelectLinesPostSortingByLookupInFile(
            string dataFilePath,
            string lookupFilePath,
            LookupType lookupType, string lookupTypeString,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesPostSortingByLookupInFile}.{lookupTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(dataFilePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<LookupType> processingParameters = new OperationTypeOutputParameters<LookupType>(
                outputFilePath,
                lookupType);

            var dualFileProcessor
                = new DualFileProcessor<
                    LineAsParsedLineExtractor, UnusedType, ParsedLine,
                    LookupProcessor, OperationTypeOutputParameters<LookupType>>(
                    dataFilePath,
                    firstExtractionParameters: null,
                    lookupFilePath,
                    secondExtractionParameters: null,
                    processingParameters);

            dualFileProcessor.ProcessFiles();
        }

        private static void SelectLinesPostSortingByColumnValueLookupInFile(
            string dataFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            string lookupFilePath,
            LookupType lookupType, string lookupTypeString,
            string outputFilePath)
        {
            ColumnExtractionParameters dataFileExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            ColumnExtractionParameters lookupFileExtractionParameters = new ColumnExtractionParameters(
                columnSeparator,
                columnNumber: 1,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnStringLookupInFile}.{columnNumber}.{dataTypeString.ToLower()}.{lookupTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(dataFilePath, outputFileExtension, firstArgument: null, secondArgument: null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeOutputParameters<LookupType> processingParameters = new OperationTypeOutputParameters<LookupType>(
                outputFilePath,
                lookupType);

            var dualFileProcessor
                = new DualFileProcessor<
                    ColumnExtractor, ColumnExtractionParameters, ParsedLine,
                    LookupProcessor, OperationTypeOutputParameters<LookupType>>(
                    dataFilePath,
                    dataFileExtractionParameters,
                    lookupFilePath,
                    lookupFileExtractionParameters,
                    processingParameters);

            dualFileProcessor.ProcessFiles();
        }
    }
}
