using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NUnit.Framework;
using Cern.Jet.Stat.Quantile;
using Cern.Jet.Random;
using Cern.Jet.Random.Engine;
using Cern.Hep.Aida.Bin;

namespace Cern.Colt.Tests
{
    /// <summary>
    /// A class to test the QuantileBin1D code.
    /// The command line is "java Quantile1Test numExamples N"
    /// where numExamples is the number of random (Gaussian) numbers to
    /// be presented to the QuantileBin1D.add method, and N is
    /// the absolute maximum number of examples the QuantileBin1D is setup
    /// to receive in the constructord  N can be set to "L", which will use
    /// long.MaxValue, or to "I", which will use int.MaxValue, or to
    /// any positive long value.
    /// <summary>
    [TestFixture]
    public class Quantile1Test
    {
        [Test]
        public void QuantileBin1DTest()
        {
            var path = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "\\TestResult\\QuantileBin1DTest\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filename = path + "QuantileBin1DTest.log";
            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    var argv = new String[] { "100", "L" };
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
                        // Assert.Inconclusive("Unable to parse input line count argument");
                        // Assert.Inconclusive(e.Message);
                    }
                    writer.WriteLine("Got numExamples=" + numExamples);

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
                        // Assert.Inconclusive("Error parsing flag for N");
                        // Assert.Inconclusive(e.Message);
                    }
                    writer.WriteLine("Got N=" + N);

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
                    writer.WriteLine();
                    //int step = 1;
                    int step = 10;
                    for (int i = 1; i < 100;)
                    {
                        double percent = ((double)i) * 0.01;
                        double quantile = qAccum.Quantile(percent);

                        writer.WriteLine(percent.ToString("0.00") + "  " + quantile + ",  " + dbin.Quantile(percent) + ",  " + (dbin.Quantile(percent) - quantile));
                        i = i + step;
                    }
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
        /// Return the quartile values of an ordered set of doubles
        ///   assume the sorting has already been done.
        ///   
        /// This actually turns out to be a bit of a PITA, because there is no universal agreement 
        ///   on choosing the quartile values. In the case of odd values, some count the median value
        ///   in finding the 1st and 3rd quartile and some discard the median value. 
        ///   the two different methods result in two different answers.
        ///   The below method produces the arithmatic mean of the two methods, and insures the median
        ///   is given it's correct weight so that the median changes as smoothly as possible as 
        ///   more data ppints are added.
        ///    
        /// This method uses the following logic:
        /// 
        /// ===If there are an even number of data points:
        ///    Use the median to divide the ordered data set into two halves. 
        ///    The lower quartile value is the median of the lower half of the data. 
        ///    The upper quartile value is the median of the upper half of the data.
        ///    
        /// ===If there are (4n+1) data points:
        ///    The lower quartile is 25% of the nth data value plus 75% of the (n+1)th data value.
        ///    The upper quartile is 75% of the (3n+1)th data point plus 25% of the (3n+2)th data point.
        ///    
        ///===If there are (4n+3) data points:
        ///   The lower quartile is 75% of the (n+1)th data value plus 25% of the (n+2)th data value.
        ///   The upper quartile is 25% of the (3n+2)th data point plus 75% of the (3n+3)th data point.
        /// 
        /// </summary>
        internal Tuple<double, double, double> Quartiles(double[] afVal)
        {
            int iSize = afVal.Length;
            int iMid = iSize / 2; //this is the mid from a zero based index, eg mid of 7 = 3;

            double fQ1 = 0;
            double fQ2 = 0;
            double fQ3 = 0;

            if (iSize % 2 == 0)
            {
                //================ EVEN NUMBER OF POINTS: =====================
                //even between low and high point
                fQ2 = (afVal[iMid - 1] + afVal[iMid]) / 2;

                int iMidMid = iMid / 2;

                //easy split 
                if (iMid % 2 == 0)
                {
                    fQ1 = (afVal[iMidMid - 1] + afVal[iMidMid]) / 2;
                    fQ3 = (afVal[iMid + iMidMid - 1] + afVal[iMid + iMidMid]) / 2;
                }
                else
                {
                    fQ1 = afVal[iMidMid];
                    fQ3 = afVal[iMidMid + iMid];
                }
            }
            else if (iSize == 1)
            {
                //================= special case, sorry ================
                fQ1 = afVal[0];
                fQ2 = afVal[0];
                fQ3 = afVal[0];
            }
            else
            {
                //odd number so the median is just the midpoint in the array.
                fQ2 = afVal[iMid];

                if ((iSize - 1) % 4 == 0)
                {
                    //======================(4n-1) POINTS =========================
                    int n = (iSize - 1) / 4;
                    fQ1 = (afVal[n - 1] * .25) + (afVal[n] * .75);
                    fQ3 = (afVal[3 * n] * .75) + (afVal[3 * n + 1] * .25);
                }
                else if ((iSize - 3) % 4 == 0)
                {
                    //======================(4n-3) POINTS =========================
                    int n = (iSize - 3) / 4;

                    fQ1 = (afVal[n] * .75) + (afVal[n + 1] * .25);
                    fQ3 = (afVal[3 * n + 1] * .25) + (afVal[3 * n + 2] * .75);
                }
            }

            return new Tuple<double, double, double>(fQ1, fQ2, fQ3);
        }
    }
}
