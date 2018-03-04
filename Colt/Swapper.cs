// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Swapper.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Swap elements at two positions (a,b).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Colt
{
    /// <summary>
    /// Swap elements at two positions (a,b).
    /// </summary>
    /// <param name="a">
    /// The a.
    /// </param>
    /// <param name="b">
    /// The b.
    /// </param>
    public delegate void Swapper(int a, int b);
}
