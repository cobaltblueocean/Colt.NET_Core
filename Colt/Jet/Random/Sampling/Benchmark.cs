using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;
using Cern.Hep.Aida.Bin;
using Cern.Colt.List;

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
            RandomEngine engine = new MersenneTwister();

            // your favourite distribution goes here
            AbstractDistribution dist = new Gamma(alpha, lambda, engine);

            // collect random numbers and print statistics
            int size = 100000;
            var numbers = new DoubleArrayList(size);
            for (int i = 0; i < size; i++) numbers.Add(dist.NextDouble());

            DynamicBin1D bin = new DynamicBin1D();
            bin.AddAllOf(numbers);
            Console.WriteLine(bin);
        }

        public static void main(String[] args)
        {
            int size = int.Parse(args[0]);
            Boolean print = Boolean.Parse(args[1]);
            double mean = Double.Parse(args[2]);
            String generatorName = args[3];
            random(size, print, mean, generatorName);
        }

        public static void random(int size, Boolean print, double mean, String generatorName)
        {
            Console.WriteLine("Generating " + size + " random numbers per distribution...\n");

            //int large = 100000000;
            int largeVariance = 100;
            RandomEngine gen; // = new MersenneTwister();
            gen = (RandomEngine)Activator.CreateInstance(Type.GetType(generatorName)); //(RandomEngine)Class.forName(generatorName).newInstance();
        }

        public static void randomInstance(int size, Boolean print, AbstractDistribution dist)
        {
            Console.Write("\n" + dist + " ...");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Start();

            for (int i = size; --i >= 0;)
            {
                double rand = dist.NextDouble();
                if (print)
                {
                    if ((size - i - 1) % 8 == 0) Console.WriteLine();
                    Console.Write((float)rand + ", ");
                }
            }

            timer.Stop();
            Console.WriteLine("\n" + timer);
        }

        public static void test(int size, AbstractDistribution distribution)
        {
            for (int j = 0, i = size; --i >= 0; j++)
            {
                Console.Write(" " + distribution.NextDouble());
                if (j % 8 == 7) Console.WriteLine();
            }
            Console.WriteLine("\n\nGood bye.\n");
        }

        public static void test2(int size, AbstractDistribution distribution)
        {
            DynamicBin1D bin = new DynamicBin1D();
            for (int j = 0, i = size; --i >= 0; j++)
            {
                bin.Add(distribution.NextDouble());
            }
            Console.WriteLine(bin);
            Console.WriteLine("\n\nGood bye.\n");
        }

        public static void test2(int size, AbstractDistribution a, AbstractDistribution b)
        {
            DynamicBin1D binA = new DynamicBin1D();
            DynamicBin1D binB = new DynamicBin1D();
            for (int j = 0, i = size; --i >= 0; j++)
            {
                binA.Add(a.NextDouble());
                binB.Add(b.NextDouble());
            }
        }
        #endregion

        #region Local Private Methods

        #endregion

    }
}
