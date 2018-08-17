// <copyright file="QuickBitVector.cs" company="CERN">
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
    public class QuickBitVector
    {
        public static int ADDRESS_BITS_PER_UNIT = 6; // 64=2^6
        public static int BITS_PER_UNIT = 64; // = 1 << ADDRESS_BITS_PER_UNIT
        public static int BIT_INDEX_MASK = 63; // = BITS_PER_UNIT - 1;

        private static long[] pows = PrecomputePows(); //precompute bitmasks for speed

        /// <summary>
        /// Makes this class non instantiable, but still inheritable.
        /// </summary>
        public QuickBitVector()
        {
        }

        /// <summary>
        /// Returns a bit mask with bits in the specified range set to 1, all the rest set to 0.
        /// In other words, returns a bit mask having 0,1,2,3,..d,64 bits set.
        /// If <i>to-from+1==0</i> then returns zero (<i>0L</i>).
        /// Precondition (not checked): <i>to-from+1 &gt;= 0 && to-from+1 &lt;= 64</i>.
        /// </summary>
        /// <param name="from">index of start bit (inclusive)</param>
        /// <param name="to">index of end bit (inclusive).</param>
        /// <returns>the bit mask having all bits between <i>from</i> and <i>to</i> set to 1.</returns>
        public static long BitMaskWithBitsSetFromTo(int from, int to)
        {
            return pows[to - from + 1] << from;

            // This turned out to be slower:
            // 0xffffffffffffffffL == ~0L == -1L == all 64 bits set.
            // int width;
            // return (width=to-from+1) == 0 ? 0L : (0xffffffffffffffffL >>> (BITS_PER_UNIT-width)) << from;
        }

        /// <summary>
        /// Changes the bit with index <i>bitIndex</i> in the bitvector <i>bits</i> to the "clear" (<i>false</i>) state.
        /// </summary>
        /// <param name="bits">the bitvector.</param>
        /// <param name="bitIndex">the index of the bit to be cleared.</param>
        public static void Clear(long[] bits, int bitIndex)
        {
            bits[bitIndex >> ADDRESS_BITS_PER_UNIT] &= ~(1L << (bitIndex & BIT_INDEX_MASK));
        }

        /// <summary>
        /// Returns from the bitvector the value of the bit with the specified index.
        /// The value is <i>true</i> if the bit with the index <i>bitIndex</i> 
        /// is currently set; otherwise, returns <i>false</i>.
        /// </summary>
        /// <param name="bits">the bitvector.</param>
        /// <param name="bitIndex">the bit index.</param>
        /// <returns>the value of the bit with the specified index.</returns>
        public static Boolean Get(long[] bits, int bitIndex)
        {
            return ((bits[bitIndex >> ADDRESS_BITS_PER_UNIT] & (1L << (bitIndex & BIT_INDEX_MASK))) != 0);
        }

        /// <summary>
        /// Returns a long value representing bits of a bitvector from index <i>from</i> to index <i>to</i>.
        /// Bits are returned as a long value with the return value having bit 0 set to bit <code>from</code>, ..d, bit <code>to-from</code> set to bit <code>to</code>.
        /// All other bits of return value are set to 0.
        /// If <i>from &gt; to</i> then returns zero (<i>0L</i>).
        /// Precondition (not checked): <i>to-from+1 &lt;= 64</i>.
        /// </summary>
        /// <param name="bits">the bitvector.</param>
        /// <param name="from">index of start bit (inclusive).</param>
        /// <param name="to">index of end bit (inclusive).</param>
        /// <returns>the specified bits as long value.</returns>
        public static long GetLongFromTo(long[] bits, int from, int to)
        {
            if (from > to) return 0L;

            int fromIndex = from >> ADDRESS_BITS_PER_UNIT; //equivalent to from/64
            int toIndex = to >> ADDRESS_BITS_PER_UNIT;
            int fromOffset = from & BIT_INDEX_MASK; //equivalent to from%64
            int toOffset = to & BIT_INDEX_MASK;
            //this is equivalent to the above, but slower:
            //int fromIndex=from/BITS_PER_UNIT;
            //int toIndex=to/BITS_PER_UNIT;
            //int fromOffset=from%BITS_PER_UNIT;
            //int toOffset=to%BITS_PER_UNIT;


            long mask;
            if (fromIndex == toIndex)
            { //range does not cross unit boundaries; value to retrieve is contained in one single long value.
                mask = BitMaskWithBitsSetFromTo(fromOffset, toOffset);
                //return (bits[fromIndex] & mask) >>> fromOffset;
                return (Int64.Parse((bits[fromIndex] & mask).ToString()) >> fromOffset);
            }

            //range crosses unit boundaries; value to retrieve is spread over two long values.
            //get part from first long value
            mask = BitMaskWithBitsSetFromTo(fromOffset, BIT_INDEX_MASK);
            //long x1 = (bits[fromIndex] & mask) >>> fromOffset;
            long x1 = (Int64.Parse((bits[fromIndex] & mask).ToString()) >> fromOffset);

            //get part from second long value
            mask = BitMaskWithBitsSetFromTo(0, toOffset);
            long x2 = (bits[toIndex] & mask) << (BITS_PER_UNIT - fromOffset);

            //combine
            return x1 | x2;
        }

        /// <summary>
        /// Returns the index of the least significant bit in state "true".
        /// Returns 32 if no bit is in state "true".
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <example>
        /// 0x80000000 --> 31
        /// 0x7fffffff --> 0
        /// 0x00000001 --> 0
        /// 0x00000000 --> 32
        /// </example>
        public static int LeastSignificantBit(int value)
        {
            int i = -1;
            while (++i < 32 && (((1 << i) & value)) == 0) ;
            return i;
        }

        /// <summary>
        /// Constructs a low level bitvector that holds <i>size</i> elements, with each element taking <i>bitsPerElement</i> bits.
        /// </summary>
        /// <param name="size">the number of elements to be stored in the bitvector (must be &gt;= 0).</param>
        /// <param name="bitsPerElement">the number of bits one single element takes.</param>
        /// <returns>a low level bitvector.</returns>
        public static long[] MakeBitVector(int size, int bitsPerElement)
        {
            int nBits = size * bitsPerElement;
            int unitIndex = (nBits - 1) >> ADDRESS_BITS_PER_UNIT;
            long[] bitVector = new long[unitIndex + 1];
            return bitVector;
        }

        /// <summary>
        /// Returns the index of the most significant bit in state "true".
        /// Returns -1 if no bit is in state "true".
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <example>
        /// 0x80000000 --> 31
        /// 0x7fffffff --> 30
        /// 0x00000001 --> 0
        /// 0x00000000 --> -1
        /// </example>
        public static int MostSignificantBit(int value)
        {
            int i = 32;
            while (--i >= 0 && (((1 << i) & value)) == 0) ;
            return i;
        }

        /// <summary>
        /// Returns the index within the unit that contains the given bitIndex.
        /// </summary>
        /// <param name="bitIndex"></param>
        /// <returns></returns>
        public static int Offset(int bitIndex)
        {
            return bitIndex & BIT_INDEX_MASK;
            //equivalent to bitIndex%64
        }

        /// <summary>
        /// Initializes a table with numbers having 1,2,3,..d,64 bits set.
        /// pows[i] has bits [0..i-1] set.
        /// pows[64] == -1L == ~0L == has all 64 bits set --> correct.
        /// to speedup calculations in subsequent methods.
        /// </summary>
        /// <returns></returns>
        private static long[] PrecomputePows()
        {
            long[] pows = new long[BITS_PER_UNIT + 1];
            long value = ~0L;
            for (int i = BITS_PER_UNIT + 1; --i >= 1;)
            {
                //pows[i] = value >>> (BITS_PER_UNIT - i);
                pows[i] = (Int64.Parse(value.ToString()) >> (BITS_PER_UNIT - i));

                //Console.WriteLine((i)+":"+pows[i]);
            }
            pows[0] = 0L;
            //Console.WriteLine((0)+":"+pows[0]);
            return pows;

            //OLD STUFF
            /*
            for (int i=BITS_PER_UNIT+1; --i >= 0; ) {
                pows[i]=value;
                value = value >>> 1;
                Console.WriteLine((i)+":"+pows[i]);
            }
            */

            /*
            long[] pows=new long[BITS_PER_UNIT];
            for (int i=0; i<BITS_PER_UNIT-1; i++) {
                pows[i]=System.Math.Round(System.Math.Pow(2.0,i+1))-1;
                Console.WriteLine((i)+":"+pows[i]);
            }
            pows[BITS_PER_UNIT-1] = ~0L;
            Console.WriteLine((BITS_PER_UNIT-1)+":"+pows[BITS_PER_UNIT-1]);
            return pows;
            */
        }

        /// <summary>
        /// Sets the bit with index <i>bitIndex</i> in the bitvector <i>bits</i> to the state specified by <i>value</i>.
        /// </summary>
        /// <param name="bits">the bitvector.</param>
        /// <param name="bitIndex">the index of the bit to be changed.</param>
        /// <param name="value">the value to be stored in the bit.</param>
        public static void Put(long[] bits, int bitIndex, Boolean value)
        {
            if (value)
                Set(bits, bitIndex);
            else
                Clear(bits, bitIndex);
        }

        /// <summary>
        /// Sets bits of a bitvector from index<code> from</code> to index <code>to</code> to the bits of<code> value</code>.
        /// Bit<code> from</code> is set to bit 0 of<code> value</code>, ..d, bit<code> to</code> is set to bit<code> to-from</code> of <code>value</code>.
        /// All other bits stay unaffected.
        /// If<i> from &gt; to</i> then does nothing.
        /// Precondition (not checked): <i>to-from+1 &lt;= 64</i>.
        /// </summary>
        /// <param name="bits">the bitvector.</param>
        /// <param name="value">the value to be copied into the bitvector.</param>
        /// <param name="from">index of start bit (inclusive).</param>
        /// <param name="to">index of end bit (inclusive).</param>
        public static void PutLongFromTo(long[] bits, long value, int from, int to)
        {
            if (from > to) return;

            int fromIndex = from >> ADDRESS_BITS_PER_UNIT; //equivalent to from/64
            int toIndex = to >> ADDRESS_BITS_PER_UNIT;
            int fromOffset = from & BIT_INDEX_MASK; //equivalent to from%64
            int toOffset = to & BIT_INDEX_MASK;
            /*
            this is equivalent to the above, but slower:
            int fromIndex=from/BITS_PER_UNIT;
            int toIndex=to/BITS_PER_UNIT;
            int fromOffset=from%BITS_PER_UNIT;	
            int toOffset=to%BITS_PER_UNIT;
            */

            //make sure all unused bits to the left are cleared.
            long mask;
            mask = BitMaskWithBitsSetFromTo(to - from + 1, BIT_INDEX_MASK);
            long cleanValue = value & (~mask);

            long shiftedValue;

            if (fromIndex == toIndex)
            { //range does not cross unit boundaries; should go into one single long value.
                shiftedValue = cleanValue << fromOffset;
                mask = BitMaskWithBitsSetFromTo(fromOffset, toOffset);
                bits[fromIndex] = (bits[fromIndex] & (~mask)) | shiftedValue;
                return;

            }

            //range crosses unit boundaries; value should go into two long values.
            //copy into first long value.
            shiftedValue = cleanValue << fromOffset;
            mask = BitMaskWithBitsSetFromTo(fromOffset, BIT_INDEX_MASK);
            bits[fromIndex] = (bits[fromIndex] & (~mask)) | shiftedValue;

            //copy into second long value.
            //shiftedValue = cleanValue >>> (BITS_PER_UNIT - fromOffset);
            shiftedValue = (Int64.Parse(cleanValue.ToString()) >> (BITS_PER_UNIT - fromOffset));

            mask = BitMaskWithBitsSetFromTo(0, toOffset);
            bits[toIndex] = (bits[toIndex] & (~mask)) | shiftedValue;
        }

        /// <summary>
        /// Changes the bit with index <i>bitIndex</i> in the bitvector <i>bits</i> to the "set" (<i>true</i>) state.
        /// </summary>
        /// <param name="bits">the bitvector.</param>
        /// <param name="bitIndex">the index of the bit to be set.</param>
        public static void Set(long[] bits, int bitIndex)
        {
            bits[bitIndex >> ADDRESS_BITS_PER_UNIT] |= 1L << (bitIndex & BIT_INDEX_MASK);
        }

        /// <summary>
        /// Returns the index of the unit that contains the given bitIndex.
        /// </summary>
        /// <param name="bitIndex"></param>
        /// <returns></returns>
        public static int Unit(int bitIndex)
        {
            return bitIndex >> ADDRESS_BITS_PER_UNIT;
            //equivalent to bitIndex/64
        }
    }
}
