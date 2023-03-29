// <copyright file="BitVector.Cs" company="CERN">
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

namespace Cern.Colt.Bitvector
{
    public class BitVector
    {
        /*
        * Bits are packed into arrays of "units."  Currently a unit is a long,
        * which consists of 64 bits, requiring 6 address bitsd  The choice of unit
        * is determined purely by performance concerns.
        */

        /// <summary>
        /// The bits of this objectd  The ith bit is stored in bits[i/64] at
        /// bit position i % 64 (where bit position 0 refers to the least
        /// significant bit and 63 refers to the most significant bit).
        /// @serial
        /// </summary>
        protected long[] bits;

        protected int nbits; //the size

        /// <summary>
        /// Gets the size of the receiver, or sets to shrink or to expand the receiver so that it holds <i>newSize</i> bits.
        /// If the receiver is expanded, additional <i>false</i> bits are added to the end.
        /// If the receiver is shrinked, all bits between the old size and the new size are lost; their memory is subject to garbage collection.
        /// (This method introduces a new backing array of elementsd WARNING: if you have more than one BitVector or BitMatrix sharing identical backing elements, be sure you know what you are doingd)
        /// </summary>
        /// <exception cref="ArgumentException">if <i>size &lt; 0</i>.</exception>
        public int Size
        {
            get
            {
                return nbits;
            }
            set
            {
                if (value != Size)
                {
                    BitVector newVector = new BitVector(value);
                    newVector.ReplaceFromToWith(0, System.Math.Min(Size, value) - 1, this, 0);
                    SetElements(newVector.Elements, value);
                }
            }
        }

        /// <summary>
        /// IntProcedure for method indexOfFromTo(.d)
        /// </summary>
        public Cern.Colt.Function.IntProcedureDelegate IndexProcedure = new Cern.Colt.Function.IntProcedureDelegate((index) =>
        {
            var foundPos = index;
            return false;
        });

        public Boolean this[int bitIndex]
        {
            get {
                if (bitIndex < 0 || bitIndex >= nbits) throw new IndexOutOfRangeException(bitIndex.ToString());
                return QuickBitVector.Get(bits, bitIndex);
            }
            set {
                if (bitIndex < 0 || bitIndex >= nbits) throw new IndexOutOfRangeException(bitIndex.ToString());
                if (value)
                    QuickBitVector.Set(bits, bitIndex);
                else
                    QuickBitVector.Clear(bits, bitIndex);
            }
        }

        /// <summary>
        /// You normally need not use this methodd Use this method only if performance is criticald 
        /// Constructs a bit vector with the given backing bits and size.
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        ///
        /// <p>A bitvector is modelled as a long array, i.ed <i>long[] bits</i> holds bits of a bitvector.
        /// Each long value holds 64 bits.
        /// The i-th bit is stored in bits[i/64] at
        /// bit position i % 64 (where bit position 0 refers to the least
        /// significant bit and 63 refers to the most significant bit).
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="size"></param>
        /// <exception cref="ArgumentException">if <i>size &lt; 0 || size &gt; bits.Length*64</i>.</exception>
        public BitVector(long[] bits, int size)
        {
            SetElements(bits, size);
        }

        /// <summary>
        /// Constructs a bit vector that holds <i>size</i> bitsd All bits are initially <i>false</i>.
        /// </summary>
        /// <param name="size">the number of bits the bit vector shall have.</param>
        /// <exception cref="ArgumentException">if <i>size &lt; 0</i>.</exception>
        public BitVector(int size) : this(QuickBitVector.MakeBitVector(size, 1), size)
        {

        }

        /// <summary>
        /// Performs a logical <b>AND</b> of the receiver with another bit vector (A = A & B).
        /// The receiver is modified so that a bit in it has the
        /// value <code>true</code> if and only if it already had the 
        /// value <code>true</code> and the corresponding bit in the other bit vector
        /// argument has the value <code>true</code>.
        /// </summary>
        /// <param name="other">a bit vector.</param>
        /// <exception cref="ArgumentException">if <i>Size &gt; other.Count</i>.</exception>
        public void And(BitVector other)
        {
            if (this == other) return;
            CheckSize(other);
            long[] theBits = this.bits; // cached for speed.
            long[] otherBits = other.bits; //cached for speed.
            for (int i = theBits.Length; --i >= 0;) theBits[i] &= otherBits[i];
        }

        /// <summary>
        /// Clears all of the bits in receiver whose corresponding
        /// bit is set in the other bitvector (A = A \ B).
        /// In other words, determines the difference (A=A\B) between two bitvectors.
        /// </summary>
        /// <param name="other">a bitvector with which to mask the receiver.</param>
        /// <exception cref="ArgumentException">if <i>Size &gt; other.Count</i>.</exception>
        public void AndNot(BitVector other)
        {
            CheckSize(other);
            long[] theBits = this.bits; // cached for speed.
            long[] otherBits = other.bits; //cached for speed.
            for (int i = theBits.Length; --i >= 0;) theBits[i] &= ~otherBits[i];
        }

        /// <summary>
        /// Returns the number of bits currently in the <i>true</i> state.
        /// Optimized for speedd Particularly quick if the receiver is either sparse or dense.
        /// </summary>
        /// <returns></returns>
        public int Cardinality()
        {
            int cardinality = 0;
            int fullUnits = NumberOfFullUnits();
            int bitsPerUnit = QuickBitVector.BITS_PER_UNIT;

            // determine cardinality on full units
            long[] theBits = bits;
            for (int i = fullUnits; --i >= 0;)
            {
                long val = theBits[i];
                if (val == -1L)
                { // all bits set?
                    cardinality += bitsPerUnit;
                }
                else if (val != 0L)
                { // more than one bit set?
                    for (int j = bitsPerUnit; --j >= 0;)
                    {
                        if ((val & (1L << j)) != 0) cardinality++;
                    }
                }
            }

            // determine cardinality on remaining partial unit, if any.
            for (int j = NumberOfBitsInPartialUnit(); --j >= 0;)
            {
                if ((theBits[fullUnits] & (1L << j)) != 0) cardinality++;
            }

            return cardinality;
        }

        /// <summary>
        /// Checks if the given range is within the contained array's bounds.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="theSize"></param>
        protected static void CheckRangeFromTo(int from, int to, int theSize)
        {
            if (from < 0 || from > to || to >= theSize)
                throw new IndexOutOfRangeException("from: " + from + ", to: " + to + ", size=" + theSize);
        }

        /// <summary>
        /// Sanity check for operations requiring another bitvector with at least the same size.
        /// </summary>
        /// <param name="other"></param>
        protected void CheckSize(BitVector other)
        {
            if (nbits > other.Size) throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_IncompatibleSizes, nbits, other.Size));
        }

        /// <summary>
        /// Clears all bits of the receiver.
        /// </summary>
        public void Clear()
        {
            long[] theBits = this.bits;
            for (int i = theBits.Length; --i >= 0;) theBits[i] = 0L;

            //new longArrayList(bits).FillFromToWith(0,Size-1,0L);
        }

        /// <summary>
        /// Changes the bit with index <i>bitIndex</i> to the "clear" (<i>false</i>) state.
        /// </summary>
        /// <param name="bitIndex">the index of the bit to be cleared.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>bitIndex&lt;0 || bitIndex&gt;=Size</i></exception>
        public void Clear(int bitIndex)
        {
            if (bitIndex < 0 || bitIndex >= nbits) throw new IndexOutOfRangeException(bitIndex.ToString());
            QuickBitVector.Clear(bits, bitIndex);
        }

        /// <summary>
        /// Cloning this <code>BitVector</code> produces a new <code>BitVector</code> 
        /// that is equal to it.
        /// The clone of the bit vector is another bit vector that has exactly the 
        /// same bits set to <code>true</code> as this bit vector and the same 
        /// current size, but independent state.
        /// </summary>
        /// <returns>a deep copy of this bit vector.</returns>
        public Object Clone()
        {
            BitVector clone = (BitVector)base.MemberwiseClone();
            if (this.bits != null) clone.bits = (long[])this.bits.Clone();
            return clone;
        }

        /// <summary>
        /// Returns a deep copy of the receiver; calls <code>clone()</code> and casts the result.
        /// </summary>
        /// <returns>a deep copy of the receiver.</returns>
        public BitVector Copy()
        {
            return (BitVector)Clone();
        }

        /// <summary>
        /// You normally need not use this methodd Use this method only if performance is criticald 
        /// Returns the bit vector's backing bits.
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the returned array directly via the [] operator, be sure you know what you're doing.
        ///
        /// <p>A bitvector is modelled as a long array, i.ed <i>long[] bits</i> holds bits of a bitvector.
        /// Each long value holds 64 bits.
        /// The i-th bit is stored in bits[i/64] at
        /// bit position i % 64 (where bit position 0 refers to the least
        /// significant bit and 63 refers to the most significant bit).
        /// </summary>
        public long[] Elements
        {
            get
            {
                return bits;
            }
        }

        /// <summary>
        /// You normally need not use this methodd Use this method only if performance is criticald 
        /// Sets the bit vector's backing bits and size.
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        ///
        /// <p>A bitvector is modelled as a long array, i.ed <i>long[] bits</i> holds bits of a bitvector.
        /// Each long value holds 64 bits.
        /// The i-th bit is stored in bits[i/64] at
        /// bit position i % 64 (where bit position 0 refers to the least
        /// significant bit and 63 refers to the most significant bit).
        /// </summary>
        /// <param name="bits">the backing bits of the bit vector.</param>
        /// <param name="size">the number of bits the bit vector shall hold.</param>
        /// <exception cref="ArgumentException">if <i>size &lt; 0 || size &gt; bits.Length*64</i>.</exception>
        public void SetElements(long[] bits, int size)
        {
            if (size < 0 || size > bits.Length * QuickBitVector.BITS_PER_UNIT) throw new ArgumentException();
            this.bits = bits;
            this.nbits = size;
        }

        /// <summary>
        /// Compares this object against the specified object.
        /// The result is <code>true</code> if and only if the argument is 
        /// not <code>null</code> and is a <code>BitVector</code> object
        /// that has the same size as the receiver and 
        /// the same bits set to <code>true</code> as the receiver.
        /// That is, for every nonnegative <code>int</code> index <code>k</code>, 
        /// <pre>((BitVector)obj).Get(k) == this.Get(k)</pre>
        /// must be true.
        /// </summary>
        /// <param name="obj">the object to compare with.</param>
        /// <returns><code>true</code> if the objects are the same;
        ///          <code>false</code> otherwise.</returns>
        public override Boolean Equals(Object obj)
        {
            if (obj == null || !(obj is BitVector))
                return false;
            if (this == obj)
                return true;

            BitVector other = (BitVector)obj;
            if (Size != other.Size) return false;

            int fullUnits = NumberOfFullUnits();
            // perform logical comparison on full units
            for (int i1 = fullUnits; --i1 >= 0;)
                if (bits[i1] != other.bits[i1]) return false;

            // perform logical comparison on remaining bits
            int i2 = fullUnits * QuickBitVector.BITS_PER_UNIT;
            for (int times = NumberOfBitsInPartialUnit(); --times >= 0;)
            {
                if (Get(i2) != other.Get(i2)) return false;
                i2++;
            }

            return true;
        }

        /// <summary>
        /// Applies a procedure to each bit index within the specified range that holds a bit in the given state.
        /// Starts at index <i>from</i>, moves rightwards to <i>to</i>.
        /// Useful, for example, if you want to copy bits into an image or somewhere else.
        /// <p>
        /// Optimized for speedd Particularly quick if one of the following conditions holds
        /// <ul>
        /// <li><i>state==true</i> and the receiver is sparse (<i>cardinality()</i> is small compared to <i>Size</i>).
        /// <li><i>state==false</i> and the receiver is dense (<i>cardinality()</i> is large compared to <i>Size</i>).
        /// </ul>
        /// </summary>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <param name="state">element to search for.</param>
        /// <param name="foundPos">the index that found</param>
        /// <param name="procedure">a procedure object taking as argument the current bit indexd Stops iteration if the procedure returns <i>false</i>, otherwise continuesd </param>
        /// <returns><i>false</i> if the procedure stopped before all elements where iterated over, <i>true</i> otherwised</returns>
        /// <exception cref="IndexOutOfRangeException">if (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>).</exception>
        public Boolean ForEachIndexFromToInState(int from, int to, Boolean state, ref int foundPos, Cern.Colt.Function.IntProcedureDelegate procedure)
        {
            /*
            // this version is equivalent to the low level version below, but about 100 times slower for large ranges.
            if (nbits==0) return true;
            checkRangeFromTo(from, to, nbits);
            long[] theBits = this.bits; // cached for speed
            int Length=to-from+1;
            for (int i=from; --Length >= 0; i++) {
                if (QuickBitVector.Get(theBits,i)==state) {
                    if (!function(i)) return false;
                }
            }
            return true;
            */


            /*
             * This low level implementation exploits the fact that for any full unit one can determine in O(1)
             * whether it contains at least one true bit,
             * and whether it contains at least one false bit.
             * Thus, 64 bits can often be skipped with one simple comparison if the vector is either sparse or dense.
             *
             * However, careful coding must be done for leading and/or trailing units which are not entirely contained in the query range.
             */

            if (nbits == 0) return true;
            CheckRangeFromTo(from, to, nbits);
            //Console.WriteLine("\n");
            //Console.WriteLine(this);
            //Console.WriteLine("from="+from+", to="+to+", bit="+state);

            // Cache some vars for speed.
            long[] theBits = this.bits;
            int bitsPerUnit = QuickBitVector.BITS_PER_UNIT;

            // Prepare
            int fromUnit = QuickBitVector.Unit(from);
            int toUnit = QuickBitVector.Unit(to);
            int i = from; // current bitvector index

            // Iterate over the leading partial unit, if any.
            int bitIndex = QuickBitVector.Offset(from);
            int partialWidth;
            if (bitIndex > 0)
            { // There exists a leading partial unit.
                partialWidth = System.Math.Min(to - from + 1, bitsPerUnit - bitIndex);
                //Console.WriteLine("partialWidth1="+partialWidth);
                for (; --partialWidth >= 0; i++)
                {
                    if (QuickBitVector.Get(theBits, i) == state)
                    {
                        foundPos = i;
                        if (!procedure(i)) return false;
                    }
                }
                fromUnit++; // leading partial unit is done.
            }

            if (i > to) return true; // done

            // If there is a trailing partial unit, then there is one full unit less to test.
            bitIndex = QuickBitVector.Offset(to);
            if (bitIndex < bitsPerUnit - 1)
            {
                toUnit--; // trailing partial unit needs to be tested extra.
                partialWidth = bitIndex + 1;
            }
            else
            {
                partialWidth = 0;
            }
            //Console.WriteLine("partialWidth2="+partialWidth);

            // Iterate over all full units, if any.
            // (It does not matter that iterating over partial units is a little bit slow,
            // the only thing that matters is that iterating over full units is quickd)
            long comparator;
            if (state) comparator = 0L;
            else comparator = ~0L; // all 64 bits set

            //Console.WriteLine("fromUnit="+fromUnit+", toUnit="+toUnit);
            for (int unit = fromUnit; unit <= toUnit; unit++)
            {
                long val = theBits[unit];
                if (val != comparator)
                {
                    // at least one element within current unit matches.
                    // iterate over all bits within current unit.
                    if (state)
                    {
                        for (int j = 0, k = bitsPerUnit; --k >= 0; i++)
                        {
                            if ((val & (1L << j++)) != 0L)
                            { // is bit set?
                                foundPos = i;
                                if (!procedure(i)) return false;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0, k = bitsPerUnit; --k >= 0; i++)
                        {
                            if ((val & (1L << j++)) == 0L)
                            { // is bit cleared?
                                foundPos = i;
                                if (!procedure(i)) return false;
                            }
                        }
                    }
                }
                else
                {
                    i += bitsPerUnit;
                }
            }

            //Console.WriteLine("trail with i="+i);	

            // Iterate over trailing partial unit, if any.
            for (; --partialWidth >= 0; i++)
            {
                if (QuickBitVector.Get(theBits, i) == state)
                {
                    foundPos = i;
                    if (!procedure(i)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns from the bitvector the value of the bit with the specified index.
        /// The value is <i>true</i> if the bit with the index <i>bitIndex</i> 
        /// is currently set; otherwise, returns <i>false</i>.
        /// </summary>
        /// <param name="bitIndex">the bit index.</param>
        /// <returns>the value of the bit with the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>bitIndex&lt;0 || bitIndex&gt;=Size</i></exception>
        public Boolean Get(int bitIndex)
        {
            return this[bitIndex];
        }

        /// <summary>
        /// Returns a long value representing bits of the receiver from index <i>from</i> to index <i>to</i>.
        /// Bits are returned as a long value with the return value having bit 0 set to bit <code>from</code>, .d, bit <code>to-from</code> set to bit <code>to</code>.
        /// All other bits of the return value are set to 0.
        /// If <i>to-from+1==0</i> then returns zero (<i>0L</i>).
        /// </summary>
        /// <param name="from">index of start bit (inclusive).</param>
        /// <param name="to">index of end bit (inclusive).</param>
        /// <returns>the specified bits as long value.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>from&lt;0 || from&gt;=Size || to&lt;0 || to&gt;=Size || to-from+1&lt;0 || to-from+1>64</i></exception>
        public long GetLongFromTo(int from, int to)
        {
            int width = to - from + 1;
            if (width == 0) return 0L;
            if (from < 0 || from >= nbits || to < 0 || to >= nbits || width < 0 || width > QuickBitVector.BITS_PER_UNIT) throw new IndexOutOfRangeException("from:" + from + ", to:" + to);
            return QuickBitVector.GetLongFromTo(bits, from, to);
        }

        /// <summary>
        /// Returns from the bitvector the value of the bit with the specified index; <b>WARNING:</b> Does not check preconditions.
        /// The value is <i>true</i> if the bit with the index <i>bitIndex</i> 
        /// is currently set; otherwise, returns <i>false</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid values without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <i>bitIndex &gt;= 0 && bitIndex &lt; Size</i>.
        /// </summary>
        /// <param name="bitIndex">the bit index.</param>
        /// <returns>the value of the bit with the specified index.</returns>
        public Boolean GetQuick(int bitIndex)
        {
            return QuickBitVector.Get(bits, bitIndex);
        }

        /// <summary>
        /// Returns a hash code value for the receiverd The hash code 
        /// depends only on which bits have been set within the receiver.
        /// The algorithm used to compute it may 
        /// be described as follows.<p>
        /// Suppose the bits in the receiver were to be stored 
        /// in an array of <code>long</code> ints called, say, 
        /// <code>bits</code>, in such a manner that bit <code>k</code> is 
        /// set in the receiver (for nonnegative values of 
        /// <code>k</code>) if and only if the expression 
        /// <pre>((k&gt;&gt;6) &lt; bits.Length) && ((bits[k&gt;&gt;6] & (1L &lt;&lt; (bit & 0x3F))) != 0)</pre>
        /// is trued Then the following definition of the <code>hashCode</code> 
        /// method would be a correct implementation of the actual algorithm:
        /// <pre>
        /// public override int GetHashCode() {
        ///      long h = 1234;
        ///      for (int i = bits.Length; --i &gt;= 0; ) {
        ///           h ^= bits[i] * (i + 1);
        ///      }
        ///      return (int)((h &gt;&gt; 32) ^ h);
        /// }</pre>
        /// Note that the hash code values change if the set of bits is altered.
        /// </summary>
        /// <returns>a hash code value for the receiver.</returns>
        public override int GetHashCode()
        {
            long h = 1234;
            for (int i = bits.Length; --i >= 0;) h ^= bits[i] * (i + 1);

            return (int)((h >> 32) ^ h);
        }

        /// <summary>
        /// Returns the index of the first occurrence of the specified
        /// stated Returns <code>-1</code> if the receiver does not contain this state.
        /// Searches between <code>from</code>, inclusive and <code>to</code>, inclusive.
        /// <p>
        /// Optimized for speedd Preliminary performance (200Mhz Pentium Pro, JDK 1.2, NT): size=10^6, from=0, to=size-1, receiver contains matching state in the very end --> 0.002 seconds elapsed time.
        /// </summary>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <param name="state">state to search for.</param>
        /// <returns>the index of the first occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.</returns>
        /// <exception cref="IndexOutOfRangeException">if (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>).</exception>
        public int IndexOfFromTo(int from, int to, Boolean state)
        {
            var indexProcedure = new Cern.Colt.Function.IntProcedureDelegate((a) => { return false; });
            int foundPos = 0;
            ForEachIndexFromToInState(from, to, state, ref foundPos, indexProcedure);
            return foundPos;
        }

        /// <summary>
        /// Performs a logical <b>NOT</b> on the bits of the receiver (A = ~A).
        /// </summary>
        public void Not()
        {
            long[] theBits = this.bits;
            for (int i = theBits.Length; --i >= 0;) theBits[i] = ~theBits[i];
        }

        /// <summary>
        /// Returns the number of bits used in the trailing PARTIAL unit.
        /// Returns zero if there is no such trailing partial unit.
        /// </summary>
        /// <returns></returns>
        public int NumberOfBitsInPartialUnit()
        {
            return QuickBitVector.Offset(nbits);
        }

        /// <summary>
        /// Returns the number of units that are FULL (not PARTIAL).
        /// </summary>
        /// <returns></returns>
        public int NumberOfFullUnits()
        {
            return QuickBitVector.Unit(nbits);
        }

        /// <summary>
        /// Performs a logical <b>OR</b> of the receiver with another bit vector (A = A | B).
        /// The receiver is modified so that a bit in it has the
        /// value <code>true</code> if and only if it either already had the 
        /// value <code>true</code> or the corresponding bit in the other bit vector
        /// argument has the value <code>true</code>.
        /// </summary>
        /// <param name="other">a bit vector.</param>
        /// <exception cref="ArgumentException">if <i>Size &gt; other.Count</i>.</exception>
        public void Or(BitVector other)
        {
            if (this == other) return;
            CheckSize(other);
            long[] theBits = this.bits; // cached for speed.
            long[] otherBits = other.bits; //cached for speed.
            for (int i = theBits.Length; --i >= 0;) theBits[i] |= otherBits[i];
        }

        /// <summary>
        /// Constructs and returns a new bit vector which is a copy of the given range.
        /// The new bitvector has <i>Size==to-from+1</i>.
        /// </summary>
        /// <param name="from">the start index within the receiver, inclusive.</param>
        /// <param name="to">the end index within the receiver, inclusive.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">if <i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size))</i>.</exception>
        public BitVector PartFromTo(int from, int to)
        {
            if (nbits == 0 || to == from - 1) return new BitVector(0);
            CheckRangeFromTo(from, to, nbits);

            int width = to - from + 1;
            BitVector part = new BitVector(width);
            part.ReplaceFromToWith(0, width - 1, this, from);
            return part;
        }

        /// <summary>
        /// Sets the bit with index <i>bitIndex</i> to the state specified by <i>value</i>.
        /// </summary>
        /// <param name="bitIndex">the index of the bit to be changed.</param>
        /// <param name="value">the value to be stored in the bit.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>bitIndex&lt;0 || bitIndex&gt;=Size</i></exception>
        public void Put(int bitIndex, Boolean value)
        {
            this[bitIndex] = value;
        }

        /// <summary>
        /// Sets bits of the receiver from index <code>from</code> to index <code>to</code> to the bits of <code>value</code>.
        /// Bit <code>from</code> is set to bit 0 of <code>value</code>, .d, bit <code>to</code> is set to bit <code>to-from</code> of <code>value</code>.
        /// All other bits stay unaffected.
        /// If <i>to-from+1==0</i> then does nothing.
        /// </summary>
        /// <param name="value">the value to be copied into the receiver.</param>
        /// <param name="from">index of start bit (inclusive).</param>
        /// <param name="to">index of end bit (inclusive).</param>
        /// <exception cref="IndexOutOfRangeException">if <i>from&lt;0 || from&gt;=Size || to&lt;0 || to&gt;=Size || to-from+1&lt;0 || to-from+1>64</i>.</exception>
        public void PutLongFromTo(long value, int from, int to)
        {
            int width = to - from + 1;
            if (width == 0) return;
            if (from < 0 || from >= nbits || to < 0 || to >= nbits || width < 0 || width > QuickBitVector.BITS_PER_UNIT) throw new IndexOutOfRangeException("from:" + from + ", to:" + to);
            QuickBitVector.PutLongFromTo(bits, value, from, to);
        }

        /// <summary>
        /// Sets the bit with index <i>bitIndex</i> to the state specified by <i>value</i>; <b>WARNING:</b> Does not check preconditions.
        ///
        /// <p>Provided with invalid parameters this method may set invalid values without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <i>bitIndex &gt;= 0 && bitIndex &lt; Size</i>.
        /// </summary>
        /// <param name="bitIndex">the index of the bit to be changed.</param>
        /// <param name="value">the value to be stored in the bit.</param>
        public void PutQuick(int bitIndex, Boolean value)
        {
            if (value)
                QuickBitVector.Set(bits, bitIndex);
            else
                QuickBitVector.Clear(bits, bitIndex);
        }

        /// <summary>
        /// Replaces the bits of the receiver in the given range with the bits of another bit vector.
        /// Replaces the range <i>[from,to]</i> with the contents of the range <i>[sourceFrom,sourceFrom+to-from]</i>, all inclusive.
        /// If <i>source==this</i> and the source and destination range intersect in an ambiguous way, then replaces as if using an intermediate auxiliary copy of the receiver.
        /// <p>
        /// Optimized for speedd Preliminary performance (200Mhz Pentium Pro, JDK 1.2, NT): replace 10^6 ill aligned bits --> 0.02 seconds elapsed time.
        /// </summary>
        /// <param name="from">the start index within the receiver, inclusive.</param>
        /// <param name="to">the end index within the receiver, inclusive.</param>
        /// <param name="source">the source bitvector to copy from.</param>
        /// <param name="sourceFrom">the start index within <i>source</i>, inclusive.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size || sourceFrom&lt;0 || sourceFrom+to-from+1>source.Count))</i>.</exception>
        public void ReplaceFromToWith(int from, int to, BitVector source, int sourceFrom)
        {
            if (nbits == 0 || to == from - 1) return;
            CheckRangeFromTo(from, to, nbits);
            int Length = to - from + 1;
            if (sourceFrom < 0 || sourceFrom + Length > source.Size)
            {
                throw new IndexOutOfRangeException();
            }

            if (source.bits == this.bits && from <= sourceFrom && sourceFrom <= to)
            { // dangerous intersection
                source = source.Copy();
            }

            long[] theBits = this.bits; // cached for speed.
            long[] sourceBits = source.bits; // cached for speed.

            /* 
            This version is equivalent to the version below but 20 times slower..
            for (int i=from; --Length >= 0; i++, sourceFrom++) {
                QuickBitVector.Put(theBits,i,QuickBitVector.Get(sourceBits,sourceFrom));
            }
            */

            // Low level implementation for speed.
            // This could be done even faster by implementing on even lower levelsd But then the code would probably become a "don't touch" piece.
            int width = to - from + 1;
            int blocks = QuickBitVector.Unit(width); // width/64
            int bitsPerUnit = QuickBitVector.BITS_PER_UNIT;
            int bitsPerUnitMinusOne = bitsPerUnit - 1;

            // copy entire 64 bit blocks, if any.
            for (int i = blocks; --i >= 0;)
            {
                long val1 = QuickBitVector.GetLongFromTo(sourceBits, sourceFrom, sourceFrom + bitsPerUnitMinusOne);
                QuickBitVector.PutLongFromTo(theBits, val1, from, from + bitsPerUnitMinusOne);
                sourceFrom += bitsPerUnit;
                from += bitsPerUnit;
            }

            // copy trailing bits, if any.
            int offset = QuickBitVector.Offset(width); //width%64
            long val2 = QuickBitVector.GetLongFromTo(sourceBits, sourceFrom, sourceFrom + offset - 1);
            QuickBitVector.PutLongFromTo(theBits, val2, from, from + offset - 1);
        }

        /// <summary>
        /// Sets the bits in the given range to the state specified by <i>value</i>.
        /// <p>
        /// Optimized for speedd Preliminary performance (200Mhz Pentium Pro, JDK 1.2, NT): replace 10^6 ill aligned bits --> 0.002 seconds elapsed time.
        /// </summary>
        /// <param name="from">the start index, inclusive.</param>
        /// <param name="to">the end index, inclusive.</param>
        /// <param name="value">the value to be stored in the bits of the range.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>.</exception>
        public void ReplaceFromToWith(int from, int to, Boolean value)
        {
            if (nbits == 0 || to == from - 1) return;
            CheckRangeFromTo(from, to, nbits);
            long[] theBits = this.bits; // cached for speed

            int fromUnit = QuickBitVector.Unit(from);
            int fromOffset = QuickBitVector.Offset(from);
            int toUnit = QuickBitVector.Unit(to);
            int toOffset = QuickBitVector.Offset(to);
            int bitsPerUnit = QuickBitVector.BITS_PER_UNIT;

            long filler;
            if (value) filler = ~0L;
            else filler = 0L;

            int bitIndex = from;
            if (fromUnit == toUnit)
            { // only one unit to do
                QuickBitVector.PutLongFromTo(theBits, filler, bitIndex, bitIndex + to - from);
                //slower: for (; bitIndex<=to; ) QuickBitVector.Put(theBits,bitIndex++,value);
                return;
            }

            // treat leading partial unit, if any.
            if (fromOffset > 0)
            { // fix by Olivier Janssens
                QuickBitVector.PutLongFromTo(theBits, filler, bitIndex, bitIndex + bitsPerUnit - fromOffset);
                bitIndex += bitsPerUnit - fromOffset + 1;
                /* slower:
                for (int i=bitsPerUnit-fromOffset; --i >= 0; ) {
                    QuickBitVector.Put(theBits,bitIndex++,value);
                }*/
                fromUnit++;
            }
            if (toOffset < bitsPerUnit - 1) toUnit--; // there is a trailing partial unit

            // treat full units, if any.
            for (int i = fromUnit; i <= toUnit;) theBits[i++] = filler;
            if (fromUnit <= toUnit) bitIndex += (toUnit - fromUnit + 1) * bitsPerUnit;

            // treat trailing partial unit, if any.
            if (toOffset < bitsPerUnit - 1)
            {
                QuickBitVector.PutLongFromTo(theBits, filler, bitIndex, to);
                /* slower:
                for (int i=toOffset+1; --i >= 0; ) {
                    QuickBitVector.Put(theBits,bitIndex++,value);
                }*/
            }
        }

        /// <summary>
        /// Changes the bit with index <i>bitIndex</i> to the "set" (<i>true</i>) state.
        /// </summary>
        /// <param name="bitIndex">the index of the bit to be set.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>bitIndex&lt;0 || bitIndex&gt;=Size</i></exception>
        public void Set(int bitIndex)
        {
            if (bitIndex < 0 || bitIndex >= nbits) throw new IndexOutOfRangeException(bitIndex.ToString());
            QuickBitVector.Set(bits, bitIndex);
        }

        /// <summary>
        /// Returns a string representation of the receiverd For every index 
        /// for which the receiver contains a bit in the "set" (<i>true</i>)
        /// state, the decimal representation of that index is included in 
        /// the resultd Such indeces are listed in order from lowest to 
        /// highest, separated by ",&nbsp;" (a comma and a space) and 
        /// surrounded by braces.
        /// </summary>
        /// <returns>a string representation of this bit vector.</returns>
        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder(nbits);
            String separator = "";
            buffer.Append('{');

            for (int i = 0; i < nbits; i++)
            {
                if (Get(i))
                {
                    buffer.Append(separator);
                    separator = ", ";
                    buffer.Append(i);
                }
            }

            buffer.Append('}');
            return buffer.ToString();
        }

        /// <summary>
        /// Performs a logical <b>XOR</b> of the receiver with another bit vector (A = A ^ B).
        /// The receiver is modified so that a bit in it has the
        /// value <code>true</code> if and only if one of the following statements holds:
        /// <ul>
        /// <li>The bit initially has the value <code>true</code>, and the 
        ///     corresponding bit in the argument has the value <code>false</code>.
        /// <li>The bit initially has the value <code>false</code>, and the 
        ///     corresponding bit in the argument has the value <code>true</code>d 
        /// </ul>
        /// </summary>
        /// <param name="other">a bit vector.</param>
        /// <exception cref="ArgumentException">if <i>Size &gt; other.Count</i>.</exception>
        public void Xor(BitVector other)
        {
            CheckSize(other);
            long[] theBits = this.bits; // cached for speed.
            long[] otherBits = other.bits; //cached for speed.
            for (int i = theBits.Length; --i >= 0;) theBits[i] ^= otherBits[i];
        }
    }
}
