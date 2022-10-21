////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Reflection;

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// This class can be used to get information about the Proteus assembly.
    /// </summary>
    public abstract class ProteusInfo
    {
        public static AssemblyName GetAssemblyInfo()
        {
            return Assembly.GetExecutingAssembly().GetName();
        }
    }
}
