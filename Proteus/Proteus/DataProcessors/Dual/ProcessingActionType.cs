////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Dual
{
    /// <summary>
    /// Types of processing actions.
    /// </summary>
    public enum ProcessingActionType
    {
        NotSet = 0,

        AdvanceFirst = 1,
        AdvanceSecond = 2,
        AdvanceBoth = 3,
        Terminate = 4,
    }
}
