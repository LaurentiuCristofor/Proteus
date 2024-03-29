﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;

using LaurentiuCristofor.Proteus.Common.Logging;

namespace LaurentiuCristofor.Proteus.Common.Utilities
{
    /// <summary>
    /// A class used to measure the time elapsed between its initialization and a call to StopAndReport().
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// The Stopwatch instance used to measure time.
        /// </summary>
        protected Stopwatch Stopwatch { get; set; }

        /// <summary>
        /// The message to log before reporting the elapsed time.
        /// </summary>
        protected string StopMessage { get; set; }

        /// <summary>
        /// The number of line endings to print after we printed the elapsed time.
        /// </summary>
        protected uint CountFinalLineEndings { get; set; }

        /// <summary>
        /// Creates a timer for measuring a time interval and specifies the messages that should be printed when the time counting starts and stops.
        /// </summary>
        /// <param name="startMessage">The message to log at the start of the interval.</param>
        /// <param name="stopMessage">The message to log at the end of the interval.</param>
        /// <param name="countFinalLineEndings">The number of line endings to print after we printed the elapsed time.</param>
        public Timer(string startMessage, string stopMessage, uint countFinalLineEndings = 1)
        {
            StopMessage = stopMessage;
            CountFinalLineEndings = countFinalLineEndings;

            if (startMessage != null)
            {
                LoggingManager.GetLogger().Log(startMessage);
            }

            Stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Creates a timer that prints a single message.
        /// </summary>
        /// <param name="stopMessage"></param>
        public Timer(string stopMessage) : this(null, stopMessage)
        {
        }

        /// <summary>
        /// Creates a timer that prints no messages.
        /// </summary>
        public Timer() : this (null, null, 0)
        {
        }

        /// <summary>
        /// Stops the Stopwatch instance and reports the elapsed time.
        /// <returns>The elapsed TimeSpan value.</returns>
        /// </summary>
        public TimeSpan StopAndReport()
        {
            Stopwatch.Stop();

            TimeSpan timeSpan = Stopwatch.Elapsed;
            string formattedTimeSpan = Timer.FormatTimeSpan(timeSpan);

            if (StopMessage != null)
            {
                LoggingManager.GetLogger().Log($"{StopMessage}{formattedTimeSpan}");
            }

            uint count = CountFinalLineEndings;
            while (count-- > 0)
            {
                LoggingManager.GetLogger().LogLine(string.Empty);
            }

            return timeSpan;
        }

        /// <summary>
        /// Formats a TimeSpan instance.
        /// </summary>
        /// <param name="timeSpan">The TimeSpan instance to format.</param>
        /// <returns>A string representing the TimeSpan value.</returns>
        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return String.Format($"{timeSpan.Hours:00}h:{timeSpan.Minutes:00}m:{timeSpan.Seconds:00}s.{timeSpan.Milliseconds:000}ms");
        }
    }
}
