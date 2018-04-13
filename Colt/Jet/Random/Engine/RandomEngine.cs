using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Random.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RandomEngine
    {
        public double Apply(double dummy)
        {
            return Raw();
        }

        public int Apply(int dummy)
        {
            return NextInt32();
        }

        /// <summary>
        /// Constructs and returns a new uniform random number engine seeded with the current time.
        /// Currently this is <see cref="Cern.Jet.Random.MersenneTwister"/>.
        /// </summary>
        /// <returns></returns>
        public static RandomEngine MakeDefault()
        {
            return new Cern.Jet.Random.Engine.MersenneTwister(Environment.TickCount);
        }

        public abstract UInt32 NextUInt32();

        public virtual Int32 NextInt32()
        {
            return (Int32)NextDouble();
        }

        public virtual long NextLong()
        {
            return ((NextUInt32() & 0xFFFFFFFFL) << 32) | ((NextUInt32() & 0xFFFFFFFFL));
        }

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

        public virtual double NextDouble()
        {
            long r1, r2;
            r1 = NextLong();
            r2 = NextLong();
            return (r1 * (double)(2 << 11) + r2) / (double)(2 << 53);
        }

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
