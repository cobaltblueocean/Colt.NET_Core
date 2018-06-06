namespace Cern.Colt.Matrix.ObjectAlgorithms
{
    using System;
    using System.Text;
    using Implementation;
    using Cern.Colt.Matrix;

    public class Formatter : AbstractFormatter
    {
        /**
 * Constructs and returns a matrix formatter with alignment <i>LEFT</i>.
 */
        public Formatter(): this(LEFT)
        {
            
        }
        /**
         * Constructs and returns a matrix formatter.
         * @param alignment the given alignment used to align a column.
         */
        public Formatter(String alignment)
        {
            SetAlignment(alignment);
        }
        /**
         * Converts a given cell to a String; no alignment considered.
         */
        protected String Form(AbstractMatrix1D matrix, int index)
        {
            return this.Form((ObjectMatrix1D)matrix, index);
        }
        /**
         * Converts a given cell to a String; no alignment considered.
         */
        protected String Form(ObjectMatrix1D matrix, int index)
        {
            Object value = matrix.get(index);
            if (value == null) return "";
            return value.ToString();
        }
        /**
         * Returns a string representations of all cells; no alignment considered.
         */
        protected override String[][] Format(AbstractMatrix2D matrix)
        {
            return this.Format((ObjectMatrix2D)matrix);
        }
        /**
         * Returns a string representations of all cells; no alignment considered.
         */
        protected String[][] Format(ObjectMatrix2D matrix)
        {
            String[][] strings = new String[matrix.Rows][];
            strings = strings.Initialize(matrix.Rows, matrix.Columns);
            for (int row = matrix.Rows; --row >= 0;) strings[row] = formatRow(matrix.ViewRow(row));
            return strings;
        }
        /**
         * Returns a string <i>s</i> such that <i>Object[] m = s</i> is a legal Java statement.
         * @param matrix the matrix to format.
         */
        public String ToSourceCode(ObjectMatrix1D matrix)
        {
            Formatter copy = (Formatter)this.Clone();
            copy.SetPrintShape(false);
            copy.SetColumnSeparator(", ");
            String lead = "{";
            String trail = "};";
            return lead + copy.ToString(matrix) + trail;
        }
        /**
         * Returns a string <i>s</i> such that <i>Object[] m = s</i> is a legal Java statement.
         * @param matrix the matrix to format.
         */
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
        /**
         * Returns a string <i>s</i> such that <i>Object[] m = s</i> is a legal Java statement.
         * @param matrix the matrix to format.
         */
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
        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */
        protected override String ToString(AbstractMatrix2D matrix)
        {
            return this.ToString((ObjectMatrix2D)matrix);
        }
        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */
        public String ToString(ObjectMatrix1D matrix)
        {
            ObjectMatrix2D easy = matrix.like2D(1, matrix.Size);
            easy.viewRow(0).assign(matrix);
            return ToString(easy);
        }
        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */
        public String ToString(ObjectMatrix2D matrix)
        {
            return base.ToString(matrix);
        }
        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */
        public String ToString(ObjectMatrix3D matrix)
        {
            StringBuilder buf = new StringBuilder();
            Boolean oldPrintShape = this.printShape;
            this.printShape = false;
            for (int slice = 0; slice < matrix.slices(); slice++)
            {
                if (slice != 0) buf.Append(sliceSeparator);
                buf.Append(ToString(matrix.viewSlice(slice)));
            }
            this.printShape = oldPrintShape;
            if (printShape) buf.Insert(0, Shape(matrix) + "\n");
            return buf.ToString();
        }
        /**
         * Returns a string representation of the given matrix with axis as well as rows and columns labeled.
         * Pass <i>null</i> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.

         * @param matrix The matrix to format.
         * @param rowNames The headers of all rows (to be put to the left of the matrix).
         * @param columnNames The headers of all columns (to be put to above the matrix).
         * @param rowAxisName The label of the y-axis.
         * @param columnAxisName The label of the x-axis.
         * @param title The overall title of the matrix to be formatted.
         * @return the matrix converted to a string.
*/
        public String ToTitleString(ObjectMatrix2D matrix, String[] rowNames, String[] columnNames, String rowAxisName, String columnAxisName, String title)
        {
            if (matrix.Size == 0) return "Empty matrix";
            String oldFormat = this.Format;
            this.Format = LEFT;

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
            titleMatrix.viewPart(r, c, rows, columns).assign(matrix);

            // insert column axis name in leading row
            if (r > 0) titleMatrix.viewRow(0).viewPart(c, columns).assign(columnNames);

            // insert row axis name in leading column
            if (rowAxisName != null)
            {
                String[] rowAxisStrings = new String[rowAxisName.Length];
                for (int i = rowAxisName.Length; --i >= 0;) rowAxisStrings[i] = rowAxisName.Substring(i, i + 1);
                titleMatrix.viewColumn(0).viewPart(r, rowAxisName.Length).assign(rowAxisStrings);
            }
            // insert row names in next leading columns
            if (rowNames != null) titleMatrix.viewColumn(c - 2).viewPart(r, rows).assign(rowNames);

            // insert vertical "---------" separator line in next leading column
            if (c > 0) titleMatrix.viewColumn(c - 2 + 1).viewPart(0, rows + r).assign("|");

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

            this.format = oldFormat;

            return total.ToString();
        }
        /**
         * Returns a string representation of the given matrix with axis as well as rows and columns labeled.
         * Pass <i>null</i> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.

         * @param matrix The matrix to format.
         * @param sliceNames The headers of all slices (to be put above each slice).
         * @param rowNames The headers of all rows (to be put to the left of the matrix).
         * @param columnNames The headers of all columns (to be put to above the matrix).
         * @param sliceAxisName The label of the z-axis (to be put above each slice).
         * @param rowAxisName The label of the y-axis.
         * @param columnAxisName The label of the x-axis.
         * @param title The overall title of the matrix to be formatted.
         * @return the matrix converted to a string.
*/
        public String ToTitleString(ObjectMatrix3D matrix, String[] sliceNames, String[] rowNames, String[] columnNames, String sliceAxisName, String rowAxisName, String columnAxisName, String title)
        {
            if (matrix.Size == 0) return "Empty matrix";
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < matrix.slices(); i++)
            {
                if (i != 0) buf.Append(sliceSeparator);
                buf.Append(ToTitleString(matrix.viewSlice(i), rowNames, columnNames, rowAxisName, columnAxisName, title + "\n" + sliceAxisName + "=" + sliceNames[i]));
            }
            return buf.ToString();
        }
    }
}
