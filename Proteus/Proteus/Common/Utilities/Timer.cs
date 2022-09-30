////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;

namespace LaurentiuCristofor.Proteus.Common.Utilities
{
    public class Timer
    {
        protected Stopwatch Stopwatch { get; set; }

        protected string StopMessage { get; set; }

        public Timer(string startMessage, string stopMessage)
        {
            this.StopMessage = stopMessage;
            if (startMessage != null)
            {
                OutputInterface.Log(startMessage);
            }
            this.Stopwatch = new Stopwatch();
            this.Stopwatch.Start();
        }

        public Timer(string stopMessage) : this(null, stopMessage)
        {
        }

        public void StopAndReport()
        {
            this.Stopwatch.Stop();
            TimeSpan timeSpan = this.Stopwatch.Elapsed;
            if (this.StopMessage != null)
            {
                OutputInterface.Log($"{this.StopMessage}{Timer.FormatTimeSpan(timeSpan)}");
            }
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return String.Format($"{timeSpan.Hours:00}h:{timeSpan.Minutes:00}m:{timeSpan.Seconds:00}s.{timeSpan.Milliseconds / 10:00}ms");
        }
    }
}
