////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Cabeiro.Common;

namespace LaurentiuCristofor.Cabeiro.OnlineHelp
{
    /// <summary>
    /// A description of a command.
    /// </summary>
    public class CommandDescription
    {
        /// <summary>
        /// The actual command string.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The category of the command.
        /// </summary>
        public CommandCategory Category { get; set; }

        /// <summary>
        /// A short description of the command.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A description of the command's arguments.
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Additional notes on the command or its arguments.
        /// </summary>
        public string Notes { get; set; }

        public CommandDescription(
            string command,
            CommandCategory category,
            string description,
            string arguments,
            string notes)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new CabeiroException($"Internal error: invalid command string for CommandDescription: {command}");
            }

            if (category == CommandCategory.NotSet)
            {
                throw new CabeiroException("Internal error: invalid (unset) category for CommandDescription!");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new CabeiroException($"Internal error: invalid description string for CommandDescription: {description}");
            }

            if (string.IsNullOrWhiteSpace(arguments))
            {
                throw new CabeiroException($"Internal error: invalid arguments string for CommandDescription: {arguments}");
            }

            Command = command;
            Category = category;
            Description = description;
            Arguments = arguments;
            Notes = notes;
        }

        /// <summary>
        /// Displays the information for this command.
        /// </summary>
        public void Display()
        {
            Console.WriteLine($"{Constants.Formatting.TextEmphasisWings} {Command.ToUpper()} command {Constants.Formatting.TextEmphasisWings}\n");

            Console.WriteLine($"Category: {Category.ToString()}\n");

            Console.WriteLine($"Description:\n\n\t{Description}\n");

            Console.WriteLine($"Arguments:\n\n\t{Arguments}\n");

            if (!string.IsNullOrWhiteSpace(Notes))
            {
                Console.WriteLine($"{Notes}");
            }
        }
    }
}
