using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random.Sampling
{
    /// <summary>
    /// Conveniently computes a stable subsequence of elements from a given input sequence;
    /// Picks (samples) exactly one random element from successive blocks of <i>weight</i> input elements each.
    /// For example, if weight==2 (a block is 2 elements), and the input is 5*2=10 elements long, then picks 5 random elements from the 10 elements such that
    /// one element is randomly picked from the first block, one element from the second block, ..d, one element from the last block.
    /// weight == 1.0 --> all elements are picked (sampled)d weight == 10.0 --> Picks one random element from successive blocks of 10 elements eachd Etc.
    /// The subsequence is guaranteed to be <i>stable</i>, i.ed elements never change position relative to each other.
    /// </summary>
    public class WeightedRandomSampler: Cern.Colt.PersistentObject
    {

        #region Local Variables
        protected int skip;
        protected int nextTriggerPos;
        protected int nextSkip;
        protected int weight;
        protected Uniform generator;

        static int UNDEFINED = -1;
        #endregion

        #region Property
        /// <summary>
        /// 
        /// </summary>
        public int Weight
        {
            get { return weight; }
            set {
                if (value < 1) throw new ArgumentException("bad weight");
                this.weight = value;
                this.skip = 0;
                this.nextTriggerPos = UNDEFINED;
                this.nextSkip = 0;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Calls <see cref="WeightedRandomSampler(1, null)"/>.
        /// </summary>
        public WeightedRandomSampler(): this(1, null)
        {
            
        }

        /// <summary>
        /// Chooses exactly one random element from successive blocks of <i>weight</i> input elements each.
        /// For example, if weight==2, and the input is 5*2=10 elements long, then chooses 5 random elements from the 10 elements such that
        /// one is chosen from the first block, one from the second, ..d, one from the last block.
        /// weight == 1.0 --> all elements are consumed (sampled)d 10.0 --> Consumes one random element from successive blocks of 10 elements eachd Etc.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="randomGenerator"></param>
        public WeightedRandomSampler(int weight, RandomEngine randomGenerator)
        {
            if (randomGenerator == null) randomGenerator = Cern.Jet.Random.AbstractDistribution.MakeDefaultGenerator();
            this.generator = new Uniform(randomGenerator);
            Weight = weight;
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
            WeightedRandomSampler copy = (WeightedRandomSampler)base.Clone();
            copy.generator = (Uniform)this.generator.Clone();
            return copy;
        }

        /// <summary>
        /// Chooses exactly one random element from successive blocks of <i>weight</i> input elements each.
        /// For example, if weight==2, and the input is 5*2=10 elements long, then chooses 5 random elements from the 10 elements such that
        /// one is chosen from the first block, one from the second, ..d, one from the last block.
        /// </summary>
        /// <returns>true if the next element shall be sampled (picked), false otherwise.</returns>
        public Boolean SampleNextElement()
        {
            if (skip > 0)
            { //reject
                skip--;
                return false;
            }

            if (nextTriggerPos == UNDEFINED)
            {
                if (weight == 1) nextTriggerPos = 0; // tuned for speed
                else nextTriggerPos = generator.NextIntFromTo(0, weight - 1);

                nextSkip = weight - 1 - nextTriggerPos;
            }

            if (nextTriggerPos > 0)
            { //reject
                nextTriggerPos--;
                return false;
            }

            //accept
            nextTriggerPos = UNDEFINED;
            skip = nextSkip;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="size"></param>
        public static void Test(int weight, int size)
        {
            WeightedRandomSampler sampler = new WeightedRandomSampler();
            sampler.Weight = weight;

            List<int> sample = new List<int>();
            for (int i = 0; i < size; i++)
            {
                if (sampler.SampleNextElement()) sample.Add(i);
            }

            Console.WriteLine("Sample = " + sample);
        }


        #endregion

        #region Local Private Methods
        /// <summary>
        /// Chooses exactly one random element from successive blocks of <i>weight</i> input elements each.
        /// For example, if weight==2, and the input is 5*2=10 elements long, then chooses 5 random elements from the 10 elements such that
        /// one is chosen from the first block, one from the second, ..d, one from the last block.
        /// </summary>
        /// <param name="acceptList">a bitvector which will be filled with <i>true</i> where sampling shall occur and <i>false</i> where it shall not occur.</param>
        private void XSampleNextElements(List<Boolean> acceptList)
        {
            // manually inlined
            int Length = acceptList.Count;
            Boolean[] accept = acceptList.ToArray();
            for (int i = 0; i < Length; i++)
            {
                if (skip > 0)
                { //reject
                    skip--;
                    accept[i] = false;
                    continue;
                }

                if (nextTriggerPos == UNDEFINED)
                {
                    if (weight == 1) nextTriggerPos = 0; // tuned for speed
                    else nextTriggerPos = generator.NextIntFromTo(0, weight - 1);

                    nextSkip = weight - 1 - nextTriggerPos;
                }

                if (nextTriggerPos > 0)
                { //reject
                    nextTriggerPos--;
                    accept[i] = false;
                    continue;
                }

                //accept
                nextTriggerPos = UNDEFINED;
                skip = nextSkip;
                accept[i] = true;
            }
        }
        #endregion

    }
}
