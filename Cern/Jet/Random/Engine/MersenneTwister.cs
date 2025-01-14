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

        /* for tempering */
        protected static uint TEMPERING_MASK_B = TEMPER1;
        protected static uint TEMPERING_MASK_C = TEMPER2;

        protected int[] mt = new int[N];
        protected int mti;
        protected static int mag0 = 0x0;
        protected static uint mag1 = MATRIX_A;

        // https://stackoverflow.com/questions/75840391/why-is-the-default-seed-of-mt19937-5489
        protected const int DEFAULT_SEED = 5489;
        protected const int OLD_DEFAULT_SEED = 4357;
        #endregion

        #region Property
        public static int DefaultSeed
        {
            get { return DEFAULT_SEED; }
        }

        public static int OldDefaultSeed
        {
            get { return OLD_DEFAULT_SEED; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs and returns a random number generator with a default seed, which is a <b>constant</b>.
        /// Thus using this constructor will yield generators that always produce exactly the same sequence.
        /// This method is mainly intended to ease testing and debugging.
        /// </summary>
        /// <see href="https://stackoverflow.com/questions/75840391/why-is-the-default-seed-of-mt19937-5489"/>
        public MersenneTwister() : this(DEFAULT_SEED) { }

        /// <summary>
        /// Constructs and returns a random number generator with the given seed.
        /// </summary>
        /// <param name="seed"></param>
        public MersenneTwister(int seed)
        {
            SetSeed((uint)seed);
        }

        /// <summary>
        /// Constructs and returns a random number generator seeded with the given date.
        /// </summary>
        /// <param name="d">DateTime that will get UnixTimeMIlliseconds</param>
        public MersenneTwister(DateTime d): this((int)d.CurrentTimeMillis())
        {
            
        }
        #endregion

        #region Implement Methods

        /// <summary>
        /// Returns a copy of the receiver; the copy will produce identical sequences.
        /// After this call has returned, the copy and the receiver have equal but separate state.
        /// </summary>
        /// <returns>a copy of the receiver.</returns>
        public override RandomEngine Clone()
        {
            RandomEngine copy = (RandomEngine)this.MemberwiseClone();

            return copy;
        }

        /// <summary>
        /// Generates N words at one time.
        /// </summary>
        protected void NextBlock()
        {
            /*
            // ******************** OPTIMIZED **********************
            // only 5-10% faster ?
            int y;

            int kk;
            int[] cache = mt; // cached for speed
            int kkM;
            int limit = N-M;
            for (kk=0,kkM=kk+M; kk<limit; kk++,kkM++) {
                y = (cache[kk]&UPPER_MASK)|(cache[kk+1]&LOWER_MASK);
                cache[kk] = cache[kkM] ^ (y >>> 1) ^ ((y & 0x1) == 0 ? mag0 : mag1);
            }
            limit = N-1;
            for (kkM=kk+(M-N); kk<limit; kk++,kkM++) {
                y = (cache[kk]&UPPER_MASK)|(cache[kk+1]&LOWER_MASK);
                cache[kk] = cache[kkM] ^ (y >>> 1) ^ ((y & 0x1) == 0 ? mag0 : mag1);
            }
            y = (cache[N-1]&UPPER_MASK)|(cache[0]&LOWER_MASK);
            cache[N-1] = cache[M-1] ^ (y >>> 1) ^ ((y & 0x1) == 0 ? mag0 : mag1);

            this.mt = cache;
            this.mti = 0;
            */



            // ******************** UNOPTIMIZED **********************
            int y;
            int kk;

            for (kk = 0; kk < N - M; kk++)
            {
                y = (int)(((uint)mt[kk] & UPPER_MASK) | ((uint)mt[kk + 1] & LOWER_MASK));
                mt[kk] = mt[kk + M] ^ (y >>> 1) ^ ((y & 0x1) == 0 ? mag0 : (int)mag1);
            }
            for (; kk < N - 1; kk++)
            {
                y = (int)(((uint)mt[kk] & UPPER_MASK) | ((uint)mt[kk + 1] & LOWER_MASK));
                mt[kk] = (int)((uint)mt[kk + (M - N)] ^ (y >>> 1) ^ ((y & 0x1) == 0 ? mag0 : mag1));
            }
            y = (int)((mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK));
            mt[N - 1] = (int)(mt[M - 1] ^ (y >>> 1) ^ ((y & 0x1) == 0 ? mag0 : mag1));

            this.mti = 0;
        }

        /// <summary>
        /// Generate new random value of Unsigned Int32
        /// </summary>
        /// <returns>new random value of Unsigned Int32</returns>
        public override uint NextUInt32()
        {
            // generate 32 random bits
            // Discard - Kei Nakai (2024.12.2) ---------------------------------------------------------------------------------
            //uint y;

            //if (mti >= N)
            //{
            //    const uint LOWER_MASK = 2147483647;
            //    const uint UPPER_MASK = 0x80000000;
            //    uint[] mag01 = { 0, MATRIX_A };

            //    int kk;
            //    for (kk = 0; kk < N - M; kk++)
            //    {
            //        y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
            //        mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 1];
            //    }

            //    for (; kk < N - 1; kk++)
            //    {
            //        y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
            //        mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 1];
            //    }

            //    y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
            //    mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 1];
            //    mti = 0;
            //}

            //y = mt[mti++];

            //// Tempering (May be omitted):
            //y ^= y >> TEMPER3;
            //y ^= (y << TEMPER4) & TEMPER1;
            //y ^= (y << TEMPER5) & TEMPER2;
            //y ^= y >> TEMPER6;
            //return y;
            // ------------------------------------------------------------------

            /* Each single bit including the sign bit will be random */
            if (mti == N) NextBlock(); // generate N ints at one time

            uint y = (uint)mt[mti++];
            y ^= y >>> TEMPER3; // y ^= TEMPERING_SHIFT_U(y );
            y ^= (y << TEMPER4) & TEMPERING_MASK_B; // y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
            y ^= (y << TEMPER5) & TEMPERING_MASK_C; // y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;	
                                               // y &= 0xffffffff; //you may delete this line if word size = 32 
            y ^= y >>> TEMPER6; // y ^= TEMPERING_SHIFT_L(y);

            return y;
        }

        /// <summary>
        /// Generate new random value of Int32
        /// </summary>
        /// <returns>new random value of Int32</returns>
        public override int NextInt32()
        {
            // generate 32 random bits
            // Discard - Kei Nakai (2024.12.2) ---------------------------------------------------------------------------------
            //uint y;

            //if (mti >= N)
            //{
            //    const uint LOWER_MASK = 2147483647;
            //    const uint UPPER_MASK = 0x80000000;
            //    uint[] mag01 = { 0, MATRIX_A };

            //    int kk;
            //    for (kk = 0; kk < N - M; kk++)
            //    {
            //        y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
            //        mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 1];
            //    }

            //    for (; kk < N - 1; kk++)
            //    {
            //        y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
            //        mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 1];
            //    }

            //    y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
            //    mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 1];
            //    mti = 0;
            //}

            //y = mt[mti++];

            //// Tempering (May be omitted):
            //y ^= y >> TEMPER3;
            //y ^= (y << TEMPER4) & TEMPER1;
            //y ^= (y << TEMPER5) & TEMPER2;
            //y ^= y >> TEMPER6;
            //return y;
            // ------------------------------------------------------------------

            /* Each single bit including the sign bit will be random */
            if (mti == N) NextBlock(); // generate N ints at one time

            int y = (int)mt[mti++];
            y ^= y >>> TEMPER3; // y ^= TEMPERING_SHIFT_U(y );
            y ^= (int)((uint)(y << TEMPER4) & TEMPERING_MASK_B); // y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
            y ^= (int)((uint)(y << TEMPER5) & TEMPERING_MASK_C); // y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;	
                                                    // y &= 0xffffffff; //you may delete this line if word size = 32 
            y ^= y >>> TEMPER6; // y ^= TEMPERING_SHIFT_L(y);

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
                mt[i] = (int)(((uint)mt[i] ^ (((uint)mt[i - 1] ^ ((uint)mt[i - 1] >> 30)) * 1664525U)) + seeds[j] + (uint)j);
                i++; j++;
                if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
                if (j >= length) j = 0;
            }
            for (k = N - 1; k != 0; k--)
            {
                mt[i] = (int)((mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941U)) - i);
                if (++i >= N) { mt[0] = mt[N - 1]; i = 1; }
            }
            uint ut = 0x80000000U;
            mt[0] = (int)ut; // MSB is 1; assuring non-zero initial array
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
            mt[0] = (int)(seed & 0xffffffff);
            for (int i = 1; i < N; i++)
            {
                mt[i] = (int)((1812433253U * ((uint)mt[i - 1] ^ ((uint)mt[i - 1] >> 30)) + (uint)i));
                /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
                /* In the previous versions, MSBs of the seed affect   */
                /* only MSBs of the array mt[].                        */
                /* 2002/01/09 modified by Makoto Matsumoto             */
                uint ut = 0xffffffff;
                mt[i] &= (int)ut;
                /* for >32 bit machines */
            }
            mti = N;
        }
        #endregion

        #region Local Private Methods


        #endregion
    }
}
