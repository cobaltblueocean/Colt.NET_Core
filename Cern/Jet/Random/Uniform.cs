using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random
{
    /// <summary>
    /// Uniform distribution; <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node292.html#SECTION0002920000000000000000"> Math definition</A>
    /// and <A HREF="http://www.statsoft.com/textbook/glosu.html#Uniform Distribution"> animated definition</A>.
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unlock.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are lock.
    /// <p>
    /// </summary>
    public class Uniform : AbstractContinousDistribution
    {

        #region Local Variables
        protected double min;
        protected double max;

        private object syncLock;

        // The uniform random number generated shared by all <b>static</b> methodsd 
        protected static Uniform shared = new Uniform(MakeDefaultGenerator());

        #endregion

        #region Property

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a uniform distribution with the given minimum and maximum, using a <see cref="MersenneTwister"/> seeded with the given seed.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="seed"></param>
        public Uniform(double min, double max, int seed): this(min, max, new MersenneTwister(seed))
        {
            
        }

        /// <summary>
        /// Constructs a uniform distribution with the given minimum and maximum.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="randomGenerator"></param>
        public Uniform(double min, double max, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetState(min, max);
        }

        /// <summary>
        /// Constructs a uniform distribution with <tt>min=0.0</tt> and <tt>max=1.0</tt>.
        /// </summary>
        /// <param name="randomGenerator"></param>
        public Uniform(RandomEngine randomGenerator): this(0, 1, randomGenerator)
        {
            
        }
        #endregion

        #region Implement Methods
        public override double NextDouble()
        {
            return min + (max - min) * randomGenerator.Raw();
        }

        public override int NextInt()
        {
            return NextIntFromTo((int)System.Math.Round(min), (int)System.Math.Round(max));
        }

        #endregion

        #region Local Public Methods
        public double CDF(double x)
        {
            if (x <= min) return 0.0;
            if (x >= max) return 1.0;
            return (x - min) / (max - min);
        }

        public Boolean NextBoolean()
        {
            return randomGenerator.Raw() > 0.5;
        }

        public double NextDoubleFromTo(double from, double to)
        {
            return from + (to - from) * randomGenerator.Raw();
        }

        public float NextFloatFromTo(float from, float to)
        {
            return (float)NextDoubleFromTo(from, to);
        }

        public int NextIntFromTo(int from, int to)
        {
            return (int)((long)from + (long)((1L + (long)to - (long)from) * randomGenerator.Raw()));
        }

        public long NextLongFromTo(long from, long to)
        {
            /* Doing the thing turns out to be more tricky than expected.
               avoids overflows and underflows.
               treats cases like from=-1, to=1 and the like right.
               the following code would NOT solve the problem: return (long)(Doubles.randomFromTo(from,to));

               rounding avoids the unsymmetric behaviour of casts from double to long: (long) -0.7 = 0, (long) 0.7 = 0.
               checking for overflows and underflows is also necessary.
            */

            // first the most likely and also the fastest case.
            if (from >= 0 && to < long.MaxValue)
            {
                return from + (long)(NextDoubleFromTo(0.0, to - from + 1));
            }

            // would we get a numeric overflow?
            // if not, we can still handle the case rather efficient.
            double diff = ((double)to) - (double)from + 1.0;
            if (diff <= long.MaxValue)
            {
                return from + (long)(NextDoubleFromTo(0.0, diff));
            }

            // now the pathologic boundary cases.
            // they are handled rather slow.
            long random;
            if (from == long.MinValue)
            {
                if (to == long.MaxValue)
                {
                    //return System.Math.Round(nextDoubleFromTo(from,to));
                    int i1 = NextIntFromTo(int.MinValue, int.MaxValue);
                    int i2 = NextIntFromTo(int.MinValue, int.MaxValue);
                    return ((i1 & 0xFFFFFFFFL) << 32) | (i2 & 0xFFFFFFFFL);
                }
                random = (long)System.Math.Round(NextDoubleFromTo((double)from, (double)to + 1));
                if (random > to) random = from;
            }
            else
            {
                random = (long)System.Math.Round(NextDoubleFromTo((double)from - 1, (double)to));
                if (random < from) random = to;
            }
            return random;
        }

        public double PDF(double x)
        {
            if (x <= min || x >= max) return 0.0;
            return 1.0 / (max - min);
        }

        /// <summary>
        /// Sets the internal state.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetState(double min, double max)
        {
            if (max < min) { SetState(max, min); return; }
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Returns a uniformly distributed random <tt>Boolean</tt>.
        /// </summary>
        /// <returns></returns>
        public static Boolean StaticNextBoolean()
        {
            lock(shared) {
                return shared.NextBoolean();
            }
        }

        /// <summary>
        /// Returns a uniformly distributed random number in the open interval <tt>(0,1)</tt> (excluding <tt>0</tt> and <tt>1</tt>).
        /// </summary>
        /// <returns></returns>
        public static double StaticNextDouble()
        {
            //Interlocked.Read(ref shared.nextDouble());
            lock (shared)
            {
                return shared.NextDouble();
            }
        }

        /// <summary>
        /// Returns a uniformly distributed random number in the open interval <tt>(from,to)</tt> (excluding <tt>from</tt> and <tt>to</tt>).
        /// Pre conditions: <tt>from &lt;= to</tt>.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double StaticNextDoubleFromTo(double from, double to)
        {
            lock(shared) {
                return shared.NextDoubleFromTo(from, to);
            }
        }

        /// <summary>
        /// Returns a uniformly distributed random number in the open interval <tt>(from,to)</tt> (excluding <tt>from</tt> and <tt>to</tt>).
        /// Pre conditions: <tt>from &lt;= to</tt>.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float StaticNextFloatFromTo(float from, float to)
        {
            lock(shared) {
                return shared.NextFloatFromTo(from, to);
            }
        }

        /// <summary>
        /// Returns a uniformly distributed random number in the closed interval <tt>[from,to]</tt> (including <tt>from</tt> and <tt>to</tt>).
        /// Pre conditions: <tt>from &lt;= to</tt>.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int StaticNextIntFromTo(int from, int to)
        {
            lock(shared) {
                return shared.NextIntFromTo(from, to);
            }
        }

        /// <summary>
        /// Returns a uniformly distributed random number in the closed interval <tt>[from,to]</tt> (including <tt>from</tt> and <tt>to</tt>).
        /// Pre conditions: <tt>from &lt;= to</tt>.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static long StaticNextLongFromTo(long from, long to)
        {
            lock(shared) {
                return shared.NextLongFromTo(from, to);
            }
        }

        /// <summary>
        /// Sets the uniform random number generation engine shared by all <b>static</b> methods.
        /// </summary>
        /// <param name="randomGenerator">the new uniform random number generation engine to be shared.</param>
        public static void StaticSetRandomEngine(RandomEngine randomGenerator)
        {
            lock(shared) {
                shared.RandomGenerator = randomGenerator;
            }
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + min + "," + max + ")";
        }
        #endregion

        #region Local Private Methods

        #endregion

    }
}
