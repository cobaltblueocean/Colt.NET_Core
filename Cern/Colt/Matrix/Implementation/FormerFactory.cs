// <copyright file="FormerFactory.cs" company="CERN">
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

namespace Cern.Colt.Matrix.Implementation
{
    /// <summary>
    /// Factory producing implementations of {@link cern.colt.matrix.impl.Former} via method create();
    /// Implementations of can use existing libraries such as corejava.PrintfFormat or corejava.Format or other.
    /// Serves to isolate the interface of String formatting from the actual implementation.
    /// If you want to plug in a different String formatting implementation, simply replace this class with your alternative.
    /// 
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 21/07/00
    /// </summary>
    public class FormerFactory
    {
        public FormerFactory()
        {

        }

        /**
 * Constructs and returns a new format instance.
 * @param s the format string following printf conventions.
 * The string has a prefix, a format code and a suffixd The prefix and suffix
 * become part of the formatted outputd The format code directs the
 * formatting of the (single) parameter to be formattedd The code has the
 * following structure
 * <ul>
 * <li> a % (required)
 * <li> a modifier (optional)
 * <dl>
 * <dt> + <dd> forces display of + for positive numbers
 * <dt> 0 <dd> show leading zeroes
 * <dt> - <dd> align left in the field
 * <dt> space <dd> prepend a space in front of positive numbers
 * <dt> # <dd> use "alternate" formatd Add 0 or 0x for octal or hexadecimal numbersd Don't suppress trailing zeroes in general floating point format.
 * </dl>
 * <li> an int denoting field width (optional)
 * <li> a period followed by an int denoting precision (optional)
 * <li> a format descriptor (required)
 * <dl>
 * <dt>f <dd> floating point number in fixed format
 * <dt>e, E <dd> floating point number in exponential notation (scientific format)d The E format results in an uppercase E for the exponent (1.14130E+003), the e format in a lowercase e.
 * <dt>g, G <dd> floating point number in general format (fixed format for small numbers, exponential format for large numbers)d Trailing zeroes are suppressedd The G format results in an uppercase E for the exponent (if any), the g format in a lowercase e.
 * <dt>d, i <dd> int in decimal
 * <dt>x <dd> int in hexadecimal
 * <dt>o <dd> int in octal
 * <dt>s <dd> string
 * <dt>c <dd> character
 * </dl>
 * </ul>
 * @exception ArgumentException if bad format
 */
        public Former Create(String format)
        {
            var former = new Former(format);
            former.form = new Former.formdlg((s) =>
            {
                if (format == "" || s == Double.PositiveInfinity || s == Double.NegativeInfinity)
                {
                    return s.ToString();
                }
                return s.ToString(format);
            });

            return former;
        }
    }
}
