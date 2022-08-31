﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors;
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
            }
        }

        /// <summary>
        /// Checks that Cabeiro is used with the correct Proteus version.
        /// </summary>
        private static void ValidateProteusVersion()
        {
            AssemblyName proteusInfo = ProteusInfo.GetAssemblyInfo();
            AssemblyName cabeiroInfo = CabeiroInfo.GetAssemblyInfo();

            if (proteusInfo.Version.Major != cabeiroInfo.Version.Major)
            {
                CommandRegistry.DisplayProgramVersion();
                throw new CabeiroException("The major version of the Proteus library does not match the major version of Cabeiro!");
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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.EditLines))
            {
                const int minimumArgumentNumber = 3;
                const int maximumArgumentNumber = 6;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    Tuple<StringEditType, int> operationInfo = ArgumentParser.ParseStringEditType(arguments[2]);
                    string firstArgument;
                    string secondArgument;
                    string outputFilePath;
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 3, arguments, out firstArgument, out secondArgument, out outputFilePath);

                    EditLines(
                        arguments[1],
                        operationInfo.Item1, arguments[2],
                        firstArgument,
                        secondArgument,
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
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    Tuple<StringEditType, int> operationInfo = ArgumentParser.ParseStringEditType(arguments[4]);
                    string firstArgument;
                    string secondArgument;
                    string outputFilePath;
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 5, arguments, out firstArgument, out secondArgument, out outputFilePath);

                    EditColumnValues(
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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesHavingColumnValue))
            {
                const int minimumArgumentNumber = 7;
                const int maximumArgumentNumber = 9;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    int columnNumber = ArgumentParser.GetPositiveInteger(arguments[2]);
                    string columnSeparator = ArgumentParser.ParseSeparator(arguments[3]);
                    DataType dataType = ArgumentParser.ParseDataType(arguments[4]);
                    Tuple<ComparisonType, int> operationInfo = ArgumentParser.ParseComparisonType(arguments[5]);
                    string firstArgument;
                    string secondArgument;
                    string outputFilePath;
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 6, arguments, out firstArgument, out secondArgument, out outputFilePath);

                    SelectLinesHavingColumnValue(
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
            else if (ArgumentParser.IsCommand(arguments[0], CabeiroConstants.Commands.SelectLinesByLineNumber))
            {
                const int minimumArgumentNumber = 4;
                const int maximumArgumentNumber = 6;
                if (ArgumentParser.HasExpectedArgumentNumber(arguments.Length, minimumArgumentNumber, maximumArgumentNumber))
                {
                    Tuple<LineNumberSelectionType, int> operationInfo = ArgumentParser.ParseLineNumberSelectionType(arguments[2]);
                    string firstArgument;
                    string secondArgument;
                    string outputFilePath;
                    ArgumentParser.ExtractLastArguments(operationInfo.Item2, 3, arguments, out firstArgument, out secondArgument, out outputFilePath);

                    SelectLinesByLineNumber(
                        arguments[1],
                        operationInfo.Item1, arguments[2],
                        firstArgument,
                        secondArgument,
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
            var textFileProcessor
                = new TextFileProcessor<LineExtractor, UnusedType, string, SinkProcessor, UnusedType>(
                    filePath,
                    extractionParameters: null,
                    processingParameters: null);

            textFileProcessor.ProcessFile();
        }

        private static void AnalyzeLines(string filePath, int valuesLimit)
        {
            AnalyzeParameters processingParameters = new AnalyzeParameters(valuesLimit);

            var textFileProcessor
                = new TextFileProcessor<LineAsStringPartsExtractor, UnusedType, StringParts, AnalyzeProcessor, AnalyzeParameters>(
                    filePath,
                    extractionParameters: null,
                    processingParameters: processingParameters);

            textFileProcessor.ProcessFile();
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

            var textFileProcessor
                = new TextFileProcessor<ColumnExtractor, ColumnExtractionParameters, StringParts, AnalyzeProcessor, AnalyzeParameters>(
                    filePath,
                    extractionParameters: extractionParameters,
                    processingParameters: processingParameters);

            textFileProcessor.ProcessFile();
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

            OperationTypeParameters<StringEditType> processingParameters = new OperationTypeParameters<StringEditType>(
                outputFilePath,
                editType,
                firstArgument,
                secondArgument);

            var textFileProcessor
                = new TextFileProcessor<LineAsStringPartsExtractor, UnusedType, StringParts, EditProcessor, OperationTypeParameters<StringEditType>>(
                    filePath,
                    extractionParameters: null,
                    processingParameters);

            textFileProcessor.ProcessFile();
        }

        private static void EditColumnValues(
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

            string outputFileExtension = $".{CabeiroConstants.Commands.EditColumnValues}.{columnNumber}.{editTypeString.ToLower()}";
            var filePathBuilder = new EditOperationFilePathBuilder(editType, filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeParameters<StringEditType> processingParameters = new OperationTypeParameters<StringEditType>(
                outputFilePath,
                editType,
                firstArgument,
                secondArgument);

            var textFileProcessor
                = new TextFileProcessor<ColumnExtractor, ColumnExtractionParameters, StringParts, EditProcessor, OperationTypeParameters<StringEditType>>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            textFileProcessor.ProcessFile();
        }

        private static void SelectLinesHavingColumnValue(
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

            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesHavingColumnValue}.{columnNumber}.{dataTypeString.ToLower()}.{comparisonTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeParameters<ComparisonType> processingParameters = new OperationTypeParameters<ComparisonType>(
                outputFilePath,
                comparisonType,
                firstArgument,
                secondArgument);

            var textFileProcessor
                = new TextFileProcessor<ColumnExtractor, ColumnExtractionParameters, StringParts, ColumnValueSelectProcessor, OperationTypeParameters<ComparisonType>>(
                    filePath,
                    extractionParameters,
                    processingParameters);

            textFileProcessor.ProcessFile();
        }

        private static void SelectLinesByLineNumber(
            string filePath,
            LineNumberSelectionType selectionType, string selectionTypeString,
            string firstArgument,
            string secondArgument,
            string outputFilePath)
        {
            string outputFileExtension = $".{CabeiroConstants.Commands.SelectLinesByLineNumber}.{selectionTypeString.ToLower()}";
            var filePathBuilder = new FilePathBuilder(filePath, outputFileExtension, firstArgument, secondArgument, outputFilePath);
            outputFilePath = filePathBuilder.BuildOutputFilePath();

            OperationTypeParameters<LineNumberSelectionType> processingParameters = new OperationTypeParameters<LineNumberSelectionType>(
                outputFilePath,
                selectionType,
                firstArgument,
                secondArgument);

            var textFileProcessor
                = new TextFileProcessor<LineExtractor, UnusedType, string, LineNumberSelectProcessor, OperationTypeParameters<LineNumberSelectionType>>(
                    filePath,
                    extractionParameters: null,
                    processingParameters);

            textFileProcessor.ProcessFile();
        }
    }
}
