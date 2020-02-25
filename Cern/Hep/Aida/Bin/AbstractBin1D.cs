using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Stat;
using System.Runtime.CompilerServices;
using Cern.Colt.List;

namespace Cern.Hep.Aida.Bin
{
    /// <summary>
    /// Abstract base class for all 1-dimensional bins consumes <tt>double</tt> elements.
    ///
    /// <p>
    /// This class is fully thread safe (all public methods are synchronized).
    /// Thus, you can have one or more threads adding to the bin as well as one or more threads reading and viewing the statistics of the bin <i>while it is filled</i>.
    /// For high performance, add data in large chunks (buffers) via method <tt>addAllOf</tt> rather than piecewise via method <tt>add</tt>.
    /// </summary>
    public abstract class AbstractBin1D: AbstractBin, Cern.Colt.Buffer.IDoubleBufferConsumer
    {
        #region Abstract Methods
        /// <summary>
        /// Adds the specified element to the receiver.
        /// </summary>
        /// <param name="element">element to be appended.</param>
        public abstract void Add(double element);

        /// <summary>
        /// Returns the maximum.
        /// </summary>
        /// <returns></returns>
        public abstract double Max { get; set; }

        /// <summary>
        /// Returns the minimum.
        /// </summary>
        /// <returns></returns>
        public abstract double Min { get; set; }

        /// <summary>
        /// Returns the sum of all elements, which is <tt>Sum( x[i] )</tt>.
        /// </summary>
        /// <returns></returns>
        public abstract double Sum { get; set; }

        /// <summary>
        /// Returns the sum of squares, which is <tt>Sum( x[i] * x[i] )</tt>.
        /// </summary>
        /// <returns></returns>
        public abstract double SumOfSquares { get; set; }
        #endregion

        #region Local Public Methods
        /// <summary>
        /// Adds all values of the specified list to the receiver.
        /// </summary>
        /// <param name="list">the list of which all values shall be added.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void AddAllOf(DoubleArrayList list)
        {
            AddAllOfFromTo(list, 0, list.Size - 1);
        }

        /// <summary>
        /// Adds the part of the specified list between indexes <tt>from</tt> (inclusive) and <tt>to</tt> (inclusive) to the receiver.
        /// You may want to override this method for performance reasons.
        /// </summary>
        /// <param name="list">the list of which elements shall be added.</param>
        /// <param name="from">the index of the first element to be added (inclusive).</param>
        /// <param name="to">the index of the last element to be added (inclusive).</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void AddAllOfFromTo(DoubleArrayList list, int from, int to)
        {
            for (int i = from; i <= to; i++) Add(list[i]);
        }

        /// <summary>
        /// Constructs and returns a streaming buffer connected to the receiver.
        /// Whenever the buffer is full it's contents are automatically flushed to <tt>this</tt>. 
        /// (Addding elements via a buffer to a bin is significantly faster than adding them directly.)
        /// </summary>
        /// <param name="capacity">the number of elements the buffer shall be capable of holding before overflowing and flushing to the receiver.</param>
        /// <returns>a streaming buffer having the receiver as target.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual Cern.Colt.Buffer.DoubleBuffer Buffered(int capacity)
        {
            return new Cern.Colt.Buffer.DoubleBuffer(this, capacity);
        }

        /// <summary>
        /// Computes the deviations from the receiver's measures to another bin's measures.
        /// </summary>
        /// <param name="other">the other bin to compare with</param>
        /// <returns>a summary of the deviations.</returns>
        public virtual String CompareWith(AbstractBin1D other)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("\nDifferences [percent]");
            buf.Append("\nSize: " + RelError(Size, other.Size) + " %");
            buf.Append("\nSum: " + RelError(Sum, other.Sum) + " %");
            buf.Append("\nSumOfSquares: " + RelError(SumOfSquares, other.SumOfSquares) + " %");
            buf.Append("\nMin: " + RelError(Min, other.Min) + " %");
            buf.Append("\nMax: " + RelError(Max, other.Max) + " %");
            buf.Append("\nMean: " + RelError(Mean(), other.Mean()) + " %");
            buf.Append("\nRMS: " + RelError(Rms(), other.Rms()) + " %");
            buf.Append("\nVariance: " + RelError(Variance(), other.Variance()) + " %");
            buf.Append("\nStandard deviation: " + RelError(StandardDeviation(), other.StandardDeviation()) + " %");
            buf.Append("\nStandard error: " + RelError(StandardError(), other.StandardError()) + " %");
            buf.Append("\n");
            return buf.ToString();
        }

        /// <summary>
        /// Returns whether two bins are equal; 
        /// They are equal if the other object is of the same class or a subclass of this class and both have the same size, minimum, maximum, sum and <see cref="SumOfSquares"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals(Object obj)
        {
            if (!(obj is AbstractBin1D)) return false;
            AbstractBin1D other = (AbstractBin1D)obj;
            return Size == other.Size && Min == other.Min && Max == other.Max
                && Sum == other.Sum && SumOfSquares == other.SumOfSquares;
        }

        /// <summary>
        /// Returns the arithmetic mean, which is <tt>Sum( x[i] ) / size()</tt>.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Mean()
        {
            return Sum / Size;
        }

        /// <summary>
        /// Returns the rms (Root Mean Square), which is <tt>Math.sqrt( Sum( x[i]*x[i] ) / size() )</tt>.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Rms()
        {
            return Descriptive.Rms(Size, SumOfSquares);
        }

        /// <summary>
        /// Returns the sample standard deviation, which is <tt>Math.sqrt(variance())</tt>.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double StandardDeviation()
        {
            return System.Math.Sqrt(Variance());
        }

        /// <summary>
        /// Returns the sample standard error, which is <tt>Math.sqrt(variance() / size())</tt>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double StandardError()
        {
            return Descriptive.StandardError(Size, Variance());
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
            buf.Append("\nSize: " + Size);
            buf.Append("\nSum: " + Sum);
            buf.Append("\nSumOfSquares: " + SumOfSquares);
            buf.Append("\nMin: " + Min);
            buf.Append("\nMax: " + Max);
            buf.Append("\nMean: " + Mean());
            buf.Append("\nRMS: " + Rms());
            buf.Append("\nVariance: " + Variance());
            buf.Append("\nStandard deviation: " + StandardDeviation());
            buf.Append("\nStandard error: " + StandardError());
            /*
            buf.Append("\nValue: "+value());
            buf.Append("\nError(0): "+error(0));
            */
            buf.Append("\n");
            return buf.ToString();
        }

        /// <summary>
        /// Trims the capacity of the receiver to be the receiver's current size.
        /// Releases any superfluos internal memory.
        /// An application can use this operation to minimize the storage of the receiver.
        /// This default implementation does nothing.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void TrimToSize() { }

        /// <summary>
        /// Returns the sample variance, which is <tt>Sum( (x[i]-mean())<sup>2</sup> )  /  (size()-1)</tt>.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Variance()
        {
            return Descriptive.SampleVariance(Size, Sum, SumOfSquares);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Local Protected Methods
        /// <summary>
        /// Computes the relative error (in percent) from one measure to another.
        /// </summary>
        /// <param name="measure1"></param>
        /// <param name="measure2"></param>
        /// <returns></returns>
        protected double RelError(double measure1, double measure2)
        {
            return 100 * (1 - measure1 / measure2);
        }
        #endregion
    }
}
