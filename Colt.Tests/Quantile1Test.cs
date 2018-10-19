using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NUnit.Framework;
using Cern.Jet.Stat.Quantile;
using Cern.Jet.Random;
using Cern.Jet.Random.Engine;
using Cern.Hep.Aida.Bin;

namespace Colt.Tests
{
        /**
         * A class to test the QuantileBin1D code.
         * The command line is "java Quantile1Test numExamples N"
         * where numExamples is the number of random (Gaussian) numbers to
         * be presented to the QuantileBin1D.add method, and N is
         * the absolute maximum number of examples the QuantileBin1D is setup
         * to receive in the constructord  N can be set to "L", which will use
         * long.MaxValue, or to "I", which will use int.MaxValue, or to 
         * any positive long value.
         */
        public class Quantile1Test
        {
            public static void QuantileBin1DTest(String[] argv)
            {
                /*
                 * Get the number of examples from the first argument
                 */
                int numExamples = 0;
                try
                {
                    numExamples = int.Parse(argv[0]);
                }
                catch (Exception e)
                {
                    Assert.Inconclusive("Unable to parse input line count argument");
                    Assert.Inconclusive(e.Message);
                }
                Console.WriteLine("Got numExamples=" + numExamples);

                /*
                 * Get N from the second argument
                 */
                long N = 0;
                try
                {
                    if (argv[1].Equals("L"))
                    {
                        N = long.MaxValue;
                    }
                    else if (argv[1].Equals("I"))
                    {
                        N = (long)int.MaxValue;
                    }
                    else
                    {
                        N = long.Parse(argv[1]);
                    }
                }
                catch (Exception e)
                {
                    Assert.Inconclusive("Error parsing flag for N");
                    Assert.Inconclusive(e.Message);
                }
                Console.WriteLine("Got N=" + N);

                /*
                 * Set up the QuantileBin1D object
                 */
                DRand rand = new DRand(new DateTime());
                QuantileBin1D qAccum = new QuantileBin1D(false,
                                     N,
                                     1e-4,
                                     1e-3,
                                     200,
                                     rand,
                                     false,
                                     false,
                                     2);

                DynamicBin1D dbin = new DynamicBin1D();

                /*
                 * Use a new random number generator to generate numExamples
                 * random gaussians, and add them to the QuantileBin1D
                 */
                Uniform dataRand = new Uniform(new DRand(7757));
                for (int i = 1; i <= numExamples; i++)
                {
                    double gauss = dataRand.NextDouble();
                    qAccum.Add(gauss);
                    dbin.Add(gauss);
                }

            /*
             * print out the percentiles
             */
            //DecimalFormat fmt = new DecimalFormat("0.00");
            Console.WriteLine();
                //int step = 1;
                int step = 10;
                for (int i = 1; i < 100;)
                {
                    double percent = ((double)i) * 0.01;
                    double quantile = qAccum.Quantile(percent);
                    Console.WriteLine(percent.ToString("0.00") + "  " + quantile + ",  " + dbin.Quantile(percent) + ",  " + (dbin.Quantile(percent) - quantile));
                    i = i + step;
                }
            }
        }
}
