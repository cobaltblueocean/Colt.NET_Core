using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt
{
    public class GenericPermuting
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected GenericPermuting() { }


        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        public static int[] permutation(long p, int N)
        {
            if (p < 1) throw new ArgumentException("Permutations are enumerated 1 .d N!");
            if (N < 0) throw new ArgumentException("Must satisfy N >= 0");

            int[] permutation = new int[N];

            if (N > 20)
            { // factorial(21) would overflow 64-bit long)
              // Simply make a list (0,1,..N-1) and randomize it, seeded with "p"d 
              // Note that this is perhaps not what you want...
                for (int i = N; --i >= 0;) permutation[i] = i;
                var gen = new Cern.Jet.Random.Uniform(new Cern.Jet.Random.Engine.MersenneTwister((int)p));
                for (int i = 0; i < N - 1; i++)
                {
                    int random = gen.NextIntFromTo(i, N - 1);

                    //swap(i, random)
                    int tmp = permutation[random];
                    permutation[random] = permutation[i];
                    permutation[i] = tmp;
                }

                return permutation;
            }

            // the normal case - exact enumeration
            if (p > Cern.Jet.Math.Arithmetic.LongFactorial(N))
                throw new ArgumentException("N too large (a sequence of N elements only has N! permutations).");

            var tmp2 = new int[N];
            for (int i = 1; i <= N; i++) tmp2[i - 1] = i;

            long io = p - 1;
            for (int M = N - 1; M >= 1; M--)
            {
                long fac = Cern.Jet.Math.Arithmetic.LongFactorial(M);
                int i = ((int)(io / fac)) + 1;
                io = io % fac;
                permutation[N - M - 1] = tmp2[i-1];

                for (int j = i; j <= M; j++) tmp2[j - 1] = tmp2[j];
            }
            if (N > 0) permutation[N - 1] = tmp2[0];

            for (int i = N; --i >= 0;) permutation[i] = permutation[i] - 1;
            return permutation;
        }

        public static void permute(int[] list, int[] indexes)
        {
            int[] copy = (int[])list.Clone();
            for (int i = list.Length; --i >= 0;) list[i] = copy[indexes[i]];
        }

        public static void permute(int[] indexes, Cern.Colt.Swapper swapper, int[] work)
        {
            permute(indexes, swapper, work, null);
        }

        public static void permute(int[] indexes, Cern.Colt.Swapper swapper, int[] work1, int[] work2)
        {
            // "tracks" and "pos" keeps track of the current indexes of the elements
            // Example: We have a list==[A,B,C,D,E], indexes==[0,4,1,2,3] and swap B and E we need to know that the element formlerly at index 1 is now at index 4, and the one formerly at index 4 is now at index 1.
            // Otherwise we stumble over our own feet and produce nonsense.
            // Initially index i really is at index i, but this will change due to swapping.

            // work1, work2 to avoid high frequency memalloc's
            int s = indexes.Length;
            int[] tracks = work1;
            int[] pos = work2;
            if (tracks == null || tracks.Length < s) tracks = new int[s];
            if (pos == null || pos.Length < s) pos = new int[s];
            for (int i = s; --i >= 0;)
            {
                tracks[i] = i;
                pos[i] = i;
            }

            for (int i = 0; i < s; i++)
            {
                int index = indexes[i];
                int track = tracks[index];

                if (i != track)
                {
                    swapper(i, track);
                    tracks[index] = i; tracks[pos[i]] = track;
                    int tmp = pos[i]; pos[i] = pos[track]; pos[track] = tmp;
                }
            }
        }

        public static void permute(Object[] list, int[] indexes)
        {
            Object[] copy = (Object[])list.Clone();
            for (int i = list.Length; --i >= 0;) list[i] = copy[indexes[i]];
        }

        #endregion

        #region Local Private Methods

        #endregion

    }
}
