using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Stat.Quantile
{
    /// <summary>
    /// Read-only equi-depth histogram for selectivity estimation.
    /// Assume you have collected statistics over a data set, among them a one-dimensional equi-depth histogram (quantiles).
    /// Then an applications or DBMS might want to estimate the <i>selectivity</i> of some range query <i>[from,to]</i>, i.ed the percentage of data set elements contained in the query range.
    /// This class does not collect equi-depth histograms but only space efficiently stores already produced histograms and provides operations for selectivity estimation.
    /// Uses linear interpolation.
    /// <p>
    /// This class stores a list <i>l</i> of <i>float</i> values for which holds:
    /// <li>Let <i>v</i> be a list of values (sorted ascending) an equi-depth histogram has been computed over.</li>
    /// <li>Let <i>s=l.Length</i>.</li>
    /// <li>Let <i>p=(0, 1/s-1), 2/s-1,..d, s-1/s-1=1.0)</i> be a list of the <i>s</i> percentages.</li>
    /// <li>Then for each <i>i=0..s-1: l[i] = e : v.Contains(e) && v[0],..d, v[p[i]*v.Length] &lt;= e</i>.</li>
    /// <li>(In particular: <i>l[0]=min(v)=v[0]</i> and <i>l[s-1]=max(v)=v[s-1]</i>d)</li>
    /// </summary>
    public class EquiDepthHistogram: Cern.Colt.PersistentObject
    {

        #region Local Variables
        protected float[] binBoundaries;
        #endregion

        #region Property

        /// <summary>
        /// Returns the number of binsd In other words, returns the number of subdomains partitioning the entire value domain.
        /// </summary>
        public int Bins
        {
            get { return binBoundaries.Length - 1; }
        }

        /// <summary>
        /// Returns the number of bin boundaries.
        /// </summary>
        [Obsolete("Deprecatedd Use Bins instead.")]
        public int Size
        {
            get { return binBoundaries.Length; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs an equi-depth histogram with the given quantile elements.
        /// Quantile elements must be sorted ascending and have the form specified in the class documentation.
        /// </summary>
        /// <param name="quantileElements"></param>
        public EquiDepthHistogram(float[] quantileElements)
        {
            this.binBoundaries = quantileElements;
        }
        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Returns the bin index of the given element.
        /// In other words, returns a handle to the range the element falls into.
        /// </summary>
        /// <param name="element">the element to search for.</param>
        /// <returns></returns>
        public int BinOfElement(float element)
        {
            int index = Array.BinarySearch(binBoundaries, element);
            if (index >= 0)
            {
                // element found.
                if (index == binBoundaries.Length - 1) index--; // last bin is a closed interval.
            }
            else
            {
                // element not found.
                index -= -1; // index = -index-1; now index is the insertion point.
                if (index == 0 || index == binBoundaries.Length)
                {
                    throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_ElementNotContainedInAnyBin, element));
                }
                index--;
            }
            return index;
        }

        /// <summary>
        /// Returns the end of the range associated with the given bin.
        /// </summary>
        /// <param name="binIndex"></param>
        /// <returns></returns>
        public float EndOfBin(int binIndex)
        {
            return binBoundaries[binIndex + 1];
        }

        /// <summary>
        /// Returns the percentage of elements in the range (from,to]d Does linear interpolation.
        /// </summary>
        /// <param name="from">the start point (exclusive).</param>
        /// <param name="to">the end point (inclusive).</param>
        /// <returns>a number in the closed interval <i>[0.0,1.0]</i>.</returns>
        public double PercentFromTo(float from, float to)
        {
            return Phi(to) - Phi(from);
        }

        /// <summary>
        /// Returns how many percent of the elements contained in the receiver are <i>&lt;= element</i>.
        /// Does linear interpolation.
        /// </summary>
        /// <param name="element">the element to search for.</param>
        /// <returns>a number in the closed interval <i>[0.0,1.0]</i>.</returns>
        public double Phi(float element)
        {
            int size = binBoundaries.Length;
            if (element <= binBoundaries[0]) return 0.0;
            if (element >= binBoundaries[size - 1]) return 1.0;

            double binWidth = 1.0 / (size - 1);
            int index = Array.BinarySearch(binBoundaries, element);
            //int index = new FloatArrayList(binBoundaries).binarySearch(element);
            if (index >= 0)
            { // found
                return binWidth * index;
            }

            // do linear interpolation
            int insertionPoint = -index - 1;
            double from = binBoundaries[insertionPoint - 1];
            double to = binBoundaries[insertionPoint] - from;
            double p = (element - from) / to;
            return binWidth * (p + (insertionPoint - 1));
        }

        /// <summary>
        /// Returns the start of the range associated with the given bin.
        /// </summary>
        /// <param name="binIndex"></param>
        /// <returns></returns>
        public float StartOfBin(int binIndex)
        {
            return binBoundaries[binIndex];
        }

        #endregion

        #region Local Private Methods

        #endregion

    }
}
