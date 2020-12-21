using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Stat.Quantile
{
    /// <summary>
    /// A set of buffers holding <i>double</i> elements; internally used for computing approximate quantiles.
    /// </summary>
    public class DoubleBufferSet: BufferSet
    {

        #region Local Variables
        protected DoubleBuffer[] buffers;
        private Boolean nextTriggerCalculationState; //tmp var only
        #endregion

        #region Property

        /// <summary>
        /// Gets and sets the buffers.
        /// </summary>
        public DoubleBuffer[] Buffers
        {
            get { return buffers; }
            set { buffers = value; }
        }

        /// <summary>
        /// Return the number of buffers.
        /// </summary>
        public int BufferSize
        {
            get { return buffers.Length; }
        }

        /// <summary>
        /// Gets the number of elements within a buffer.
        /// </summary>
        public int NumberOfElements
        {
            get { return buffers[0].NumberOfElements; }
        }

        /// <summary>
        /// Return the number of buffers.
        /// </summary>
        [Obsolete("B is deprecated, please use BufferSize property instead.")]
        public int B
        {
            get { return buffers.Length; }
        }

        /// <summary>
        /// Gets the number of elements within a buffer.
        /// </summary>
        [Obsolete("K is deprecated, please use NumberOfElements property instead.")]
        public int k
        {
            get { return buffers[0].NumberOfElements; }
        }

        /// <summary>
        /// Gets the number of elements in all buffers.
        /// </summary>
        public long TotalSize
        {
            get
            {
                DoubleBuffer[] fullBuffers = GetFullOrPartialBuffers();
                long totalSize = 0;
                for (int i = fullBuffers.Length; --i >= 0;)
                {
                    totalSize += fullBuffers[i].Size * fullBuffers[i].Weight;
                }

                return totalSize;
            }
        }

        #endregion

        #region Implement Property

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a buffer set with b buffers, each having k elements
        /// </summary>
        /// <param name="b">the number of buffers</param>
        /// <param name="k">the number of elements per buffer</param>
        public DoubleBufferSet(int b, int k)
        {
            this.buffers = new DoubleBuffer[b];
            this.Clear(k);
        }
        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Returns an empty buffer if at least one exists.
        /// Preferably returns a buffer which has already been used,
        /// i.e. a buffer which has already been allocated.
        /// </summary>
        /// <returns></returns>
        public DoubleBuffer GetFirstEmptyBuffer()
        {
            DoubleBuffer emptyBuffer = null;
            for (int i = buffers.Length; --i >= 0;)
            {
                if (buffers[i].IsEmpty)
                {
                    if (buffers[i].IsAllocated) return buffers[i];
                    emptyBuffer = buffers[i];
                }
            }

            return emptyBuffer;
        }

        /// <summary>
        /// Returns all full or partial buffers.
        /// </summary>
        /// <returns></returns>
        public DoubleBuffer[] GetFullOrPartialBuffers()
        {
            //count buffers
            int count = 0;
            for (int i = buffers.Length; --i >= 0;)
            {
                if (!buffers[i].IsEmpty) count++;
            }

            //collect buffers
            DoubleBuffer[] collectedBuffers = new DoubleBuffer[count];
            int j = 0;
            for (int i = buffers.Length; --i >= 0;)
            {
                if (!buffers[i].IsEmpty)
                {
                    collectedBuffers[j++] = buffers[i];
                }
            }

            return collectedBuffers;
        }

        /// <summary>
        /// Determines all full buffers having the specified level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>all full buffers having the specified level</returns>
        public DoubleBuffer[] GetFullOrPartialBuffersWithLevel(int level)
        {
            //count buffers
            int count = 0;
            for (int i = buffers.Length; --i >= 0;)
            {
                if ((!buffers[i].IsEmpty) && buffers[i].Level == level) count++;
            }

            //collect buffers
            DoubleBuffer[] collectedBuffers = new DoubleBuffer[count];
            int j = 0;
            for (int i = buffers.Length; --i >= 0;)
            {
                if ((!buffers[i].IsEmpty) && buffers[i].Level == level)
                {
                    collectedBuffers[j++] = buffers[i];
                }
            }

            return collectedBuffers;
        }

        /// <summary>
        /// Get the minimum level of all buffers which are full.
        /// </summary>
        /// <returns>The minimum level of all buffers which are full.</returns>
        public int GetMinLevelOfFullOrPartialBuffers()
        {
            int b = this.BufferSize;
            int minLevel = int.MaxValue;
            DoubleBuffer buffer;

            for (int i = 0; i < b; i++)
            {
                buffer = this.buffers[i];
                if ((!buffer.IsEmpty) && (buffer.Level < minLevel))
                {
                    minLevel = buffer.Level;
                }
            }
            return minLevel;
        }

        /// <summary>
        /// Returns the number of empty buffers.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfEmptyBuffers()
        {
            int count = 0;
            for (int i = buffers.Length; --i >= 0;)
            {
                if (buffers[i].IsEmpty) count++;
            }

            return count;
        }

        /// <summary>
        /// Returns all empty buffers.
        /// </summary>
        /// <returns></returns>
        public DoubleBuffer GetPartialBuffer()
        {
            for (int i = buffers.Length; --i >= 0;)
            {
                if (buffers[i].IsPartial) return buffers[i];
            }
            return null;
        }

        /// <summary>
        /// Removes all elements from the receiver.  The receiver will
        /// be empty after this call returns, and its memory requirements will be close to zero.
        /// </summary>
        public void Clear()
        {
            Clear(this.NumberOfElements);
        }

        /// <summary>
        /// Returns a deep copy of the receiver.
        /// </summary>
        /// <returns>a deep copy of the receiver.</returns>
        public override Object Clone()
        {
            DoubleBufferSet copy = (DoubleBufferSet)base.Clone();

            copy.buffers = (DoubleBuffer[])copy.buffers.Clone();
            for (int i = buffers.Length; --i >= 0;)
            {
                copy.buffers[i] = (DoubleBuffer)copy.buffers[i].Clone();
            }
            return copy;
        }

        /// <summary>
        /// Collapses the specified full buffers (must not include partial buffer).
        /// </summary>
        /// <param name="buffers">the buffers to be collapsed (all of them must be full or partially full)</param>
        /// <returns>a full buffer containing the collapsed values. The buffer has accumulated weight.</returns>
        public DoubleBuffer Collapse(DoubleBuffer[] buffers)
        {
            //determine W
            int W = 0;                              //sum of all weights
            for (int i = 0; i < buffers.Length; i++) { W += buffers[i].Weight; }

            //determine outputTriggerPositions
            int k = this.NumberOfElements;
            long[] triggerPositions = new long[k];
            for (int j = 0; j < k; j++) { triggerPositions[j] = this.NextTriggerPosition(j, W); }

            //do the main work: determine values at given positions in sorted sequence
            double[] outputValues = this.GetValuesAtPositions(buffers, triggerPositions);

            //mark all full buffers as empty, except the first, which will contain the output
            for (int b = 1; b < buffers.Length; b++) buffers[b].Clear();

            DoubleBuffer outputBuffer = buffers[0];
            outputBuffer.Values.Clear();
            outputBuffer.Values.SetElements(outputValues);
            outputBuffer.Weight =W;

            return outputBuffer;
        }

        /// <summary>
        /// Returns whether the specified element is contained in the receiver.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Boolean Contains(double element)
        {
            for (int i = buffers.Length; --i >= 0;)
            {
                if ((!buffers[i].IsEmpty) && buffers[i].Contains(element))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Applies a procedure to each element of the receiver, if any.
        /// Iterates over the receiver in no particular order.
        /// </summary>
        /// <param name="procedure">the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. </param>
        /// <returns></returns>
        public Boolean ForEach(Cern.Colt.Function.DoubleProcedure procedure)
        {
            for (int i = buffers.Length; --i >= 0;)
            {
                for (int w = buffers[i].Weight; --w >= 0;)
                {
                    if (!(buffers[i].Values.ForEach(procedure))) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the number of elements currently needed to store all contained elements.
        /// </summary>
        /// <returns></returns>
        public long Memory()
        {
            long memory = 0;
            for (int i = buffers.Length; --i >= 0;)
            {
                memory = memory + buffers[i].Memory();
            }
            return memory;
        }

        /// <summary>
        /// Returns how many percent of the elements contained in the receiver are <tt>&lt;= element</tt>.
        /// Does linear interpolation if the element is not contained but lies in between two contained elements.
        /// </summary>
        /// <param name="element">the element to search for.</param>
        /// <returns>the percentage <tt>p</tt> of elements <tt>&lt;= element</tt> (<tt>0.0 &lt;= p &lt;=1.0)</tt>.</returns>
        public double Phi(double element)
        {
            double elementsLessThanOrEqualToElement = 0.0;
            for (int i = buffers.Length; --i >= 0;)
            {
                if (!buffers[i].IsEmpty)
                {
                    elementsLessThanOrEqualToElement += buffers[i].Weight * buffers[i].Rank(element);
                }
            }

            return elementsLessThanOrEqualToElement / TotalSize;
        }

        /// <summary>
        /// Return a String representation of the receiver
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            for (int b = 0; b < this.BufferSize; b++)
            {
                if (!buffers[b].IsEmpty)
                {
                    buf.Append("buffer#" + b + " = ");
                    buf.Append(buffers[b].ToString() + "\n");
                }
            }
            return buf.ToString();
        }

        #endregion

        #region Local internal Methods
        /// <summary>
        /// Removes all elements from the receiver.  The receiver will
        /// be empty after this call returns, and its memory requirements will be close to zero.
        /// </summary>
        /// <param name="k"></param>
        internal void Clear(int k)
        {
            for (int i = BufferSize; --i >= 0;) this.buffers[i] = new DoubleBuffer(k);
            this.nextTriggerCalculationState = true;
        }

        /// <summary>
        /// Determines all values of the specified buffers positioned at the specified triggerPositions within the sorted sequence and fills them into outputValues.
        /// </summary>
        /// <param name="buffers">the buffers to be searched (all must be full or partial) </param>
        /// <param name="triggerPositions">the positions of elements within the sorted sequence to be retrieved</param>
        /// <returns>outputValues a list filled with the values at triggerPositions</returns>
        internal double[] GetValuesAtPositions(DoubleBuffer[] buffers, long[] triggerPositions)
        {
            //if (buffers.Length==0) 
            //{
            //	throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_BufferLengthIsZero);
            //}

            //System.out.println("triggers="+cern.it.util.Arrays.ToString(positions));

            //new List<Double>(outputValues).FillFromToWith(0, outputValues.Length-1, 0.0f);
            //delte the above line, it is only for testing

            //cern.it.util.Log.println("\nEntering getValuesAtPositions...");
            //cern.it.util.Log.println("hitPositions="+cern.it.util.Arrays.ToString(positions));

            // sort buffers.
            for (int i = buffers.Length; --i >= 0;)
            {
                buffers[i].Sort();
            }

            // collect some infos into fast cache; for tuning purposes only.
            int[] bufferSizes = new int[buffers.Length];
            double[][] bufferValues = new double[buffers.Length][];
            int totalBuffersSize = 0;
            for (int i = buffers.Length; --i >= 0;)
            {
                bufferSizes[i] = buffers[i].Size;
                bufferValues[i] = buffers[i].Values.ToArray();
                totalBuffersSize += bufferSizes[i];
                //cern.it.util.Log.println("buffer["+i+"]="+buffers[i].Values);
            }

            // prepare merge of equi-distant elements within buffers into output values

            // first collect some infos into fast cache; for tuning purposes only.
            int buffersSize = buffers.Length;
            int triggerPositionsLength = triggerPositions.Length;

            // now prepare the important things.
            int j = 0;                              //current position in collapsed values
            int[] cursors = new int[buffers.Length];    //current position in each buffer; init with zeroes
            long counter = 0;                       //current position in sorted sequence
            long nextHit = triggerPositions[j];     //next position in sorted sequence to trigger output population
            double[] outputValues = new double[triggerPositionsLength];

            if (totalBuffersSize == 0)
            {
                // nothing to output, because no elements have been filled (we are empty).
                // return meaningless values
                for (int i = 0; i < triggerPositions.Length; i++)
                {
                    outputValues[i] = Double.NaN;
                }
                return outputValues;
            }

            // fill all output values with equi-distant elements.
            while (j < triggerPositionsLength)
            {
                //System.out.println("\nj="+j);
                //System.out.println("counter="+counter);
                //System.out.println("nextHit="+nextHit);

                // determine buffer with smallest value at cursor position.
                double minValue = Double.PositiveInfinity;
                int minBufferIndex = -1;

                for (int b = buffersSize; --b >= 0;)
                {
                    //DoubleBuffer buffer = buffers[b];
                    //if (cursors[b] < buffer.Length) { 
                    if (cursors[b] < bufferSizes[b])
                    {
                        ///double value = buffer.Values[cursors[b]];
                        double value = bufferValues[b][cursors[b]];
                        if (value <= minValue)
                        {
                            minValue = value;
                            minBufferIndex = b;
                        }
                    }
                }

                DoubleBuffer minBuffer = buffers[minBufferIndex];

                // trigger copies into output sequence, if necessary.
                counter += minBuffer.Weight;
                while (counter > nextHit && j < triggerPositionsLength)
                {
                    outputValues[j++] = minValue;
                    //System.out.println("adding to output="+minValue);
                    if (j < triggerPositionsLength) nextHit = triggerPositions[j];
                }


                // that element has now been treated, move further.
                cursors[minBufferIndex]++;
                //System.out.println("cursors="+cern.it.util.Arrays.ToString(cursors));

            } //end while (j<k)

            //cern.it.util.Log.println("returning output="+cern.it.util.Arrays.ToString(outputValues));
            return outputValues;
        }

        /// <summary>
        /// Computes the next triggerPosition for collapse
        /// </summary>
        /// <param name="j">specifies that the j-th trigger position is to be computed</param>
        /// <param name="W">the accumulated weights</param>
        /// <returns>the next triggerPosition for collapse</returns>
        internal long NextTriggerPosition(int j, long W)
        {
            long nextTriggerPosition;

            if (W % 2L != 0)
            { //is W odd?
                nextTriggerPosition = j * W + (W + 1) / 2;
            }

            else
            { //W is even
              //alternate between both possible next hit positions upon successive invocations
                if (nextTriggerCalculationState)
                {
                    nextTriggerPosition = j * W + W / 2;
                }
                else
                {
                    nextTriggerPosition = j * W + (W + 2) / 2;
                }
            }

            return nextTriggerPosition;
        }

        #endregion

        #region Local Private Methods

        #endregion

    }
}
