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
                // Validate the version of the Proteus library.
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
            const int expectedProteusMajorVersion = 2;
            const int expectedProteusMinorVersion = 4;

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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.CalculateConditionalEntropy))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 7;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                int secondColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[5]);
                DataType secondDataType = ArgumentParser.ParseDataType(arguments[6]);

                CalculateConditionalEntropy(
                    inputFilePath,
                    columnNumber,
                    columnSeparator,
                    dataType,
                    secondColumnNumber,
                    secondDataType);
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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.Sort)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.CustomSort))
            {
                bool isCustom = ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.CustomSort);

                const int minimumArgumentNumber = 2;
                const int maximumArgumentNumber = 3;
                ArgumentParser.CheckExpectedArgumentNumber(
                    arguments.Length,
                    isCustom ? minimumArgumentNumber + 1 : minimumArgumentNumber,
                    isCustom ? maximumArgumentNumber + 1 : maximumArgumentNumber);

                string inputFilePath = arguments[1];
                Tuple<SortingAlgorithmType, int> operationInfo
                    = isCustom
                    ? ArgumentParser.ParseSortingAlgorithmType(arguments[2])
                    : new Tuple<SortingAlgorithmType, int>(SortingAlgorithmType.NotSet, 0);
                ArgumentParser.ExtractLastArguments(operationInfo.Item2, isCustom ? 3 : 2, arguments, out _, out string outputFilePath);

                SortFile(
                    inputFilePath,
                    operationInfo.Item1, isCustom ? arguments[2] : null,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SortByColumnValue)
                || ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.CustomSortByColumnValue))
            {
                bool isCustom = ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.CustomSortByColumnValue);

                const int minimumArgumentNumber = 5;
                const int maximumArgumentNumber = 6;
                ArgumentParser.CheckExpectedArgumentNumber(
                    arguments.Length,
                    isCustom ? minimumArgumentNumber + 1 : minimumArgumentNumber,
                    isCustom ? maximumArgumentNumber + 1 : maximumArgumentNumber);

                string inputFilePath = arguments[1];
                int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                Tuple<SortingAlgorithmType, int> operationInfo
                    = isCustom
                    ? ArgumentParser.ParseSortingAlgorithmType(arguments[5])
                    : new Tuple<SortingAlgorithmType, int>(SortingAlgorithmType.NotSet, 0);
                ArgumentParser.ExtractLastArguments(operationInfo.Item2, isCustom ? 6 : 5, arguments, out _, out string outputFilePath);

                SortFileByColumnValue(
                    inputFilePath,
                    columnNumber,
                    columnSeparator,
                    dataType, arguments[4],
                    operationInfo.Item1, isCustom ? arguments[5] : null,
                    outputFilePath);
                return;
            }
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.Shuffle))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 4;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                int seedValue = int.Parse(arguments[1]);
                string inputFilePath = arguments[2];
                ArgumentParser.ExtractLastArguments(0, 3, arguments, out _, out string outputFilePath);

                ShuffleFile(
                    seedValue,
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
                    operationInfo.Item1, arguments[7],
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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByTwoColumnValues))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 8;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                Tuple<ComparisonType, int> operationInfo = ArgumentParser.ParseTwoColumnComparisonType(arguments[5]);
                int secondColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[6]);
                ArgumentParser.ExtractLastArguments(operationInfo.Item2, 7, arguments, out _, out string outputFilePath);

                SelectLinesByTwoColumnValues(
                    inputFilePath,
                    columnNumber,
                    columnSeparator,
                    dataType, arguments[4],
                    operationInfo.Item1, arguments[5],
                    secondColumnNumber,
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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByTwoColumnStrings))
            {
                const int minimumArgumentNumber = 6;
                const int maximumArgumentNumber = 7;
                ArgumentParser.CheckExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber);

                string inputFilePath = arguments[1];
                int columnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[2]);
                string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                Tuple<StringSelectionType, int> operationInfo = ArgumentParser.ParseTwoColumnSelectionType(arguments[4]);
                int secondColumnNumber = ArgumentParser.GetStrictlyPositiveInteger(arguments[5]);
                ArgumentParser.ExtractLastArguments(operationInfo.Item2, 6, arguments, out _, out string outputFilePath);

                SelectLinesByTwoColumnStrings(
                    inputFilePath,
                    columnNumber,
                    columnSeparator,
                    operationInfo.Item1, arguments[4],
                    secondColumnNumber,
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

                int seedValue = int.Parse(arguments[1]);
                string inputFilePath = arguments[2];
                int sampleSize = ArgumentParser.GetStrictlyPositiveInteger(arguments[3]);
                ArgumentParser.ExtractLastArguments(0, 4, arguments, out _, out string outputFilePath);

                SelectLinesSample(
                    seedValue,
                    inputFilePath,
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

                int seedValue = int.Parse(arguments[1]);
                string inputFilePath = arguments[2];
                int setsCount = ArgumentParser.GetStrictlyPositiveInteger(arguments[3]);
                ArgumentParser.ExtractLastArguments(0, 4, arguments, out _, out string outputFilePath);

                SplitLinesIntoRandomSets(
                    seedValue,
                    inputFilePath,
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
            var processingParameters = new AnalyzeParameters(DataType.String, valuesLimit);

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
            var extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            var processingParameters = new AnalyzeParameters(dataType, valuesLimit);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, AnalyzeProcessor, AnalyzeParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void CalculateConditionalEntropy(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType,
            int secondColumnNumber,
            DataType secondDataType)
        {
            var extractionParameters = new TwoColumnValuesExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType,
                secondColumnNumber,
                secondDataType);

            var processingParameters = new ConditionalEntropyParameters(dataType, secondDataType);

            var fileProcessor
                = new FileProcessor<TwoColumnValuesExtractor, TwoColumnValuesExtractionParameters, TwoExtractedValues, ConditionalEntropyProcessor, ConditionalEntropyParameters>(
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

            var processingParameters = new BaseOutputParameters(
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
            SortingAlgorithmType algorithmType, string algorithmTypeString,
            string outputFilePath)
        {
            string outputFileExtension
                = (algorithmType == SortingAlgorithmType.NotSet)
                ? $".{CabeiroConstants.Commands.Sort}"
                : $".{CabeiroConstants.Commands.CustomSort}.{algorithmTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new OutputOperationParameters<SortingAlgorithmType>(
                outputFilePath,
                algorithmType);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, SortProcessor, OutputOperationParameters<SortingAlgorithmType>>(
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
            SortingAlgorithmType algorithmType, string algorithmTypeString,
            string outputFilePath)
        {
            var extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension
                = (algorithmType == SortingAlgorithmType.NotSet)
                ? $".{CabeiroConstants.Commands.SortByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.CustomSortByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}.{algorithmTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new OutputOperationParameters<SortingAlgorithmType>(
                outputFilePath,
                algorithmType);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, SortByColumnValueProcessor, OutputOperationParameters<SortingAlgorithmType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void ShuffleFile(
            int seedValue,
            string inputFilePath,
            string outputFilePath)
        {
            string outputFileExtension
                = (seedValue >= 0)
                ? $".{CabeiroConstants.Commands.Shuffle}.{seedValue}"
                : $".{CabeiroConstants.Commands.Shuffle}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            int[] intParameters = new int[] { seedValue };

            var processingParameters = new OutputExtraParameters(
                outputFilePath,
                intParameters);

            var fileProcessor
                = new FileProcessor<LineExtractor, Unused, string, ShuffleProcessor, OutputExtraParameters>(
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
            var extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.OrderColumns}.{newFirstColumnsList}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            string[] stringParameters = new string[] { newFirstColumnsList };

            var processingParameters = new OutputExtraParameters(
                outputFilePath,
                stringParameters);

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

            ParseEditArguments(editType, editArguments, out string[] stringArguments, out int[] intArguments);

            var processingParameters = new OutputExtraOperationParameters<StringEditType>(
                outputFilePath,
                editType,
                stringArguments,
                intArguments);

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
            var extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.EditColumnStrings}.{columnNumber}.{editTypeString.ToLower()}";
            var filePathBuilder = new EditOperationFilePathBuilder(editType, inputFilePath, outputFileExtension, editArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            ParseEditArguments(editType, editArguments, out string[] stringArguments, out int[] intArguments);

            var processingParameters = new OutputExtraOperationParameters<StringEditType>(
                outputFilePath,
                editType,
                stringArguments,
                intArguments);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, EditStringProcessor, OutputExtraOperationParameters<StringEditType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void ParseEditArguments(StringEditType editType, string[] editArguments, out string[] stringArguments, out int[] intArguments)
        {
            stringArguments = null;
            intArguments = null;

            switch (editType)
            {
                case StringEditType.TrimCharsStart:
                case StringEditType.TrimCharsEnd:
                case StringEditType.TrimChars:
                case StringEditType.PrefixLineNumbers:
                case StringEditType.AddPrefix:
                case StringEditType.AddSuffix:
                case StringEditType.DeletePrefix:
                case StringEditType.DeleteSuffix:
                case StringEditType.DeleteContentBeforeMarker:
                case StringEditType.DeleteContentAfterMarker:
                case StringEditType.KeepContentBeforeMarker:
                case StringEditType.KeepContentAfterMarker:
                case StringEditType.DeleteContentBeforeLastMarker:
                case StringEditType.DeleteContentAfterLastMarker:
                case StringEditType.KeepContentBeforeLastMarker:
                case StringEditType.KeepContentAfterLastMarker:
                case StringEditType.ReplaceContent:
                case StringEditType.InsertContentBeforeMarker:
                case StringEditType.InsertContentAfterMarker:
                case StringEditType.InsertContentBeforeLastMarker:
                case StringEditType.InsertContentAfterLastMarker:
                case StringEditType.DeleteContentBetweenMarkers:
                case StringEditType.KeepContentBetweenMarkers:
                case StringEditType.KeepContentOutsideMarkers:
                case StringEditType.DeleteContentBetweenLastMarkers:
                case StringEditType.KeepContentBetweenLastMarkers:
                case StringEditType.KeepContentOutsideLastMarkers:
                case StringEditType.DeleteContentBetweenInnermostMarkers:
                case StringEditType.KeepContentBetweenInnermostMarkers:
                case StringEditType.KeepContentOutsideInnermostMarkers:
                case StringEditType.DeleteContentBetweenOutermostMarkers:
                case StringEditType.KeepContentBetweenOutermostMarkers:
                case StringEditType.KeepContentOutsideOutermostMarkers:
                case StringEditType.Set:
                case StringEditType.SetIfEquals:
                    {
                        // These operations take one or two string arguments.
                        //
                        string firstArgument = editArguments[0];
                        if (editArguments.Length > 1)
                        {
                            string secondArgument = editArguments[1];
                            stringArguments = new string[] { firstArgument, secondArgument };
                        }
                        else
                        {
                            stringArguments = new string[] { firstArgument };
                        }
                        break;
                    }

                case StringEditType.PadLeft:
                case StringEditType.PadRight:
                    // These operations take a string and an int argument, in this order.
                    //
                    stringArguments = new string[] { editArguments[0] };
                    intArguments = new int[] { int.Parse(editArguments[1]) };
                    break;

                case StringEditType.InsertContentAtIndex:
                    // These operations take an int and a string argument, int this order.
                    //
                    stringArguments = new string[] { editArguments[1] };
                    intArguments = new int[] { int.Parse(editArguments[0]) };
                    break;

                case StringEditType.DeleteFirstCharacters:
                case StringEditType.DeleteLastCharacters:
                case StringEditType.KeepFirstCharacters:
                case StringEditType.KeepLastCharacters:
                case StringEditType.DeleteContentAtIndex:
                case StringEditType.KeepContentAtIndex:
                    {
                        // These operations take one or two int arguments.
                        //
                        int firstArgument = int.Parse(editArguments[0]);
                        if (editArguments.Length > 1)
                        {
                            int secondArgument = int.Parse(editArguments[1]);
                            intArguments = new int[] { firstArgument, secondArgument };
                        }
                        else
                        {
                            intArguments = new int[] { firstArgument };
                        }
                        break;
                    }
            }
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
            var extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.EditColumnValues}.{columnNumber}.{dataTypeString.ToLower()}.{editTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, editArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            // Construct IDataHolder argument list.
            //
            IDataHolder[] dataHolderArguments = (editArguments.Length > 0) ? new IDataHolder[] { DataHolderOperations.BuildAndCheckDataHolder(dataType, editArguments[0]) } : null;

            var processingParameters = new OutputExtraOperationParameters<ValueEditType>(
                outputFilePath,
                editType,
                dataHolderArguments);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, EditColumnValueProcessor, OutputExtraOperationParameters<ValueEditType>>(
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

            string[] stringArguments = new string[] { lineValue };
            ulong[] ulongArguments = (insertionArguments.Length > 0) ? new ulong[] { ulong.Parse(insertionArguments[0]) } : null;

            var processingParameters = new OutputExtraOperationParameters<PositionInsertionType>(
                outputFilePath,
                insertionType,
                stringArguments,
                ulongArguments);

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
            var firstExtractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                firstFileColumnNumber,
                dataType);

            var secondExtractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                secondFileColumnNumber,
                dataType);

            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.JoinLinesPostSorting}.{firstFileColumnNumber}.{dataTypeString.ToLower()}.{joinTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.JoinLines}.{firstFileColumnNumber}.{dataTypeString.ToLower()}.{joinTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, joinArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new OutputExtraOperationParameters<JoinType>(
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

            string[] stringArguments = new string[] { columnSeparator };

            var processingParameters = new OutputExtraParameters(
                outputFilePath,
                stringArguments);

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
            var extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.TransformLines}.{transformationTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, transformationArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            int[] intArguments = (transformationArguments.Length > 0) ? new int[] { int.Parse(transformationArguments[0]) } : null;

            var processingParameters = new OutputExtraOperationParameters<LineTransformationType>(
                outputFilePath,
                transformationType,
                intArguments);

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
            var extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.TransformColumns}.{transformationTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, transformationArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            // The string parameter is always present and is the last one.
            //
            string[] stringParameters = new string[] { transformationArguments[transformationArguments.Length - 1] };

            // There are one or two int parameters.
            //
            int[] intParameters;
            if (transformationArguments.Length > 2)
            {
                intParameters = new int[] { int.Parse(transformationArguments[0]), int.Parse(transformationArguments[1]) };
            }
            else
            {
                intParameters = new int[] { int.Parse(transformationArguments[0]) };
            }

            var processingParameters = new OutputExtraOperationParameters<ColumnTransformationType>(
                outputFilePath,
                transformationType,
                stringParameters,
                intParameters);

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
            var extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}.{comparisonTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, comparisonArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            // There are one or two data holder parameters.
            //
            IDataHolder[] dataHolderParameters;
            IDataHolder firstArgument = DataHolderOperations.BuildAndCheckDataHolder(dataType, comparisonArguments[0]);
            if (comparisonArguments.Length > 1)
            {
                IDataHolder secondArgument = DataHolderOperations.BuildAndCheckDataHolder(dataType, comparisonArguments[1]);

                dataHolderParameters = new IDataHolder[] { firstArgument, secondArgument };
            }
            else
            {
                dataHolderParameters = new IDataHolder[] { firstArgument };
            }

            var processingParameters = new OutputExtraOperationParameters<ComparisonType>(
                outputFilePath,
                comparisonType,
                dataHolderParameters);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, SelectLineByColumnValueProcessor, OutputExtraOperationParameters<ComparisonType>>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SelectLinesByTwoColumnValues(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            DataType dataType, string dataTypeString,
            ComparisonType comparisonType, string comparisonTypeString,
            int secondColumnNumber,
            string outputFilePath)
        {
            var extractionParameters = new TwoColumnValuesExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType,
                secondColumnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByTwoColumnValues}.{columnNumber}.{dataTypeString.ToLower()}.{comparisonTypeString.ToLower()}.{secondColumnNumber}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new OutputOperationParameters<ComparisonType>(
                outputFilePath,
                comparisonType);

            var fileProcessor
                = new FileProcessor<TwoColumnValuesExtractor, TwoColumnValuesExtractionParameters, TwoExtractedValues, SelectLineByTwoColumnValuesProcessor, OutputOperationParameters<ComparisonType>>(
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

            // There are one or two int parameters.
            //
            ulong[] ulongParameters;
            if (selectionArguments.Length > 1)
            {
                ulongParameters = new ulong[] { ulong.Parse(selectionArguments[0]), ulong.Parse(selectionArguments[1]) };
            }
            else
            {
                ulongParameters = new ulong[] { ulong.Parse(selectionArguments[0]) };
            }

            var processingParameters = new OutputExtraOperationParameters<PositionSelectionType>(
                outputFilePath,
                selectionType,
                ulongParameters);

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
            var extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectColumnsByNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, selectionArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            // There are one or two int parameters.
            //
            int[] intParameters;
            if (selectionArguments.Length > 1)
            {
                intParameters = new int[] { int.Parse(selectionArguments[0]), int.Parse(selectionArguments[1]) };
            }
            else
            {
                intParameters = new int[] { int.Parse(selectionArguments[0]) };
            }

            var processingParameters = new OutputExtraOperationParameters<PositionSelectionType>(
                outputFilePath,
                selectionType,
                intParameters);

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

            int[] intArguments = null;
            if (selectionType == StringSelectionType.HasLengthBetween 
                || selectionType == StringSelectionType.HasLengthNotBetween)
            {
                intArguments = new int[] { int.Parse(selectionArguments[0]), int.Parse(selectionArguments[1]) };
                selectionArguments = null;
            }

            var processingParameters = new OutputExtraOperationParameters<StringSelectionType>(
                outputFilePath,
                selectionType,
                selectionArguments,
                intArguments);

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
            var extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnString}.{columnNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, selectionArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new OutputExtraOperationParameters<StringSelectionType>(
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

        private static void SelectLinesByTwoColumnStrings(
            string inputFilePath,
            int columnNumber,
            string columnSeparator,
            StringSelectionType selectionType, string selectionTypeString,
            int secondColumnNumber,
            string outputFilePath)
        {
            var extractionParameters = new TwoColumnValuesExtractionParameters(
                columnSeparator,
                columnNumber,
                DataType.String,
                secondColumnNumber,
                DataType.String);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnString}.{columnNumber}.{selectionTypeString.ToLower()}.{secondColumnNumber}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new OutputOperationParameters<StringSelectionType>(
                outputFilePath,
                selectionType);

            var fileProcessor
                = new FileProcessor<TwoColumnValuesExtractor, TwoColumnValuesExtractionParameters, TwoExtractedValues, SelectLineByTwoColumnStringsProcessor, OutputOperationParameters<StringSelectionType>>(
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
            var extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByColumnCount}.{comparisonTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, comparisonArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            // There are one or two data holder parameters.
            //
            IDataHolder[] dataHolderParameters;
            IDataHolder firstArgument = DataHolderOperations.BuildAndCheckDataHolder(DataType.Integer, comparisonArguments[0]);
            if (comparisonArguments.Length > 1)
            {
                IDataHolder secondArgument = DataHolderOperations.BuildAndCheckDataHolder(DataType.Integer, comparisonArguments[1]);

                dataHolderParameters = new IDataHolder[] { firstArgument, secondArgument };
            }
            else
            {
                dataHolderParameters = new IDataHolder[] { firstArgument };
            }

            var processingParameters = new OutputExtraOperationParameters<ComparisonType>(
                outputFilePath,
                comparisonType,
                dataHolderParameters);

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

            var processingParameters = new OutputOperationParameters<RelativeValueSelectionType>(
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
            var extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueRelativeToOtherLines}.{columnNumber}.{dataTypeString.ToLower()}.{selectionTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.SelectLinesByColumnValueRelativeToOtherLines}.{columnNumber}.{dataTypeString.ToLower()}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new OutputOperationParameters<RelativeValueSelectionType>(
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

            var processingParameters = new OutputOperationParameters<LookupType>(
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
            var dataFileExtractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                dataFileColumnNumber,
                dataType);

            var lookupFileExtractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                lookupFileColumnNumber,
                dataType);

            string outputFileExtension
                = isSorted
                ? $".{CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueLookupInFile}.{dataFileColumnNumber}.{dataTypeString.ToLower()}.{lookupTypeString.ToLower()}"
                : $".{CabeiroConstants.Commands.SelectLinesByColumnValueLookupInFile}.{dataFileColumnNumber}.{dataTypeString.ToLower()}.{lookupTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(dataFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new OutputOperationParameters<LookupType>(
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
            int seedValue,
            string inputFilePath,
            int sampleSize,
            string outputFilePath)
        {
            string outputFileExtension
                = (seedValue >= 0)
                ? $".{CabeiroConstants.Commands.SelectLinesSample}.{seedValue}.{sampleSize}"
                : $".{CabeiroConstants.Commands.SelectLinesSample}.{sampleSize}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            int[] intParameters = new int[] { seedValue, sampleSize };

            var processingParameters = new OutputExtraParameters(
                outputFilePath,
                intParameters);

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
            string[] stringParameters = new string[] { CabeiroConstants.Files.Extensions.Txt };
            ulong[] ulongParameters = new ulong[] { rangeSize };

            var processingParameters = new OutputExtraParameters(
                outputFilePath,
                stringParameters,
                ulongParameters);

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
            var extractionParameters = new ColumnStringsExtractionParameters(
                columnSeparator);

            // The file name will be bundled by the processor with the number of each column, so we exclude the text extension from it;
            // it will get appended by the processor internally.
            //
            string outputFileExtension = $".{CabeiroConstants.Commands.SplitColumns}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath(excludeTextExtension: true);

            // The file extension is needed because the processor will need to append it for each file it creates.
            //
            string[] stringParameters = new string[] { CabeiroConstants.Files.Extensions.Txt };

            var processingParameters = new OutputExtraParameters(
                outputFilePath,
                stringParameters);

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
            var extractionParameters = new OneColumnValueExtractionParameters(
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
            string[] stringParameters = new string[] { CabeiroConstants.Files.Extensions.Txt };

            var processingParameters = new OutputExtraParameters(
                outputFilePath,
                stringParameters);

            var fileProcessor
                = new FileProcessor<OneColumnValueExtractor, OneColumnValueExtractionParameters, OneExtractedValue, SplitColumnValuesProcessor, OutputExtraParameters>(
                    inputFilePath,
                    extractionParameters,
                    processingParameters);

            fileProcessor.ProcessFile();
        }

        private static void SplitLinesIntoRandomSets(
            int seedValue,
            string inputFilePath,
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
            string[] stringParameters = new string[] { CabeiroConstants.Files.Extensions.Txt };
            int[] intParameters = new int[] { seedValue, setsCount };

            var processingParameters = new OutputExtraParameters(
                outputFilePath,
                stringParameters,
                intParameters);

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
            var extractionParameters = new TwoColumnValuesExtractionParameters(
                columnSeparator,
                firstColumnNumber,
                firstDataType,
                secondColumnNumber,
                secondDataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.SortBySecondColumnValue}.{secondColumnNumber}.{secondDataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new BaseOutputParameters(
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

            var processingParameters = new BaseOutputParameters(
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
            var extractionParameters = new OneColumnValueExtractionParameters(
                columnSeparator,
                columnNumber,
                dataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.MergeLinesByColumnValue}.{columnNumber}.{dataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(firstFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new BaseOutputParameters(
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
            var extractionParameters = new TwoColumnValuesExtractionParameters(
                columnSeparator,
                firstColumnNumber,
                firstDataType,
                secondColumnNumber,
                secondDataType);

            string outputFileExtension = $".{CabeiroConstants.Commands.FindStateTransitions}.{secondColumnNumber}.{secondDataTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(inputFilePath, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new BaseOutputParameters(
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
            var generationParameters = new DistributionGenerationParameters(
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

            var processingParameters = new BaseOutputParameters(
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
            double[] progressionArguments = (generationArguments.Length == 2) ? new double[] { double.Parse(generationArguments[0]), double.Parse(generationArguments[1]) } : null;

            var generationParameters = new ProgressionGenerationParameters(
                generationCount,
                progressionType,
                progressionArguments);

            string outputFileExtension = $".{CabeiroConstants.Commands.GenerateProgression}.{progressionTypeString.ToLower()}.{generationCount}";
            var filePathBuilder = new FilePathBuilder(CabeiroConstants.Program.Name, outputFileExtension, generationArguments, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new BaseOutputParameters(
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
            var generationParameters = new SampleGenerationParameters(
                seedValue,
                generationCount,
                totalCount);

            string outputFileExtension
                = (seedValue >= 0)
                ? $".{CabeiroConstants.Commands.GenerateSample}.{seedValue}.{totalCount}.{generationCount}"
                : $".{CabeiroConstants.Commands.GenerateSample}.{totalCount}.{generationCount}";
            var filePathBuilder = new FilePathBuilder(CabeiroConstants.Program.Name, outputFileExtension, /*operationArguments:*/ null, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            var processingParameters = new BaseOutputParameters(
                outputFilePath);

            var fileGenerationProcessor
                = new FileGenerationProcessor<SampleGenerator, SampleGenerationParameters>(
                    generationParameters,
                    processingParameters);

            fileGenerationProcessor.GenerateFile();
        }
    }
}
