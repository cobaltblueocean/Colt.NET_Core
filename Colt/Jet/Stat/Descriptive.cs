using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.List;

namespace Cern.Jet.Stat
{
    /// <summary>
    /// Basic descriptive statistics.
    /// </summary>
    public class Descriptive
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Descriptive() { }

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods

        /// <summary>
        /// Returns the auto-correlation of a data sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lag"></param>
        /// <param name="mean"></param>
        /// <param name="variance"></param>
        /// <returns></returns>
        public static double AutoCorrelation(DoubleArrayList data, int lag, double mean, double variance)
        {
            int N = data.Size;
            if (lag >= N) throw new ArgumentException("Lag is too large");

            double[] elements = data.ToArray();
            double run = 0;
            for (int i = lag; i < N; ++i)
                run += (elements[i] - mean) * (elements[i - lag] - mean);

            return (run / (N - lag)) / variance;
        }

        /// <summary>
        /// Returns the correlation of two data sequences.
        /// That is <tt>covariance(data1, data2)/(standardDev1 * standardDev2)</tt>.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="standardDev1"></param>
        /// <param name="data2"></param>
        /// <param name="standardDev2"></param>
        /// <returns></returns>
        public static double Correlation(DoubleArrayList data1, double standardDev1, DoubleArrayList data2, double standardDev2)
        {
            return Covariance(data1, data2) / (standardDev1 * standardDev2);
        }

        /// <summary>
        /// Returns the covariance of two data sequences, which is 
        /// <tt>cov(x,y) = (1/(size()-1)) * Sum((x[i]-mean(x)) * (y[i]-mean(y)))</tt>.
        /// See the <A HREF="http://www.cquest.utoronto.ca/geog/ggr270y/notes/not05efg.html"> math definition</A>.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public static double Covariance(DoubleArrayList data1, DoubleArrayList data2)
        {
            int size = data1.Size;
            if (size != data2.Size || size == 0) throw new ArgumentException();
            double[] elements1 = data1.ToArray();
            double[] elements2 = data2.ToArray();

            double sumx = elements1[0], sumy = elements2[0], Sxy = 0;
            for (int i = 1; i < size; ++i)
            {
                double x = elements1[i];
                double y = elements2[i];
                sumx += x;
                Sxy += (x - sumx / (i + 1)) * (y - sumy / i);
                sumy += y;
                // Exercise for the reader: Why does this give us the right answer?
            }
            return Sxy / (size - 1);
        }

        /// <summary>
        /// Durbin-Watson computation.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double DurbinWatson(DoubleArrayList data)
        {
            int size = data.Size;
            if (size < 2) throw new ArgumentException("data sequence must contain at least two values.");

            double[] elements = data.ToArray();
            double run = 0;
            double run_sq = 0;
            run_sq = elements[0] * elements[0];
            for (int i = 1; i < size; ++i)
            {
                double x = elements[i] - elements[i - 1];
                run += x * x;
                run_sq += elements[i] * elements[i];
            }

            return run / run_sq;
        }

        /// <summary>
        /// Computes the frequency (number of occurances, count) of each distinct value in the given sorted data.
        /// After this call returns both <tt>distinctValues</tt> and <tt>frequencies</tt> have a new size (which is equal for both), which is the number of distinct values in the sorted data.
        /// <p>
        /// Distinct values are filled into <tt>distinctValues</tt>, starting at index 0.
        /// The frequency of each distinct value is filled into <tt>frequencies</tt>, starting at index 0.
        /// As a result, the smallest distinct value (and its frequency) can be found at index 0, the second smallest distinct value (and its frequency) at index 1, ..., the largest distinct value (and its frequency) at index <tt>distinctValues.size()-1</tt>.
        /// </summary>
        /// <param name="sortedData">the data; must be sorted ascending.</param>
        /// <param name="distinctValues">a list to be filled with the distinct values; can have any size.</param>
        /// <param name="frequencies">a list to be filled with the frequencies; can have any size; set this parameter to <tt>null</tt> to ignore it.</param>
        public static void Frequencies(DoubleArrayList sortedData, DoubleArrayList distinctValues, IntArrayList frequencies)
        {
            distinctValues.Clear();
            if (frequencies != null) frequencies.Clear();

            double[] sortedElements = sortedData.ToArray();
            int size = sortedData.Size;
            int i = 0;

            while (i < size)
            {
                double element = sortedElements[i];
                int cursor = i;

                // determine run Length (number of equal elements)
                while (++i < size && sortedElements[i] == element) ;

                int runLength = i - cursor;
                distinctValues.Add(element);
                if (frequencies != null) frequencies.Add(runLength);
            }
        }

        /// <summary>
        /// Returns the geometric mean of a data sequence.
        /// Note that for a geometric mean to be meaningful, the minimum of the data sequence must not be less or equal to zero.
        /// 
        /// The geometric mean is given by <tt>pow( Product( data[i] ), 1/size)</tt>
        /// which is equivalent to <tt>Math.exp( Sum( Log(data[i]) ) / size)</tt>.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sumOfLogarithms"></param>
        /// <returns></returns>
        public static double GeometricMean(int size, double sumOfLogarithms)
        {
            return System.Math.Exp(sumOfLogarithms / size);

            // this version would easily results in overflows
            //return System.Math.Pow(product, 1/size);
        }

        /// <summary>
        /// Returns the geometric mean of a data sequence.
        /// Note that for a geometric mean to be meaningful, the minimum of the data sequence must not be less or equal to zero.
        /// 
        /// The geometric mean is given by <tt>pow( Product( data[i] ), 1/data.size())</tt>.
        /// This method tries to avoid overflows at the expense of an equivalent but somewhat slow definition:
        /// <tt>geo = Math.exp( Sum( Log(data[i]) ) / data.size())</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double GeometricMean(DoubleArrayList data)
        {
            return GeometricMean(data.Size, SumOfLogarithms(data, 0, data.Size - 1));
        }

        /// <summary>
        /// Returns the harmonic mean of a data sequence.
        /// </summary>
        /// <param name="size">the number of elements in the data sequence.</param>
        /// <param name="sumOfInversions"><tt>Sum( 1.0 / data[i])</tt>.</param>
        /// <returns></returns>
        public static double HarmonicMean(int size, double sumOfInversions)
        {
            return size / sumOfInversions;
        }

        /// <summary>
        /// Incrementally maintains and updates minimum, maximum, sum and sum of squares of a data sequence.
        ///
        /// Assume we have already recorded some data sequence elements 
        /// and know their minimum, maximum, sum and sum of squares.
        /// Assume further, we are to record some more elements 
        /// and to derive updated values of minimum, maximum, sum and sum of squares.
        /// <p>
        /// This method computes those updated values without needing to know the already recorded elements.
        /// This is interesting for interactive online monitoring and/or applications that cannot keep the entire huge data sequence in memory.
        /// <p>
        /// <br>Definition of sumOfSquares: <tt>sumOfSquares(n) = Sum ( data[i]/// data[i] )</tt>.
        /// 
        /// inOut the old values in the following format:
        /// <ul>
        /// <li><tt>inOut[0]</tt> is the old minimum.
        /// <li><tt>inOut[1]</tt> is the old maximum.
        /// <li><tt>inOut[2]</tt> is the old sum.
        /// <li><tt>inOut[3]</tt> is the old sum of squares.
        /// </ul>
        /// If no data sequence elements have so far been recorded set the values as follows 
        /// <ul>
        /// <li><tt>inOut[0] = Double.POSITIVE_INFINITY</tt> as the old minimum.
        /// <li><tt>inOut[1] = Double.NEGATIVE_INFINITY</tt> as the old maximum.
        /// <li><tt>inOut[2] = 0.0</tt> as the old sum.
        /// <li><tt>inOut[3] = 0.0</tt> as the old sum of squares.
        /// </ul>
        /// 
        /// </summary>
        /// <param name="data">the additional elements to be incorporated into min, max, etc.</param>
        /// <param name="from">the index of the first element within <tt>data</tt> to consider.</param>
        /// <param name="to">the index of the last element within <tt>data</tt> to consider.</param>
        /// <param name="inOut">inOut the old values.  output the updated values filled into the <tt>inOut</tt> array.</param>
        public static void IncrementalUpdate(DoubleArrayList data, int from, int to, ref double[] inOut)
        {
            CheckRangeFromTo(from, to, data.Size);

            // read current values
            double min = inOut[0];
            double max = inOut[1];
            double sum = inOut[2];
            double sumSquares = inOut[3];

            double[] elements = data.ToArray();

            for (; from <= to; from++)
            {
                double element = elements[from];
                sum += element;
                sumSquares += element * element;
                if (element < min) min = element;
                if (element > max) max = element;

                /*
                double oldDeviation = element - mean;
                mean += oldDeviation / (N+1);
                sumSquaredDeviations += (element-mean)*oldDeviation; // cool, huh?
                */

                /*
                double oldMean = mean;
                mean += (element - mean)/(N+1);
                if (N > 0) {
                    sumSquaredDeviations += (element-mean)*(element-oldMean); // cool, huh?
                }
                */

            }

            // store new values
            inOut[0] = min;
            inOut[1] = max;
            inOut[2] = sum;
            inOut[3] = sumSquares;

            // At this point of return the following postcondition holds:
            // data.Size-from elements have been consumed by this call.
        }

        /// <summary>
        /// Incrementally maintains and updates various sums of powers of the form <tt>Sum(data[i]<sup>k</sup>)</tt>.
        ///
        /// Assume we have already recorded some data sequence elements <tt>data[i]</tt>
        /// and know the values of <tt>Sum(data[i]<sup>from</sup>), Sum(data[i]<sup>from+1</sup>), ..., Sum(data[i]<sup>to</sup>)</tt>.
        /// Assume further, we are to record some more elements 
        /// and to derive updated values of these sums.
        /// <p>
        /// This method computes those updated values without needing to know the already recorded elements.
        /// This is interesting for interactive online monitoring and/or applications that cannot keep the entire huge data sequence in memory.
        /// For example, the incremental computation of moments is based upon such sums of powers:
        /// <p>
        /// The moment of <tt>k</tt>-th order with constant <tt>c</tt> of a data sequence,
        /// is given by <tt>Sum( (data[i]-c)<sup>k</sup> ) / data.size()</tt>.
        /// It can incrementally be computed by using the equivalent formula
        /// <p>
        /// <tt>moment(k,c) = m(k,c) / data.size()</tt> where
        /// <br><tt>m(k,c) = Sum( -1<sup>i</sup> * b(k,i) * c<sup>i</sup> * sumOfPowers(k-i))</tt> for <tt>i = 0 .. k</tt> and
        /// <br><tt>b(k,i) = </tt><see cref="Cern.Jet.Math.Arithmetic.Binomial(long, long)"/> and
        /// <br><tt>sumOfPowers(k) = Sum( data[i]<sup>k</sup> )</tt>.
        /// <p>
        /// inOut the old values of the sums in the following format:
        /// <ul>
        /// <li><tt>sumOfPowers[0]</tt> is the old <tt>Sum(data[i]<sup>fromSumIndex</sup>)</tt>.
        /// <li><tt>sumOfPowers[1]</tt> is the old <tt>Sum(data[i]<sup>fromSumIndex+1</sup>)</tt>.
        /// <li>...
        /// <li><tt>sumOfPowers[toSumIndex-fromSumIndex]</tt> is the old <tt>Sum(data[i]<sup>toSumIndex</sup>)</tt>.
        /// </ul>
        /// If no data sequence elements have so far been recorded set all old values of the sums to <tt>0.0</tt>.
        /// </summary>
        /// <param name="data">the additional elements to be incorporated into min, max, etc.</param>
        /// <param name="from">the index of the first element within <tt>data</tt> to consider.</param>
        /// <param name="to">the index of the last element within <tt>data</tt> to consider.</param>
        /// <param name="fromSumIndex"></param>
        /// <param name="toSumIndex"></param>
        /// <param name="sumOfPowers">the old values of the sums, and returns the updated values filled into the <tt>sumOfPowers</tt> array.</param>
        public static void IncrementalUpdateSumsOfPowers(DoubleArrayList data, int from, int to, int fromSumIndex, int toSumIndex, ref double[] sumOfPowers)
        {
            int size = data.Size;
            int lastIndex = toSumIndex - fromSumIndex;
            if (from > size || lastIndex + 1 > sumOfPowers.Length) throw new ArgumentException();
            double[] elements;

            // optimized for common parameters
            if (fromSumIndex == 1)
            { // handle quicker
                if (toSumIndex == 2)
                {
                    elements = data.ToArray();
                    double sum = sumOfPowers[0];
                    double sumSquares = sumOfPowers[1];
                    for (int i = from - 1; ++i <= to;)
                    {
                        double element = elements[i];
                        sum += element;
                        sumSquares += element * element;
                        //if (element < min) min = element;
                        //else if (element > max) max = element;
                    }
                    sumOfPowers[0] += sum;
                    sumOfPowers[1] += sumSquares;
                    return;
                }
                else if (toSumIndex == 3)
                {
                    elements = data.ToArray();
                    double sum = sumOfPowers[0];
                    double sumSquares = sumOfPowers[1];
                    double sum_xxx = sumOfPowers[2];
                    for (int i = from - 1; ++i <= to;)
                    {
                        double element = elements[i];
                        sum += element;
                        sumSquares += element * element;
                        sum_xxx += element * element * element;
                        //if (element < min) min = element;
                        //else if (element > max) max = element;
                    }
                    sumOfPowers[0] += sum;
                    sumOfPowers[1] += sumSquares;
                    sumOfPowers[2] += sum_xxx;
                    return;
                }
                else if (toSumIndex == 4)
                { // handle quicker
                    elements = data.ToArray();
                    double sum = sumOfPowers[0];
                    double sumSquares = sumOfPowers[1];
                    double sum_xxx = sumOfPowers[2];
                    double sum_xxxx = sumOfPowers[3];
                    for (int i = from - 1; ++i <= to;)
                    {
                        double element = elements[i];
                        sum += element;
                        sumSquares += element * element;
                        sum_xxx += element * element * element;
                        sum_xxxx += element * element * element * element;
                        //if (element < min) min = element;
                        //else if (element > max) max = element;
                    }
                    sumOfPowers[0] += sum;
                    sumOfPowers[1] += sumSquares;
                    sumOfPowers[2] += sum_xxx;
                    sumOfPowers[3] += sum_xxxx;
                    return;
                }
            }

            if (fromSumIndex == toSumIndex || (fromSumIndex >= -1 && toSumIndex <= 5))
            { // handle quicker
                for (int i = fromSumIndex; i <= toSumIndex; i++)
                {
                    sumOfPowers[i - fromSumIndex] += SumOfPowerDeviations(data, i, 0.0, from, to);
                }
                return;
            }


            // now the most general case:
            // optimized for maximum speed, but still not quite quick
            elements = data.ToArray();

            for (int i = from - 1; ++i <= to;)
            {
                double element = elements[i];
                double pow = System.Math.Pow(element, fromSumIndex);

                int j = 0;
                for (int m = lastIndex; --m >= 0;)
                {
                    sumOfPowers[j++] += pow;
                    pow *= element;
                }
                sumOfPowers[j] += pow;
            }

            // At this point of return the following postcondition holds:
            // data.Size-fromIndex elements have been consumed by this call.
        }

        /// <summary>
        /// Incrementally maintains and updates sum and sum of squares of a <i>weighted</i> data sequence.
        ///
        /// Assume we have already recorded some data sequence elements 
        /// and know their sum and sum of squares.
        /// Assume further, we are to record some more elements 
        /// and to derive updated values of sum and sum of squares.
        /// <p>
        /// This method computes those updated values without needing to know the already recorded elements.
        /// This is interesting for interactive online monitoring and/or applications that cannot keep the entire huge data sequence in memory.
        /// <p>
        /// <br>Definition of sum: <tt>sum = Sum ( data[i]/// weights[i] )</tt>.
        /// <br>Definition of sumOfSquares: <tt>sumOfSquares = Sum ( data[i]/// data[i]/// weights[i])</tt>.
        /// 
        /// The inOut parameter values in the following format:
        /// <ul>
        /// <li><tt>inOut[0]</tt> is the old sum.
        /// <li><tt>inOut[1]</tt> is the old sum of squares.
        /// </ul>
        /// If no data sequence elements have so far been recorded set the values as follows 
        /// <ul>
        /// <li><tt>inOut[0] = 0.0</tt> as the old sum.
        /// <li><tt>inOut[1] = 0.0</tt> as the old sum of squares.
        /// </ul>
        /// </summary>
        /// <param name="data">the additional elements to be incorporated into min, max, etc.</param>
        /// <param name="weights">the weight of each element within <tt>data</tt>.</param>
        /// <param name="from">the index of the first element within <tt>data</tt> (and <tt>weights</tt>) to consider.</param>
        /// <param name="to">the index of the last element within <tt>data</tt> (and <tt>weights</tt>) to consider.  The method incorporates elements <tt>data[from], ..., data[to]</tt>.</param>
        /// <param name="inOut">input the old values, and return the updated values filled into the <tt>inOut</tt> array.</param>
        public static void IncrementalWeightedUpdate(DoubleArrayList data, DoubleArrayList weights, int from, int to, double[] inOut)
        {
            int dataSize = data.Size;
            CheckRangeFromTo(from, to, dataSize);
            if (dataSize != weights.Size) throw new ArgumentException("from=" + from + ", to=" + to + ", data.Size=" + dataSize + ", weights.Size=" + weights.Size);

            // read current values
            double sum = inOut[0];
            double sumOfSquares = inOut[1];

            double[] elements = data.ToArray();
            double[] w = weights.ToArray();

            for (int i = from - 1; ++i <= to;)
            {
                double element = elements[i];
                double weight = w[i];
                double prod = element * weight;

                sum += prod;
                sumOfSquares += element * prod;
            }

            // store new values
            inOut[0] = sum;
            inOut[1] = sumOfSquares;

            // At this point of return the following postcondition holds:
            // data.Size-from elements have been consumed by this call.
        }

        /// <summary>
        /// Returns the kurtosis (aka excess) of a data sequence.
        /// </summary>
        /// <param name="moment4">the fourth central moment, which is <tt>moment(data,4,mean)</tt>.</param>
        /// <param name="standardDeviation">the standardDeviation.</param>
        /// <returns></returns>
        public static double Kurtosis(double moment4, double standardDeviation)
        {
            return -3 + moment4 / (standardDeviation * standardDeviation * standardDeviation * standardDeviation);
        }

        /// <summary>
        /// Returns the kurtosis (aka excess) of a data sequence, which is <tt>-3 + moment(data,4,mean) / standardDeviation<sup>4</sup></tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        /// <returns></returns>
        public static double Kurtosis(DoubleArrayList data, double mean, double standardDeviation)
        {
            return Kurtosis(Moment(data, 4, mean), standardDeviation);
        }

        /// <summary>
        /// Returns the lag-1 autocorrelation of a dataset; 
        /// Note that this method has semantics different from <tt>autoCorrelation(..., 1)</tt>;
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double Lag1(DoubleArrayList data, double mean)
        {
            int size = data.Size;
            double[] elements = data.ToArray();
            double r1;
            double q = 0;
            double v = (elements[0] - mean) * (elements[0] - mean);

            for (int i = 1; i < size; i++)
            {
                double delta0 = (elements[i - 1] - mean);
                double delta1 = (elements[i] - mean);
                q += (delta0 * delta1 - q) / (i + 1);
                v += (delta1 * delta1 - v) / (i + 1);
            }

            r1 = q / v;
            return r1;
        }

        /// <summary>
        /// Returns the largest member of a data sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double Max(DoubleArrayList data)
        {
            int size = data.Size;
            if (size == 0) throw new ArgumentException();

            double[] elements = data.ToArray();
            double max = elements[size - 1];
            for (int i = size - 1; --i >= 0;)
            {
                if (elements[i] > max) max = elements[i];
            }

            return max;
        }

        /// <summary>
        /// Returns the arithmetic mean of a data sequence; 
        /// That is <tt>Sum( data[i] ) / data.size()</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double Mean(DoubleArrayList data)
        {
            return Sum(data) / data.Size;
        }

        /// <summary>
        /// Returns the mean deviation of a dataset.
        /// That is <tt>Sum (Math.abs(data[i]-mean)) / data.size())</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double MeanDeviation(DoubleArrayList data, double mean)
        {
            double[] elements = data.ToArray();
            int size = data.Size;
            double sum = 0;
            for (int i = size; --i >= 0;) sum += System.Math.Abs(elements[i] - mean);
            return sum / size;
        }

        /// <summary>
        /// Returns the median of a sorted data sequence.
        /// </summary>
        /// <param name="sortedData">the data sequence; <b>must be sorted ascending</b>.</param>
        /// <returns></returns>
        public static double Median(DoubleArrayList sortedData)
        {
            return Quantile(sortedData, 0.5);
            /*
            double[] sortedElements = sortedData.ToArray();
            int n = sortedData.Size;
            int lhs = (n - 1) / 2 ;
            int rhs = n / 2 ;

            if (n == 0) return 0.0 ;

            double median;
            if (lhs == rhs) median = sortedElements[lhs] ;
            else median = (sortedElements[lhs] + sortedElements[rhs])/2.0 ;

            return median;
            */
        }

        /// <summary>
        /// Returns the smallest member of a data sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double Min(DoubleArrayList data)
        {
            int size = data.Size;
            if (size == 0) throw new ArgumentException();

            double[] elements = data.ToArray();
            double min = elements[size - 1];
            for (int i = size - 1; --i >= 0;)
            {
                if (elements[i] < min) min = elements[i];
            }

            return min;
        }

        /// <summary>
        /// Returns the moment of <tt>k</tt>-th order with constant <tt>c</tt> of a data sequence,
        /// which is <tt>Sum( (data[i]-c)<sup>k</sup> ) / data.size()</tt>.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="c"></param>
        /// <param name="size">the number of elements of the data sequence.</param>
        /// <param name="sumOfPowers"><tt>sumOfPowers[m] == Sum( data[i]<sup>m</sup>) )</tt> for <tt>m = 0,1,..,k</tt> as returned by method <see cref="IncrementalUpdateSumsOfPowers(List{double}, int, int, int, int, ref double[])"/>.  In particular there must hold <tt>sumOfPowers.length == k+1</tt>.</param>
        /// <returns></returns>
        public static double Moment(int k, double c, int size, double[] sumOfPowers)
        {
            double sum = 0;
            int sign = 1;
            for (int i = 0; i <= k; i++)
            {
                double y;
                if (i == 0) y = 1;
                else if (i == 1) y = c;
                else if (i == 2) y = c * c;
                else if (i == 3) y = c * c * c;
                else y = System.Math.Pow(c, i);
                //sum += sign * 
                sum += sign * Cern.Jet.Math.Arithmetic.Binomial(k, i) * y * sumOfPowers[k - i];
                sign = -sign;
            }
            /*
            for (int i=0; i<=k; i++) {
                sum += sign * cern.jet.math.Arithmetic.binomial(k,i) * System.Math.Pow(c, i) * sumOfPowers[k-i];
                sign = -sign;
            }
            */
            return sum / size;
        }

        /// <summary>
        /// Returns the moment of <tt>k</tt>-th order with constant <tt>c</tt> of a data sequence,
        /// which is <tt>Sum( (data[i]-c)<sup>k</sup> ) / data.size()</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double Moment(DoubleArrayList data, int k, double c)
        {
            return SumOfPowerDeviations(data, k, c) / data.Size;
        }

        /// <summary>
        /// Returns the pooled mean of two data sequences.
        /// That is <tt>(size1/// mean1 + size2/// mean2) / (size1 + size2)</tt>.
        /// </summary>
        /// <param name="size1">the number of elements in data sequence 1.</param>
        /// <param name="mean1">the mean of data sequence 1.</param>
        /// <param name="size2">the number of elements in data sequence 2.</param>
        /// <param name="mean2">the mean of data sequence 2.</param>
        /// <returns></returns>
        public static double PooledMean(int size1, double mean1, int size2, double mean2)
        {
            return (size1 * mean1 + size2 * mean2) / (size1 + size2);
        }

        /// <summary>
        /// Returns the pooled variance of two data sequences.
        /// That is <tt>(size1/// variance1 + size2/// variance2) / (size1 + size2)</tt>;
        /// </summary>
        /// <param name="size1">the number of elements in data sequence 1.</param>
        /// <param name="variance1">the variance of data sequence 1.</param>
        /// <param name="size2">the number of elements in data sequence 2.</param>
        /// <param name="variance2">the variance of data sequence 2.</param>
        /// <returns></returns>
        public static double PooledVariance(int size1, double variance1, int size2, double variance2)
        {
            return (size1 * variance1 + size2 * variance2) / (size1 + size2);
        }

        /// <summary>
        /// Returns the product, which is <tt>Prod( data[i] )</tt>.
        /// In other words: <tt>data[0]*data[1]*...*data[data.size()-1]</tt>.
        /// This method uses the equivalent definition:
        /// <tt>prod = pow( exp( Sum( Log(x[i]) ) / size(), size())</tt>.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sumOfLogarithms"></param>
        /// <returns></returns>
        public static double Product(int size, double sumOfLogarithms)
        {
            return System.Math.Pow(System.Math.Exp(sumOfLogarithms / size), size);
        }

        /// <summary>
        /// Returns the product of a data sequence, which is <tt>Prod( data[i] )</tt>.
        /// In other words: <tt>data[0]*data[1]*...*data[data.size()-1]</tt>.
        /// Note that you may easily get numeric overflows.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double Product(DoubleArrayList data)
        {
            int size = data.Size;
            double[] elements = data.ToArray();

            double product = 1;
            for (int i = size; --i >= 0;) product *= elements[i];

            return product;
        }

        /// <summary>
        /// Returns the <tt>phi-</tt>quantile; that is, an element <tt>elem</tt> for which holds that <tt>phi</tt> percent of data elements are less than <tt>elem</tt>.
        /// The quantile need not necessarily be contained in the data sequence, it can be a linear interpolation.
        /// </summary>
        /// <param name="sortedData">the data sequence; <b>must be sorted ascending</b>.</param>
        /// <param name="phi">the percentage; must satisfy <tt>0 &lt;= phi &lt;= 1</tt>.</param>
        /// <returns></returns>
        public static double Quantile(DoubleArrayList sortedData, double phi)
        {
            double[] sortedElements = sortedData.ToArray();
            int n = sortedData.Size;

            double index = phi * (n - 1);
            int lhs = (int)index;
            double delta = index - lhs;
            double result;

            if (n == 0) return 0.0;

            if (lhs == n - 1)
            {
                result = sortedElements[lhs];
            }
            else
            {
                result = (1 - delta) * sortedElements[lhs] + delta * sortedElements[lhs + 1];
            }

            return result;
        }

        /// <summary>
        /// Returns how many percent of the elements contained in the receiver are <tt>&lt;= element</tt>.
        /// Does linear interpolation if the element is not contained but lies in between two contained elements.
        /// </summary>
        /// <param name="sortedList">the list to be searched (must be sorted ascending).</param>
        /// <param name="element">the element to search for.</param>
        /// <returns>the percentage <tt>phi</tt> of elements <tt>&lt;= element</tt> (<tt>0.0 &lt;= phi &lt;= 1.0)</tt>.</returns>
        public static double QuantileInverse(DoubleArrayList sortedList, double element)
        {
            return RankInterpolated(sortedList, element) / sortedList.Size;
        }

        /// <summary>
        /// Returns the quantiles of the specified percentages.
        /// The quantiles need not necessarily be contained in the data sequence, it can be a linear interpolation.
        /// </summary>
        /// <param name="sortedData">the data sequence; <b>must be sorted ascending</b>.</param>
        /// <param name="percentages">the percentages for which quantiles are to be computed.  Each percentage must be in the interval <tt>[0.0,1.0]</tt>.</param>
        /// <returns>the quantiles.</returns>
        public static DoubleArrayList Quantiles(DoubleArrayList sortedData, DoubleArrayList percentages)
        {
            int s = percentages.Size;
            DoubleArrayList quantiles = new DoubleArrayList(s);

            for (int i = 0; i < s; i++)
            {
                quantiles.Add(Quantile(sortedData, percentages[i]));
            }

            return quantiles;
        }

        /// <summary>
        /// Returns the linearly interpolated number of elements in a list less or equal to a given element.
        /// The rank is the number of elements &lt;= element.
        /// Ranks are of the form <tt>{0, 1, 2,..., sortedList.size()}</tt>.
        /// If no element is &lt;= element, then the rank is zero.
        /// If the element lies in between two contained elements, then linear interpolation is used and a non integer value is returned.
        /// </summary>
        /// <param name="sortedList">the list to be searched (must be sorted ascending).</param>
        /// <param name="element">the element to search for.</param>
        /// <returns>the rank of the element.</returns>
        public static double RankInterpolated(DoubleArrayList sortedList, double element)
        {
            int index = sortedList.BinarySearch(element);

            if (index >= 0)
            { // element found
              // skip to the right over multiple occurances of element.
                int to1 = index + 1;
                int s = sortedList.Size;
                while (to1 < s && sortedList[to1] == element) to1++;
                return to1;
            }

            // element not found
            int insertionPoint = -index - 1;
            if (insertionPoint == 0 || insertionPoint == sortedList.Size) return insertionPoint;

            double from = sortedList[insertionPoint - 1];
            double to = sortedList[insertionPoint];
            double delta = (element - from) / (to - from); //linear interpolation
            return insertionPoint + delta;
        }

        /// <summary>
        /// Returns the RMS (Root-Mean-Square) of a data sequence.
        /// That is <tt>Math.sqrt(Sum( data[i]*data[i] ) / data.size())</tt>.
        /// The RMS of data sequence is the square-root of the mean of the squares of the elements in the data sequence.
        /// It is a measure of the average "size" of the elements of a data sequence.
        /// </summary>
        /// <param name="size">the number of elements in the data sequence.</param>
        /// <param name="sumOfSquares"><tt>sumOfSquares(data) == Sum( data[i]*data[i] )</tt> of the data sequence.</param>
        /// <returns></returns>
        public static double Rms(int size, double sumOfSquares)
        {
            return System.Math.Sqrt(sumOfSquares / size);
        }

        /// <summary>
        /// Returns the sample kurtosis (aka excess) of a data sequence.
        ///
        /// Ref: R.R. Sokal, F.J. Rohlf, Biometry: the principles and practice of statistics
        /// in biological research (W.H. Freeman and Company, New York, 1998, 3rd edition)
        /// p. 114-115.
        /// </summary>
        /// <param name="size">the number of elements of the data sequence.</param>
        /// <param name="moment4">the fourth central moment, which is <tt>moment(data,4,mean)</tt>.</param>
        /// <param name="sampleVariance">the <b>sample variance</b>.</param>
        /// <returns></returns>
        public static double SampleKurtosis(int size, double moment4, double sampleVariance)
        {
            int n = size;
            double s2 = sampleVariance; // (y-ymean)^2/(n-1)
            double m4 = moment4 * n;    // (y-ymean)^4
            return m4 * n * (n + 1) / ((n - 1) * (n - 2) * (n - 3) * s2 * s2)
                 - 3.0 * (n - 1) * (n - 1) / ((n - 2) * (n - 3));
        }

        /// <summary>
        /// Returns the sample kurtosis (aka excess) of a data sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mean"></param>
        /// <param name="sampleVariance"></param>
        /// <returns></returns>
        public static double SampleKurtosis(DoubleArrayList data, double mean, double sampleVariance)
        {
            return SampleKurtosis(data.Size, Moment(data, 4, mean), sampleVariance);
        }

        /// <summary>
        /// Return the standard error of the sample kurtosis.
        ///
        /// Ref: R.R. Sokal, F.J. Rohlf, Biometry: the principles and practice of statistics
        /// in biological research (W.H. Freeman and Company, New York, 1998, 3rd edition)
        /// p. 138.
        /// </summary>
        /// <param name="size">the number of elements of the data sequence.</param>
        /// <returns></returns>
        public static double SampleKurtosisStandardError(int size)
        {
            int n = size;
            return System.Math.Sqrt(24.0 * n * (n - 1) * (n - 1) / ((n - 3) * (n - 2) * (n + 3) * (n + 5)));
        }

        /// <summary>
        /// Returns the sample skew of a data sequence.
        ///
        /// Ref: R.R. Sokal, F.J. Rohlf, Biometry: the principles and practice of statistics
        /// in biological research (W.H. Freeman and Company, New York, 1998, 3rd edition)
        /// p. 114-115.
        /// </summary>
        /// <param name="size">the number of elements of the data sequence.</param>
        /// <param name="moment3">the third central moment, which is <tt>moment(data,3,mean)</tt>.</param>
        /// <param name="sampleVariance">the <b>sample variance</b>.</param>
        /// <returns></returns>
        public static double SampleSkew(int size, double moment3, double sampleVariance)
        {
            int n = size;
            double s = System.Math.Sqrt(sampleVariance); // sqrt( (y-ymean)^2/(n-1) )
            double m3 = moment3 * n;    // (y-ymean)^3
            return n * m3 / ((n - 1) * (n - 2) * s * s * s);
        }

        /// <summary>
        /// Returns the sample skew of a data sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mean"></param>
        /// <param name="sampleVariance"></param>
        /// <returns></returns>
        public static double SampleSkew(DoubleArrayList data, double mean, double sampleVariance)
        {
            return SampleSkew(data.Size, Moment(data, 3, mean), sampleVariance);
        }

        /// <summary>
        /// Return the standard error of the sample skew.
        ///
        /// Ref: R.R. Sokal, F.J. Rohlf, Biometry: the principles and practice of statistics
        /// in biological research (W.H. Freeman and Company, New York, 1998, 3rd edition)
        /// p. 138.
        /// </summary>
        /// <param name="size">the number of elements of the data sequence.</param>
        /// <returns></returns>
        public static double SampleSkewStandardError(int size)
        {
            int n = size;
            return System.Math.Sqrt(6.0 * n * (n - 1) / ((n - 2) * (n + 1) * (n + 3)));
        }

        /// <summary>
        /// Returns the sample standard deviation.
        ///
        /// Ref: R.R. Sokal, F.J. Rohlf, Biometry: the principles and practice of statistics
        /// in biological research (W.H. Freeman and Company, New York, 1998, 3rd edition)
        /// p. 53.
        /// </summary>
        /// <param name="size">the number of elements of the data sequence.</param>
        /// <param name="sampleVariance">the <b>sample variance</b>.</param>
        /// <returns></returns>
        public static double SampleStandardDeviation(int size, double sampleVariance)
        {
            double s, Cn;
            int n = size;

            // The standard deviation calculated as the sqrt of the variance underestimates
            // the unbiased standard deviation.
            s = System.Math.Sqrt(sampleVariance);
            // It needs to be multiplied by this correction factor.
            if (n > 30)
            {
                Cn = 1 + 1.0 / (4 * (n - 1)); // Cn = 1+1/(4*(n-1));
            }
            else
            {
                Cn = System.Math.Sqrt((n - 1) * 0.5) * Gamma.GetGamma((n - 1) * 0.5) / Gamma.GetGamma(n * 0.5);
            }
            return Cn * s;
        }

        /// <summary>
        /// Returns the sample variance of a data sequence.
        /// That is <tt>(sumOfSquares - mean*sum) / (size - 1)</tt> with <tt>mean = sum/size</tt>.
        /// </summary>
        /// <param name="size">the number of elements of the data sequence. </param>
        /// <param name="sum"><tt>== Sum( data[i] )</tt>.</param>
        /// <param name="sumOfSquares"><tt>== Sum( data[i]*data[i] )</tt>.</param>
        /// <returns></returns>
        public static double SampleVariance(int size, double sum, double sumOfSquares)
        {
            double mean = sum / size;
            return (sumOfSquares - mean * sum) / (size - 1);
        }

        /// <summary>
        /// Returns the sample variance of a data sequence.
        /// That is <tt>Sum ( (data[i]-mean)^2 ) / (data.size()-1)</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double SampleVariance(DoubleArrayList data, double mean)
        {
            double[] elements = data.ToArray();
            int size = data.Size;
            double sum = 0;
            // find the sum of the squares 
            for (int i = size; --i >= 0;)
            {
                double delta = elements[i] - mean;
                sum += delta * delta;
            }

            return sum / (size - 1);
        }

        /// <summary>
        /// Returns the sample weighted variance of a data sequence.
        /// That is <tt>(sumOfSquaredProducts  -  sumOfProducts/// sumOfProducts / sumOfWeights) / (sumOfWeights - 1)</tt>.
        /// </summary>
        /// <param name="sumOfWeights"><tt>== Sum( weights[i] )</tt>. </param>
        /// <param name="sumOfProducts"><tt>== Sum( data[i]/// weights[i] )</tt>.</param>
        /// <param name="sumOfSquaredProducts"><tt>== Sum( data[i]/// data[i]/// weights[i] )</tt>.</param>
        /// <returns></returns>
        public static double SampleWeightedVariance(double sumOfWeights, double sumOfProducts, double sumOfSquaredProducts)
        {
            return (sumOfSquaredProducts - sumOfProducts * sumOfProducts / sumOfWeights) / (sumOfWeights - 1);
        }

        /// <summary>
        /// Returns the skew of a data sequence.
        /// </summary>
        /// <param name="moment3">the third central moment, which is <tt>moment(data,3,mean)</tt>.</param>
        /// <param name="standardDeviation">the standardDeviation.</param>
        /// <returns></returns>
        public static double Skew(double moment3, double standardDeviation)
        {
            return moment3 / (standardDeviation * standardDeviation * standardDeviation);
        }

        /// <summary>
        /// Returns the skew of a data sequence, which is <tt>moment(data,3,mean) / standardDeviation<sup>3</sup></tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        /// <returns></returns>
        public static double Skew(DoubleArrayList data, double mean, double standardDeviation)
        {
            return Skew(Moment(data, 3, mean), standardDeviation);
        }

        #endregion

        #region Local Protected Methods

        /// <summary>
        /// Checks if the given range is within the contained array's bounds.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="theSize"></param>
        protected static void CheckRangeFromTo(int from, int to, int theSize)
        {
            if (to == from - 1) return;
            if (from < 0 || from > to || to >= theSize)
                throw new IndexOutOfRangeException("from: " + from + ", to: " + to + ", size=" + theSize);
        }

        /// <summary>
        /// Splits (partitions) a list into sublists such that each sublist contains the elements with a given range.
        /// <tt>splitters=(a,b,c,...,y,z)</tt> defines the ranges <tt>[-inf,a), [a,b), [b,c), ..., [y,z), [z,inf]</tt>.
        /// <p><b>Examples:</b><br>
        /// <ul>
        /// <tt>data = (1,2,3,4,5,8,8,8,10,11)</tt>.
        /// <br><tt>splitters=(2,8)</tt> yields 3 bins: <tt>(1), (2,3,4,5) (8,8,8,10,11)</tt>.
        /// <br><tt>splitters=()</tt> yields 1 bin: <tt>(1,2,3,4,5,8,8,8,10,11)</tt>.
        /// <br><tt>splitters=(-5)</tt> yields 2 bins: <tt>(), (1,2,3,4,5,8,8,8,10,11)</tt>.
        /// <br><tt>splitters=(100)</tt> yields 2 bins: <tt>(1,2,3,4,5,8,8,8,10,11), ()</tt>.
        /// </ul>
        /// </summary>
        /// <param name="sortedList">the list to be partitioned (must be sorted ascending).</param>
        /// <param name="splitters">the points at which the list shall be partitioned (must be sorted ascending).</param>
        /// <returns>the sublists (an array with <tt>length == splitters.size() + 1</tt>.  Each sublist is returned sorted ascending.</returns>
        public static DoubleArrayList[] Split(DoubleArrayList sortedList, DoubleArrayList splitters)
        {
            // assertion: data is sorted ascending.
            // assertion: splitValues is sorted ascending.
            int noOfBins = splitters.Size + 1;

            DoubleArrayList[] bins = new DoubleArrayList[noOfBins];
            for (int j = noOfBins; --j >= 0;) bins[j] = new DoubleArrayList();

            int listSize = sortedList.Size;
            int nextStart = 0;
            int i = 0;
            while (nextStart < listSize && i < noOfBins - 1)
            {
                double splitValue = splitters[i];
                int index = sortedList.BinarySearch(splitValue);
                if (index < 0)
                { // splitValue not found
                    int insertionPosition = -index - 1;
                    //bins[i].addAllOfFromTo(sortedList, nextStart, insertionPosition - 1);
                    bins[i].AddAllOfFromTo(sortedList, nextStart, insertionPosition - 1);
                    nextStart = insertionPosition;
                }
                else
                { // splitValue found
                  // For multiple identical elements ("runs"), binarySearch does not define which of all valid indexes is returned.
                  // Thus, skip over to the first element of a run.
                    do
                    {
                        index--;
                    } while (index >= 0 && sortedList[index] == splitValue);

                    //bins[i].addAllOfFromTo(sortedList, nextStart, index);
                    bins[i].AddAllOfFromTo(sortedList, nextStart, index);
                    nextStart = index + 1;
                }
                i++;
            }

            // now fill the remainder
            //bins[noOfBins - 1].addAllOfFromTo(sortedList, nextStart, sortedList.Size - 1);
            bins[noOfBins - 1].AddAllOfFromTo(sortedList, nextStart, sortedList.Size - 1);

            return bins;
        }

        /// <summary>
        /// Returns the standard deviation from a variance.
        /// </summary>
        /// <param name="variance"></param>
        /// <returns></returns>
        public static double StandardDeviation(double variance)
        {
            return System.Math.Sqrt(variance);
        }

        /// <summary>
        /// Returns the standard error of a data sequence.
        /// That is <tt>Math.sqrt(variance/size)</tt>.
        /// </summary>
        /// <param name="size">the number of elements in the data sequence.</param>
        /// <param name="variance">the variance of the data sequence.</param>
        /// <returns></returns>
        public static double StandardError(int size, double variance)
        {
            return System.Math.Sqrt(variance / size);
        }

        /// <summary>
        /// Modifies a data sequence to be standardized.
        /// Changes each element <tt>data[i]</tt> as follows: <tt>data[i] = (data[i]-mean)/standardDeviation</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        public static void Standardize(DoubleArrayList data, double mean, double standardDeviation)
        {
            double[] elements = data.ToArray();
            for (int i = data.Size; --i >= 0;) elements[i] = (elements[i] - mean) / standardDeviation;
        }

        /// <summary>
        /// Returns the sum of a data sequence.
        /// That is <tt>Sum( data[i] )</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double Sum(DoubleArrayList data)
        {
            return SumOfPowerDeviations(data, 1, 0.0);
        }

        /// <summary>
        /// Returns the sum of inversions of a data sequence,
        /// which is <tt>Sum( 1.0 / data[i])</tt>.
        /// </summary>
        /// <param name="data">the data sequence.</param>
        /// <param name="from">the index of the first data element (inclusive).</param>
        /// <param name="to">the index of the last data element (inclusive).</param>
        /// <returns></returns>
        public static double SumOfInversions(DoubleArrayList data, int from, int to)
        {
            return SumOfPowerDeviations(data, -1, 0.0, from, to);
        }

        /// <summary>
        /// Returns the sum of logarithms of a data sequence, which is <tt>Sum( Log(data[i])</tt>.
        /// </summary>
        /// <param name="data">the data sequence.</param>
        /// <param name="from">the index of the first data element (inclusive).</param>
        /// <param name="to">the index of the last data element (inclusive).</param>
        /// <returns></returns>
        public static double SumOfLogarithms(DoubleArrayList data, int from, int to)
        {
            double[] elements = data.ToArray();
            double logsum = 0;
            for (int i = from - 1; ++i <= to;) logsum += System.Math.Log(elements[i]);
            return logsum;
        }

        /// <summary>
        /// Returns <tt>Sum( (data[i]-c)<sup>k</sup> )</tt>; optimized for common parameters like <tt>c == 0.0</tt> and/or <tt>k == -2 .. 4</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double SumOfPowerDeviations(DoubleArrayList data, int k, double c)
        {
            return SumOfPowerDeviations(data, k, c, 0, data.Size - 1);
        }

        /// <summary>
        /// Returns <tt>Sum( (data[i]-c)<sup>k</sup> )</tt> for all <tt>i = from .. to</tt>; optimized for common parameters like <tt>c == 0.0</tt> and/or <tt>k == -2 .. 5</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <param name="c"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double SumOfPowerDeviations(DoubleArrayList data, int k, double c, int from, int to)
        {
            double[] elements = data.ToArray();
            double sum = 0;
            double v;
            int i;
            switch (k)
            { // optimized for speed
                case -2:
                    if (c == 0.0) for (i = from - 1; ++i <= to;) { v = elements[i]; sum += 1 / (v * v); }
                    else for (i = from - 1; ++i <= to;) { v = elements[i] - c; sum += 1 / (v * v); }
                    break;
                case -1:
                    if (c == 0.0) for (i = from - 1; ++i <= to;) sum += 1 / (elements[i]);
                    else for (i = from - 1; ++i <= to;) sum += 1 / (elements[i] - c);
                    break;
                case 0:
                    sum += to - from + 1;
                    break;
                case 1:
                    if (c == 0.0) for (i = from - 1; ++i <= to;) sum += elements[i];
                    else for (i = from - 1; ++i <= to;) sum += elements[i] - c;
                    break;
                case 2:
                    if (c == 0.0) for (i = from - 1; ++i <= to;) { v = elements[i]; sum += v * v; }
                    else for (i = from - 1; ++i <= to;) { v = elements[i] - c; sum += v * v; }
                    break;
                case 3:
                    if (c == 0.0) for (i = from - 1; ++i <= to;) { v = elements[i]; sum += v * v * v; }
                    else for (i = from - 1; ++i <= to;) { v = elements[i] - c; sum += v * v * v; }
                    break;
                case 4:
                    if (c == 0.0) for (i = from - 1; ++i <= to;) { v = elements[i]; sum += v * v * v * v; }
                    else for (i = from - 1; ++i <= to;) { v = elements[i] - c; sum += v * v * v * v; }
                    break;
                case 5:
                    if (c == 0.0) for (i = from - 1; ++i <= to;) { v = elements[i]; sum += v * v * v * v * v; }
                    else for (i = from - 1; ++i <= to;) { v = elements[i] - c; sum += v * v * v * v * v; }
                    break;
                default:
                    for (i = from - 1; ++i <= to;) sum += System.Math.Pow(elements[i] - c, k);
                    break;
            }
            return sum;
        }

        /// <summary>
        /// Returns the sum of powers of a data sequence, which is <tt>Sum ( data[i]<sup>k</sup> )</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static double SumOfPowers(DoubleArrayList data, int k)
        {
            return SumOfPowerDeviations(data, k, 0);
        }

        /// <summary>
        /// Returns the sum of squared mean deviation of of a data sequence.
        /// That is <tt>variance/// (size-1) == Sum( (data[i] - mean)^2 )</tt>.
        /// </summary>
        /// <param name="size">the number of elements of the data sequence. </param>
        /// <param name="variance">the variance of the data sequence.</param>
        /// <returns></returns>
        public static double SumOfSquaredDeviations(int size, double variance)
        {
            return variance * (size - 1);
        }

        /// <summary>
        /// Returns the sum of squares of a data sequence.
        /// That is <tt>Sum ( data[i]*data[i] )</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double SumOfSquares(DoubleArrayList data)
        {
            return SumOfPowerDeviations(data, 2, 0.0);
        }

        /// <summary>
        /// Returns the trimmed mean of a sorted data sequence.
        /// </summary>
        /// <param name="sortedData">the data sequence; <b>must be sorted ascending</b>.</param>
        /// <param name="mean">the mean of the (full) sorted data sequence.</param>
        /// <param name="left">the number of leading elements to trim.</param>
        /// <param name="right">the number of trailing elements to trim.</param>
        /// <returns></returns>
        public static double TrimmedMean(DoubleArrayList sortedData, double mean, int left, int right)
        {
            int N = sortedData.Size;
            if (N == 0) throw new ArgumentException("Empty data.");
            if (left + right >= N) throw new ArgumentException("Not enough data.");

            double[] sortedElements = sortedData.ToArray();
            int N0 = N;
            for (int i = 0; i < left; ++i)
                mean += (mean - sortedElements[i]) / (--N);
            for (int i = 0; i < right; ++i)
                mean += (mean - sortedElements[N0 - 1 - i]) / (--N);
            return mean;
        }

        /// <summary>
        /// Returns the variance from a standard deviation.
        /// </summary>
        /// <param name="standardDeviation"></param>
        /// <returns></returns>
        public static double Variance(double standardDeviation)
        {
            return standardDeviation * standardDeviation;
        }

        /// <summary>
        /// Returns the variance of a data sequence.
        /// That is <tt>(sumOfSquares - mean*sum) / size</tt> with <tt>mean = sum/size</tt>.
        /// </summary>
        /// <param name="size">the number of elements of the data sequence. </param>
        /// <param name="sum"><tt>== Sum( data[i] )</tt>.</param>
        /// <param name="sumOfSquares"><tt>== Sum( data[i]*data[i] )</tt>.</param>
        /// <returns></returns>
        public static double Variance(int size, double sum, double sumOfSquares)
        {
            double mean = sum / size;
            return (sumOfSquares - mean * sum) / size;
        }

        /// <summary>
        /// Returns the weighted mean of a data sequence.
        /// That is <tt> Sum (data[i]/// weights[i]) / Sum ( weights[i] )</tt>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static double WeightedMean(DoubleArrayList data, DoubleArrayList weights)
        {
            int size = data.Size;
            if (size != weights.Size || size == 0) throw new ArgumentException();

            double[] elements = data.ToArray();
            double[] theWeights = weights.ToArray();
            double sum = 0.0;
            double weightsSum = 0.0;
            for (int i = size; --i >= 0;)
            {
                double w = theWeights[i];
                sum += elements[i] * w;
                weightsSum += w;
            }

            return sum / weightsSum;
        }

        /// <summary>
        /// Returns the weighted RMS (Root-Mean-Square) of a data sequence.
        /// That is <tt>Sum( data[i]/// data[i]/// weights[i]) / Sum( data[i]/// weights[i] )</tt>,
        /// or in other words <tt>sumOfProducts / sumOfSquaredProducts</tt>.
        /// </summary>
        /// <param name="sumOfProducts"><tt>== Sum( data[i]/// weights[i] )</tt>.</param>
        /// <param name="sumOfSquaredProducts"><tt>== Sum( data[i]/// data[i]/// weights[i] )</tt>.</param>
        /// <returns></returns>
        public static double WeightedRMS(double sumOfProducts, double sumOfSquaredProducts)
        {
            return sumOfProducts / sumOfSquaredProducts;
        }

        /// <summary>
        /// Returns the winsorized mean of a sorted data sequence.
        /// </summary>
        /// <param name="sortedData">the data sequence; <b>must be sorted ascending</b>.</param>
        /// <param name="mean">the mean of the (full) sorted data sequence.</param>
        /// <param name="left">the number of leading elements to trim.</param>
        /// <param name="right">the number of trailing elements to trim.</param>
        /// <returns></returns>
        public static double WinsorizedMean(DoubleArrayList sortedData, double mean, int left, int right)
        {
            int N = sortedData.Size;
            if (N == 0) throw new ArgumentException("Empty data.");
            if (left + right >= N) throw new ArgumentException("Not enough data.");

            double[] sortedElements = sortedData.ToArray();

            double leftElement = sortedElements[left];
            for (int i = 0; i < left; ++i)
                mean += (leftElement - sortedElements[i]) / N;

            double rightElement = sortedElements[N - 1 - right];
            for (int i = 0; i < right; ++i)
                mean += (rightElement - sortedElements[N - 1 - i]) / N;

            return mean;
        }
        #endregion


        #region Local Private Methods

        /// <summary>
        /// Both covariance versions yield the same results but the one above is faster 
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        private static double Covariance2(DoubleArrayList data1, DoubleArrayList data2)
        {
            int size = data1.Size;
            double mean1 = Descriptive.Mean(data1);
            double mean2 = Descriptive.Mean(data2);
            double covariance = 0.0D;
            for (int i = 0; i < size; i++)
            {
                double x = data1[i];
                double y = data2[i];

                covariance += (x - mean1) * (y - mean2);
            }

            return covariance / (double)(size - 1);
        }



        #endregion

    }
}
