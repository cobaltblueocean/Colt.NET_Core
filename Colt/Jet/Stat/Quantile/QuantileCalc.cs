using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Stat.Quantile
{
    /// <summary>
    /// Computes b and k vor various parameters.
    /// </summary>
    public class QuantileCalc
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Efficiently computes the binomial coefficient, often also referred to as "n over k" or "n choose k".
        /// The binomial coefficient is defined as n!/((n-k)!*k!).
        /// Tries to avoid numeric overflowsd 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns>the binomial coefficient.</returns>
        public static double Binomial(long n, long k)
        {
            if (k == 0 || k == n) { return 1.0; }

            // since Binomial(n,k)==Binomial(n,n-k), we can enforce the faster variant,
            // which is also the variant minimizing number overflows.
            if (k > n / 2.0) k = n - k;

            double binomial = 1.0;
            long N = n - k + 1;
            for (long i = k; i > 0;)
            {
                binomial *= ((double)N++) / (double)(i--);
            }
            return binomial;
        }

        /// <summary>
        /// Returns the smallest <code>long &gt;= value</code>.
        /// <dt>Examples: <code>1.0 -> 1, 1.2 -> 2, 1.9 -> 2</code>.
        /// This method is safer than using (long) System.Math.Ceiling(value), because of possible rounding error.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Ceiling(double value)
        {
            return (long)System.Math.Round(System.Math.Ceiling(value));
        }

        /// <summary>
        /// Computes the number of buffers and number of values per buffer such that
        /// quantiles can be determined with an approximation error no more than epsilon with a certain probability.
        ///
        /// Assumes that quantiles are to be computed over N values.
        /// The required sampling rate is computed and stored in the first element of the provided <i>returnSamplingRate</i> array, which, therefore must be at least of Length 1.
        /// </summary>
        /// <param name="N">the number of values over which quantiles shall be computed (e.g <i>10^6</i>).</param>
        /// <param name="epsilon">the approximation error which is guaranteed not to be exceeded (e.gd <i>0.001</i>)(<i>0 &lt;= epsilon &lt;= 1</i>)d To get exact result, set <i>epsilon=0.0</i>;</param>
        /// <param name="delta">the probability that the approximation error is more than than epsilon (e.gd <i>0.0001</i>)(<i>0 &lt;= delta &lt;= 1</i>)d To avoid probabilistic answers, set <i>delta=0.0</i>.</param>
        /// <param name="quantiles">the number of quantiles to be computed (e.gd <i>100</i>)(<i>quantiles &gt;= 1</i>)d If unknown in advance, set this number large, e.gd <i>quantiles &gt;= 10000</i>.</param>
        /// <param name="returnSamplingRate">a <i>double[1]</i> where the sampling rate is to be filled in.</param>
        /// <returns><i>long[2]</i> - <i>long[0]</i>=the number of buffers, <i>long[1]</i>=the number of elements per buffer, <i>returnSamplingRate[0]</i>=the required sampling rate.</returns>
        public static long[] known_N_compute_B_and_K(long N, double epsilon, double delta, int quantiles, double[] returnSamplingRate)
        {
            if (delta > 0.0)
            {
                return known_N_compute_B_and_K_slow(N, epsilon, delta, quantiles, returnSamplingRate);
            }
            returnSamplingRate[0] = 1.0;
            return known_N_compute_B_and_K_quick(N, epsilon);
        }

        /// <summary>
        /// Computes the number of buffers and number of values per buffer such that quantiles can be determined with a <b>guaranteed</b> approximation error no more than epsilon.
        /// Assumes that quantiles are to be computed over N values.
        /// </summary>
        /// <param name="N">the anticipated number of values over which quantiles shall be determined.</param>
        /// <param name="epsilon">the approximation error which is guaranteed not to be exceeded (e.gd <i>0.001</i>)(<i>0 &lt;= epsilon &lt;= 1</i>)d To get exact result, set <i>epsilon=0.0</i>;</param>
        /// <returns><i>long[2]</i> - <i>long[0]</i>=the number of buffers, <i>long[1]</i>=the number of elements per buffer.</returns>
        protected static long[] known_N_compute_B_and_K_quick(long N, double epsilon)
        {
            long[] result;

            if (epsilon <= 0.0)
            {
                // no way around exact quantile search
                result = new long[2];
                result[0] = 1;
                result[1] = N;
                return result;
            }

            int maxBuffers = 50;
            int maxHeight = 50;
            double N_double = (double)N;
            double c = N_double * epsilon * 2.0;
            int[] heightMaximums = new int[maxBuffers - 1];

            // for each b, determine maximum height, i.ed the height for which x<=0 and x is a maximum
            // with x = Binomial(b+h-2, h-1) - Binomial(b+h-3, h-3) + Binomial(b+h-3, h-2) - N * epsilon * 2.0
            for (int b1 = 2; b1 <= maxBuffers; b1++)
            {
                int h = 3;

                while (h <= maxHeight && // skip heights until x<=0
                        (h - 2) * ((double)System.Math.Round(Binomial(b1 + h - 2, h - 1))) -
                        ((double)System.Math.Round(Binomial(b1 + h - 3, h - 3))) +
                        ((double)System.Math.Round(Binomial(b1 + h - 3, h - 2))) - c
                        > 0.0
                      ) { h++; }
                //from now on x is monotonically growing...
                while (h <= maxHeight && // skip heights until x>0
                        (h - 2) * ((double)System.Math.Round(Binomial(b1 + h - 2, h - 1))) -
                        ((double)System.Math.Round(Binomial(b1 + h - 3, h - 3))) +
                        ((double)System.Math.Round(Binomial(b1 + h - 3, h - 2))) - c
                        <= 0.0
                      ) { h++; }
                h--; //go back to last height

                // was x>0 or did we loop without finding anything?
                int hMax;
                if (h >= maxHeight &&
                        (h - 2) * ((double)System.Math.Round(Binomial(b1 + h - 2, h - 1))) -
                        ((double)System.Math.Round(Binomial(b1 + h - 3, h - 3))) +
                        ((double)System.Math.Round(Binomial(b1 + h - 3, h - 2))) - c
                        > 0.0)
                {
                    hMax = int.MinValue;
                }
                else
                {
                    hMax = h;
                }

                heightMaximums[b1 - 2] = hMax; //safe some space
            } //end for


            // for each b, determine the smallest k satisfying the constraints, i.e.
            // for each b, determine kMin, with kMin = N/Binomial(b+hMax-2,hMax-1)
            long[] kMinimums = new long[maxBuffers - 1];
            for (int b2 = 2; b2 <= maxBuffers; b2++)
            {
                int h = heightMaximums[b2 - 2];
                long kMin = long.MaxValue;
                if (h > int.MinValue)
                {
                    double value = ((double)System.Math.Round(Binomial(b2 + h - 2, h - 1)));
                    long tmpK = Ceiling(N_double / value);
                    if (tmpK <= long.MaxValue)
                    {
                        kMin = tmpK;
                    }
                }
                kMinimums[b2 - 2] = kMin;
            }

            // from all b's, determine b that minimizes b*kMin
            long multMin = long.MaxValue;
            int minB = -1;
            for (int b3 = 2; b3 <= maxBuffers; b3++)
            {
                if (kMinimums[b3 - 2] < long.MaxValue)
                {
                    long mult = ((long)b3) * ((long)kMinimums[b3 - 2]);
                    if (mult < multMin)
                    {
                        multMin = mult;
                        minB = b3;
                    }
                }
            }

            long b, k;
            if (minB != -1)
            { // epsilon large enough?
                b = minB;
                k = kMinimums[minB - 2];
            }
            else
            {     // epsilon is very small or zero.
                b = 1; // the only possible solution without violating the 
                k = N; // approximation guarantees is exact quantile search.
            }

            result = new long[2];
            result[0] = b;
            result[1] = k;
            return result;
        }

        /// <summary>
        /// Computes the number of buffers and number of values per buffer such that
        /// quantiles can be determined with an approximation error no more than epsilon with a certain probability.
        /// Assumes that quantiles are to be computed over N values.
        /// The required sampling rate is computed and stored in the first element of the provided <i>returnSamplingRate</i> array, which, therefore must be at least of Length 1.
        /// </summary>
        /// <param name="N">the anticipated number of values over which quantiles shall be computed (e.g 10^6).</param>
        /// <param name="epsilon">the approximation error which is guaranteed not to be exceeded (e.gd <i>0.001</i>)(<i>0 &lt;= epsilon &lt;= 1</i>)d To get exact result, set <i>epsilon=0.0</i>;</param>
        /// <param name="delta">the probability that the approximation error is more than than epsilon (e.gd <i>0.0001</i>)(<i>0 &lt;= delta &lt;= 1</i>)d To avoid probabilistic answers, set <i>delta=0.0</i>.</param>
        /// <param name="quantiles">the number of quantiles to be computed (e.gd <i>100</i>)(<i>quantiles &gt;= 1</i>)d If unknown in advance, set this number large, e.gd <i>quantiles &gt;= 10000</i>.</param>
        /// <param name="returnSamplingRate">a <i>double[1]</i> where the sampling rate is to be filled in.</param>
        /// <returns><i>long[2]</i> - <i>long[0]</i>=the number of buffers, <i>long[1]</i>=the number of elements per buffer, <i>returnSamplingRate[0]</i>=the required sampling rate.</returns>
        protected static long[] known_N_compute_B_and_K_slow(long N, double epsilon, double delta, int quantiles, double[] returnSamplingRate)
        {
            long[] result;

            // delta can be set to zero, i.ed, all quantiles should be approximate with probability 1	
            if (epsilon <= 0.0)
            {
                // no way around exact quantile search
                result = new long[2];
                result[0] = 1;
                result[1] = N;
                returnSamplingRate[0] = 1.0;
                return result;
            }


            int maxBuffers = 50;
            int maxHeight = 50;
            double N_double = N;

            // One possibility is to use one buffer of size N
            //
            long ret_b = 1;
            long ret_k = N;
            double sampling_rate = 1.0;
            long memory = N;


            // Otherwise, there are at least two buffers (b >= 2)
            // and the height of the tree is at least three (h >= 3)
            //
            // We restrict the search for b and h to MAX_BINOM, a large enough value for
            // practical values of    epsilon >= 0.001   and    delta >= 0.00001
            //
            double logarithm = System.Math.Log(2.0 * quantiles / delta);
            double c = 2.0 * epsilon * N_double;
            for (long b = 2; b < maxBuffers; b++)
                for (long h = 3; h < maxHeight; h++)
                {
                    double binomial = Binomial(b + h - 2, h - 1);
                    long tmp = Ceiling(N_double / binomial);
                    if ((b * tmp < memory) &&
                            ((h - 2) * binomial - Binomial(b + h - 3, h - 3) + Binomial(b + h - 3, h - 2)
                            <= c))
                    {
                        ret_k = tmp;
                        ret_b = b;
                        memory = ret_k * b;
                        sampling_rate = 1.0;
                    }
                    if (delta > 0.0)
                    {
                        double t = (h - 2) * Binomial(b + h - 2, h - 1) - Binomial(b + h - 3, h - 3) + Binomial(b + h - 3, h - 2);
                        double u = logarithm / epsilon;
                        double v = Binomial(b + h - 2, h - 1);
                        double w = logarithm / (2.0 * epsilon * epsilon);

                        // From our SIGMOD 98 paper, we have two equantions to satisfy:
                        // t  <= u * alpha/(1-alpha)^2
                        // kv >= w/(1-alpha)^2
                        //
                        // Denoting 1/(1-alpha)    by x,
                        // we see that the first inequality is equivalent to
                        // t/u <= x^2 - x
                        // which is satisfied by x >= 0.5 + 0.5 * sqrt (1 + 4t/u)
                        // Plugging in this value into second equation yields
                        // k >= wx^2/v

                        double x = 0.5 + 0.5 * System.Math.Sqrt(1.0 + 4.0 * t / u);
                        long k = Ceiling(w * x * x / v);
                        if (b * k < memory)
                        {
                            ret_k = k;
                            ret_b = b;
                            memory = b * k;
                            sampling_rate = N_double * 2.0 * epsilon * epsilon / logarithm;
                        }
                    }
                }

            result = new long[2];
            result[0] = ret_b;
            result[1] = ret_k;
            returnSamplingRate[0] = sampling_rate;
            return result;
        }

        public static void main(String[] args)
        {
            test_B_and_K_Calculation(args);
        }

        public static void test_B_and_K_Calculation(String[] args)
        {
            Boolean known_N;
            if (args == null) known_N = false;
            else known_N = Boolean.Parse(args[0]); //new Boolean.(args[0]).BooleanValue();

            int[] quantiles = { 1, 1000 };

            long[] sizes = { 100000, 1000000, 10000000, 1000000000 };

            double[] deltas = { 0.0, 0.001, 0.0001, 0.00001 };

            double[] epsilons = { 0.0, 0.1, 0.05, 0.01, 0.005, 0.001, 0.0000001 };



            if (!known_N) sizes = new long[] { 0 };
            Console.WriteLine("\n\n");
            if (known_N)
                Console.WriteLine("Computing b's and k's for KNOWN N");
	else 
		Console.WriteLine("Computing b's and k's for UNKNOWN N");
            Console.WriteLine("mem [elements/1024]");
            Console.WriteLine("***********************************");

            for (int q = 0; q < quantiles.Length; q++)
            {
                int p = quantiles[q];
                Console.WriteLine("------------------------------");
                Console.WriteLine("computing for p = " + p);
                for (int s = 0; s < sizes.Length; s++)
                {
                    long N = sizes[s];
                    Console.WriteLine("   ------------------------------");
                    Console.WriteLine("   computing for N = " + N);
                    for (int d = 0; d < deltas.Length; d++)
                    {
                        double delta = deltas[d];
                        Console.WriteLine("      ------------------------------");
                        Console.WriteLine("      computing for delta = " + delta);
                        for (int e = 0; e < epsilons.Length; e++)
                        {
                            double epsilon = epsilons[e];

                            double[] returnSamplingRate = new double[1];
                            long[] result;
                            if (known_N)
                            {
                                result = known_N_compute_B_and_K(N, epsilon, delta, p, returnSamplingRate);
                            }
                            else
                            {
                                result = unknown_N_compute_B_and_K(epsilon, delta, p);
                            }

                            long b = result[0];
                            long k = result[1];
                            Console.Write("         (e,d,N,p)=(" + epsilon + "," + delta + "," + N + "," + p + ") --> ");
                            Console.Write("(b,k,mem");
                            if (known_N) Console.Write(",sampling");
                            Console.Write(")=(" + b + "," + k + "," + (b * k / 1024));
                            if (known_N) Console.Write("," + returnSamplingRate[0]);
                            Console.WriteLine(")");
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Computes the number of buffers and number of values per buffer such that quantiles can be determined with an approximation error no more than epsilon with a certain probability.
        /// </summary>
        /// <param name="epsilon">the approximation error which is guaranteed not to be exceeded (e.gd <i>0.001</i>)(<i>0 &lt;= epsilon &lt;= 1</i>)d To get exact results, set <i>epsilon=0.0</i>;</param>
        /// <param name="delta">the probability that the approximation error is more than than epsilon (e.gd <i>0.0001</i>)(<i>0 &lt;= delta &lt;= 1</i>)d To get exact results, set <i>delta=0.0</i>.</param>
        /// <param name="quantiles">the number of quantiles to be computed (e.gd <i>100</i>)(<i>quantiles &gt;= 1</i>)d If unknown in advance, set this number large, e.gd <i>quantiles &gt;= 10000</i>.</param>
        /// <returns><i>long[3]</i> - <i>long[0]</i>=the number of buffers, <i>long[1]</i>=the number of elements per buffer, <i>long[2]</i>=the tree height where sampling shall start.</returns>
        public static long[] unknown_N_compute_B_and_K(double epsilon, double delta, int quantiles)
        {
            long[] result;
            // delta can be set to zero, i.ed, all quantiles should be approximate with probability 1	
            if (epsilon <= 0.0 || delta <= 0.0)
            {
                // no way around exact quantile search
                result = new long[3];
                result[0] = 1;
                result[1] = long.MaxValue;
                result[2] = long.MaxValue;
                return result;
            }

            int max_b = 50;
            int max_h = 50;
            int max_H = 50;
            int max_Iterations = 2;

            long best_b = long.MaxValue;
            long best_k = long.MaxValue;
            long best_h = long.MaxValue;
            long best_memory = long.MaxValue;

            double pow = System.Math.Pow(2.0, max_H);
            double logDelta = System.Math.Log(2.0 / (delta / quantiles)) / (2.0 * epsilon * epsilon);
            //double logDelta =  System.Math.Log(2.0/(quantiles*delta)) / (2.0*epsilon*epsilon);

            while (best_b == long.MaxValue && max_Iterations-- > 0)
            { //until we find a solution
              // identify that combination of b and h that minimizes b*k.
              // exhaustive search.
                for (int b = 2; b <= max_b; b++)
                {
                    for (int h = 2; h <= max_h; h++)
                    {
                        double Ld = Binomial(b + h - 2, h - 1);
                        double Ls = Binomial(b + h - 3, h - 1);

                        // now we have k>=c*(1-alpha)^-2.
                        // let's compute c.
                        //double c = System.Math.Log(2.0/(delta/quantiles)) / (2.0*epsilon*epsilon*System.Math.Min(Ld, 8.0*Ls/3.0));
                        double c = logDelta / System.Math.Min(Ld, 8.0 * Ls / 3.0);

                        // now we have k>=d/alpha.
                        // let's compute d.				
                        double beta = Ld / Ls;
                        double cc = (beta - 2.0) * (max_H - 2.0) / (beta + pow - 2.0);
                        double d = (h + 3 + cc) / (2.0 * epsilon);

                        /*
                        double d = (Ld*(h+max_H-1.0)  +  Ls*((h+1)*pow - 2.0*(h+max_H)))   /   (Ld + Ls*(pow-2.0));
                        d = (d + 2.0) / (2.0*epsilon);
                        */

                        // now we have c*(1-alpha)^-2 == d/alpha.
                        // we solve this equation for alpha yielding two solutions
                        // alpha_1,2 = (c + 2*d  +-  Sqrt(c*c + 4*c*d))/(2*d)				
                        double f = c * c + 4.0 * c * d;
                        if (f < 0.0) continue; // non real solution to equation
                        double root = System.Math.Sqrt(f);
                        double alpha_one = (c + 2.0 * d + root) / (2.0 * d);
                        double alpha_two = (c + 2.0 * d - root) / (2.0 * d);

                        // any alpha must satisfy 0<alpha<1 to yield valid solutions
                        Boolean alpha_one_OK = false;
                        Boolean alpha_two_OK = false;
                        if (0.0 < alpha_one && alpha_one < 1.0) alpha_one_OK = true;
                        if (0.0 < alpha_two && alpha_two < 1.0) alpha_two_OK = true;
                        if (alpha_one_OK || alpha_two_OK)
                        {
                            double alpha = alpha_one;
                            if (alpha_one_OK && alpha_two_OK)
                            {
                                // take the alpha that minimizes d/alpha
                                alpha = System.Math.Max(alpha_one, alpha_two);
                            }
                            else if (alpha_two_OK)
                            {
                                alpha = alpha_two;
                            }

                            // now we have k=Ceiling(Max(d/alpha, (h+1)/(2*epsilon)))
                            long k = Ceiling(System.Math.Max(d / alpha, (h + 1) / (2.0 * epsilon)));
                            if (k > 0)
                            { // valid solution?
                                long memory = b * k;
                                if (memory < best_memory)
                                {
                                    // found a solution requiring less memory
                                    best_k = k;
                                    best_b = b;
                                    best_h = h;
                                    best_memory = memory;
                                }
                            }
                        }
                    } //end for h
                } //end for b

                if (best_b == long.MaxValue)
                {
                    Console.WriteLine("Warning: Computing b and k looks like a lot of work!");
                    // no solution found so fard very unlikelyd Anyway, try again.
                    max_b *= 2;
                    max_h *= 2;
                    max_H *= 2;
                }
            } //end while

            result = new long[3];
            if (best_b == long.MaxValue)
            {
                // no solution found.
                // no way around exact quantile search.
                result[0] = 1;
                result[1] = long.MaxValue;
                result[2] = long.MaxValue;
            }
            else
            {
                result[0] = best_b;
                result[1] = best_k;
                result[2] = best_h;
            }

            return result;
        }
        #endregion

        #region Local Private Methods

        #endregion

    }
}
