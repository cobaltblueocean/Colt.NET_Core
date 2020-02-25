// <copyright file="AbstractCollection.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.List
{
    /// <summary>
    /// Abstract base class for resizable collections holding objects or primitive data types such as <code>int</code>, <code>float</code>, etc.
    /// First see the<a href="package-summary.html"> package summary</a> and javadoc<a href="package-tree.html"> tree view</a> to get the broad picture.
    /// <p>
    /// <b>Note that this implementation is not synchronized.</b>
    /// 
    /// @author wolfgang.hoschek @cern.ch
    /// @version 1.0, 09/24/99
    /// <see cref = "IList{T}"></ see>
    /// <see cref= "Array"></ see >
    /// <summary>

    public abstract class AbstractCollection<T> : PersistentObject, ICollection<T>
    {
        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// <summary>
        protected AbstractCollection() { }
        /// <summary>
        /// Removes all elements from the receiverd  The receiver will
        /// be empty after this call returns.
        /// <summary>
        public abstract void Clear();
        /// <summary>
        /// Tests if the receiver has no elements.
        /// 
        /// <summary>
        /// <returns> <code>true</code> if the receiver has no elements;</returns>
        ///          <code>false</code> otherwise.
        public Boolean IsEmpty
        {
            get { return Size == 0; }
        }
        /// <summary>
        /// Returns the number of elements contained in the receiver.
        /// 
        /// <summary>
        /// @returns  the number of elements contained in the receiver.</returns>
        public abstract int Size { get; set; }
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }

        /// <summary>
        /// Returns a <code>java.util.List</code> containing all the elements in the receiver.
        /// <summary>
        public abstract List<T> ToList();
        /// <summary>
        /// Returns a string representation of the receiver, containing
        /// the String representation of each element.
        /// <summary>
        public override String ToString()
        {
            return ToList().ToString();
        }

        public abstract void Add(T item);
        public abstract bool Contains(T item);
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract bool Remove(T item);
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

