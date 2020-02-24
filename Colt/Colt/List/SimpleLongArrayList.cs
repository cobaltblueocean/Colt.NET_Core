// <copyright file="SimpleLongArrayList.cs" company="CERN">
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
    public class SimpleLongArrayList : AbstractLongList
    {
        /// <summary>
        /// The array buffer into which the elements of the list are stored.
        /// The capacity of the list is the Length of this array buffer.
        /// @serial
        /// <summary>
        private long[] _elements;

        public long[] Elements
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
        public SimpleLongArrayList() : this(10)
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
        public SimpleLongArrayList(long[] elements)
        {
            this.SetElements(elements);
        }
        /// <summary>
        /// Constructs an empty list with the specified initial capacity.
        /// 
        /// <summary>
        /// <param name=""> initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.</param>
        public SimpleLongArrayList(int initialCapacity) : base()
        {

            if (initialCapacity < 0)
                throw new ArgumentException("Illegal Capacity: " + initialCapacity);

            this.SetElements(new long[initialCapacity]);
            this.SetSize(0);
        }

        public override void CopyTo(long[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public override void EnsureCapacity(int minCapacity)
        {
            _elements.EnsureCapacity(minCapacity);
        }

        public override IEnumerator<long> GetEnumerator()
        {
            foreach (var item in _elements)
                yield return item;
        }

        public override void Insert(int index, long item)
        {
            BeforeInsert(index, item);
        }

        protected override long GetQuick(int index)
        {
            return _elements[index];
        }

        protected override void SetQuick(int index, long element)
        {
            _elements[index] = element;
        }
    }
}
