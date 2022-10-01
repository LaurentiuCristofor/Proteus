////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Proteus.Common.Logging
{
    /// <summary>
    /// An ILogger implementation that writes to Out and Error streams.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Output(string message)
        {
            Console.Out.Write(message);
        }

        public void OutputLine(string message)
        {
            Console.Out.WriteLine(message);
        }

        public void Log(string message)
        {
            Console.Error.Write(message);
        }

        public void LogLine(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}
