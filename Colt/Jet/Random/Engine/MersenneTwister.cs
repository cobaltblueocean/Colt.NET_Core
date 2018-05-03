using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Random.Engine
{
    public class MersenneTwister : RandomEngine
    {

        #region Local Variables
        protected const int N = 624;
        protected const int M = 397;
        protected const uint MATRIX_A = 0x9908b0df;
        protected const uint UPPER_MASK = 0x80000000;
        protected const uint LOWER_MASK = 0x7fffffff;
        protected const uint TEMPER1 = 0x9d2c5680;
        protected const uint TEMPER2 = 0xefc60000;
        protected const int TEMPER3 = 11;
        protected const int TEMPER4 = 7;
        protected const int TEMPER5 = 15;
        protected const int TEMPER6 = 18;

        protected uint[] mt;
        protected int mti;
        private uint[] mag01;

        protected const int DEFAULT_SEED = 4357;
        #endregion

        #region Property

        #endregion

        #region Constructor
        public MersenneTwister() : this(Environment.TickCount) { }

        public MersenneTwister(int seed)
        {
            SetSeed((uint)seed);
        }

        public MersenneTwister(DateTime d): this((int)d.Ticks)
        {
            
        }
        #endregion

        #region Implement Methods

        public override RandomEngine Clone()
        {
            RandomEngine copy = (RandomEngine)this.MemberwiseClone();

            return copy;
        }

        /// <summary>
        /// Generate new random value of Unsigned Int32
        /// </summary>
        /// <returns>new random value of Unsigned Int32</returns>
        public override uint NextUInt32()
        {
            uint y;
            if (mti >= N) { NextBlock(); mti = 0; }
            y = mt[mti++];
            y ^= (y >> TEMPER3);
            y ^= (y << TEMPER4) & TEMPER1;
            y ^= (y << TEMPER5) & TEMPER2;
            y ^= (y >> TEMPER6);
            return y;
        }

        #endregion

        #region Local Public Methods

        /// <summary>
        /// Generates N words at one time.
        /// </summary>
        protected void NextBlock()
        {
            int kk = 1;
            uint y;
            uint p;
            y = mt[0] & UPPER_MASK;
            do
            {
                p = mt[kk];
                mt[kk - 1] = mt[kk + (M - 1)] ^ ((y | (p & LOWER_MASK)) >> 1) ^ mag01[p & 1];
                y = p & UPPER_MASK;
            } while (++kk < N - M + 1);
            do
            {
                p = mt[kk];
                mt[kk - 1] = mt[kk + (M - N - 1)] ^ ((y | (p & LOWER_MASK)) >> 1) ^ mag01[p & 1];
                y = p & UPPER_MASK;
            } while (++kk < N);
            p = mt[0];
            mt[N - 1] = mt[M - 1] ^ ((y | (p & LOWER_MASK)) >> 1) ^ mag01[p & 1];
        }

        /// <summary>
        /// Returns a 32 bit uniformly distributed random number in the closed interval <tt>[Integer.MinValue,Integer.MaxValue]</tt> (including <tt>Integer.MinValue</tt> and <tt>Integer.MaxValue</tt>).
        /// </summary>
        /// <returns></returns>
        public uint NextInt()
        {
            /* Each single bit including the sign bit will be random */
            if (mti == N) NextBlock(); // generate N ints at one time

            uint y = mt[mti++];
            //
            y ^= (uint.Parse(y.ToString()) >> 11); // y ^= TEMPERING_SHIFT_U(y );
            y ^= (y << 7) & TEMPER1; // y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
            y ^= (y << 15) & TEMPER2; // y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;	
                                               // y &= 0xffffffff; //you may delete this line if word size = 32 
            y ^= (uint.Parse(y.ToString()) >> 18); // y ^= TEMPERING_SHIFT_L(y);

            return y;
        }

        /// <summary>
        /// Sets the receiver's seed. 
        /// This method resets the receiver's entire internal state.
        /// </summary>
        /// <param name="seed"></param>
        protected void SetSeed(uint seed)
        {
            mt = new uint[N];
            mti = N + 1;
            mag01 = new uint[] { 0x0U, MATRIX_A };
            //initialize
            mt[0] = seed;
            for (int i = 1; i < N; i++)
                mt[i] = (uint)(1812433253 * (mt[i - 1] ^ (mt[i - 1] >> 30)) + i);
        }
        #endregion

        #region Local Private Methods

        #endregion
    }
}
