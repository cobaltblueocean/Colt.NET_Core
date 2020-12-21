// <copyright file="BooleanArrayList.cs" company="CERN">
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
using Cern.Colt.Function;

namespace Cern.Colt.List
{
    /// <summary>
    /// Resizable list holding<code> Boolean</code> _elements; implemented with arrays.
    /// First see the<a href="package-summary.html"> package summary</a> and javadoc<a href="package-tree.html"> tree view</a> to get the broad picture.
    /// <summary>
    public class BooleanArrayList : AbstractBooleanList
    {
        /// <summary>
        /// The array buffer into which the _elements of the list are stored.
        /// The capacity of the list is the Length of this array buffer.
        /// @serial
        /// <summary>
        private Boolean[] _elements;

        Boolean[] Elements
        {
            get { return _elements; }
            set { _elements = value; }
        }

        public override int Count => Size;

        public override bool IsReadOnly => false;

        /// <summary>
        /// Constructs an empty list.
        /// <summary>
        public BooleanArrayList() : this(10)
        {

        }
        /// <summary>
        /// Constructs a list containing the specified _elementsd
        /// The initial Size and capacity of the list is the Length of the array.
        /// 
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <param name="_elements">the array to be backed by the the constructed list</param>
        public BooleanArrayList(Boolean[] elements)
        {
            _elements = elements;
        }
        /// <summary>
        /// Constructs an empty list with the specified initial capacity.
        /// 
        /// <summary>
        /// <param name=""> initialCapacity   the number of _elements the receiver can hold without auto-expanding itself by allocating new internal memory.</param>
        public BooleanArrayList(int initialCapacity) : this(new Boolean[initialCapacity])
        {
            SetSizeRaw(0);
        }
        /// <summary>
        /// Appends the specified element to the end of this list.
        /// 
        /// <summary>
        /// <param name="element">element to be appended to this list.</param>
        public override void Add(Boolean element)
        {
            // overridden for performance only.
            if (Size == _elements.Length)
            {
                EnsureCapacity(Size + 1);
            }
            _elements[Size++] = element;
        }
        /// <summary>
        /// Inserts the specified element before the specified position into the receiverd
        /// Shifts the element currently at that position (if any) and
        /// any subsequent _elements to the right.
        /// 
        /// <summary>
        /// <param name="index">index before which the specified element is to be inserted (must be in [0,Size]).</param>
        /// <param name="element">element to be inserted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>index &lt; 0 || index &gt; Size</i>). </exception>
        public override void BeforeInsert(int index, Boolean element)
        {
            // overridden for performance only.
            if (index > Size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + Size);
            EnsureCapacity(Size + 1);
            Array.Copy(_elements, index, _elements, index + 1, Size - index);
            _elements[index] = element;
            Size++;
        }

        /// <summary>
        /// Returns a deep copy of the receiverd
        /// 
        /// <summary>
        /// <returns> a deep copy of the receiver.</returns>
        //public Object Clone()
        //{
        //    // overridden for performance only.
        //    BooleanArrayList clone = new BooleanArrayList((Boolean[])_elements.Clone());
        //    clone.setSizeRaw(Size);
        //    return clone;
        //}

        /// <summary>
        /// Returns a deep copy of the receiver; uses <code>clone()</code> and casts the result.
        /// 
        /// <summary>
        /// <returns> a deep copy of the receiver.</returns>
        public BooleanArrayList Copy()
        {
            return (BooleanArrayList)Clone();
        }
        /// <summary>
        /// Sorts the specified range of the receiver into ascending numerical order (<i>false &lt; true</i>)d
        /// 
        /// The sorting algorithm is a count sortd This algorithm offers guaranteed
        /// O(n) performance without auxiliary memory.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        public void CountSortFromTo(int from, int to)
        {
            if (Size == 0) return;
            CheckRangeFromTo(from, to, Size);

            Boolean[] the_elements = _elements;
            int trues = 0;
            for (int i = from; i <= to;) if (the_elements[i++]) trues++;

            int falses = to - from + 1 - trues;
            if (falses > 0) FillFromToWith(from, from + falses - 1, false);
            if (trues > 0) FillFromToWith(from + falses, from + falses - 1 + trues, true);
        }
        /// <summary>
        /// Returns the _elements currently stored, including invalid _elements between Size and capacity, if any.
        /// 
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the returned array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <returns>the _elements currently stored.</returns>
        public override Boolean[] GetElements()
        {
            return _elements;
        }
        /// <summary>
        /// Sets the receiver's _elements to be the specified array (not a copy of it).
        /// 
        /// The Size and capacity of the list is the Length of the array.
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <param name="_elements">the new _elements to be stored.</param>
        /// <returns>the receiver itself.</returns>
        public override AbstractBooleanList SetElements(Boolean[] _elements)
        {
            this._elements = _elements;
            this.Size = _elements.Length;
            return this;
        }
        /// <summary>
        /// Ensures that the receiver can hold at least the specified number of _elements without needing to allocate new internal memory.
        /// If necessary, allocates new internal memory and increases the capacity of the receiver.
        /// 
        /// <summary>
        /// <param name=""> minCapacity   the desired minimum capacity.</param>
        public override void EnsureCapacity(int minCapacity)
        {
            _elements = _elements.EnsureCapacity(minCapacity);
        }
        /// <summary>
        /// Compares the specified Object with the receiverd
        /// Returns true if and only if the specified Object is also an ArrayList of the same type, both Lists have the
        /// same Size, and all corresponding pairs of _elements in the two Lists are identical.
        /// In other words, two Lists are defined to be equal if they contain the
        /// same _elements in the same order.
        /// 
        /// <summary>
        /// <param name="otherObj">the Object to be compared for equality with the receiver.</param>
        /// <returns>true if the specified Object is equal to the receiver.</returns>
        public override Boolean Equals(Object otherObj)
        { //delta
          // overridden for performance only.
            if (!(otherObj is BooleanArrayList)) return base.Equals(otherObj);
            if (this == otherObj) return true;
            if (otherObj == null) return false;
            BooleanArrayList other = (BooleanArrayList)otherObj;
            if (Size != other.Size) return false;

            Boolean[] the_elements = GetElements();
            Boolean[] other_elements = other.ToArray();
            for (int i = Size; --i >= 0;)
            {
                if (the_elements[i] != other_elements[i]) return false;
            }
            return true;
        }
        /// <summary>
        /// Applies a procedure to each element of the receiver, if any.
        /// Starts at index 0, moving rightwards.
        /// <summary>
        /// <param name="procedure">   the procedure to be appliedd Stops iteration if the procedure returns <i>false</i>, otherwise continuesd</param>
        /// <returns><i>false</i> if the procedure stopped before all _elements where iterated over, <i>true</i> otherwised</returns>
        public override Boolean ForEach(BooleanProcedure procedure)
        {
            // overridden for performance only.
            Boolean[] the_elements = _elements;
            int theSize = Size;

            for (int i = 0; i < theSize;) if (!procedure(the_elements[i++])) return false;
            return true;
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (index </exception>
        /// 		  &lt; 0 || index &gt;= Size).
        [Obsolete("Deprecated, use this[index] property instead.", true)]
        public override Boolean Get(int index)
        {
            // overridden for performance only.
            if (index >= Size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + Size);
            return _elements[index];
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver; <b>WARNING:</b> Does not check preconditionsd
        /// Provided with invalid parameters this method may return invalid _elements without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <i>index &gt;= 0 && index &lt; Size</i>.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        protected override Boolean GetQuick(int index)
        {
            return _elements[index];
        }
        /// <summary>
        /// Returns the index of the first occurrence of the specified
        /// elementd Returns <code>-1</code> if the receiver does not contain this element.
        /// Searches between <code>from</code>, inclusive and <code>to</code>, inclusive.
        /// Tests for identity.
        /// 
        /// <summary>
        /// <param name="element">element to search for.</param>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <returns> the index of the first occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.</returns>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>). </exception>
        public override int IndexOfFromTo(Boolean element, int from, int to)
        {
            // overridden for performance only.
            if (Size == 0) return -1;
            CheckRangeFromTo(from, to, Size);

            Boolean[] the_elements = _elements;
            for (int i = from; i <= to; i++)
            {
                if (element == the_elements[i]) { return i; } //found
            }
            return -1; //not found
        }
        /// <summary>
        /// Returns the index of the last occurrence of the specified
        /// elementd Returns <code>-1</code> if the receiver does not contain this element.
        /// Searches beginning at <code>to</code>, inclusive until <code>from</code>, inclusive.
        /// Tests for identity.
        /// 
        /// <summary>
        /// <param name="element">element to search for.</param>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <returns> the index of the last occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.</returns>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>). </exception>
        public override int LastIndexOfFromTo(Boolean element, int from, int to)
        {
            // overridden for performance only.
            if (Size == 0) return -1;
            CheckRangeFromTo(from, to, Size);

            Boolean[] the_elements = _elements;
            for (int i = to; i >= from; i--)
            {
                if (element == the_elements[i]) { return i; } //found
            }
            return -1; //not found
        }
        /// <summary>
        /// Sorts the specified range of the receiver into ascending order (<i>false &lt; true</i>)d
        /// 
        /// The sorting algorithm is <b>not</b> a mergesort, but rather a countsort.
        /// This algorithm offers guaranteed O(n) performance.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>). </exception>
        public override void MergeSortFromTo(int from, int to)
        {
            CountSortFromTo(from, to);
        }
        /// <summary>
        /// Returns a new list of the part of the receiver between <code>from</code>, inclusive, and <code>to</code>, inclusive.
        /// <summary>
        /// <param name="from">the index of the first element (inclusive).</param>
        /// <param name="to">the index of the last element (inclusive).</param>
        /// <returns>a new list</returns>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>). </exception>
        public override AbstractBooleanList PartFromTo(int from, int to)
        {
            if (Size == 0) return new BooleanArrayList(0);

            CheckRangeFromTo(from, to, Size);

            Boolean[] part = new Boolean[to - from + 1];
            Array.Copy(_elements, from, part, 0, to - from + 1);
            return new BooleanArrayList(part);
        }
        /// <summary>
        /// Sorts the specified range of the receiver into ascending order (<i>false &lt; true</i>)d
        /// 
        /// The sorting algorithm is <b>not</b> a quicksort, but rather a countsort.
        /// This algorithm offers guaranteed O(n) performance.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>). </exception>
        public override void QuickSortFromTo(int from, int to)
        {
            CountSortFromTo(from, to);
        }
        /// <summary>
        /// Removes from the receiver all _elements that are contained in the specified list.
        /// Tests for identity.
        /// 
        /// <summary>
        /// <param name="other">the other list.</param>
        /// <returns><code>true</code> if the receiver changed as a result of the call.</returns>
        public override Boolean RemoveAll(AbstractBooleanList other)
        {
            // overridden for performance only.
            if (!(other is BooleanArrayList)) return base.RemoveAll(other);

            /* There are two possibilities to do the thing
               a) use other.IndexOf(..d)
               b) sort other, then use other.BinarySearch(..d)

               Let's try to figure out which one is fasterd Let M=Size, N=other.Size, then
               a) takes O(M*N) steps
               b) takes O(N*logN + M*logN) steps (sorting is O(N*logN) and binarySearch is O(logN))

               Hence, if N*logN + M*logN < M*N, we use b) otherwise we use a).
            */
            if (other.Size == 0) { return false; } //nothing to do
            int limit = other.Size - 1;
            int j = 0;
            Boolean[] the_elements = _elements;
            int mySize = Size;

            double N = (double)other.Size;
            double M = (double)mySize;
            if ((N + M) * Cern.Jet.Math.Arithmetic.Log2(N) < M * N)
            {
                // it is faster to sort other before searching in it
                BooleanArrayList sortedList = (BooleanArrayList)other.Clone();
                sortedList.QuickSort();

                for (int i = 0; i < mySize; i++)
                {
                    if (sortedList.BinarySearchFromTo(the_elements[i], 0, limit) < 0) the_elements[j++] = the_elements[i];
                }
            }
            else
            {
                // it is faster to search in other without sorting
                for (int i = 0; i < mySize; i++)
                {
                    if (other.IndexOfFromTo(the_elements[i], 0, limit) < 0) the_elements[j++] = the_elements[i];
                }
            }

            Boolean modified = (j != mySize);
            SetSize(j);
            return modified;
        }
        /// <summary>
        /// Replaces a number of _elements in the receiver with the same number of _elements of another list.
        /// Replaces _elements in the receiver, between <code>from</code> (inclusive) and <code>to</code> (inclusive),
        /// with _elements of <code>other</code>, starting from <code>otherFrom</code> (inclusive).
        /// 
        /// <summary>
        /// <param name="from">the position of the first element to be replaced in the receiver</param>
        /// <param name="to">the position of the last element to be replaced in the receiver</param>
        /// <param name="other">list holding _elements to be copied into the receiver.</param>
        /// <param name="otherFrom">position of first element within other list to be copied.</param>
        public override void ReplaceFromToWithFrom(int from, int to, AbstractBooleanList other, int otherFrom)
        {
            // overridden for performance only.
            if (!(other is BooleanArrayList)) {
                // slower
                base.ReplaceFromToWithFrom(from, to, other, otherFrom);
                return;
            }
            int Length = to - from + 1;
            if (Length > 0)
            {
                CheckRangeFromTo(from, to, Size);
                CheckRangeFromTo(otherFrom, otherFrom + Length - 1, other.Size);
                Array.Copy(((BooleanArrayList)other)._elements, otherFrom, _elements, from, Length);
            }
        }
        /// <summary>
        /// Retains (keeps) only the _elements in the receiver that are contained in the specified other list.
        /// In other words, removes from the receiver all of its _elements that are not contained in the
        /// specified other listd
        /// <summary>
        /// <param name="other">the other list to test against.</param>
        /// <returns><code>true</code> if the receiver changed as a result of the call.</returns>
        public override Boolean RetainAll(AbstractBooleanList other)
        {
            // overridden for performance only.
            if (!(other is BooleanArrayList)) return base.RetainAll(other);

            /* There are two possibilities to do the thing
               a) use other.IndexOf(..d)
               b) sort other, then use other.BinarySearch(..d)

               Let's try to figure out which one is fasterd Let M=Size, N=other.Size, then
               a) takes O(M*N) steps
               b) takes O(N*logN + M*logN) steps (sorting is O(N*logN) and binarySearch is O(logN))

               Hence, if N*logN + M*logN < M*N, we use b) otherwise we use a).
            */
            int limit = other.Size - 1;
            int j = 0;
            Boolean[] the_elements = _elements;
            int mySize = Size;

            double N = (double)other.Size;
            double M = (double)mySize;
            if ((N + M) * Cern.Jet.Math.Arithmetic.Log2(N) < M * N)
            {
                // it is faster to sort other before searching in it
                BooleanArrayList sortedList = (BooleanArrayList)other.Clone();
                sortedList.QuickSort();

                for (int i = 0; i < mySize; i++)
                {
                    if (sortedList.BinarySearchFromTo(the_elements[i], 0, limit) >= 0) the_elements[j++] = the_elements[i];
                }
            }
            else
            {
                // it is faster to search in other without sorting
                for (int i = 0; i < mySize; i++)
                {
                    if (other.IndexOfFromTo(the_elements[i], 0, limit) >= 0) the_elements[j++] = the_elements[i];
                }
            }

            Boolean modified = (j != mySize);
            SetSize(j);
            return modified;
        }
        /// <summary>
        /// Reverses the _elements of the receiver.
        /// Last becomes first, second last becomes second first, and so on.
        /// <summary>
        public override void Reverse()
        {
            // overridden for performance only.
            Boolean tmp;
            int limit = Size / 2;
            int j = Size - 1;

            Boolean[] the_elements = _elements;
            for (int i = 0; i < limit;)
            { //swap
                tmp = the_elements[i];
                the_elements[i++] = the_elements[j];
                the_elements[j--] = tmp;
            }
        }
        /// <summary>
        /// Replaces the element at the specified position in the receiver with the specified element.
        /// 
        /// <summary>
        /// <param name="index">index of element to replace.</param>
        /// <param name="element">element to be stored at the specified position.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (index </exception>
        /// 		  &lt; 0 || index &gt;= Size).
        [Obsolete("Deprecated, use this[index] property instead.", true)]
        public override void Set(int index, Boolean element)
        {
            // overridden for performance only.
            if (index >= Size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + Size);
            _elements[index] = element;
        }
        /// <summary>
        /// Replaces the element at the specified position in the receiver with the specified element; <b>WARNING:</b> Does not check preconditions.
        /// Provided with invalid parameters this method may access invalid indexes without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <i>index &gt;= 0 && index &lt; Size</i>.
        /// 
        /// <summary>
        /// <param name="index">index of element to replace.</param>
        /// <param name="element">element to be stored at the specified position.</param>
        protected override void SetQuick(int index, Boolean element)
        {
            _elements[index] = element;
        }
        /// <summary>
        /// Randomly permutes the part of the receiver between <code>from</code> (inclusive) and <code>to</code> (inclusive)d
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be permuted.</param>
        /// <param name="to">the index of the last element (inclusive) to be permuted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>). </exception>
        public override void ShuffleFromTo(int from, int to)
        {
            // overridden for performance only.
            if (Size == 0) { return; }
            CheckRangeFromTo(from, to, Size);

            var gen = new Cern.Jet.Random.Uniform(new Cern.Jet.Random.Engine.DRand(new DateTime()));
            Boolean tmpElement;
            Boolean[] the_elements = _elements;
            int random;
            for (int i = from; i < to; i++)
            {
                random = gen.NextIntFromTo(i, to);

                //swap(i, random)
                tmpElement = the_elements[random];
                the_elements[random] = the_elements[i];
                the_elements[i] = tmpElement;
            }
        }
        /// <summary>
        /// Sorts the specified range of the receiver into ascending orderd
        /// 
        /// The sorting algorithm is countsort.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>). </exception>
        public override void SortFromTo(int from, int to)
        {
            CountSortFromTo(from, to);
        }
        /// <summary>
        /// Trims the capacity of the receiver to be the receiver's current
        /// Sized Releases any basefluos internal memoryd An application can use this operation to minimize the
        /// storage of the receiver.
        /// <summary>
        public override void TrimToSize()
        {
            _elements = _elements.TrimToCapacity(Size);
        }

        public override void Insert(int index, bool item)
        {
            BeforeInsert(index, item);
        }

        public override void CopyTo(bool[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public override IEnumerator<bool> GetEnumerator()
        {
            foreach (var item in _elements)
                yield return item;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
