////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Types
{
    /// <summary>
    /// Types of data distributions.
    /// </summary>
    public enum DataDistributionType
    {
        NotSet = 0,

        // Standard distributions.
        //
        Uniform = 1,
        Normal = 2,
        Exponential = 3,
        Poisson = 4,
    }
}
