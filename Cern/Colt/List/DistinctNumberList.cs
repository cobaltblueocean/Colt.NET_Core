// <copyright file="DistinctNumberList.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.List
{
    /// <summary>
    /// Resizable compressed list holding numbers; based on the fact that a number from a large list with few distinct values need not take more than <tt>log(distinctValues)</tt> bits; implemented with a <tt>MinMaxNumberList</tt>.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// <p>
    /// This class can, for example, be useful when making large lists of numbers persistent.
    /// Also useful when very large lists would otherwise consume too much main memory.
    /// <p>
    /// You can add, get and set elements quite similar to <tt>java.util.ArrayList</tt>.
    /// <p>
    /// <b>Applicability:</b> Applicable if data is highly skewed and legal values are known in advance. Robust in the presence of "outliers".
    /// <p>
    /// <b>Performance:</b> Operations <tt>get()</tt>, <tt>size()</tt> and <tt>clear()</tt> are <tt>O(1)</tt>, i.e. run in constant time.
    /// Operations like <tt>add()</tt> and <tt>set()</tt> are <tt>O(log(distinctValues.length))</tt>.
    /// <p>
    /// Upon instantiation a contract is signed that defines the distinct values allowed to be hold in this list.
    /// It is not legal to store elements other than specified by the contract.
    /// Any attempt to violate the contract will throw an <tt>IllegalArgumentException</tt>.
    /// <p>
    /// Although access methods are only defined on <tt>long</tt> values you can also store
    /// all other primitive data types: <tt>boolean</tt>, <tt>byte</tt>, <tt>short</tt>, <tt>int</tt>, <tt>long</tt>, <tt>float</tt>, <tt>double</tt> and <tt>char</tt>.
    /// You can do this by explicitly representing them as <tt>long</tt> values.
    /// Use casts for discrete data types.
    /// Use the methods of <tt>java.lang.Float</tt> and <tt>java.lang.Double</tt> for floating point data types:
    /// Recall that with those methods you can convert any floating point value to a <tt>long</tt> value and back <b>without losing any precision</b>:
    /// <p>
    /// <b>Example usage:</b><pre>
    /// DistinctNumberList list = ... instantiation goes here
    /// double d1 = 1.234;
    /// list.add(Double.doubleToLongBits(d1));
    /// double d2 = Double.longBitsToDouble(list.get(0));
    /// if (d1!=d2) System.out.println("This is impossible!");
    /// 
    /// DistinctNumberList list2 = ... instantiation goes here
    /// float f1 = 1.234f;
    /// list2.add((long) Float.floatToIntBits(f1));
    /// float f2 = Float.intBitsToFloat((int)list2.get(0));
    /// if (f1!=f2) System.out.println("This is impossible!");
    /// </pre>
    /// 
    /// <summary>
    /// <see cref="LongArrayList"></see>
    /// <see cref="MinMaxNumberList"></see>
    /// <see cref="java.lang.Float"></see>
    /// <see cref="java.lang.Double"></see>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    public class DistinctNumberList : AbstractLongList
    {
        protected long[] distinctValues;
        protected MinMaxNumberList elements;

        public override int Size { set => SetSize(value); }

        public override int Count => Size;

        public override bool IsReadOnly => false;

        /// <summary>
        /// Constructs an empty list with the specified initial capacity and the specified distinct values allowed to be hold in this list.
        /// 
        /// <summary>
        /// <param name=""> distinctValues   an array sorted ascending containing the distinct values allowed to be hold in this list.</param>
        /// <param name=""> initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.</param>
        public DistinctNumberList(long[] distinctValues, int initialCapacity)
        {
            SetUp(distinctValues, initialCapacity);
        }
        /// <summary>
        /// Appends the specified element to the end of this list.
        /// 
        /// <summary>
        /// <param name="element">element to be appended to this list.</param>
        public override void Add(long element)
        {
            //overridden for performance only.
            elements.Add(CodeOf(element));
            Size++;
        }
        /// <summary>
        /// Returns the code that shall be stored for the given element.
        /// <summary>
        protected int CodeOf(long element)
        {
            int index = Array.BinarySearch(distinctValues, element);
            if (index < 0) throw new ArgumentException("Element=" + element + " not contained in distinct elements.");
            return index;
        }
        /// <summary>
        /// Ensures that the receiver can hold at least the specified number of elements without needing to allocate new internal memory.
        /// If necessary, allocates new internal memory and increases the capacity of the receiver.
        /// 
        /// <summary>
        /// <param name=""> minCapacity   the desired minimum capacity.</param>
        public override void EnsureCapacity(int minCapacity)
        {
            elements.EnsureCapacity(minCapacity);
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver; <b>WARNING:</b> Does not check preconditions.
        /// Provided with invalid parameters this method may return invalid elements without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <tt>index &gt;= 0 && index &lt; size()</tt>.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        protected override long GetQuick(int index)
        {
            return distinctValues[(int)(elements[index])];
        }
        /// <summary>
        /// Removes from the receiver all elements whose index is between
        /// <code>from</code>, inclusive and <code>to</code>, inclusive.  Shifts any succeeding
        /// elements to the left (reduces their index).
        /// This call shortens the list by <tt>(to - from + 1)</tt> elements.
        /// 
        /// <summary>
        /// <param name="from">index of first element to be removed.</param>
        /// <param name="to">index of last element to be removed.</param>
        /// <exception cref="IndexOutOfBoundsException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public override void RemoveFromTo(int from, int to)
        {
            elements.RemoveFromTo(from, to);
            Size -= to - from + 1;
        }
        /// <summary>
        /// Replaces the element at the specified position in the receiver with the specified element; <b>WARNING:</b> Does not check preconditions.
        /// Provided with invalid parameters this method may access invalid indexes without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <tt>index &gt;= 0 && index &lt; size()</tt>.
        /// 
        /// <summary>
        /// <param name="index">index of element to replace.</param>
        /// <param name="element">element to be stored at the specified position.</param>
        protected override void SetQuick(int index, long element)
        {
            elements[index] = CodeOf(element);
        }
        /// <summary>
        /// Sets the size of the receiver without modifying it otherwise.
        /// This method should not release or allocate new memory but simply set some instance variable like <tt>size</tt>.
        /// <summary>
        protected override void SetSizeRaw(int newSize)
        {
            base.SetSizeRaw(newSize);
            elements.SetSize(newSize);
        }
        /// <summary>
        /// Sets the receiver to an empty list with the specified initial capacity and the specified distinct values allowed to be hold in this list.
        /// 
        /// <summary>
        /// <param name=""> distinctValues   an array sorted ascending containing the distinct values allowed to be hold in this list.</param>
        /// <param name=""> initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.</param>
        protected void SetUp(long[] distinctValues, int initialCapacity)
        {
            this.distinctValues = distinctValues;
            //java.util.Arrays.sort(this.distinctElements);
            this.elements = new MinMaxNumberList(0, distinctValues.Length - 1, initialCapacity);
        }
        /// <summary>
        /// Trims the capacity of the receiver to be the receiver's current
        /// size. An application can use this operation to minimize the
        /// storage of the receiver.
        /// <summary>
        public override void TrimToSize()
        {
            elements.TrimToSize();
        }

        public override void Insert(int index, long item)
        {
            BeforeInsert(index, item);
        }

        public override void CopyTo(long[] array, int arrayIndex)
        {
            Elements.CopyTo(array, arrayIndex);
        }

        public override IEnumerator<long> GetEnumerator()
        {
            foreach (var item in Elements)
                yield return item;
        }
    }
}
