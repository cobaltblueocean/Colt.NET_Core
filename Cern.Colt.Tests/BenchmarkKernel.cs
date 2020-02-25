// <copyright file="BenchmarkKernel.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Diagnostics;

namespace Cern.Colt.Tests
{
    public static class BenchmarkKernel
    {
        /**
         * Executes procedure repeatadly until more than minSeconds have elapsed.
         */
        public static float Run(double minSeconds, Double2DProcedure.TimerProcedure procedure)
        {
            //Timer t = new Timer();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            long iter = 0;
            long minMillis = (long)(minSeconds * 1000);
            TimeSpan ts = stopWatch.Elapsed;

            long begin = (long)stopWatch.Elapsed.TotalMilliseconds;  //System.currentTimeMillis();
            long limit = begin + minMillis;
            while (stopWatch.Elapsed.TotalMilliseconds < limit)
            {
                //procedure.init();
                procedure(null);
                iter++;
            }
            long end = (long)stopWatch.Elapsed.TotalMilliseconds;
            if (minSeconds / iter < 0.1)
            {
                // unreliable timing due to very fast iteration;
                // reading, starting and stopping timer distorts measurement
                // do it again with minimal timer overhead
                //Console.WriteLine("iter="+iter+", minSeconds/iter="+minSeconds/iter);
                begin = (long)stopWatch.Elapsed.TotalMilliseconds;
                for (long i = iter; --i >= 0;)
                {
                    procedure(null);
                }
                end = (long)stopWatch.Elapsed.TotalMilliseconds;
            }

            long begin2 = (long)stopWatch.Elapsed.TotalMilliseconds;
            long dummy = 1; // prevent compiler from optimizing away the loop
            for (long i = iter; --i >= 0;)
            {
                dummy *= i;
                procedure(null);
            }
            long end2 = (long)stopWatch.Elapsed.TotalMilliseconds;
            long elapsed = (end - begin) - (end2 - begin2);
            //if (dummy != 0) throw new RuntimeException("dummy != 0");


            stopWatch.Stop();

            return (float)elapsed / 1000.0f / iter;
        }

        public static String SystemInfo()
        {
            return "test";
        }

        /**
         * Returns a String with the system's properties (vendor, version, operating system, etcd)
         */
        public static String SystemInfo2()
        {
            String[] properties = {
                ".NET Framework Description",
                ".NET Framework Version",
                "OS Name",
                "OS Version",
                "OS Architecture",
                "Bits",
                "Runtime Architecture",
                "Manufacture",
                "Environment Platform"
            };

            //// build string matrix
            //var matrix = new Cern.Colt.Matrix.Implementation.DenseObjectMatrix2D(properties.Length, 2);
            //matrix.ViewColumn(0).Assign(properties);

            //// retrieve property values
            //for (int i = 0; i < properties.Length; i++)
            //{
            //    String value = "";

            //    //switch (properties[i])
            //    //{
            //    //    //case ".NET Framework Description":
            //    //    //    value = Cern.Colt.Karnel.FrameworkDescription;
            //    //    //    break;
            //    //    //case ".NET Framework Version":
            //    //    //    value = Cern.Colt.Karnel.FrameworkVersion;
            //    //    //    break;
            //    //    //case "OS Name":
            //    //    //    value = Cern.Colt.Karnel.OSDescription;
            //    //    //    break;
            //    //    //case "OS Version":
            //    //    //    value = Cern.Colt.Karnel.OSVersionString;
            //    //    //    break;
            //    //    //case "OS Architecture":
            //    //    //    value = Cern.Colt.Karnel.OSArchitecture.ToString();
            //    //    //    break;
            //    //    //case "Bits":
            //    //    //    value = Cern.Colt.Karnel.OSBits.ToString();
            //    //    //    break;
            //    //    //case "Runtime Architecture":
            //    //    //    value = Cern.Colt.Karnel.RuntimeArchitecture.ToString();
            //    //    //    break;
            //    //    //case "Manufacture":
            //    //    //    value = Cern.Colt.Karnel.Manufacture;
            //    //    //    break;
            //    //    //case "Environment Platform":
            //    //    //    value = Cern.Colt.Karnel.EnvironmentPlatform.ToString();
            //    //    //    break;
            //    //}
            //    if (value == null) value = "?"; // prop not available
            //    matrix[i, 1] = value;
            //}

            //// format matrix
            //var formatter = new Cern.Colt.Matrix.ObjectAlgorithms.Formatter();
            //formatter.SetPrintShape(false);
            //return formatter.ToString(matrix);

            return properties.ToString();
        }
    }
}
