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
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Cern.Hep.Aida;
using Cern.Hep.Aida.Ref;

namespace Cern.Colt.Tests
{
    [Parallelizable(ParallelScope.ContextMask)]
    public class AidaRefTest1
    {
        [Test]
        public void TestMain()
        {
            var path = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "\\TestResult\\AidaRefTest1\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Random r = new Random();
            IHistogram1D h1 = new Histogram1D("AIDA 1D Histogram", 40, -3, 3);
            for (int i = 0; i < 10000; i++) h1.Fill(r.NextDouble());

            IHistogram2D h2 = new Histogram2D("AIDA 2D Histogram", 40, -3, 3, 40, -3, 3);
            for (int i = 0; i < 10000; i++) h2.Fill(r.NextDouble(), r.NextDouble());

            //// Write the results as a PlotML files!
            writeAsXML(h1, path + "aida1.xml");
            writeAsXML(h2, path + "aida2.xml");

            //// Try some projections
            writeAsXML(h2.ProjectionX, path + "projectionX.xml");
            writeAsXML(h2.ProjectionY, path + "projectionY.xml");

            ClassicAssert.Pass();
        }

        private static void writeAsXML(IHistogram1D h, String filename)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
                    writer.WriteLine("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">");
                    writer.WriteLine("<plotML>");
                    writer.WriteLine("<plot>");
                    writer.WriteLine("<dataArea>");
                    writer.WriteLine("<data1d>");
                    writer.WriteLine("<bins1d title=\"" + h.Title + "\">");
                    for (int i = 0; i < h.XAxis.Bins; i++)
                    {
                        writer.WriteLine(h.BinEntries(i) + "," + h.BinError(i));
                    }
                    writer.WriteLine("</bins1d>");
                    writer.Write("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
                    writer.Write(" min=\"" + h.XAxis.LowerEdge + "\"");
                    writer.Write(" max=\"" + h.XAxis.UpperEdge + "\"");
                    writer.Write(" numberOfBins=\"" + h.XAxis.Bins + "\"");
                    writer.WriteLine("/>");
                    writer.WriteLine("<statistics>");
                    writer.WriteLine("<statistic name=\"Entries\" value=\"" + h.Entries + "\"/>");
                    writer.WriteLine("<statistic name=\"Underflow\" value=\"" + h.BinEntries(HistogramType.UNDERFLOW.ToInt()) + "\"/>");
                    writer.WriteLine("<statistic name=\"Overflow\" value=\"" + h.BinEntries(HistogramType.OVERFLOW.ToInt()) + "\"/>");
                    if (!Double.IsNaN(h.Mean)) writer.WriteLine("<statistic name=\"Mean\" value=\"" + h.Mean + "\"/>");
                    if (!Double.IsNaN(h.Rms)) writer.WriteLine("<statistic name=\"RMS\" value=\"" + h.Rms + "\"/>");
                    writer.WriteLine("</statistics>");
                    writer.WriteLine("</data1d>");
                    writer.WriteLine("</dataArea>");
                    writer.WriteLine("</plot>");
                    writer.WriteLine("</plotML>");
                    writer.Close();
                }

            }
            catch (IOException x)
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.Write(x.StackTrace);
                }
            }
        }
        private static void writeAsXML(IHistogram2D h, String filename)
        {
            try
            {
                StreamWriter writer = new StreamWriter(filename);
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
                writer.WriteLine("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">");
                writer.WriteLine("<plotML>");
                writer.WriteLine("<plot>");
                writer.WriteLine("<dataArea>");
                writer.WriteLine("<data2d type=\"xxx\">");
                writer.WriteLine("<bins2d title=\"" + h.Title + "\" xSize=\"" + h.XAxis.Bins + "\" ySize=\"" + h.YAxis.Bins + "\">");
                for (int i = 0; i < h.XAxis.Bins; i++)
                    for (int j = 0; j < h.YAxis.Bins; j++)
                    {
                        writer.WriteLine(h.BinEntries(i, j) + "," + h.BinError(i, j));
                    }
                writer.WriteLine("</bins2d>");
                writer.Write("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
                writer.Write(" min=\"" + h.XAxis.LowerEdge + "\"");
                writer.Write(" max=\"" + h.XAxis.UpperEdge + "\"");
                writer.Write(" numberOfBins=\"" + h.XAxis.Bins + "\"");
                writer.WriteLine("/>");
                writer.Write("<binnedDataAxisAttributes type=\"double\" axis=\"y0\"");
                writer.Write(" min=\"" + h.YAxis.LowerEdge + "\"");
                writer.Write(" max=\"" + h.YAxis.UpperEdge + "\"");
                writer.Write(" numberOfBins=\"" + h.YAxis.Bins + "\"");
                writer.WriteLine("/>");
                //writer.WriteLine("<statistics>");
                //writer.WriteLine("<statistic name=\"Entries\" value=\""+h.entries()+"\"/>");
                //writer.WriteLine("<statistic name=\"MeanX\" value=\""+h.meanX()+"\"/>");
                //writer.WriteLine("<statistic name=\"RmsX\" value=\""+h.rmsX()+"\"/>");
                //writer.WriteLine("<statistic name=\"MeanY\" value=\""+h.meanY()+"\"/>");
                //writer.WriteLine("<statistic name=\"RmsY\" value=\""+h.rmsY()+"\"/>");
                //writer.WriteLine("</statistics>");
                writer.WriteLine("</data2d>");
                writer.WriteLine("</dataArea>");
                writer.WriteLine("</plot>");
                writer.WriteLine("</plotML>");
                writer.Close();
            }
            catch (IOException x)
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.Write(x.StackTrace);
                }
            }
        }

    }
}