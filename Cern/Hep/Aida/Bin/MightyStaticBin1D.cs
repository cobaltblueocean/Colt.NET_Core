using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Cern.Jet.Stat;
using Cern.Colt.List;

namespace Cern.Hep.Aida.Bin
{
    /// <summary>
    /// Static and the same as its superclass, except that it can do more: Additionally computes moments of arbitrary integer order, harmonic mean, geometric mean, etc.
    /// 
    /// Constructors need to be told what functionality is required for the given use case.
    /// Only maintains aggregate measures (incrementally) - the added elements themselves are not kept.
    /// </summary>
    public class MightyStaticBin1D: StaticBin1D
    {

        #region Local Variables
        private Boolean _hasSumOfLogarithms = false;
        private double _sumOfLogarithms = 0.0; // Sum( Log(x[i]) )

        private Boolean _hasSumOfInversions = false;
        private double _sumOfInversions = 0.0; // Sum( 1/x[i] )

        private double[] _sumOfPowers = null;  // Sum( x[i]^3 ) .d Sum( x[i]^max_k )
        #endregion

        #region Property
        /// <summary>
        /// Gets whether <tt>sumOfInversions()</tt> can return meaningful results.  Return <tt>false</tt> if the bin was constructed with insufficient parametrization, <tt>true</tt> otherwise.
        /// </summary>
        public virtual Boolean HasSumOfInversions
        {
            get { return this._hasSumOfInversions; }
            set { this._hasSumOfInversions = value; }
        }

        /// <summary>
        /// Gets to tell whether <tt>sumOfLogarithms()</tt> can return meaningful results.  Return <tt>false</tt> if the bin was constructed with insufficient parametrization, <tt>true</tt> otherwise.
        /// </summary>
        public virtual Boolean HasSumOfLogarithms
        {
            get { return this._hasSumOfLogarithms; }
            set { this._hasSumOfLogarithms = value; }
        }

        /// <summary>
        /// Gets the sum of logarithms, which is <tt>Sum( Log(x[i]) )</tt>.
        /// </summary>
        public virtual double SumOfLogarithms
        {
            get {
                if (!this.HasSumOfLogarithms) return Double.NaN;
                //if (! this.HasSumOfLogarithms) throw new IllegalOperationException("You must specify upon instance construction that the sum of logarithms shall be computed.");
                return _sumOfLogarithms;
            }
            set { _sumOfLogarithms = value; }
        }

        /// <summary>
        /// Gets the sum of inversions, which is <tt>Sum( 1 / x[i] )</tt>.
        /// </summary>
        public virtual double SumOfInversions
        {
            get {
                if (!this.HasSumOfInversions) return Double.NaN;
                //if (! this.HasSumOfInversions) throw new IllegalOperationException("You must specify upon instance construction that the sum of inversions shall be computed.");
                return _sumOfInversions;
            }
            set { _sumOfInversions = value; }
        }

        /// <summary>
        /// Gets sum of powers
        /// </summary>
        public virtual double[] SumOfPowers
        {
            get { return _sumOfPowers; }
            set { _sumOfPowers = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs and returns an empty bin with limited functionality but good performance; equivalent to <tt>MightyStaticBin1D(false,false,4)</tt>.
        /// </summary>
        public MightyStaticBin1D(): this(false, false, 4)
        {
            
        }

        /// <summary>
        /// Constructs and returns an empty bin with the given capabilities.
        /// </summary>
        /// <param name="hasSumOfLogarithms">Tells whether <see cref="SumOfLogarithms"/> can return meaningful results. Set this parameter to <tt>false</tt> if measures of sum of logarithms, geometric mean and product are not required.</param>
        /// <param name="hasSumOfInversions">Tells whether <see cref="SumOfInversions"/> can return meaningful results. Set this parameter to <tt>false</tt> if measures of sum of inversions, harmonic mean and sumOfPowers(-1) are not required.</param>
        /// <param name="maxOrderForSumOfPowers">The maximum order <tt>k</tt> for which <see cref="GetSumOfPowers(int)"/> can return meaningful results.  Set this parameter to at least 3 if the skew is required, to at least 4 if the kurtosis is required.  In general, if moments are required set this parameter at least as large as the largest required moment.  This method always substitutes <see cref="System.Math.Max(2, maxOrderForSumOfPowers)"/> for the parameter passed in.  Thus, <see cref="GetSumOfPowers(0..2)"/> always returns meaningful results.</param>
        public MightyStaticBin1D(Boolean hasSumOfLogarithms, Boolean hasSumOfInversions, int maxOrderForSumOfPowers)
        {
            SetMaxOrderForSumOfPowers(maxOrderForSumOfPowers);
            this.HasSumOfLogarithms = hasSumOfLogarithms;
            this.HasSumOfInversions = hasSumOfInversions;
            this.Clear();
        }
        #endregion

        #region Local Public Methods
        /// <summary>
        /// Adds the part of the specified list between indexes <tt>from</tt> (inclusive) and <tt>to</tt> (inclusive) to the receiver.
        /// </summary>
        /// <param name="list">the list of which elements shall be added.</param>
        /// <param name="from">the index of the first element to be added (inclusive).</param>
        /// <param name="to">the index of the last element to be added (inclusive).</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void AddAllOfFromTo(DoubleArrayList list, int from, int to)
        {
            base.AddAllOfFromTo(list, from, to);

            if (_sumOfPowers != null)
            {
                //int max_k = this.min_k + this.SumOfPowers.Length-1;
                Descriptive.IncrementalUpdateSumsOfPowers(list, from, to, 3, GetMaxOrderForSumOfPowers(), ref _sumOfPowers);
            }

            if (this.HasSumOfInversions)
            {
                this.SumOfInversions += Descriptive.SumOfInversions(list, from, to);
            }

            if (this.HasSumOfLogarithms)
            {
                this.SumOfLogarithms += Descriptive.SumOfLogarithms(list, from, to);
            }
        }

        /// <summary>
        /// Returns a deep copy of the receiver.
        /// </summary>
        /// <returns>a deep copy of the receiver.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override Object Clone()
        {
            MightyStaticBin1D clone = (MightyStaticBin1D)base.Clone();
            if (_sumOfPowers != null) clone.SumOfPowers = (double[])clone.SumOfPowers.Clone();
            return clone;
        }

        /// <summary>
        /// Computes the deviations from the receiver's measures to another bin's measures.
        /// </summary>
        /// <param name="other">the other bin to compare with</param>
        /// <returns>a summary of the deviations.</returns>
        public override String CompareWith(AbstractBin1D other)
        {
            StringBuilder buf = new StringBuilder(base.CompareWith(other));
            if (other is MightyStaticBin1D) {
                MightyStaticBin1D m = (MightyStaticBin1D)other;
                if (HasSumOfLogarithms && m.HasSumOfLogarithms)
                    buf.Append("geometric mean: " + RelError(GeometricMean(), m.GeometricMean()) + " %\n");
                if (HasSumOfInversions && m.HasSumOfInversions)
                    buf.Append("harmonic mean: " + RelError(HarmonicMean(), m.HarmonicMean()) + " %\n");
                if (HasSumOfPowers(3) && m.HasSumOfPowers(3))
                    buf.Append("skew: " + RelError(Skew(), m.Skew()) + " %\n");
                if (HasSumOfPowers(4) && m.HasSumOfPowers(4))
                    buf.Append("kurtosis: " + RelError(Kurtosis(), m.Kurtosis()) + " %\n");
                buf.Append("\n");
            }
            return buf.ToString();
        }

        /// <summary>
        /// Returns the geometric mean, which is <tt>Product( x[i] )<sup>1.0/size()</sup></tt>.
        /// 
        /// This method tries to avoid overflows at the expense of an equivalent but somewhat inefficient definition:
        /// <tt>geoMean = exp( Sum( Log(x[i]) ) / size())</tt>.
        /// Note that for a geometric mean to be meaningful, the minimum of the data sequence must not be less or equal to zero.
        /// </summary>
        /// <returns>the geometric mean; <tt>Double.NaN</tt> if <tt>!hasSumOfLogarithms()</tt>.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual double GeometricMean()
        {
            return Descriptive.GeometricMean(Size, SumOfLogarithms);
        }

        /// <summary>
        /// Returns the maximum order <tt>k</tt> for which sums of powers are retrievable, as specified upon instance construction.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual int GetMaxOrderForSumOfPowers()
        {
            /* order 0..2 is always recorded.
               order 0 is Size
               order 1 is sum()
               order 2 is sum_xx()
            */
            if (this._sumOfPowers == null) return 2;

            return 2 + this._sumOfPowers.Length;
        }

        /// <summary>
        /// Returns the minimum order <tt>k</tt> for which sums of powers are retrievable, as specified upon instance construction.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual int GetMinOrderForSumOfPowers()
        {
            int minOrder = 0;
            if (HasSumOfInversions) minOrder = -1;
            return minOrder;
        }

        /// <summary>
        /// Returns the harmonic mean, which is <tt>size() / Sum( 1/x[i] )</tt>.
        /// Remember: If the receiver contains at least one element of <tt>0.0</tt>, the harmonic mean is <tt>0.0</tt>.
        /// </summary>
        /// <returns>the harmonic mean; <tt>Double.NaN</tt> if <tt>!hasSumOfInversions()</tt>.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual double HarmonicMean()
        {
            return Descriptive.HarmonicMean(Size, SumOfInversions);
        }

        /// <summary>
        /// Tells whether <see cref="GetSumOfPowers(int)"/> can return meaningful results.
        /// Defined as <see cref="HasSumOfPowers(int)"/> &lt;==&gt; <see cref="GetMinOrderForSumOfPowers()"/> &lt;= k && k &lt;= <see cref="GetMaxOrderForSumOfPowers()"/>.
        /// A return value of <tt>true</tt> implies that <see cref="HasSumOfPowers(k - 1)"/> .. <see cref="HasSumOfPowers(0)"/> will also return <tt>true</tt>.
        /// See the constructors for proper parametrization.
        /// <p>
        /// <b>Details</b>: 
        /// <see cref="HasSumOfPowers(0..2)"/> will always yield <tt>true</tt>.
        /// <see cref="HasSumOfPowers(-1)"/> &lt;==&gt; <see cref="HasSumOfInversions"/>.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual Boolean HasSumOfPowers(int k)
        {
            return GetMinOrderForSumOfPowers() <= k && k <= GetMaxOrderForSumOfPowers();
        }

        /// <summary>
        /// Returns the kurtosis (aka excess), which is <tt>-3 + <see cref="Moment(4, <see cref="AbstractBin1D.Mean()"/>)"/> / <see cref="AbstractBin1D.StandardDeviation()"/><sup>4</sup></tt>.
        /// </summary>
        /// <returns>the kurtosis; <tt>Double.NaN</tt> if <tt>!hasSumOfPowers(4)</tt>.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual double Kurtosis()
        {
            return Descriptive.Kurtosis(Moment(4, Mean()), StandardDeviation());
        }

        /// <summary>
        /// Returns the moment of <tt>k</tt>-th order with value <tt>c</tt>, which is <tt>Sum( (x[i]-c)<sup>k</sup> ) / size()</tt>.
        /// </summary>
        /// <param name="k">the order; must be greater than or equal to zero.</param>
        /// <param name="c">any number.</param>
        /// <returns><tt>Double.NaN</tt> if <tt>!hasSumOfPower(k)</tt>.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual double Moment(int k, double c)
        {
            if (k < 0) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_KMustBePositive);
            //checkOrder(k);
            if (!HasSumOfPowers(k)) return Double.NaN;

            int maxOrder = System.Math.Min(k, GetMaxOrderForSumOfPowers());
            DoubleArrayList sumOfPows = new DoubleArrayList(maxOrder + 1);
            sumOfPows.Add(Size);
            sumOfPows.Add(Sum);
            sumOfPows.Add(SumOfSquares);
            for (int i = 3; i <= maxOrder; i++) sumOfPows.Add(GetSumOfPowers(i));

            return Descriptive.Moment(k, c, Size, sumOfPows.ToArray());
        }

        /// <summary>
        /// Returns the product, which is <tt>Prod( x[i] )</tt>.
        /// In other words: <tt>x[0]*x[1]*...*x[size()-1]</tt>.
        /// </summary>
        /// <returns>the product; <tt>Double.NaN</tt> if <tt>!hasSumOfLogarithms()</tt>.</returns>
        public virtual double Product()
        {
            return Descriptive.Product(Size, SumOfLogarithms);
        }

        /// <summary>
        /// Returns the skew, which is <tt>moment(3,mean()) / standardDeviation()<sup>3</sup></tt>.
        /// </summary>
        /// <returns>the skew; <tt>Double.NaN</tt> if <tt>!hasSumOfPowers(3)</tt>.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual double Skew()
        {
            return Descriptive.Skew(Moment(3, Mean()), StandardDeviation());
        }

        /// <summary>
        /// Returns the <tt>k-th</tt> order sum of powers, which is <tt>Sum( x[i]<sup>k</sup> )</tt>.
        /// </summary>
        /// <param name="k">the order of the powers.</param>
        /// <returns>the sum of powers; <tt>Double.NaN</tt> if <tt>!hasSumOfPowers(k)</tt>.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual double GetSumOfPowers(int k)
        {
            if (!HasSumOfPowers(k)) return Double.NaN;
            //checkOrder(k);	
            if (k == -1) return SumOfInversions;
            if (k == 0) return Size;
            if (k == 1) return Sum;
            if (k == 2) return SumOfSquares;

            return this.SumOfPowers[k - 3];
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override String ToString()
        {
            StringBuilder buf = new StringBuilder(base.ToString());

            if (HasSumOfLogarithms)
            {
                buf.Append("Geometric mean: " + GeometricMean());
                buf.Append("\nProduct: " + Product() + "\n");
            }

            if (HasSumOfInversions)
            {
                buf.Append("Harmonic mean: " + HarmonicMean());
                buf.Append("\nSum of inversions: " + SumOfInversions + "\n");
            }

            int maxOrder = GetMaxOrderForSumOfPowers();
            int maxPrintOrder = System.Math.Min(6, maxOrder); // don't print tons of measures
            if (maxOrder > 2)
            {
                if (maxOrder >= 3)
                {
                    buf.Append("Skew: " + Skew() + "\n");
                }
                if (maxOrder >= 4)
                {
                    buf.Append("Kurtosis: " + Kurtosis() + "\n");
                }
                for (int i = 3; i <= maxPrintOrder; i++)
                {
                    buf.Append("Sum of powers(" + i + "): " + GetSumOfPowers(i) + "\n");
                }
                for (int k = 0; k <= maxPrintOrder; k++)
                {
                    buf.Append("Moment(" + k + ",0): " + Moment(k, 0) + "\n");
                }
                for (int k = 0; k <= maxPrintOrder; k++)
                {
                    buf.Append("Moment(" + k + ",Mean()): " + Moment(k, Mean()) + "\n");
                }
            }
            return buf.ToString();
        }

        #endregion

        #region Local Protected Methods
        /// <summary>
        /// Resets the values of all measures.
        /// </summary>
        protected new void ClearAllMeasures()
        {
            base.ClearAllMeasures();

            this.SumOfLogarithms = 0.0;
            this.SumOfInversions = 0.0;

            if (this.SumOfPowers != null)
            {
                for (int i = this.SumOfPowers.Length; --i >= 0;)
                {
                    this.SumOfPowers[i] = 0.0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="max_k"></param>
        protected void SetMaxOrderForSumOfPowers(int max_k)
        {
            //if (max_k < ) throw new ArgumentException();

            if (max_k <= 2)
            {
                this.SumOfPowers = null;
            }
            else
            {
                this.SumOfPowers = new double[max_k - 2];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        protected void XCheckOrder(int k)
        {
            //if (! isLegalOrder(k)) return Double.NaN;
            //if (! xisLegalOrder(k)) throw new IllegalOperationException("Illegal order of sum of powers: k="+k+"d Upon instance construction legal range was fixed to be "+getMinOrderForSumOfPowers()+" <= k <= "+getMaxOrderForSumOfPowers());
        }

        /// <summary>
        /// Returns whether two bins are equal; 
        /// They are equal if the other object is of the same class or a subclass of this class and both have the same size, minimum, maximum, sum, sumOfSquares, sumOfInversions and sumOfLogarithms.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected Boolean XEquals(Object obj)
        {
            if (!(obj is MightyStaticBin1D)) return false;
            MightyStaticBin1D other = (MightyStaticBin1D)obj;
            return base.Equals(other) && SumOfInversions == other.SumOfInversions && SumOfLogarithms == other.SumOfLogarithms;
        }

        /// <summary>
        /// Tells whether <tt>sumOfPowers(fromK) .. sumOfPowers(toK)</tt> can return meaningful results.
        /// </summary>
        /// <param name="fromK"></param>
        /// <param name="toK"></param>
        /// <returns><tt>false</tt> if the bin was constructed with insufficient parametrization, <tt>true</tt> otherwise.</returns>
        protected Boolean XHasSumOfPowers(int fromK, int toK)
        {
            if (fromK > toK) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_fromK_MustBeLessOrEqualTo_toK);
            return GetMinOrderForSumOfPowers() <= fromK && toK <= GetMaxOrderForSumOfPowers();
        }

        /// <summary>
        /// Returns <tt>getMinOrderForSumOfPowers() <= k && k <= getMaxOrderForSumOfPowers()</tt>.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected Boolean XIsLegalOrder(int k)
        {
            return GetMinOrderForSumOfPowers() <= k && k <= GetMaxOrderForSumOfPowers();
        }
        #endregion
    }
}
