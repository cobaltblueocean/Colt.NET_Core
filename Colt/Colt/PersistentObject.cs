// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistentObject.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   This empty class is the common root for all persistent capable classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Cern.Colt
{
    using System;

    /// <summary>
    /// This empty class is the common root for all persistent capable classes.
    /// </summary>
    [Serializable]
    public abstract class PersistentObject : ICloneable
    {
        /// <summary>
        /// Returns a copy of the receiver.
        /// </summary>
        /// <returns>
        /// a copy of the receiver.
        /// </returns>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
