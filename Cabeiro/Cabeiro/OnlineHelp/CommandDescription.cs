////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
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
        public string Command { get; protected set; }

        /// <summary>
        /// The category of the command.
        /// </summary>
        public CommandCategory Category { get; protected set; }

        /// <summary>
        /// A short description of the command.
        /// </summary>
        public string ShortDescription { get; protected set; }

        /// <summary>
        /// A longer description of the command, optional.
        /// </summary>
        public string LongDescription { get; protected set; }

        /// <summary>
        /// A description of the command's arguments.
        /// </summary>
        public string Arguments { get; protected set; }

        /// <summary>
        /// Additional notes on the command or its arguments, optional.
        /// </summary>
        public string Notes { get; protected set; }

        public CommandDescription(
            string command,
            CommandCategory category,
            string shortDescription,
            string longDescription,
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

            if (string.IsNullOrWhiteSpace(shortDescription))
            {
                throw new CabeiroException($"Internal error: invalid short description string for CommandDescription: {shortDescription}");
            }

            if (string.IsNullOrWhiteSpace(arguments))
            {
                throw new CabeiroException($"Internal error: invalid arguments string for CommandDescription: {arguments}");
            }

            this.Command = command;
            this.Category = category;
            this.ShortDescription = shortDescription;
            this.LongDescription = longDescription;
            this.Arguments = arguments;
            this.Notes = notes;
        }

        /// <summary>
        /// Displays the information for this command.
        /// </summary>
        public void Display()
        {
            Console.WriteLine($"{Constants.Formatting.TextEmphasisWings} {this.Command.ToUpper()} command {Constants.Formatting.TextEmphasisWings}\n");

            Console.WriteLine($"Category: {this.Category}\n");

            Console.WriteLine($"Description:\n\n\t{this.ShortDescription}\n");

            if (!String.IsNullOrEmpty(this.LongDescription))
            {
                Console.WriteLine($"{this.LongDescription}\n");
            }

            Console.WriteLine($"Arguments:\n\n\t{this.Arguments}\n");

            if (!string.IsNullOrWhiteSpace(this.Notes))
            {
                Console.WriteLine($"Notes:\n\n{this.Notes}");
            }
        }
    }
}
