// <copyright file="Formatter.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
    using System;
    using System.Text;

namespace Cern.Colt.Matrix.ObjectAlgorithms
{
    using Implementation;
    using Cern.Colt.Matrix;

    public class Formatter : AbstractFormatter
    {
        /// <summary>
        /// Constructs and returns a matrix formatter with alignment <i>LEFT</i>.
        /// </summary>
        public Formatter(): this(LEFT)
        {
            
        }

        /// <summary>
        /// Constructs and returns a matrix formatter.
        /// </summary>
        /// <param name="alignment">the given alignment used to align a column.</param>
        public Formatter(String alignment)
        {
            SetAlignment(alignment);
        }

        /// <summary>
        /// Converts a given cell to a String; no alignment considered.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected String Form(AbstractMatrix1D matrix, int index)
        {
            return this.Form((ObjectMatrix1D)matrix, index);
        }

        /// <summary>
        /// Converts a given cell to a String; no alignment considered.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected String Form(ObjectMatrix1D matrix, int index)
        {
            Object value = matrix[index];
            if (value == null) return "";
            return value.ToString();
        }

        /// <summary>
        /// Returns a string representations of all cells; no alignment considered.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        protected override String[][] Format(AbstractMatrix2D matrix)
        {
            return this.Format((ObjectMatrix2D)matrix);
        }

        /// <summary>
        /// Returns a string representations of all cells; no alignment considered.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        protected String[][] Format(ObjectMatrix2D matrix)
        {
            String[][] strings = new String[matrix.Rows][];
            strings = strings.Initialize(matrix.Rows, matrix.Columns);
            for (int row = matrix.Rows; --row >= 0;) strings[row] = FormatRow(matrix.ViewRow(row));
            return strings;
        }

        /// <summary>
        /// Returns a string <i>s</i> such that <i>Object[] m = s</i> is a legal C# statement.
        /// </summary>
        /// <param name="matrix">the matrix to format.</param>
        /// <returns></returns>
        public String ToSourceCode(ObjectMatrix1D matrix)
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
        public String ToSourceCode(ObjectMatrix2D matrix)
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
        public String ToSourceCode(ObjectMatrix3D matrix)
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
        /// <param name="matrix">the matrix to convert.</param>
        /// <returns></returns>
        protected override String ToString(AbstractMatrix2D matrix)
        {
            return this.ToString((ObjectMatrix2D)matrix);
        }

        /// <summary>
        /// Returns a string representation of the given matrix.
        /// </summary>
        /// <param name="matrix">the matrix to convert.</param>
        /// <returns></returns>
        public String ToString(ObjectMatrix1D matrix)
        {
            ObjectMatrix2D easy = matrix.Like2D(1, matrix.Size);
            easy.ViewRow(0).Assign(matrix);
            return ToString(easy);
        }

        /// <summary>
        /// Returns a string representation of the given matrix.
        /// </summary>
        /// <param name="matrix">the matrix to convert.</param>
        /// <returns></returns>
        public String ToString(ObjectMatrix2D matrix)
        {
            return base.ToString(matrix);
        }

        /// <summary>
        /// Returns a string representation of the given matrix.
        /// </summary>
        /// <param name="matrix">the matrix to convert.</param>
        /// <returns></returns>
        public String ToString(ObjectMatrix3D matrix)
        {
            StringBuilder buf = new StringBuilder();
            Boolean oldPrintShape = this.printShape;
            this.printShape = false;
            for (int slice = 0; slice < matrix.Slices; slice++)
            {
                if (slice != 0) buf.Append(sliceSeparator);
                buf.Append(ToString(matrix.ViewSlice(slice)));
            }
            this.printShape = oldPrintShape;
            if (printShape) buf.Insert(0, Shape(matrix) + "\n");
            return buf.ToString();
        }

        /// <summary>
        /// Returns a string representation of the given matrix with axis as well as rows and columns labeled.
        /// Pass <i>null</i> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.
        /// </summary>
        /// <param name="matrix">The matrix to format.</param>
        /// <param name="rowNames">The headers of all rows (to be put to the left of the matrix).</param>
        /// <param name="columnNames">The headers of all columns (to be put to above the matrix).</param>
        /// <param name="rowAxisName">The label of the y-axis.</param>
        /// <param name="columnAxisName">The label of the x-axis.</param>
        /// <param name="title">The overall title of the matrix to be formatted.</param>
        /// <returns>the matrix converted to a string.</returns>
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

            int height = r + System.Math.Max(rows, rowAxisName == null ? 0 : rowAxisName.Length);
            int width = c + columns;

            // make larger matrix holding original matrix and naming strings
            ObjectMatrix2D titleMatrix = matrix.Like(height, width);

            // insert original matrix into larger matrix
            titleMatrix.ViewPart(r, c, rows, columns).Assign(matrix);

            // insert column axis name in leading row
            if (r > 0) titleMatrix.ViewRow(0).ViewPart(c, columns).Assign(columnNames);

            // insert row axis name in leading column
            if (rowAxisName != null)
            {
                String[] rowAxisStrings = new String[rowAxisName.Length];
                for (int i = rowAxisName.Length; --i >= 0;) rowAxisStrings[i] = rowAxisName.Substring(i, i + 1);
                titleMatrix.ViewColumn(0).ViewPart(r, rowAxisName.Length).Assign(rowAxisStrings);
            }
            // insert row names in next leading columns
            if (rowNames != null) titleMatrix.ViewColumn(c - 2).ViewPart(r, rows).Assign(rowNames);

            // insert vertical "---------" separator line in next leading column
            if (c > 0) titleMatrix.ViewColumn(c - 2 + 1).ViewPart(0, rows + r).Assign("|");

            // convert the large matrix to a string
            Boolean oldPrintShape = this.printShape;
            this.printShape = false;
            String str = ToString(titleMatrix);
            this.printShape = oldPrintShape;

            // insert horizontal "--------------" separator line
            StringBuilder total = new StringBuilder(str);
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
        public String ToTitleString(ObjectMatrix3D matrix, String[] sliceNames, String[] rowNames, String[] columnNames, String sliceAxisName, String rowAxisName, String columnAxisName, String title)
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
