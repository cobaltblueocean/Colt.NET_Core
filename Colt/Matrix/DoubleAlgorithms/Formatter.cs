// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Formatter.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Flexible, well human readable matrix print formatting; By default decimal point aligned.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Colt.Matrix.DoubleAlgorithms
{
    using Implementation;

    /// <summary>
    /// Flexible, well human readable matrix print formatting; By default decimal point aligned.
    /// </summary>
    public class Formatter : AbstractFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Formatter"/> class with format <tt>"%G"</tt>.
        /// </summary>
        public Formatter()
        {
            formatString = "%G";
            alignmentString = DECIMAL;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Formatter"/> class.
        /// </summary>
        /// <param name="format">
        /// The given format used to convert a single cell value..
        /// </param>
        public Formatter(string format)
        {
            formatString = format;
            alignmentString = DECIMAL;
        }

        /// <summary>
        /// Returns a string representations of all cells; no alignment considered.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// A string representation of all cells.
        /// </returns>
        public string[][] Format(DoubleMatrix2D matrix)
        {
            var strings = new string[matrix.Rows][];
            for (int row = matrix.Rows; --row >= 0;) strings[row] = formatRow(matrix.ViewRow(row));
            return strings;
        }

        /// <summary>
        /// Returns a string representation of the given matrix.
        /// </summary>
        /// <param name="matrix">
        /// The matrix to convert.
        /// </param>
        /// <returns>
        /// A string representation of the given matrix.
        /// </returns>
        public string ToString(DoubleMatrix1D matrix)
        {
            DoubleMatrix2D easy = matrix.Like2D(1, matrix.Size());
            easy.ViewRow(0).Assign(matrix);
            return ToString(easy);
        }

        /// <summary>
        /// Returns a string representation of the given matrix.
        /// </summary>
        /// <param name="matrix">
        /// The matrix to convert.
        /// </param>
        /// <returns>
        /// A string representation of the given matrix.
        /// </returns>
        public string ToString(DoubleMatrix2D matrix)
        {
            return base.toString(matrix);
        }

        /// <summary>
        /// Returns a string representations of all cells; no alignment considered.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// A string representations of all cells.
        /// </returns>
        protected override string[][] format(AbstractMatrix2D matrix)
        {
            return Format((DoubleMatrix2D)matrix);
        }

        /// <summary>
        /// Returns the index of the decimal point.
        /// </summary>
        /// <param name="s">
        /// The string.
        /// </param>
        /// <returns>
        /// The index of the decimal point.
        /// </returns>
        protected int indexOfDecimalPoint(string s)
        {
            int i = s.LastIndexOf('.');
            if (i < 0) i = s.LastIndexOf('e');
            if (i < 0) i = s.LastIndexOf('E');
            if (i < 0) i = s.Length;
            return i;
        }

        /// <summary>
        /// Returns the number of characters before the decimal point.
        /// </summary>
        /// <param name="s">
        /// The string.
        /// </param>
        /// <returns>
        /// The number of characters before the decimal point.
        /// </returns>
        protected override int lead(string s)
        {
            if (alignmentString.Equals(DECIMAL)) return indexOfDecimalPoint(s);
            return base.lead(s);
        }

        /// <summary>
        /// Returns a string representation of the given matrix.
        /// </summary>
        /// <param name="matrix">
        /// The matrix to convert.
        /// </param>
        /// <returns>
        /// A string representation of the given matrix.
        /// </returns>
        protected override string toString(AbstractMatrix2D matrix)
        {
            return this.ToString((DoubleMatrix2D)matrix);
        }
    }
}
