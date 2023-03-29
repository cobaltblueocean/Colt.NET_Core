// <copyright file="AbstractCharList.cs" company="CERN">
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
using Cern.Colt.Buffer;
using Cern.Colt.Function;

namespace Cern.Colt.List
{
    /// <summary>
    /// Abstract base class for resizable lists holding <code>int</code> elements; abstract.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// <summary>
    public abstract class AbstractCharList : AbstractList<Char>
    {
        /// <summary>
        /// The _size of the list.
        /// This is a READ_ONLY variable for all methods but setSizeRaw(int newSize) !!!
        /// If you violate this principle in subclasses, you should exactly know what you are doing.
        /// @serial
        /// 
        /// <summary>
        protected int _size;
        /// <summary>
        /// Returns the number of elements contained in the receiver.
        /// 
        /// <summary>
        /// @returns  the number of elements contained in the receiver.</returns>
        public override int Size
        {
            get { return _size; }
        }

        public virtual Char[] Elements
        {
            get
            {
                var myElements = new Char[_size];
                for (int i = _size; --i >= 0;) myElements[i] = GetQuick(i);
                return myElements;
            }
            set
            {
                Clear();
                AddAllOfFromTo(new CharArrayList(value), 0, value.Length - 1);
            }
        }
        public override Char this[int index]
        {
            get
            {
                if (index >= _size || index < 0)
                    throw new IndexOutOfRangeException("Index: " + index + ", Size: " + _size);
                return GetQuick(index);
            }
            set
            {
                if (index >= _size || index < 0)
                    throw new IndexOutOfRangeException("Index: " + index + ", Size: " + _size);
                SetQuick(index, value);
            }
        }

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// <summary>
        protected AbstractCharList() { }
        /// <summary>
        /// Appends the specified element to the end of this list.
        /// 
        /// <summary>
        /// <param name="element">element to be appended to this list.</param>
        public override void Add(Char element)
        {
            BeforeInsert(_size, element);
        }
        /// <summary>
        /// Appends all elements of the specified list to the receiver.
        /// <summary>
        /// <param name="list">the list of which all elements shall be appended.</param>
        public virtual void AddAllOf(CharArrayList other)
        {
            AddAllOfFromTo(other, 0, other.Size - 1);
        }
        /// <summary>
        /// Appends the part of the specified list between <code>from</code> (inclusive) and <code>to</code> (inclusive) to the receiver.
        /// 
        /// <summary>
        /// <param name="other">the list to be added to the receiver.</param>
        /// <param name="from">the index of the first element to be appended (inclusive).</param>
        /// <param name="to">the index of the last element to be appended (inclusive).</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>other.Count&gt;0 && (from&lt;0 || from&gt;to || to&gt;=other.Count)</i>). </exception>
        public virtual void AddAllOfFromTo(AbstractCharList other, int from, int to)
        {
            BeforeInsertAllOfFromTo(_size, other, from, to);
        }
        /// <summary>
        /// Inserts the specified element before the specified position into the receiverd
        /// Shifts the element currently at that position (if any) and
        /// any subsequent elements to the right.
        /// 
        /// <summary>
        /// <param name="index">index before which the specified element is to be inserted (must be in [0,_size]).</param>
        /// <param name="element">element to be inserted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>index &lt; 0 || index &gt; _size()</i>). </exception>
        public virtual void BeforeInsert(int index, Char element)
        {
            BeforeInsertDummies(index, 1);
            Set(index, element);
        }
        /// <summary>
        /// Inserts the part of the specified list between <code>otherFrom</code> (inclusive) and <code>otherTo</code> (inclusive) before the specified position into the receiverd
        /// Shifts the element currently at that position (if any) and
        /// any subsequent elements to the right.
        /// 
        /// <summary>
        /// <param name="index">index before which to insert first element from the specified list (must be in [0,_size])..</param>
        /// <param name="other">list of which a part is to be inserted into the receiver.</param>
        /// <param name="from">the index of the first element to be inserted (inclusive).</param>
        /// <param name="to">the index of the last element to be inserted (inclusive).</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>other.Count&gt;0 && (from&lt;0 || from&gt;to || to&gt;=other.Count)</i>). </exception>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>index &lt; 0 || index &gt; _size()</i>). </exception>
        public virtual void BeforeInsertAllOfFromTo(int index, AbstractCharList other, int from, int to)
        {
            int Length = to - from + 1;
            this.BeforeInsertDummies(index, Length);
            this.ReplaceFromToWithFrom(index, index + Length - 1, other, from);
        }

        /// <summary>
        /// Inserts <i>Length</i> dummy elements before the specified position into the receiverd
        /// Shifts the element currently at that position (if any) and
        /// any subsequent elements to the right.
        /// <b>This method must set the new _size to be <i>_size()+Length</i>.
        /// 
        /// <summary>
        /// <param name="index">index before which to insert dummy elements (must be in [0,_size])..</param>
        /// <param name="Length">number of dummy elements to be inserted.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || index &gt; _size()</i>. </exception>
        protected override void BeforeInsertDummies(int index, int Length)
        {
            if (index > _size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + _size);
            if (Length > 0)
            {
                EnsureCapacity(_size + Length);
                SetSizeRaw(_size + Length);
                ReplaceFromToWithFrom(index + Length, _size - 1, this, index);
            }
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
        /// <returns>index of the search key, if it is contained in the receiver;</returns>
        ///	otherwise, <i>(-(<i>insertion point</i>) - 1)</i>d The<i>insertion
        /// point</i> is defined as the the point at which the value would
        /// be inserted into the receiver: the index of the first
        ///	element greater than the key, or<i> receiver.Count</i>, if all
        /// elements in the receiver are less than the specified keyd  Note
        /// that this guarantees that the return value will be &gt;= 0 if
        ///	and only if the key is found.
        /// <see cref="java.util.Arrays"></see>
        public virtual int BinarySearch(Char key)
        {
            return this.BinarySearchFromTo(key, 0, _size - 1);
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
        ///	otherwise, <i>(-(<i>insertion point</i>) - 1)</i>d The<i>insertion
        /// point</i> is defined as the the point at which the value would
        /// be inserted into the receiver: the index of the first
        ///	element greater than the key, or<i> receiver.Count</i>, if all
        /// elements in the receiver are less than the specified keyd  Note
        /// that this guarantees that the return value will be &gt;= 0 if
        ///	and only if the key is found.
        /// <see cref="java.util.Arrays"></see>
        public virtual int BinarySearchFromTo(Char key, int from, int to)
        {
            int low = from;
            int high = to;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                int midVal = Get(mid);

                if (midVal < key) low = mid + 1;
                else if (midVal > key) high = mid - 1;
                else return mid; // key found
            }
            return -(low + 1);  // key not found.
        }
        /// <summary>
        /// Returns a deep copy of the receiverd
        /// 
        /// <summary>
        /// <returns> a deep copy of the receiver.</returns>
        public override Object Clone()
        {
            return PartFromTo(0, _size - 1);
        }
        /// <summary>
        /// Returns true if the receiver contains the specified element.
        /// 
        /// <summary>
        /// <param name="element">element whose presence in the receiver is to be tested.</param>
        public override Boolean Contains(Char elem)
        {
            return IndexOfFromTo(elem, 0, _size - 1) >= 0;
        }
        /// <summary>
        /// Deletes the first element from the receiver that is identical to the specified element.
        /// Does nothing, if no such matching element is contained.
        /// 
        /// <summary>
        /// <param name="element">the element to be deleted.</param>
        public virtual void Delete(Char element)
        {
            int index = IndexOfFromTo(element, 0, _size - 1);
            if (index >= 0) Remove(index);
        }
        /// <summary>
        /// Returns the elements currently stored, possibly including invalid elements between _size and capacity.
        /// 
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, this method may decide <b>not to copy the array</b>.
        /// So if subsequently you modify the returned array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <returns>the elements currently stored.</returns>
        public virtual Char[] GetElements()
        {
            var myElements = new Char[_size];
            for (int i = _size; --i >= 0;) myElements[i] = GetQuick(i);
            return myElements;
        }
        /// <summary>
        /// Sets the receiver's elements to be the specified array.
        /// The _size and capacity of the list is the Length of the array.
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, this method may decide <b>not to copy the array</b>.
        /// So if subsequently you modify the returned array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <param name="elements">the new elements to be stored.</param>
        /// <returns>the receiver itself.</returns>
        public virtual AbstractCharList SetElements(Char[] elements)
        {
            Clear();
            AddAllOfFromTo(new CharArrayList(elements), 0, elements.Length - 1);
            return this;
        }
        /// <summary>
        /// Ensures that the receiver can hold at least the specified number of elements without needing to allocate new internal memory.
        /// If necessary, allocates new internal memory and increases the capacity of the receiver.
        /// 
        /// <summary>
        /// <param name=""> minCapacity   the desired minimum capacity.</param>
        public abstract void EnsureCapacity(int minCapacity);
        /// <summary>
        /// Compares the specified Object with the receiverd
        /// Returns true if and only if the specified Object is also an ArrayList of the same type, both Lists have the
        /// same _size, and all corresponding pairs of elements in the two Lists are identical.
        /// In other words, two Lists are defined to be equal if they contain the
        /// same elements in the same order.
        /// 
        /// <summary>
        /// <param name="otherObj">the Object to be compared for equality with the receiver.</param>
        /// <returns>true if the specified Object is equal to the receiver.</returns>
        public override Boolean Equals(Object otherObj)
        { //delta
            if (!(otherObj is AbstractCharList)) { return false; }
            if (this == otherObj) return true;
            if (otherObj == null) return false;
            AbstractCharList other = (AbstractCharList)otherObj;
            if (Size != other.Size) return false;

            for (int i = Size; --i >= 0;)
            {
                if (GetQuick(i) != other.GetQuick(i)) return false;
            }
            return true;
        }
        /// <summary>
        /// Sets the specified range of elements in the specified array to the specified value.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be filled with the specified value.</param>
        /// <param name="to">the index of the last element (inclusive) to be filled with the specified value.</param>
        /// <param name="val">the value to be stored in the specified elements of the receiver.</param>
        public virtual void FillFromToWith(int from, int to, Char val)
        {
            CheckRangeFromTo(from, to, this._size);
            for (int i = from; i <= to;) SetQuick(i++, val);
        }
        /// <summary>
        /// Applies a procedure to each element of the receiver, if any.
        /// Starts at index 0, moving rightwards.
        /// <summary>
        /// <param name="procedure">   the procedure to be appliedd Stops iteration if the procedure returns <i>false</i>, otherwise continuesd</param>
        /// <returns><i>false</i> if the procedure stopped before all elements where iterated over, <i>true</i> otherwised</returns>
        public virtual Boolean ForEach(IntProcedureDelegate procedure)
        {
            for (int i = 0; i < _size;) if (!procedure(Get(i++))) return false;
            return true;
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (index </exception>
        /// 		  &lt; 0 || index &gt;= _size()).
        public virtual int Get(int index)
        {
            if (index >= _size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + _size);
            return GetQuick(index);
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver; <b>WARNING:</b> Does not check preconditionsd
        /// Provided with invalid parameters this method may return invalid elements without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <i>index &gt;= 0 && index &lt; _size()</i>.
        /// 
        /// This method is normally only used internally in large loops where bounds are explicitly checked before the loop and need no be rechecked within the loop.
        /// However, when desperately, you can give this method <i>public</i> visibility in subclasses.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        protected abstract Char GetQuick(int index);
        /// <summary>
        /// Returns the index of the first occurrence of the specified
        /// elementd Returns <code>-1</code> if the receiver does not contain this element.
        /// 
        /// <summary>
        /// <param name=""> element   the element to be searched for.</param>
        /// <returns> the index of the first occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.</returns>
        public override int IndexOf(Char element)
        { //delta
            return IndexOfFromTo(element, 0, _size - 1);
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
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=_size())</i>). </exception>
        public virtual int IndexOfFromTo(int element, int from, int to)
        {
            CheckRangeFromTo(from, to, _size);

            for (int i = from; i <= to; i++)
            {
                if (element == GetQuick(i)) return i; //found
            }
            return -1; //not found
        }
        /// <summary>
        /// Returns the index of the last occurrence of the specified
        /// elementd Returns <code>-1</code> if the receiver does not contain this element.
        /// 
        /// <summary>
        /// <param name=""> element   the element to be searched for.</param>
        /// <returns> the index of the last occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.</returns>
        public virtual int LastIndexOf(int element)
        {
            return LastIndexOfFromTo(element, 0, _size - 1);
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
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=_size())</i>). </exception>
        public virtual int LastIndexOfFromTo(int element, int from, int to)
        {
            CheckRangeFromTo(from, to, Size);

            for (int i = to; i >= from; i--)
            {
                if (element == GetQuick(i)) return i; //found
            }
            return -1; //not found
        }
        /// <summary>
        /// Sorts the specified range of the receiver into ascending orderd
        /// 
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        /// 
        /// <p><b>You should never call this method unless you are sure that this particular sorting algorithm is the right one for your data set.</b>
        /// It is generally better to call <i>sort()</i> or <i>sortFromTo(..d)</i> instead, because those methods automatically choose the best sorting algorithm.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=_size())</i>). </exception>
        public override void MergeSortFromTo(int from, int to)
        {
            int mySize = Size;
            CheckRangeFromTo(from, to, mySize);

            var myElements = GetElements();
            Cern.Colt.Sorting.MergeSort(myElements, from, to + 1);
            SetElements(myElements);
            SetSizeRaw(mySize);
        }
        /// <summary>
        /// Sorts the receiver according
        /// to the order induced by the specified comparatord  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <i>c.CompareTo(e1, e2)</i> must not throw a
        /// <i>ClassCastException</i> for any elements <i>e1</i> and
        /// <i>e2</i> in the range).<p>
        /// 
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        /// 
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be</param>
        ///        sorted.
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <param name="c">the comparator to determine the order of the receiver.</param>
        /// <exception cref="ClassCastException">if the array contains elements that are not </exception>
        ///	       <i>mutually comparable</i> using the specified comparator.
        /// <exception cref="ArgumentException">if <i>fromIndex &gt; toIndex</i> </exception>
        /// <exception cref="ArrayIndexOutOfRangeException">if <i>fromIndex &lt; 0</i> or </exception>
        ///	       <i>toIndex &gt; a.Length</i>
        /// <see cref="Comparator"></see>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=_size())</i>). </exception>
        public virtual void MergeSortFromTo(int from, int to, CharComparatorDelegate c)
        {
            int mySize = Size;
            CheckRangeFromTo(from, to, mySize);

            var myElements = GetElements();
            Cern.Colt.Sorting.MergeSort(myElements, from, to + 1, c);
            SetElements(myElements);
            SetSizeRaw(mySize);
        }
        /// <summary>
        /// Returns a new list of the part of the receiver between <code>from</code>, inclusive, and <code>to</code>, inclusive.
        /// <summary>
        /// <param name="from">the index of the first element (inclusive).</param>
        /// <param name="to">the index of the last element (inclusive).</param>
        /// <returns>a new list</returns>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=_size())</i>). </exception>
        public virtual AbstractCharList PartFromTo(int from, int to)
        {
            CheckRangeFromTo(from, to, _size);

            int Length = to - from + 1;
            var part = new CharArrayList(Length);
            part.AddAllOfFromTo(this, from, to);
            return part;
        }
        /// <summary>
        /// Sorts the specified range of the receiver into
        /// ascending numerical orderd  The sorting algorithm is a tuned quicksort,
        /// adapted from Jon Ld Bentley and Md Douglas McIlroy's "Engineering a
        /// Sort Function", Software-Practice and Experience, Vold 23(11)
        /// Pd 1249-1265 (November 1993)d  This algorithm offers n*log(n)
        /// performance on many data sets that cause other quicksorts to degrade to
        /// quadratic performance.
        /// 
        /// <p><b>You should never call this method unless you are sure that this particular sorting algorithm is the right one for your data set.</b>
        /// It is generally better to call <i>sort()</i> or <i>sortFromTo(..d)</i> instead, because those methods automatically choose the best sorting algorithm.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=_size())</i>). </exception>
        public override void QuickSortFromTo(int from, int to)
        {
            int mySize = Size;
            CheckRangeFromTo(from, to, mySize);

            var myElements = GetElements();
            Array.Sort(myElements, from, to + 1);
            //cern.colt.Sorting.mergeSort(myElements, from, to+1); // TODO just for debugging

            SetElements(myElements);
            SetSizeRaw(mySize);
        }
        /// <summary>
        /// Sorts the receiver according
        /// to the order induced by the specified comparatord  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <i>c.CompareTo(e1, e2)</i> must not throw a
        /// <i>ClassCastException</i> for any elements <i>e1</i> and
        /// <i>e2</i> in the range).<p>
        /// 
        /// The sorting algorithm is a tuned quicksort,
        /// adapted from Jon Ld Bentley and Md Douglas McIlroy's "Engineering a
        /// Sort Function", Software-Practice and Experience, Vold 23(11)
        /// Pd 1249-1265 (November 1993)d  This algorithm offers n*log(n)
        /// performance on many data sets that cause other quicksorts to degrade to
        /// quadratic performance.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be</param>
        ///        sorted.
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <param name="c">the comparator to determine the order of the receiver.</param>
        /// <exception cref="ClassCastException">if the array contains elements that are not </exception>
        ///	       <i>mutually comparable</i> using the specified comparator.
        /// <exception cref="ArgumentException">if <i>fromIndex &gt; toIndex</i> </exception>
        /// <exception cref="ArrayIndexOutOfRangeException">if <i>fromIndex &lt; 0</i> or </exception>
        ///	       <i>toIndex &gt; a.Length</i>
        /// <see cref="Comparator"></see>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=_size())</i>). </exception>
        public virtual void QuickSortFromTo(int from, int to, CharComparatorDelegate c)
        {
            int mySize = Size;
            CheckRangeFromTo(from, to, mySize);

            var myElements = GetElements();
            Cern.Colt.Sorting.QuickSort(myElements, from, to + 1, c);
            SetElements(myElements);
            SetSizeRaw(mySize);
        }
        /// <summary>
        /// Removes from the receiver all elements that are contained in the specified list.
        /// Tests for identity.
        /// 
        /// <summary>
        /// <param name="other">the other list.</param>
        /// <returns><code>true</code> if the receiver changed as a result of the call.</returns>
        public virtual Boolean RemoveAll(AbstractCharList other)
        {
            if (other.Size == 0) return false; //nothing to do
            int limit = other.Size - 1;
            int j = 0;

            for (int i = 0; i < _size; i++)
            {
                if (other.IndexOfFromTo(GetQuick(i), 0, limit) < 0) SetQuick(j++, GetQuick(i));
            }

            Boolean modified = (j != _size);
            SetSize(j);
            return modified;
        }
        /// <summary>
        /// Removes from the receiver all elements whose index is between
        /// <code>from</code>, inclusive and <code>to</code>, inclusived  Shifts any succeeding
        /// elements to the left (reduces their index).
        /// This call shortens the list by <i>(to - from + 1)</i> elements.
        /// 
        /// <summary>
        /// <param name="from">index of first element to be removed.</param>
        /// <param name="to">index of last element to be removed.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=_size())</i>). </exception>
        public override void RemoveFromTo(int from, int to)
        {
            CheckRangeFromTo(from, to, _size);
            int numMoved = _size - to - 1;
            if (numMoved > 0)
            {
                ReplaceFromToWithFrom(from, from - 1 + numMoved, this, to + 1);
                //fillFromToWith(from+numMoved, _size-1, 0.0f); //delta
            }
            int width = to - from + 1;
            if (width > 0) SetSizeRaw(_size - width);
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
        public virtual void ReplaceFromToWithFrom(int from, int to, AbstractCharList other, int otherFrom)
        {
            int Length = to - from + 1;
            if (Length > 0)
            {
                CheckRangeFromTo(from, to, Size);
                CheckRangeFromTo(otherFrom, otherFrom + Length - 1, other.Size);

                // unambiguous copy (it may hold other==this)
                if (from <= otherFrom)
                {
                    for (; --Length >= 0;) SetQuick(from++, other.GetQuick(otherFrom++));
                }
                else
                {
                    int otherTo = otherFrom + Length - 1;
                    for (; --Length >= 0;) SetQuick(to--, other.GetQuick(otherTo--));
                }
            }
        }
        /// <summary>
        /// Replaces the part between <code>from</code> (inclusive) and <code>to</code> (inclusive) with the other list's
        /// part between <code>otherFrom</code> and <code>otherTo</code>d
        /// Powerful (and tricky) method!
        /// Both parts need not be of the same _size (part A can both be smaller or larger than part B).
        /// Parts may overlap.
        /// Receiver and other list may (but most not) be identical.
        /// If <code>from &gt; to</code>, then inserts other part before <code>from</code>.
        /// 
        /// <summary>
        /// <param name="from">the first element of the receiver (inclusive)</param>
        /// <param name="to">the last element of the receiver (inclusive)</param>
        /// <param name="other">the other list (may be identical with receiver)</param>
        /// <param name="otherFrom">the first element of the other list (inclusive)</param>
        /// <param name="otherTo">the last element of the other list (inclusive)</param>
        /// 
        /// <p><b>Examples:</b><pre>
        /// a=[0, 1, 2, 3, 4, 5, 6, 7]
        /// b=[50, 60, 70, 80, 90]
        /// a.R(..d)=a.replaceFromToWithFromTo(..d)
        /// 
        /// a.R(3,5,b,0,4)-->[0, 1, 2, 50, 60, 70, 80, 90, 6, 7]
        /// a.R(1,6,b,0,4)-->[0, 50, 60, 70, 80, 90, 7]
        /// a.R(0,6,b,0,4)-->[50, 60, 70, 80, 90, 7]
        /// a.R(3,5,b,1,2)-->[0, 1, 2, 60, 70, 6, 7]
        /// a.R(1,6,b,1,2)-->[0, 60, 70, 7]
        /// a.R(0,6,b,1,2)-->[60, 70, 7]
        /// a.R(5,3,b,0,4)-->[0, 1, 2, 3, 4, 50, 60, 70, 80, 90, 5, 6, 7]
        /// a.R(5,0,b,0,4)-->[0, 1, 2, 3, 4, 50, 60, 70, 80, 90, 5, 6, 7]
        /// a.R(5,3,b,1,2)-->[0, 1, 2, 3, 4, 60, 70, 5, 6, 7]
        /// a.R(5,0,b,1,2)-->[0, 1, 2, 3, 4, 60, 70, 5, 6, 7]
        /// 
        /// Extreme cases:
        /// a.R(5,3,b,0,0)-->[0, 1, 2, 3, 4, 50, 5, 6, 7]
        /// a.R(5,3,b,4,4)-->[0, 1, 2, 3, 4, 90, 5, 6, 7]
        /// a.R(3,5,a,0,1)-->[0, 1, 2, 0, 1, 6, 7]
        /// a.R(3,5,a,3,5)-->[0, 1, 2, 3, 4, 5, 6, 7]
        /// a.R(3,5,a,4,4)-->[0, 1, 2, 4, 6, 7]
        /// a.R(5,3,a,0,4)-->[0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 5, 6, 7]
        /// a.R(0,-1,b,0,4)-->[50, 60, 70, 80, 90, 0, 1, 2, 3, 4, 5, 6, 7]
        /// a.R(0,-1,a,0,4)-->[0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 5, 6, 7]
        /// a.R(8,0,a,0,4)-->[0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4]
        /// </pre>
        public virtual void ReplaceFromToWithFromTo(int from, int to, AbstractCharList other, int otherFrom, int otherTo)
        {
            if (otherFrom > otherTo)
            {
                throw new IndexOutOfRangeException("otherFrom: " + otherFrom + ", otherTo: " + otherTo);
            }

            if (this == other && to - from != otherTo - otherFrom)
            { // avoid stumbling over my own feet
                ReplaceFromToWithFromTo(from, to, PartFromTo(otherFrom, otherTo), 0, otherTo - otherFrom);
                return;
            }

            int Length = otherTo - otherFrom + 1;
            int diff = Length;
            int theLast = from - 1;

            if (to >= from)
            {
                diff -= (to - from + 1);
                theLast = to;
            }

            if (diff > 0)
            {
                BeforeInsertDummies(theLast + 1, diff);
            }
            else
            {
                if (diff < 0)
                {
                    RemoveFromTo(theLast + diff, theLast - 1);
                }
            }

            if (Length > 0)
            {
                ReplaceFromToWithFrom(from, from + Length - 1, other, otherFrom);
            }
        }
        /// <summary>
        /// Replaces the part of the receiver starting at <code>from</code> (inclusive) with all the elements of the specified collection.
        /// Does not alter the _size of the receiver.
        /// Replaces exactly <i>System.Math.Max(0,System.Math.Min(_size()-from, other.Count))</i> elements.
        /// 
        /// <summary>
        /// <param name="from">the index at which to copy the first element from the specified collection.</param>
        /// <param name="other">Collection to replace part of the receiver</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (index &lt; 0 || index &gt;= _size()). </exception>
        public override void ReplaceFromWith(int from, ICollection<Char> other)
        {
            CheckRange(from, Size);
            var e = other.GetEnumerator();
            int index = from;
            int limit = System.Math.Min(Size - from, other.Count);
            for (int i = 0; i < limit; i++)
                Set(index++, (e.Next())); //delta
        }
        /// <summary>
        /// Retains (keeps) only the elements in the receiver that are contained in the specified other list.
        /// In other words, removes from the receiver all of its elements that are not contained in the
        /// specified other listd
        /// <summary>
        /// <param name="other">the other list to test against.</param>
        /// <returns><code>true</code> if the receiver changed as a result of the call.</returns>
        public virtual Boolean RetainAll(AbstractCharList other)
        {
            if (other.Size == 0)
            {
                if (_size == 0) return false;
                SetSize(0);
                return true;
            }

            int limit = other.Size - 1;
            int j = 0;
            for (int i = 0; i < _size; i++)
            {
                if (other.IndexOfFromTo(GetQuick(i), 0, limit) >= 0) SetQuick(j++, GetQuick(i));
            }

            Boolean modified = (j != _size);
            SetSize(j);
            return modified;
        }
        /// <summary>
        /// Reverses the elements of the receiver.
        /// Last becomes first, second last becomes second first, and so on.
        /// <summary>
        public override void Reverse()
        {
            Char tmp;
            int limit = Size / 2;
            int j = Size - 1;

            for (int i = 0; i < limit;)
            { //swap
                tmp = GetQuick(i);
                SetQuick(i++, GetQuick(j));
                SetQuick(j--, tmp);
            }
        }
        /// <summary>
        /// Replaces the element at the specified position in the receiver with the specified element.
        /// 
        /// <summary>
        /// <param name="index">index of element to replace.</param>
        /// <param name="element">element to be stored at the specified position.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || index &gt;= _size()</i>. </exception>
        public virtual void Set(int index, Char element)
        {
            if (index >= _size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + _size);
            SetQuick(index, element);
        }
        /// <summary>
        /// Replaces the element at the specified position in the receiver with the specified element; <b>WARNING:</b> Does not check preconditions.
        /// Provided with invalid parameters this method may access invalid indexes without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <i>index &gt;= 0 && index &lt; _size()</i>.
        /// 
        /// This method is normally only used internally in large loops where bounds are explicitly checked before the loop and need no be rechecked within the loop.
        /// However, when desperately, you can give this method <i>public</i> visibility in subclasses.
        /// 
        /// <summary>
        /// <param name="index">index of element to replace.</param>
        /// <param name="element">element to be stored at the specified position.</param>
        protected abstract void SetQuick(int index, Char element);
        /// <summary>
        /// Sets the _size of the receiver without modifying it otherwise.
        /// This method should not release or allocate new memory but simply set some instance variable like <i>_size</i>.
        /// 
        /// If your subclass overrides and delegates _size changing methods to some other object,
        /// you must make sure that those overriding methods not only update the _size of the delegate but also of this class.
        /// For example:
        /// public DatabaseList extends AbstractCharList {
        ///    ...
        ///    public void removeFromTo(int from,int to) {
        ///       myDatabase.removeFromTo(from,to);
        ///       this.setSizeRaw(_size-(to-from+1));
        ///    }
        /// }
        /// <summary>
        protected virtual void SetSizeRaw(int newSize)
        {
            _size = newSize;
        }
        /// <summary>
        /// Randomly permutes the part of the receiver between <code>from</code> (inclusive) and <code>to</code> (inclusive)d
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be permuted.</param>
        /// <param name="to">the index of the last element (inclusive) to be permuted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<i>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=_size())</i>). </exception>
        public override void ShuffleFromTo(int from, int to)
        {
            CheckRangeFromTo(from, to, Size);

            Cern.Jet.Random.Uniform gen = new Cern.Jet.Random.Uniform(Cern.Jet.Random.Uniform.MakeDefaultGenerator());
            for (int i = from; i < to; i++)
            {
                int random = gen.NextIntFromTo(i, to);

                //swap(i, random)
                Char tmpElement = GetQuick(random);
                SetQuick(random, GetQuick(i));
                SetQuick(i, tmpElement);
            }
        }
        /// <summary>
        /// Returns a list which is a concatenation of <code>times</code> times the receiver.
        /// <summary>
        /// <param name="times">the number of times the receiver shall be copied.</param>
        public virtual AbstractCharList Times(int times)
        {
            AbstractCharList newList = new CharArrayList(times * Size);
            for (int i = times; --i >= 0;)
            {
                newList.AddAllOfFromTo(this, 0, Size - 1);
            }
            return newList;
        }
        /// <summary>
        /// Returns a <code>java.util.List</code> containing all the elements in the receiver.
        /// <summary>
        public virtual Char[] ToArray()
        {
            int mySize = Size;
            var list = new Char[mySize];
            for (int i = 0; i < mySize; i++) list[i] = this[i];
            return list;
        }
        /// <summary>
        /// Returns a <code>java.util.List</code> containing all the elements in the receiver.
        /// <summary>
        public override List<Char> ToList()
        {
            int mySize = Size;
            var list = new List<Char>(mySize);
            for (int i = 0; i < mySize; i++) list.Add(this[i]);
            return list;
        }
        /// <summary>
        /// Returns a string representation of the receiver, containing
        /// the String representation of each element.
        /// <summary>
        public override String ToString()
        {
            return PartFromTo(0, Size - 1).ToArray().ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
