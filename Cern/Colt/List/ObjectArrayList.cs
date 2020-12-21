// <copyright file="ObjectArrayList.cs" company="CERN">
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
using Cern.Colt.Function;

namespace Cern.Colt.List
{
    /// <summary>
    /// Resizable list holding <code>Object</code> elements; implemented with arrays.
    /// First see the<a href="package-summary.html"> package summary</a> and javadoc<a href="package-tree.html"> tree view</a> to get the broad picture.
    /// </summary>
    public class ObjectArrayList : AbstractList<Object>
    {
        /// <summary>
        /// The array buffer into which the elements of the list are stored.
        /// The capacity of the list is the length of this array buffer.
        /// @serial
        /// <summary>
        private Object[] _elements;

        /// <summary>
        /// The size of the list.
        /// @serial
        /// <summary>
        private int _size;
        /// <summary>
        /// Constructs an empty list.
        /// <summary>


        public Object[] Elements
        {
            get { return _elements; }
            set
            {
                _elements = value;
                this.Size = _elements.Length;
            }
        }

        public override int Size
        {
            get { return _size; }
            set
            {
                base.SetSize(value);
            }
        }

        public override int Count => Size;

        public override bool IsReadOnly => false;

        public override Object this[int index] {
            get { return _elements[index]; }
            set { _elements[index] = value; }
        }

        public ObjectArrayList() : this(10)
        {

        }
        /// <summary>
        /// Constructs a list containing the specified elements.
        /// The initial size and capacity of the list is the length of the array.
        /// 
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <param name="elements">the array to be backed by the the constructed list</param>
        public ObjectArrayList(Object[] elements)
        {
            SetElements(elements);
        }
        /// <summary>
        /// Constructs an empty list with the specified initial capacity.
        /// 
        /// <summary>
        /// <param name=""> initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.</param>
        public ObjectArrayList(int initialCapacity) : this(new Object[initialCapacity])
        {
            _size = 0;
        }
        /// <summary>
        /// Appends the specified element to the end of this list.
        /// 
        /// <summary>
        /// <param name="element">element to be appended to this list.</param>
        public override void Add(Object element)
        {
            if (_size == _elements.Length) EnsureCapacity(_size + 1);
            _elements[_size++] = element;
        }
        /// <summary>
        /// Appends the part of the specified list between <code>from</code> (inclusive) and <code>to</code> (inclusive) to the receiver.
        /// 
        /// <summary>
        /// <param name="other">the list to be added to the receiver.</param>
        /// <param name="from">the index of the first element to be appended (inclusive).</param>
        /// <param name="to">the index of the last element to be appended (inclusive).</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>other.size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=other.size())</tt>). </exception>
        public void AddAllOfFromTo(ObjectArrayList other, int from, int to)
        {
            BeforeInsertAllOfFromTo(_size, other, from, to);
        }
        /// <summary>
        /// Inserts the specified element before the specified position into the receiver.
        /// Shifts the element currently at that position (if any) and
        /// any subsequent elements to the right.
        /// 
        /// <summary>
        /// <param name="index">index before which the specified element is to be inserted (must be in [0,size]).</param>
        /// <param name="element">element to be inserted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>index &lt; 0 || index &gt; size()</tt>). </exception>
        public void BeforeInsert(int index, Object element)
        {
            // overridden for performance only.
            if (index > _size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + _size);
            EnsureCapacity(_size + 1);
            Array.Copy(_elements, index, _elements, index + 1, _size - index);
            _elements[index] = element;
            _size++;
        }
        /// <summary>
        /// Inserts the part of the specified list between <code>otherFrom</code> (inclusive) and <code>otherTo</code> (inclusive) before the specified position into the receiver.
        /// Shifts the element currently at that position (if any) and
        /// any subsequent elements to the right.
        /// 
        /// <summary>
        /// <param name="index">index before which to insert first element from the specified list (must be in [0,size])..</param>
        /// <param name="other">list of which a part is to be inserted into the receiver.</param>
        /// <param name="from">the index of the first element to be inserted (inclusive).</param>
        /// <param name="to">the index of the last element to be inserted (inclusive).</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>other.size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=other.size())</tt>). </exception>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>index &lt; 0 || index &gt; size()</tt>). </exception>
        public void BeforeInsertAllOfFromTo(int index, ObjectArrayList other, int from, int to)
        {
            int length = to - from + 1;
            this.BeforeInsertDummies(index, length);
            this.ReplaceFromToWithFrom(index, index + length - 1, other, from);
        }
        /// <summary>
        /// Inserts length dummies before the specified position into the receiver.
        /// Shifts the element currently at that position (if any) and
        /// any subsequent elements to the right.
        /// 
        /// <summary>
        /// <param name="index">index before which to insert dummies (must be in [0,size])..</param>
        /// <param name="length">number of dummies to be inserted.</param>
        protected override void BeforeInsertDummies(int index, int length)
        {
            if (index > _size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + _size);
            if (length > 0)
            {
                EnsureCapacity(_size + length);
                Array.Copy(_elements, index, _elements, index + length, _size - index);
                _size += length;
            }
        }
        /// <summary>
        /// Searches the receiver for the specified value using
        /// the binary search algorithm. The receiver must be sorted into ascending order
        /// according to the <i>natural ordering</i> of its elements (as by the sort method)
        /// prior to making this call.
        /// If it is not sorted, the results are undefined: in particular, the call
        /// may enter an infinite loop.  If the receiver contains multiple elements
        /// equal to the specified object, there is no guarantee which instance
        /// will be found.
        /// 
        /// <summary>
        /// <param name="key">the value to be searched for.</param>
        /// <returns>index of the search key, if it is contained in the receiver;</returns>
        /// otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
        /// point</i> is defined as the the point at which the value would
        /// 	       be inserted into the receiver: the index of the first
        /// element greater than the key, or <tt>receiver.size()</tt>, if all
        /// elements in the receiver are less than the specified key.  Note
        /// that this guarantees that the return value will be &gt;= 0 if
        /// and only if the key is found.
        /// <see cref="Comparable"></see>
        /// <see cref="java.util.Arrays"></see>
        public int BinarySearch(Object key)
        {
            return this.BinarySearchFromTo(key, 0, _size - 1);
        }
        /// <summary>
        /// Searches the receiver for the specified value using
        /// the binary search algorithm. The receiver must be sorted into ascending order
        /// according to the <i>natural ordering</i> of its elements (as by the sort method)
        /// prior to making this call.
        /// If it is not sorted, the results are undefined: in particular, the call
        /// may enter an infinite loop.  If the receiver contains multiple elements
        /// equal to the specified object, there is no guarantee which instance
        /// will be found.
        /// 
        /// 
        /// <summary>
        /// <param name="key">the value to be searched for.</param>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <returns>index of the search key, if it is contained in the receiver;</returns>
        /// otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
        /// point</i> is defined as the the point at which the value would
        /// 	       be inserted into the receiver: the index of the first
        /// element greater than the key, or <tt>receiver.size()</tt>, if all
        /// elements in the receiver are less than the specified key.  Note
        /// that this guarantees that the return value will be &gt;= 0 if
        /// and only if the key is found.
        /// <see cref="Comparable"></see>
        /// <see cref="java.util.Arrays"></see>
        public int BinarySearchFromTo(Object key, int from, int to)
        {
            int low = from;
            int high = to;

            while (low <= high)
            {
                int mid = (low + high) / 2;
                Object midVal = _elements[mid];
                int cmp = ((IComparable)midVal).CompareTo(key);

                if (cmp < 0) low = mid + 1;
                else if (cmp > 0) high = mid - 1;
                else return mid; // key found
            }
            return -(low + 1);  // key not found.
        }
        /// <summary>
        /// Searches the receiver for the specified value using
        /// the binary search algorithm. The receiver must be sorted into ascending order
        /// according to the specified comparator.  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <tt>c.compare(e1, e2)</tt> must not throw a
        /// <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
        /// <tt>e2</tt> in the range).<p>
        /// 
        /// If the receiver is not sorted, the results are undefined: in particular, the call
        /// may enter an infinite loop.  If the receiver contains multiple elements
        /// equal to the specified object, there is no guarantee which instance
        /// will be found.
        /// 
        /// 
        /// <summary>
        /// <param name="key">the value to be searched for.</param>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <param name="comparator">the comparator by which the receiver is sorted.</param>
        /// <exception cref="ClassCastException">if the receiver contains elements that are not </exception>
        ///	<i>mutually comparable</i> using the specified comparator.
        /// <returns>index of the search key, if it is contained in the receiver;</returns>
        ///	otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
        /// point</i> is defined as the the point at which the value would
        /// 	       be inserted into the receiver: the index of the first
        /// element greater than the key, or <tt>receiver.size()</tt>, if all
        /// elements in the receiver are less than the specified key.  Note
        /// that this guarantees that the return value will be &gt;= 0 if
        /// and only if the key is found.
        /// <see cref="Cern.Colt.Sorting"></see>
        /// <see cref="java.util.Arrays"></see>
        /// <see cref="java.util.Comparator"></see>
        public int BinarySearchFromTo(Object key, int from, int to, ObjectComparator<Object> comparator)
        {
            return Cern.Colt.Sorting.BinarySearchFromTo(this.Elements, key, from, to, comparator);
        }
        /// <summary>
        /// Returns a copy of the receiver such that the copy and the receiver <i>share</i> the same elements, but do not share the same array to index them;
        /// So modifying an object in the copy modifies the object in the receiver and vice versa;
        /// However, structurally modifying the copy (for example changing its size, setting other objects at indexes, etc.) does not affect the receiver and vice versa.
        /// 
        /// <summary>
        /// <returns> a copy of the receiver.</returns>
        public override Object Clone()
        {
            ObjectArrayList v = (ObjectArrayList)base.Clone();
            v.Elements = (Object[])_elements.Clone();
            return v;
        }
        /// <summary>
        /// Returns true if the receiver contains the specified element.
        /// Tests for equality or identity as specified by testForEquality.
        /// 
        /// <summary>
        /// <param name="element">element to search for.</param>
        /// <param name="testForEquality">if true -> test for equality, otherwise for identity.</param>
        public Boolean Contains(Object elem, Boolean testForEquality)
        {
            return IndexOfFromTo(elem, 0, _size - 1, testForEquality) >= 0;
        }
        /// <summary>
        /// Returns a copy of the receiver; call <code>clone()</code> and casts the result.
        /// Returns a copy such that the copy and the receiver <i>share</i> the same elements, but do not share the same array to index them;
        /// So modifying an object in the copy modifies the object in the receiver and vice versa;
        /// However, structurally modifying the copy (for example changing its size, setting other objects at indexes, etc.) does not affect the receiver and vice versa.
        /// 
        /// <summary>
        /// <returns> a copy of the receiver.</returns>
        public ObjectArrayList Copy()
        {
            return (ObjectArrayList)Clone();
        }
        /// <summary>
        /// Deletes the first element from the receiver that matches the specified element.
        /// Does nothing, if no such matching element is contained.
        /// 
        /// Tests elements for equality or identity as specified by <tt>testForEquality</tt>.
        /// When testing for equality, two elements <tt>e1</tt> and
        /// <tt>e2</tt> are <i>equal</i> if <tt>(e1==null ? e2==null :
        /// e1.Equals(e2))</tt>.)
        /// 
        /// <summary>
        /// <param name="testForEquality">if true -> tests for equality, otherwise for identity.</param>
        /// <param name="element">the element to be deleted.</param>
        public void Delete(Object element, Boolean testForEquality)
        {
            int index = IndexOfFromTo(element, 0, _size - 1, testForEquality);
            if (index >= 0) RemoveFromTo(index, index);
        }
        /// <summary>
        /// Sets the receiver's elements to be the specified array (not a copy of it).
        /// 
        /// The _size and capacity of the list is the length of the array.
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        /// 
        /// <summary>
        /// <param name="elements">the new elements to be stored.</param>
        /// <returns>the receiver itself.</returns>
        public ObjectArrayList SetElements(Object[] elements)
        {
            this._elements = elements;
            this._size = elements.Length;
            return this;
        }
        /// <summary>
        /// Ensures that the receiver can hold at least the specified number of elements without needing to allocate new internal memory.
        /// If necessary, allocates new internal memory and increases the capacity of the receiver.
        /// 
        /// <summary>
        /// <param name=""> minCapacity   the desired minimum capacity.</param>
        public void EnsureCapacity(int minCapacity)
        {
            _elements = _elements.EnsureCapacity(minCapacity);
        }
        /// <summary>
        /// Compares the specified Object with the receiver for equality.
        /// Returns true if and only if the specified Object is also an ObjectArrayList, both lists have the
        /// same size, and all corresponding pairs of elements in the two lists are equal.
        /// In other words, two lists are defined to be equal if they contain the
        /// same elements in the same order.
        /// Two elements <tt>e1</tt> and
        /// <tt>e2</tt> are <i>equal</i> if <tt>(e1==null ? e2==null :
        /// e1.Equals(e2))</tt>.)
        /// 
        /// <summary>
        /// <param name="otherObj">the Object to be compared for equality with the receiver.</param>
        /// <returns>true if the specified Object is equal to the receiver.</returns>
        public override Boolean Equals(Object otherObj)
        { //delta
            return Equals(otherObj, true);
        }
        /// <summary>
        /// Compares the specified Object with the receiver for equality.
        /// Returns true if and only if the specified Object is also an ObjectArrayList, both lists have the
        /// same size, and all corresponding pairs of elements in the two lists are the same.
        /// In other words, two lists are defined to be equal if they contain the
        /// same elements in the same order.
        /// Tests elements for equality or identity as specified by <tt>testForEquality</tt>.
        /// When testing for equality, two elements <tt>e1</tt> and
        /// <tt>e2</tt> are <i>equal</i> if <tt>(e1==null ? e2==null :
        /// e1.Equals(e2))</tt>.)
        /// 
        /// <summary>
        /// <param name="otherObj">the Object to be compared for equality with the receiver.</param>
        /// <param name="testForEquality">if true -> tests for equality, otherwise for identity.</param>
        /// <returns>true if the specified Object is equal to the receiver.</returns>
        public Boolean Equals(Object otherObj, Boolean testForEquality)
        { //delta
            if (!(otherObj is ObjectArrayList)) { return false; }
            if (this == otherObj) return true;
            if (otherObj == null) return false;
            ObjectArrayList other = (ObjectArrayList)otherObj;
            if (_elements == other.Elements) return true;
            if (_size != other.Size) return false;

            Object[] otherElements = other.Elements;
            Object[] theElements = _elements;
            if (!testForEquality)
            {
                for (int i = _size; --i >= 0;)
                {
                    if (theElements[i] != otherElements[i]) return false;
                }
            }
            else
            {
                for (int i = _size; --i >= 0;)
                {
                    if (!(theElements[i] == null ? otherElements[i] == null : theElements[i].Equals(otherElements[i]))) return false;
                }
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
        public void FillFromToWith(int from, int to, Object val)
        {
            CheckRangeFromTo(from, to, this._size);
            for (int i = from; i <= to;) SetQuick(i++, val);
        }
        /// <summary>
        /// Applies a procedure to each element of the receiver, if any.
        /// Starts at index 0, moving rightwards.
        /// <summary>
        /// <param name="procedure">   the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues.</param>
        /// <returns><tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise.</returns>
        public Boolean ForEach(ObjectProcedure<Object> procedure)
        {
            Object[] theElements = _elements;
            int theSize = _size;

            for (int i = 0; i < theSize;) if (!procedure(theElements[i++])) return false;
            return true;
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (index &lt; 0 || index &gt;= size()). </exception>
        public Object Get(int index)
        {
            if (index >= _size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + _size);
            return _elements[index];
        }
        /// <summary>
        /// Returns the element at the specified position in the receiver; <b>WARNING:</b> Does not check preconditions.
        /// Provided with invalid parameters this method may return invalid elements without throwing any exception!
        /// <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
        /// Precondition (unchecked): <tt>index &gt;= 0 && index &lt; size()</tt>.
        /// 
        /// <summary>
        /// <param name="index">index of element to return.</param>
        public Object GetQuick(int index)
        {
            return _elements[index];
        }
        /// <summary>
        /// Returns the index of the first occurrence of the specified
        /// element. Returns <code>-1</code> if the receiver does not contain this element.
        /// 
        /// Tests for equality or identity as specified by testForEquality.
        /// 
        /// <summary>
        /// <param name="testForEquality">if <code>true</code> -> test for equality, otherwise for identity.</param>
        /// <returns> the index of the first occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.</returns>
        public int IndexOf(Object element, Boolean testForEquality)
        {
            return this.IndexOfFromTo(element, 0, _size - 1, testForEquality);
        }
        /// <summary>
        /// Returns the index of the first occurrence of the specified
        /// element. Returns <code>-1</code> if the receiver does not contain this element.
        /// Searches between <code>from</code>, inclusive and <code>to</code>, inclusive.
        /// 
        /// Tests for equality or identity as specified by <code>testForEquality</code>.
        /// 
        /// <summary>
        /// <param name="element">element to search for.</param>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <param name="testForEquality">if </code>true</code> -> test for equality, otherwise for identity.</param>
        /// <returns> the index of the first occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.</returns>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public int IndexOfFromTo(Object element, int from, int to, Boolean testForEquality)
        {
            if (_size == 0) return -1;
            CheckRangeFromTo(from, to, _size);

            Object[] theElements = _elements;
            if (testForEquality && element != null)
            {
                for (int i = from; i <= to; i++)
                {
                    if (element.Equals(theElements[i])) { return i; } //found
                }

            }
            else
            {
                for (int i = from; i <= to; i++)
                {
                    if (element == theElements[i]) { return i; } //found
                }
            }
            return -1; //not found
        }
        /// <summary>
        /// Determines whether the receiver is sorted ascending, according to the <i>natural ordering</i> of its
        /// elements.  All elements in this range must implement the
        /// <tt>Comparable</tt> interface.  Furthermore, all elements in this range
        /// must be <i>mutually comparable</i> (that is, <tt>e1.compareTo(e2)</tt>
        /// must not throw a <tt>ClassCastException</tt> for any elements
        /// <tt>e1</tt> and <tt>e2</tt> in the array).<p>
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <returns><tt>true</tt> if the receiver is sorted ascending, <tt>false</tt> otherwise.</returns>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>_size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public Boolean IsSortedFromTo(int from, int to)
        {
            if (_size == 0) return true;
            CheckRangeFromTo(from, to, _size);

            Object[] theElements = _elements;
            for (int i = from + 1; i <= to; i++)
            {
                if (((IComparable)theElements[i]).CompareTo((IComparable)theElements[i - 1]) < 0) return false;
            }
            return true;
        }
        /// <summary>
        /// Returns the index of the last occurrence of the specified
        /// element. Returns <code>-1</code> if the receiver does not contain this element.
        /// Tests for equality or identity as specified by <code>testForEquality</code>.
        /// 
        /// <summary>
        /// <param name=""> element   the element to be searched for.</param>
        /// <param name="testForEquality">if <code>true</code> -> test for equality, otherwise for identity.</param>
        /// <returns> the index of the last occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.</returns>
        public int LastIndexOf(Object element, Boolean testForEquality)
        {
            return LastIndexOfFromTo(element, 0, _size - 1, testForEquality);
        }
        /// <summary>
        /// Returns the index of the last occurrence of the specified
        /// element. Returns <code>-1</code> if the receiver does not contain this element.
        /// Searches beginning at <code>to</code>, inclusive until <code>from</code>, inclusive.
        /// Tests for equality or identity as specified by <code>testForEquality</code>.
        /// 
        /// <summary>
        /// <param name="element">element to search for.</param>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <param name="testForEquality">if <code>true</code> -> test for equality, otherwise for identity.</param>
        /// <returns> the index of the last occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.</returns>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public int LastIndexOfFromTo(Object element, int from, int to, Boolean testForEquality)
        {
            if (_size == 0) return -1;
            CheckRangeFromTo(from, to, _size);

            Object[] theElements = _elements;
            if (testForEquality && element != null)
            {
                for (int i = to; i >= from; i--)
                {
                    if (element.Equals(theElements[i])) { return i; } //found
                }

            }
            else
            {
                for (int i = to; i >= from; i--)
                {
                    if (element == theElements[i]) { return i; } //found
                }
            }
            return -1; //not found
        }
        /// <summary>
        /// Sorts the specified range of the receiver into
        /// ascending order, according to the <i>natural ordering</i> of its
        /// elements.  All elements in this range must implement the
        /// <tt>Comparable</tt> interface.  Furthermore, all elements in this range
        /// must be <i>mutually comparable</i> (that is, <tt>e1.compareTo(e2)</tt>
        /// must not throw a <tt>ClassCastException</tt> for any elements
        /// <tt>e1</tt> and <tt>e2</tt> in the array).<p>
        /// 
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        /// 
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist).  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        /// 
        /// <p><b>You should never call this method unless you are sure that this particular sorting algorithm is the right one for your data set.</b>
        /// It is generally better to call <tt>sort()</tt> or <tt>sortFromTo(...)</tt> instead, because those methods automatically choose the best sorting algorithm.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public override void MergeSortFromTo(int from, int to)
        {
            if (_size == 0) return;
            CheckRangeFromTo(from, to, _size);
            Array.Sort(_elements, from, to + 1);
        }
        /// <summary>
        /// Sorts the receiver according
        /// to the order induced by the specified comparator.  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <tt>c.compare(e1, e2)</tt> must not throw a
        /// <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
        /// <tt>e2</tt> in the range).<p>
        /// 
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        /// 
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist).  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be</param>
        ///        sorted.
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <param name="c">the comparator to determine the order of the receiver.</param>
        /// <exception cref="ClassCastException">if the array contains elements that are not </exception>
        /// <i>mutually comparable</i> using the specified comparator.
        /// <exception cref="IllegalArgumentException">if <tt>fromIndex &gt; toIndex</tt> </exception>
        /// <exception cref="ArrayIndexOutOfRangeException">if <tt>fromIndex &lt; 0</tt> or </exception>
        /// <tt>toIndex &gt; a.length</tt>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        /// <see cref="Comparator"></see>
        public void MergeSortFromTo(int from, int to, IComparer<Object> c)
        {
            if (_size == 0) return;
            CheckRangeFromTo(from, to, _size);
            Array.Sort<Object>(_elements, from, to + 1, c);
        }
        /// <summary>
        /// Returns a new list of the part of the receiver between <code>from</code>, inclusive, and <code>to</code>, inclusive.
        /// <summary>
        /// <param name="from">the index of the first element (inclusive).</param>
        /// <param name="to">the index of the last element (inclusive).</param>
        /// <returns>a new list</returns>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public ObjectArrayList PartFromTo(int from, int to)
        {
            if (_size == 0) return new ObjectArrayList(0);

            CheckRangeFromTo(from, to, _size);

            Object[] part = new Object[to - from + 1];
            Array.Copy(_elements, from, part, 0, to - from + 1);
            return new ObjectArrayList(part);
        }
        /// <summary>
        /// Sorts the specified range of the receiver into
        /// ascending order, according to the <i>natural ordering</i> of its
        /// elements.  All elements in this range must implement the
        /// <tt>Comparable</tt> interface.  Furthermore, all elements in this range
        /// must be <i>mutually comparable</i> (that is, <tt>e1.compareTo(e2)</tt>
        /// must not throw a <tt>ClassCastException</tt> for any elements
        /// <tt>e1</tt> and <tt>e2</tt> in the array).<p>

        /// The sorting algorithm is a tuned quicksort,
        /// adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
        /// Sort Function", Software-Practice and Experience, Vol. 23(11)
        /// P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
        /// performance on many data sets that cause other quicksorts to degrade to
        /// quadratic performance.
        /// 
        /// <p><b>You should never call this method unless you are sure that this particular sorting algorithm is the right one for your data set.</b>
        /// It is generally better to call <tt>sort()</tt> or <tt>sortFromTo(...)</tt> instead, because those methods automatically choose the best sorting algorithm.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public override void QuickSortFromTo(int from, int to)
        {
            if (_size == 0) return;
            CheckRangeFromTo(from, to, _size);
            Cern.Colt.Sorting.QuickSort(_elements, from, to + 1);
        }
        /// <summary>
        /// Sorts the receiver according
        /// to the order induced by the specified comparator.  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <tt>c.compare(e1, e2)</tt> must not throw a
        /// <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
        /// <tt>e2</tt> in the range).<p>

        /// The sorting algorithm is a tuned quicksort,
        /// adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
        /// Sort Function", Software-Practice and Experience, Vol. 23(11)
        /// P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
        /// performance on many data sets that cause other quicksorts to degrade to
        /// quadratic performance.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <param name="c">the comparator to determine the order of the receiver.</param>
        /// <exception cref="ClassCastException">if the array contains elements that are not </exception>
        /// <i>mutually comparable</i> using the specified comparator.
        /// <exception cref="IllegalArgumentException">if <tt>fromIndex &gt; toIndex</tt> </exception>
        /// <exception cref="ArrayIndexOutOfRangeException">if <tt>fromIndex &lt; 0</tt> or </exception>
        /// <tt>toIndex &gt; a.length</tt>
        /// <see cref="Comparator"></see>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public void QuickSortFromTo(int from, int to, ObjectComparator<Object> c)
        {
            if (_size == 0) return;
            CheckRangeFromTo(from, to, _size);
            Cern.Colt.Sorting.QuickSort(_elements, from, to + 1, c);
        }
        /// <summary>
        /// Removes from the receiver all elements that are contained in the specified list.
        /// Tests for equality or identity as specified by <code>testForEquality</code>.
        /// 
        /// <summary>
        /// <param name="other">the other list.</param>
        /// <param name="testForEquality">if <code>true</code> -> test for equality, otherwise for identity.</param>
        /// <returns><code>true</code> if the receiver changed as a result of the call.</returns>
        public Boolean RemoveAll(ObjectArrayList other, Boolean testForEquality)
        {
            if (other._size == 0) return false; //nothing to do
            int limit = other._size - 1;
            int j = 0;
            Object[] theElements = _elements;
            for (int i = 0; i < _size; i++)
            {
                if (other.IndexOfFromTo(theElements[i], 0, limit, testForEquality) < 0) theElements[j++] = theElements[i];
            }

            Boolean modified = (j != _size);
            SetSize(j);
            return modified;
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
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public override void RemoveFromTo(int from, int to)
        {
            CheckRangeFromTo(from, to, _size);
            int numMoved = _size - to - 1;
            if (numMoved >= 0)
            {
                Array.Copy(_elements, to + 1, _elements, from, numMoved);
                FillFromToWith(from + numMoved, _size - 1, null); //delta
            }
            int width = to - from + 1;
            if (width > 0) _size -= width;
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
        public void ReplaceFromToWithFrom(int from, int to, ObjectArrayList other, int otherFrom)
        {
            int length = to - from + 1;
            if (length > 0)
            {
                CheckRangeFromTo(from, to, _size);
                CheckRangeFromTo(otherFrom, otherFrom + length - 1, other._size);
                Array.Copy(other.Elements, otherFrom, _elements, from, length);
            }
        }
        /// <summary>
        /// Replaces the part between <code>from</code> (inclusive) and <code>to</code> (inclusive) with the other list's
        /// part between <code>otherFrom</code> and <code>otherTo</code>.
        /// Powerful (and tricky) method!
        /// Both parts need not be of the same size (part A can both be smaller or larger than part B).
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
        /// a.R(...)=a.replaceFromToWithFromTo(...)
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
        public void ReplaceFromToWithFromTo(int from, int to, ObjectArrayList other, int otherFrom, int otherTo)
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

            int length = otherTo - otherFrom + 1;
            int diff = length;
            int theLast = from - 1;

            //System.out.println("from="+from);
            //System.out.println("to="+to);
            //System.out.println("diff="+diff);

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

            if (length > 0)
            {
                Array.Copy(other.Elements, otherFrom, _elements, from, length);
            }
        }
        /// <summary>
        /// Replaces the part of the receiver starting at <code>from</code> (inclusive) with all the elements of the specified collection.
        /// Does not alter the size of the receiver.
        /// Replaces exactly <tt>Math.max(0,Math.min(size()-from, other.size()))</tt> elements.
        /// 
        /// <summary>
        /// <param name="from">the index at which to copy the first element from the specified collection.</param>
        /// <param name="other">Collection to replace part of the receiver</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (index &lt; 0 || index &gt;= size()). </exception>
        public override void ReplaceFromWith(int from, ICollection<Object> other)
        {
            CheckRange(from, _size);
            var e = other.GetEnumerator();
            int index = from;
            int limit = Math.Min(_size - from, other.Count);
            for (int i = 0; i < limit; i++)
                _elements[index++] = e.Next(); //delta
        }
        /// <summary>
        /// Retains (keeps) only the elements in the receiver that are contained in the specified other list.
        /// In other words, removes from the receiver all of its elements that are not contained in the
        /// specified other list.
        /// Tests for equality or identity as specified by <code>testForEquality</code>.
        /// <summary>
        /// <param name="other">the other list to test against.</param>
        /// <param name="testForEquality">if <code>true</code> -> test for equality, otherwise for identity.</param>
        /// <returns><code>true</code> if the receiver changed as a result of the call.</returns>
        public Boolean RetainAll(ObjectArrayList other, Boolean testForEquality)
        {
            if (other.Size == 0)
            {
                if (_size == 0) return false;
                SetSize(0);
                return true;
            }

            int limit = other.Size - 1;
            int j = 0;
            Object[] theElements = _elements;

            for (int i = 0; i < _size; i++)
            {
                if (other.IndexOfFromTo(theElements[i], 0, limit, testForEquality) >= 0) theElements[j++] = theElements[i];
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
            Object tmp;
            int limit = _size / 2;
            int j = _size - 1;

            Object[] theElements = _elements;
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
        /// 		  &lt; 0 || index &gt;= size()).
        public void Set(int index, Object element)
        {
            if (index >= _size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + _size);
            _elements[index] = element;
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
        public void SetQuick(int index, Object element)
        {
            _elements[index] = element;
        }
        /// <summary>
        /// Randomly permutes the part of the receiver between <code>from</code> (inclusive) and <code>to</code> (inclusive).
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be permuted.</param>
        /// <param name="to">the index of the last element (inclusive) to be permuted.</param>
        /// <exception cref="IndexOutOfRangeException">index is out of range (<tt>size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=size())</tt>). </exception>
        public override void ShuffleFromTo(int from, int to)
        {
            if (_size == 0) return;
            CheckRangeFromTo(from, to, _size);

            var gen = new Cern.Jet.Random.Uniform(new Cern.Jet.Random.Engine.DRand(new DateTime()));
            Object tmpElement;
            Object[] theElements = _elements;
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
        /// Returns a list which is a concatenation of <code>times</code> times the receiver.
        /// <summary>
        /// <param name="times">the number of times the receiver shall be copied.</param>
        public ObjectArrayList Times(int times)
        {
            ObjectArrayList newList = new ObjectArrayList(times * _size);
            for (int i = times; --i >= 0;)
            {
                newList.AddAllOfFromTo(this, 0, _size - 1);
            }
            return newList;
        }
        /// <summary>
        /// Returns an array containing all of the elements in the receiver in the
        /// correct order.  The runtime type of the returned array is that of the
        /// specified array.  If the receiver fits in the specified array, it is
        /// returned therein.  Otherwise, a new array is allocated with the runtime
        /// type of the specified array and the size of the receiver.
        /// <p>
        /// If the receiver fits in the specified array with room to spare
        /// (i.e., the array has more elements than the receiver),
        /// the element in the array immediately following the end of the
        /// receiver is set to null.  This is useful in determining the length
        /// of the receiver <em>only</em> if the caller knows that the receiver
        /// does not contain any null elements.
        /// 
        /// <summary>
        /// <param name="array">the array into which the elements of the receiver are to</param>
        ///	be stored, if it is big enough; otherwise, a new array of the
        /// 		same runtime type is allocated for this purpose.
        /// <returns>an array containing the elements of the receiver.</returns>
        /// <exception cref="ArrayStoreException">the runtime type of <tt>array</tt> is not a supertype </exception>
        /// of the runtime type of every element in the receiver.
        public Object[] ToArray(Object[] array)
        {
            if (array.Length < _size)
                array = new Object[_size];

            Object[] theElements = _elements;
            for (int i = _size; --i >= 0;) array[i] = theElements[i];

            if (array.Length > _size) array[_size] = null;

            return array;
        }
        /// <summary>
        /// Returns a <code>java.util.ArrayList</code> containing all the elements in the receiver.
        /// <summary>
        public override List<Object> ToList()
        {
            int mySize = _size;
            Object[] theElements = _elements;
            var list = new List<Object>(mySize);
            for (int i = 0; i < mySize; i++) list.Add(theElements[i]);
            return list;
        }
        /// <summary>
        /// Returns a string representation of the receiver, containing
        /// the String representation of each element.
        /// <summary>
        public override String ToString()
        {
            return PartFromTo(0, _size - 1).Elements.ToString();
        }
        /// <summary>
        /// Trims the capacity of the receiver to be the receiver's current
        /// size. Releases any superfluos internal memory. An application can use this operation to minimize the
        /// storage of the receiver.
        /// <summary>
        public override void TrimToSize()
        {
            _elements = _elements.TrimToCapacity(_size);
        }

        public override int IndexOf(object elem)
        {
            return this.IndexOf(elem, true);
        }

        public override void Insert(int index, object item)
        {
            BeforeInsert(index, item);
        }

        public override bool Contains(object elem)
        {
            return IndexOf(elem) >= 0;
        }

        public override void CopyTo(object[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public override IEnumerator<object> GetEnumerator()
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
