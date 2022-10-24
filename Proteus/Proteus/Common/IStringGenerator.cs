////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// An interface for generating data in string format.
    /// </summary>
    public interface IStringGenerator
    {
        /// <summary>
        /// Returns the next generated string.
        /// </summary>
        /// <returns>The generated string or null if no more strings could be generated.</returns>
        string NextString();
    }
}
