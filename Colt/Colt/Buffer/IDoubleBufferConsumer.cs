using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Buffer
{
    /// <summary>
    /// Target of a streaming <tt>DoubleBuffer</tt> into which data is flushed upon buffer overflow.
    /// </summary>
    public interface IDoubleBufferConsumer
    {
        /// <summary>
        /// Adds all elements of the specified list to the receiver.
        /// </summary>
        /// <param name="list">the list of which all elements shall be added.</param>
        void AddAllOf(List<Double> list);
    }
}
