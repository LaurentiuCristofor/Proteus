////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Cabeiro.Common;

using CabeiroConstants = LaurentiuCristofor.Cabeiro.Common.Constants;

namespace LaurentiuCristofor.Cabeiro.OnlineHelp
{
    public abstract class CommandRegistry
    {
        // The list of command categories. This controls the order in which categories are displayed.
        //
        private static List<CommandCategory> categories;

        // A map for converting category names to category enum values. It is case-insensitive.
        //
        private static Dictionary<string, CommandCategory> mapCategoryStringToEnum;

        // A map for retrieving command descriptions based on their name.
        //
        private static Dictionary<string, CommandDescription> mapCommandNameToDescription;

        // A map for retrieving a list of command descriptions based on a command category enum value.
        //
        private static Dictionary<CommandCategory, List<CommandDescription>> mapCategoryToDescriptionList;

        public static void Initialize()
        {
            // Initialize our data structures.
            //
            categories = new List<CommandCategory>();
            mapCategoryStringToEnum = new Dictionary<string, CommandCategory>();
            mapCommandNameToDescription = new Dictionary<string, CommandDescription>();
            mapCategoryToDescriptionList = new Dictionary<CommandCategory, List<CommandDescription>>();

            CommandDescription commandDescription;

            // Command descriptions are hard-coded below.
            //
            // Help commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.Help,
                CommandCategory.Help,
                $"Obtain help on {CabeiroConstants.Program.Name} functionality.",
                /*longDescription:*/ null,
                $"[{CabeiroConstants.Commands.Arguments.Descriptions.CommandName}"
                + $" | {CabeiroConstants.Commands.Arguments.All}"
                + $" | {CabeiroConstants.Commands.Arguments.Categories}"
                + $" | {CabeiroConstants.Commands.Arguments.Category} {CabeiroConstants.Commands.Arguments.Descriptions.CategoryName}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.HelpCategoriesText}");
            RegisterCommandDescription(commandDescription);

            // Information commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.CountLines,
                CommandCategory.Information,
                "(C)ount lines",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}",
                $"{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.AnalyzeLines,
                CommandCategory.Information,
                "(A)nalyze (L)ines",
                CabeiroConstants.Commands.Descriptions.AnalyzeLines,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LimitValues}",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.LimitValuesText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.ConsoleOutput}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearUnique}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.AnalyzeColumnValues,
                CommandCategory.Information,
                "(A)nalyze (C)olumn (V)alues",
                CabeiroConstants.Commands.Descriptions.AnalyzeColumnValues,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LimitValues}",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.LimitValuesText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.ConsoleOutput}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearUnique}");
            RegisterCommandDescription(commandDescription);

            // Ordering commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.Invert,
                CommandCategory.Ordering,
                "(I)nvert file",
                CabeiroConstants.Commands.Descriptions.Invert,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Notes.MemoryRequirementLinearTotal}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.Sort,
                CommandCategory.Ordering,
                "(S)ort file",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Notes.MemoryRequirementLinearTotal}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SortByColumnValue,
                CommandCategory.Ordering,
                "(S)ort file (B)y (C)olumn (V)alue",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearTotal}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.CustomSort,
                CommandCategory.Ordering,
                "(C)ustom (S)ort file",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.AlgorithmType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.SortingAlgorithmTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearTotal}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.CustomSortByColumnValue,
                CommandCategory.Ordering,
                "(C)ustom (S)ort file (B)y (C)olumn (V)alue",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.AlgorithmType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.SortingAlgorithmTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearTotal}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.Shuffle,
                CommandCategory.Ordering,
                "(SH)uffle file",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Notes.MemoryRequirementLinearTotal}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.OrderColumns,
                CommandCategory.Ordering,
                "(O)rder (C)olumns",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.NewFirstColumnsList}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.NewFirstColumnsListText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            // Editing commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.EditLines,
                CommandCategory.Editing,
                "(E)dit (L)ines",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.EditType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.StringEditTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.EditColumnStrings,
                CommandCategory.Editing,
                "(E)dit (C)olumn (S)trings",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.EditType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.StringEditTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.EditColumnValues,
                CommandCategory.Editing,
                "(E)dit (C)olumn (V)alues",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.EditType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.ValueEditTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.InsertLine,
                CommandCategory.Editing,
                "(I)nsert (L)ine",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LineValue}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.InsertionType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.PositionInsertionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.JoinLines,
                CommandCategory.Editing,
                "(J)oin (L)ines",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.FirstInputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstFileColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SecondInputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SecondFileColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.JoinType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.JoinTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearTotalSecond}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.ConcatenateLines,
                CommandCategory.Editing,
                "(C)oncatenate (L)ines",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.FirstInputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SecondInputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.TransformLines,
                CommandCategory.Editing,
                "(T)ransform (L)ines",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.TransformationType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.LineTransformationTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.LineTransformationTypeUniteText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.TransformColumns,
                CommandCategory.Editing,
                "(T)ransform (C)olumns",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.TransformationType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} {CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.ThirdArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.ColumnTransformationTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            // Line selection commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByColumnValue,
                CommandCategory.Selection,
                "(S)elect (L)ines (B)y (C)olumn (V)alue",
                CabeiroConstants.Commands.Descriptions.SelectLinesByColumnValue,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ComparisonType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.ComparisonTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByNumber,
                CommandCategory.Selection,
                "(S)elect (L)ines (B)y (N)umber",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.PositionSelectionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}"
                + $" {CabeiroConstants.Commands.Notes.MemoryRequirementExceptionLast}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectColumnsByNumber,
                CommandCategory.Selection,
                "(S)elect (C)olumns (B)y (N)umber",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.PositionSelectionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByLineString,
                CommandCategory.Selection,
                "(S)elect (L)ines (B)y (L)ine (S)tring",
                CabeiroConstants.Commands.Descriptions.SelectLinesByLineString,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.StringSelectionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByColumnString,
                CommandCategory.Selection,
                "(S)elect (L)ines (B)y (C)olumn (S)tring",
                CabeiroConstants.Commands.Descriptions.SelectLinesByColumnString,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.StringSelectionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByColumnCount,
                CommandCategory.Selection,
                "(S)elect (L)ines (B)y (C)olumn (C)ount",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ComparisonType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.ComparisonTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByLineStringRelativeToOtherLines,
                CommandCategory.Selection,
                "(S)elect (L)ines (B)y (L)ine (S)tring (R)elative to other lines",
                CabeiroConstants.Commands.Descriptions.SelectLinesByLineStringRelativeToOtherLines,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.RelativeLineSelectionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearUnique}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByColumnValueRelativeToOtherLines,
                CommandCategory.Selection,
                "(S)elect (L)ines (B)y (C)olumn (V)alue (R)elative to other lines",
                CabeiroConstants.Commands.Descriptions.SelectLinesByColumnValueRelativeToOtherLines,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.RelativeValueSelectionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearUnique}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByLookupInFile,
                CommandCategory.Selection,
                "(S)elect (L)ines (B)y (L)ookup (I)n (F)ile",
                CabeiroConstants.Commands.Descriptions.SelectLinesByLookupInFile,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.LookupTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearUniqueLookup}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByColumnValueLookupInFile,
                CommandCategory.Selection,
                "(S)elect (L)ines (B)y (C)olumn (V)alue (L)ookup (I)n (F)ile",
                CabeiroConstants.Commands.Descriptions.SelectLinesByColumnValueLookupInFile,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataFileColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupFileColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.LookupTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearUniqueLookup}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesSample,
                CommandCategory.Selection,
                "(S)elect (L)ines (S)ample",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SampleSize}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Notes.MemoryRequirementLinearSample}");
            RegisterCommandDescription(commandDescription);

            // Splitting commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SplitLineRanges,
                CommandCategory.Splitting,
                "(SP)lit (L)ine (R)anges",
                CabeiroConstants.Commands.Descriptions.SplitLineRanges,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.RangeSize}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SplitColumns,
                CommandCategory.Splitting,
                "(SP)lit (C)olumns",
                CabeiroConstants.Commands.Descriptions.SplitColumns,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Notes.MemoryRequirementConstantColumns}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SplitColumnValues,
                CommandCategory.Splitting,
                "(SP)lit (C)olumn (V)alues",
                CabeiroConstants.Commands.Descriptions.SplitColumnValues,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearUnique}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SplitLinesIntoRandomSets,
                CommandCategory.Splitting,
                "(SP)lit (L)ines (I)nto (R)andom (S)ets",
                CabeiroConstants.Commands.Descriptions.SplitLinesIntoRandomSets,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SeedValue}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SetsCount}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.SeedValueText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearSets}");
            RegisterCommandDescription(commandDescription);

            // PostSorting commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SortBySecondColumnValue,
                CommandCategory.Post_Sorting,
                "(S)ort file (B)y (2)nd (C)olumn (V)alue",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SortedColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SortedColumnDataType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearPrimaryColumnRepetitions}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.MergeLines,
                CommandCategory.Post_Sorting,
                "(M)erge (L)ines",
                CabeiroConstants.Commands.Descriptions.MergeLines,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.FirstInputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SecondInputFilePath}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.MergeLinesByColumnValue,
                CommandCategory.Post_Sorting,
                "(M)erge (L)ines (B)y (C)olumn (V)alue",
                CabeiroConstants.Commands.Descriptions.MergeLinesByColumnValue,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.FirstInputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SecondInputFilePath}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesPostSortingByLineStringRelativeToOtherLines,
                CommandCategory.Post_Sorting,
                "(S)elect (L)ines (P)ost (S)orting (B)y (L)ine (S)tring (R)elative to other lines",
                CabeiroConstants.Commands.Descriptions.SelectLinesPostSortingByLineStringRelativeToOtherLines,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.RelativeLineSelectionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueRelativeToOtherLines,
                CommandCategory.Post_Sorting,
                "(S)elect (L)ines (P)ost (S)orting (B)y (C)olumn (V)alue (R)elative to other lines",
                CabeiroConstants.Commands.Descriptions.SelectLinesPostSortingByColumnValueRelativeToOtherLines,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.RelativeValueSelectionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesPostSortingByLookupInFile,
                CommandCategory.Post_Sorting,
                "(S)elect (L)ines (P)ost (S)orting (B)y (L)ookup (I)n (F)ile",
                CabeiroConstants.Commands.Descriptions.SelectLinesPostSortingByLookupInFile,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.LookupTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesPostSortingByColumnValueLookupInFile,
                CommandCategory.Post_Sorting,
                "(S)elect (L)ines (P)ost (S)orting (B)y (C)olumn (V)alue (L)ookup (I)n (F)ile",
                CabeiroConstants.Commands.Descriptions.SelectLinesPostSortingByColumnValueLookupInFile,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataFileColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupFileColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LookupType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.LookupTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.JoinLinesPostSorting,
                CommandCategory.Post_Sorting,
                "(J)oin (L)ines (P)ost (S)orting",
                /*longDescription:*/ null,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.FirstInputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstFileColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SecondInputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SecondFileColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.JoinType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.JoinTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementLinearJoinKeyRepetitions}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.FindStateTransitions,
                CommandCategory.Post_Sorting,
                "(F)ind (S)tate (T)ransitions",
                CabeiroConstants.Commands.Descriptions.FindStateTransitions,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SortedColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SortedColumnDataType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstantLine}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.GenerateDistribution,
                CommandCategory.Generation,
                "(GEN)erate (D)istribution",
                CabeiroConstants.Commands.Descriptions.GenerateDistribution,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.SeedValue}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.GenerationCount}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DistributionType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.DistributionMean}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.SeedValueText}"
                + $"\n\n{CabeiroConstants.Commands.Arguments.Descriptions.DistributionTypeText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstant}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.GenerateSample,
                CommandCategory.Generation,
                "(GEN)erate (S)ample",
                CabeiroConstants.Commands.Descriptions.GenerateSample,
                $"{CabeiroConstants.Commands.Arguments.Descriptions.SeedValue}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.GenerationCount}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.TotalCount}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.SeedValueText}"
                + $"\n\n{CabeiroConstants.Commands.Notes.MemoryRequirementConstant}");
            RegisterCommandDescription(commandDescription);
        }

        /// <summary>
        /// Displays the description of a command.
        /// </summary>
        /// <param name="commandName">The command whose description we want displayed.</param>
        public static void DisplayCommandDescription(string commandName)
        {
            CommandDescription commandDescription;

            // Lookup command name.
            //
            if (!mapCommandNameToDescription.TryGetValue(commandName.ToLower(), out commandDescription))
            {
                Console.WriteLine($"{CabeiroConstants.Program.Name} does not have a command named '{commandName}'");
                return;
            }

            commandDescription.Display();
        }

        /// <summary>
        /// Displays a list of commands in a category.
        /// </summary>
        /// <param name="category">The category name whose commands we want displayed.</param>
        public static void DisplayCategoryCommands(string category)
        {
            CommandCategory commandCategory;

            // Convert category string into category enum value.
            //
            if (!mapCategoryStringToEnum.TryGetValue(category.ToLower(), out commandCategory))
            {
                Console.WriteLine($"{CabeiroConstants.Program.Name} does not have a command category named '{category}'");
                return;
            }

            DisplayCategoryCommands(commandCategory);
        }

        /// <summary>
        /// Displays a list of commands in a category.
        /// </summary>
        /// <param name="commandCategory">The category enum value whose commands we want displayed.</param>
        private static void DisplayCategoryCommands(CommandCategory commandCategory)
        {
            List<CommandDescription> commandDescriptions;

            // Lookup command category.
            //
            if (!mapCategoryToDescriptionList.TryGetValue(commandCategory, out commandDescriptions))
            {
                throw new CabeiroException($"Internal error: The category '{commandCategory.ToString().ToUpper()}' has been defined without any commands!");
            }

            Console.WriteLine($"{commandCategory.ToString().ToUpper()} commands:\n");

            foreach (CommandDescription commandDescription in commandDescriptions)
            {
                Console.WriteLine($"\t{commandDescription.Command.ToUpper()} - {commandDescription.ShortDescription}");
            }
        }

        /// <summary>
        /// Displays a list of all available command categories.
        /// </summary>
        public static void DisplayAllCommandCategories()
        {
            if (categories.Count == 0)
            {
                throw new CabeiroException($"Internal error: No command categories have been registered!");
            }

            Console.WriteLine($"{CabeiroConstants.Program.Name} command categories are:\n");

            foreach (CommandCategory commandCategory in categories)
            {
                Console.WriteLine($"\t{commandCategory}");
            }
        }

        /// <summary>
        /// Displays all commands in all categories.
        /// </summary>
        public static void DisplayAllCommands()
        {
            DisplayProgramDescription(displayGetStartedTip: false);

            if (categories.Count == 0)
            {
                throw new CabeiroException($"Internal error: No command categories have been registered!");
            }

            Console.WriteLine($"Full list of {CabeiroConstants.Program.Name} commands:");

            foreach (CommandCategory commandCategory in categories)
            {
                Console.WriteLine();
                DisplayCategoryCommands(commandCategory);
            }
        }

        /// <summary>
        /// Displays the version of both Cabeiro and Proteus.
        /// </summary>
        public static void DisplayProgramVersion()
        {
            AssemblyName proteusInfo = ProteusInfo.GetAssemblyInfo();
            AssemblyName cabeiroInfo = CabeiroInfo.GetAssemblyInfo();

            Console.WriteLine($"{cabeiroInfo.Name} program version: {cabeiroInfo.Version.Major}.{cabeiroInfo.Version.Minor}");
            Console.WriteLine($"{proteusInfo.Name} library version: {proteusInfo.Version.Major}.{proteusInfo.Version.Minor}");
        }

        /// <summary>
        /// Displays a description of Cabeiro functionality.
        /// </summary>
        /// <param name="displayGetStartedTip">Whether to display the GetStarted tip or not.</param>
        public static void DisplayProgramDescription(bool displayGetStartedTip = true)
        {
            DisplayProgramVersion();

            Console.WriteLine(CabeiroConstants.Program.Description);

            if (displayGetStartedTip)
            {
                Console.WriteLine(CabeiroConstants.Program.GetStartedTip);
            }
        }

        /// <summary>
        /// Registers a command description and update all our command registry data structures.
        /// </summary>
        /// <param name="commandDescription">The command description instance to register.</param>
        private static void RegisterCommandDescription(CommandDescription commandDescription)
        {
            CommandCategory commandCategory = commandDescription.Category;

            // First, check for a command name conflict.
            //
            if (mapCommandNameToDescription.ContainsKey(commandDescription.Command.ToLower()))
            {
                throw new CabeiroException($"Internal error: Command: {commandDescription.Command} has already been registered!");
            }

            // If we do not already have an entry for the command's category, create one now.
            //
            if (!mapCategoryToDescriptionList.ContainsKey(commandCategory))
            {
                mapCategoryToDescriptionList.Add(commandCategory, new List<CommandDescription>());
            }

            // Get the category's list of commands to update.
            //
            List<CommandDescription> categoryCommands = mapCategoryToDescriptionList[commandCategory];

            // Add command description to our data structures.
            //
            mapCommandNameToDescription.Add(commandDescription.Command.ToLower(), commandDescription);
            categoryCommands.Add(commandDescription);

            // If the command category was not registered already, register it now.
            //
            if (!mapCategoryStringToEnum.ContainsKey(commandCategory.ToString().ToLower()))
            {
                // Add command category to our data structures.
                //
                categories.Add(commandCategory);
                mapCategoryStringToEnum.Add(commandCategory.ToString().ToLower(), commandCategory);
            }
        }
    }
}
