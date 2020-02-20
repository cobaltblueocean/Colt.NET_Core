using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Stat.Quantile;
using Cern.Jet.Random.Engine;
using System.Runtime.CompilerServices;
using Cern.Jet.Stat;

namespace Cern.Hep.Aida.Bin
{
    /// <summary>
    /// 
    /// 1-dimensional non-rebinnable bin holding<i> double</i> elements with scalable quantile operations defined upon;
    /// Using little main memory, quickly computes approximate quantiles over very large data sequences with and even without a-priori knowledge of the number of elements to be filled;
    ///     Conceptually a strongly lossily compressed multiset(or bag);
    ///     Guarantees to respect the worst case approximation error specified upon instance construction.
    ///     First see the <a href = "package-summary.html" > package summary</a> and javadoc<a href="package-tree.html"> tree view</a> to get the broad picture.
    /// <p>
    /// <b>Motivation and Problem:</b>
    /// Intended to help scale applications requiring quantile computation.
    /// Quantile computation on very large data sequences is problematic, for the following reasons:
    /// Computing quantiles requires sorting the data sequence.
    ///     To sort a data sequence the entire data sequence needs to be available.
    ///     Thus, data cannot be thrown away during filling (as done by static bins like { @link StaticBin1D }
    ///     and {@link MightyStaticBin1D}).
    /// It needs to be kept, either in main memory or on disk.
    /// There is often not enough main memory available.
    /// Thus, during filling data needs to be streamed onto disk.
    /// Sorting disk resident data is prohibitively time consuming.
    /// As a consequence, traditional methods either need very large memories (like { @link DynamicBin1D}) or time consuming disk based sorting.
    /// <p>
    /// This class proposes to efficiently solve the problem, at the expense of producing approximate rather than exact results.
    /// It can deal with infinitely many elements without resorting to disk.
    /// The main memory requirements are smaller than for any other known approximate technique by an order of magnitude.
    /// They get even smaller if an upper limit on the maximum number of elements ever to be added is known a-priori.
    /// <p>
    /// <b>Approximation error:</b>
    /// The approximation guarantees are parametrizable and explicit but probabilistic, and apply for arbitrary value distributions and arrival distributions of the data sequence.
    /// In other words, this class guarantees to respect the worst case approximation error specified upon instance construction to a certain probability.
    /// Of course, if it is specified that the approximation error should not exceed some number <i>very close</i> to zero,
    /// this class will potentially consume just as much memory as any of the traditional exact techniques would do.
    /// However, for errors larger than 10<sup>-5</sup>, its memory requirements are modest, as shown by the table below.
    /// <p>
    /// <b>Main memory requirements:</b>
    /// Given in megabytes, assuming a single element(<i>double</i>) takes 8 byte.
    /// The number of elements required is then<i> MB*1024*1024/8</i>.
    /// <p>
    /// <p>
    /// <b>Parameters:</b>
    /// <ul>
    /// <li><i>epsilon</i> - the maximum allowed approximation error on quantiles; in <i>[0.0,1.0]</i>.
    /// To get exact rather than approximate quantiles, HashSet<i> epsilon = 0.0 </ tt >;
    /// <li><i>delta</i> - the probability allowed that the approximation error fails to be smaller than epsilon; in <i>[0.0,1.0]</i>.
    /// To avoid probabilistic answers, set <i>delta=0.0</i>.
    /// For example, <i>delta = 0.0001</i> is equivalent to a confidence of<i>99.99%</i>.
    /// <li><i>quantiles</i> - the number of quantiles to be computed; in <i>[0, int.MaxValue]</i>.
    /// <li><i>is N known?</i> - specifies whether the exact size of the dataset over which quantiles are to be computed is known.
    /// <li>N<sub> max</sub> - the exact dataset size, if known.Otherwise, an upper limit on the dataset size.If no upper limit is known set to infinity (<i>long.MaxValue</i>).
    /// </ul>
    /// 	N<sub> max</sub>=inf - we are sure that exactly (<i>known</i>) or less than(<i>unknown</i>) infinity elements will be added.
    /// <br>N<sub> max</sub>=10<sup>6</sup> - we are sure that exactly (<i>known</i>) or less than(<i>unknown</i>) 10<sup>6</sup> elements will be added.
    /// <br>N<sub> max</sub>=10<sup>7</sup> - we are sure that exactly (<i>known</i>) or less than(<i>unknown</i>) 10<sup>7</sup> elements will be added.
    /// <br>N<sub> max</sub>=10<sup>8</sup> - we are sure that exactly (<i>known</i>) or less than(<i>unknown</i>) 10<sup>8</sup> elements will be added.
    /// <p>
    /// <table width = "75%" border= "1" cellpadding= "6" cellspacing= "0" align= "center" >
    ///   < tr align= "center" valign= "middle" >
    ///     < td width= "20%" nowrap colspan = "13" bgcolor= "#33CC66" >< font color= "#000000" ></ font >
    ///       < div align= "center" >< font color= "#000000" size= "5" > Required main memory
    ///         [MB]</font></div>
    /// 	  </td>
    ///   </tr>
    ///   <tr align = "center" valign= "middle" >
    ///     < td width= "7%" nowrap rowspan = "2" bgcolor= "#FF9966" >< font color= "#000000" >#quantiles</font></td>
    /// 	< td width= "6%" nowrap rowspan = "2" bgcolor= "#FF9966" >
    ///       < div align= "center" ></ div >
    ///       < div align= "center" ></ div >
    ///       < div align= "center" >< font color= "#000000" > epsilon </ font ></ div >
    ///     </ td >
    ///     < td width= "6%" nowrap rowspan = "2" bgcolor= "#FF9966" >< font color= "#000000" > delta </ font ></ td >
    ///     < td width= "1%" nowrap rowspan = "31" > &nbsp;</td>
    /// 	<td nowrap colspan="4" bgcolor="#FF9966"><font color = "#000000" > N unknown</font></td>
    /// 	<td width = "1%" nowrap align = "center" valign="middle" bgcolor="#C0C0C0" rowspan="31"><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font ></ td >
    ///                                                                                                                     < td nowrap colspan = "4" bgcolor="#FF9966"><font color = "#000000" > N known</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "7%" nowrap bgcolor = "#FF9966" >< font color="#000000">N<sub> max</sub>=inf</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 6 </ sup ></ font ></ td >
    ///           < td width= "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 7 </ sup ></ font ></ td >
    ///           < td width= "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 8 </ sup ></ font ></ td >
    ///           < td width= "8%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= inf </ font ></ td >
    ///           < td width= "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 6 </ sup ></ font ></ td >
    ///           < td width= "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 7 </ sup ></ font ></ td >
    ///           < td width= "19%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 8 </ sup ></ font ></ td >
    ///         </ tr >
    ///         < tr align= "center" valign= "middle" >
    ///           < td nowrap bgcolor = "#C0C0C0" colspan= "3" >< font color= "#000000" ></ font >
    ///             < div align= "center" ></ div >
    ///             < font color= "#000000" ></ font ></ td >
    ///           < td nowrap colspan = "4" > &nbsp;</td>
    /// 	<td nowrap colspan="4">&nbsp;</td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "7%" nowrap bgcolor = "#FFCCCC" >< font color="#000000">any</font></td>
    /// 	<td width = "6%" nowrap bgcolor = "#FFCCCC" >
    ///         < div align="center"><font color = "#000000" > 0 </ font ></ div >
    ///        </ td >
    ///        < td width="6%" nowrap bgcolor = "#FFCCCC" >< font color="#000000">any</font></td>
    /// 	<td width = "7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">infinity</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">7.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">76</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">762</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">infinity</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">7.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">76</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">762</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "7%" nowrap rowspan = "6" bgcolor="#FFCCCC"><font color = "#000000" > any </ font ></ td >
    ///         < td width="6%" nowrap bgcolor = "#FFCCCC" >
    ///            < div align="center"><font color = "#000000" > 10 < sup > -1 </ sup ></ font ></ div >
    ///           </ td >
    ///           < td width="6%" nowrap rowspan = "6" bgcolor="#FFCCCC"><font color = "#000000" > 0 </ font ></ td >
    ///              < td width="7%" nowrap rowspan = "6" bgcolor="#66CCFF"><font color = "#000000" > infinity </ font ></ td >
    ///                 < td width="9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.003</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.005</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.006</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.003</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.005</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.006</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap bgcolor = "#FFCCCC" >
    ///         < div align="center"><font color = "#000000" > 10 < sup > -2 </ sup ></ font ></ div >
    ///        </ td >
    ///        < td width="9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.02</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.05</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.31</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.02</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.05</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap bgcolor = "#FFCCCC" >
    ///         < div align="center"><font color = "#000000" > 10 < sup > -3 </ sup ></ font ></ div >
    ///        </ td >
    ///        < td width="9%" nowrap align = "center" valign="middle" bgcolor="#66CCFF"><font color = "#000000" > 0.12 </ font ></ td >
    ///           < td width="9%" nowrap align = "center" valign="middle" bgcolor="#66CCFF"><font color = "#000000" > 0.2 </ font ></ td >
    ///              < td width="9%" nowrap align = "center" valign="middle" bgcolor="#66CCFF"><font color = "#000000" > 0.3 </ font ></ td >
    ///                 < td width="8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.7</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.12</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.2</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.3</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap bgcolor = "#FFCCCC" >
    ///         < div align="center"><font color = "#000000" > 10 < sup > -4 </ sup ></ font ></ div >
    ///        </ td >
    ///        < td width="9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">26.9</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap bgcolor = "#FFCCCC" >
    ///         < div align="center"><font color = "#000000" > 10 < sup > -5 </ sup ></ font ></ div >
    ///        </ td >
    ///        < td width="9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">205</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap bgcolor = "#FFCCCC" >
    ///         < div align="center"><font color = "#000000" > 10 < sup > -6 </ sup ></ font ></ div >
    ///        </ td >
    ///        < td width="9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">7.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">25.4</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">63.6</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1758</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">7.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">25.4</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">63.6</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td nowrap bgcolor="#C0C0C0" colspan="3"><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font ></ td >
    ///           < td nowrap colspan = "4" > &nbsp;</td>
    /// 	<td nowrap colspan="4">&nbsp;</td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "7%" nowrap bgcolor = "#FFCCCC" rowspan="8"><font color = "#000000" > 100 </ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font></td>
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC" rowspan="2"> 
    /// 	  <div align = "center" >< font color="#000000">10<sup> -2</sup></font></div>
    /// 	  <font color = "#000000" ></ font ></ td >
    ///     < td width="6%" nowrap bgcolor = "#FFCCCC" >< font color="#000000">10<sup> -1</sup></font></td>
    /// 	<td width = "7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.033</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.021</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.020</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.020</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.020</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.020</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap bgcolor = "#FFCCCC" >< font color="#000000">10<sup> -5</sup></font></td>
    /// 	<td width = "7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.038</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.021</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.04</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.024</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.020</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.020</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.020</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC" rowspan="2"> 
    /// 	  <div align = "center" >< font color="#000000">10<sup> -3</sup></font></div>
    /// 	  <font color = "#000000" ></ font ></ td >
    ///     < td width="6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -1 </ sup ></ font ></ td >
    ///        < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.48</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.12</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.2</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.3</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.32</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.12</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.2</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.3</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -5 </ sup ></ font ></ td >
    ///         < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.54</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.12</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.2</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.3</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.37</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.12</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.2</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.3</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC" rowspan="2"> 
    /// 	  <div align = "center" >< font color="#000000">10<sup> -4</sup></font></div>
    /// 	  <font color = "#000000" ></ font ></ td >
    ///     < td width="6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -1 </ sup ></ font ></ td >
    ///        < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">4.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -5 </ sup ></ font ></ td >
    ///         < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">7.2</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">5.2</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.6</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC" rowspan="2"> 
    /// 	  <div align = "center" >< font color="#000000">10<sup> -5</sup></font></div>
    /// 	  <font color = "#000000" ></ font ></ td >
    ///     < td width="6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -1 </ sup ></ font ></ td >
    ///        < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">86</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">63</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -5 </ sup ></ font ></ td >
    ///         < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">94</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">70</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td nowrap bgcolor="#C0C0C0" colspan="3"><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font ></ td >
    ///           < td nowrap colspan = "4" > &nbsp;</td>
    /// 	<td nowrap colspan="4">&nbsp;</td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "7%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC" rowspan="8"><font color = "#000000" > 10000 </ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font><font color = "#000000" ></ font >< font color="#000000"></font></td>
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC" rowspan="2"> 
    /// 	  <div align = "center" >< font color="#000000">10<sup> -2</sup></font></div>
    /// 	  <font color = "#000000" ></ font ></ td >
    ///     < td width="6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -1 </ sup ></ font ></ td >
    ///        < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.04</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.02</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.04</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.02</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.02</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.02</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.02</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -5 </ sup ></ font ></ td >
    ///         < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.04</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.02</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.04</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.02</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.03</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC" rowspan="2"> 
    /// 	  <div align = "center" >< font color="#000000">10<sup> -3</sup></font></div>
    /// 	  <font color = "#000000" ></ font ></ td >
    ///     < td width="6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -1 </ sup ></ font ></ td >
    ///        < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.52</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.12</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.21</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.3</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.35</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.12</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.21</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.3</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -5 </ sup ></ font ></ td >
    ///         < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.56</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.12</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.21</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.3</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.38</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.12</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.21</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.3</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC" rowspan="2"> 
    /// 	  <div align = "center" >< font color="#000000">10<sup> -4</sup></font></div>
    /// 	  <font color = "#000000" ></ font ></ td >
    ///     < td width="6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -1 </ sup ></ font ></ td >
    ///        < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">7.0</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.64</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">5.0</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.64</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -5 </ sup ></ font ></ td >
    ///         < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">7.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.64</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">5.4</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">0.64</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">1.2</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.1</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC" rowspan="2"> 
    /// 	  <div align = "center" >< font color="#000000">10<sup> -5</sup></font></div>
    /// 	  <font color = "#000000" ></ font ></ td >
    ///     < td width="6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -1 </ sup ></ font ></ td >
    ///        < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">90</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">67</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FFCCCC"><font color = "#000000" > 10 < sup > -5 </ sup ></ font ></ td >
    ///         < td width="7%" nowrap bgcolor = "#66CCFF" >< font color="#000000">96</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    /// 	<td width = "8%" nowrap bgcolor = "#66CCFF" >< font color="#000000">71</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">2.5</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#66CCFF" >< font color="#000000">6.4</font></td>
    /// 	<td width = "19%" nowrap bgcolor = "#66CCFF" >< font color="#000000">11.6</font></td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "19%" nowrap align = "center" valign="middle" colspan="3">&nbsp;</td>
    /// 	<td width = "34%" nowrap colspan = "4" > &nbsp;</td>
    /// 	<td width = "45%" nowrap colspan = "4" > &nbsp;</td>
    ///   </tr>
    ///   <tr align = "center" valign="middle"> 
    /// 	<td width = "7%" nowrap align = "center" valign="middle" bgcolor="#FF9966" rowspan="2"><font color = "#000000" >#quantiles</font></td>
    /// 	< td width="6%" nowrap align = "center" valign="middle" bgcolor="#FF9966" rowspan="2">epsilon</td>
    /// 	<td width = "6%" nowrap align = "center" valign="middle" bgcolor="#FF9966" rowspan="2">delta</td>
    /// 	<td width = "7%" nowrap bgcolor = "#FF9966" >< font color="#000000">N<sub> max</sub>=inf</font></td>
    /// 	<td width = "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 6 </ sup ></ font ></ td >
    ///           < td width= "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 7 </ sup ></ font ></ td >
    ///           < td width= "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 8 </ sup ></ font ></ td >
    ///           < td width= "7%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= inf </ font ></ td >
    ///           < td width= "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 6 </ sup ></ font ></ td >
    ///           < td width= "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 7 </ sup ></ font ></ td >
    ///           < td width= "9%" nowrap bgcolor = "#FF9966" >< font color= "#000000" > N < sub > max </ sub >= 10 < sup > 8 </ sup ></ font ></ td >
    ///         </ tr >
    ///         < tr align= "center" valign= "middle" >
    ///           < td nowrap colspan = "4" bgcolor= "#FF9966" >< font color= "#000000" > N unknown</font></td>
    /// 	<td nowrap colspan= "4" bgcolor= "#FF9966" >< font color= "#000000" > N known</font></td>
    ///   </tr>
    ///   <tr align = "center" valign= "middle" >
    ///           < td width= "20%" nowrap align = "center" valign= "middle" colspan= "13" bgcolor= "#33CC66" >< font color= "#000000" size= "5" > Required
    ///             main memory [MB]</font></td>
    ///   </tr>
    /// </table>
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// After: Gurmeet Singh Manku, Sridhar Rajagopalan and Bruce G.Lindsay,
    ///       Random Sampling Techniques for Space Efficient Online Computation of Order Statistics of Large Datasets.
    /// Proc.of the 1999 ACM SIGMOD Int.Conf.on Management of Data,
    ///       Paper available <A HREF = "http:/// www-cad.eecs.berkeley.edu/~manku/papers/unknown.ps.gz" > here </ A >.
    ///       < p >
    ///       and
    ///       < p >
    ///       Gurmeet Singh Manku, Sridhar Rajagopalan and Bruce G.Lindsay,
    ///       Approximate Medians and other Quantiles in One Pass and with Limited Memory,
    ///       Proc.of the 1998 ACM SIGMOD Int.Conf.on Management of Data,
    ///       Paper available <A HREF = "http:/// www-cad.eecs.berkeley.edu/~manku/papers/quantiles.ps.gz" > here </ A >.
    ///       < p >
    ///       The broad picture is as follows.Two concepts are used: <i>Shrinking</i> and <i>Sampling</i>.
    ///       Shrinking takes a data sequence, sorts it and produces a shrinked data sequence by picking every k-th element and throwing away all the rest.
    /// The shrinked data sequence is an approximation to the original data sequence.
    ///       <p>
    /// Imagine a large data sequence (residing on disk or being generated in memory on the fly) and a main memory<i>block</i> of <i>n= b * k </ tt > elements(< tt > b </ tt > is the number of buffers, < tt > k </ tt > is the number of elements per buffer).
    /// Fill elements from the data sequence into the block until it is full or the data sequence is exhausted.
    /// When the block (or a subset of buffers) is full and the data sequence is not exhausted, apply shrinking to lossily compress a number of buffers into one single buffer.
    /// Repeat these steps until all elements of the data sequence have been consumed.
    /// Now the block is a shrinked approximation of the original data sequenced 
    /// Treating it as if it would be the original data sequence, we can determine quantiles in main memory.
    /// <p>
    /// Now, the whole thing boils down to the question of: Can we choose<i> b</i> and<i> k</i> (the number of buffers and the buffer size) such that<i>b* k</i> is minimized,
    /// yet quantiles determined upon the block are<i> guaranteed</i> to be away from the true quantiles no more than some<i> epsilon</i>?
    /// It turns out, we cand It also turns out that the required main memory block size<i> n = b * k </ tt > is usually moderate (see the table above).
    /// <p>
    /// The theme can be combined with random sampling to further reduce main memory requirements, at the expense of probabilistic guarantees.
    /// Sampling filters the data sequence and feeds only selected elements to the algorithm outlined above.
    /// Sampling is turned on or off, depending on the parametrization.
    /// <p>
    /// This quick overview does not go into important details, such as assigning proper<i>weights</i> to buffers, how to choose subsets of buffers to shrink, etc.
    /// For more information consult the papers cited above.
    /// <p>
    /// <b>Time Performance:</b>
    /// <p>
    /// <div align = "center" > Pentium Pro 200 Mhz, SunJDK 1.2.2, NT, java -classic,<br>
    ///   filling 10 <sup>4</sup> elements at a time, reading 100 percentiles at a time,<br>
    ///   hasSumOfLogarithms()=false, hasSumOfInversions()=false, getMaxOrderForSumOfPowers()=2<br>
    /// </div>
    /// <center>
    ///   <table border cellpadding="6" cellspacing="0" align="center" width="623">
    /// 	<tr valign = "middle" >
    ///       < td align="center" height="50" colspan="9" bgcolor="#33CC66" nowrap> <font size = "5" > Performance </ font ></ td >
    ///      </ tr >
    ///      < tr valign="middle"> 
    /// 	  <td align = "center" width="56" height="100" rowspan="2" bgcolor="#FF9966" nowrap> 
    /// 		Quantiles</td>
    /// 	  <td align = "center" width="44" height="100" rowspan="2" bgcolor="#FF9966" nowrap> 
    /// 		Epsilon</td>
    /// 	  <td align = "center" width="32" height="100" rowspan="2" bgcolor="#FF9966" nowrap> 
    /// 		Delta</td>
    /// 	  <td align = "center" width="1" height="150" rowspan="7" nowrap>&nbsp; </td>
    /// 	  <td align = "center" height="50" colspan="2" bgcolor="#33CC66" nowrap> <font size = "5" > Filling </ font >
    ///           < br >
    ///           [#elements/sec] </td>
    /// 	  < td align = "center" width = "1" height = "150" rowspan = "7" nowrap > &nbsp; </td>
    /// 	  <td align = "center" height="50" colspan="2" bgcolor="#33CC66"> <font size = "5" > Quantile
    ///         computation</font><br>
    /// 		[#quantiles/sec] </td>
    /// 	</tr>
    /// 	<tr valign="middle" bgcolor="#FF9966" nowrap> 
    /// 	  <td align="center" width="75" height="50" nowrap valign="middle"> <font color="#000000">N
    ///         unknown,<br>
    ///         N<sub>max</sub>=inf</font></td>
    /// 	  <td align="center" width="77" height="50" nowrap valign="middle"> <font color="#000000">N
    ///         known,<br>
    ///         N<sub>max</sub>=10<sup>7</sup></font> </td>
    /// 	  <td align="center" width="106" height="50" nowrap valign="middle"> <font color="#000000">N
    ///         unknown,<br>
    ///         N<sub>max</sub>=inf</font></td>
    /// 	  <td align="center" width="103" height="50" nowrap valign="middle"> <font color="#000000">N
    ///         known,<br>
    ///         N<sub>max</sub>=10<sup>7</sup></font> </td>
    /// 	</tr>
    /// 	<tr valign="middle"> 
    /// 	  <td align="center" height="31" colspan="3" nowrap>&nbsp; </td>
    /// 	  <td align="center" height="31" colspan="2" nowrap>&nbsp; </td>
    /// 	  <td align="center" height="31" colspan="2" nowrap>&nbsp; </td>
    /// 	</tr>
    /// 	<tr valign="middle"> 
    /// 	  <td align="center" width="56" rowspan="4" bgcolor="#FFCCCC" nowrap> 10<sup>4</sup></td>
    /// 	  <td align="center" width="44" bgcolor="#FFCCCC" nowrap> 10 <sup> -1</sup></td>
    /// 	  <td align="center" width="32" bgcolor="#FFCCCC" nowrap rowspan="4"> 10 <sup> 
    /// 		-1</sup> </td>
    /// 	  <td width="75" bgcolor="#66CCFF" nowrap align="center"> 
    /// 		<p>1600000</p>
    /// 	  </td>
    /// 	  <td width="77" bgcolor="#66CCFF" nowrap align="center">1300000</td>
    /// 	  <td align="center" width="106" bgcolor="#66CCFF" nowrap>250000 </td>
    /// 	  <td align="center" width="103" bgcolor="#66CCFF" nowrap>130000 </td>
    /// 	</tr>
    /// 	<tr valign="middle"> 
    /// 	  <td align="center" width="44" bgcolor="#FFCCCC"> 10 <sup> -2</sup></td>
    /// 	  <td width="75" bgcolor="#66CCFF" align="center">360000</td>
    /// 	  <td width="77" bgcolor="#66CCFF" align="center">1200000</td>
    /// 	  <td align="center" width="106" bgcolor="#66CCFF">50000 </td>
    /// 	  <td align="center" width="103" bgcolor="#66CCFF">20000 </td>
    /// 	</tr>
    /// 	<tr valign="middle"> 
    /// 	  <td align="center" width="44" bgcolor="#FFCCCC"> 10 <sup> -3</sup></td>
    /// 	  <td width="75" bgcolor="#66CCFF" align="center">150000</td>
    /// 	  <td width="77" bgcolor="#66CCFF" align="center">200000</td>
    /// 	  <td align="center" width="106" bgcolor="#66CCFF">3600 </td>
    /// 	  <td align="center" width="103" bgcolor="#66CCFF">3000 </td>
    /// 	</tr>
    /// 	<tr valign="middle"> 
    /// 	  <td align="center" width="44" bgcolor="#FFCCCC"> 10 <sup> -4</sup></td>
    /// 	  <td width="75" bgcolor="#66CCFF" align="center">120000</td>
    /// 	  <td width="77" bgcolor="#66CCFF" align="center">170000</td>
    /// 	  <td align="center" width="106" bgcolor="#66CCFF">80 </td>
    /// 	  <td align="center" width="103" bgcolor="#66CCFF">1000 </td>
    /// 	</tr>
    ///   </table>
    /// </center>
    /// 
    /// </summary>
    public class QuantileBin1D : MightyStaticBin1D
    {

        #region Local Variables
        protected IDoubleQuantileFinder finder = null;
        #endregion

        #region Property

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        protected QuantileBin1D(): base(false, false, 2)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epsilon"></param>
        public QuantileBin1D(double epsilon): this(false, long.MaxValue, epsilon, 0.001, 10000, new Cern.Jet.Random.Engine.DRand(new DateTime()))
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="known_N"></param>
        /// <param name="N"></param>
        /// <param name="epsilon"></param>
        /// <param name="delta"></param>
        /// <param name="quantiles"></param>
        /// <param name="randomGenerator"></param>
        public QuantileBin1D(Boolean known_N, long N, double epsilon, double delta, int quantiles, RandomEngine randomGenerator): this(known_N, N, epsilon, delta, quantiles, randomGenerator, false, false, 2)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="known_N"></param>
        /// <param name="N"></param>
        /// <param name="epsilon"></param>
        /// <param name="delta"></param>
        /// <param name="quantiles"></param>
        /// <param name="randomGenerator"></param>
        /// <param name="hasSumOfLogarithms"></param>
        /// <param name="hasSumOfInversions"></param>
        /// <param name="maxOrderForSumOfPowers"></param>
        public QuantileBin1D(Boolean known_N, long N, double epsilon, double delta, int quantiles, RandomEngine randomGenerator, Boolean hasSumOfLogarithms, Boolean hasSumOfInversions, int maxOrderForSumOfPowers): base(hasSumOfLogarithms, hasSumOfInversions, maxOrderForSumOfPowers)
        {
            this.finder = QuantileFinderFactory.NewDoubleQuantileFinder(known_N, N, epsilon, delta, quantiles, randomGenerator);
            this.Clear();
        }
        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void AddAllOfFromTo(List<Double> list, int from, int to)
        {
            base.AddAllOfFromTo(list, from, to);
            if (this.finder != null) this.finder.AddAllOfFromTo(list, from, to);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Clear()
        {
            base.Clear();
            if (this.finder != null) this.finder.Clear();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override Object Clone()
        {
            QuantileBin1D clone = (QuantileBin1D)base.Clone();
            if (this.finder != null) clone.finder = (IDoubleQuantileFinder)clone.finder.Clone();
            return clone;
        }

        public override String CompareWith(AbstractBin1D other)
        {
            StringBuilder buf = new StringBuilder(base.CompareWith(other));
            if (other is QuantileBin1D) {
                QuantileBin1D q = (QuantileBin1D)other;
                buf.Append("25%, 50% and 75% Quantiles: " + RelError(Quantile(0.25), q.Quantile(0.25)) + ", " + RelError(Quantile(0.5), q.Quantile(0.5)) + ", " + RelError(Quantile(0.75), q.Quantile(0.75)));
                buf.Append("\nquantileInverse(mean): " + RelError(QuantileInverse(Mean()), q.QuantileInverse(q.Mean())) + " %");
                buf.Append("\n");
            }
            return buf.ToString();
        }

        public virtual double Median()
        {
            return Quantile(0.5);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual double Quantile(double phi)
        {
            return Quantiles(new List<Double>(new double[] { phi }))[0];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual double QuantileInverse(double element)
        {
            return finder.Phi(element);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual List<Double> Quantiles(List<Double> phis)
        {
            return finder.QuantileElements(phis);
        }

        public int SizeOfRange(double minElement, double maxElement)
        {
            return (int)System.Math.Round(Size * (QuantileInverse(maxElement) - QuantileInverse(minElement)));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public MightyStaticBin1D[] SplitApproximately(List<Double> percentages, int k)
        {
            /*
               percentages = [p0, p1, p2, ..d, p(size-2), p(size-1)]
               defines bins [p0,p1), [p1,p2), ..d, [p(size-2),p(size-1))
               each bin is divided into k equi-percent-distant sub bins (subintervals).
               e.gd k = 2 means "compute" with a resolution (accuracy) of 2 subbins (subintervals) per bin,

               percentages = [0.1, 0.2, 0.3, ..d, 0.9, 1.0] means
               bin 0 holds the first 0.1-0.0=10% of the sorted elements,
               bin 1 holds the next  0.2-0.1=10% of the sorted elements,
               ...

               bins =          [0.1, 0.2), [0.2, 0.3), ..d, [0.9, 1.0)
                     subBins = [0.1,    0.15,     0.2,     0.25,    0.3,    ....]

                               [0.1, 0.15) [0.15, 0.2)             [0.3, 0.35) [0.35, 0.4)

                                                 [0.2, 0.25) [0.25, 0.3)

             */
            int percentSize = percentages.Count;
            if (k < 1 || percentSize < 2) throw new ArgumentException();

            double[] percent = percentages.ToArray();
            int noOfBins = percentSize - 1;


            // construct subintervals
            double[] subBins = new double[1 + k * (percentSize - 1)];
            subBins[0] = percent[0];
            int c = 1;

            for (int i = 0; i < noOfBins; i++)
            {
                double step = (percent[i + 1] - percent[i]) / k;
                for (int j = 1; j <= k; j++)
                {
                    subBins[c++] = percent[i] + j * step;
                }
            }

            // compute quantile elements;
            double[] quantiles = Quantiles(new List<Double>(subBins)).ToArray();

            // collect summary statistics for each bin.
            // one bin's statistics are the integrated statistics of its subintervals.
            MightyStaticBin1D[] splitBins = new MightyStaticBin1D[noOfBins];
            int maxOrderForSumOfPowers = GetMaxOrderForSumOfPowers();
            maxOrderForSumOfPowers = System.Math.Min(10, maxOrderForSumOfPowers); // don't compute tons of measures

            int dataSize = this.Size;
            c = 0;
            for (int i = 0; i < noOfBins; i++)
            { // for each bin
                double step = (percent[i + 1] - percent[i]) / k;
                double binSum = 0;
                double binSumOfSquares = 0;
                double binSumOfLogarithms = 0;
                double binSumOfInversions = 0;
                double[] binSumOfPowers = null;
                if (maxOrderForSumOfPowers > 2)
                {
                    binSumOfPowers = new double[maxOrderForSumOfPowers - 2];
                }

                double binMax;
                                double binMin = quantiles[c++];
                double safe_min = binMin;
                double subIntervalSize = dataSize * step;

                for (int j = 1; j <= k; j++)
                { // integrate all subintervals
                    binMax = quantiles[c++];
                    double binMean = (binMin + binMax) / 2;
                    binSum += binMean * subIntervalSize;
                    binSumOfSquares += binMean * binMean * subIntervalSize;
                    if (this.HasSumOfLogarithms)
                    {
                        binSumOfLogarithms += (System.Math.Log(binMean)) * subIntervalSize;
                    }
                    if (this.HasSumOfInversions)
                    {
                        binSumOfInversions += (1 / binMean) * subIntervalSize;
                    }
                    if (maxOrderForSumOfPowers >= 3) binSumOfPowers[0] += binMean * binMean * binMean * subIntervalSize;
                    if (maxOrderForSumOfPowers >= 4) binSumOfPowers[1] += binMean * binMean * binMean * binMean * subIntervalSize;
                    for (int p = 5; p <= maxOrderForSumOfPowers; p++)
                    {
                        binSumOfPowers[p - 3] += System.Math.Pow(binMean, p) * subIntervalSize;
                    }

                    binMin = binMax;
                }
                c--;

                // example: bin(0) contains (0.2-0.1) == 10% of all elements
                int binSize = (int)System.Math.Round((percent[i + 1] - percent[i]) * dataSize);
                binMax = binMin;
                binMin = safe_min;

                // fill statistics
                splitBins[i] = new MightyStaticBin1D(this.HasSumOfLogarithms, this.HasSumOfInversions, maxOrderForSumOfPowers);
                if (binSize > 0)
                {
                    splitBins[i].Size = binSize;
                    splitBins[i].Min = binMin;
                    splitBins[i].Max = binMax;
                    splitBins[i].Sum = binSum;
                    splitBins[i].SumOfSquares = binSumOfSquares;
                    splitBins[i].SumOfLogarithms = binSumOfLogarithms;
                    splitBins[i].SumOfInversions = binSumOfInversions;
                    splitBins[i].SumOfPowers = binSumOfPowers;
                }
                /*
                double binMean = binSum / binSize;
                Console.WriteLine("size="+binSize);
                Console.WriteLine("min="+binMin);
                Console.WriteLine("max="+binMax);
                Console.WriteLine("mean="+binMean);
                Console.WriteLine("sum_x="+binSum);
                Console.WriteLine("sum_xx="+binSumOfSquares);
                Console.WriteLine("rms="+System.Math.Sqrt(binSumOfSquares / binSize));
                Console.WriteLine();
                */

            }
            return splitBins;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public MightyStaticBin1D[] SplitApproximately(Hep.Aida.IAxis axis, int k)
        {
            List<Double> percentages = new List<Double>(new Hep.Aida.Ref.Converter().Edges(axis));
            percentages.Insert(0, Double.NegativeInfinity);
            percentages.Add(Double.PositiveInfinity);
            for (int i = percentages.Count; --i >= 0;)
            {
                percentages[i] = QuantileInverse(percentages[i]);
            }

            return SplitApproximately(percentages, k);
        }
        /**
         * Returns a String representation of the receiver.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override String ToString()
        {
            StringBuilder buf = new StringBuilder(base.ToString());
            buf.Append("25%, 50%, 75% Quantiles: " + Quantile(0.25) + ", " + Quantile(0.5) + ", " + Quantile(0.75));
            //buf.Append("10%, 25%, 50%, 75%, 90% Quantiles: "+quantile(0.1) + ", "+ quantile(0.25) + ", "+ quantile(0.5) + ", " + quantile(0.75) + ", " + quantile(0.9));
            buf.Append("\nquantileInverse(median): " + QuantileInverse(Median()));
            buf.Append("\n");
            return buf.ToString();
        }
        #endregion

        #region Local Protected Methods

        #endregion

        #region Local Private Methods

        #endregion

    }
}
