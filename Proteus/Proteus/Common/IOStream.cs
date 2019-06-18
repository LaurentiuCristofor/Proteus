////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// An abstraction to separate the library from the output streams.
    /// </summary>
    internal abstract class IOStream
    {
        internal static void Output(string s)
        {
            Console.Out.Write(s);
        }

        internal static void OutputLine(string s)
        {
            Console.Out.WriteLine(s);
        }

        internal static void Log(string s)
        {
            Console.Error.Write(s);
        }

        internal static void LogLine(string s)
        {
            Console.Error.WriteLine(s);
        }

        internal static void LogWarning(string s)
        {
            Console.Error.WriteLine(s);
        }
    }
}
