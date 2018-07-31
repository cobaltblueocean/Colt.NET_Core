using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Buffer
{
    public class ObjectBuffer : Cern.Colt.PersistentObject, IObjectBufferConsumer
    {

        #region Local Variables
        protected IObjectBufferConsumer target;
        internal Object[] Elements { get; private set; }

        // vars cached for speed
        protected List<Object> list;
        protected int capacity;
        protected int size;
        #endregion

        #region Property

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs and returns a new buffer with the given target.
        /// </summary>
        /// <param name="target">the target to flush to.</param>
        /// <param name="capacity">the number of points the buffer shall be capable of holding before overflowing and flushing to the target.</param>
        public ObjectBuffer(IObjectBufferConsumer target, int capacity)
        {
            this.target = target;
            this.capacity = capacity;
            this.Elements = new Object[capacity];
            this.list = new List<Object>(Elements);
            this.size = 0;
        }
        #endregion

        #region Implement Methods

        /// <summary>
        /// Adds all elements of the specified list to the receiver.
        /// </summary>
        /// <param name="list">the list of which all elements shall be added.</param>
        public void AddAllOf(List<object> list)
        {
            int listSize = list.Count;
            if (this.size + listSize >= this.capacity) Flush();
            this.target.AddAllOf(list);
        }

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Adds the specified element to the receiver.
        /// </summary>
        /// <param name="element">the element to add.</param>
        public void Add(object element)
        {
            if (this.size == this.capacity) Flush();
            this.Elements[size++] = element;
        }

        /// <summary>
        /// Sets the receiver's size to zero.
        /// In other words, forgets about any internally buffered elements.
        /// </summary>
        public void Clear()
        {
            this.size = 0;
        }

        /// <summary>
        /// Adds all internally buffered elements to the receiver's target, then resets the current buffer size to zero.
        /// </summary>
        public void Flush()
        {
            if (this.size > 0)
            {
                list.SetSize(this.size);
                this.target.AddAllOf(list);
                this.size = 0;
            }
        }

        #endregion

        #region Local Private Methods

        #endregion
    }
}
