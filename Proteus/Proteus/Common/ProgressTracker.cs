////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A character based execution progress tracker.
    /// </summary>
    public abstract class ProgressTracker
    {
        public const ulong OneMillion = 1000000;
        public const ulong OneHundredThousand = OneMillion / 10;
        public const ulong TenThousand = OneHundredThousand / 10;

        public const string OneMillionMarker = "O";
        public const string OneHundredThousandMarker = "o";
        public const string TenThousandMarker = ".";

        /// <summary>
        /// Displays a character, as appropriate, based on the progress indicated by the count of execution units.
        /// </summary>
        /// <param name="countExecutionUnits"></param>
        public static void Track(ulong countExecutionUnits)
        {
            if (countExecutionUnits % OneMillion == 0)
            {
                ulong countMillions = countExecutionUnits / OneMillion;

                OutputInterface.Log($"{OneMillionMarker}({countMillions})");
            }
            else if (countExecutionUnits % OneHundredThousand == 0)
            {
                OutputInterface.Log(OneHundredThousandMarker);
            }
            else if (countExecutionUnits % TenThousand == 0)
            {
                OutputInterface.Log(TenThousandMarker);
            }
        }

        /// <summary>
        /// Prepares the display of a new progress sequence, starting from a new line.
        /// </summary>
        public static void Reset()
        {
            OutputInterface.LogLine(string.Empty);
        }
    }
}
