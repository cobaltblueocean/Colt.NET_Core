using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random
{
    /// <summary>
    /// Abstract base class for all random distributions.
    /// 
    /// A subclass of this class need to override method <see cref="NextDouble()"/> and, in rare cases, also <see cref="NextInt()"/>.
    /// <p>
    /// Currently all subclasses use a uniform pseudo-random number generation engine and transform its results to the target distribution.
    /// Thus, they expect such a uniform engine upon instance construction.
    /// <p>
    /// <see cref="Cern.Jet.Random.Engine.MersenneTwister"/> is recommended as uniform pseudo-random number generation engine, since it is very strong and at the same time quick.
    /// {@link #makeDefaultGenerator()} will conveniently construct and return such a magic thing.
    /// You can also, for example, use <see cref="Cern.Jet.Random.Engine.DRand"/>, a quicker (but much weaker) uniform random number generation engine.
    /// Of course, you can also use other strong uniform random number generation engines. 
    ///
    /// <p>
    /// <b>Ressources on the Web:</b>
    /// <dt>Check the Web version of the <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node1.html"> CERN Data Analysis Briefbook </A>. This will clarify the definitions of most distributions.
    /// <dt>Also consult the <A HREF="http://www.statsoftinc.com/textbook/stathome.html"> StatSoft Electronic Textbook</A> - the definite web book.
    /// <p>
    /// <b>Other useful ressources:</b>
    /// <dt><A HREF="http://www.stats.gla.ac.uk/steps/glossary/probability_distributions.html"> Another site </A> and <A HREF="http://www.statlets.com/usermanual/glossary.htm"> yet another site </A>describing the definitions of several distributions.
    /// <dt>You may want to check out a <A HREF="http://www.stat.berkeley.edu/users/stark/SticiGui/Text/gloss.htm"> Glossary of Statistical Terms</A>.
    /// <dt>The GNU Scientific Library contains an extensive (but hardly readable) <A HREF="http://sourceware.cygnus.com/gsl/html/gsl-ref_toc.html#TOC26"> list of definition of distributions</A>.
    /// <dt>Use this Web interface to <A HREF="http://www.stat.ucla.edu/calculators/cdf"> plot all sort of distributions</A>.
    /// <dt>Even more ressources: <A HREF="http://www.animatedsoftware.com/statglos/statglos.htm"> Internet glossary of Statistical Terms</A>,
    /// <A HREF="http://www.ruf.rice.edu/~lane/hyperstat/index.html"> a text book</A>,
    /// <A HREF="http://www.stat.umn.edu/~jkuhn/courses/stat3091f/stat3091f.html"> another text book</A>.
    /// <dt>Finally, a good link list <A HREF="http://www.execpc.com/~helberg/statistics.html"> Statistics on the Web</A>.
    /// <p>
    /// </summary>
    public abstract class AbstractDistribution
    {

        #region Local Variables
        private RandomEngine _randomGenerator;

        #endregion

        #region Property
        /// <summary>
        /// the uniform random generator internally used.
        /// </summary>
        public virtual RandomEngine RandomGenerator
        {
            get { return _randomGenerator; }
            set { _randomGenerator = value; }
        }
        #endregion

        #region Constructor

        #endregion

        #region Abstract methods
        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public abstract double NextDouble();

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Constructs and returns a new uniform random number generation engine seeded with the current time.
        /// Currently this is <see cref="Cern.Jet.Random.Engine.MersenneTwister"/>
        /// </summary>
        /// <returns>A new instance of <see cref="Cern.Jet.Random.Engine.MersenneTwister"/></returns>
        public static RandomEngine MakeDefaultGenerator()
        {
            return Cern.Jet.Random.Engine.RandomEngine.MakeDefault();
        }

        /// <summary>
        /// Equivalent to <see cref="NextDouble()"/>.
        /// This has the effect that distributions can now be used as function objects, returning a random number upon function evaluation.
        /// </summary>
        /// <param name="dummy"></param>
        /// <returns></returns>
        public virtual double Apply(double dummy)
        {
            return NextDouble();
        }

        /// <summary>
        /// Equivalent to <see cref="NextInt()"/>.
        /// This has the effect that distributions can now be used as function objects, returning a random number upon function evaluation.
        /// </summary>
        /// <param name="dummy"></param>
        /// <returns></returns>
        public virtual int Apply(int dummy)
        {
            return NextInt();
        }

        /// <summary>
        /// Returns a deep copy of the receiver; the copy will produce identical sequences.
        /// After this call has returned, the copy and the receiver have equal but separate state.
        /// </summary>
        /// <returns>a copy of the receiver.</returns>
        public virtual Object Clone()
        {
            AbstractDistribution copy = (AbstractDistribution)base.MemberwiseClone();
            if (this._randomGenerator != null) copy.RandomGenerator = (RandomEngine)this._randomGenerator.Clone();
            return copy;
        }

        /// <summary>
        /// Returns a random number from the distribution; returns <see cref="System.Math.Round(nextDouble())"/>.
        /// Override this method if necessary.
        /// </summary>
        /// <returns></returns>
        public virtual int NextInt()
        {
            return (int)System.Math.Round(NextDouble());
        }
        #endregion

        #region Local Private Methods

        #endregion

    }
}
