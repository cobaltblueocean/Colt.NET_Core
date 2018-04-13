using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Function
{
    /// <summary>
    /// Interface that represents a procedure object: a procedure that takes 
    /// a single argument and does not return a value. 
    /// </summary>
    public abstract class ByteProcedure
    {
        /// <summary>
        /// Applies a procedure to an argument.
        /// Optionally can return a boolean flag to inform the object calling the procedure.
        /// </summary>
        /// <param name="element">element passed to the procedure.</param>
        /// <returns>a flag  to inform the object calling the procedure.</returns>
        /// <example>
        /// Example: forEach() methods often use procedure objects.
        /// To signal to a forEach() method whether iteration should continue normally or terminate (because for example a matching element has been found),
        /// a procedure can return <tt>false</tt> to indicate termination and <tt>true</tt> to indicate continuation.
        /// </example>
        public delegate Boolean Apply(Byte element);
    }
}
