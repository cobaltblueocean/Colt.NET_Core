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
    /// AidaRefTest2 Description
    /// </summary>
    [TestFixture]
    public class AidaRefTest2
    {
        [Test]
        public void TestMain()
        {
            var path = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "\\TestResult\\AidaRefTest2\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Random r = new Random();
            IHistogram1D h1 = new Histogram1D("AIDA 1D Histogram", 40, -3, 3);
            for (int i = 0; i < 10000; i++) h1.Fill(r.NextDouble());

            IHistogram2D h2 = new Histogram2D("AIDA 2D Histogram", 40, -3, 3, 40, -3, 3);
            for (int i = 0; i < 10000; i++) h2.Fill(r.NextDouble(), r.NextDouble());

            // Write the results as a PlotML files!
            
            writeAsXML(h1, path + "test1_aida1.xml");
            writeAsXML(h2, path + "test1_aida2.xml");

            // Try some projections

            writeAsXML(h2.ProjectionX, path + "test1_projectionX.xml");
            writeAsXML(h2.ProjectionY, path + "test1_projectionY.xml");
        }

        [Test]
        public static void TestMain2()
        {
            var path = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "\\TestResult\\AidaRefTest2\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            double[] bounds = { -30, 0, 30, 1000 };
            Random r = new Random();
            IHistogram1D h1 = new Histogram1D("AIDA 1D Histogram", new VariableAxis(bounds));
            //IHistogram1D h1 = new Histogram1D("AIDA 1D Histogram",2,-3,3);
            for (int i = 0; i < 10000; i++) h1.Fill(r.NextDouble());

            IHistogram2D h2 = new Histogram2D("AIDA 2D Histogram", new VariableAxis(bounds), new VariableAxis(bounds));
            //IHistogram2D h2 = new Histogram2D("AIDA 2D Histogram",2,-3,3, 2,-3,3);
            for (int i = 0; i < 10000; i++) h2.Fill(r.NextDouble(), r.NextDouble());

            //IHistogram3D h3 = new Histogram3D("AIDA 3D Histogram",new VariableAxis(bounds),new VariableAxis(bounds),new VariableAxis(bounds));
            IHistogram3D h3 = new Histogram3D("AIDA 3D Histogram", 10, -2, +2, 5, -2, +2, 3, -2, +2);
            for (int i = 0; i < 10000; i++) h3.Fill(r.NextDouble(), r.NextDouble(), r.NextDouble());

            // Write the results as a PlotML files!
            writeAsXML(h1, path + "test2_aida1.xml");
            writeAsXML(h2, path + "test2_aida2.xml");
            writeAsXML(h3, path + "test2_aida3.xml");

            // Try some projections

            writeAsXML(h2.ProjectionX, path + "test2_projectionX.xml");
            writeAsXML(h2.ProjectionY, path + "test2_projectionY.xml");
        }
        private static void writeAsXML(IHistogram1D h, String filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine(new Converter().ToString(h));
                //System.out.println(new Converter().toXML(h));
                /*
                try
                {
                    PrintWriter out = new PrintWriter(new FileWriter(filename));
                    out.println(new Converter().toXML(h));
                    out.close();
                }
                catch (IOException x) { x.printStackTrace(); }
                */
            }
        }

        private static void writeAsXML(IHistogram2D h, String filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine(new Converter().ToString(h));
                //System.out.println(new Converter().toXML(h));
                /*
                try
                {
                    PrintWriter out = new PrintWriter(new FileWriter(filename));
                    out.println(new Converter().toXML(h));
                    out.close();
                }
                catch (IOException x) { x.printStackTrace(); }
                */
            }
        }

        private static void writeAsXML(IHistogram3D h, String filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine(new Converter().ToString(h));
                //System.out.println(new Converter().toXML(h));
                /*
                try
                {
                    PrintWriter out = new PrintWriter(new FileWriter(filename));
                    out.println(new Converter().toXML(h));
                    out.close();
                }
                catch (IOException x) { x.printStackTrace(); }
                */
            }
        }
    }
}
