// ---------------------------------------------------------------------------
// Campari Software
//
// FrameworkVersion.cs
//
//
// ---------------------------------------------------------------------------
// Copyright (C) 2006-2007 Campari Software
// All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
// ---------------------------------------------------------------------------
/* The Code Project Open License (CPOL)

Preamble
This License governs Your use of the Work. This License is intended to allow
developers to use the Source Code and Executable Files provided as part of
the Work in any application in any form. 

The main points subject to the terms of the License are:

   * Source Code and Executable Files can be used in commercial applications; 
   * Source Code and Executable Files can be redistributed; and 
   * Source Code can be modified to create derivative works. 
   * No claim of suitability, guarantee, or any warranty whatsoever is provided. 
     The software is provided "as-is". 

This License is entered between You, the individual or other entity reading
or otherwise making use of the Work licensed pursuant to this License and 
the individual or other entity which offers the Work under the terms of this
License ("Author").

For full license details, see http://www.codeproject.com/info/cpol10.aspx
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Utility
{
    #region enum FrameworkVersion
    /// <summary>
    /// Specifies the .NET Framework versions
    /// </summary>
    public enum FrameworkVersion
    {
        /// <summary>
        /// .NET Framework 1.0
        /// </summary>
        Fx10,

        /// <summary>
        /// .NET Framework 1.1
        /// </summary>
        Fx11,

        /// <summary>
        /// .NET Framework 2.0
        /// </summary>
        Fx20,

        /// <summary>
        /// .NET Framework 3.0
        /// </summary>
        Fx30,

        /// <summary>
        /// .NET Framework 3.5
        /// </summary>
        Fx35,

        /// <summary>
        /// .NET Framework 4.0
        /// </summary>
        Fx40,

        /// <summary>
        /// .NET Framework 4.5
        /// </summary>
        Fx45,

        /// <summary>
        /// .NET Framework 4.5.1
        /// </summary>
        Fx451,

        /// <summary>
        /// .NET Framework 4.5.2
        /// </summary>
        Fx452,

        /// <summary>
        /// .NET Framework 4.6
        /// </summary>
        Fx46,

        /// <summary>
        /// .NET Framework 4.6.1
        /// </summary>
        Fx461,

        /// <summary>
        /// .NET Framework 4.6.2
        /// </summary>
        Fx462,

        /// <summary>
        /// .NET Framework 4.7
        /// </summary>
        Fx47,

        /// <summary>
        /// .NET Framework 4.7.1
        /// </summary>
        Fx471,

        /// <summary>
        /// .NET Framework 4.7.2
        /// </summary>
        Fx472,

        /// <summary>
        /// .NET Framework 4.8
        /// </summary>
        Fx48,
    }
    #endregion
}
