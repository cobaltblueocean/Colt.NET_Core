﻿// --------------------------------------------------------------------------------------------------------------------
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

namespace Cern.Colt.Matrix.DoubleAlgorithms
{
    using System;
    using System.Text;
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
            for (int row = matrix.Rows; --row >= 0;) strings[row] = FormatRow(matrix.ViewRow(row));
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
            DoubleMatrix2D easy = matrix.Like2D(1, matrix.Size);
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
            return base.ToString(matrix);
        }

        /// <summary>
        /// Returns a string representation of the given matrix.
        /// </summary>
        /// <param name="matrix">the matrix to convert.</param>
        /// <returns>A string representation of the given matrix.</returns>
        public String ToString(DoubleMatrix3D matrix)
        {
            var buf = new StringBuilder();
            Boolean oldPrintShape = this.printShape;
            this.printShape = false;
            for (int slice = 0; slice < matrix.Slices; slice++)
            {
                if (slice != 0) buf.Append(sliceSeparator);
                buf.Append(ToString((AbstractMatrix2D)matrix.ViewSlice(slice)));
            }
            this.printShape = oldPrintShape;
            if (printShape) buf.Insert(0, Shape(matrix) + "\n");
            return buf.ToString();
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
        protected override string[][] Format(AbstractMatrix2D matrix)
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
        protected int IndexOfDecimalPoint(string s)
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
        protected override int Lead(string s)
        {
            if (alignmentString.Equals(DECIMAL)) return IndexOfDecimalPoint(s);
            return base.Lead(s);
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
        protected override string ToString(AbstractMatrix2D matrix)
        {
            return this.ToString((DoubleMatrix2D)matrix);
        }

        protected String ToTitleString(DoubleMatrix2D matrix, String[] rowNames, String[] columnNames, String rowAxisName, String columnAxisName, String title)
        {
            if (matrix.Size == 0) return "Empty matrix";
            String[][] s = Format(matrix);
            //String oldAlignment = this.alignment;
            //this.alignment = DECIMAL;
            Align(s);
            //this.alignment = oldAlignment;
            return new Cern.Colt.Matrix.DoubleAlgorithms.Formatter().ToTitleString(Cern.Colt.Matrix.ObjectFactory2D.Dense.Make(s), rowNames, columnNames, rowAxisName, columnAxisName, title);
        }

        public String ToTitleString(ObjectMatrix2D matrix, String[] rowNames, String[] columnNames, String rowAxisName, String columnAxisName, String title)
        {
            if (matrix.Size == 0) return "Empty matrix";
            String oldFormat = this.formatString;
            this.formatString = LEFT;

            int rows = matrix.Rows;
            int columns = matrix.Columns;

            // determine how many rows and columns are needed
            int r = 0;
            int c = 0;
            r += (columnNames == null ? 0 : 1);
            c += (rowNames == null ? 0 : 1);
            c += (rowAxisName == null ? 0 : 1);
            c += (rowNames != null || rowAxisName != null ? 1 : 0);

            int height = r + Math.Max(rows, rowAxisName == null ? 0 : rowAxisName.Length);
            int width = c + columns;

            // make larger matrix holding original matrix and naming strings
            Cern.Colt.Matrix.ObjectMatrix2D titleMatrix = matrix.Like(height, width);

            // insert original matrix into larger matrix
            titleMatrix.ViewPart(r, c, rows, columns).Assign(matrix);

            // insert column axis name in leading row
            if (r > 0) titleMatrix.ViewRow(0).viewPart(c, columns).assign(columnNames);

            // insert row axis name in leading column
            if (rowAxisName != null)
            {
                String[] rowAxisStrings = new String[rowAxisName.Length];
                for (int i = rowAxisName.Length; --i >= 0;) rowAxisStrings[i] = rowAxisName.Substring(i, i + 1);
                titleMatrix.ViewColumn(0).viewPart(r, rowAxisName.Length).assign(rowAxisStrings);
            }
            // insert row names in next leading columns
            if (rowNames != null) titleMatrix.ViewColumn(c - 2).viewPart(r, rows).assign(rowNames);

            // insert vertical "---------" separator line in next leading column
            if (c > 0) titleMatrix.ViewColumn(c - 2 + 1).viewPart(0, rows + r).assign("|");

            // convert the large matrix to a string
            Boolean oldPrintShape = this.printShape;
            this.printShape = false;
            String str = ToString(titleMatrix);
            this.printShape = oldPrintShape;

            // insert horizontal "--------------" separator line
            var total = new StringBuilder(str);
            if (columnNames != null)
            {
                int i = str.IndexOf(rowSeparator);
                total.Insert(i + 1, Repeat('-', i) + rowSeparator);
            }
            else if (columnAxisName != null)
            {
                int i = str.IndexOf(rowSeparator);
                total.Insert(0, Repeat('-', i) + rowSeparator);
            }

            // insert line for column axis name
            if (columnAxisName != null)
            {
                int j = 0;
                if (c > 0) j = str.IndexOf('|');
                String s = Blanks(j);
                if (c > 0) s = s + "| ";
                s = s + columnAxisName + "\n";
                total.Insert(0, s);
            }

            // insert title
            if (title != null) total.Insert(0, title + "\n");

            this.formatString = oldFormat;

            return total.ToString();
        }
    }
}
