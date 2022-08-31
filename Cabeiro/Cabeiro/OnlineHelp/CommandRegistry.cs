////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
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
                $"[{CabeiroConstants.Commands.Arguments.Descriptions.CommandName}"
                + $" | {CabeiroConstants.Commands.Arguments.All}"
                + $" | {CabeiroConstants.Commands.Arguments.Categories}"
                + $" | {CabeiroConstants.Commands.Arguments.Category} {CabeiroConstants.Commands.Arguments.Descriptions.CategoryName}]",
                $"\t- using the {CabeiroConstants.Commands.Arguments.Descriptions.CommandName} option will produce detailed information on the specified command."
                + $"\n\t- '{CabeiroConstants.Commands.Arguments.All}' - will list all available commands."
                + $"\n\t- '{CabeiroConstants.Commands.Arguments.Categories}' - will list all command categories."
                + $"\n\t- '{CabeiroConstants.Commands.Arguments.Category}' - will list all commands in the specified category.");
            RegisterCommandDescription(commandDescription);

            // Information commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.CountLines,
                CommandCategory.Information,
                $"(C)ount lines",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}",
                null);
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.AnalyzeLines,
                CommandCategory.Information,
                $"(A)nalyze (L)ines",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LimitValues}",
                "where:"
                + $"{CabeiroConstants.Commands.Arguments.Descriptions.LimitValuesText}"
                + $"{CabeiroConstants.Commands.Notes.ConsoleOutput}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.AnalyzeColumnValues,
                CommandCategory.Information,
                $"(A)nalyze (C)olumn (V)alues",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.LimitValues}",
                "where:"
                + $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"{CabeiroConstants.Commands.Arguments.Descriptions.LimitValuesText}"
                + $"{CabeiroConstants.Commands.Notes.ConsoleOutput}");
            RegisterCommandDescription(commandDescription);

            // Editing commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.EditLines,
                CommandCategory.Editing,
                $"(E)dit (L)ines",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.EditType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                "where:"
                + $"{CabeiroConstants.Commands.Arguments.Descriptions.EditTypeText}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.EditColumnValues,
                CommandCategory.Editing,
                $"(E)dit (C)olumn (V)alues",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.EditType}"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                "where:"
                + $"{CabeiroConstants.Commands.Arguments.Descriptions.EditTypeText}");
            RegisterCommandDescription(commandDescription);

            // Line selection commands.
            //
            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesHavingColumnValue,
                CommandCategory.Selection,
                $"(S)elect (L)ines (H)aving (C)olumn (V)alue",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnNumber}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.DataType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ComparisonType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                "where:"
                + $"{CabeiroConstants.Commands.Arguments.Descriptions.DataTypeText}"
                + $"{CabeiroConstants.Commands.Arguments.Descriptions.ComparisonTypeText}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectLinesByLineNumber,
                CommandCategory.Selection,
                $"(S)elect (L)ines (B)y (L)ine (N)umber",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                "where:"
                + $"{CabeiroConstants.Commands.Arguments.Descriptions.NumberSelectionTypeText}");
            RegisterCommandDescription(commandDescription);

            commandDescription = new CommandDescription(
                CabeiroConstants.Commands.SelectColumnsByColumnNumber,
                CommandCategory.Selection,
                $"(S)elect (C)olumns (B)y (C)olumn (N)umber",
                $"{CabeiroConstants.Commands.Arguments.Descriptions.InputFilePath}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.ColumnSeparator}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.SelectionType}"
                + $" {CabeiroConstants.Commands.Arguments.Descriptions.FirstArgument} [{CabeiroConstants.Commands.Arguments.Descriptions.SecondArgument}]"
                + $" [{CabeiroConstants.Commands.Arguments.Descriptions.OutputFilePath}]",
                "where:"
                + $"{CabeiroConstants.Commands.Arguments.Descriptions.NumberSelectionTypeText}");
            RegisterCommandDescription(commandDescription);
        }

        /// <summary>
        /// Register a command description and update all our command registry data structures.
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

        /// <summary>
        /// Display the description of a command.
        /// </summary>
        /// <param name="commandName">The command whose description we want displayed.</param>
        public static void DisplayCommandDescription(string commandName)
        {
            CommandDescription commandDescription;

            // Lookup command name.
            //
            if (!mapCommandNameToDescription.TryGetValue(commandName.ToLower(), out commandDescription))
            {
                Console.WriteLine($"{CabeiroConstants.Program.Name} does not include a command named '{commandName}'");
                return;
            }

            commandDescription.Display();
        }

        /// <summary>
        /// Display a list of commands in a category.
        /// </summary>
        /// <param name="category">The category name whose commands we want displayed.</param>
        public static void DisplayCategoryCommands(string category)
        {
            CommandCategory commandCategory;

            // Convert category string into category enum value.
            //
            if (!mapCategoryStringToEnum.TryGetValue(category.ToLower(), out commandCategory))
            {
                Console.WriteLine($"{CabeiroConstants.Program.Name} does not include a command category named '{category}'");
                return;
            }

            DisplayCategoryCommands(commandCategory);
        }

        /// <summary>
        /// Display a list of commands in a category.
        /// </summary>
        /// <param name="commandCategory">The category enum value whose commands we want displayed.</param>
        private static void DisplayCategoryCommands(CommandCategory commandCategory)
        {
            List<CommandDescription> commandDescriptions;

            // Lookup command category.
            //
            if (!mapCategoryToDescriptionList.TryGetValue(commandCategory, out commandDescriptions))
            {
                Console.WriteLine($"{CabeiroConstants.Program.Name} does not include any commands in category '{commandCategory}'");
                return;
            }

            Console.WriteLine($"{commandCategory.ToString().ToUpper()} commands:\n");

            foreach (CommandDescription commandDescription in commandDescriptions)
            {
                Console.WriteLine($"\t{commandDescription.Command.ToUpper()} - {commandDescription.Description}");
            }
        }

        /// <summary>
        /// Display a list of all available command categories.
        /// </summary>
        public static void DisplayAllCommandCategories()
        {
            if (categories.Count == 0)
            {
                Console.WriteLine($"Sorry, {CabeiroConstants.Program.Name} does not include any command categories yet!");
                return;
            }

            Console.WriteLine($"{CabeiroConstants.Program.Name} command categories are:\n");

            foreach (CommandCategory commandCategory in categories)
            {
                Console.WriteLine($"\t{commandCategory.ToString()}");
            }
        }

        /// <summary>
        /// Display all commands in all categories.
        /// </summary>
        public static void DisplayAllCommands()
        {
            DisplayProgramDescription(displayGetStartedTip: false);

            Console.WriteLine();

            if (categories.Count == 0)
            {
                Console.WriteLine($"Sorry, {CabeiroConstants.Program.Name} does not include any commands yet!");
                return;
            }

            Console.WriteLine($"Full list of {CabeiroConstants.Program.Name} commands:\n");

            foreach (CommandCategory commandCategory in categories)
            {
                DisplayCategoryCommands(commandCategory);
                Console.WriteLine("\n");
            }
        }

        /// <summary>
        /// Display the version of both Cabeiro and Proteus.
        /// </summary>
        public static void DisplayProgramVersion()
        {
            AssemblyName proteusInfo = ProteusInfo.GetAssemblyInfo();
            AssemblyName cabeiroInfo = CabeiroInfo.GetAssemblyInfo();

            Console.WriteLine($"{cabeiroInfo.Name} program version: {cabeiroInfo.Version}");
            Console.WriteLine($"{proteusInfo.Name} library version: {proteusInfo.Version}");
        }

        /// <summary>
        /// Display a description of Cabeiro functionality.
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
    }
}
