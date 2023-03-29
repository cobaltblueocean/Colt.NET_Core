// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractFormatter.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Abstract base class for flexible, well human readable matrix print formatting.
//   Value type independent.
//   A single cell is formatted via a format string.
//   Columns can be aligned left, centered, right and by decimal point.
//   <para>A column can be broader than specified by the parameter <tt>minColumnWidth</tt>
//   (because a cell may not fit into that width) but a column is never smaller than
//   <tt>minColumnWidth</tt>. Normally one does not need to specify <tt>minColumnWidth</tt>.
//   Cells in a row are separated by a separator string, similar separators can be set for rows and slices.
//   For more info, see the concrete subclasses.
//   </para>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    using System;
    using System.Text;

    /// <summary>
    /// Abstract base class for flexible, well human readable matrix print formatting.
    /// Value type independent.
    /// A single cell is formatted via a format string.
    /// Columns can be aligned left, centered, right and by decimal point. 
    /// <para>A column can be broader than specified by the parameter <tt>minColumnWidth</tt> 
    /// (because a cell may not fit into that width) but a column is never smaller than 
    /// <tt>minColumnWidth</tt>. Normally one does not need to specify <tt>minColumnWidth</tt>.
    /// Cells in a row are separated by a separator string, similar separators can be set for rows and slices.
    /// For more info, see the concrete subclasses.
    /// </para>
    /// </summary>
    public abstract class AbstractFormatter : PersistentObject
    {
        /// <summary>
        /// The alignment string aligning the cells of a column to the left.
        /// </summary>
        public const string LEFT = "left";

        /// <summary>
        /// The alignment string aligning the cells of a column to its center.
        /// </summary>
        public const string CENTER = "center";

        /// <summary>
        /// The alignment string aligning the cells of a column to the right.
        /// </summary>
        public const string RIGHT = "right";

        /// <summary>
        /// The alignment string aligning the cells of a column to the decimal point.
        /// </summary>
        public const string DECIMAL = "decimal";

        /// <summary>
        /// The default minimum number of characters a column may have; currently <tt>1</tt>.
        /// </summary>
        public const int DEFAULT_MIN_COLUMN_WIDTH = 1;

        /// <summary>
        /// The default string separating any two columns from another; currently <tt>" "</tt>.
        /// </summary>
        public const string DEFAULT_COLUMN_SEPARATOR = " ";

        /// <summary>
        /// The default string separating any two rows from another; currently <tt>"\n"</tt>.
        /// </summary>
        public const string DEFAULT_ROW_SEPARATOR = "\n";

        /// <summary>
        /// The default string separating any two slices from another; currently <tt>"\n\n"</tt>.
        /// </summary>
        public const string DEFAULT_SLICE_SEPARATOR = "\n\n";

        /// <summary>
        /// The default alignment string; currently left.
        /// </summary>
        protected string alignmentString = LEFT;

        /// <summary>
        /// The default format string for formatting a single cell value; currently <tt>"%G"</tt>.
        /// </summary>
        protected string formatString = "G";  //"%G"

        /// <summary>
        /// The default minimum number of characters a column may have; currently <tt>1</tt>.
        /// </summary>
        protected int minColumnWidth = DEFAULT_MIN_COLUMN_WIDTH;

        /// <summary>
        /// The default string separating any two columns from another; currently <tt>" "</tt>.
        /// </summary>
        protected string columnSeparator = DEFAULT_COLUMN_SEPARATOR;

        /// <summary>
        /// The default string separating any two rows from another; currently <tt>"\n"</tt>.
        /// </summary>
        protected string rowSeparator = DEFAULT_ROW_SEPARATOR;

        /// <summary>
        /// The default string separating any two slices from another; currently <tt>"\n\n"</tt>.
        /// </summary>
        protected string sliceSeparator = DEFAULT_SLICE_SEPARATOR;

        /// <summary>
        /// Tells whether String representations are to be preceded with summary of the shape; currently <tt>true</tt>.
        /// </summary>
        protected bool printShape = true;

        protected static FormerFactory factory = new FormerFactory();

        /// <summary>
        /// for efficient String manipulations
        /// </summary>
        private static readonly string[] blanksCache = SetupBlanksCache();

        /// <summary>
        /// Returns a short string representation describing the shape of the matrix.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// A short string representation describing the shape of the matrix.
        /// </returns>
        public static string Shape<T>(IMatrix1D<T> matrix)
        {
            return matrix.Size + " matrix";
        }

        /// <summary>
        /// Returns a short string representation describing the shape of the matrix.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// A short string representation describing the shape of the matrix.
        /// </returns>
        public static string Shape<T>(IMatrix2D<T> matrix)
        {
            return matrix.Rows + " x " + matrix.Columns + " matrix ";
        }

        /// <summary>
        /// Returns a short string representation describing the shape of the matrix.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// A short string representation describing the shape of the matrix.
        /// </returns>
        public static string Shape<T>(IMatrix3D<T> matrix)
        {
            return matrix.Slices + " x " + matrix.Rows + " x " + matrix.Columns + " matrix ";
        }

        /// <summary>
        /// Sets the column alignment (left,center,right,decimal).
        /// </summary>
        /// <param name="align">
        /// The new alignment to be used; must be one of <tt>{LEFT,CENTER,RIGHT,DECIMAL}</tt>.
        /// </param>
        public void SetAlignment(string align)
        {
            this.alignmentString = align;
        }

        /// <summary>
        /// Sets the string separating any two columns from another.
        /// </summary>
        /// <param name="separator">
        /// The new column separator to be used.
        /// </param>
        public void SetColumnSeparator(string separator)
        {
            this.columnSeparator = separator;
        }

        /// <summary>
        /// Sets the way a <i>single</i> cell value is to be formatted.
        /// </summary>
        /// <param name="f">
        /// The new format to be used.
        /// </param>
        public void SetFormat(string f)
        {
            this.formatString = f;
        }

        /// <summary>
        /// Sets the minimum number of characters a column may have.
        /// </summary>
        /// <param name="width">
        /// The new minimum column width to be used.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public void SetMinColumnWidth(int width)
        {
            if (width < 0) throw new ArgumentOutOfRangeException();
            this.minColumnWidth = width;
        }

        /// <summary>
        /// Specifies whether a string representation of a matrix is to be preceded with a summary of its shape
        /// </summary>
        /// <param name="shape">
        /// <tt>true</tt> shape summary is printed, otherwise not printed.
        /// </param>
        public void SetPrintShape(bool shape)
        {
            this.printShape = shape;
        }

        /// <summary>
        /// Sets the string separating any two rows from another.
        /// </summary>
        /// <param name="separator">
        /// The new row separator to be used.
        /// </param>
        public void SetRowSeparator(string separator)
        {
            this.rowSeparator = separator;
        }

        /// <summary>
        /// Sets the string separating any two slices from another.
        /// </summary>
        /// <param name="separator">
        /// The new slice separator to be used.
        /// </param>
        public void SetSliceSeparator(string separator)
        {
            this.sliceSeparator = separator;
        }

        /// <summary>
        /// Cache for faster string processing.
        /// </summary>
        /// <returns>
        /// </returns>
        protected static string[] SetupBlanksCache()
        {
            // Pre-fabricate 40 static strings with 0,1,2,..,39 blanks, for usage within method blanks(length).
            // Now, we don't need to construct and fill them on demand, and garbage collect them again.
            // All 40 strings share the identical char[] array, only with different offset and length --> somewhat smaller static memory footprint
            const int Size = 40;
            var result = new string[Size];
            var buf = new StringBuilder(Size);
            for (int i = Size; --i >= 0; )
                buf.Append(' ');
            var str = buf.ToString();
            for (int i = Size; --i >= 0; )
                result[i] = str.Substring(0, i);
            return result;
        }

        /// <summary>
        /// Modifies the strings in a column of the string matrix to be aligned (left,centered,right,decimal).
        /// </summary>
        /// <param name="strings">
        /// The strings.
        /// </param>
        protected void Align(string[][] strings)
        {
            int rows = strings.Length;
            int columns = 0;
            if (rows > 0) columns = strings[0].Length;

            var maxColWidth = new int[columns];
            int[] maxColLead = null;
            bool isDecimal = alignmentString.Equals(DECIMAL);
            if (isDecimal) maxColLead = new int[columns];

            // for each column, determine alignment parameters
            for (int column = 0; column < columns; column++)
            {
                int maxWidth = minColumnWidth;
                int maxLead = int.MinValue;
                for (int row = 0; row < rows; row++)
                {
                    string s = strings[row][column];
                    maxWidth = Math.Max(maxWidth, s.Length);
                    if (isDecimal) maxLead = Math.Max(maxLead, Lead(s));
                }

                maxColWidth[column] = maxWidth;
                if (isDecimal) maxColLead[column] = maxLead;
            }

            // format each row according to alignment parameters
            for (int row = 0; row < rows; row++)
            {
                AlignRow(strings[row], maxColWidth, maxColLead);
            }
        }

        /// <summary>
        /// Converts a row into a string.
        /// </summary>
        /// <param name="align">
        /// The alignment.
        /// </param>
        /// <returns>
        /// The alignment code.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the alignment is not left, enter, right or decimal.
        /// </exception>
        protected int AlignmentCode(string align)
        {
            // {-1,0,1,2} = {left,centered,right,decimal point}
            if (align.Equals(LEFT)) return -1;
            if (alignmentString.Equals(CENTER)) return 0;
            if (alignmentString.Equals(RIGHT)) return 1;
            if (alignmentString.Equals(DECIMAL)) return 2;
            throw new ArgumentOutOfRangeException(alignmentString, "unknown alignment: " + alignmentString);
        }

        /// <summary>
        /// Modifies the strings the string matrix to be aligned (left,centered,right,decimal).
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="maxColWidth">
        /// The max col width.
        /// </param>
        /// <param name="maxColLead">
        /// The max col lead.
        /// </param>
        /// <exception cref="ApplicationException">
        /// </exception>
        protected void AlignRow(string[] row, int[] maxColWidth, int[] maxColLead)
        {
            StringBuilder s;

            int columns = row.Length;
            for (int column = 0; column < columns; column++)
            {
                s = new StringBuilder();
                string c = row[column];
                if (alignmentString.Equals(RIGHT))
                {
                    s.Append(Blanks(maxColWidth[column] - s.Length));
                    s.Append(c);
                }
                else if (alignmentString.Equals(DECIMAL))
                {
                    s.Append(Blanks(maxColLead[column] - Lead(c)));
                    s.Append(c);
                    s.Append(Blanks(maxColWidth[column] - s.Length));
                }
                else if (alignmentString.Equals(CENTER))
                {
                    s.Append(Blanks((maxColWidth[column] - c.Length) / 2));
                    s.Append(c);
                    s.Append(Blanks(maxColWidth[column] - s.Length));
                }
                else if (alignmentString.Equals(LEFT))
                {
                    s.Append(c);
                    s.Append(Blanks(maxColWidth[column] - s.Length));
                }
                else throw new ApplicationException();

                row[column] = s.ToString();
            }
        }

        /// <summary>
        /// Returns a string with <tt>length</tt> blanks.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// A string with <tt>length</tt> blanks.
        /// </returns>
        protected string Blanks(int length)
        {
            if (length < 0) length = 0;
            if (length < blanksCache.Length) return blanksCache[length];

            var buf = new StringBuilder(length);
            for (int k = 0; k < length; k++)
                buf.Append(' ');
            return buf.ToString();
        }

        /// <summary>
        /// Converts a given cell to a String; no alignment considered.
        /// </summary>
        protected abstract String Form<T>(IMatrix1D<T> matrix, int index, Former formatter);

        /// <summary>
        /// Returns a string representations of all cells; no alignment considered.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// A string representations of all cells.
        /// </returns>
        protected abstract string[][] Format<T>(IMatrix2D<T> matrix);

        /// <summary>
        /// Returns a string representations of all cells; no alignment considered.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// A string representations of all cells.
        /// </returns>
        protected string[] FormatRow<T>(IMatrix1D<T> vector) 
        {
            Former formatter = null;
            formatter = factory.Create(formatString);
            int s = vector.Size;
            String[] strings = new String[s];
            for (int i = 0; i < s; i++)
            {
                strings[i] = Form(vector, i, formatter);
            }
            return strings;
        }

        /// <summary>
        /// Returns the number of characters or the number of characters before the decimal point.
        /// </summary>
        /// <param name="s">
        /// The string.
        /// </param>
        /// <returns>
        /// The number of characters or the number of characters before the decimal point.
        /// </returns>
        protected virtual int Lead(string s)
        {
            return s.Length;
        }

        /// <summary>
        /// Returns a string with the given character repeated <tt>length</tt> times.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// A string with the given character repeated <tt>length</tt> times.
        /// </returns>
        protected string Repeat(char character, int length)
        {
            if (character == ' ') return Blanks(length);
            if (length < 0) length = 0;
            var buf = new StringBuilder(length);
            for (int k = 0; k < length; k++)
                buf.Append(character);
            return buf.ToString();
        }

        /// <summary>
        /// Returns a single string representation of the given string matrix.
        /// </summary>
        /// <param name="strings">
        /// The matrix to be converted to a single string..
        /// </param>
        /// <returns>
        /// A single string representation of the given string matrix.
        /// </returns>
        protected virtual string ToString(string[][] strings)
        {
            int rows = strings.Length;
            int columns = strings.Length <= 0 ? 0 : strings[0].Length;

            var total = new StringBuilder();
            StringBuilder s;
            for (int row = 0; row < rows; row++)
            {
                s = new StringBuilder();
                for (int column = 0; column < columns; column++)
                {
                    s.Append(strings[row][column]);
                    if (column < columns - 1) s.Append(columnSeparator);
                }

                total.Append(s);
                if (row < rows - 1) total.Append(rowSeparator);
            }

            return total.ToString();
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
        protected virtual string ToString<T>(IMatrix2D<T> matrix)
        {
            var strings = this.Format(matrix);
            Align(strings);
            var total = new StringBuilder(ToString(strings));
            if (printShape) total.Insert(0, Shape(matrix) + "\n");
            return total.ToString();
        }
    }
}
