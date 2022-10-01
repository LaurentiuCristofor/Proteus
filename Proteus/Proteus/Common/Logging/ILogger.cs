////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Logging
{
    /// <summary>
    /// A logging interface for enabling Proteus to output information.
    /// 
    /// Output methods are used for processing results.
    /// Log methods are used for informational and warning messages.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Output a result message without adding a line terminator.
        /// </summary>
        /// <param name="message">The message to output.</param>
        void Output(string message);

        /// <summary>
        /// Output a result message and add a line terminator.
        /// </summary>
        /// <param name="message">The message to output.</param>
        void OutputLine(string message);

        /// <summary>
        /// Log a result message without adding a line terminator.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Log(string message);

        /// <summary>
        /// Log a result message and add a line terminator.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogLine(string message);

        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogWarning(string message);
    }
}
