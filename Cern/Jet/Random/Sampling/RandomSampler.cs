﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random.Sampling
{
    /// <summary>
    /// Space and time efficiently computes a sorted <i>Simple Random Sample Without Replacement (SRSWOR)</i>, that is, a sorted set of <tt>n</tt> random numbers from an interval of <tt>N</tt> numbers;
    /// Example: Computing <tt>n=3</tt> random numbers from the interval <tt>[1,50]</tt> may yield the sorted random set <tt>(7,13,47)</tt>.
    /// Since we are talking about a set (sampling without replacement), no element will occur more than once.
    /// Each number from the <tt>N</tt> numbers has the same probability to be included in the <tt>n</tt> chosen numbers.
    ///
    /// <p><b>Problem:</b> This class solves problems including the following: <i>
    /// Suppose we have a file containing 10^12 objects.
    /// We would like to take a truly random subset of 10^6 objects and do something with it, 
    /// for example, compute the sum over some instance field, or whatever.
    /// How do we choose the subset? In particular, how do we avoid multiple equal elements? How do we do this quick and without consuming excessive memory? How do we avoid slowly jumping back and forth within the file? </i>
    ///
    /// <p><b>Sorted Simple Random Sample Without Replacement (SRSWOR):</b>
    /// What are the exact semantics of this class? What is a SRSWOR? In which sense exactly is a returned set "random"?
    /// It is random in the sense, that each number from the <tt>N</tt> numbers has the same probability to be included in the <tt>n</tt> chosen numbers.
    /// For those who think in implementations rather than abstract interfaces:
    /// <i>Suppose, we have an empty list.
    /// We pick a random number between 1 and 10^12 and add it to the list only if it was not already picked before, i.e. if it is not already contained in the list.
    /// We then do the same thing again and again until we have eventually collected 10^6 distinct numbers.
    /// Now we sort the set ascending and return it.</i>
    /// <dt>It is exactly in this sense that this class returns "random" sets.
    /// <b>Note, however, that the implementation of this class uses a technique orders of magnitudes better (both in time and space) than the one outlined above.</b> 
    ///
    /// <p><b>Performance:</b> Space requirements are zero. Running time is <tt>O(n)</tt> on average, <tt>O(N)</tt> in the worst case.
    /// <h2 align=center>Performance (200Mhz Pentium Pro, JDK 1.2, NT)</h2>
    /// <center>
    ///   <table border="1">
    ///     <tr> 
    ///       <td align="center" width="20%">n</td>
    ///       <td align="center" width="20%">N</td>
    ///       <td align="center" width="20%">Speed [seconds]</td>
    ///     </tr>
    ///     <tr> 
    ///       <td align="center" width="20%">10<sup>3</sup></td>
    ///       <td align="center" width="20%">1.2*10<sup>3</sup></td>
    ///       <td align="center" width="20">0.0014</td>
    ///     </tr>
    ///     <tr> 
    ///       <td align="center" width="20%">10<sup>3</sup></td>
    ///       <td align="center" width="20%">10<sup>7</sup></td>
    ///       <td align="center" width="20">0.006</td>
    ///     </tr>
    ///     <tr> 
    ///       <td align="center" width="20%">10<sup>5</sup></td>
    ///       <td align="center" width="20%">10<sup>7</sup></td>
    ///       <td align="center" width="20">0.7</td>
    ///     </tr>
    ///     <tr> 
    ///       <td align="center" width="20%">9.0*10<sup>6</sup></td>
    ///       <td align="center" width="20%">10<sup>7</sup></td>
    ///       <td align="center" width="20">8.5</td>
    ///     </tr>
    ///     <tr> 
    ///       <td align="center" width="20%">9.9*10<sup>6</sup></td>
    ///       <td align="center" width="20%">10<sup>7</sup></td>
    ///       <td align="center" width="20">2.0 (samples more than 95%)</td>
    ///     </tr>
    ///     <tr> 
    ///       <td align="center" width="20%">10<sup>4</sup></td>
    ///       <td align="center" width="20%">10<sup>12</sup></td>
    ///       <td align="center" width="20">0.07</td>
    ///     </tr>
    ///     <tr> 
    ///       <td align="center" width="20%">10<sup>7</sup></td>
    ///       <td align="center" width="20%">10<sup>12</sup></td>
    ///       <td align="center" width="20">60</td>
    ///     </tr>
    ///   </table>
    /// </center>
    ///
    /// <p><b>Scalability:</b> This random sampler is designed to be scalable. In iterator style, it is able to compute and deliver sorted random sets stepwise in units called <i>blocks</i>.
    /// Example: Computing <tt>n=9</tt> random numbers from the interval <tt>[1,50]</tt> in 3 blocks may yield the blocks <tt>(7,13,14), (27,37,42), (45,46,49)</tt>.
    /// (The maximum of a block is guaranteed to be less than the minimum of its successor block. Every block is sorted ascending. No element will ever occur twice, both within a block and among blocks.)
    /// A block can be computed and retrieved with method <tt>nextBlock</tt>.
    /// Successive calls to method <tt>nextBlock</tt> will deliver as many random numbers as required.
    ///
    /// <p>Computing and retrieving samples in blocks is useful if you need very many random numbers that cannot be stored in main memory at the same time.
    /// For example, if you want to compute 10^10 such numbers you can do this by computing them in blocks of, say, 500 elements each.
    /// You then need only space to keep one block of 500 elements (i.e. 4 KB).
    /// When you are finished processing the first 500 elements you call <tt>nextBlock</tt> to fill the next 500 elements into the block, process them, and so on.
    /// If you have the time and need, by using such blocks you can compute random sets up to <tt>n=10^19</tt> random numbers.
    ///
    /// <p>If you do not need the block feature, you can also directly call 
    /// the static methods of this class without needing to construct a <tt>RandomSampler</tt> instance first.
    ///
    /// <p><b>Random number generation:</b> By default uses <tt>MersenneTwister</tt>, a very strong random number generator, much better than <tt>java.util.Random</tt>.
    /// You can also use other strong random number generators of Paul Houle's RngPack package.
    /// For example, <tt>Ranecu</tt>, <tt>Ranmar</tt> and <tt>Ranlux</tt> are strong well analyzed research grade pseudo-random number generators with known periods.
    ///
    /// <p><b>Implementation:</b> after J.S. Vitter, An Efficient Algorithm for Sequential Random Sampling,
    /// ACM Transactions on Mathematical Software, Vol 13, 1987.
    /// Paper available <A HREF="http://www.cs.duke.edu/~jsv"> here</A>.
    /// </summary>
    public class RandomSampler : Cern.Colt.PersistentObject
    {

        #region Local Variables
        long my_n;
        long my_N;
        long my_low;
        RandomEngine _randomGenerator;

        #endregion

        #region Property
        public RandomEngine RandomGenerator
        {
            get { return _randomGenerator; }
            set { _randomGenerator = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a random sampler that computes and delivers sorted random sets in blocks.
        /// A set block can be retrieved with method <tt>nextBlock</tt>.
        /// Successive calls to method <tt>nextBlock</tt> will deliver as many random numbers as required.
        /// </summary>
        /// <param name="n">the total number of elements to choose (must be <tt>n &gt;= 0</tt> and <tt>n &lt;= N</tt>).</param>
        /// <param name="N">the interval to choose random numbers from is <tt>[low,low+N-1]</tt>.</param>
        /// <param name="low">the interval to choose random numbers from is <tt>[low,low+N-1]</tt>. Hint: If <tt>low==0</tt>, then random numbers will be drawn from the interval <tt>[0,N-1]</tt>.</param>
        /// <param name="randomGenerator">a random number generator. Set this parameter to <tt>null</tt> to use the default random number generator.</param>
        public RandomSampler(long n, long N, long low, RandomEngine randomGenerator)
        {
            if (n < 0) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_NMustBeGraterThanOrEqualToZero);
            if (n > N) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_NMustBeLessThanOrEqualToN);
            this.my_n = n;
            this.my_N = N;
            this.my_low = low;

            if (randomGenerator == null) randomGenerator = Cern.Jet.Random.AbstractDistribution.MakeDefaultGenerator();
            this._randomGenerator = randomGenerator;
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
            RandomSampler copy = (RandomSampler)base.Clone();
            copy._randomGenerator = (RandomEngine)this._randomGenerator.Clone();
            return copy;
        }

        /// <summary>
        /// Tests this class.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(String[] args)
        {
            long n = long.Parse(args[0]);
            long N = long.Parse(args[1]);
            long low = long.Parse(args[2]);
            int chunkSize = int.Parse(args[3]);
            int times = int.Parse(args[4]);

            //test(n, N, low, chunkSize, times);
            //testNegAlphaInv(args);
        }

        /// <summary>
        /// Computes the next <tt>count</tt> random numbers of the sorted random set specified on instance construction
        /// and fills them into <tt>values</tt>, starting at index <tt>fromIndex</tt>.
        ///
        /// <p>Numbers are filled into the specified array starting at index <tt>fromIndex</tt> to the right.
        /// The array is returned sorted ascending in the range filled with numbers.
        /// </summary>
        /// <param name="count">the number of elements to be filled into <tt>values</tt> by this call (must be &gt;= 0).</param>
        /// <param name="values">the array into which the random numbers are to be filled; must have a length <tt>&gt;= count+fromIndex</tt>.</param>
        /// <param name="fromIndex">the first index within <tt>values</tt> to be filled with numbers (inclusive).</param>
        public void NextBlock(int count, long[] values, int fromIndex)
        {
            if (count > my_n) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_RandomSampleExhausted);
            if (count < 0) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_NegativeCount);

            if (count == 0) return; //nothing to do

            Sample(my_n, my_N, count, my_low, values, fromIndex, _randomGenerator);

            long lastSample = values[fromIndex + count - 1];
            my_n -= count;
            my_N = my_N - lastSample - 1 + my_low;
            my_low = lastSample + 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="N"></param>
        /// <param name="count"></param>
        /// <param name="low"></param>
        /// <param name="values"></param>
        /// <param name="fromIndex"></param>
        /// <param name="randomGenerator"></param>
        public static void Sample(long n, long N, int count, long low, long[] values, int fromIndex, RandomEngine randomGenerator)
        {
            if (n <= 0 || count <= 0) return;
            if (count > n) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_CountMustNotBeGreaterThanN);
            if (randomGenerator == null) randomGenerator = Cern.Jet.Random.AbstractDistribution.MakeDefaultGenerator();

            if (count == N)
            { // rare case treated quickly
                long val = low;
                int limit = fromIndex + count;
                for (int i = fromIndex; i < limit;) values[i++] = val++;
                return;
            }

            if (n < N * 0.95)
            { // || System.Math.Min(count,N-n)>maxTmpMemoryAllowed) {
                SampleMethodD(n, N, count, low, values, fromIndex, randomGenerator);
            }
            else
            { // More than 95% of all numbers shall be sampled.	
                RejectMethodD(n, N, count, low, values, fromIndex, randomGenerator);
            }


        }
        #endregion

        #region Local Protected Methods

        /// <summary>
        /// Efficiently computes a sorted random set of <tt>count</tt> elements from the interval <tt>[low,low+N-1]</tt>.
        /// Since we are talking about a random set, no element will occur more than once.
        ///
        /// <p>Running time is <tt>O(count)</tt>, on average. Space requirements are zero.
        ///
        /// <p>Numbers are filled into the specified array starting at index <tt>fromIndex</tt> to the right.
        /// The array is returned sorted ascending in the range filled with numbers.
        /// </summary>
        /// <param name="n">the total number of elements to choose (must be &gt;= 0).</param>
        /// <param name="N">the interval to choose random numbers from is <tt>[low,low+N-1]</tt>.</param>
        /// <param name="count">the number of elements to be filled into <tt>values</tt> by this call (must be &gt;= 0 and &lt;=<tt>n</tt>). Normally, you will set <tt>count=n</tt>.</param>
        /// <param name="low">the interval to choose random numbers from is <tt>[low,low+N-1]</tt>. Hint: If <tt>low==0</tt>, then draws random numbers from the interval <tt>[0,N-1]</tt>.</param>
        /// <param name="values">the array into which the random numbers are to be filled; must have a length <tt>&gt;= count+fromIndex</tt>.</param>
        /// <param name="fromIndex">the first index within <tt>values</tt> to be filled with numbers (inclusive).</param>
        /// <param name="randomGenerator">a random number generator.</param>
        protected static void RejectMethodD(long n, long N, int count, long low, long[] values, int fromIndex, RandomEngine randomGenerator)
        {
            /*  This algorithm is applicable if a large percentage (90%..100%) of N shall be sampled.
                In such cases it is more efficient than sampleMethodA() and sampleMethodD().
                The idea is that it is more efficient to express
                sample(n,N,count) in terms of reject(N-n,N,count)
                and then invert the result.
                For example, sampling 99% turns into sampling 1% plus inversion.

                This algorithm is the same as method sampleMethodD(..d) with the exception that sampled elements are rejected, and not sampled elements included in the result set.
            */
            n = N - n; // IMPORTANT !!!

            double nreal, Nreal, ninv, nmin1inv, U, X, Vprime, y1, y2, top, bottom, negSreal, qu1real;
            long qu1, t, limit;
            //long threshold;
            long S;
            long chosen = -1 + low;

            //long negalphainv = -13;  //tuning paramter, determines when to switch from method D to method Ad Dependent on programming language, platform, etc.

            nreal = n; ninv = 1.0 / nreal; Nreal = N;
            Vprime = System.Math.Exp(System.Math.Log(randomGenerator.Raw()) * ninv);
            qu1 = -n + 1 + N; qu1real = -nreal + 1.0 + Nreal;
            //threshold = -negalphainv * n;

            while (n > 1 && count > 0)
            { //&& threshold<N) {
                nmin1inv = 1.0 / (-1.0 + nreal);
                for (;;)
                {
                    for (;;)
                    { // step D2: generate U and X
                        X = Nreal * (-Vprime + 1.0); S = (long)X;
                        if (S < qu1) break;
                        Vprime = System.Math.Exp(System.Math.Log(randomGenerator.Raw()) * ninv);
                    }
                    U = randomGenerator.Raw(); negSreal = -S;

                    //step D3: Accept?
                    y1 = System.Math.Exp(System.Math.Log(U * Nreal / qu1real) * nmin1inv);
                    Vprime = y1 * (-X / Nreal + 1.0) * (qu1real / (negSreal + qu1real));
                    if (Vprime <= 1.0) break; //break inner loop

                    //step D4: Accept?
                    y2 = 1.0; top = -1.0 + Nreal;
                    if (n - 1 > S)
                    {
                        bottom = -nreal + Nreal; limit = -S + N;
                    }
                    else
                    {
                        bottom = -1.0 + negSreal + Nreal; limit = qu1;
                    }
                    for (t = N - 1; t >= limit; t--)
                    {
                        y2 = (y2 * top) / bottom;
                        top--;
                        bottom--;
                    }
                    if (Nreal / (-X + Nreal) >= y1 * System.Math.Exp(System.Math.Log(y2) * nmin1inv))
                    {
                        // accept !
                        Vprime = System.Math.Exp(System.Math.Log(randomGenerator.Raw()) * nmin1inv);
                        break; //break inner loop
                    }
                    Vprime = System.Math.Exp(System.Math.Log(randomGenerator.Raw()) * ninv);
                } //end for

                //step D5: reject the (S+1)st record !			
                int iter = count; //int iter = (int)(System.Math.Min(S,count));
                if (S < iter) iter = (int)S;

                count -= iter;
                for (; --iter >= 0;) values[fromIndex++] = ++chosen;
                chosen++;

                N -= S + 1; Nreal = negSreal + (-1.0 + Nreal);
                n--; nreal--; ninv = nmin1inv;
                qu1 = -S + qu1; qu1real = negSreal + qu1real;
                //threshold += negalphainv;
            } //end while


            if (count > 0)
            { //special case n==1
              //reject the (S+1)st record !
                S = (long)(N * Vprime);

                int iter = count; //int iter = (int)(System.Math.Min(S,count));
                if (S < iter) iter = (int)S;

                count -= iter;
                for (; --iter >= 0;) values[fromIndex++] = ++chosen;

                chosen++;

                // fill the rest
                for (; --count >= 0;) values[fromIndex++] = ++chosen;
            }
        }

        /// <summary>
        /// Efficiently computes a sorted random set of <tt>count</tt> elements from the interval <tt>[low,low+N-1]</tt>.
        /// Since we are talking about a random set, no element will occur more than once.
        ///
        /// <p>Running time is <tt>O(count)</tt>, on average. Space requirements are zero.
        ///
        /// <p>Numbers are filled into the specified array starting at index <tt>fromIndex</tt> to the right.
        /// The array is returned sorted ascending in the range filled with numbers.
        ///
        /// <p><b>Random number generation:</b> By default uses <tt>MersenneTwister</tt>, a very strong random number generator, much better than <tt>java.util.Random</tt>.
        /// You can also use other strong random number generators of Paul Houle's RngPack package.
        /// For example, <tt>Ranecu</tt>, <tt>Ranmar</tt> and <tt>Ranlux</tt> are strong well analyzed research grade pseudo-random number generators with known periods.
        /// </summary>
        /// <param name="n">the total number of elements to choose (must be <tt>n &gt;= 0</tt> and <tt>n &lt;= N</tt>).</param>
        /// <param name="N">the interval to choose random numbers from is <tt>[low,low+N-1]</tt>.</param>
        /// <param name="count">the number of elements to be filled into <tt>values</tt> by this call (must be &gt;= 0 and &lt;=<tt>n</tt>). Normally, you will set <tt>count=n</tt>.</param>
        /// <param name="low">the interval to choose random numbers from is <tt>[low,low+N-1]</tt>. Hint: If <tt>low==0</tt>, then draws random numbers from the interval <tt>[0,N-1]</tt>.</param>
        /// <param name="values">the array into which the random numbers are to be filled; must have a length <tt>&gt;= count+fromIndex</tt>.</param>
        /// <param name="fromIndex">the first index within <tt>values</tt> to be filled with numbers (inclusive).</param>
        /// <param name="randomGenerator">a random number generator. Set this parameter to <tt>null</tt> to use the default random number generator.</param>
        protected static void SampleMethodA(long n, long N, int count, long low, long[] values, int fromIndex, RandomEngine randomGenerator)
        {
            double V, quot, Nreal, top;
            long S;
            long chosen = -1 + low;

            top = N - n;
            Nreal = N;
            while (n >= 2 && count > 0)
            {
                V = randomGenerator.Raw();
                S = 0;
                quot = top / Nreal;
                while (quot > V)
                {
                    S++;
                    top--;
                    Nreal--;
                    quot = (quot * top) / Nreal;
                }
                chosen += S + 1;
                values[fromIndex++] = chosen;
                count--;
                Nreal--;
                n--;
            }

            if (count > 0)
            {
                // special case n==1
                S = (long)(System.Math.Round(Nreal) * randomGenerator.Raw());
                chosen += S + 1;
                values[fromIndex] = chosen;
            }
        }

        /// <summary>
        /// Computes a sorted random set of <tt>count</tt> elements from the interval <tt>[low,low+N-1]</tt>.
        /// Since we are talking about a random set, no element will occur more than once.
        ///
        /// <p>Running time is <tt>O(N)</tt>, on average. Space requirements are zero.
        ///
        /// <p>Numbers are filled into the specified array starting at index <tt>fromIndex</tt> to the right.
        /// The array is returned sorted ascending in the range filled with numbers.
        /// </summary>
        /// <param name="n">the total number of elements to choose (must be &gt;= 0).</param>
        /// <param name="N">the interval to choose random numbers from is <tt>[low,low+N-1]</tt>.</param>
        /// <param name="count">the number of elements to be filled into <tt>values</tt> by this call (must be &gt;= 0 and &lt;=<tt>n</tt>). Normally, you will set <tt>count=n</tt>.</param>
        /// <param name="low">the interval to choose random numbers from is <tt>[low,low+N-1]</tt>. Hint: If <tt>low==0</tt>, then draws random numbers from the interval <tt>[0,N-1]</tt>.</param>
        /// <param name="values">the array into which the random numbers are to be filled; must have a length <tt>&gt;= count+fromIndex</tt>.</param>
        /// <param name="fromIndex">the first index within <tt>values</tt> to be filled with numbers (inclusive).</param>
        /// <param name="randomGenerator">a random number generator.</param>
        protected static void SampleMethodD(long n, long N, int count, long low, long[] values, int fromIndex, RandomEngine randomGenerator)
        {
            double nreal, Nreal, ninv, nmin1inv, U, X, Vprime, y1, y2, top, bottom, negSreal, qu1real;
            long qu1, threshold, t, limit;
            long S;
            long chosen = -1 + low;

            long negalphainv = -13;  //tuning paramter, determines when to switch from method D to method Ad Dependent on programming language, platform, etc.

            nreal = n; ninv = 1.0 / nreal; Nreal = N;
            Vprime = System.Math.Exp(System.Math.Log(randomGenerator.Raw()) * ninv);
            qu1 = -n + 1 + N; qu1real = -nreal + 1.0 + Nreal;
            threshold = -negalphainv * n;

            while (n > 1 && count > 0 && threshold < N)
            {
                nmin1inv = 1.0 / (-1.0 + nreal);
                for (;;)
                {
                    for (;;)
                    { // step D2: generate U and X
                        X = Nreal * (-Vprime + 1.0); S = (long)X;
                        if (S < qu1) break;
                        Vprime = System.Math.Exp(System.Math.Log(randomGenerator.Raw()) * ninv);
                    }
                    U = randomGenerator.Raw(); negSreal = -S;

                    //step D3: Accept?
                    y1 = System.Math.Exp(System.Math.Log(U * Nreal / qu1real) * nmin1inv);
                    Vprime = y1 * (-X / Nreal + 1.0) * (qu1real / (negSreal + qu1real));
                    if (Vprime <= 1.0) break; //break inner loop

                    //step D4: Accept?
                    y2 = 1.0; top = -1.0 + Nreal;
                    if (n - 1 > S)
                    {
                        bottom = -nreal + Nreal; limit = -S + N;
                    }
                    else
                    {
                        bottom = -1.0 + negSreal + Nreal; limit = qu1;
                    }
                    for (t = N - 1; t >= limit; t--)
                    {
                        y2 = (y2 * top) / bottom;
                        top--;
                        bottom--;
                    }
                    if (Nreal / (-X + Nreal) >= y1 * System.Math.Exp(System.Math.Log(y2) * nmin1inv))
                    {
                        // accept !
                        Vprime = System.Math.Exp(System.Math.Log(randomGenerator.Raw()) * nmin1inv);
                        break; //break inner loop
                    }
                    Vprime = System.Math.Exp(System.Math.Log(randomGenerator.Raw()) * ninv);
                } //end for

                //step D5: select the (S+1)st record !
                chosen += S + 1;
                values[fromIndex++] = chosen;
                /*
                // invert
                for (int iter=0; iter<S && count > 0; iter++) {
                    values[fromIndex++] = ++chosen;
                    count--;
                }
                chosen++;
                */
                count--;

                N -= S + 1; Nreal = negSreal + (-1.0 + Nreal);
                n--; nreal--; ninv = nmin1inv;
                qu1 = -S + qu1; qu1real = negSreal + qu1real;
                threshold += negalphainv;
            } //end while


            if (count > 0)
            {
                if (n > 1)
                { //faster to use method A to finish the sampling
                    SampleMethodA(n, N, count, chosen + 1, values, fromIndex, randomGenerator);
                }
                else
                {
                    //special case n==1
                    S = (long)(N * Vprime);
                    chosen += S + 1;
                    values[fromIndex++] = chosen;
                }
            }
        }

        #endregion

        #region Local Private Methods

        #endregion

    }
}
