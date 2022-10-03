////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Logging
{
    /// <summary>
    /// An abstract class for managing the ILogger instance that should be used by Proteus.
    /// </summary>
    public abstract class LoggingManager
    {
        /// <summary>
        /// The ILogger instance that the library will use.
        /// </summary>
        static ILogger theLogger;

        /// <summary>
        /// Initialize the ILogger instance that the library will use.
        /// </summary>
        /// <param name="logger">The ILogger instance to use; if null, the library will create and use a new instance of ConsoleLogger.</param>
        public static void Initialize(ILogger logger = null)
        {
           theLogger = logger ?? new ConsoleLogger();
        }

        /// <summary>
        /// Retrieve the ILogger instance.
        /// </summary>
        /// <returns>The ILogger instance.</returns>
        public static ILogger GetLogger()
        {
            if (theLogger == null)
            {
                throw new ProteusException("Logging was not initialized! LoggingManager.Initialize() should be called before accessing any Proteus functionality that requires logging.");
            }

            return theLogger;
        }
    }
}
