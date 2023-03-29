using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Random.Engine
{
    /// <summary>
    /// Mersenne Twister Random Number Generation
    /// </summary>
    /// <see href="https://www.prowaretech.com/articles/current/dot-net/mersenne-twister-random-number-generator#!"></see>
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

        protected uint[] mt = new uint[N];
        protected uint mti;

        protected const int DEFAULT_SEED = 4357;
        #endregion

        #region Property
        public static int DefaultSeed
        {
            get { return DEFAULT_SEED; }
        }
        #endregion

        #region Constructor
        public MersenneTwister() : this(Guid.NewGuid().GetHashCode()) { }

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
            // generate 32 random bits
            uint y;

            if (mti >= N)
            {
                const uint LOWER_MASK = 2147483647;
                const uint UPPER_MASK = 0x80000000;
                uint[] mag01 = { 0, MATRIX_A };

                int kk;
                for (kk = 0; kk < N - M; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 1];
                }

                for (; kk < N - 1; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 1];
                }

                y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 1];
                mti = 0;
            }

            y = mt[mti++];

            // Tempering (May be omitted):
            y ^= y >> TEMPER3;
            y ^= (y << TEMPER4) & TEMPER1;
            y ^= (y << TEMPER5) & TEMPER2;
            y ^= y >> TEMPER6;
            return y;
        }

        public override double NextDouble()
        {
            // output random float number in the interval 0 <= x < 1
            uint r = NextUInt32(); // get 32 random bits
            if (BitConverter.IsLittleEndian)
            {
                byte[] i0 = BitConverter.GetBytes((r << 20));
                byte[] i1 = BitConverter.GetBytes(((r >> 12) | 0x3FF00000));
                byte[] bytes = { i0[0], i0[1], i0[2], i0[3], i1[0], i1[1], i1[2], i1[3] };
                double f = BitConverter.ToDouble(bytes, 0);
                return f - 1.0;
            }
            return r * (1.0 / (0xFFFFFFFF + 1.0));
        }
        #endregion

        #region Local Public Methods

        public void RandomInitByArray(uint[] seeds)
        {
            // seed by more than 32 bits
            uint i, j;
            int k;
            int length = seeds.Length;
            SetSeed(19650218U);
            if (length <= 0) return;
            i = 1; j = 0;
            k = (N > length ? N : length);
            for (; k != 0; k--)
            {
                mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U)) + seeds[j] + j;
                i++; j++;
                if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
                if (j >= length) j = 0;
            }
            for (k = N - 1; k != 0; k--)
            {
                mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941U)) - i;
                if (++i >= N) { mt[0] = mt[N - 1]; i = 1; }
            }
            mt[0] = 0x80000000U; // MSB is 1; assuring non-zero initial array
        }

        public int IRandom(int min, int max)
        {
            // output random integer in the interval min <= x <= max
            int r;
            r = (int)((max - min + 1) * NextDouble()) + min; // multiply interval with random and truncate
            if (r > max)
                r = max;
            if (max < min)
                return -2147483648;
            return r;
        }

        /// <summary>
        /// Sets the receiver's seed. 
        /// This method resets the receiver's entire internal state.
        /// </summary>
        /// <param name="seed"></param>
        protected void SetSeed(uint seed)
        {
            mt[0] = seed;
            for (mti = 1; mti < N; mti++)
                mt[mti] = (1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
        }
        #endregion

        #region Local Private Methods


        #endregion
    }
}
