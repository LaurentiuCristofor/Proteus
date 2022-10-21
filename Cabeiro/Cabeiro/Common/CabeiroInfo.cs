////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Reflection;

namespace LaurentiuCristofor.Cabeiro.Common
{
    /// <summary>
    /// This class can be used to get information about the Cabeiro assembly.
    /// </summary>
    public abstract class CabeiroInfo
    {
        public static AssemblyName GetAssemblyInfo()
        {
            return Assembly.GetExecutingAssembly().GetName();
        }
    }
}
