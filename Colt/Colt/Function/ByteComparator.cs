using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Function
{

    /// <summary>
    /// A comparison function which imposes a <i>total ordering</i> on some
    /// collection of elements.  Comparators can be passed to a sort method (such as
    /// <tt>cern.colt.Sorting.quickSort</tt>) to allow precise control over the sort order.<p>
    ///
    /// Note: It is generally a good idea for comparators to implement
    /// <tt>java.io.Serializable</tt>, as they may be used as ordering methods in
    /// serializable data structures.  In
    /// order for the data structure to serialize successfully, the comparator (if
    /// provided) must implement <tt>Serializable</tt>.<p>
    /// </summary>
    public interface ByteComparator
    {

        /// <summary>
        /// Compares its two arguments for order.  Returns a negative integer,
        /// zero, or a positive integer as the first argument is less than, equal
        /// to, or greater than the second.
        /// </summary>
        /// <param name="o1">
        /// The first argument.
        /// </param>
        /// <param name="o2">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The comparison result.
        /// </returns>
        int Compare(byte o1, byte o2);

        /// <summary>
        /// Indicates whether some other object is &quot;equal to&quot; this
        /// Comparator.  This method must obey the general contract of
        /// <tt>Object.Equals(Object)</tt>.  Additionally, this method can return
        /// <tt>true</tt> <i>only</i> if the specified Object is also a comparator
        /// and it imposes the same ordering as this comparator.  Thus,
        /// <code>comp1.equals(comp2)</code> implies that <tt>sgn(comp1.compare(o1,
        /// o2))==sgn(comp2.compare(o1, o2))</tt> for every element
        /// <tt>o1</tt> and <tt>o2</tt>.<p>
        /// </summary>
        /// <param name="obj">the reference object with which to compare.</param>
        /// <returns>true if the specified object is also a comparator and it imposes the same ordering as this comparator.</returns>
        Boolean Equals(Object obj);
    }
}
