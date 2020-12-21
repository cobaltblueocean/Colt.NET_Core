// <copyright file=".cs" company="CERN">
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
    /// Resizable list holding <code>int</code> elements; implemented with arrays.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// </summary>
    public class FloatArrayList : AbstractFloatList
    {
        /// <summary>
        /// The array buffer into which the elements of the list are stored.
        /// The capacity of the list is the Length of this array buffer.
        /// @serial
        /// <summary>
        private float[] _elements;

        public override float[] Elements
        {
            get { return _elements; }
            set
            {
                _elements = value;
                this.Size = _elements.Length;
            }
        }

        public override int Size { 
            set
            {
                base.SetSizeRaw(value);
            }
        }

        public override int Count => Size;

        public override bool IsReadOnly => false;

        /// <summary>
        /// Constructs an empty list.
        /// <summary>
        public FloatArrayList() : this(10)
        {

        }
        /// <summary>
        /// Constructs a list containing the specified elementsd
        /// The initial Size and capacity of the list is the Length of the array.
        /// 
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <param name="elements">the array to be backed by the the constructed list</param>
        public FloatArrayList(float[] elements)
        {
            Elements = elements;
        }
        /// <summary>
        /// Constructs a list containing the specified elementsd
        /// The initial Size and capacity of the list is the Length of the array.
        /// 
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <param name="elements">the array to be backed by the the constructed list</param>
        public FloatArrayList(List<float> elements)
        {
            Elements = elements.ToArray();
        }
        /// <summary>
        /// Constructs an empty list with the specified initial capacity.
        /// 
        /// <summary>
        /// <param name=""> initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.</param>
        public FloatArrayList(int initialCapacity) : this(new float[initialCapacity])
        {
            SetSizeRaw(0);
        }
        /// <summary>
        /// Appends the specified element to the end of this list.
        /// 
        /// <summary>
        /// <param name="element">element to be appended to this list.</param>
        public override void Add(float element)
        {
            // overridden for performance onlyd  
            if (Size == _elements.Length) EnsureCapacity(Size + 1);
            _elements[Size++] = element;
        }
        /// <summary>
        /// Inserts the specified element before the specified position into the receiverd
        /// Shifts the element currently at that position (if any) and
        /// any subsequent elements to the right.
        /// 
        /// <summary>
        /// <param name="index">index before which the specified element is to be inserted (must be in [0,Size]).</param>
        /// <param name="element">element to be inserted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>index &lt; 0 || index &gt; Size</i>). </exception>
        public override void BeforeInsert(int index, float element)
        {
            // overridden for performance only.
            if (Size == index)
            {
                Add(element);
                return;
            }
            if (index > Size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + Size);
            EnsureCapacity(Size + 1);
            Array.Copy(_elements, index, _elements, index + 1, Size - index);
            _elements[index] = element;
            Size++;
        }
        /// <summary>
        /// Searches the receiver for the specified value using
        /// the binary search algorithmd  The receiver must <strong>must</strong> be
        /// sorted (as by the sort method) prior to making this calld  If
        /// it is not sorted, the results are undefined: in particular, the call
        /// may enter an infinite loopd  If the receiver contains multiple elements
        /// equal to the specified object, there is no guarantee which instance
        /// will be found.
        /// 
        /// <summary>
        /// <param name="key">the value to be searched for.</param>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <returns>index of the search key, if it is contained in the receiver;</returns>
        ///	       otherwise, <i>(-(<i>insertion point</i>) - 1)</i>d  The <i>insertion
        ///	       point</i> is defined as the the point at which the value would
        /// 	       be inserted into the receiver: the index of the first
        ///	       element greater than the key, or <i>receiver.Count</i>, if all
        ///	       elements in the receiver are less than the specified keyd  Note
        ///	       that this guarantees that the return value will be &gt;= 0 if
        ///	       and only if the key is found.
        /// <see cref="Cern.Colt.Sorting"></see>
        /// <see cref="java.util.Arrays"></see>
        public override int BinarySearchFromTo(float key, int from, int to)
        {
            return Cern.Colt.Sorting.BinarySearchFromTo(_elements, key, from, to);
        }
        /// <summary>
        /// Returns a deep copy of the receiverd
        /// 
        /// <summary>
        /// <returns> a deep copy of the receiver.</returns>
        public override Object Clone()
        {
            // overridden for performance only.
            FloatArrayList clone = new FloatArrayList((float[])_elements.Clone());
            clone.SetSizeRaw(Size);
            return clone;
        }
        /// <summary>
        /// Returns a deep copy of the receiver; uses <code>clone()</code> and casts the result.
        /// 
        /// <summary>
        /// <returns> a deep copy of the receiver.</returns>
        public FloatArrayList Copy()
        {
            return (FloatArrayList)Clone();
        }
        /// <summary>
        /// Returns the elements currently stored, including invalid elements between Size and capacity, if any.
        /// 
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the returned array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <returns>the elements currently stored.</returns>
        public override float[] GetElements()
        {
            return _elements;
        }
        /// <summary>
        /// Sets the receiver's elements to be the specified array (not a copy of it).
        /// 
        /// The Size and capacity of the list is the Length of the array.
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <param name="elements">the new elements to be stored.</param>
        /// <returns>the receiver itself.</returns>
        public override AbstractFloatList SetElements(float[] elements)
        {
            this._elements = elements;
            this.Size = elements.Length;
            return this;
        }
        /// <summary>
        /// Ensures that the receiver can hold at least the specified number of elements without needing to allocate new internal memory.
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
        /// same Size, and all corresponding pairs of elements in the two Lists are identical.
        /// In other words, two Lists are defined to be equal if they contain the
        /// same elements in the same order.
        /// 
        /// <summary>
        /// <param name="otherObj">the Object to be compared for equality with the receiver.</param>
        /// <returns>true if the specified Object is equal to the receiver.</returns>
        public override Boolean Equals(Object otherObj)
        { //delta
          // overridden for performance only.
            if (!(otherObj is FloatArrayList)) return base.Equals(otherObj);
            if (this == otherObj) return true;
            if (otherObj == null) return false;
            var other = (FloatArrayList)otherObj;
            if (Size != other.Size) return false;

            var theElements = GetElements();
            var otherElements = other.ToArray();
            for (int i = Size; --i >= 0;)
            {
                if (theElements[i] != otherElements[i]) return false;
            }
            return true;
        }
        /// <summary>
        /// Applies a procedure to each element of the receiver, if any.
        /// Starts at index 0, moving rightwards.
        /// <summary>
        /// <param name="procedure">   the procedure to be appliedd Stops iteration if the procedure returns <i>false</i>, otherwise continuesd</param>
        /// <returns><i>false</i> if the procedure stopped before all elements where iterated over, <i>true</i> otherwised</returns>
        public override Boolean ForEach(FloatProcedure procedure)
        {
            // overridden for performance only.
            var theElements = _elements;
            int theSize = Size;

            for (int i = 0; i < theSize;) if (!procedure(theElements[i++])) return false;
            return true;
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (index </exception>
        /// 		  &lt; 0 || index &gt;= Size).
        public override float Get(int index)
        {
            // overridden for performance only.
            if (index >= Size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + Size);
            return _elements[index];
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver; <b>WARNING:</b> Does not check preconditionsd
        /// Provided with invalid parameters this method may return invalid elements without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <i>index &gt;= 0 && index &lt; Size</i>.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        protected override float GetQuick(int index)
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
        public override int IndexOfFromTo(float element, int from, int to)
        {
            // overridden for performance only.
            if (Size == 0) return -1;
            CheckRangeFromTo(from, to, Size);

            var theElements = _elements;
            for (int i = from; i <= to; i++)
            {
                if (element == theElements[i]) { return i; } //found
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
        public override int LastIndexOfFromTo(int element, int from, int to)
        {
            // overridden for performance only.
            if (Size == 0) return -1;
            CheckRangeFromTo(from, to, Size);

            var theElements = _elements;
            for (int i = to; i >= from; i--)
            {
                if (element == theElements[i]) { return i; } //found
            }
            return -1; //not found
        }
        /// <summary>
        /// Returns a new list of the part of the receiver between <code>from</code>, inclusive, and <code>to</code>, inclusive.
        /// <summary>
        /// <param name="from">the index of the first element (inclusive).</param>
        /// <param name="to">the index of the last element (inclusive).</param>
        /// <returns>a new list</returns>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>Size&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size)</i>). </exception>
        public override AbstractFloatList PartFromTo(int from, int to)
        {
            if (Size == 0) return new FloatArrayList(0);

            CheckRangeFromTo(from, to, Size);

            var part = new float[to - from + 1];
            Array.Copy(_elements, from, part, 0, to - from + 1);
            return new FloatArrayList(part);
        }
        /// <summary>
        /// Removes from the receiver all elements that are contained in the specified list.
        /// Tests for identity.
        /// 
        /// <summary>
        /// <param name="other">the other list.</param>
        /// <returns><code>true</code> if the receiver changed as a result of the call.</returns>
        public override Boolean RemoveAll(AbstractFloatList other)
        {
            // overridden for performance only.
            if (!(other is FloatArrayList)) return base.RemoveAll(other);

            /* There are two possibilities to do the thing
               a) use other.IndexOf(..d)
               b) sort other, then use other.BinarySearch(..d)

               Let's try to figure out which one is fasterd Let M=Size, N=other.Size, then
               a) takes O(M*N) steps
               b) takes O(N*logN + M*logN) steps (sorting is O(N*logN) and binarySearch is O(logN))

               Hence, if N*logN + M*logN < M*N, we use b) otherwise we use a).
            */
            if (other.Size == 0) { return false; } //nothing to do
            var limit = other.Size - 1;
            int j = 0;
            var theElements = _elements;
            int mySize = Size;

            int N = (int)other.Size;
            int M = (int)mySize;
            if ((N + M) * Cern.Jet.Math.Arithmetic.Log2(N) < M * N)
            {
                // it is faster to sort other before searching in it
                FloatArrayList sortedList = (FloatArrayList)other.Clone();
                sortedList.QuickSort();

                for (int i = 0; i < mySize; i++)
                {
                    if (sortedList.BinarySearchFromTo(theElements[i], 0, limit) < 0) theElements[j++] = theElements[i];
                }
            }
            else
            {
                // it is faster to search in other without sorting
                for (int i = 0; i < mySize; i++)
                {
                    if (other.IndexOfFromTo(theElements[i], 0, limit) < 0) theElements[j++] = theElements[i];
                }
            }

            Boolean modified = (j != mySize);
            SetSize(j);
            return modified;
        }
        /// <summary>
        /// Replaces a number of elements in the receiver with the same number of elements of another list.
        /// Replaces elements in the receiver, between <code>from</code> (inclusive) and <code>to</code> (inclusive),
        /// with elements of <code>other</code>, starting from <code>otherFrom</code> (inclusive).
        /// 
        /// <summary>
        /// <param name="from">the position of the first element to be replaced in the receiver</param>
        /// <param name="to">the position of the last element to be replaced in the receiver</param>
        /// <param name="other">list holding elements to be copied into the receiver.</param>
        /// <param name="otherFrom">position of first element within other list to be copied.</param>
        public override void ReplaceFromToWithFrom(int from, int to, AbstractFloatList other, int otherFrom)
        {
            // overridden for performance only.
            if (!(other is FloatArrayList))
            {
                // slower
                base.ReplaceFromToWithFrom(from, to, other, otherFrom);
                return;
            }
            int Length = to - from + 1;
            if (Length > 0)
            {
                CheckRangeFromTo(from, to, Size);
                CheckRangeFromTo(otherFrom, otherFrom + Length - 1, other.Size);
                Array.Copy(((FloatArrayList)other).Elements, otherFrom, _elements, from, Length);
            }
        }
        /// <summary>
        /// Retains (keeps) only the elements in the receiver that are contained in the specified other list.
        /// In other words, removes from the receiver all of its elements that are not contained in the
        /// specified other listd
        /// <summary>
        /// <param name="other">the other list to test against.</param>
        /// <returns><code>true</code> if the receiver changed as a result of the call.</returns>
        public override Boolean RetainAll(AbstractFloatList other)
        {
            // overridden for performance only.
            if (!(other is FloatArrayList)) return base.RetainAll(other);

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
            var theElements = _elements;
            int mySize = Size;

            int N = (int)other.Size;
            int M = (int)mySize;
            if ((N + M) * Cern.Jet.Math.Arithmetic.Log2(N) < M * N)
            {
                // it is faster to sort other before searching in it
                FloatArrayList sortedList = (FloatArrayList)other.Clone();
                sortedList.QuickSort();

                for (int i = 0; i < mySize; i++)
                {
                    if (sortedList.BinarySearchFromTo(theElements[i], 0, limit) >= 0) theElements[j++] = theElements[i];
                }
            }
            else
            {
                // it is faster to search in other without sorting
                for (int i = 0; i < mySize; i++)
                {
                    if (other.IndexOfFromTo(theElements[i], 0, limit) >= 0) theElements[j++] = theElements[i];
                }
            }

            Boolean modified = (j != mySize);
            SetSize(j);
            return modified;
        }
        /// <summary>
        /// Reverses the elements of the receiver.
        /// Last becomes first, second last becomes second first, and so on.
        /// <summary>
        public override void Reverse()
        {
            // overridden for performance only.
            float tmp;
            int limit = Size / 2;
            int j = Size - 1;

            var theElements = _elements;
            for (int i = 0; i < limit;)
            { //swap
                tmp = theElements[i];
                theElements[i++] = theElements[j];
                theElements[j--] = tmp;
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
        public override void Set(int index, float element)
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
        protected override void SetQuick(int index, float element)
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
            float tmpElement;
            var theElements = _elements;
            int random;
            for (int i = from; i < to; i++)
            {
                random = gen.NextIntFromTo(i, to);

                //swap(i, random)
                tmpElement = theElements[random];
                theElements[random] = theElements[i];
                theElements[i] = tmpElement;
            }
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

        public override void Insert(int index, float item)
        {
            BeforeInsert(index, item);
        }

        public override void CopyTo(float[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public override IEnumerator<float> GetEnumerator()
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
