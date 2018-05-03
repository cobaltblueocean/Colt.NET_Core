using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Cern.Jet.Stat;
namespace Cern.Hep.Aida.Bin
{
    public class StaticBin1D : AbstractBin1D
    {

        #region Local Variables
        // The number of elements consumed by incremental parameter maintainance.
        protected int size = 0;

        // cached parameters
        protected double min = 0.0;    // Min( x[i] )
        protected double max = 0.0;    // Max( x[i] )
        protected double sum = 0.0;    // Sum( x[i] )
        protected double sum_xx = 0.0; // Sum( x[i]*x[i] )

        [NonSerialized]
        static protected double[] arguments = new double[20];
        #endregion

        #region Implement Property
        /// <summary>
        /// Returns <i>false</i>.
        /// Returns whether a client can obtain all elements added to the receiver.
        /// In other words, tells whether the receiver internally preserves all added elements.
        /// If the receiver is rebinnable, the elements can be obtained via <i>elements()</i> methods.
        /// </summary>
        public override bool IsRebinnable
        {
            get
            {
                return false;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the receiver.
        /// </summary>
        public override int Size
        {
            get
            {
                return this.size;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs and returns an empty bin.
        /// </summary>
        public StaticBin1D()
        {
            Clear();
        }
        #endregion

        #region Implement Methods
        /// <summary>
        /// Adds the specified element to the receiver.
        /// </summary>
        /// <param name="element">element to be appended.</param>
        public override void Add(double element)
        {
            this.AddAllOf(new List<Double>(new double[] { element }));
        }

        /// <summary>
        /// Removes all elements from the receiver.
        /// The receiver will be empty after this call returns.
        /// </summary>
        public override void Clear()
        {
            ClearAllMeasures();
            this.size = 0;
        }

        /// <summary>
        /// Returns the maximum.
        /// </summary>
        /// <returns></returns>
        public override double Max()
        {
            return this.max;
        }

        /// <summary>
        /// Returns the minimum.
        /// </summary>
        /// <returns></returns>
        public override double Min()
        {
            return this.min;
        }

        /// <summary>
        /// Returns the sum of all elements, which is <i>Sum( x[i] )</i>.
        /// </summary>
        /// <returns></returns>
        public override double Sum()
        {
            return this.sum;
        }

        /// <summary>
        /// Returns the sum of squares, which is <i>Sum( x[i] * x[i] )</i>.
        /// </summary>
        /// <returns></returns>
        public override double SumOfSquares()
        {
            return this.sum_xx;
        }
        #endregion

        #region Local Public Methods
        /// <summary>
        /// Adds the part of the specified list between indexes <i>from</i> (inclusive) and <i>to</i> (inclusive) to the receiver.
        /// </summary>
        /// <param name="list">the list of which elements shall be added.</param>
        /// <param name="from">the index of the first element to be added (inclusive).</param>
        /// <param name="to">the index of the last element to be added (inclusive).</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public new void AddAllOfFromTo(List<Double> list, int from, int to)
        {
            //if (this.arguments == null) setUpCache();
            lock(arguments) {
                // prepare arguments
                arguments[0] = this.min;
                arguments[1] = this.max;
                arguments[2] = this.sum;
                arguments[3] = this.sum_xx;

                Descriptive.IncrementalUpdate(list, from, to, ref arguments);

                // store the new parameters back
                this.min = arguments[0];
                this.max = arguments[1];
                this.sum = arguments[2];
                this.sum_xx = arguments[3];

                this.size += to - from + 1;
            }
        }
        #endregion

        #region Local Protected Methods
        /// <summary>
        /// Resets the values of all measures.
        /// </summary>
        protected void ClearAllMeasures()
        {
            this.min = Double.PositiveInfinity;
            this.max = Double.NegativeInfinity;
            this.sum = 0.0;
            this.sum_xx = 0.0;
        }
        #endregion

        #region Local Protected Methods

        #endregion




    }
}
