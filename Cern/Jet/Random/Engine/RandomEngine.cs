using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;

namespace Cern.Jet.Random.Engine
{
    /// <summary>
    /// Abstract base class for uniform pseudo-random number generating engines.
    /// <p>
    /// Most probability distributions are obtained by using a <b>uniform</b> pseudo-random number generation engine 
    /// followed by a transformation to the desired distribution.
    /// Thus, subclasses of this class are at the core of computational statistics, simulations, Monte Carlo methods, etc.
    /// <p>
    /// Subclasses produce uniformly distributed <tt>int</tt>'s and <tt>long</tt>'s in the closed intervals <tt>[Integer.MIN_VALUE,Integer.MAX_VALUE]</tt> and <tt>[Long.MIN_VALUE,Long.MAX_VALUE]</tt>, respectively, 
    /// as well as <tt>float</tt>'s and <tt>double</tt>'s in the open unit intervals <tt>(0.0f,1.0f)</tt> and <tt>(0.0,1.0)</tt>, respectively.
    /// <p>
    /// Subclasses need to override one single method only: <tt>nextInt()</tt>.
    /// All other methods generating different data types or ranges are usually layered upon <tt>nextInt()</tt>.
    /// <tt>long</tt>'s are formed by concatenating two 32 bit <tt>int</tt>'s.
    /// <tt>float</tt>'s are formed by dividing the interval <tt>[0.0f,1.0f]</tt> into 2<sup>32</sup> sub intervals, then randomly choosing one subinterval.
    /// <tt>double</tt>'s are formed by dividing the interval <tt>[0.0,1.0]</tt> into 2<sup>64</sup> sub intervals, then randomly choosing one subinterval.
    /// <p>
    /// Note that this implementation is <b>not synchronized</b>.
    /// </summary>
    public abstract class RandomEngine
    {
        public IntFunctionDelegate ApplyIntFunction()
        {
            return new IntFunctionDelegate((a) => { return NextInt32(); });
        }

        public DoubleFunctionDelegate ApplyDoubleFunction()
        {
            return new DoubleFunctionDelegate((a) => { return Raw(); });
        }

        /// <summary>
        /// Equivalent to <tt><see cref="Raw()"/></tt>.
        /// This has the effect that random engines can now be used as function objects, returning a random number upon function evaluation.
        /// </summary>
        /// <param name="dummy"></param>
        /// <returns></returns>
        public double Apply(double dummy)
        {
            return Raw();
        }

        /// <summary>
        /// Equivalent to <tt><see cref="NextInt32()"/></tt>.
        /// This has the effect that random engines can now be used as function objects, returning a random number upon function evaluation.
        /// </summary>
        /// <param name="dummy"></param>
        /// <returns></returns>
        public int Apply(int dummy)
        {
            return NextInt32();
        }

        /// <summary>
        /// Make a copy of this object
        /// </summary>
        /// <returns></returns>
        public abstract RandomEngine Clone();

        /// <summary>
        /// Constructs and returns a new uniform random number engine seeded with the current time.
        /// Currently this is <see cref="Cern.Jet.Random.MersenneTwister"/>.
        /// </summary>
        /// <returns></returns>
        public static RandomEngine MakeDefault()
        {
            return new Cern.Jet.Random.Engine.MersenneTwister(Environment.TickCount);
        }

        /// <summary>
        /// Returns a 32 bit uniformly distributed random number in the open unit interval <code>(0.0f,1.0f)</code> (excluding 0.0f and 1.0f).
        /// </summary>
        /// <returns></returns>
        public abstract UInt32 NextUInt32();

        /// <summary>
        /// Returns a 32 bit uniformly distributed random number in the closed interval <tt>[<see cref="int.MinValue"/>,<see cref="int.MaxValue"/>]</tt> (including <tt><see cref="int.MinValue"/></tt> and <tt><see cref="int.MaxValue"/></tt>);
        /// </summary>
        /// <returns></returns>
        public virtual Int32 NextInt32()
        {
            return (Int32)NextDouble();
        }

        /// <summary>
        /// Returns a 64 bit uniformly distributed random number in the closed interval <tt>[<see cref="long.MinValue"/>,<see cref="long.MaxValue"/>]</tt> (including <tt><see cref="long.MinValue"/></tt> and <tt><see cref="long.MaxValue"/></tt>).
        /// </summary>
        /// <returns></returns>
        public virtual long NextLong()
        {
            return ((NextUInt32() & 0xFFFFFFFFL) << 32) | ((NextUInt32() & 0xFFFFFFFFL));
        }

        /// <summary>
        /// Returns a Byte uniformly distributed random number.
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void NextBytes(byte[] buffer)
        {
            int i = 0;
            Int32 r;
            while (i + 4 <= buffer.Length)
            {
                r = NextInt32();
                buffer[i++] = (byte)r;
                buffer[i++] = (byte)(r >> 8);
                buffer[i++] = (byte)(r >> 16);
                buffer[i++] = (byte)(r >> 24);
            }
            if (i >= buffer.Length) return;
            r = NextInt32();
            buffer[i++] = (byte)r;
            if (i >= buffer.Length) return;
            buffer[i++] = (byte)(r >> 8);
            if (i >= buffer.Length) return;
            buffer[i++] = (byte)(r >> 16);
        }

        /// <summary>
        /// Returns a 64 bit uniformly distributed random number in the open unit interval <code>(0.0,1.0)</code> (excluding 0.0 and 1.0).
        /// </summary>
        /// <returns></returns>
        public virtual double NextDouble()
        {
            long r1, r2;
            r1 = NextLong();
            r2 = NextLong();
            return (r1 * (double)(2 << 11) + r2) / (double)(2 << 53);
        }

        /// <summary>
        /// Returns a 32 bit uniformly distributed random number in the open unit interval <code>(0.0,1.0)</code> (excluding 0.0 and 1.0).
        /// </summary>
        /// <returns></returns>
        public double Raw()
        {
            UInt32 nextInt;
            do
            { // accept anything but zero
                nextInt = NextUInt32(); // in [Integer.MinValue,Integer.MaxValue]-interval
            } while (nextInt == 0);

            // transform to (0.0,1.0)-interval
            // 2.3283064365386963E-10 == 1.0 / System.Math.Pow(2,32)
            return (double)(nextInt & 0xFFFFFFFFL) * 2.3283064365386963E-10;
        }
    }
}
