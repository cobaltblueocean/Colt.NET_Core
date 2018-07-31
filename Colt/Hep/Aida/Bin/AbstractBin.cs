using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Cern.Hep.Aida.Bin
{
    /// <summary>
    /// Abstract base class for all arbitrary-dimensional bins consumes <i>double</i> elements.
    ///
    /// <p>
    /// This class is fully thread safe (all public methods are synchronized).
    /// Thus, you can have one or more threads adding to the bin as well as one or more threads reading and viewing the statistics of the bin <i>while it is filled</i>.
    /// For high performance, add data in large chunks (buffers) via method <i>addAllOf</i> rather than piecewise via method <i>add</i>.
    /// </summary>
    public abstract class AbstractBin: Cern.Colt.PersistentObject
    {
        #region Property
        /// <summary>
        /// Returns whether a client can obtain all elements added to the receiver.
        /// In other words, tells whether the receiver internally preserves all added elements.
        /// If the receiver is rebinnable, the elements can be obtained via <tt>elements()</tt> methods.
        /// </summary>
        public abstract Boolean IsRebinnable { get; set; }

        /// <summary>
        /// Returns the number of elements contained.
        /// </summary>
        public abstract int Size { get; set; }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Removes all elements from the receiver.
        /// The receiver will be empty after this call returns.
        /// </summary>
        public abstract void Clear();

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Returns <see cref="Center(0)"/>.
        /// </summary>
        /// <returns></returns>
        public double Center()
        {
            return Center(0);
        }

        /// <summary>
        /// Returns a custom definable "center" measure; override this method if necessary.
        /// Returns the absolute or relative center of this bin.
        /// For example, the center of gravity.
        /// 
        /// The <i>real</i> absolute center can be obtained as follow:
        /// <tt>partition(i).min(j) * bin(j).offset() + bin(j).center(i)</tt>,
        /// where <tt>i</tt> is the dimension.
        /// and <tt>j</tt> is the index of this bin.
        ///
        /// <p>This default implementation always returns 0.5.
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Center(int dimension)
        {
            return 0.5;
        }

        /// <summary>
        /// Returns whether two objects are equal;
        /// This default implementation returns true if the other object is a bin 
        /// and has the same size, value, error and center.
        /// </summary>
        /// <param name="otherObj"></param>
        /// <returns></returns>
        public override Boolean Equals(Object otherObj)
        {
            if (!(otherObj is AbstractBin)) return false;
            AbstractBin other = (AbstractBin)otherObj;
            return Size == other.Size && Value() == other.Value() && Error() == other.Error() && Center() == other.Center();
        }

        /// <summary>
        /// Returns <see cref="Error(0)"/>
        /// </summary>
        /// <returns></returns>
        public double Error()
        {
            return Error(0);
        }

        /// <summary>
        /// Returns a custom definable error measure; override this method if necessary.
        /// This default implementation always returns <tt>0</tt>.
        /// </summary>
        /// <param name="dimension">the dimension to be considered.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Error(int dimension)
        {
            return 0;
        }

        /// <summary>
        /// Returns <see cref="Offset(0)"/>
        /// </summary>
        /// <returns></returns>
        public double Offset()
        {
            return Offset(0);
        }

        /// <summary>
        /// Returns the relative or absolute position for the center of the bin; override this method if necessary.
        /// Returns 1.0 if a relative center is stored in the bin.
        /// Returns 0.0 if an absolute center is stored in the bin.
        ///
        /// <p>This default implementation always returns 1.0 (relative).
        /// </summary>
        /// <param name="dimension">the index of the considered dimension (zero based);</param>
        /// <returns></returns>
        public double Offset(int dimension)
        {
            return 1.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(this.GetType().Name);
            buf.Append("\n-------------");
            /*
            buf.Append("\nValue: "+value());
            buf.Append("\nError: "+error());
            buf.Append("\nRMS: "+rms()+"\n");
            */
            buf.Append("\n");
            return buf.ToString();
        }

        /// <summary>
        /// Trims the capacity of the receiver to be the receiver's current size.
        /// Releases any superfluos internal memory.
        /// An application can use this operation to minimize the storage of the receiver.
        ///
        /// This default implementation does nothing.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void TrimToSize() { }

        /// <summary>
        /// Returns <see cref="Value(0)"/>
        /// </summary>
        /// <returns></returns>
        public double Value()
        {
            return Value(0);
        }

        /// <summary>
        /// Returns a custom definable "value" measure; override this method if necessary.
        /// This default implementation always returns 0.0.
        /// </summary>
        /// <param name="dimension">the dimension to be considered.</param>
        /// <returns></returns>
        public double Value(int dimension)
        {
            return 0;
        }
        #endregion
    }
}
