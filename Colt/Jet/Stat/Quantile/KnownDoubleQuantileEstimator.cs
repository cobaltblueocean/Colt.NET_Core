using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;
using Cern.Jet.Random.Sampling;

namespace Cern.Jet.Stat.Quantile
{
    public class KnownDoubleQuantileEstimator : DoubleQuantileEstimator
    {

        #region Local Variables
        protected double beta; //correction factor for phis

        protected Boolean weHadMoreThanOneEmptyBuffer;

        protected RandomSamplingAssistant samplingAssistant;
        protected double samplingRate; // see method sampleNextElement()
        protected long N; // see method sampleNextElement()
        #endregion

        #region Property

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="k"></param>
        /// <param name="N"></param>
        /// <param name="samplingRate"></param>
        /// <param name="generator"></param>
        public KnownDoubleQuantileEstimator(int b, int k, long N, double samplingRate, RandomEngine generator)
        {
            this.samplingRate = samplingRate;
            this.N = N;

            if (this.samplingRate <= 1.0)
            {
                this.samplingAssistant = null;
            }
            else
            {
                this.samplingAssistant = new RandomSamplingAssistant((long)System.Math.Floor(N / samplingRate), N, generator);
            }

            SetUp(b, k);
            this.Clear();
        }
        #endregion

        #region Implement Methods

        protected override void NewBuffer()
        {
            int numberOfEmptyBuffers = this.BufferSet.GetNumberOfEmptyBuffers();
            //DoubleBuffer[] emptyBuffers = this.BufferSet.GetEmptyBuffers();
            if (numberOfEmptyBuffers == 0) throw new InvalidOperationException("Oops, no empty buffer.");

            this.CurrentBufferToFill = this.BufferSet.GetFirstEmptyBuffer();
            if (numberOfEmptyBuffers == 1 && !this.weHadMoreThanOneEmptyBuffer)
            {
                this.CurrentBufferToFill.Level = this.BufferSet.GetMinLevelOfFullOrPartialBuffers();
            }
            else
            {
                this.weHadMoreThanOneEmptyBuffer = true;
                this.CurrentBufferToFill.Level = 0;
                /*
                for (int i=0; i<emptyBuffers.Length; i++) {
                    emptyBuffers[i].level = 0;			
                }
                */
            }
            //currentBufferToFill.state = DoubleBuffer.PARTIAL;
            this.CurrentBufferToFill.Weight = 1;

        }

        protected override void PostCollapse(DoubleBuffer[] toCollapse)
        {
            this.weHadMoreThanOneEmptyBuffer = false;
        }

        protected override bool SampleNextElement()
        {
            if (samplingAssistant == null) return true;

            /*
             * This is a KNOWN N quantile finder!
             * One should not try to fill more than N elements,
             * because otherwise we can't give explicit approximation guarantees anymore.
             * Use an UNKNOWN quantile finder instead if your app may fill more than N elements.
             *
             * However, to make this class meaningful even under wired use cases, we actually do allow to fill more than N elements (without explicit approxd guarantees, of course).
             * Normally, elements beyond N will not get sampled because the sampler is exhaustedd 
             * Therefore the histogram will no more change no matter how much you fill.
             * This might not be what the user expects.
             * Therefore we use a new (unexhausted) sampler with the same parametrization.
             *
             * If you want this class to ignore any elements beyong N, then comment the following line.
             */
            //if ((totalElementsFilled-1) % N == 0) setSamplingRate(samplingRate, N); // delete if appropriate

            return samplingAssistant.SampleNextElement();
        }

        #endregion

        #region Local Public Methods
        public new void Clear()
        {
            base.Clear();
            this.beta = 1.0;
            this.weHadMoreThanOneEmptyBuffer = false;
            //this.setSamplingRate(samplingRate,N);

            RandomSamplingAssistant assist = this.samplingAssistant;
            if (assist != null)
            {
                this.samplingAssistant = new RandomSamplingAssistant((long)System.Math.Floor(N / samplingRate), N, assist.RandomGenerator);
            }
        }

        public new Object Clone()
        {
            KnownDoubleQuantileEstimator copy = (KnownDoubleQuantileEstimator)base.Clone();
            if (this.samplingAssistant != null) copy.samplingAssistant = (RandomSamplingAssistant)copy.samplingAssistant.Clone();
            return copy;
        }

        public new List<Double> QuantileElements(List<Double> phis)
        {
            /*
            * The KNOWN quantile finder reads off quantiles from FULL buffers only.
            * Since there might be a partially full buffer, this method first satisfies this constraint by temporarily filling a few +infinity, -infinity elements to make up a full block.
            * This is in full conformance with the explicit approximation guarantees.
            *
            * For those of you working on online apps:
            * The approximation guarantees are given for computing quantiles AFTER N elements have been filled, not for intermediate displays.
            * If you have one thread filling and another thread displaying concurrently, you will note that in the very beginning the infinities will dominate the display.
            * This could confuse users, because, of course, they don't expect any infinities, even if they "disappear" after a short while.
            * To prevent panic exclude phi's close to zero or one in the early phases of processing.
            */
            DoubleBuffer partial = this.BufferSet.GetPartialBuffer();
            int missingValues = 0;
            if (partial != null)
            { // any auxiliary infinities needed?
                missingValues = BufferSet.BufferSize - partial.Size;
                if (missingValues <= 0) throw new InvalidOperationException("Oops! illegal missing values.");

                //System.out.println("adding "+missingValues+" infinity elements...");
                this.AddInfinities(missingValues, partial);

                //determine beta (N + Infinity values = beta * N)
                this.beta = (this.TotalElementsFilled + missingValues) / (double)this.TotalElementsFilled;
            }
            else
            {
                this.beta = 1.0;
            }

            List<Double> quantileElements = base.QuantileElements(phis);

            // restore state we were in before.
            // remove the temporarily added infinities.
            if (partial != null) RemoveInfinitiesFrom(missingValues, partial);

            // now you can continue filling the remaining values, if any.
            return quantileElements;
        }
        #endregion

        #region Local Protected Methods
        protected void AddInfinities(int missingInfinities, DoubleBuffer buffer)
        {
            RandomSamplingAssistant oldAssistant = this.samplingAssistant;
            this.samplingAssistant = null; // switch off sampler
                                           //double[] infinities = new double[missingInfinities];

            Boolean even = true;
            for (int i = 0; i < missingInfinities; i++)
            {
                if (even) buffer.Values.Add(Double.MaxValue);
                else buffer.Values.Add(-Double.MaxValue);

                //if (even) {infinities[i]=Double.MaxValue;}
                //else	  {infinities[i]=-Double.MaxValue;}

                //if (even) {this.Add(Double.MaxValue);}
                //else	  {this.Add(-Double.MaxValue);}
                even = !even;
            }

            //buffer.Values.addAllOfFromTo(new List<Double>(infinities),0,missingInfinities-1);

            //this.totalElementsFilled -= infinities;

            this.samplingAssistant = oldAssistant; // switch on sampler again
        }

        protected new DoubleBuffer[] BuffersToCollapse()
        {
            int minLevel = this.BufferSet.GetMinLevelOfFullOrPartialBuffers();
            return this.BufferSet.GetFullOrPartialBuffersWithLevel(minLevel);
        }

        protected new List<Double> PreProcessPhis(List<Double> phis)
        {
            if (beta > 1.0)
            {
                phis = phis.Copy();
                for (int i = phis.Count; --i >= 0;)
                {
                    phis[i] = (2 * phis[i] + beta - 1) / (2 * beta);
                }
            }
            return phis;
        }

        protected void RemoveInfinitiesFrom(int infinities, DoubleBuffer buffer)
        {
            int plusInf = 0;
            int minusInf = 0;
            // count them (this is not very clever but it's safe)
            Boolean even = true;
            for (int i = 0; i < infinities; i++)
            {
                if (even) plusInf++;
                else minusInf++;
                even = !even;
            }

            buffer.Values.RemoveRange(buffer.Size - plusInf, buffer.Size - 1);
            buffer.Values.RemoveRange(0, minusInf - 1);
            //this.totalElementsFilled -= infinities;
        }
        #endregion

        #region Local Private Methods

        #endregion
    }
}
