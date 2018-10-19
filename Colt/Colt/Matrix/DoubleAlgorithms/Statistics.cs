// <copyright file="Statistics.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;
using Cern.Hep.Aida.Bin;
using Cern.Colt.Matrix.Implementation;
using F1 = Cern.Jet.Math.Functions.DoubleFunctions;
using F2 = Cern.Jet.Math.Functions.DoubleDoubleFunctions;

namespace Cern.Colt.Matrix.DoubleAlgorithms
{
    /// <summary>
    /// delegate that represents a function object: a function that takes 
    /// two argument vectors and returns a single value.
    /// </summary>
    /// <param name="x">the first argument vector passed to the function.</param>
    /// <param name="y">the second argument vector passed to the function.</param>
    /// <returns>the result of the function.</returns>
    public delegate double VectorVectorFunction(Cern.Colt.Matrix.DoubleMatrix1D x, Cern.Colt.Matrix.DoubleMatrix1D y);

    public static class Statistics
    {
        //private static Cern.Jet.Math.Functions.DoubleDoubleFunctions F;

        /// <summary>
        /// Euclidean distance function; <i>Sqrt(Sum( (x[i]-y[i])^2 ))</i>.
        /// </summary>
        /// <returns></returns>
        public static VectorVectorFunction EUCLID()
        {
            return (a, b) => System.Math.Sqrt(a.Aggregate(b, F2.Plus, F2.Chain(F1.Square, F2.Minus)));
        }

        /// <summary>
        /// Bray-Curtis distance function; <i>Sum( abs(x[i]-y[i]) )  /  Sum( x[i]+y[i] )</i>.
        /// </summary>
        /// <returns></returns>
        public static VectorVectorFunction BRAY_CURTIS()
        {
            return (a, b) => a.Aggregate(b, F2.Plus, F2.Chain(F1.Abs, F2.Minus)) / a.Aggregate(b, F2.Plus, F2.Plus);

        }

        /// <summary>
        /// Canberra distance function; <i>Sum( abs(x[i]-y[i]) / abs(x[i]+y[i]) )</i>.
        /// </summary>
        /// <returns></returns>
        public static VectorVectorFunction CANBERRA()
        {
            DoubleDoubleFunction fun()
            {
                return (a, b) => System.Math.Abs(a - b) / System.Math.Abs(a + b);
            }

            return (a, b) => a.Aggregate(b, F2.Plus, fun());
        }

        /// <summary>
        /// Maximum distance function; <i>Max( abs(x[i]-y[i]) )</i>.
        /// </summary>
        /// <returns></returns>
        public static VectorVectorFunction MAXIMUM()
        {
            return (a, b) => a.Aggregate(b, F2.Max, F2.Chain(F1.Abs, F2.Minus));
        }

        /// <summary>
        /// Manhattan distance function; <i>Sum( abs(x[i]-y[i]) )</i>.
        /// </summary>
        /// <returns></returns>
        public static VectorVectorFunction MANHATTAN()
        {
            return (a, b) => a.Aggregate(b, F2.Plus, F2.Chain(F1.Abs, F2.Minus));
        }

        /// <summary>
        /// Applies the given aggregation functions to each column and stores the results in a the result matrix.
        /// If matrix has shape <i>m x n</i>, then result must have shape <i>aggr.Length x n</i>.
        /// Tip: To do aggregations on rows use dice views (transpositions), as in <i>aggregate(matrix.viewDice(),aggr,result.viewDice())</i>.
        /// </summary>
        /// <param name="matrix">any matrix; a column holds the values of a given variable.</param>
        /// <param name="aggr">the aggregation functions to be applied to each column.</param>
        /// <param name="result">the matrix to hold the aggregation results.</param>
        /// <returns><i>result</i> (for convenience only).</returns>
        /// <see cref="Formatter"/>
        /// <see cref="Hep.Aida.Bin.BinFunction1D"/>
        /// <see cref="Hep.Aida.Bin.BinFunctions1D"/>
        public static DoubleMatrix2D Aggregate(DoubleMatrix2D matrix, Hep.Aida.Bin.BinFunction1D[] aggr, DoubleMatrix2D result)
        {
            DynamicBin1D bin = new DynamicBin1D();
            double[] elements = new double[matrix.Rows];
            List<Double> values = new List<Double>(elements);
            for (int column = matrix.Columns; --column >= 0;)
            {
                matrix.ViewColumn(column).ToArray(ref elements); // copy column into values
                bin.Clear();
                bin.AddAllOf(values);
                for (int i = aggr.Length; --i >= 0;)
                {
                    result[i, column] = aggr[i](bin);
                }
            }
            return result;
        }

        /// <summary>
        /// /// Fills all cell values of the given vector into a bin from which statistics measures can be retrieved efficiently.
        /// Cells values are copied.
        /// </summary>
        /// <param name="vector">the vector to analyze.</param>
        /// <returns>a bin holding the statistics measures of the vector.</returns>
        /// <example>
        /// Tip: Use <i>Console.WriteLine(bin(vector))</i> to print most measures computed by the bind Example:
        /// <table>
        /// <td class="PRE"> 
        /// <pre>
        /// Size: 20000
        /// Sum: 299858.02350278624
        /// SumOfSquares: 5399184.154095971
        /// Min: 0.8639113139711261
        /// Max: 59.75331890541892
        /// Mean: 14.992901175139313
        /// RMS: 16.43043540825375
        /// Variance: 45.17438077634358
        /// Standard deviation: 6.721188940681818
        /// Standard error: 0.04752598277592142
        /// Geometric mean: 13.516615397064466
        /// Product: Infinity
        /// Harmonic mean: 11.995174297952191
        /// Sum of inversions: 1667.337172700724
        /// Skew: 0.8922838940067878
        /// Kurtosis: 1.1915828121825598
        /// Sum of powers(3): 1.1345828465808412E8
        /// Sum of powers(4): 2.7251055344494686E9
        /// Sum of powers(5): 7.367125643433887E10
        /// Sum of powers(6): 2.215370909100143E12
        /// Moment(0,0): 1.0
        /// Moment(1,0): 14.992901175139313
        /// Moment(2,0): 269.95920770479853
        /// Moment(3,0): 5672.914232904206
        /// Moment(4,0): 136255.27672247344
        /// Moment(5,0): 3683562.8217169433
        /// Moment(6,0): 1.1076854545500715E8
        /// Moment(0,mean()): 1.0
        /// Moment(1,mean()): -2.0806734113421045E-14
        /// Moment(2,mean()): 45.172122057305664
        /// Moment(3,mean()): 270.92018671421
        /// Moment(4,mean()): 8553.8664869067
        /// Moment(5,mean()): 153357.41712233616
        /// Moment(6,mean()): 4273757.570142922
        /// 25%, 50% and 75% Quantiles: 10.030074811938091, 13.977982089912224,
        /// 18.86124362967137
        /// quantileInverse(mean): 0.559163335012079
        /// Distinct elements & frequencies not printed (too many).
        /// </pre>
        /// </td>
        /// </table>
        /// </example>
        public static DynamicBin1D Bin(DoubleMatrix1D vector)
        {
            DynamicBin1D bin = new DynamicBin1D();
            bin.AddAllOf(DoubleFactory1D.Dense.ToList(vector));
            return bin;
        }

        /// <summary>
        /// Modifies the given covariance matrix to be a correlation matrix (in-place).
        /// The correlation matrix is a square, symmetric matrix consisting of nothing but correlation coefficients.
        /// The rows and the columns represent the variables, the cells represent correlation coefficientsd 
        /// The diagonal cells (i.ed the correlation between a variable and itself) will equal 1, for the simple reason that the correlation coefficient of a variable with itself equals 1d 
        /// The correlation of two column vectors x and y is given by <i>corr(x,y) = cov(x,y) / (stdDev(x)*stdDev(y))</i> (Pearson's correlation coefficient).
        /// A correlation coefficient varies between -1 (for a perfect negative relationship) to +1 (for a perfect positive relationship)d 
        /// See the <A HREF="http://www.cquest.utoronto.ca/geog/ggr270y/notes/not05efg.html"> math definition</A>
        /// and <A HREF="http://www.stat.berkeley.edu/users/stark/SticiGui/Text/gloss.htm#correlation_coef"> another def</A>.
        /// Compares two column vectors at a timed Use dice views to compare two row vectors at a time.
        /// </summary>
        /// <param name="covariance">covariance a covariance matrix, as, for example, returned by method <see cref="Covariance(DoubleMatrix2D)"/>.</param>
        /// <returns>the modified covariance, now correlation matrix (for convenience only).</returns>
        public static DoubleMatrix2D Correlation(DoubleMatrix2D covariance)
        {
            for (int i = covariance.Columns; --i >= 0;)
            {
                for (int j = i; --j >= 0;)
                {
                    double stdDev1 = System.Math.Sqrt(covariance[i, i]);
                    double stdDev2 = System.Math.Sqrt(covariance[j, j]);
                    double cov = covariance[i, j];
                    double corr = cov / (stdDev1 * stdDev2);

                    covariance[i, j] = corr;
                    covariance[j, i] = corr; // symmetric
                }
            }
            for (int i = covariance.Columns; --i >= 0;) covariance[i, i] = 1;

            return covariance;
        }

        /// <summary>
         /// Constructs and returns the covariance matrix of the given matrix.
         /// The covariance matrix is a square, symmetric matrix consisting of nothing but covariance coefficientsd 
         /// The rows and the columns represent the variables, the cells represent covariance coefficientsd 
         /// The diagonal cells (i.ed the covariance between a variable and itself) will equal the variances.
         /// The covariance of two column vectors x and y is given by <i>cov(x,y) = (1/n) * Sum((x[i]-mean(x)) * (y[i]-mean(y)))</i>.
         /// See the <A HREF="http://www.cquest.utoronto.ca/geog/ggr270y/notes/not05efg.html"> math definition</A>.
         /// Compares two column vectors at a timed Use dice views to compare two row vectors at a time.
        /// </summary>
        /// <param name="matrix">any matrix; a column holds the values of a given variable.</param>
        /// <returns>the covariance matrix (<i>n x n, n=matrix.Columns</i>).</returns>
        public static DoubleMatrix2D Covariance(DoubleMatrix2D matrix)
        {
            int rows = matrix.Rows;
            int columns = matrix.Columns;
            DoubleMatrix2D covariance = new DenseDoubleMatrix2D(columns, columns);

            double[] sums = new double[columns];
            DoubleMatrix1D[] cols = new DoubleMatrix1D[columns];
            for (int i = columns; --i >= 0;)
            {
                cols[i] = matrix.ViewColumn(i);
                sums[i] = cols[i].ZSum();
            }

            for (int i = columns; --i >= 0;)
            {
                for (int j = i + 1; --j >= 0;)
                {
                    double sumOfProducts = cols[i].ZDotProduct(cols[j]);
                    double cov = (sumOfProducts - sums[i] * sums[j] / rows) / rows;
                    covariance[i, j] = cov;
                    covariance[j, i] = cov; // symmetric
                }
            }
            return covariance;
        }

        /// <summary>
        /// 2-d OLAP cube operator; Fills all cells of the given vectors into the given histogram.
        /// If you use Hep.Aida.Ref.Converter.ToString(histo) on the result, the OLAP cube of x-"column" vsd y-"column" , summing the weights "column" will be printed.
        /// For example, aggregate sales by product by region.
        /// <p>
        /// Computes the distinct values of x and y, yielding histogram axes that capture one distinct value per bin.
        /// Then fills the histogram.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="weights"></param>
        /// <returns>the histogram containing the cube.</returns>
        /// <exception cref="ArgumentException">if <i>x.Count != y.Count || y.Count != weights.Count</i>.</exception>
        /// <example>
        /// Example output:
        /// <table>
        /// <td class="PRE"> 
        /// <pre>
        /// Cube:
        /// &nbsp;&nbsp;&nbsp;Entries=5000, ExtraEntries=0
        /// &nbsp;&nbsp;&nbsp;MeanX=4.9838, RmsX=NaN
        /// &nbsp;&nbsp;&nbsp;MeanY=2.5304, RmsY=NaN
        /// &nbsp;&nbsp;&nbsp;xAxis: Min=0, Max=10, Bins=11
        /// &nbsp;&nbsp;&nbsp;yAxis: Min=0, Max=5, Bins=6
        /// Heights:
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| X
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| 0   1   2   3   4   5   6   7   8   9   10  | Sum 
        /// ----------------------------------------------------------
        /// Y 5   |  30  53  51  52  57  39  65  61  55  49  22 |  534
        /// &nbsp;&nbsp;4   |  43 106 112  96  92  94 107  98  98 110  47 | 1003
        /// &nbsp;&nbsp;3   |  39 134  87  93 102 103 110  90 114  98  51 | 1021
        /// &nbsp;&nbsp;2   |  44  81 113  96 101  86 109  83 111  93  42 |  959
        /// &nbsp;&nbsp;1   |  54  94 103  99 115  92  98  97 103  90  44 |  989
        /// &nbsp;&nbsp;0   |  24  54  52  44  42  56  46  47  56  53  20 |  494
        /// ----------------------------------------------------------
        /// &nbsp;&nbsp;Sum | 234 522 518 480 509 470 535 476 537 493 226 |     
        /// </pre>
        /// </td>
        /// </table>
        /// </example>
        public static Hep.Aida.IHistogram2D Cube(DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix1D weights)
        {
            if (x.Size != y.Size || y.Size != weights.Size) throw new ArgumentException("vectors must have same size");

            double epsilon = 1.0E-9;
            List<Double> distinct = new List<Double>();
            double[] vals = new double[x.Size];
            List<Double> sorted = new List<Double>(vals);

            // compute distinct values of x
            vals = x.ToArray(); // copy x into vals
            sorted.Sort();
            Cern.Jet.Stat.Descriptive.Frequencies(sorted, distinct, null);
            // since bins are right-open [from,to) we need an additional dummy bin so that the last distinct value does not fall into the overflow bin
            if (distinct.Count > 0) distinct.Add(distinct[distinct.Count - 1] + epsilon);
            distinct.TrimExcess();
            Hep.Aida.IAxis xaxis = new Hep.Aida.Ref.VariableAxis(distinct.ToArray());

            // compute distinct values of y
            vals = y.ToArray();
            sorted.Sort();
            Cern.Jet.Stat.Descriptive.Frequencies(sorted, distinct, null);
            // since bins are right-open [from,to) we need an additional dummy bin so that the last distinct value does not fall into the overflow bin
            if (distinct.Count > 0) distinct.Add(distinct[distinct.Count - 1] + epsilon);
            distinct.TrimExcess();
            Hep.Aida.IAxis yaxis = new Hep.Aida.Ref.VariableAxis(distinct.ToArray());

            Hep.Aida.IHistogram2D histo = new Hep.Aida.Ref.Histogram2D("Cube", xaxis, yaxis);
            return Histogram(histo, x, y, weights);
        }

        /// <summary>
        /// 3-d OLAP cube operator; Fills all cells of the given vectors into the given histogram.
        /// If you use Hep.Aida.Ref.Converter.ToString(histo) on the result, the OLAP cube of x-"column" vsd y-"column" vsd z-"column", summing the weights "column" will be printed.
        /// For example, aggregate sales by product by region by time.
        /// <p>
        /// Computes the distinct values of x and y and z, yielding histogram axes that capture one distinct value per bin.
        /// Then fills the histogram.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="weights"></param>
        /// <returns>the histogram containing the cube.</returns>
        /// <excption cref="ArgumentException">if <i>x.Count != y.Count || x.Count != z.Count || x.Count != weights.Count</i>.</excption>
        public static Hep.Aida.IHistogram3D cube(DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix1D z, DoubleMatrix1D weights)
        {
            if (x.Size != y.Size || x.Size != z.Size || x.Size != weights.Size) throw new ArgumentException("vectors must have same size");

            double epsilon = 1.0E-9;
            List<Double> distinct = new List<Double>();
            double[] vals = new double[x.Size];
            List<Double> sorted = new List<Double>(vals);

            // compute distinct values of x
            vals = x.ToArray(); // copy x into vals
            sorted.Sort();
            Cern.Jet.Stat.Descriptive.Frequencies(sorted, distinct, null);
            // since bins are right-open [from,to) we need an additional dummy bin so that the last distinct value does not fall into the overflow bin
            if (distinct.Count > 0) distinct.Add(distinct[distinct.Count - 1] + epsilon);
            distinct.TrimExcess();
            Hep.Aida.IAxis xaxis = new Hep.Aida.Ref.VariableAxis(distinct.ToArray());

            // compute distinct values of y
            vals = y.ToArray();
            sorted.Sort();
            Cern.Jet.Stat.Descriptive.Frequencies(sorted, distinct, null);
            // since bins are right-open [from,to) we need an additional dummy bin so that the last distinct value does not fall into the overflow bin
            if (distinct.Count > 0) distinct.Add(distinct[distinct.Count - 1] + epsilon);
            distinct.TrimExcess();
            Hep.Aida.IAxis yaxis = new Hep.Aida.Ref.VariableAxis(distinct.ToArray());

            // compute distinct values of z
            vals = z.ToArray();
            sorted.Sort();
            Cern.Jet.Stat.Descriptive.Frequencies(sorted, distinct, null);
            // since bins are right-open [from,to) we need an additional dummy bin so that the last distinct value does not fall into the overflow bin
            if (distinct.Count > 0) distinct.Add(distinct[distinct.Count - 1] + epsilon);
            distinct.TrimExcess();
            Hep.Aida.IAxis zaxis = new Hep.Aida.Ref.VariableAxis(distinct.ToArray());

            Hep.Aida.IHistogram3D histo = new Hep.Aida.Ref.Histogram3D("Cube", xaxis, yaxis, zaxis);
            return Histogram(histo, x, y, z, weights);
        }

        /// <summary>
        /// Constructs and returns the distance matrix of the given matrix.
        /// The distance matrix is a square, symmetric matrix consisting of nothing but distance coefficientsd
        /// The rows and the columns represent the variables, the cells represent distance coefficientsd 
        /// The diagonal cells (i.ed the distance between a variable and itself) will be zero.
        /// Compares two column vectors at a timed Use dice views to compare two row vectors at a time.
        /// </summary>
        /// <param name="matrix">any matrix; a column holds the values of a given variable (vector).</param>
        /// <param name="distanceFunction">(EUCLID, CANBERRA, ..d, or any user defined distance function operating on two vectors).</param>
        /// <returns>the distance matrix (<i>n x n, n=matrix.Columns</i>).</returns>
        public static DoubleMatrix2D Distance(DoubleMatrix2D matrix, VectorVectorFunction distanceFunction)
        {
            int columns = matrix.Columns;
            DoubleMatrix2D distance = new DenseDoubleMatrix2D(columns, columns);

            // cache views
            DoubleMatrix1D[] cols = new DoubleMatrix1D[columns];
            for (int i = columns; --i >= 0;)
            {
                cols[i] = matrix.ViewColumn(i);
            }

            // work out all permutations
            for (int i = columns; --i >= 0;)
            {
                for (int j = i; --j >= 0;)
                {
                    double d = distanceFunction(cols[i], cols[j]);
                    distance[i, j] = d;
                    distance[j, i] = d; // symmetric
                }
            }
            return distance;
        }

        /// <summary>
        /// Fills all cells of the given vector into the given histogram.
        /// </summary>
        /// <param name="histo"></param>
        /// <param name="vector"></param>
        /// <returns><i>histo</i> (for convenience only).</returns>
        public static Hep.Aida.IHistogram1D Histogram(Hep.Aida.IHistogram1D histo, DoubleMatrix1D vector)
        {
            for (int i = vector.Size; --i >= 0;)
            {
                histo.Fill(vector[i]);
            }
            return histo;
        }

        /// <summary>
        /// Fills all cells of the given vectors into the given histogram.
        /// </summary>
        /// <param name="histo"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns><i>histo</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>x.Count != y.Count</i>.</exception>
        public static Hep.Aida.IHistogram2D Histogram(Hep.Aida.IHistogram2D histo, DoubleMatrix1D x, DoubleMatrix1D y)
        {
            if (x.Size != y.Size) throw new ArgumentException("vectors must have same size");
            for (int i = x.Size; --i >= 0;)
            {
                histo.Fill(x[i], y[i]);
            }
            return histo;
        }

        /// <summary>
        /// Fills all cells of the given vectors into the given histogram.
        /// </summary>
        /// <param name="histo"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="weights"></param>
        /// <returns><i>histo</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>x.Count != y.Count || y.Count != weights.Count</i>.</exception>
        public static Hep.Aida.IHistogram2D Histogram(Hep.Aida.IHistogram2D histo, DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix1D weights)
        {
            if (x.Size != y.Size || y.Size != weights.Size) throw new ArgumentException("vectors must have same size");
            for (int i = x.Size; --i >= 0;)
            {
                histo.Fill(x[i], y[i], weights[i]);
            }
            return histo;
        }

        /// <summary>
        /// Fills all cells of the given vectors into the given histogram.
        /// </summary>
        /// <param name="histo"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="weights"></param>
        /// <returns><i>histo</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>x.Count != y.Count || x.Count != z.Count || x.Count != weights.Count</i>.</exception>
        public static Hep.Aida.IHistogram3D Histogram(Hep.Aida.IHistogram3D histo, DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix1D z, DoubleMatrix1D weights)
        {
            if (x.Size != y.Size || x.Size != z.Size || x.Size != weights.Size) throw new ArgumentException("vectors must have same size");
            for (int i = x.Size; --i >= 0;)
            {
                histo.Fill(x[i], y[i], z[i], weights[i]);
            }
            return histo;
        }
    }
}
