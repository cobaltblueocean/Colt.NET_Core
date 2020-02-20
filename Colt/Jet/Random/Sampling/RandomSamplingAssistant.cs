using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random.Sampling
{
    /// <summary>
    /// Conveniently computes a stable <i>Simple Random Sample Without Replacement (SRSWOR)</i> subsequence of <tt>n</tt> elements from a given input sequence of <tt>N</tt> elements;
    /// Example: Computing a sublist of <tt>n=3</tt> random elements from a list <tt>(1,...,50)</tt> may yield the sublist <tt>(7,13,47)</tt>.
    /// The subsequence is guaranteed to be <i>stable</i>, i.e. elements never change position relative to each other.
    /// Each element from the <tt>N</tt> elements has the same probability to be included in the <tt>n</tt> chosen elements.
    /// This class is a convenience adapter for <tt>RandomSampler</tt> using blocks.
    /// </summary>
    public class RandomSamplingAssistant: Cern.Colt.PersistentObject
    {

        #region Local Variables
        protected RandomSampler sampler;
        protected long[] buffer;
        protected int bufferPosition;

        protected long skip;
        protected long n;

        static int MAX_BUFFER_SIZE = 200;
        #endregion

        #region Property
        /// <summary>
        /// Gets the used random generator.
        /// </summary>
        public RandomEngine RandomGenerator
        {
            get { return this.sampler.RandomGenerator; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a random sampler that samples <tt>n</tt> random elements from an input sequence of <tt>N</tt> elements.
        /// </summary>
        /// <param name="n">the total number of elements to choose (must be &gt;= 0).</param>
        /// <param name="N">number of elements to choose from (must be &gt;= n).</param>
        /// <param name="randomGenerator">a random number generator. Set this parameter to <tt>null</tt> to use the default random number generator.</param>
        public RandomSamplingAssistant(long n, long N, RandomEngine randomGenerator)
        {
            this.n = n;
            this.sampler = new RandomSampler(n, N, 0, randomGenerator);
            this.buffer = new long[(int)System.Math.Min(n, MAX_BUFFER_SIZE)];
            if (n > 0) this.buffer[0] = -1; // start with the right offset

            FetchNextBlock();
        }
        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Returns a deep copy of the receiver.
        /// </summary>
        /// <returns></returns>
        public override Object Clone()
        {
            RandomSamplingAssistant copy = (RandomSamplingAssistant)base.Clone();
            copy.sampler = (RandomSampler)this.sampler.Clone();
            return copy;
        }

        /// <summary>
        /// Tests random sampling.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(String[] args)
        {
            long n = long.Parse(args[0]);
            long N = long.Parse(args[1]);
            //test(n,N);
            TestArraySampling((int)n, (int)N);
        }

        /// <summary>
        /// Just shows how this class can be used; samples n elements from and int[] array.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static int[] SampleArray(int n, int[] elements)
        {
            RandomSamplingAssistant assistant = new RandomSamplingAssistant(n, elements.Length, null);
            int[] sample = new int[n];
            int j = 0;
            int Length = elements.Length;
            for (int i = 0; i < Length; i++)
            {
                if (assistant.SampleNextElement()) sample[j++] = elements[i];
            }
            return sample;
        }

        /// <summary>
        /// Returns whether the next element of the input sequence shall be sampled (picked) or not.
        /// </summary>
        /// <returns><tt>true</tt> if the next element shall be sampled (picked), <tt>false</tt> otherwise.</returns>
        public Boolean SampleNextElement()
        {
            if (n == 0) return false; //reject
            if (skip-- > 0) return false; //reject

            //accept
            n--;
            if (bufferPosition < buffer.Length - 1)
            {
                skip = buffer[bufferPosition + 1] - buffer[bufferPosition++];
                --skip;
            }
            else
            {
                FetchNextBlock();
            }

            return true;
        }

        /// <summary>
        /// Tests the methods of this class.
        /// To do benchmarking, comment the lines printing stuff to the console.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="N"></param>
        public static void Test(long n, long N)
        {
            RandomSamplingAssistant assistant = new RandomSamplingAssistant(n, N, null);

            List<long> sample = new List<long>((int)n);
            DateTime begin = DateTime.Now;

            for (long i = 0; i < N; i++)
            {
                if (assistant.SampleNextElement())
                {
                    sample.Add(i);
                }

            }

            DateTime end = DateTime.Now;
            var diff = end.Subtract(begin).TotalMilliseconds;

            Console.WriteLine(diff);
            Console.WriteLine("sample=" + sample);
            Console.WriteLine("Good bye.\n");
        }

        /// <summary>
        /// Tests the methods of this class.
        /// To do benchmarking, comment the lines printing stuff to the console.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="N"></param>
        public static void TestArraySampling(int n, int N)
        {
            int[] elements = new int[N];
            for (int i = 0; i < N; i++) elements[i] = i;

            DateTime begin = DateTime.Now;

            int[] sample = SampleArray(n, elements);


            DateTime end = DateTime.Now;
            var diff = end.Subtract(begin).TotalMilliseconds;

            Console.WriteLine(diff);

            /*
            System.out.print("\nElements = [");
            for (int i=0; i<N-1; i++) System.out.print(elements[i]+", ");
            System.out.print(elements[N-1]);
            Console.WriteLine("]");


            System.out.print("\nSample = [");
            for (int i=0; i<n-1; i++) System.out.print(sample[i]+", ");
            System.out.print(sample[n-1]);
            Console.WriteLine("]");
            */

            Console.WriteLine("Good bye.\n");
        }

        #endregion

        #region Local Protected Methods
        /// <summary>
        /// 
        /// </summary>
        protected void FetchNextBlock()
        {
            if (n > 0)
            {
                long last = buffer[bufferPosition];
                sampler.NextBlock((int)System.Math.Min(n, MAX_BUFFER_SIZE), buffer, 0);
                skip = buffer[0] - last - 1;
                bufferPosition = 0;
            }
        }
        #endregion

        #region Local Private Methods
        /// <summary>
        /// Returns whether the next elements of the input sequence shall be sampled (picked) or not.
        /// one is chosen from the first block, one from the second, ..., one from the last block.
        /// </summary>
        /// <param name="acceptList">a bitvector which will be filled with <tt>true</tt> where sampling shall occur and <tt>false</tt> where it shall not occur.</param>
        private void XSampleNextElements(List<Boolean> acceptList)
        {
            // manually inlined
            int Length = acceptList.Count;
            Boolean[] accept = acceptList.ToArray();
            for (int i = 0; i < Length; i++)
            {
                if (n == 0)
                {
                    accept[i] = false;
                    continue;
                } //reject
                if (skip-- > 0)
                {
                    accept[i] = false;
                    continue;
                } //reject

                //accept
                n--;
                if (bufferPosition < buffer.Length - 1)
                {
                    skip = buffer[bufferPosition + 1] - buffer[bufferPosition++];
                    --skip;
                }
                else
                {
                    FetchNextBlock();
                }

                accept[i] = true;
            }
        }
        #endregion

    }
}
