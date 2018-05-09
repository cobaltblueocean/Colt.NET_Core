using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;
using Cern.Jet.Math;
namespace Cern.Jet.Stat.Quantile
{
    public class QuantileFinderFactory
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor
        /// <summary>
        /// Make this class non instantiabled Let still allow others to inherit.
        /// </summary>
        protected QuantileFinderFactory()
        {
        }
        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        public static long[] known_N_compute_B_and_K(long N, double epsilon, double delta, int quantiles, double[] returnSamplingRate)
        {
            returnSamplingRate[0] = 1.0;
            if (epsilon <= 0.0)
            {
                // no way around exact quantile search
                long[] result = new long[2];
                result[0] = 1;
                result[1] = N;
                return result;
            }
            if (epsilon >= 1.0 || delta >= 1.0)
            {
                // can make any error we wish
                long[] result = new long[2];
                result[0] = 2;
                result[1] = 1;
                return result;
            }

            if (delta > 0.0)
            {
                return known_N_compute_B_and_K_slow(N, epsilon, delta, quantiles, returnSamplingRate);
            }
            return known_N_compute_B_and_K_quick(N, epsilon);
        }

        public static IDoubleQuantileFinder newDoubleQuantileFinder(Boolean known_N, long N, double epsilon, double delta, int quantiles, RandomEngine generator)
        {
            //Boolean known_N = true;
            //if (N==long.MaxValue) known_N = false;
            // check parameters.
            // if they are illegal, keep quite and return an exact finder.
            if (epsilon <= 0.0 || N < 1000) return new ExactDoubleQuantileFinder();
            if (epsilon > 1) epsilon = 1;
            if (delta < 0) delta = 0;
            if (delta > 1) delta = 1;
            if (quantiles < 1) quantiles = 1;
            if (quantiles > N) N = quantiles;

            KnownDoubleQuantileEstimator finder;
            if (known_N)
            {
                double[] samplingRate = new double[1];
                long[] resultKnown = known_N_compute_B_and_K(N, epsilon, delta, quantiles, samplingRate);
                long b = resultKnown[0];
                long k = resultKnown[1];
                if (b == 1) return new ExactDoubleQuantileFinder();
                return new KnownDoubleQuantileEstimator((int)b, (int)k, N, samplingRate[0], generator);
            }
            else
            {
                long[] resultUnknown = unknown_N_compute_B_and_K(epsilon, delta, quantiles);
                long b1 = resultUnknown[0];
                long k1 = resultUnknown[1];
                long h1 = resultUnknown[2];
                double preComputeEpsilon = -1.0;
                if (resultUnknown[3] == 1) preComputeEpsilon = epsilon;

                //if (N==long.MaxValue) { // no maximum N provided by user.

                // if (true) fixes bug reported by LarryPeranich@fairisaac.com
                if (true)
                { // no maximum N provided by user.
                    if (b1 == 1) return new ExactDoubleQuantileFinder();
                    return new UnknownDoubleQuantileEstimator((int)b1, (int)k1, (int)h1, preComputeEpsilon, generator);
                }

                // determine whether UnknownFinder or KnownFinder with maximum N requires less memory.
                double[] samplingRate = new double[1];

                // IMPORTANT: for known finder, switch sampling off (delta == 0) !!!
                // with knownN-sampling we can only guarantee the errors if the input sequence has EXACTLY N elements.
                // with knownN-no sampling we can also guarantee the errors for sequences SMALLER than N elements.
                long[] resultKnown = known_N_compute_B_and_K(N, epsilon, 0, quantiles, samplingRate);

                long b2 = resultKnown[0];
                long k2 = resultKnown[1];

                if (b2 * k2 < b1 * k1)
                { // the KnownFinder is smaller
                    if (b2 == 1) return new ExactDoubleQuantileFinder();
                    return new KnownDoubleQuantileEstimator((int)b2, (int)k2, N, samplingRate[0], generator);
                }

                // the UnknownFinder is smaller
                if (b1 == 1) return new ExactDoubleQuantileFinder();
                return new UnknownDoubleQuantileEstimator((int)b1, (int)k1, (int)h1, preComputeEpsilon, generator);
            }
        }

        public static List<Double> newEquiDepthPhis(int quantiles)
        {
            List<Double> phis = new List<Double>(quantiles - 1);
            for (int i = 1; i <= quantiles - 1; i++) phis.Add(i / (double)quantiles);
            return phis;
        }

        public static long[] unknown_N_compute_B_and_K(double epsilon, double delta, int quantiles)
        {
            return unknown_N_compute_B_and_K_raw(epsilon, delta, quantiles);
        }

        protected static long[] unknown_N_compute_B_and_K_raw(double epsilon, double delta, int quantiles)
        {
            long[] result;
            // delta can be set to zero, i.ed, all quantiles should be approximate with probability 1	
            if (epsilon <= 0.0)
            {
                result = new long[4];
                result[0] = 1;
                result[1] = long.MaxValue;
                result[2] = long.MaxValue;
                result[3] = 0;
                return result;
            }
            if (epsilon >= 1.0 || delta >= 1.0)
            {
                // can make any error we wish
                result = new long[4];
                result[0] = 2;
                result[1] = 1;
                result[2] = 3;
                result[3] = 0;
                return result;
            }
            if (delta <= 0.0)
            {
                // no way around exact quantile search
                result = new long[4];
                result[0] = 1;
                result[1] = long.MaxValue;
                result[2] = long.MaxValue;
                result[3] = 0;
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
                        double Ld = Arithmetic.Binomial(b + h - 2, h - 1);
                        double Ls = Arithmetic.Binomial(b + h - 3, h - 1);

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
                            long k = (long)System.Math.Ceiling(System.Math.Max(d / alpha, (h + 1) / (2.0 * epsilon)));
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

            result = new long[4];
            result[3] = 0;
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

        #region Local Protected Methods
        protected static long[] known_N_compute_B_and_K_quick(long N, double epsilon)
        {
            int maxBuffers = 50;
            int maxHeight = 50;
            double N_double = (double)N;
            double c = N_double * epsilon * 2.0;
            int[] heightMaximums = new int[maxBuffers - 1];

            // for each b, determine maximum height, i.ed the height for which x<=0 and x is a maximum
            // with x = binomial(b+h-2, h-1) - binomial(b+h-3, h-3) + binomial(b+h-3, h-2) - N * epsilon * 2.0
            for (int b1 = 2; b1 <= maxBuffers; b1++)
            {
                int h = 3;

                while (h <= maxHeight && // skip heights until x<=0
                        (h - 2) * (Arithmetic.Binomial(b1 + h - 2, h - 1)) -
                        (Arithmetic.Binomial(b1 + h - 3, h - 3)) +
                        (Arithmetic.Binomial(b1 + h - 3, h - 2)) - c
                        > 0.0
                      ) { h++; }
                //from now on x is monotonically growing...
                while (h <= maxHeight && // skip heights until x>0
                        (h - 2) * (Arithmetic.Binomial(b1 + h - 2, h - 1)) -
                        (Arithmetic.Binomial(b1 + h - 3, h - 3)) +
                        (Arithmetic.Binomial(b1 + h - 3, h - 2)) - c
                        <= 0.0
                      ) { h++; }
                h--; //go back to last height

                // was x>0 or did we loop without finding anything?
                int hMax;
                if (h >= maxHeight &&
                        (h - 2) * (Arithmetic.Binomial(b1 + h - 2, h - 1)) -
                        (Arithmetic.Binomial(b1 + h - 3, h - 3)) +
                        (Arithmetic.Binomial(b1 + h - 3, h - 2)) - c
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
            // for each b, determine kMin, with kMin = N/binomial(b+hMax-2,hMax-1)
            long[] kMinimums = new long[maxBuffers - 1];
            for (int b2 = 2; b2 <= maxBuffers; b2++)
            {
                int h = heightMaximums[b2 - 2];
                long kMin = long.MaxValue;
                if (h > int.MinValue)
                {
                    double value = (Arithmetic.Binomial(b2 + h - 2, h - 1));
                    long tmpK = (long)(System.Math.Ceiling(N_double / value));
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

            long[] result = new long[2];
            result[0] = b;
            result[1] = k;
            return result;
        }

        protected static long[] known_N_compute_B_and_K_slow(long N, double epsilon, double delta, int quantiles, double[] returnSamplingRate)
        {
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
                    double binomial = Arithmetic.Binomial(b + h - 2, h - 1);
                    long tmp = (long)System.Math.Ceiling(N_double / binomial);
                    if ((b * tmp < memory) &&
                            ((h - 2) * binomial - Arithmetic.Binomial(b + h - 3, h - 3) + Arithmetic.Binomial(b + h - 3, h - 2)
                            <= c))
                    {
                        ret_k = tmp;
                        ret_b = b;
                        memory = ret_k * b;
                        sampling_rate = 1.0;
                    }
                    if (delta > 0.0)
                    {
                        double t = (h - 2) * Arithmetic.Binomial(b + h - 2, h - 1) - Arithmetic.Binomial(b + h - 3, h - 3) + Arithmetic.Binomial(b + h - 3, h - 2);
                        double u = logarithm / epsilon;
                        double v = Arithmetic.Binomial(b + h - 2, h - 1);
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
                        long k = (long)System.Math.Ceiling(w * x * x / v);
                        if (b * k < memory)
                        {
                            ret_k = k;
                            ret_b = b;
                            memory = b * k;
                            sampling_rate = N_double * 2.0 * epsilon * epsilon / logarithm;
                        }
                    }
                }

            long[] result = new long[2];
            result[0] = ret_b;
            result[1] = ret_k;
            returnSamplingRate[0] = sampling_rate;
            return result;
        }


        #endregion

        #region Local Private Methods

        #endregion

    }
}
