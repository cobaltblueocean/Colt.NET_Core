// <copyright file=".cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;
using Cern.Jet.Stat;
using System.Runtime.CompilerServices;
using Cern.Colt.List;

namespace Cern.Hep.Aida.Bin
{
    /// <summary>
    /// 1-dimensional rebinnable bin holding <i>double</i> elements;
    /// Efficiently computes advanced statistics of data sequences.
    /// Technically speaking, a multiset (or bag) with efficient statistics operations defined upon.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// <p>
    /// The data filled into a <i>DynamicBin1D</i> is internally preserved in the bind
    /// As a consequence this bin can compute more than only basic statistics.
    /// On the other hand side, if you add huge amounts of elements, you may run out of memory (each element takes 8 bytes).
    /// If this drawbacks matter, consider to use {@link StaticBin1D},
    /// which overcomes them at the expense of limited functionality.
    /// <p>
    /// This class is fully thread safe (all public methods are synchronized).
    /// Thus, you can have one or more threads adding to the bin as well as one or more threads reading and viewing the statistics of the bin <i>while it is filled</i>.
    /// For high performance, add data in large chunks (buffers) via method <i>addAllOf</i> rather than piecewise via method <i>add</i>.
    /// <p>
    /// If your favourite statistics measure is not directly provided by this class,
    /// check out {@link Cern.Jet.stat.Descriptive} in combination with methods {@link #elements()} and {@link #SortedElements()}d
    /// <p>
    /// <b>Implementation</b>:
    /// Lazy evaluation, caching, incremental maintainance.
    /// 
    /// </summary>
    /// <see cref="Cern.Jet.stat.Descriptive"></see>
    /// @author wolfgang.hoschek@Cern.ch
    /// @version 0.9, 03-Jul-99
    public class DynamicBin1D : QuantileBin1D
    {
        // Never ever use "this.Size" as it would be intuitive!
        // This class abuses "this.Size"d "this.Size" DOES NOT REFLECT the number of elements contained in the receiver!
        // Instead, "this.Size" reflects the number of elements incremental stats computation has already processed.

        /// <summary>
        /// The elements contained in this bin.
        /// </summary>
        private DoubleArrayList _elements = null;

        /// <summary>
        /// The elements contained in this bin, sorted ascending.
        /// </summary>
        private DoubleArrayList _sortedElements = null;

        /// <summary>
        /// Preserve element order under all circumstances?
        /// </summary>
        private Boolean _fixedOrder = false;

        // cached parameters
        //protected double skew = 0.0; 
        //protected double kurtosis = 0.0; 

        // cache states
        private Boolean isSorted = true;
        private Boolean isIncrementalStatValid = true;
        //protected Boolean isSkewValid = true;
        //protected Boolean isKurtosisValid = true;

        private Boolean isSumOfInversionsValid = true;
        private Boolean isSumOfLogarithmsValid = true;
        //protected Boolean isSumOfPowersValid = true;

        private readonly object syncLock = new object();

        /// <summary>
        /// Constructs and returns an empty bin; implicitly calls {@link #setFixedOrder(Boolean) setFixedOrder(false)}.
        /// </summary>
        public DynamicBin1D() : base()
        {
            this.Clear();
            this._elements = new DoubleArrayList();
            this._sortedElements = new DoubleArrayList(0);
            this.FixedOrder = false;
            this.HasSumOfLogarithms = true;
            this.HasSumOfInversions = true;
        }

        /// <summary>
        /// Adds the specified element to the receiver.
        /// 
        /// </summary>
        /// <param name="element">element to be appended.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Add(double element)
        {
            _elements.Add(element);
            InvalidateAll();
        }

        /// <summary>
        /// Adds the part of the specified list between indexes <i>from</i> (inclusive) and <i>to</i> (inclusive) to the receiver.
        /// 
        /// </summary>
        /// <param name="list">the list of which elements shall be added.</param>
        /// <param name="from">the index of the first element to be added (inclusive).</param>
        /// <param name="to">the index of the last element to be added (inclusive).</param>
        /// <exception cref="IndexOutOfRangeException">if <i>list.Count&gt;0 && (from&lt;0 || from&gt;to || to&gt;=list.Count)</i>. </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void AddAllOfFromTo(DoubleArrayList list, int from, int to)
        {
            this._elements.AddAllOfFromTo(list, from, to);
            this.InvalidateAll();
        }

        /// <summary>
        /// Applies a function to each element and aggregates the results.
        /// Returns a value <i>v</i> such that <i>v==a(Size())</i> where <i>a(i) == aggr( a(i-1), f(x(i)) )</i> and terMinators are <i>a(1) == f(x(0)), a(0)==Double.NaN</i>.
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// Cern.Jet.math.Functions F = Cern.Jet.math.Functions.Functions;
        /// bin = 0 1 2 3 
        ///
        /// // Sum( x[i]*x[i] )
        /// bin.aggregate(F.plus,F.square);
        /// --> 14
        /// </pre>
        /// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
        /// </summary>
        /// <param name="aggr">an aggregation function taking as first argument the current aggregation and as second argument the transformed current element.</param>
        /// <param name="f">a function transforMing the current element.</param>
        /// <returns>the aggregated measure.</returns>
        /// <see cref="Cern.Jet.math.Functions"></see>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Aggregate(Cern.Colt.Function.DoubleDoubleFunction aggr, Cern.Colt.Function.DoubleFunction f)
        {
            int s = Size;
            if (s == 0) return Double.NaN;
            double a = f(_elements[s - 1]);
            for (int i = s - 1; --i >= 0;)
            {
                a = aggr(a, f(_elements[i]));
            }
            return a;
        }

        /// <summary>
        /// Removes all elements from the receiver.
        /// The receiver will be empty after this call returns.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Clear()
        {
            base.Clear();

            if (this._elements != null) this._elements.Clear();
            if (this.SortedElements != null) this.SortedElements.Clear();

            this.ArgumentCheckerAll();
        }

        /// <summary>
        /// Returns a deep copy of the receiver.
        /// 
        /// </summary>
        /// <returns>a deep copy of the receiver.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override Object Clone()
        {
            DynamicBin1D clone = (DynamicBin1D)base.Clone();
            if (this._elements != null) clone._elements = clone._elements.Copy();
            if (this.SortedElements != null) clone.SortedElements = clone.SortedElements.Copy();
            return clone;
        }

        /// <summary>
        /// Returns the correlation of two bins, which is <i>corr(x,y) = covariance(x,y) / (stdDev(x)*stdDev(y))</i> (Pearson's correlation coefficient).
        /// A correlation coefficient varies between -1 (for a perfect negative relationship) to +1 (for a perfect positive relationship)d
        /// See the <A HREF="http://www.cquest.utoronto.ca/geog/ggr270y/notes/not05efg.html"> math definition</A>
        /// and <A HREF="http://www.stat.berkeley.edu/users/stark/SticiGui/Text/gloss.htm#correlation_coef"> another def</A>.
        /// </summary>
        /// <param name="other">the bin to compare with.</param>
        /// <returns>the correlation.</returns>
        /// <exception cref="ArgumentException">if <i>Size() != other.Count</i>. </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Correlation(DynamicBin1D other)
        {
            lock (syncLock)
            {
                return Covariance(other) / (StandardDeviation() * other.StandardDeviation());
            }
        }

        /// <summary>
        /// Returns the covariance of two bins, which is <i>cov(x,y) = (1/Size()) * Sum((x[i]-mean(x)) * (y[i]-mean(y)))</i>.
        /// See the <A HREF="http://www.cquest.utoronto.ca/geog/ggr270y/notes/not05efg.html"> math definition</A>.
        /// </summary>
        /// <param name="other">the bin to compare with.</param>
        /// <returns>the covariance.</returns>
        /// <exception cref="ArgumentException">if <i>Size() != other.Count</i>. </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Covariance(DynamicBin1D other)
        {
            lock (syncLock)
            {
                if (Size != other.Size) throw new ArgumentException("both bins must have same Size");
                double s = 0;
                for (int i = Size; --i >= 0;)
                {
                    s += this._elements[i] * other._elements[i];
                }

                double cov = (s - Sum * other.Sum / Size) / Size;
                return cov;
            }
        }

        /// <summary>
        /// Returns a copy of the currently stored elements.
        /// Concerning the order in which elements are returned, see {@link #setFixedOrder(Boolean)}.
        /// </summary>
        /// <returns>a copy of the currently stored elements.</returns>
        public DoubleArrayList Elements
        {
            get
            {
                // safe since we are already synchronized.
                if (elements_unsafe() != null)
                    return elements_unsafe().Copy();
                else
                    return null;
            }
            set
            {
                this._elements = value;
            }
        }

        /// <summary>
        /// Returns the currently stored elements; <b>WARNING:</b> not a copy of them.
        /// Thus, improper usage of the returned list may not only corrupt the receiver's internal state, but also break thread safety!
        /// Only provided for performance and memory sensitive applications.
        /// Do not modify the returned list unless you know exactly what you're doing.
        /// This method can be used in a thread safe, clean <i>and</i> performant way by
        /// explicitly synchronizing on the bin, as follows:
        /// <pre>
        /// ..
        /// double sinSum = 0;
        /// synchronized (dynamicBin) { // lock out anybody else
        ///     DoubleArrayList elements = dynamicBin.elements_unsafe();
        ///     // read each element and do something with it, for example
        /// 	   double[] values = elements.ToArray(); // zero-copy
        /// 	   for (int i=dynamicBin.Count; --i >=0; ) {
        ///         sinSum += System.Math.Sin(values[i]);
        /// 	   }
        /// }
        /// Console.WriteLine(sinSum);
        /// ..
        /// </pre>
        /// 
        /// Concerning the order in which elements are returned, see {@link #setFixedOrder(Boolean)}.
        /// </pre>summary>
        /// <returns>the currently stored elements.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected DoubleArrayList elements_unsafe()
        {
            return this._elements;
        }

        /// <summary>
        /// Returns whether two bins are equald
        /// They are equal if the other object is of the same class or a subclass of this class and both
        /// have the same Size, Minimum, Maximum, sum and sumOfSquares
        /// and have the same elements, order being irrelevant (multiset equality).
        /// <p>
        /// Definition of <i>Equality</i> for multisets:
        /// A,B are equal <=> A is a baseset of B and B is a baseset of A.
        /// (Elements must occur the same number of times, order is irrelevantd)
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override Boolean Equals(Object obj)
        {
            if (!(obj is DynamicBin1D)) return false;
            if (!base.Equals(obj)) return false;

            DynamicBin1D other = (DynamicBin1D)obj;
            double[] s1 = sortedElements_unsafe().ToArray();
            lock (syncLock)
            {
                double[] s2 = other.sortedElements_unsafe().ToArray();
                int n = Size;
                return Includes(s1, s2, 0, n, 0, n) &&
                    Includes(s2, s1, 0, n, 0, n);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        private static Boolean Includes(double[] array1, double[] array2, int first1, int last1, int first2, int last2)
        {
            while (first1 < last1 && first2 < last2)
            {
                if (array2[first2] < array1[first1])
                    return false;
                else if (array1[first1] < array2[first2])
                    ++first1;
                else
                {
                    ++first1;
                    ++first2;
                }
            }

            return first2 == last2;
        }

        /// <summary>
        /// Computes the frequency (number of occurances, count) of each distinct element.
        /// After this call returns both <i>distinctElements</i> and <i>frequencies</i> have a new Size (which is equal for both), which is the number of distinct elements currently contained.
        /// <p>
        /// Distinct elements are filled into <i>distinctElements</i>, starting at index 0.
        /// The frequency of each distinct element is filled into <i>frequencies</i>, starting at index 0.
        /// Further, both <i>distinctElements</i> and <i>frequencies</i> are sorted ascending by "element" (in sync, of course).
        /// As a result, the smallest distinct element (and its frequency) can be found at index 0, the second smallest distinct element (and its frequency) at index 1, .d, the largest distinct element (and its frequency) at index <i>distinctElements.Count-1</i>.
        /// <p>
        /// <b>Example:</b>
        /// <br>
        /// <i>elements = (8,7,6,6,7) --> distinctElements = (6,7,8), frequencies = (2,2,1)</i>
        /// 
        /// </summary>
        /// <param name="distinctElements">a list to be filled with the distinct elements; can have any Size.</param>
        /// <param name="frequencies">     a list to be filled with the frequencies; can have any Size; set this parameter to <i>null</i> to ignore it.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Frequencies(DoubleArrayList distinctElements, IntArrayList frequencies)
        {
            Descriptive.Frequencies(sortedElements_unsafe(), distinctElements, frequencies);
        }

        /// <summary>
        /// Returns a map holding the frequency distribution, that is, (distintElement,frequency) pairs.
        /// The frequency (count) of an element is its number of occurances.
        /// <p>
        /// <b>Example:</b>
        /// <br>
        /// <i>elements = (8,7,6,6,7) --> map.keys = (8,6,7), map.Values = (1,2,2)</i>
        /// 
        /// </summary>
        /// <returns>a map holding the frequency distribution.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private Dictionary<double, int> FrequencyMap()
        {
            //Cern.Colt.Map.OpenDoubleIntHashMap.hashCollisions = 0;
            // fill a map that collects frequencies
            //Cern.Colt.Map.AbstractDoubleIntMap map = new Cern.Colt.Map.OpenDoubleIntHashMap();
            var map = new Dictionary<double, int>();
            //Cern.Colt.Timer timer = new Cern.Colt.Timer().start();
            for (int i = Size; --i >= 0;)
            {
                double element = this.Elements[i];
                //double element = i; // benchmark only TODO
                //double element = i%1000; // benchmark only TODO
                map.AddOrUpdate(element, 1 + map[element]);
            }
            //timer.stop();
            //Console.WriteLine("filling map took = "+timer);
            //Console.WriteLine("collisions="+Cern.Colt.Map.OpenDoubleIntHashMap.hashCollisions);

            return map;
        }

        /// <summary>
        /// Returns <i>int.MaxValue</i>, the Maximum order <i>k</i> for which sums of powers are retrievable.
        /// </summary>
        /// <see cref="#hasSumOfPowers(int)"></see>
        /// <see cref="#sumOfPowers(int)"></see>
        public int MaxOrderForSumOfPowers
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        /// Returns <i>int.MinValue</i>, the Minimum order <i>k</i> for which sums of powers are retrievable.
        /// </summary>
        /// <see cref="#hasSumOfPowers(int)"></see>
        /// <see cref="#sumOfPowers(int)"></see>
        public int MinOrderForSumOfPowers
        {
            get { return int.MinValue; }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="element">element to be appended.</param>
        protected void InvalidateAll()
        {
            this.isSorted = false;
            this.isIncrementalStatValid = false;

            //this.isSkewValid = false;
            //this.isKurtosisValid = false;

            this.isSumOfInversionsValid = false;
            this.isSumOfLogarithmsValid = false;
        }

        /// <summary>
        /// Returns <i>true</i>.
        /// Returns whether a client can obtain all elements added to the receiver.
        /// In other words, tells whether the receiver internally preserves all added elements.
        /// If the receiver is rebinnable, the elements can be obtained via <i>elements()</i> methods.
        /// 
        /// </summary>
        public override Boolean IsRebinnable
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the Maximum.
        /// </summary>
        public override double Max
        {
            get
            {
                if (!isIncrementalStatValid) UpdateIncrementalStats();
                return base.Max;
            }
        }

        /// <summary>
        /// Returns the Minimum.
        /// </summary>
        public override double Min
        {
            get
            {
                if (!isIncrementalStatValid) UpdateIncrementalStats();
                return base.Min;
            }
        }

        /// <summary>
        /// Returns the Moment of <i>k</i>-th order with value <i>c</i>,
        /// which is <i>Sum( (x[i]-c)<sup>k</sup> ) / Size()</i>.
        /// </summary>
        /// <param name="k">the order; any number - can be less than zero, zero or greater than zero.</param>
        /// <param name="c">any number.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override double Moment(int k, double c)
        {
            // currently no caching for this parameter
            return Descriptive.Moment(this.Elements, k, c);
        }

        /// <summary>
        /// Returns the exact <i>phi-</i>Quantile; that is, the smallest contained element <i>elem</i> for which holds that <i>phi</i> percent of elements are less than <i>elem</i>.
        /// </summary>
        /// <param name="phi">must satisfy <i>0 &lt; phi &lt; 1</i>.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override double Quantile(double phi)
        {
            return Descriptive.Quantile(sortedElements_unsafe(), phi);
        }

        /// <summary>
        /// Returns exactly how many percent of the elements contained in the receiver are <i>&lt;= element</i>.
        /// Does linear interpolation if the element is not contained but lies in between two contained elements.
        /// 
        /// </summary>
        /// <param name="element">the element to search for.</param>
        /// <returns>the exact percentage <i>phi</i> of elements <i>&lt;= element</i> (<i>0.0 &lt;= phi &lt;= 1.0)</i>.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override double QuantileInverse(double element)
        {
            return Descriptive.QuantileInverse(sortedElements_unsafe(), element);
        }

        /// <summary>
        /// Returns the exact Quantiles of the specified percentages.
        /// </summary>
        /// <param name="percentages">the percentages for which Quantiles are to be computed.</param>
        /// Each percentage must be in the interval <i>(0.0,1.0]</i>d <i>percentages</i> must be sorted ascending.
        /// <returns>the exact Quantiles.</returns>
        public override DoubleArrayList Quantiles(DoubleArrayList percentages)
        {
            return Descriptive.Quantiles(sortedElements_unsafe(), percentages);
        }

        /// <summary>
        /// Removes from the receiver all elements that are contained in the specified list.
        /// 
        /// </summary>
        /// <param name="list">the elements to be removed.</param>
        /// <returns><code>true</code> if the receiver changed as a result of the call.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Boolean RemoveAllOf(DoubleArrayList list)
        {
            Boolean changed = this.Elements.RemoveAll(list);
            if (changed)
            {
                ClearAllMeasures();
                InvalidateAll();
                //this.Size() = 0;
                if (FixedOrder)
                {
                    this.SortedElements.RemoveAll(list);
                    this.isSorted = true;
                }
            }
            return changed;
        }

        /// <summary>
        /// Uniformly samples (chooses) <i>n</i> random elements <i>with or without replacement</i> from the contained elements and adds them to the given buffer.
        /// If the buffer is connected to a bin, the effect is that the chosen elements are added to the bin connected to the buffer.
        /// Also see {@link #buffered(int) buffered}.
        /// 
        /// </summary>
        /// <param name="n">the number of elements to choose.</param>
        /// <param name="withReplacement"><i>true</i> samples with replacement, otherwise samples without replacement.</param>
        /// <param name="randomGenerator">a random number generatord Set this parameter to <i>null</i> to use a default random number generator seeded with the current time.</param>
        /// <param name="buffer">the buffer to which chosen elements will be added.</param>
        /// <exception cref="ArgumentException">if <i>!withReplacement && n > Size()</i>. </exception>
        /// <see cref="Cern.Jet.Random.sampling"></see>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Sample(int n, Boolean withReplacement, RandomEngine randomGenerator, Cern.Colt.Buffer.DoubleBuffer buffer)
        {
            if (randomGenerator == null) randomGenerator = Cern.Jet.Random.Uniform.MakeDefaultGenerator();
            buffer.Clear();

            if (!withReplacement)
            { // without
                if (n > Size) throw new ArgumentException("n must be less than or equal to Size()");
                Cern.Jet.Random.Sampling.RandomSamplingAssistant sampler = new Cern.Jet.Random.Sampling.RandomSamplingAssistant(n, Size, randomGenerator);
                for (int i = n; --i >= 0;)
                {
                    if (sampler.SampleNextElement()) buffer.Add(this.Elements[i]);
                }
            }
            else
            { // with
                Cern.Jet.Random.Uniform uniform = new Cern.Jet.Random.Uniform(randomGenerator);
                int s = Size;
                for (int i = n; --i >= 0;)
                {
                    buffer.Add(this.Elements[uniform.NextIntFromTo(0, s - 1)]);
                }
                buffer.Flush();
            }
        }

        /// <summary>
        /// Generic bootstrap resamplingd 
        /// Quite optimized - Don't be afraid to try itd 
        /// Executes<i>resamples</i> resampling stepsd In each resampling step does the following: 
        /// <ul>
        /// <li>Uniformly samples (chooses) <i>Size()</i> random elements <i>with replacement</i> 
        ///  from<i>this</i> and fills them into an auxiliary bin <i>b1</i>d 
        /// <li>Uniformly samples (chooses) <i>other.Count</i> random elements <i>with replacement</i> 
        ///  from<i>other</i> and fills them into another auxiliary bin <i>b2</i>d 
        /// <li>Executes the comparison function <i>function</i> on both auxiliary bins 
        /// (<i>function.Apply(b1,b2)</i>) and adds the result of the function to an auxiliary bootstrap bin <i>b3</i>d 
        /// </ul>
        /// <p>Finally returns the auxiliary bootstrap bin <i>b3</i> from which the measure of interest 
        /// can be read offd </p>
        /// <p><b>Background:</b></p>
        /// <p>Also see a more <A HREF="http://garnet.acns.fsu.edu/~pkelly/bootstrap.html"> in-depth discussion</A> on bootstrapping and related randomization methods.
        /// The classical statistical test for comparing the means of two samples is the 
        /// <i>t-test</i>d Unfortunately, this test assumes that the two samples each come 
        /// from a normal distribution and that these distributions have the same standard 
        /// deviationd Quite often, however, data has a distribution that is non-normal 
        /// in many waysd In particular, distributions are often unsymmetricd For such data, 
        /// the t-test may produce misleading results and should thus not be usedd Sometimes 
        /// asymmetric data can be transformed into normally distributed data by taking 
        /// e.gd the logarithm and the t-test will then produce valid results, but this 
        /// still requires postulation of a certain distribution underlying the data, which 
        /// is often not warranted, because too little is known about the data composition.</p>
        /// <p><i>Bootstrap resampling of means differences</i> (and other differences) is 
        /// a robust replacement for the t-test and does not require assumptions about the actual 
        /// distribution of the datad The idea of bootstrapping is quite simple: simulationd 
        /// The only assumption required is that the two samples <i>a</i> and <i>b</i> 
        /// are representative for the underlying distribution with respect to the statistic 
        /// that is being tested - this assumption is of course implicit in all statistical 
        /// testsd We can now generate lots of further samples that correspond to the two 
        /// given ones, by sampling <i>with replacement</i>d This process is called <i>resampling</i>d 
        /// A resample can (and usually will) have a different mean than the original one 
        /// and by drawing hundreds or thousands of such resamples <i>a<sub>r</sub></i> 
        /// from<i>a</i> and <i>b<sub>r</sub></i> from <i>b</i> we can compute the 
        /// so-called bootstrap distribution of all the differences &quot;mean of <i>a<sub>r</sub></i> 
        /// Minus mean of <i>b<sub>r</sub></i>&quot;d That is, a bootstrap bin filled with the differencesd Now we 
        /// can compute, what fraction of these differences is, say, greater than zerod 
        /// Let's assume we have computed 1000 resamples of both <i>a</i> and <i>b</i> 
        /// and found that only <i>8</i> of the differences were greater than zerod Then <i>8/1000</i> 
        /// or<i>0.008</i> is the p-value (probability) for the hypothesis that the mean 
        /// of the distribution underlying <i>a</i> is actually larger than the mean of 
        /// the distribution underlying <i>b</i>d From this bootstrap test, we can clearly 
        /// reject the hypothesis.</p>
        /// <p>Instead of using means differences, we can also use other differences, for 
        /// example, the median differences.</p>
        /// <p>Instead of p-values we can also read arbitrary confidence intervals from the 
        /// bootstrap bind For example, <i>90%</i> of all bootstrap differences 
        /// are left of the value <i>-3.5</i>, hence a left <i>90%</i> confidence interval 
        /// for the difference would be <i>(3.5,infinity)</i>; in other words: the difference 
        /// is <i>3.5</i> or larger with probability <i>0.9</i>.</p>
        /// <p>Sometimes we would like to compare not only means and medians, but also the 
        /// variability(spread) of two samplesd The conventional method of doing this is 
        /// the<i>F-test</i>, which compares the standard deviationsd It is related to 
        /// the t-test and, like the latter, assumes the two samples to come from a normal 
        /// distributiond The F-test is very sensitive to data with deviations from normalityd 
        /// Instead we can again resort to more robust bootstrap resampling and compare a measure of 
        /// spread, for example the inter-quartile ranged This way we compute a <i>bootstrap 
        /// resampling of inter-quartile range differences</i> in order to arrive at a test 
        ///  for inequality or variability.
        /// </p>
        /// <p> 
        /// <b>Example:</b> 
        /// <table>
        /// <td class="PRE"> 
        /// <pre>
        /// // v1,v2 - the two samples to compare against each other
        /// double[] v1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9,10,  21,  22,23,24,25,26,27,28,29,30,31};
        /// double[] v2 = {10,11,12,13,14,15,16,17,18,19,  20,  30,31,32,33,34,35,36,37,38,39};
        /// hep.aida.bin.DynamicBin1D X = new hep.aida.bin.DynamicBin1D();
        /// hep.aida.bin.DynamicBin1D Y = new hep.aida.bin.DynamicBin1D();
        /// X.addAllOf(new Cern.Colt.list.DoubleArrayList(v1));
        /// Y.addAllOf(new Cern.Colt.list.DoubleArrayList(v2));
        /// Cern.Jet.Random.Engine.RandomEngine random = new Cern.Jet.Random.Engine.MersenneTwister();
        /// 
        /// // bootstrap resampling of differences of means:
        /// BinBinFunction1D diff = new BinBinFunction1D() {
        /// &nbsp;&nbsp;&nbsp;public double apply(DynamicBin1D x, DynamicBin1D y) {return x.mean() - y.mean();}
        /// };
        /// 
        /// // bootstrap resampling of differences of medians:
        /// BinBinFunction1D diff = new BinBinFunction1D() {
        /// &nbsp;&nbsp;&nbsp;public double apply(DynamicBin1D x, DynamicBin1D y) {return x.median() - y.median();}
        /// };
        /// 
        /// // bootstrap resampling of differences of inter-quartile ranges:
        /// BinBinFunction1D diff = new BinBinFunction1D() {
        /// &nbsp;&nbsp;&nbsp;public double apply(DynamicBin1D x, DynamicBin1D y) {return (x.Quantile(0.75)-x.Quantile(0.25)) - (y.Quantile(0.75)-y.Quantile(0.25)); }
        /// };
        /// 
        /// DynamicBin1D boot = X.sampleBootstrap(Y,1000,random,diff);
        /// 
        /// Cern.Jet.math.Functions F = Cern.Jet.math.Functions.Functions;
        ///  Console.WriteLine("p-value="+ (boot.aggregate(F.plus, F.greater(0)) / boot.Count));
        ///         Console.WriteLine("left 90% confidence interval = ("+boot.Quantile(0.9) + ",infinity)");
        /// 
        /// -->
        /// // bootstrap resampling of differences of means:
        /// p-value=0.0080
        /// left 90% confidence interval = (-3.571428571428573,infinity)
        /// 
        /// // bootstrap resampling of differences of medians:
        /// p-value=0.36
        /// left 90% confidence interval = (5.0,infinity)
        /// 
        /// // bootstrap resampling of differences of inter-quartile ranges:
        /// p-value=0.5699
        /// left 90% confidence interval = (5.0,infinity)
        /// </pre>
        /// </td>
        /// </table>
        /// </summary>
        /// <param name="other">the other bin to compare the receiver againstd</param>
        /// <param name="resamples">the number of times resampling shall be doned</param>
        /// <param name="randomGenerator">a random number generatord Set this parameter to <i>null</i> to use a default random number</param>
        /// generator seeded with the current timed 
        /// <param name="function">a difference function comparing two samples; takes as first argument a sample of <i>this</i> and as second argument</param>
        /// a sample of <i>other</i>d 
        /// <returns>a bootstrap bin holding the results of <i>function</i> of each resampling step.</returns>
        /// <see cref="Cern.Colt.GenericPermuting#permutation(long,int)"></see>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DynamicBin1D SampleBootstrap(DynamicBin1D other, int resamples, Cern.Jet.Random.Engine.RandomEngine randomGenerator, BinBinFunction1D function)
        {
            if (randomGenerator == null) randomGenerator = Cern.Jet.Random.Uniform.MakeDefaultGenerator();

            // since "resamples" can be quite large, we care about performance and memory
            int MaxCapacity = 1000;
            int s1 = Size;
            int s2 = other.Size;

            // prepare auxiliary bins and buffers
            DynamicBin1D sample1 = new DynamicBin1D();
            Cern.Colt.Buffer.DoubleBuffer buffer1 = sample1.Buffered(System.Math.Min(MaxCapacity, s1));

            DynamicBin1D sample2 = new DynamicBin1D();
            Cern.Colt.Buffer.DoubleBuffer buffer2 = sample2.Buffered(System.Math.Min(MaxCapacity, s2));

            DynamicBin1D bootstrap = new DynamicBin1D();
            Cern.Colt.Buffer.DoubleBuffer bootBuffer = bootstrap.Buffered(System.Math.Min(MaxCapacity, resamples));

            // resampling steps
            for (int i = resamples; --i >= 0;)
            {
                sample1.Clear();
                sample2.Clear();

                this.Sample(s1, true, randomGenerator, buffer1);
                other.Sample(s2, true, randomGenerator, buffer2);

                bootBuffer.Add(function(sample1, sample2));
            }
            bootBuffer.Flush();
            return bootstrap;
        }

        /// <summary>
        /// DeterMines whether the receivers internally preserved elements may be reordered or not.
        /// <ul>
        /// <li><i>fixedOrder==false</i> allows the order in which elements are returned by method <i>elements()</i> to be different from the order in which elements are added.
        /// <li><i>fixedOrder==true</i> guarantees that under all circumstances the order in which elements are returned by method <i>elements()</i> is identical to the order in which elements are added.
        /// However, the latter consumes twice as much memory if operations involving sorting are requested.
        /// This option is usually only required if a 2-dimensional bin, formed by two 1-dimensional bins, needs to be rebinnable.
        /// </ul>
        /// <p>
        /// Naturally, if <i>fixedOrder</i> is set to <i>true</i> you should not already have added elements to the receiver; it should be empty.
        /// </summary>
        public Boolean FixedOrder
        {
            get { return this._fixedOrder; }
            set
            {
                //if (Size() > 0) throw new System.Exception("must be called before starting to add elements.");
                this._fixedOrder = value;
            }
        }

        /// <summary>
        /// Returns the number of elements contained in the receiver.
        /// 
        /// </summary>
        /// <returns>  the number of elements contained in the receiver.</returns>
        public override int Size
        {
            get { return Elements.Size; }
            // Never ever use "this.Size" as it would be intuitive!
            // This class abuses "this.Size"d "this.Size" DOES NOT REFLECT the number of elements contained in the receiver!
            // Instead, "this.Size" reflects the number of elements incremental stats computation has already processed.
        }

        /// <summary>
        /// Sorts elements if not already sorted.
        /// </summary>
        protected void Sort()
        {
            if (!this.isSorted)
            {
                if (this.FixedOrder)
                {
                    this.SortedElements.Clear();
                    this.SortedElements.AddAllOfFromTo(this.Elements, 0, this.Elements.Size - 1);
                    this.SortedElements.Sort();
                }
                else
                {
                    /*
                       Call updateIncrementalStats() because after sorting we no more know
                       what elements are still to be done by updateIncrementalStats()
                       and would therefore later need to rebuild incremental stats from scratch.
                    */
                    UpdateIncrementalStats();
                    InvalidateAll();

                    this.Elements.Sort();
                    this.isIncrementalStatValid = true;
                }
                this.isSorted = true;
            }
        }

        /// <summary>
        /// Returns a copy of the currently stored elements, sorted ascending.
        /// Concerning the memory required for operations involving sorting, see {@link #setFixedOrder(Boolean)}.
        /// 
        /// </summary>
        /// <returns>a copy of the currently stored elements, sorted ascending.</returns>
        public DoubleArrayList SortedElements
        {
            get
            {
                if (elements_unsafe() != null)
                    // safe since we are already synchronized.
                    return sortedElements_unsafe().Copy();
                else
                    return null;
            }
            set
            {
                this.SortedElements = value;
            }
        }

        /// <summary>
        /// Returns the currently stored elements, sorted ascending; <b>WARNING:</b> not a copy of them;
        /// Thus, improper usage of the returned list may not only corrupt the receiver's internal state, but also break thread safety!
        /// Only provided for performance and memory sensitive applications.
        /// Do not modify the returned elements unless you know exactly what you're doing.
        /// This method can be used in a thread safe, clean <i>and</i> performant way by
        /// explicitly synchronizing on the bin, as follows:
        /// <pre>
        /// ..
        /// synchronized (dynamicBin) { // lock out anybody else
        ///     DoubleArrayList elements = dynamicBin.SortedElements_unsafe();
        /// 	   // read each element and do something with it, e.g.
        /// 	   double[] values = elements.ToArray(); // zero-copy
        /// 	   for (int i=dynamicBin.Count; --i >=0; ) {
        ///         foo(values[i]);
        /// 	   }
        /// }
        /// ..
        /// </pre>
        /// 
        /// Concerning the memory required for operations involving sorting, see {@link #setFixedOrder(Boolean)}.
        /// </summary>
        /// <returns>the currently stored elements, sorted ascending.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected DoubleArrayList sortedElements_unsafe()
        {
            Sort();
            if (FixedOrder) return this._sortedElements;
            return this._elements;
        }

        /// <summary>
        /// Modifies the receiver to be standardized.
        /// Changes each element <i>x[i]</i> as follows: <i>x[i] = (x[i]-mean)/standardDeviation</i>.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Standardize(double mean, double standardDeviation)
        {
            Descriptive.Standardize(this.Elements, mean, standardDeviation);
            ClearAllMeasures();
            InvalidateAll();
            //this.Size = 0;
        }

        /// <summary>
        /// Returns the sum of all elements, which is <i>Sum( x[i] )</i>.
        /// </summary>
        public override double Sum
        {
            get
            {
                if (!isIncrementalStatValid) UpdateIncrementalStats();
                return base.Sum;
            }
        }

        /// <summary>
        /// Returns the sum of inversions, which is <i>Sum( 1 / x[i] )</i>.
        /// </summary>
        public override double SumOfInversions
        {
            get
            {
                if (!isSumOfInversionsValid) UpdateSumOfInversions();
                return base.SumOfInversions;
            }
        }

        /// <summary>
        /// Returns the sum of logarithms, which is <i>Sum( Log(x[i]) )</i>.
        /// </summary>
        public override double SumOfLogarithms
        {
            get
            {

                if (!isSumOfLogarithmsValid) UpdateSumOfLogarithms();
                return base.SumOfLogarithms;
           
            }
        }

        /// <summary>
        /// Returns the <i>k-th</i> order sum of powers, which is <i>Sum( x[i]<sup>k</sup> )</i>.
        /// </summary>
        /// <param name="k">the order of the powers.</param>
        /// <returns>the sum of powers.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override double GetSumOfPowers(int k)
        {
            // no chaching for this measure
            if (k >= -1 && k <= 2) return base.GetSumOfPowers(k);

            return Descriptive.SumOfPowers(this.Elements, k);
        }

        /// <summary>
        /// Returns the sum of squares, which is <i>Sum( x[i] * x[i] )</i>.
        /// </summary>
        public override double SumOfSquares
        {
            get
            {
                if (!isIncrementalStatValid) UpdateIncrementalStats();
                return base.SumOfSquares;
            }
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override String ToString()
        {
            StringBuilder buf = new StringBuilder(base.ToString());
            DoubleArrayList distinctElements = new DoubleArrayList();
            IntArrayList freq = new IntArrayList();
            Frequencies(distinctElements, freq);
            if (distinctElements.Size < 100)
            { // don't cause unintended floods
                buf.Append("Distinct elements: " + distinctElements + "\n");
                buf.Append("Frequencies: " + freq + "\n");
            }
            else
            {
                buf.Append("Distinct elements & frequencies not printed (too many).");
            }
            return buf.ToString();
        }

        /// <summary>
        /// Removes the <i>s</i> smallest and <i>l</i> largest elements from the receiver.
        /// The receivers Size will be reduced by <i>s + l</i> elements.
        /// 
        /// </summary>
        /// <param name="s">the number of smallest elements to trim away (<i>s >= 0</i>).</param>
        /// <param name="l">the number of largest elements to trim away (<i>l >= 0</i>).</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Trim(int s, int l)
        {
            DoubleArrayList elems = SortedElements;
            Clear();
            AddAllOfFromTo(elems, s, elems.Size - 1 - l);
        }

        /// <summary>
        /// Returns the trimmed mean.
        /// That is the mean of the data <i>if</i> the <i>s</i> smallest and <i>l</i> largest elements <i>would</i> be removed from the receiver (they are not removed).
        /// 
        /// </summary>
        /// <param name="s">the number of smallest elements to trim away (<i>s >= 0</i>).</param>
        /// <param name="l">the number of largest elements to trim away (<i>l >= 0</i>).</param>
        /// <returns>the trimmed mean.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double TrimmedMean(int s, int l)
        {
            // no caching for this parameter.
            return Descriptive.TrimmedMean(sortedElements_unsafe(), Mean(), s, l);
        }

        /// <summary>
        /// Trims the capacity of the receiver to be the receiver's current Size.
        /// (This has nothing to do with trimMing away smallest and largest elementsd The method name is used to be consistent with JDK practiced)
        /// <p>
        /// Releases any basefluos internal memory.
        /// An application can use this operation to Minimize the storage of the receiver.
        /// Does not affect functionality.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void TrimToSize()
        {
            this.Elements.TrimToSize();

            this.SortedElements.Clear();
            this.SortedElements.TrimToSize();
            if (FixedOrder) this.isSorted = false;
        }

        /// <summary>
        /// assertion: isBasicParametersValid == false
        /// 
        /// </summary>
        protected void UpdateIncrementalStats()
        {
            // prepare arguments
            double[] arguments = new double[4];
            arguments[0] = base.Min;
            arguments[1] = base.Max;
            arguments[2] = base.Sum;
            arguments[3] = base.SumOfSquares;

            Descriptive.IncrementalUpdate(this.Elements, this.Size, this.Elements.Size - 1, ref arguments);

            // store the new parameters back
            base.Min = arguments[0];
            base.Max = arguments[1];
            base.Sum = arguments[2];
            base.SumOfSquares = arguments[3];

            this.isIncrementalStatValid = true;
            //this.Size = this.Elements.Count; // next time we don't need to redo the stuff we have just done..
        }

        /// <summary>
        /// assertion: isBasicParametersValid == false
        /// 
        /// </summary>
        protected void UpdateSumOfInversions()
        {
            base.SumOfInversions = Descriptive.SumOfInversions(this.Elements, 0, Size - 1);
            this.isSumOfInversionsValid = true;
        }
        /// <summary>
        /// 
        /// </summary>
        protected void UpdateSumOfLogarithms()
        {
            base.SumOfLogarithms = Descriptive.SumOfLogarithms(this.Elements, 0, Size - 1);
            this.isSumOfLogarithmsValid = true;
        }
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="element">element to be appended.</param>
        protected void ArgumentCheckerAll()
        {
            this.isSorted = true;
            this.isIncrementalStatValid = true;

            //this.isSkewValid = true;
            //this.isKurtosisValid = true;

            this.isSumOfInversionsValid = true;
            this.isSumOfLogarithmsValid = true;
        }
    }
}
