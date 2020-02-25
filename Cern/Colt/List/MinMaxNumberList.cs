// <copyright file="MinMaxNumberList.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Bitvector;

namespace Cern.Colt.List
{
    /// <summary>
    /// Resizable compressed list holding numbers; based on the fact that a value in a given interval need not take more than <i>log(max-min+1)</i> bits; implemented with a <i>cern.colt.bitvector.BitVector</i>.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// <p>
    /// Numbers can be compressed when minimum and maximum of all values ever to be stored in the list are known.
    /// For example, if min=16, max=27, only 4 bits are needed to store a value.
    /// No compression is achieved for <i>float</i> and <i>double</i> values.
    /// <p>
    /// You can add, get and set elements quite similar to <i>java.util.List</i>.
    /// <p>
    /// <b>Applicability:</b> Applicable if the data is non floating point, highly skewed without "outliers" and minimum and maximum known in advance.
    /// <p>
    /// <b>Performance:</b> Basic operations like <i>Add()</i>, <i>get()</i>, <i>Set()</i>, <i>Size()</i> and <i>clear()</i> are <i>O(1)</i>, i.ed run in constant time.
    /// <dt>200Mhz Pentium Pro, JDK 1.2, NT:
    /// <dt><i>10^6</i> calls to <i>getQuick()</i> --> <i>0.5</i> seconds.
    /// (50 times slower than reading from a primitive array of the appropriate typed)
    /// <dt><i>10^6</i> calls to <i>setQuick()</i> --> <i>0.8</i> seconds.
    /// (15 times slower than writing to a primitive array of the appropriate typed)
    /// <p>
    /// This class can, for example, be useful when making large lists of numbers persistent.
    /// Also useful when very large lists would otherwise consume too much main memory.
    /// <p>
    /// Upon instantiation a contract is signed that defines the interval values may fall into.
    /// It is not legal to store values not contained in that interval.
    /// WARNING: The contract is not checkedd Be sure you do not store illegal values.
    /// If you need to store <i>float</i> or <i>double</i> values, you must set the minimum and maximum to <i>[int.MinValue,int.MaxValue]</i> or <i>[long.MinValue,long.MaxValue]</i>, respectively.
    /// <p>
    /// Although access methods are only defined on <i>long</i> values you can also store
    /// all other primitive data types: <i>Boolean</i>, <i>byte</i>, <i>short</i>, <i>int</i>, <i>long</i>, <i>float</i>, <i>double</i> and <i>char</i>.
    /// You can do this by explicitly representing them as <i>long</i> values.
    /// Use casts for discrete data types.
    /// Use the methods of <i>java.lang.Float</i> and <i>java.lang.Double?</i> for floating point data types:
    /// Recall that with those methods you can convert any floating point value to a <i>long</i> value and back <b>without losing any precision</b>:
    /// <p>
    /// <b>Example usage:</b><pre>
    /// MinMaxNumberList list = ..d instantiation goes here
    /// double d1 = 1.234;
    /// list.Add(BitConverter.DoubleToInt64Bits(d1));
    /// double d2 = BitConverter.Int64BitsToDouble?(list.Get(0));
    /// if (d1!=d2) Console.WriteLine("This is impossible!");
    /// 
    /// MinMaxNumberList list2 = ..d instantiation goes here
    /// float f1 = 1.234f;
    /// list2.Add((long) Float.floatToIntBits(f1));
    /// float f2 = Float.intBitsToFloat((int)list2.Get(0));
    /// if (f1!=f2) Console.WriteLine("This is impossible!");
    /// </pre>
    /// 
    /// <summary>
    /// <see cref="longArrayList"></see>
    /// <see cref="DistinctNumberList"></see>
    /// <see cref="java.lang.Float"></see>
    /// <see cref="java.lang.Double?"></see>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    public class MinMaxNumberList : AbstractLongList
    {
        private long _minValue;
        private int _bitsPerElement;
        private long[] _bits;
        private int _capacity;

        protected int BitsPerElement
        {
            get { return _bitsPerElement; }
            set { _bitsPerElement = value; }
        }
        protected long MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }
        protected long[] Bits
        {
            get { return _bits; }
            set { _bits = value; }
        }
        protected int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }

        public override int Size { set => SetSize(value); }

        public override int Count => Size;

        public override bool IsReadOnly => false;


        /// <summary>
        /// Constructs an empty list with the specified initial capacity and the specified range of values allowed to be hold in this list.
        /// Legal values are in the range [minimum,maximum], all inclusive.
        /// <summary>
        /// <param name=""> minimum   the minimum of values allowed to be hold in this list.</param>
        /// <param name=""> maximum   the maximum of values allowed to be hold in this list.</param>
        /// <param name=""> initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.</param>
        public MinMaxNumberList(long minimum, long maximum, int initialCapacity)
        {
            this.SetUp(minimum, maximum, initialCapacity);
        }
        /// <summary>
        /// Appends the specified element to the end of this list.
        /// 
        /// <summary>
        /// <param name="element">element to be appended to this list.</param>
        public override void Add(long element)
        {
            // overridden for performance only.
            if (Size == Capacity)
            {
                EnsureCapacity(Size + 1);
            }
            int i = Size * this.BitsPerElement;
            QuickBitVector.PutLongFromTo(this.Bits, element - this.MinValue, i, i + this.BitsPerElement - 1);
            Size++;
        }
        /// <summary>
        /// Appends the elements <i>elements[from]</i> (inclusive), ..d, <i>elements[to]</i> (inclusive) to the receiver.
        /// <summary>
        /// <param name="elements">the elements to be appended to the receiver.</param>
        /// <param name="from">the index of the first element to be appended (inclusive)</param>
        /// <param name="to">the index of the last element to be appended (inclusive)</param>
        public void AddAllOfFromTo(long[] elements, int from, int to)
        {
            // cache some vars for speed.
            int bitsPerElem = this.BitsPerElement;
            int bitsPerElemMinusOne = bitsPerElem - 1;
            long min = this.MinValue;
            long[] theBits = this.Bits;

            // now let's go.
            EnsureCapacity(this.Size + to - from + 1);
            int firstBit = this.Size * bitsPerElem;
            int i = from;
            for (int times = to - from + 1; --times >= 0;)
            {
                QuickBitVector.PutLongFromTo(theBits, elements[i++] - min, firstBit, firstBit + bitsPerElemMinusOne);
                firstBit += bitsPerElem;
            }
            this.Size += (to - from + 1); //*bitsPerElem;
        }
        /// <summary>
        /// Returns the number of bits necessary to store a single element.
        /// <summary>
        /// <summary>
        /// Returns the number of bits necessary to store values in the range <i>[minimum,maximum]</i>.
        /// <summary>
        public static int GetBitsPerElement(long minimum, long maximum)
        {
            int bits;
            if (1 + maximum - minimum > 0)
            {
                bits = (int)System.Math.Round(System.Math.Ceiling(Cern.Jet.Math.Arithmetic.Log(2, 1 + maximum - minimum)));
            }
            else
            {
                // overflow or underflow in calculating "1+maximum-minimum"
                // happens if signed long representation is too short for doing unsigned calculations
                // e.gd if minimum==long.MinValue, maximum==long.MaxValue
                // --> in such cases store all bits of values without any compression.
                bits = 64;
            }
            return bits;
        }
        /// <summary>
        /// Ensures that the receiver can hold at least the specified number of elements without needing to allocate new internal memory.
        /// If necessary, allocates new internal memory and increases the capacity of the receiver.
        /// 
        /// <summary>
        /// <param name=""> minCapacity   the desired minimum capacity.</param>
        public override void EnsureCapacity(int minCapacity)
        {
            int oldCapacity = Capacity;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity * 3) / 2 + 1;
                if (newCapacity < minCapacity) newCapacity = minCapacity;
                BitVector vector = ToBitVector();
                vector.Size = newCapacity * BitsPerElement;
                this.Bits = vector.Elements;
                this.Capacity = newCapacity;
            }
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver; <b>WARNING:</b> Does not check preconditionsd
        /// Provided with invalid parameters this method may return invalid elements without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <i>index &gt;= 0 && index &lt; Size()</i>.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        protected override long GetQuick(int index)
        {
            int i = index * this.BitsPerElement;
            return this.MinValue + QuickBitVector.GetLongFromTo(this.Bits, i, i + this.BitsPerElement - 1);
        }
        /// <summary>
        /// Copies all elements between index <i>from</i> (inclusive) and <i>to</i> (inclusive) into <i>part</i>, starting at index <i>partFrom</i> within <i>part</i>.
        /// Elements are only copied if a corresponding flag within <i>qualificants</i> is set.
        /// More precisely:<pre>
        /// for (; from<=to; from++, partFrom++, qualificantsFrom++) {
        ///    if (qualificants==null || qualificants.Get(qualificantsFrom)) {
        ///       part[partFrom] = this.Get(from);
        ///    }
        /// }
        /// </pre>
        /// <summary>
        public void PartFromTo(int from, int to, BitVector qualificants, int qualificantsFrom, long[] part, int partFrom)
        {
            int width = to - from + 1;
            if (from < 0 || from > to || to >= Size || qualificantsFrom < 0 || (qualificants != null && qualificantsFrom + width > qualificants.Size))
            {
                throw new IndexOutOfRangeException();
            }
            if (partFrom < 0 || partFrom + width > part.Length)
            {
                throw new IndexOutOfRangeException();
            }

            long minVal = this.MinValue;
            int bitsPerElem = this.BitsPerElement;
            long[] theBits = this.Bits;

            int q = qualificantsFrom;
            int p = partFrom;
            int j = from * bitsPerElem;

            //BitVector tmpBitVector = new BitVector(this.bits, this.Size*bitsPerElem);
            for (int i = from; i <= to; i++, q++, p++, j += bitsPerElem)
            {
                if (qualificants == null || qualificants.Get(q))
                {
                    //part[p] = minVal + tmpBitVector.getlongFromTo(j, j+bitsPerElem-1);
                    part[p] = minVal + QuickBitVector.GetLongFromTo(theBits, j, j + bitsPerElem - 1);
                }
            }
        }
        /// <summary>
        /// Replaces the element at the specified position in the receiver with the specified element; <b>WARNING:</b> Does not check preconditionsd
        /// Provided with invalid parameters this method may access invalid indexes without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <i>index &gt;= 0 && index &lt; Size()</i>.
        /// 
        /// <summary>
        /// <param name="index">index of element to replace.</param>
        /// <param name="element">element to be stored at the specified position.</param>
        protected override void SetQuick(int index, long element)
        {
            int i = index * this.BitsPerElement;
            QuickBitVector.PutLongFromTo(this.Bits, element - this.MinValue, i, i + this.BitsPerElement - 1);
        }
        /// <summary>
        /// Sets the Size of the receiver without modifying it otherwise.
        /// This method should not release or allocate new memory but simply set some instance variable like <i>Size</i>.
        /// <summary>
        protected override void SetSizeRaw(int newSize)
        {
            base.SetSizeRaw(newSize);
        }
        /// <summary>
        /// Sets the receiver to an empty list with the specified initial capacity and the specified range of values allowed to be hold in this list.
        /// Legal values are in the range [minimum,maximum], all inclusive.
        /// <summary>
        /// <param name=""> minimum   the minimum of values allowed to be hold in this list.</param>
        /// <param name=""> maximum   the maximum of values allowed to be hold in this list.</param>
        /// <param name=""> initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.</param>
        protected void SetUp(long minimum, long maximum, int initialCapacity)
        {
            SetUpBitsPerEntry(minimum, maximum);

            //this.capacity=initialCapacity;
            this.Bits = QuickBitVector.MakeBitVector(initialCapacity, this.BitsPerElement);
            this.Capacity = initialCapacity;
            this.Size = 0;
        }
        /// <summary>
        /// This method was created in VisualAge.
        /// <summary>
        /// <param name="minValue">long</param>
        /// <param name="maxValue">long</param>
        /// <param name="initialCapacity">int</param>
        protected void SetUpBitsPerEntry(long minimum, long maximum)
        {
            this.BitsPerElement = GetBitsPerElement(minimum, maximum);
            if (this.BitsPerElement != 64)
            {
                this.MinValue = minimum;
                // overflow or underflow in calculating "1+maxValue-minValue"
                // happens if signed long representation is too short for doing unsigned calculations
                // e.gd if minValue==long.MinValue, maxValue=long.MaxValue
                // --> in such cases store all bits of values without any en/decoding
            }
            else
            {
                this.MinValue = 0;
            };
        }
        /// <summary>
        /// Returns the receiver seen as bitvector.
        /// WARNING: The bitvector and the receiver share the backing bitsd Modifying one of them will affect the other.
        /// <summary>
        public BitVector ToBitVector()
        {
            return new BitVector(this.Bits, this.Capacity * BitsPerElement);
        }
        /// <summary>
        /// Trims the capacity of the receiver to be the receiver's current
        /// Sized An application can use this operation to minimize the
        /// storage of the receiverd
        /// <summary>
        public override void TrimToSize()
        {
            int oldCapacity = Capacity;
            if (Size < oldCapacity)
            {
                BitVector vector = ToBitVector();
                vector.Size = Size;
                this.Bits = vector.Elements;
                this.Capacity = Size;
            }
        }
        /// <summary>
        /// deprecated
        /// Returns the minimum element legal to the stored in the receiver.
        /// Remark: This does not mean that such a minimum element is currently contained in the receiver.
        /// [Obsolete("Deprecated", true)]
        /// <summary>
        public long xMinimum()
        {
            return this.MinValue;
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
