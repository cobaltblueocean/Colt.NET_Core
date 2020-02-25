using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.List
{
    public abstract class AbstractList<T> : AbstractCollection<T>, IList<T>
    {
        public abstract T this[int index] { get; set; }

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// <summary>
        protected AbstractList() { }
        /// <summary>
        /// Appends all of the elements of the specified Collection to the
        /// receiver.
        /// 
        /// <summary>
        /// <exception cref="ClassCastException">if an element in the collection is not </exception>
        /// of the same parameter type of the receiver.
        public virtual void AddAllOf(ICollection<T> collection)
        {
            this.BeforeInsertAllOf(Size, collection);
        }
        /// <summary>Inserts all elements of the specified collection before the specified position into the receiverd
        /// Shifts the element
        /// currently at that position (if any) and any subsequent elements to
        /// the right (increases their indices)d
        /// 
        /// <summary>
        /// <param name="index">index before which to insert first element from the specified collection.</param>
        /// <param name="collection">the collection to be inserted</param>
        /// <exception cref="ClassCastException">if an element in the collection is not </exception>
        /// of the same parameter type of the receiver.
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || index &gt; Size</i>. </exception>
        public virtual void BeforeInsertAllOf(int index, ICollection<T> collection)
        {
            this.BeforeInsertDummies(index, collection.Count);
            this.ReplaceFromWith(index, collection);
        }
        /// <summary>
        /// Inserts <i>Length</i> dummy elements before the specified position into the receiverd
        /// Shifts the element currently at that position (if any) and
        /// any subsequent elements to the right.
        /// <b>This method must set the new size to be <i>Size+Length</i>.
        /// 
        /// <summary>
        /// <param name="index">index before which to insert dummy elements (must be in [0,size])..</param>
        /// <param name="Length">number of dummy elements to be inserted.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || index &gt; Size</i>. </exception>
        protected abstract void BeforeInsertDummies(int index, int Length);
        /// <summary>
        /// Checks if the given index is in range.
        /// <summary>
        protected static void CheckRange(int index, int theSize)
        {
            if (index >= theSize || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + theSize);
        }
        /// <summary>
        /// Checks if the given range is within the contained array's bounds.
        /// <summary>
        /// <exception cref="IndexOutOfRangeException">if <i>to!=from-1 || from&lt;0 || from&gt;to || to&gt;=Size</i>. </exception>
        protected static void CheckRangeFromTo(int from, int to, int theSize)
        {
            if (to == from - 1) return;
            if (from < 0 || from > to || to >= theSize)
                throw new IndexOutOfRangeException("from: " + from + ", to: " + to + ", size=" + theSize);
        }
        /// <summary>
        /// Removes all elements from the receiverd  The receiver will
        /// be empty after this call returns, but keep its current capacity.
        /// <summary>
        public override void Clear()
        {
            RemoveFromTo(0, Size - 1);
        }
        /// <summary>
        /// Sorts the receiver into ascending orderd
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        /// 
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        /// 
        /// <p><b>You should never call this method unless you are sure that this particular sorting algorithm is the right one for your data set.</b>
        /// It is generally better to call <i>sort()</i> or <i>sortFromTo(..d)</i> instead, because those methods automatically choose the best sorting algorithm.
        /// <summary>
        public virtual void MergeSort()
        {
            MergeSortFromTo(0, Size - 1);
        }
        /// <summary>
        /// Sorts the receiver into ascending orderd
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
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
        /// <exception cref="IndexOutOfRangeException">if <i>(from&lt;0 || from&gt;to || to&gt;=Size) && to!=from-1</i>. </exception>
        public abstract void MergeSortFromTo(int from, int to);
        /// <summary>
        /// Sorts the receiver into
        /// ascending orderd  The sorting algorithm is a tuned quicksort,
        /// adapted from Jon Ld Bentley and Md Douglas McIlroy's "Engineering a
        /// Sort Function", Software-Practice and Experience, Vold 23(11)
        /// Pd 1249-1265 (November 1993)d  This algorithm offers n*log(n)
        /// performance on many data sets that cause other quicksorts to degrade to
        /// quadratic performance.
        /// 
        /// <p><b>You should never call this method unless you are sure that this particular sorting algorithm is the right one for your data set.</b>
        /// It is generally better to call <i>sort()</i> or <i>sortFromTo(..d)</i> instead, because those methods automatically choose the best sorting algorithm.
        /// <summary>
        public virtual void QuickSort()
        {
            QuickSortFromTo(0, Size - 1);
        }
        /// <summary>
        /// Sorts the specified range of the receiver into
        /// ascending orderd  The sorting algorithm is a tuned quicksort,
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
        /// <exception cref="IndexOutOfRangeException">if <i>(from&lt;0 || from&gt;to || to&gt;=Size) && to!=from-1</i>. </exception>
        public abstract void QuickSortFromTo(int from, int to);

        /// <summary>
        /// Removes the element at the specified position from the receiver.
        /// Shifts any subsequent elements to the left.
        /// 
        /// <summary>
        /// <param name="index">the index of the element to removed.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || index &gt;= Size</i>. </exception>
        public virtual void Remove(Object item)
        {
            RemoveActual((T)item);
        }

        public override bool Remove(T item)
        {
            return RemoveActual(item);
        }

        private Boolean RemoveActual(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
            {
                return false;
            }
            else
            {
                RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        /// Removes the element at the specified position from the receiver.
        /// Shifts any subsequent elements to the left.
        /// 
        /// <summary>
        /// <param name="index">the index of the element to removed.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || index &gt;= Size</i>. </exception>
        public virtual void RemoveAt(int index)
        {
            RemoveFromTo(index, index);
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
        /// <exception cref="IndexOutOfRangeException">if <i>(from&lt;0 || from&gt;to || to&gt;=Size) && to!=from-1</i>. </exception>
        public abstract void RemoveFromTo(int fromIndex, int toIndex);
        /// <summary>
        /// Replaces the part of the receiver starting at <code>from</code> (inclusive) with all the elements of the specified collection.
        /// Does not alter the size of the receiver.
        /// Replaces exactly <i>System.Math.Max(0,System.Math.Min(Size-from, other.Count))</i> elements.
        /// 
        /// <summary>
        /// <param name="from">the index at which to copy the first element from the specified collection.</param>
        /// <param name="other">Collection to replace part of the receiver</param>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || index &gt;= Size</i>. </exception>
        public abstract void ReplaceFromWith(int from, ICollection<T> other);
        /// <summary>
        /// Reverses the elements of the receiver.
        /// Last becomes first, second last becomes second first, and so on.
        /// <summary>
        public abstract void Reverse();
        /// <summary>
        /// Sets the size of the receiver.
        /// If the new size is greater than the current size, new null or zero items are added to the end of the receiver.
        /// If the new size is less than the current size, all components at index newSize and greater are discarded.
        /// This method does not release any basefluos internal memoryd Use method <i>trimToSize</i> to release basefluos internal memory.
        /// <summary>
        /// <param name="newSize">the new size of the receiver.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>newSize &lt; 0</i>. </exception>
        public virtual void SetSize(int newSize)
        {
            if (newSize < 0) throw new IndexOutOfRangeException("newSize:" + newSize);

            int currentSize = Size;
            if (newSize != currentSize)
            {
                if (newSize > currentSize) BeforeInsertDummies(currentSize, newSize - currentSize);
                else if (newSize < currentSize) RemoveFromTo(newSize, currentSize - 1);
            }
        }
        /// <summary>
        /// Randomly permutes the receiverd After invocation, all elements will be at random positions.
        /// <summary>
        public virtual void Shuffle()
        {
            ShuffleFromTo(0, Size - 1);
        }
        /// <summary>
        /// Randomly permutes the receiver between <code>from</code> (inclusive) and <code>to</code> (inclusive)d
        /// <summary>
        /// <param name="from">the start position (inclusive)</param>
        /// <param name="to">the end position (inclusive)</param>
        /// <exception cref="IndexOutOfRangeException">if <i>(from&lt;0 || from&gt;to || to&gt;=Size) && to!=from-1</i>. </exception>
        public abstract void ShuffleFromTo(int from, int to);
        /// <summary>
        /// Sorts the receiver into ascending orderd
        /// 
        /// The sorting algorithm is dynamically chosen according to the characteristics of the data set.
        /// 
        /// This implementation simply calls <i>sortFromTo(..d)</i>.
        /// Override <i>sortFromTo(..d)</i> if you can determine which sort is most appropriate for the given data set.
        /// <summary>
        public virtual void Sort()
        {
            SortFromTo(0, Size - 1);
        }
        /// <summary>
        /// Sorts the specified range of the receiver into ascending orderd
        /// 
        /// The sorting algorithm is dynamically chosen according to the characteristics of the data set.
        /// This default implementation simply calls quickSort.
        /// Override this method if you can determine which sort is most appropriate for the given data set.
        /// 
        /// <summary>
        /// <param name="from">the index of the first element (inclusive) to be sorted.</param>
        /// <param name="to">the index of the last element (inclusive) to be sorted.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>(from&lt;0 || from&gt;to || to&gt;=Size) && to!=from-1</i>. </exception>
        public virtual void SortFromTo(int from, int to)
        {
            QuickSortFromTo(from, to);
        }
        /// <summary>
        /// Trims the capacity of the receiver to be the receiver's current
        /// sized Releases any basefluos internal memoryd An application can use this operation to minimize the
        /// storage of the receiver.
        /// <p>
        /// This default implementation does nothingd Override this method in space efficient implementations.
        /// <summary>
        public virtual void TrimToSize()
        {
        }

        public abstract int IndexOf(T item);
        public abstract void Insert(int index, T item);
    }
}
