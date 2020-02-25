using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Cern.Colt;
using Cern.Colt.List;
using Cern.Jet.Stat.Quantile;

namespace Cern.Colt.Tests
{
    /// <summary>
    /// A class holding test cases for exact and approximate quantile finders.
    /// </summary>
    [TestFixture]
    public class QuantileFinderTest
    {
        [Test]
        public void TestMain()
        {
            var path = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "\\TestResult\\QuantileFinderTest\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filename = path + "QuantileFinderTest.log";

            //Boolean known_N;
            //if (args==null) known_N = false;
            //else known_N = new Boolean(args[0]).BooleanValue();

            int[] quantiles = { 100, 10000 };
            //int[] quantiles = {1,100,10000};

            long[] sizes = { long.MaxValue, 1000000, 10000000, 100000000 };

            double[] deltas = { 0.0, 0.1, 0.00001 };
            //double[] deltas = {0.0, 0.001, 0.00001, 0.000001};

            //double[] epsilons = {0.0, 0.01, 0.001, 0.0001, 0.00001};
            double[] epsilons = { 0.0, 0.1, 0.01, 0.001, 0.0001, 0.00001, 0.000001 };


            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    //if (! known_N) sizes = new long[] {0};
                    writer.WriteLine("\n\n");
                    //if (known_N) 
                    //	Console.WriteLine("Computing b's and k's for KNOWN N");
                    //else 
                    //	Console.WriteLine("Computing b's and k's for UNKNOWN N");
                    writer.WriteLine("mem [System.Math.Round(elements/1000.0)]");
                    writer.WriteLine("***********************************");
                    Timer timer = new Timer();
                    timer.Start();

                    for (int q = 0; q < quantiles.Length; q++)
                    {
                        int p = quantiles[q];
                        writer.WriteLine("------------------------------");
                        writer.WriteLine("computing for p = " + p);
                        for (int s = 0; s < sizes.Length; s++)
                        {
                            long N = sizes[s];
                            writer.WriteLine("   ------------------------------");
                            writer.WriteLine("   computing for N = " + N);
                            for (int e = 0; e < epsilons.Length; e++)
                            {
                                double epsilon = epsilons[e];
                                writer.WriteLine("      ------------------------------");
                                writer.WriteLine("      computing for e = " + epsilon);
                                for (int d = 0; d < deltas.Length; d++)
                                {
                                    double delta = deltas[d];
                                    for (int knownCounter = 0; knownCounter < 2; knownCounter++)
                                    {
                                        Boolean known_N;
                                        if (knownCounter == 0) known_N = true;
                                        else known_N = false;

                                        IDoubleQuantileFinder finder = QuantileFinderFactory.NewDoubleQuantileFinder(known_N, N, epsilon, delta, p, null);
                                        //Console.WriteLine(finder.this.GetType().Name);
                                        /*
                                        double[] returnSamplingRate = new double[1];
                                        long[] result;
                                        if (known_N) {
                                            result = QuantileFinderFactory.known_N_compute_B_and_K(N, epsilon, delta, p, returnSamplingRate);
                                        }
                                        else {
                                            result = QuantileFinderFactory.unknown_N_compute_B_and_K(epsilon, delta, p);
                                            long b1 = result[0];
                                            long k1 = result[1];

                                            if (N>=0) {
                                                long[] resultKnown = QuantileFinderFactory.known_N_compute_B_and_K(N, epsilon, delta, p, returnSamplingRate);
                                                long b2 = resultKnown[0];
                                                long k2 = resultKnown[1];

                                                if (b2 * k2 < b1 * k1) { // the KnownFinder is smaller
                                                    result = resultKnown;
                                                }
                                            }
                                        }


                                        long b = result[0];
                                        long k = result[1];
                                        */
                                        String knownStr = known_N ? "  known" : "unknown";
                                        long mem = finder.TotalMemory();
                                        if (mem == 0) mem = N;
                                        //else if (mem==0 && !known_N && N<0) mem = long.MaxValue; // actually infinity
                                        //else if (mem==0 && !known_N && N>=0) mem = N;
                                        //Console.Write("         (e,d,N,p)=("+epsilon+","+delta+","+N+","+p+") --> ");
                                        writer.Write("         (known, d)=(" + knownStr + ", " + delta + ") --> ");
                                        //Console.Write("(mem,b,k,memF");
                                        writer.Write("(MB,mem");
                                        //if (known_N) Console.Write(",sampling");
                                        //Console.Write(")=("+(System.Math.Round(b*k/1000.0))+","+b+","+k+", "+System.Math.Round(b*k*8/1024.0/1024.0));
                                        //Console.Write(")=("+b*k/1000.0+","+b+","+k+", "+b*k*8/1024.0/1024.0+", "+System.Math.Round(b*k*8/1024.0/1024.0));
                                        writer.Write(")=(" + mem * 8.0 / 1024.0 / 1024.0 + ",  " + mem / 1000.0 + ",  " + System.Math.Round(mem * 8.0 / 1024.0 / 1024.0));
                                        //if (known_N) Console.Write(","+returnSamplingRate[0]);
                                        writer.WriteLine(")");
                                    }
                                }
                            }
                        }
                    }
                    timer.Stop();
                }
            }
            catch (IOException x)
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.Write(x.StackTrace);
                }
            }
        }

        /// <summary>
        /// Finds the first and last indexes of a specific element within a sorted list.
        /// <summary>
        /// <returns>int[]</returns>
        /// <param name="list">cern.colt.list.DoubleArrayList</param>
        /// <param name="element">the element to search for</param>
        protected static IntArrayList BinaryMultiSearch(DoubleArrayList list, double element)
        {
            int index = list.BinarySearch(element);
            if (index < 0) return null; //not found

            int from = index - 1;
            while (from >= 0 && list[from] == element) from--;
            from++;

            int to = index + 1;
            while (to < list.Count && list[to] == element) to++;
            to--;

            return new IntArrayList(new int[] { from, to });
        }
        /// <summary>
        /// Observed epsilon
        /// <summary>
        public static double Epsilon(int size, double phi, double rank)
        {
            double s = size;
            //Console.WriteLine("\n");
            //Console.WriteLine("s="+size+", rank="+rank+", phi="+phi+", eps="+System.Math.Abs((rank)/s - phi));
            //Console.WriteLine("\n");
            return System.Math.Abs(rank / s - phi);
        }
        /// <summary>
        /// Observed epsilon
        /// <summary>
        public static double Epsilon(DoubleArrayList sortedList, double phi, double element)
        {
            double rank = Cern.Jet.Stat.Descriptive.RankInterpolated(sortedList, element);
            return Epsilon(sortedList.Count, phi, rank);
        }
        /// <summary>
        /// Observed epsilon
        /// <summary>
        public static double Epsilon(DoubleArrayList sortedList, IDoubleQuantileFinder finder, double phi)
        {
            double element = finder.QuantileElements(new DoubleArrayList(new double[] { phi }))[0];
            return Epsilon(sortedList, phi, element);
        }

        /// <summary>
        /// This method was created in VisualAge.
        /// <summary>
        /// <returns>double[]</returns>
        /// <param name="values">cern.it.hepodbms.primitivearray.DoubleArrayList</param>
        /// <param name="phis">double[]</param>
        public static double ObservedEpsilonAtPhi(double phi, ExactDoubleQuantileFinder exactFinder, IDoubleQuantileFinder approxFinder)
        {
            int N = (int)exactFinder.Size;

            int exactRank = (int)Utils.EpsilonCeiling(phi * N) - 1;
            //Console.WriteLine("exactRank="+exactRank);
            var tmp = exactFinder.QuantileElements(new DoubleArrayList(new double[] { phi }))[0]; // just to ensure exactFinder is sorted
            double approxElement = approxFinder.QuantileElements(new DoubleArrayList(new double[] { phi }))[0];
            //Console.WriteLine("approxElem="+approxElement);
            IntArrayList approxRanks = BinaryMultiSearch(exactFinder.Buffer, approxElement);
            int from = approxRanks[0];
            int to = approxRanks[1];

            int distance;
            if (from <= exactRank && exactRank <= to) distance = 0;
            else
            {
                if (from > exactRank) distance = System.Math.Abs(from - exactRank);
                else distance = System.Math.Abs(exactRank - to);
            }

            double epsilon = (double)distance / (double)N;
            return epsilon;
        }
        /// <summary>
        /// This method was created in VisualAge.
        /// <summary>
        /// <returns>double[]</returns>
        /// <param name="values">cern.it.hepodbms.primitivearray.DoubleArrayList</param>
        /// <param name="phis">double[]</param>
        public static DoubleArrayList ObservedEpsilonsAtPhis(DoubleArrayList phis, ExactDoubleQuantileFinder exactFinder, IDoubleQuantileFinder approxFinder, double desiredEpsilon)
        {
            DoubleArrayList epsilons = new DoubleArrayList(phis.Count);

            for (int i = phis.Count; --i >= 0;)
            {
                double epsilon = ObservedEpsilonAtPhi(phis[i], exactFinder, approxFinder);
                epsilons.Add(epsilon);
                if (epsilon > desiredEpsilon) Console.WriteLine("Real epsilon = " + epsilon + " is larger than desired by " + (epsilon - desiredEpsilon));
            }
            return epsilons;
        }
        /// <summary>
        /// Not yet commented.
        /// <summary>
        public static void Test()
        {
            String[] args = new String[20];

            String size = "10000";
            args[0] = size;

            //String b="5";
            String b = "12";
            args[1] = b;

            String k = "2290";
            args[2] = k;

            String enableLogging = "log";
            args[3] = enableLogging;

            String chunks = "10";
            args[4] = chunks;

            String computeExactQuantilesAlso = "exact";
            args[5] = computeExactQuantilesAlso;

            String doShuffle = "shuffle";
            args[6] = doShuffle;

            String epsilon = "0.001";
            args[7] = epsilon;

            String delta = "0.0001";
            //String delta = "0.0001";
            args[8] = delta;

            String quantiles = "1";
            args[9] = quantiles;

            String max_N = "-1";
            args[10] = max_N;


            TestQuantileCalculation(args);
        }

        /// <summary>
        /// This method was created in VisualAge.
        /// <summary>
        public static void TestLocalVarDeclarationSpeed(int size)
        {
            Console.WriteLine("free=" + Cern.Colt.Karnel.FreePhysicalMemorySize);
            Console.WriteLine("total=" + Cern.Colt.Karnel.TotalVisibleMemorySize);

            /*Timer timer = new Timer().Start();
            for (int i=0; i<size; i++) {
                for (int j=0; j<size; j++) {
                    DoubleBuffer buffer=null;
                    int val=10;
                    double f=1.0f;
                }
            }
            Console.WriteLine(timer.Stop());
            */

            Timer timer = new Timer();
            timer.Start();
            DoubleBuffer buffer;
            int val;
            double f;
            int j;

            for (int i = 0; i < size; i++)
            {
                for (j = 0; j < size; j++)
                {
                    buffer = null;
                    val = 10;
                    f = 1.0f;
                }
            }
            Console.WriteLine(timer.Interval);
            timer.Stop();

            Console.WriteLine("free=" + Cern.Colt.Karnel.FreePhysicalMemorySize);
            Console.WriteLine("total=" + Cern.Colt.Karnel.TotalVisibleMemorySize);
        }
        /// <summary>
        /// <summary>
        public static void TestQuantileCalculation(String[] args)
        {
            int size = int.Parse(args[0]);
            int b = int.Parse(args[1]);
            int k = int.Parse(args[2]);
            //cern.it.util.Log.enableLogging(args[3].Equals("log"));
            int chunks = int.Parse(args[4]);
            Boolean computeExactQuantilesAlso = args[5].Equals("exact");
            Boolean doShuffle = args[6].Equals("shuffle");
            double epsilon = Double.Parse(args[7]);
            double delta = Double.Parse(args[8]);
            int quantiles = int.Parse(args[9]);
            long max_N = long.Parse(args[10]);



            Console.WriteLine("free=" + Cern.Colt.Karnel.FreePhysicalMemorySize);
            Console.WriteLine("total=" + Cern.Colt.Karnel.TotalVisibleMemorySize);

            double[] phis = { 0.001, 0.01, 0.1, 0.5, 0.9, 0.99, 0.999, 1.0 };
            //int quantiles = phis.Length;

            Timer timer = new Timer();
            Timer timer2 = new Timer();
            IDoubleQuantileFinder approxFinder;

            approxFinder = QuantileFinderFactory.NewDoubleQuantileFinder(false, max_N, epsilon, delta, quantiles, null);
            Console.WriteLine(approxFinder);
            //new UnknownApproximateDoubleQuantileFinder(b,k);
            //approxFinder = new ApproximateDoubleQuantileFinder(b,k);
            /*
            double[] returnSamplingRate = new double[1];
            long[] result = ApproximateQuantileFinder.computeBestBandK(size*chunks, epsilon, delta, quantiles, returnSamplingRate);
            approxFinder = new ApproximateQuantileFinder((int) result[0], (int) result[1]);
            Console.WriteLine("epsilon="+epsilon);
            Console.WriteLine("delta="+delta);
            Console.WriteLine("samplingRate="+returnSamplingRate[0]);
            */


            IDoubleQuantileFinder exactFinder = QuantileFinderFactory.NewDoubleQuantileFinder(false, -1, 0.0, delta, quantiles, null);
            Console.WriteLine(exactFinder);

            DoubleArrayList list = new DoubleArrayList(size);

            for (int chunk = 0; chunk < chunks; chunk++)
            {
                list.Clear();
                int d = chunk * size;
                timer2.Start();
                for (int i = 0; i < size; i++)
                {
                    list.Add((double)(i + d));
                }
                timer2.Stop();



                //Console.WriteLine("unshuffled="+list);
                if (doShuffle)
                {
                    Timer timer3 = new Timer();
                    timer3.Start();

                    list.Shuffle();
                    Console.WriteLine("shuffling took " + timer3.Interval.ToString());

                    timer3.Stop();
                }
                //Console.WriteLine("shuffled="+list);
                //list.sort();
                //Console.WriteLine("sorted="+list);

                timer.Start();
                approxFinder.AddAllOf(list);
                timer.Stop();

                if (computeExactQuantilesAlso)
                {
                    exactFinder.AddAllOf(list);
                }

            }
            Console.WriteLine("list.Add() took" + timer2);
            Console.WriteLine("approxFinder.Add() took" + timer);

            //Console.WriteLine("free="+Cern.Colt.Karnel.FreePhysicalMemorySize);
            //Console.WriteLine("total="+Cern.Colt.Karnel.TotalVisibleMemorySize);

            timer.Stop();
            timer.Start();

            //approxFinder.close();
            DoubleArrayList approxQuantiles = approxFinder.QuantileElements(new DoubleArrayList(phis));

            Console.WriteLine(timer.Display);
            timer.Stop();

            Console.WriteLine("Phis=" + new DoubleArrayList(phis));
            Console.WriteLine("ApproxQuantiles=" + approxQuantiles);

            //Console.WriteLine("MaxLevel of full buffers="+maxLevelOfFullBuffers(approxFinder.bufferSet));

            //Console.WriteLine("total buffers filled="+ approxFinder.totalBuffersFilled);
            //Console.WriteLine("free="+Cern.Colt.Karnel.FreePhysicalMemorySize);
            //Console.WriteLine("total="+Cern.Colt.Karnel.TotalVisibleMemorySize);


            if (computeExactQuantilesAlso)
            {
                Console.WriteLine("Comparing with exact quantile computation...");

                timer.Reset();

                //exactFinder.close();
                DoubleArrayList exactQuantiles = exactFinder.QuantileElements(new DoubleArrayList(phis));
                Console.WriteLine(timer.Display);

                timer.Stop();

                Console.WriteLine("ExactQuantiles=" + exactQuantiles);


                //double[] errors1 = errors1(exactQuantiles.ToArray(), approxQuantiles.ToArray());
                //Console.WriteLine("Error1="+new DoubleArrayList(errors1));

                /*
                DoubleArrayList buffer = new DoubleArrayList((int)exactFinder.Count);
                exactFinder.forEach(
                    new cern.colt.function.DoubleFunction() {
                        public void apply(double element) {
                            buffer.Add(element);
                        }
                    }
                );
                */


                DoubleArrayList observedEpsilons = ObservedEpsilonsAtPhis(new DoubleArrayList(phis), (ExactDoubleQuantileFinder)exactFinder, approxFinder, epsilon);
                Console.WriteLine("observedEpsilons=" + observedEpsilons);

                double element = 1000.0f;


                Console.WriteLine("exact phi(" + element + ")=" + exactFinder.Phi(element));
                Console.WriteLine("apprx phi(" + element + ")=" + approxFinder.Phi(element));

                Console.WriteLine("exact elem(phi(" + element + "))=" + exactFinder.QuantileElements(new DoubleArrayList(new double[] { exactFinder.Phi(element) })));
                Console.WriteLine("apprx elem(phi(" + element + "))=" + approxFinder.QuantileElements(new DoubleArrayList(new double[] { approxFinder.Phi(element) })));
            }
        }
        /// <summary>
        /// Not yet commented.
        /// <summary>
        public static void TestRank()
        {
            DoubleArrayList list = new DoubleArrayList(new double[] { 1.0f, 5.0f, 5.0f, 5.0f, 7.0f, 10.0f });
            //Console.WriteLine(rankOfWithin(5.0f, list));
        }
    }
}
