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
using LaurentiuCristofor.Proteus.Common.Logging;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataGenerators;
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

                // Initialize Proteus library's logging interface.
                //
                LoggingManager.Initialize();

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
            const int expectedProteusMajorVersion = 1;
            const int expectedProteusMinorVersion = 3;

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
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, expectedArgumentNumber);

                string inputFilePath = arguments[1];

                CountLines(inputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.AnalyzeLines))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 3;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                int valuesLimit = ArgumentParser.GetPositiveInteger(arguments[2]);

                AnalyzeLines(
                    inputFilePath,
                    valuesLimit);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.AnalyzeColumnValues))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.Invert))
            {
                const int minimumArgumentNumber = 2;
                const int maximumArgumentNumber = 3;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                ArgumentParser.ExtractLastArguments(0, 2, arguments, out _, out string outputFilePath);

                InvertFile(
                    inputFilePath,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.Sort))
            {
                const int minimumArgumentNumber = 2;
                const int maximumArgumentNumber = 3;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                ArgumentParser.ExtractLastArguments(0, 2, arguments, out _, out string outputFilePath);

                SortFile(
                    inputFilePath,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SortByColumnValue))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.CustomSort))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 4;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                Tuple<SortingAlgorithmType, int> operationInfo = ArgumentParser.ParseSortingAlgorithmType(arguments[2]);
                ArgumentParser.ExtractLastArguments(operationInfo.Item2, 3, arguments, out _, out string outputFilePath);

                CustomSortFile(
                    inputFilePath,
                    operationInfo.Item1, arguments[2],
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.CustomSortByColumnValue))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 7;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                Tuple<SortingAlgorithmType, int> operationInfo = ArgumentParser.ParseSortingAlgorithmType(arguments[5]);
                ArgumentParser.ExtractLastArguments(operationInfo.Item2, 6, arguments, out _, out string outputFilePath);

                CustomSortFileByColumnValue(
                    inputFilePath,
                    columnNumber,
                    columnSeparator,
                    dataType, arguments[4],
                    operationInfo.Item1, arguments[5],
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.Shuffle))
            {
                const int minimumArgumentNumber = 2;
                const int maximumArgumentNumber = 3;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                ArgumentParser.ExtractLastArguments(0, 2, arguments, out _, out string outputFilePath);

                ShuffleFile(
                    inputFilePath,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.OrderColumns))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.EditLines))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.EditColumnStrings))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 8;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.EditColumnValues))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 8;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.InsertLine))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.JoinLines)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.JoinLinesPostSorting))
            {
                const int minimumArgumentNumber = 8;
                const int maximumArgumentNumber = 10;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.ConcatenateLines))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.TransformLines))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                Tuple<LineTransformationType, int> operationInfo = ArgumentParser.ParseLineTransformationType(arguments[3]);
                ArgumentParser.ExtractLastArguments(operationInfo.Item2, 4, arguments, out string[] operationArguments, out string outputFilePath);

                TransformLines(
                    inputFilePath,
                    columnSeparator,
                    operationInfo.Item1, arguments[3],
                    operationArguments,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.TransformColumns))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 8;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnValue))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 9;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByNumber))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectColumnsByNumber))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 7;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByLineString))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnString))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 8;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnCount))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 7;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByLineStringRelativeToOtherLines)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByLineStringRelativeToOtherLines))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 4;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                Tuple<RelativeValueSelectionType, int> operationInfo = ArgumentParser.ParseRelativeLineSelectionType(arguments[2]);
                ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out string outputFilePath);

                bool isSorted = ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByLineStringRelativeToOtherLines);

                SelectLinesByLineStringRelativeToOtherLines(
                    isSorted,
                    inputFilePath,
                    operationInfo.Item1, arguments[2],
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnValueRelativeToOtherLines)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueRelativeToOtherLines))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 7;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                Tuple<RelativeValueSelectionType, int> operationInfo = ArgumentParser.ParseRelativeValueSelectionType(arguments[5]);
                ArgumentParser.ExtractLastArguments(0, 6, arguments, out _, out string outputFilePath);

                bool isSorted = ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueRelativeToOtherLines);

                SelectLinesByColumnValueRelativeToOtherLines(
                    isSorted,
                    inputFilePath,
                    columnNumber,
                    columnSeparator,
                    dataType, arguments[4],
                    operationInfo.Item1, arguments[5],
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByLookupInFile)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByLookupInFile))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByColumnValueLookupInFile)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueLookupInFile))
            {
                const int minimumArgumentNumber = 8;
                const int maximumArgumentNumber = 9;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
                    operationInfo.Item1, arguments[7],
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesSample))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                int seedValue = int.Parse(arguments[2]);
                int sampleSize = ArgumentParser.GetStrictlyPositiveInteger(arguments[3]);
                ArgumentParser.ExtractLastArguments(0, 4, arguments, out _, out string outputFilePath);

                SelectLinesSample(
                    inputFilePath,
                    seedValue,
                    sampleSize,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SplitLineRanges))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 4;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                ulong rangeSize = ArgumentParser.GetUnsignedLongInteger(arguments[2]);
                ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out string outputFilePath);

                SplitLineRanges(
                    inputFilePath,
                    rangeSize,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SplitColumns))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 4;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                string columnSeparator = ArgumentParser.ParseSeparator(arguments[2]);
                ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out string outputFilePath);

                SplitColumns(
                    inputFilePath,
                    columnSeparator,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SplitColumnValues))
            {
                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SplitLinesIntoRandomSets))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                int seedValue = int.Parse(arguments[2]);
                int setsCount = ArgumentParser.GetStrictlyPositiveInteger(arguments[3]);
                ArgumentParser.ExtractLastArguments(0, 4, arguments, out _, out string outputFilePath);

                SplitLinesIntoRandomSets(
                    inputFilePath,
                    seedValue,
                    setsCount,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SortBySecondColumnValue))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 8;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.MergeLines))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 4;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string firstFilePath = arguments[1];
                string secondFilePath = arguments[2];
                ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out string outputFilePath);

                MergeLines(
                    firstFilePath,
                    secondFilePath,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.MergeLinesByColumnValue))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 7;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string firstFilePath = arguments[1];
                int firstFileColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                string secondFilePath = arguments[5];
                ArgumentParser.ExtractLastArguments(0, 6, arguments, out _, out string outputFilePath);

                MergeLinesByColumnValue(
                    firstFilePath,
                    firstFileColumnNumber,
                    columnSeparator,
                    dataType, arguments[4],
                    secondFilePath,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.FindStateTransitions))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 8;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.GenerateDistribution))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                int seedValue = int.Parse(arguments[1]);
                ulong generationCount = ArgumentParser.GetUnsignedLongInteger(arguments[2]);
                Tuple<DistributionType, int> distributionInfo = ArgumentParser.ParseDistributionType(arguments[3]);
                ArgumentParser.ExtractLastArguments(distributionInfo.Item2, 4, arguments, out string[] generationArguments, out string outputFilePath);

                GenerateDistribution(
                    seedValue,
                    generationCount,
                    distributionInfo.Item1, arguments[3],
                    generationArguments,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.GenerateProgression))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                ulong generationCount = ArgumentParser.GetUnsignedLongInteger(arguments[1]);
                Tuple<ProgressionType, int> progressionInfo = ArgumentParser.ParseProgressionType(arguments[2]);
                ArgumentParser.ExtractLastArguments(progressionInfo.Item2, 3, arguments, out string[] generationArguments, out string outputFilePath);

                GenerateProgression(
                    generationCount,
                    progressionInfo.Item1, arguments[2],
                    generationArguments,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.GenerateSample))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 5;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                int seedValue = int.Parse(arguments[1]);
                ulong generationCount = ArgumentParser.GetUnsignedLongInteger(arguments[2]);
                ulong totalCount = ArgumentParser.GetUnsignedLongInteger(arguments[3]);
                ArgumentParser.ExtractLastArguments(0, 4, arguments, out _, out string outputFilePath);

                GenerateSample(
                    seedValue,
                    generationCount,
                    totalCount,
                    outputFilePath);
                return;
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
                    /*extractionParameters:*/ null,
                    /*processingParameters:*/ null);

            fileProcessor.ProcessFile();
        }

        private static void AnalyzeLines(string inputFilePath, int valuesLimit)
        {
            AnalyzeParameters processingParameters = new AnalyzeParameters(DataType.String, valuesLimit);

            var fileProcessor
                = new FileProcessor<LineAsOneExtractedValueExtractor, Unused, OneExtractedValue, AnalyzeProcessor, AnalyzeParameters>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void AnalyzeColumnValues(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType,
            int valuesLimit)
        {
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            AnalyzeParameters processingParameters = new AnalyzeParameters(dataType, valuesLimit);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, AnalyzeProcessor, AnalyzeParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void InvertFile(
            string inputFilePath,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.Invert}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, FileInvertProcessor, BaseOutputParameters>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SortFile(
            string inputFilePath,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.Sort}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SortProcessor, BaseOutputParameters>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
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
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SortByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, SortByColumnValueProcessor, BaseOutputParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void CustomSortFile(
            string inputFilePath,
            SortingAlgorithmType algorithmType, string algorithmTypeString,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.CustomSort}.{algorithmTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<SortingAlgorithmType> processingParameters = new OutputOperationParameters<SortingAlgorithmType>(
                outputFilePath,
                algorithmType);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, CustomSortProcessor, OutputOperationParameters<SortingAlgorithmType>>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void CustomSortFileByColumnValue(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            SortingAlgorithmType algorithmType, string algorithmTypeString,
            string outputFilePath)
        {
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.CustomSortByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}.{algorithmTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<SortingAlgorithmType> processingParameters = new OutputOperationParameters<SortingAlgorithmType>(
                outputFilePath,
                algorithmType);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, CustomSortByColumnValueProcessor, OutputOperationParameters<SortingAlgorithmType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void ShuffleFile(
            string inputFilePath,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.Shuffle}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, ShuffleProcessor, BaseOutputParameters>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void OrderColumns(
            string inputFilePath,
            string columnSeparator,
            string newFirstColumnsList,
            string outputFilePath)
        {
            ColumnStringsExtractionParameters extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.OrderColumns}.{newFirstColumnsList}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraParameters processingParameters = new OutputExtraParameters(
                outputFilePath,
                new string[] { newFirstColumnsList });

            var fileProcessor
                = new FileProcessor<ColumnStringsExtractor, ColumnStringsExtractionParameters, ExtractedColumnStrings, OrderColumnsProcessor, OutputExtraParameters>(
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

            OutputExtraOperationParameters<StringEditType> processingParameters = new OutputExtraOperationParameters<StringEditType>(
                outputFilePath,
                editType,
                editArguments);

            var fileProcessor
                = new FileProcessor<LineAsOneExtractedValueExtractor, Unused, OneExtractedValue, EditStringProcessor, OutputExtraOperationParameters<StringEditType>>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
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
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.EditColumnStrings}.{columnNumber}.{editTypeString.ToLower()}";
            var filePathBuilder = new EditOperationFilePathBuilder(editType, inputFilePath, outputFileExtension, editArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraOperationParameters<StringEditType> processingParameters = new OutputExtraOperationParameters<StringEditType>(
                outputFilePath,
                editType,
                editArguments);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, EditStringProcessor, OutputExtraOperationParameters<StringEditType>>(
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
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
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

                if (argument == null)
                {
                    throw new CabeiroException($"'{editArguments[0]}' is not a valid value for the {dataType} data type !");
                }
            }

            OutputValueEditParameters processingParameters = new OutputValueEditParameters(
                outputFilePath,
                editType,
                argument);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, EditColumnValueProcessor, OutputValueEditParameters>(
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
            OutputExtraOperationParameters<PositionInsertionType> processingParameters = new OutputExtraOperationParameters<PositionInsertionType>(
                outputFilePath,
                insertionType,
                (insertionArguments.Length > 0) ? new string[] { lineValue, insertionArguments[0] } : new string[] { lineValue });

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, InsertLineProcessor, OutputExtraOperationParameters<PositionInsertionType>>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
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
            OneColumnValueExtractionParameters firstExtractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                firstFileColumnNumber,
                dataType);

            OneColumnValueExtractionParameters secondExtractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                secondFileColumnNumber,
                dataType);

            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.JoinLinesPostSorting}.{firstFileColumnNumber}.{dataTypeString.ToLower()}.{joinTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.JoinLines}.{firstFileColumnNumber}.{dataTypeString.ToLower()}.{joinTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, joinArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraOperationParameters<JoinType> processingParameters = new OutputExtraOperationParameters<JoinType>(
                outputFilePath,
                joinType,
                joinArguments);

            if (isSorted)
            {
                var dualFileProcessor
                    = new DualFileProcessor<
                        OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue,
                        JoinPostSortingProcessor, OutputExtraOperationParameters<JoinType>>(
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
                        OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue,
                        OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue,
                        JoinBuilder, Dictionary<IDataHolder, List<string>>,
                        JoinProcessor, OutputExtraOperationParameters<JoinType>>(
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
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraParameters processingParameters = new OutputExtraParameters(
                outputFilePath,
                new string[] { columnSeparator });

            var dualFileProcessor
                = new DualFileProcessor<
                    LineExtractor, Unused, string,
                    ConcatenateProcessor, OutputExtraParameters>(
                    firstFilePath,
                    /*firstExtractionParameters:*/ null,
                    secondFilePath,
                    /*secondExtractionParameters:*/ null,
                    processingParameters);

            dualFileProcessor.ProcessFiles();
        }

        private static void TransformLines(
            string inputFilePath,
            string columnSeparator,
            LineTransformationType transformationType, string transformationTypeString,
            string[] transformationArguments,
            string outputFilePath)
        {
            ColumnStringsExtractionParameters extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.TransformLines}.{transformationTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, transformationArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraOperationParameters<LineTransformationType> processingParameters = new OutputExtraOperationParameters<LineTransformationType>(
                outputFilePath,
                transformationType,
                transformationArguments);

            var fileProcessor
                = new FileProcessor<ColumnStringsExtractor, ColumnStringsExtractionParameters, ExtractedColumnStrings, TransformLinesProcessor, OutputExtraOperationParameters<LineTransformationType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void TransformColumns(
            string inputFilePath,
            string columnSeparator,
            ColumnTransformationType transformationType, string transformationTypeString,
            string[] transformationArguments,
            string outputFilePath)
        {
            ColumnStringsExtractionParameters extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.TransformColumns}.{transformationTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, transformationArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraOperationParameters<ColumnTransformationType> processingParameters = new OutputExtraOperationParameters<ColumnTransformationType>(
                outputFilePath,
                transformationType,
                transformationArguments);

            var fileProcessor
                = new FileProcessor<ColumnStringsExtractor, ColumnStringsExtractionParameters, ExtractedColumnStrings, TransformColumnsProcessor, OutputExtraOperationParameters<ColumnTransformationType>>(
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
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}.{comparisonTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, comparisonArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraOperationParameters<ComparisonType> processingParameters = new OutputExtraOperationParameters<ComparisonType>(
                outputFilePath,
                comparisonType,
                comparisonArguments);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, SelectLineByColumnValueProcessor, OutputExtraOperationParameters<ComparisonType>>(
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

            OutputExtraOperationParameters<PositionSelectionType> processingParameters = new OutputExtraOperationParameters<PositionSelectionType>(
                outputFilePath,
                selectionType,
                selectionArguments);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SelectLineByNumberProcessor, OutputExtraOperationParameters<PositionSelectionType>>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
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
            ColumnStringsExtractionParameters extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectColumnsByNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, selectionArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraOperationParameters<PositionSelectionType> processingParameters = new OutputExtraOperationParameters<PositionSelectionType>(
                outputFilePath,
                selectionType,
                selectionArguments);

            var fileProcessor
                = new FileProcessor<ColumnStringsExtractor, ColumnStringsExtractionParameters, ExtractedColumnStrings, SelectColumnByNumberProcessor, OutputExtraOperationParameters<PositionSelectionType>>(
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

            OutputExtraOperationParameters<StringSelectionType> processingParameters = new OutputExtraOperationParameters<StringSelectionType>(
                outputFilePath,
                selectionType,
                selectionArguments);

            var fileProcessor
                = new FileProcessor<LineAsOneExtractedValueExtractor, Unused, OneExtractedValue, SelectLineByStringProcessor, OutputExtraOperationParameters<StringSelectionType>>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
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
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnString}.{columnNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, selectionArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraOperationParameters<StringSelectionType> processingParameters = new OutputExtraOperationParameters<StringSelectionType>(
                outputFilePath,
                selectionType,
                selectionArguments);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, SelectLineByStringProcessor, OutputExtraOperationParameters<StringSelectionType>>(
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
            ColumnStringsExtractionParameters extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnCount}.{comparisonTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, comparisonArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraOperationParameters<ComparisonType> processingParameters = new OutputExtraOperationParameters<ComparisonType>(
                outputFilePath,
                comparisonType,
                comparisonArguments);

            var fileProcessor
                = new FileProcessor<ColumnStringsExtractor, ColumnStringsExtractionParameters, ExtractedColumnStrings, SelectLineByColumnCountProcessor, OutputExtraOperationParameters<ComparisonType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByLineStringRelativeToOtherLines(
            bool isSorted,
            string inputFilePath,
            RelativeValueSelectionType selectionType, string selectionTypeString,
            string outputFilePath)
        {
            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.SelectLinesPostSortingByLineStringRelativeToOtherLines}.{selectionTypeString}"
                : $".{CabeiroConstants.Commands.SelectLinesByLineStringRelativeToOtherLines}.{selectionTypeString}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<RelativeValueSelectionType> processingParameters = new OutputOperationParameters<RelativeValueSelectionType>(
                outputFilePath,
                selectionType);

            if (isSorted)
            {
                var fileProcessor
                    = new FileProcessor<LineAsOneExtractedValueExtractor, Unused, OneExtractedValue, SelectLinePostSortingByValueRelativeToOtherLinesProcessor, OutputOperationParameters<RelativeValueSelectionType>>(
                        inputFilePath,
                        /*extractionParameters:*/ null,
                        processingParameters);

                fileProcessor.ProcessFile();
            }
            else
            {
                var fileProcessor
                    = new FileProcessor<LineAsOneExtractedValueExtractor, Unused, OneExtractedValue, SelectLineByValueRelativeToOtherLinesProcessor, OutputOperationParameters<RelativeValueSelectionType>>(
                        inputFilePath,
                        /*extractionParameters:*/ null,
                        processingParameters);

                fileProcessor.ProcessFile();
            }
        }

        private static void SelectLinesByColumnValueRelativeToOtherLines(
            bool isSorted,
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            RelativeValueSelectionType selectionType, string selectionTypeString,
            string outputFilePath)
        {
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueRelativeToOtherLines}.{columnNumber}.{dataTypeString.ToLower()}.{selectionTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.SelectLinesByColumnValueRelativeToOtherLines}.{columnNumber}.{dataTypeString.ToLower()}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<RelativeValueSelectionType> processingParameters = new OutputOperationParameters<RelativeValueSelectionType>(
                outputFilePath,
                selectionType);

            if (isSorted)
            {
                var fileProcessor
                    = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, SelectLinePostSortingByValueRelativeToOtherLinesProcessor, OutputOperationParameters<RelativeValueSelectionType>>(
                        inputFilePath,
                        extractionParameters,
                        processingParameters);

                fileProcessor.ProcessFile();
            }
            else
            {
                var fileProcessor
                    = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, SelectLineByValueRelativeToOtherLinesProcessor, OutputOperationParameters<RelativeValueSelectionType>>(
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
            var filePathBuilder = new FilePathBuilder(dataFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<LookupType> processingParameters = new OutputOperationParameters<LookupType>(
                outputFilePath,
                lookupType);

            if (isSorted)
            {
                var dualFileProcessor
                    = new DualFileProcessor<
                        LineAsOneExtractedValueExtractor, Unused, OneExtractedValue,
                        LookupPostSortingProcessor, OutputOperationParameters<LookupType>>(
                        dataFilePath,
                        /*firstExtractionParameters:*/ null,
                        lookupFilePath,
                        /*secondExtractionParameters:*/ null,
                        processingParameters);

                dualFileProcessor.ProcessFiles();
            }
            else
            {
                var lookupFileProcessor
                    = new LookupFileProcessor<
                        LineAsOneExtractedValueExtractor, Unused, OneExtractedValue,
                        LineAsOneExtractedValueExtractor, Unused, OneExtractedValue,
                        LookupBuilder, HashSet<IDataHolder>,
                        LookupProcessor, OutputOperationParameters<LookupType>>(
                        dataFilePath,
                        /*dataFileExtractionParameters:*/ null,
                        lookupFilePath,
                        /*lookupFileExtractionParameters:*/ null,
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
            OneColumnValueExtractionParameters dataFileExtractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                dataFileColumnNumber,
                dataType);

            OneColumnValueExtractionParameters lookupFileExtractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                lookupFileColumnNumber,
                dataType);

            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueLookupInFile}.{dataFileColumnNumber}.{dataTypeString.ToLower()}.{lookupTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.SelectLinesByColumnValueLookupInFile}.{dataFileColumnNumber}.{dataTypeString.ToLower()}.{lookupTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(dataFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputOperationParameters<LookupType> processingParameters = new OutputOperationParameters<LookupType>(
                outputFilePath,
                lookupType);

            if (isSorted)
            {
                var dualFileProcessor
                    = new DualFileProcessor<
                        OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue,
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
                        OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue,
                        OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue,
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

        private static void SelectLinesSample(
            string inputFilePath,
            int seedValue,
            int sampleSize,
            string outputFilePath)
        {
            string outputFileExtension
                = (seedValue >= 0)
                ? $".{CabeiroConstants.Commands.SelectLinesSample}.{seedValue}.{sampleSize}"
                : $".{CabeiroConstants.Commands.SelectLinesSample}.{sampleSize}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OutputExtraParameters processingParameters = new OutputExtraParameters(
                outputFilePath,
                null,
                new int[] { seedValue, sampleSize });

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SampleProcessor, OutputExtraParameters>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
                    processingParameters);

            fileProcessor.ProcessFile();
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
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            OutputExtraParameters processingParameters = new OutputExtraParameters(
                outputFilePath,
                new string[] { CabeiroConstants.Files.Extensions.Txt },
                null,
                new ulong[] { rangeSize });

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SplitLineRangesProcessor, OutputExtraParameters>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SplitColumns(
            string inputFilePath,
            string columnSeparator,
            string outputFilePath)
        {
            ColumnStringsExtractionParameters extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            // The file name will be bundled by the processor with the number of each column, so we exclude the text extension from it;
            // it will get appended by the processor internally.
            //
            string outputFileExtension = $".{CabeiroConstants.Commands.SplitColumns}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            OutputExtraParameters processingParameters = new OutputExtraParameters(
                outputFilePath,
                new string[] { CabeiroConstants.Files.Extensions.Txt });

            var fileProcessor
                = new FileProcessor<ColumnStringsExtractor, ColumnStringsExtractionParameters, ExtractedColumnStrings, SplitColumnsProcessor, OutputExtraParameters>(
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
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            // The file name will be bundled by the processor with the number of each distinct column value, so we exclude the text extension from it;
            // it will get appended by the processor internally.
            //
            string outputFileExtension = $".{CabeiroConstants.Commands.SplitColumnValues}.{columnNumber}.{dataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            OutputExtraParameters processingParameters = new OutputExtraParameters(
                outputFilePath,
                new string[] { CabeiroConstants.Files.Extensions.Txt });

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, SplitColumnValuesProcessor, OutputExtraParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SplitLinesIntoRandomSets(
            string inputFilePath,
            int seedValue,
            int setsCount,
            string outputFilePath)
        {
            // The file name will be bundled by the processor with the number of each set, so we exclude the text extension from it;
            // it will get appended by the processor internally.
            //
            string outputFileExtension
                = (seedValue >= 0)
                ? $".{CabeiroConstants.Commands.SplitLinesIntoRandomSets}.{seedValue}.{setsCount}"
                : $".{CabeiroConstants.Commands.SplitLinesIntoRandomSets}.{setsCount}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            OutputExtraParameters processingParameters = new OutputExtraParameters(
                outputFilePath,
                new string[] { CabeiroConstants.Files.Extensions.Txt },
                new int[] { seedValue, setsCount });

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SplitLinesIntoRandomSetsProcessor, OutputExtraParameters>(
                    inputFilePath,
                    /*extractionParameters:*/ null,
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
            TwoColumnValuesExtractionParameters extractionParameters = new TwoColumnValuesExtractionParameters(
                columnSeparator,
                firstColumnNumber,
                firstDataType,
                secondColumnNumber,
                secondDataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SortBySecondColumnValue}.{secondColumnNumber}.{secondDataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<TwoColumnValuesExtractor, TwoColumnValuesExtractionParameters, TwoExtractedValues, SortBySecondColumnValueProcessor, BaseOutputParameters>(
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
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var dualFileProcessor
                = new DualFileProcessor<
                    LineAsOneExtractedValueExtractor, Unused, OneExtractedValue,
                    MergeProcessor, BaseOutputParameters>(
                    firstFilePath,
                    /*firstExtractionParameters:*/ null,
                    secondFilePath,
                    /*secondExtractionParameters:*/ null,
                    processingParameters);

            dualFileProcessor.ProcessFiles();
        }

        private static void MergeLinesByColumnValue(
            string firstFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            string secondFilePath,
            string outputFilePath)
        {
            OneColumnValueExtractionParameters extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.MergeLinesByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var dualFileProcessor
                = new DualFileProcessor<
                    OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue,
                    MergeProcessor, BaseOutputParameters>(
                    firstFilePath,
                    extractionParameters,
                    secondFilePath,
                    extractionParameters,
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
            TwoColumnValuesExtractionParameters extractionParameters = new TwoColumnValuesExtractionParameters(
                columnSeparator,
                firstColumnNumber,
                firstDataType,
                secondColumnNumber,
                secondDataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.FindStateTransitions}.{secondColumnNumber}.{secondDataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileProcessor
                = new FileProcessor<TwoColumnValuesExtractor, TwoColumnValuesExtractionParameters, TwoExtractedValues, FindStateTransitionsProcessor, BaseOutputParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void GenerateDistribution(
            int seedValue,
            ulong generationCount,
            DistributionType distributionType, string distributionTypeString,
            string[] generationArguments,
            string outputFilePath)
        {
            DistributionGenerationParameters generationParameters = new DistributionGenerationParameters(
                seedValue,
                generationCount,
                distributionType,
                (generationArguments.Length == 0) ? null : generationArguments[0]);

            string outputFileExtension
                = (seedValue >= 0)
                ? $".{CabeiroConstants.Commands.GenerateDistribution}.{seedValue}.{distributionTypeString.ToLower()}.{generationCount}"
                : $".{CabeiroConstants.Commands.GenerateDistribution}.{distributionTypeString.ToLower()}.{generationCount}";
            var filePathBuilder = new FilePathBuilder(CabeiroConstants.Program.Name, outputFileExtension, generationArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileGenerationProcessor
                = new FileGenerationProcessor<DistributionGenerator, DistributionGenerationParameters>(
                    generationParameters,
                    processingParameters);

            fileGenerationProcessor.GenerateFile();
        }

        private static void GenerateProgression(
            ulong generationCount,
            ProgressionType progressionType, string progressionTypeString,
            string[] generationArguments,
            string outputFilePath)
        {
            double[] progressionArguments = null;
            if (generationArguments.Length == 2)
            {
                double firstArgument = double.Parse(generationArguments[0]);
                double secondArgument = double.Parse(generationArguments[1]);
                progressionArguments = new double[] { firstArgument, secondArgument };
            }

            ProgressionGenerationParameters generationParameters = new ProgressionGenerationParameters(
                generationCount,
                progressionType,
                progressionArguments);

            string outputFileExtension = $".{CabeiroConstants.Commands.GenerateProgression}.{progressionTypeString.ToLower()}.{generationCount}";
            var filePathBuilder = new FilePathBuilder(CabeiroConstants.Program.Name, outputFileExtension, generationArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileGenerationProcessor
                = new FileGenerationProcessor<ProgressionGenerator, ProgressionGenerationParameters>(
                    generationParameters,
                    processingParameters);

            fileGenerationProcessor.GenerateFile();
        }

        private static void GenerateSample(
            int seedValue,
            ulong generationCount,
            ulong totalCount,
            string outputFilePath)
        {
            SampleGenerationParameters generationParameters = new SampleGenerationParameters(
                seedValue,
                generationCount,
                totalCount);

            string outputFileExtension
                = (seedValue >= 0)
                ? $".{CabeiroConstants.Commands.GenerateSample}.{seedValue}.{totalCount}.{generationCount}"
                : $".{CabeiroConstants.Commands.GenerateSample}.{totalCount}.{generationCount}";
            var filePathBuilder = new FilePathBuilder(CabeiroConstants.Program.Name, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            BaseOutputParameters processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileGenerationProcessor
                = new FileGenerationProcessor<SampleGenerator, SampleGenerationParameters>(
                    generationParameters,
                    processingParameters);

            fileGenerationProcessor.GenerateFile();
        }
    }
}
