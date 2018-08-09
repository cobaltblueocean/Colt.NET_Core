// <copyright file="AidaRefTest1.cs" company="CERN">
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

namespace Colt.Tests
{
    using NUnit.Framework;
    using Cern.Hep.Aida;
    using Cern.Hep.Aida.Ref;

    /// <summary>
    /// AidaRefTest1 Description
    /// </summary>
    [TestFixture]
    public class AidaRefTest1
    {
        [Test]
        public static void main(String[] argv)
        {
            Random r = new Random();
            IHistogram1D h1 = new Histogram1D("AIDA 1D Histogram", 40, -3, 3);
            for (int i = 0; i < 10000; i++) h1.Fill(r.NextDouble());

            IHistogram2D h2 = new Histogram2D("AIDA 2D Histogram", 40, -3, 3, 40, -3, 3);
            for (int i = 0; i < 10000; i++) h2.Fill(r.NextDouble(), r.NextDouble());

            // Write the results as a PlotML files!
            writeAsXML(h1, "aida1.xml");
            writeAsXML(h2, "aida2.xml");

            // Try some projections

            writeAsXML(h2.ProjectionX, "projectionX.xml");
            writeAsXML(h2.ProjectionY, "projectionY.xml");
        }

        [Test]
        private static void writeAsXML(IHistogram1D h, String filename)
        {
            try
            {
                PrintWriter out = new PrintWriter(new FileWriter(filename));
			out.println("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
			out.println("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">");
			out.println("<plotML>");
			out.println("<plot>");
			out.println("<dataArea>");
			out.println("<data1d>");
			out.println("<bins1d title=\"" + h.title() + "\">");
                for (int i = 0; i < h.xAxis().bins(); i++)
                {
				out.println(h.binEntries(i) + "," + h.binError(i));
                }
			out.println("</bins1d>");
			out.print("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
			  out.print(" min=\"" + h.xAxis().lowerEdge() + "\"");
			  out.print(" max=\"" + h.xAxis().upperEdge() + "\"");
			  out.print(" numberOfBins=\"" + h.xAxis().bins() + "\"");
			  out.println("/>");
			out.println("<statistics>");
			out.println("<statistic name=\"Entries\" value=\"" + h.entries() + "\"/>");
			out.println("<statistic name=\"Underflow\" value=\"" + h.binEntries(h.UNDERFLOW) + "\"/>");
			out.println("<statistic name=\"Overflow\" value=\"" + h.binEntries(h.OVERFLOW) + "\"/>");
                if (!Double.IsNaN(h.mean())) out.println("<statistic name=\"Mean\" value=\"" + h.mean() + "\"/>");
                if (!Double.IsNaN(h.rms())) out.println("<statistic name=\"RMS\" value=\"" + h.rms() + "\"/>");
			out.println("</statistics>");
			out.println("</data1d>");
			out.println("</dataArea>");
			out.println("</plot>");
			out.println("</plotML>");
			out.close();
            }
            catch (IOException x) { x.printStackTrace(); }
        }

        [Test]
        private static void writeAsXML(IHistogram2D h, String filename)
        {
            try
            {
                PrintWriter out = new PrintWriter(new FileWriter(filename));
			out.println("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
			out.println("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">");
			out.println("<plotML>");
			out.println("<plot>");
			out.println("<dataArea>");
			out.println("<data2d type=\"xxx\">");
			out.println("<bins2d title=\"" + h.title() + "\" xSize=\"" + h.xAxis().bins() + "\" ySize=\"" + h.yAxis().bins() + "\">");
                for (int i = 0; i < h.xAxis().bins(); i++)
                    for (int j = 0; j < h.yAxis().bins(); j++)
                    {
				out.println(h.binEntries(i, j) + "," + h.binError(i, j));
                    }
			out.println("</bins2d>");
			out.print("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
			  out.print(" min=\"" + h.xAxis().lowerEdge() + "\"");
			  out.print(" max=\"" + h.xAxis().upperEdge() + "\"");
			  out.print(" numberOfBins=\"" + h.xAxis().bins() + "\"");
			  out.println("/>");
			out.print("<binnedDataAxisAttributes type=\"double\" axis=\"y0\"");
			  out.print(" min=\"" + h.yAxis().lowerEdge() + "\"");
			  out.print(" max=\"" + h.yAxis().upperEdge() + "\"");
			  out.print(" numberOfBins=\"" + h.yAxis().bins() + "\"");
			  out.println("/>");
			//out.println("<statistics>");
			//out.println("<statistic name=\"Entries\" value=\""+h.entries()+"\"/>");
			//out.println("<statistic name=\"MeanX\" value=\""+h.meanX()+"\"/>");
			//out.println("<statistic name=\"RmsX\" value=\""+h.rmsX()+"\"/>");
			//out.println("<statistic name=\"MeanY\" value=\""+h.meanY()+"\"/>");
			//out.println("<statistic name=\"RmsY\" value=\""+h.rmsY()+"\"/>");
			//out.println("</statistics>");
			out.println("</data2d>");
			out.println("</dataArea>");
			out.println("</plot>");
			out.println("</plotML>");
			out.close();
            }
            catch (IOException x) { x.printStackTrace(); }
        }
    }
}
