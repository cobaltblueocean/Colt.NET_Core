// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Formatter.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using Implementation;

    /// <summary>
    /// Flexible, well human readable matrix print formatting; By default decimal point aligned.
    /// </summary>
    public class Formatter : AbstractFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Formatter"/> class with format <i>"%G"</i>.
        /// </summary>
        public Formatter()
        {
            formatString = "G"; //"%G"
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
        public string[][] Format(IDoubleMatrix2D matrix)
        {
            var strings = new string[matrix.Rows][];
            for (int row = matrix.Rows; --row >= 0;) strings[row] = FormatRow(matrix.ViewRow(row));
            return strings;
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
        protected override string[][] Format<T>(IMatrix2D<T> matrix)
        {
            return Format((IDoubleMatrix2D)matrix);
        }

        /// <summary>
        /// Converts a given cell to a String; no alignment considered.
        /// <summary>
        protected String Form(IDoubleMatrix1D matrix, int index, Former formatter)
        {
            return formatter.form(matrix[index]);
        }

        /// <summary>
        /// Converts a given cell to a String; no alignment considered.
        /// <summary>
        protected override String Form<T>(IMatrix1D<T> matrix, int index, Former formatter)
        {
            return this.Form((IDoubleMatrix1D)matrix, index, formatter);
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
        /// Returns a string <i>s</i> such that <i>Object[] m = s</i> is a legal C# statement.
        /// </summary>
        /// <param name="matrix">the matrix to format.</param>
        /// <returns></returns>
        public String ToSourceCode(IDoubleMatrix1D matrix)
        {
            Formatter copy = (Formatter)this.Clone();
            copy.SetPrintShape(false);
            copy.SetColumnSeparator(", ");
            String lead = "{";
            String trail = "};";
            return lead + copy.ToString(matrix) + trail;
        }

        /// <summary>
        /// Returns a string <i>s</i> such that <i>Object[] m = s</i> is a legal C# statement.
        /// </summary>
        /// <param name="matrix">the matrix to format.</param>
        /// <returns></returns>
        public String ToSourceCode(IDoubleMatrix2D matrix)
        {
            Formatter copy = (Formatter)this.Clone();
            String b3 = Blanks(3);
            copy.SetPrintShape(false);
            copy.SetColumnSeparator(", ");
            copy.SetRowSeparator("},\n" + b3 + "{");
            String lead = "{\n" + b3 + "{";
            String trail = "}\n};";
            return lead + copy.ToString(matrix) + trail;
        }

        /// <summary>
        /// Returns a string <i>s</i> such that <i>Object[] m = s</i> is a legal C# statement.
        /// </summary>
        /// <param name="matrix">the matrix to format.</param>
        /// <returns></returns>
        public String ToSourceCode(DoubleMatrix3D matrix)
        {
            Formatter copy = (Formatter)this.Clone();
            String b3 = Blanks(3);
            String b6 = Blanks(6);
            copy.SetPrintShape(false);
            copy.SetColumnSeparator(", ");
            copy.SetRowSeparator("},\n" + b6 + "{");
            copy.SetSliceSeparator("}\n" + b3 + "},\n" + b3 + "{\n" + b6 + "{");
            String lead = "{\n" + b3 + "{\n" + b6 + "{";
            String trail = "}\n" + b3 + "}\n}";
            return lead + copy.ToString(matrix) + trail;
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
        public string ToString(IDoubleMatrix1D matrix)
        {
            var easy = matrix.Like2D(1, matrix.Size);
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
        public string ToString(IDoubleMatrix2D matrix)
        {
            return base.ToString(matrix);
        }

        /// <summary>
        /// Returns a string representation of the given matrix.
        /// </summary>
        /// <param name="matrix">the matrix to convert.</param>
        /// <returns>A string representation of the given matrix.</returns>
        public String ToString(IDoubleMatrix3D matrix)
        {
            var buf = new StringBuilder();
            Boolean oldPrintShape = this.printShape;
            this.printShape = false;
            for (int slice = 0; slice < matrix.Slices; slice++)
            {
                if (slice != 0) buf.Append(sliceSeparator);
                buf.Append(ToString((IDoubleMatrix2D)matrix.ViewSlice(slice)));
            }
            this.printShape = oldPrintShape;
            if (printShape) buf.Insert(0, Shape(matrix) + "\n");
            return buf.ToString();
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
        protected override string ToString<T>(IMatrix2D<T> matrix)
        {
            return this.ToString((IDoubleMatrix2D)matrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="rowNames"></param>
        /// <param name="columnNames"></param>
        /// <param name="rowAxisName"></param>
        /// <param name="columnAxisName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        protected String ToTitleString(IDoubleMatrix2D matrix, String[] rowNames, String[] columnNames, String rowAxisName, String columnAxisName, String title)
        {
            if (matrix.Size == 0) return "Empty matrix";
            String[][] s = Format(matrix);
            //String oldAlignment = this.alignment;
            //this.alignment = DECIMAL;
            Align(s);
            //this.alignment = oldAlignment;
            return new Cern.Colt.Matrix.ObjectAlgorithms.Formatter().ToTitleString(Cern.Colt.Matrix.ObjectFactory2D.Dense.Make(s), rowNames, columnNames, rowAxisName, columnAxisName, title);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="matrix"></param>
        ///// <param name="rowNames"></param>
        ///// <param name="columnNames"></param>
        ///// <param name="rowAxisName"></param>
        ///// <param name="columnAxisName"></param>
        ///// <param name="title"></param>
        ///// <returns></returns>
        //public String ToTitleString(ObjectMatrix2D matrix, String[] rowNames, String[] columnNames, String rowAxisName, String columnAxisName, String title)
        //{
        //    if (matrix.Size == 0) return "Empty matrix";
        //    String oldFormat = this.formatString;
        //    this.formatString = LEFT;

        //    int rows = matrix.Rows;
        //    int columns = matrix.Columns;

        //    // determine how many rows and columns are needed
        //    int r = 0;
        //    int c = 0;
        //    r += (columnNames == null ? 0 : 1);
        //    c += (rowNames == null ? 0 : 1);
        //    c += (rowAxisName == null ? 0 : 1);
        //    c += (rowNames != null || rowAxisName != null ? 1 : 0);

        //    int height = r + System.Math.Max(rows, rowAxisName == null ? 0 : rowAxisName.Length);
        //    int width = c + columns;

        //    // make larger matrix holding original matrix and naming strings
        //    Cern.Colt.Matrix.ObjectMatrix2D titleMatrix = matrix.Like(height, width);

        //    // insert original matrix into larger matrix
        //    titleMatrix.ViewPart(r, c, rows, columns).Assign(matrix);

        //    // insert column axis name in leading row
        //    if (r > 0) titleMatrix.ViewRow(0).ViewPart(c, columns).Assign(columnNames);

        //    // insert row axis name in leading column
        //    if (rowAxisName != null)
        //    {
        //        String[] rowAxisStrings = new String[rowAxisName.Length];
        //        for (int i = rowAxisName.Length; --i >= 0;) rowAxisStrings[i] = rowAxisName.Substring(i, i + 1);
        //        titleMatrix.ViewColumn(0).ViewPart(r, rowAxisName.Length).Assign(rowAxisStrings);
        //    }
        //    // insert row names in next leading columns
        //    if (rowNames != null) titleMatrix.ViewColumn(c - 2).ViewPart(r, rows).Assign(rowNames);

        //    // insert vertical "---------" separator line in next leading column
        //    if (c > 0) titleMatrix.ViewColumn(c - 2 + 1).ViewPart(0, rows + r).Assign("|");

        //    // convert the large matrix to a string
        //    Boolean oldPrintShape = this.printShape;
        //    this.printShape = false;
        //    String str = ToString(titleMatrix);
        //    this.printShape = oldPrintShape;

        //    // insert horizontal "--------------" separator line
        //    var total = new StringBuilder(str);
        //    if (columnNames != null)
        //    {
        //        int i = str.IndexOf(rowSeparator);
        //        total.Insert(i + 1, Repeat('-', i) + rowSeparator);
        //    }
        //    else if (columnAxisName != null)
        //    {
        //        int i = str.IndexOf(rowSeparator);
        //        total.Insert(0, Repeat('-', i) + rowSeparator);
        //    }

        //    // insert line for column axis name
        //    if (columnAxisName != null)
        //    {
        //        int j = 0;
        //        if (c > 0) j = str.IndexOf('|');
        //        String s = Blanks(j);
        //        if (c > 0) s = s + "| ";
        //        s = s + columnAxisName + "\n";
        //        total.Insert(0, s);
        //    }

        //    // insert title
        //    if (title != null) total.Insert(0, title + "\n");

        //    this.formatString = oldFormat;

        //    return total.ToString();
        //}

        /// <summary>
        /// Same as <i>toTitleString</i> except that additionally statistical aggregates (mean, median, sum, etcd) of rows and columns are printed.
        /// Pass <i>null</i> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.
        /// </summary>
        /// <param name="matrix">The matrix to format.</param>
        /// <param name="rowNames">The headers of all rows (to be put to the left of the matrix).</param>
        /// <param name="columnNames">The headers of all columns (to be put to above the matrix).</param>
        /// <param name="rowAxisName">The label of the y-axis.</param>
        /// <param name="columnAxisName">The label of the x-axis.</param>
        /// <param name="title">The overall title of the matrix to be formatted.</param>
        /// <param name="aggr">the aggregation functions to be applied to columns and rows.</param>
        /// <returns>the matrix converted to a string.</returns>
        /// <see cref="Hep.Aida.Bin.BinFunction1D"/>
        /// <see cref="Hep.Aida.Bin.BinFunctions1D"/>
        public String ToTitleString(IDoubleMatrix2D matrix, String[] rowNames, String[] columnNames, String rowAxisName, String columnAxisName, String title, Hep.Aida.Bin.BinFunction1D[] aggr)
        {
            if (matrix.Size == 0) return "Empty matrix";
            if (aggr == null || aggr.Length == 0) return ToTitleString(matrix, rowNames, columnNames, rowAxisName, columnAxisName, title);

            var rowStats = matrix.Like(matrix.Rows, aggr.Length); // hold row aggregations
            var colStats = matrix.Like(aggr.Length, matrix.Columns); // hold column aggregations

            Cern.Colt.Matrix.DoubleAlgorithms.Statistics.Aggregate(matrix, aggr, colStats); // aggregate an entire column at a time
            Cern.Colt.Matrix.DoubleAlgorithms.Statistics.Aggregate(matrix.ViewDice(), aggr, rowStats.ViewDice()); // aggregate an entire row at a time

            // turn into strings
            // tmp holds "matrix" plus "colStats" below (needed so that numbers in a columns can be decimal point aligned)
            var tmp = matrix.Like(matrix.Rows + aggr.Length, matrix.Columns);
            tmp.ViewPart(0, 0, matrix.Rows, matrix.Columns).Assign(matrix);
            tmp.ViewPart(matrix.Rows, 0, aggr.Length, matrix.Columns).Assign(colStats);

            String[][] s1 = Format(tmp); Align(s1);
            String[][] s2 = Format(rowStats); Align(s2);

            // copy strings into a large matrix holding the source matrix and all aggregations
            Cern.Colt.Matrix.ObjectMatrix2D allStats = Cern.Colt.Matrix.ObjectFactory2D.Dense.Make(matrix.Rows + aggr.Length, matrix.Columns + aggr.Length + 1);
            allStats.ViewPart(0, 0, matrix.Rows + aggr.Length, matrix.Columns).Assign(s1);
            allStats.ViewColumn(matrix.Columns).Assign("|");
            allStats.ViewPart(0, matrix.Columns + 1, matrix.Rows, aggr.Length).Assign(s2);

            // append a vertical "|" separator plus names of aggregation functions to line holding columnNames
            if (columnNames != null)
            {
                var list = new List<Object>(columnNames);
                list.Add("|");
                list.AddRange(aggr.Select(x => x.Method.Name).ToList());
                columnNames = list.ToStringArray();
            }

            // append names of aggregation functions to line holding rowNames
            if (rowNames != null)
            {
                var list = new List<Object>(rowNames);
                list.AddRange(aggr.Select(x => x.Method.Name).ToList());
                rowNames = list.ToStringArray();
            }

            // turn large matrix into string
            String s = new Cern.Colt.Matrix.ObjectAlgorithms.Formatter().ToTitleString(allStats, rowNames, columnNames, rowAxisName, columnAxisName, title);

            // insert a horizontal "----------------------" separation line above the column stats
            // determine insertion position and line width
            int last = s.Length + 1;
            int secondLast = last;
            int v = System.Math.Max(0, rowAxisName == null ? 0 : rowAxisName.Length - matrix.Rows - aggr.Length);
            for (int k = 0; k < aggr.Length + 1 + v; k++)
            { // scan "aggr.Length+1+v" lines backwards
                secondLast = last;
                last = s.LastIndexOf(rowSeparator, last - 1);
            }
            StringBuilder buf = new StringBuilder(s);
            buf.Insert(secondLast, rowSeparator + Repeat('-', secondLast - last - 1));

            return buf.ToString();
        }

        /// <summary>
        /// Returns a string representation of the given matrix with axis as well as rows and columns labeled.
        /// Pass <i>null</i> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.
        /// </summary>
        /// <param name="matrix">The matrix to format.</param>
        /// <param name="sliceNames">The headers of all slices (to be put above each slice).</param>
        /// <param name="rowNames">The headers of all rows (to be put to the left of the matrix).</param>
        /// <param name="columnNames">The headers of all columns (to be put to above the matrix).</param>
        /// <param name="sliceAxisName">The label of the z-axis (to be put above each slice).</param>
        /// <param name="rowAxisName">The label of the y-axis.</param>
        /// <param name="columnAxisName">The label of the x-axis.</param>
        /// <param name="title">The overall title of the matrix to be formatted.</param>
        /// <param name="aggr">the aggregation functions to be applied to columns, rows.</param>
        /// <returns>the matrix converted to a string.</returns>
        /// <see cref="Hep.Aida.Bin.BinFunction1D"/>
        /// <see cref="Hep.Aida.Bin.BinFunctions1D"/>
        public String ToTitleString(IDoubleMatrix3D matrix, String[] sliceNames, String[] rowNames, String[] columnNames, String sliceAxisName, String rowAxisName, String columnAxisName, String title, Hep.Aida.Bin.BinFunction1D[] aggr)
        {
            if (matrix.Size == 0) return "Empty matrix";
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < matrix.Slices; i++)
            {
                if (i != 0) buf.Append(sliceSeparator);
                buf.Append(ToTitleString(matrix.ViewSlice(i), rowNames, columnNames, rowAxisName, columnAxisName, title + "\n" + sliceAxisName + "=" + sliceNames[i], aggr));
            }
            return buf.ToString();
        }

        /// <summary>
        /// Returns a string representation of the given matrix with axis as well as rows and columns labeled.
        /// Pass <i>null</i> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.
        /// </summary>
        /// <param name="matrix">The matrix to format.</param>
        /// <param name="sliceNames">The headers of all slices (to be put above each slice).</param>
        /// <param name="rowNames">The headers of all rows (to be put to the left of the matrix).</param>
        /// <param name="columnNames">The headers of all columns (to be put to above the matrix).</param>
        /// <param name="sliceAxisName">The label of the z-axis (to be put above each slice).</param>
        /// <param name="rowAxisName">The label of the y-axis.</param>
        /// <param name="columnAxisName">The label of the x-axis.</param>
        /// <param name="title">The overall title of the matrix to be formatted.</param>
        /// <returns>the matrix converted to a string.</returns>
        private String XToTitleString(IDoubleMatrix3D matrix, String[] sliceNames, String[] rowNames, String[] columnNames, String sliceAxisName, String rowAxisName, String columnAxisName, String title)
        {
            if (matrix.Size == 0) return "Empty matrix";
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < matrix.Slices; i++)
            {
                if (i != 0) buf.Append(sliceSeparator);
                buf.Append(ToTitleString(matrix.ViewSlice(i), rowNames, columnNames, rowAxisName, columnAxisName, title + "\n" + sliceAxisName + "=" + sliceNames[i]));
            }
            return buf.ToString();
        }
    }
}
