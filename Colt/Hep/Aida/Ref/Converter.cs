using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Matrix;
using Cern.Colt.Matrix.Implementation;
using Cern.Colt.Matrix.DoubleAlgorithms;

namespace Cern.Hep.Aida.Ref
{
    public class Converter
    {
        /// <summary>
        /// Creates a new histogram converter.
        ////
        public Converter() { }
        /// <summary> 
        /// Returns all edges of the given axis.
        //// 
        public double[] Edges(IAxis axis)
        {
            int b = axis.Bins;
            double[] bounds = new double[b + 1];
            for (int i = 0; i < b; i++) bounds[i] = axis.BinLowerEdge(i);
            bounds[b] = axis.UpperEdge;
            return bounds;
        }
        String Form(Cern.Colt.Matrix.Former formatter, double value)
        {
            return formatter.Format(value);
        }
        /// <summary> 
        /// Returns an array[h.XAxis.Bins]; ignoring extra bins.
        //// 
        protected double[] ToArrayErrors(IHistogram1D h)
        {
            int xBins = h.XAxis.Bins;
            double[] array = new double[xBins];
            for (int j = xBins; --j >= 0;)
            {
                array[j] = h.BinError(j);
            }
            return array;
        }
        /// <summary> 
        /// Returns an array[h.XAxis.Bins][h.YAxis.Bins]; ignoring extra bins.
        //// 
        protected double[][] ToArrayErrors(IHistogram2D h)
        {
            int xBins = h.XAxis.Bins;
            int yBins = h.YAxis.Bins;
            double[][] array = new double[xBins][];
            for (int i = yBins; --i >= 0;)
            {
                array[i] = new double[yBins];
                for (int j = xBins; --j >= 0;)
                {
                    array[j][i] = h.BinError(j, i);
                }
            }
            return array;
        }
        /// <summary> 
        /// Returns an array[h.XAxis.Bins]; ignoring extra bins.
        //// 
        protected double[] ToArrayHeights(IHistogram1D h)
        {
            int xBins = h.XAxis.Bins;
            double[] array = new double[xBins];
            for (int j = xBins; --j >= 0;)
            {
                array[j] = h.BinHeight(j);
            }
            return array;
        }
        /// <summary> 
        /// Returns an array[h.XAxis.Bins][h.YAxis.Bins]; ignoring extra bins.
        //// 
        protected double[][] ToArrayHeights(IHistogram2D h)
        {
            int xBins = h.XAxis.Bins;
            int yBins = h.YAxis.Bins;
            //double[][] array = new double[xBins][];
            var array = (new double[xBins, yBins]).ToJagged();
            for (int i = yBins; --i >= 0;)
            {
                //array[i] = new double[yBins];
                for (int j = xBins; --j >= 0;)
                {
                    array[j][i] = h.BinHeight(j, i);
                }
            }
            return array;
        }
        /// <summary> 
        /// Returns an array[h.XAxis.Bins][h.YAxis.Bins][h.ZAxis.Bins]; ignoring extra bins.
        //// 
        protected double[][][] ToArrayHeights(IHistogram3D h)
        {
            int xBins = h.XAxis.Bins;
            int yBins = h.YAxis.Bins;
            int zBins = h.ZAxis.Bins;
            //double[][][] array = new double[xBins][][];
            var array = (new double[xBins, yBins, zBins]).ToJagged();
            for (int j = xBins; --j >= 0;)
            {
                //array[j] = new double[yBins][];
                for (int i = yBins; --i >= 0;)
                {
                    //array[j][i] = new double[zBins];
                    for (int k = zBins; --k >= 0;)
                    {
                        array[j][i][k] = h.BinHeight(j, i, k);
                    }
                }
            }
            return array;
        }
        /// <summary>
        /// Returns a string representation of the specified arrayd  The string
        /// representation consists of a list of the arrays's elements, enclosed in square brackets
        /// (<i>"[]"</i>)d  Adjacent elements are separated by the characters
        /// <i>", "</i> (comma and space).
        /// @return a string representation of the specified array.
        ////
        protected static String ToString(double[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                    buf.Append(", ");
            }
            buf.Append("]");
            return buf.ToString();
        }
        /// <summary> 
        /// Returns a string representation of the given argument.
        //// 
        public String ToString(IAxis axis)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("Range: [" + axis.LowerEdge + "," + axis.UpperEdge + ")");
            buf.Append(", Bins: " + axis.Bins);
            buf.Append(", Bin edges: " + ToString(Edges(axis)) + "\n");
            return buf.ToString();
        }
        /// <summary> 
        /// Returns a string representation of the given argument.
        //// 
        public String ToString(IHistogram1D h)
        {
            String columnAxisName = null; //"X";
            String rowAxisName = null;
            Hep.Aida.Bin.BinFunction1D[] aggr = null; //{Hep.Aida.Bin.BinFunctions1D.sum};
            String format = "G"; //"%G"
            //String format = "%1.2G";

            Cern.Colt.Matrix.Former f = new Cern.Colt.Matrix.Implementation.FormerFactory().Create(format);
            String sep = "\n\r"; //"\n\r"; //System.getProperty("line.separator");
            int[] minMaxBins = h.MinMaxBins;
            String title = h.Title + ":" + sep +
                "   Entries=" + Form(f, h.Entries) + ", ExtraEntries=" + Form(f, h.ExtraEntries) + sep +
                "   Mean=" + Form(f, h.Mean) + ", Rms=" + Form(f, h.Rms) + sep +
                "   MinBinHeight=" + Form(f, h.BinHeight(minMaxBins[0])) + ", MaxBinHeight=" + Form(f, h.BinHeight(minMaxBins[1])) + sep +
                "   Axis: " +
                "Bins=" + Form(f, h.XAxis.Bins) +
                ", Min=" + Form(f, h.XAxis.LowerEdge) +
                ", Max=" + Form(f, h.XAxis.UpperEdge);

            String[] xEdges = new String[h.XAxis.Bins];
            for (int i = 0; i < h.XAxis.Bins; i++) xEdges[i] = Form(f, h.XAxis.BinLowerEdge(i));

            String[] yEdges = new[] { "Data" };

            Cern.Colt.Matrix.DoubleMatrix2D heights = new DenseDoubleMatrix2D(1, h.XAxis.Bins);
            heights.ViewRow(0).Assign(ToArrayHeights(h));
            //Cern.Colt.Matrix.DoubleMatrix2D errors = new Cern.Colt.Matrix.DenseDoubleMatrix2D(1,h.XAxis.Bins);
            //errors.ViewRow(0).Assign(toArrayErrors(h));

            return title + sep +
                "Heights:" + sep +
                new Formatter().ToTitleString(heights, yEdges, xEdges, rowAxisName, columnAxisName, null, aggr);
            /*
			+ sep +
			"Errors:" + sep +
			new Cern.Colt.Matrix.doublealgo.Formatter().ToTitleString(
				errors,yEdges,xEdges,rowAxisName,columnAxisName,null,aggr);
			*/
        }
        /// <summary> 
        /// Returns a string representation of the given argument.
        //// 
        public String ToString(IHistogram2D h)
        {
            String columnAxisName = "X";
            String rowAxisName = "Y";
            Hep.Aida.Bin.BinFunction1D[] aggr = { Hep.Aida.Bin.BinFunctions1D.Sum() };
            String format = "G"; // "%G"
            //String format = "%1.2G";

            Cern.Colt.Matrix.Former f = new Cern.Colt.Matrix.Implementation.FormerFactory().Create(format);
            String sep = "\n\r"; //System.getProperty("line.separator");
            int[] minMaxBins = h.MinMaxBins;
            String title = h.Title + ":" + sep +
                "   Entries=" + Form(f, h.Entries) + ", ExtraEntries=" + Form(f, h.ExtraEntries) + sep +
                "   MeanX=" + Form(f, h.MeanX) + ", RmsX=" + Form(f, h.RmsX) + sep +
                "   MeanY=" + Form(f, h.MeanY) + ", RmsY=" + Form(f, h.RmsX) + sep +
                "   MinBinHeight=" + Form(f, h.BinHeight(minMaxBins[0], minMaxBins[1])) + ", MaxBinHeight=" + Form(f, h.BinHeight(minMaxBins[2], minMaxBins[3])) + sep +

                "   xAxis: " +
                "Bins=" + Form(f, h.XAxis.Bins) +
                ", Min=" + Form(f, h.XAxis.LowerEdge) +
                ", Max=" + Form(f, h.XAxis.UpperEdge) + sep +

                "   yAxis: " +
                "Bins=" + Form(f, h.YAxis.Bins) +
                ", Min=" + Form(f, h.YAxis.LowerEdge) +
                ", Max=" + Form(f, h.YAxis.UpperEdge);

            String[] xEdges = new String[h.XAxis.Bins];
            for (int i = 0; i < h.XAxis.Bins; i++) xEdges[i] = Form(f, h.XAxis.BinLowerEdge(i));

            String[] yEdges = new String[h.YAxis.Bins];
            for (int i = 0; i < h.YAxis.Bins; i++) yEdges[i] = Form(f, h.YAxis.BinLowerEdge(i));
            new List<Object>(yEdges).Reverse(); // keep coordd system

            Cern.Colt.Matrix.DoubleMatrix2D heights = new DenseDoubleMatrix2D(ToArrayHeights(h));
            heights = heights.ViewDice().ViewRowFlip(); // keep the histo coordd system
                                                        //heights = heights.ViewPart(1,1,heights.Rows()-2,heights.columns()-2); // ignore under&overflows

            //Cern.Colt.Matrix.DoubleMatrix2D errors = new Cern.Colt.Matrix.DenseDoubleMatrix2D(toArrayErrors(h));
            //errors = errors.ViewDice().ViewRowFlip(); // keep the histo coord system
            ////errors = errors.ViewPart(1,1,errors.Rows()-2,errors.columns()-2); // ignore under&overflows

            return title + sep +
                "Heights:" + sep +
                new Formatter().ToTitleString(
                    heights, yEdges, xEdges, rowAxisName, columnAxisName, null, aggr);
            /*
			+ sep +
			"Errors:" + sep +
			new Cern.Colt.Matrix.doublealgo.Formatter().ToTitleString(
				errors,yEdges,xEdges,rowAxisName,columnAxisName,null,aggr);
			*/
        }
        /// <summary> 
        /// Returns a string representation of the given argument.
        //// 
        public String ToString(IHistogram3D h)
        {
            String columnAxisName = "X";
            String rowAxisName = "Y";
            String sliceAxisName = "Z";
            Hep.Aida.Bin.BinFunction1D[] aggr = { Hep.Aida.Bin.BinFunctions1D.Sum() };
            String format = "G"; //"%G"
            //String format = "%1.2G";

            Cern.Colt.Matrix.Former f = new Cern.Colt.Matrix.Implementation.FormerFactory().Create(format);
            String sep = "\n\r"; //System.getProperty("line.separator");
            int[] minMaxBins = h.MinMaxBins;
            String title = h.Title + ":" + sep +
                "   Entries=" + Form(f, h.Entries) + ", ExtraEntries=" + Form(f, h.ExtraEntries) + sep +
                "   MeanX=" + Form(f, h.MeanX) + ", RmsX=" + Form(f, h.RmsX) + sep +
                "   MeanY=" + Form(f, h.MeanY) + ", RmsY=" + Form(f, h.RmsX) + sep +
                "   MeanZ=" + Form(f, h.MeanZ) + ", RmsZ=" + Form(f, h.RmsZ) + sep +
                "   MinBinHeight=" + Form(f, h.BinHeight(minMaxBins[0], minMaxBins[1], minMaxBins[2])) + ", MaxBinHeight=" + Form(f, h.BinHeight(minMaxBins[3], minMaxBins[4], minMaxBins[5])) + sep +

                "   xAxis: " +
                "Bins=" + Form(f, h.XAxis.Bins) +
                ", Min=" + Form(f, h.XAxis.LowerEdge) +
                ", Max=" + Form(f, h.XAxis.UpperEdge) + sep +

                "   yAxis: " +
                "Bins=" + Form(f, h.YAxis.Bins) +
                ", Min=" + Form(f, h.YAxis.LowerEdge) +
                ", Max=" + Form(f, h.YAxis.UpperEdge) + sep +

                "   zAxis: " +
                "Bins=" + Form(f, h.ZAxis.Bins) +
                ", Min=" + Form(f, h.ZAxis.LowerEdge) +
                ", Max=" + Form(f, h.ZAxis.UpperEdge);

            String[] xEdges = new String[h.XAxis.Bins];
            for (int i = 0; i < h.XAxis.Bins; i++) xEdges[i] = Form(f, h.XAxis.BinLowerEdge(i));

            String[] yEdges = new String[h.YAxis.Bins];
            for (int i = 0; i < h.YAxis.Bins; i++) yEdges[i] = Form(f, h.YAxis.BinLowerEdge(i));
            new List<Object>(yEdges).Reverse(); // keep coordd system

            String[] zEdges = new String[h.ZAxis.Bins];
            for (int i = 0; i < h.ZAxis.Bins; i++) zEdges[i] = Form(f, h.ZAxis.BinLowerEdge(i));
            new List<Object>(zEdges).Reverse(); // keep coordd system

            DoubleMatrix3D heights = new DenseDoubleMatrix3D(ToArrayHeights(h));
            heights = heights.ViewDice(2, 1, 0).ViewSliceFlip().ViewRowFlip(); // keep the histo coordd system
                                                                               //heights = heights.ViewPart(1,1,heights.Rows()-2,heights.columns()-2); // ignore under&overflows

            //Cern.Colt.Matrix.DoubleMatrix2D errors = new Cern.Colt.Matrix.DenseDoubleMatrix2D(toArrayErrors(h));
            //errors = errors.ViewDice().ViewRowFlip(); // keep the histo coord system
            ////errors = errors.ViewPart(1,1,errors.Rows()-2,errors.columns()-2); // ignore under&overflows

            return title + sep +
                "Heights:" + sep +
                new Formatter().ToTitleString(
                    heights, zEdges, yEdges, xEdges, sliceAxisName, rowAxisName, columnAxisName, "", aggr);
            /*
			+ sep +
			"Errors:" + sep +
			new Cern.Colt.Matrix.doublealgo.Formatter().ToTitleString(
				errors,yEdges,xEdges,rowAxisName,columnAxisName,null,aggr);
			*/
        }
        /// <summary> 
        /// Returns a XML representation of the given argument.
        //// 
        public String ToXML(IHistogram1D h)
        {
            StringBuilder buf = new StringBuilder();
            String sep = "\n\r"; //System.getProperty("line.separator");
            buf.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>"); buf.Append(sep);
            buf.Append("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">"); buf.Append(sep);
            buf.Append("<plotML>"); buf.Append(sep);
            buf.Append("<plot>"); buf.Append(sep);
            buf.Append("<dataArea>"); buf.Append(sep);
            buf.Append("<data1d>"); buf.Append(sep);
            buf.Append("<bins1d title=\"" + h.Title + "\">"); buf.Append(sep);
            for (int i = 0; i < h.XAxis.Bins; i++)
            {
                buf.Append(h.BinEntries(i) + "," + h.BinError(i)); buf.Append(sep);
            }
            buf.Append("</bins1d>"); buf.Append(sep);
            buf.Append("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
            buf.Append(" min=\"" + h.XAxis.LowerEdge + "\"");
            buf.Append(" max=\"" + h.XAxis.UpperEdge + "\"");
            buf.Append(" numberOfBins=\"" + h.XAxis.Bins + "\"");
            buf.Append("/>"); buf.Append(sep);
            buf.Append("<statistics>"); buf.Append(sep);
            buf.Append("<statistic name=\"Entries\" value=\"" + h.Entries + "\"/>"); buf.Append(sep);
            buf.Append("<statistic name=\"Underflow\" value=\"" + h.BinEntries(int.Parse(HistogramType.UNDERFLOW.ToString())) + "\"/>"); buf.Append(sep);
            buf.Append("<statistic name=\"Overflow\" value=\"" + h.BinEntries(int.Parse(HistogramType.OVERFLOW.ToString())) + "\"/>"); buf.Append(sep);
            if (!Double.IsNaN(h.Mean))
            {
                buf.Append("<statistic name=\"Mean\" value=\"" + h.Mean + "\"/>"); buf.Append(sep);
            }
            if (!Double.IsNaN(h.Rms))
            {
                buf.Append("<statistic name=\"RMS\" value=\"" + h.Rms + "\"/>"); buf.Append(sep);
            }
            buf.Append("</statistics>"); buf.Append(sep);
            buf.Append("</data1d>"); buf.Append(sep);
            buf.Append("</dataArea>"); buf.Append(sep);
            buf.Append("</plot>"); buf.Append(sep);
            buf.Append("</plotML>"); buf.Append(sep);
            return buf.ToString();
        }

        /// <summary> 
        /// Returns a XML representation of the given argument.
        //// 
        public String ToXML(IHistogram2D h)
        {
            StringBuilder o = new StringBuilder();
            String sep = "\n\r"; //System.getProperty("line.separator");
            o.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>"); o.Append(sep);
            o.Append("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">"); o.Append(sep);
            o.Append("<plotML>"); o.Append(sep);
            o.Append("<plot>"); o.Append(sep);
            o.Append("<dataArea>"); o.Append(sep);
            o.Append("<data2d type=\"xxx\">"); o.Append(sep);
            o.Append("<bins2d title=\"" + h.Title + "\" xSize=\"" + h.XAxis.Bins + "\" ySize=\"" + h.YAxis.Bins + "\">"); o.Append(sep);
            for (int i = 0; i < h.XAxis.Bins; i++)
                for (int j = 0; j < h.YAxis.Bins; j++)
                {
                    o.Append(h.BinEntries(i, j) + "," + h.BinError(i, j)); o.Append(sep);
                }
            o.Append("</bins2d>"); o.Append(sep);
            o.Append("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
            o.Append(" min=\"" + h.XAxis.LowerEdge + "\"");
            o.Append(" max=\"" + h.XAxis.UpperEdge + "\"");
            o.Append(" numberOfBins=\"" + h.XAxis.Bins + "\"");
            o.Append("/>"); o.Append(sep);
            o.Append("<binnedDataAxisAttributes type=\"double\" axis=\"y0\"");
            o.Append(" min=\"" + h.YAxis.LowerEdge + "\"");
            o.Append(" max=\"" + h.YAxis.UpperEdge + "\"");
            o.Append(" numberOfBins=\"" + h.YAxis.Bins + "\"");
            o.Append("/>"); o.Append(sep);
            //o.Append("<statistics>"); o.Append(sep);
            //o.Append("<statistic name=\"Entries\" value=\""+h.Entries+"\"/>"); o.Append(sep);
            //o.Append("<statistic name=\"MeanX\" value=\""+h.MeanX+"\"/>"); o.Append(sep);
            //o.Append("<statistic name=\"RmsX\" value=\""+h.RmsX+"\"/>"); o.Append(sep);
            //o.Append("<statistic name=\"MeanY\" value=\""+h.MeanY+"\"/>"); o.Append(sep);
            //o.Append("<statistic name=\"RmsY\" value=\""+h.RmsY+"\"/>"); o.Append(sep);
            //o.Append("</statistics>"); o.Append(sep);
            o.Append("</data2d>"); o.Append(sep);
            o.Append("</dataArea>"); o.Append(sep);
            o.Append("</plot>"); o.Append(sep);
            o.Append("</plotML>"); o.Append(sep);
            return o.ToString();
        }
    }
}
