using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random.Sampling
{
    /// <summary>
    /// Benchmarks random number generation from various distributions as well as PDF and CDF lookups.
    /// </summary>
    public class Benchmark : Cern.Colt.PersistentObject
    {

        #region Local Variables
        private RandomEngine randomGenerator;
        #endregion

        #region Property
        RandomEngine RandomGenerator
        {
            get { return randomGenerator; }
            set { randomGenerator = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Benchmark() { }
        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        public static void demo1()
        {
            // Gamma distribution

            // define distribution parameters
            double mean = 5;
            double variance = 1.5;
            double alpha = mean * mean / variance;
            double lambda = 1 / (variance / mean);

            // for tests and debugging use a random engine with CONSTANT seed --> deterministic and reproducible results
            cern.jet.random.engine.RandomEngine engine = new cern.jet.random.engine.MersenneTwister();

            // your favourite distribution goes here
            cern.jet.random.AbstractDistribution dist = new cern.jet.random.Gamma(alpha, lambda, engine);

            // collect random numbers and print statistics
            int size = 100000;
            cern.colt.list.List<Double> numbers = new cern.colt.list.List<Double>(size);
            for (int i = 0; i < size; i++) numbers.Add(dist.nextDouble());

            hep.aida.bin.DynamicBin1D bin = new hep.aida.bin.DynamicBin1D();
            bin.addAllOf(numbers);
            Console.WriteLine(bin);
        }

        public static void main(String args[])
        {
            int size = int.parseInt(args[0]);
            Boolean print = new Boolean(args[1]).BooleanValue();
            double mean = new Double(args[2]).doubleValue();
            String generatorName = args[3];
            random(size, print, mean, generatorName);
        }

        public static void random(int size, Boolean print, double mean, String generatorName)
        {
            Console.WriteLine("Generating " + size + " random numbers per distribution...\n");

            //int large = 100000000;
            int largeVariance = 100;
            RandomEngine gen; // = new MersenneTwister();
            try
            {
                gen = (RandomEngine)Class.forName(generatorName).newInstance();
            }
            catch (Exception exc)
            {
                throw new InternalError(exc.getMessage());
            }
        }

        public static void randomInstance(int size, Boolean print, AbstractDistribution dist)
        {
            Console.Write("\n" + dist + " ...");
            cern.colt.Timer timer = new cern.colt.Timer().start();

            for (int i = size; --i >= 0;)
            {
                double rand = dist.nextDouble();
                if (print)
                {
                    if ((size - i - 1) % 8 == 0) Console.WriteLine();
                    Console.Write((float)rand + ", ");
                }
            }

            timer.stop();
            Console.WriteLine("\n" + timer);
        }

        public static void test(int size, AbstractDistribution distribution)
        {
            for (int j = 0, i = size; --i >= 0; j++)
            {
                Console.Write(" " + distribution.nextDouble());
                if (j % 8 == 7) Console.WriteLine();
            }
            Console.WriteLine("\n\nGood bye.\n");
        }

        public static void test2(int size, AbstractDistribution distribution)
        {
            hep.aida.bin.DynamicBin1D bin = new hep.aida.bin.DynamicBin1D();
            for (int j = 0, i = size; --i >= 0; j++)
            {
                bin.Add(distribution.nextDouble());
            }
            Console.WriteLine(bin);
            Console.WriteLine("\n\nGood bye.\n");
        }

        public static void test2(int size, AbstractDistribution a, AbstractDistribution b)
        {
            hep.aida.bin.DynamicBin1D binA = new hep.aida.bin.DynamicBin1D();
            hep.aida.bin.DynamicBin1D binB = new hep.aida.bin.DynamicBin1D();
            for (int j = 0, i = size; --i >= 0; j++)
            {
                binA.Add(a.nextDouble());
                binB.Add(b.nextDouble());
            }
        }
        #endregion

        #region Local Private Methods

        #endregion

    }
}
