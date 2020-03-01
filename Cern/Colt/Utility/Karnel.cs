using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Management;
using Microsoft.Win32;

namespace Cern.Colt
{
    /// <summary>
    /// This class is repesenting system information for all supporting platform.
    /// </summary>
    public static class Karnel
    {

        public static String FrameworkDescription
        {
            get
            {
                if (!IsRunningOnMono())
                {
                    return System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
                }
                else
                {
                    Assembly asm = Assembly.Load("Mono");
                    Type t = asm.GetType("Mono.Runtime");
                    var result = t.InvokeMember("GetDisplayName", BindingFlags.InvokeMethod, null, null, null);
                    return "Mono " + Convert.ToString(result); ;
                }
            }
        }

        public static Architecture OSArchitecture
        {
            get
            {
                if (!IsRunningOnMono())
                {
                    return System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;
                }
                else
                {
                    switch (RuntimeArchitecture)
                    {
                        case "arm":
                        case "armv8":
                            return Environment.Is64BitOperatingSystem ? Architecture.Arm64 : Architecture.Arm;
                        case "x86":
                        case "x86-64":
                        // upstream only has these values; try to pretend we're x86 if nothing matches
                        // want more? bug: https://github.com/dotnet/corefx/issues/30706
                        default:
                            return Environment.Is64BitOperatingSystem ? Architecture.X64 : Architecture.X86;
                    }
                }
            }
        }

        public static string OSDescription
        {
            get
            {
                if(IsRunningOnMono())
                {
#if WASM
				return "web"; //yes, hardcoded as right now we don't really support other environments
#else
                return Environment.OSVersion.VersionString;
#endif
                }
                else
                {
                    return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
                }
            }
        }

        public static Architecture ProcessArchitecture
        {
            get
            {
                if (IsRunningOnMono())
                {
                    // we can use the runtime's compiled config options for DllMaps here
                    // process architecure for us is runtime architecture (OS is much harder)
                    // see for values: mono-config.c
                    switch (RuntimeArchitecture)
                    {
                        case "x86":
                            return Architecture.X86;
                        case "x86-64":
                            return Architecture.X64;
                        case "arm":
                            return Architecture.Arm;
                        case "armv8":
                            return Architecture.Arm64;
                        // see comment in OSArchiteture default case
                        default:
                            return Environment.Is64BitProcess ? Architecture.X64 : Architecture.X86;
                    }
                }
                else
                {
                    return System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
                }
            }
        }

        public static bool IsOSPlatform(OSPlatform osPlatform)
        {
            if(IsRunningOnMono())
            {
#if WASM
			return osPlatform == OSPlatform.Create ("WEBASSEMBLY"); 
#else
                switch (EnvironmentPlatform)
                {
                    case PlatformID.Win32NT:
                        return osPlatform == OSPlatform.Windows;
                    case PlatformID.MacOSX:
                        return osPlatform == OSPlatform.OSX;
                    case PlatformID.Unix:
                        return osPlatform == OSPlatform.Linux;
                    default:
                        return false;
                }
#endif
            }
            else
            {
                return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(osPlatform);
            }
        }

        public static String Manufacture
        {
            get
            {
                var ManufactureName = "";

                // create management class object
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                //collection to store all management objects
                ManagementObjectCollection moc = mc.GetInstances();
                if (moc.Count != 0)
                {
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        // display general system information
                        Console.WriteLine("\nMachine Make: {0}",
                                          mo["Manufacturer"].ToString());
                    }
                }

                return ManufactureName;
            }
        }

        public static PlatformID EnvironmentPlatform
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

        public static String OSName
        {
            get { return OSInfo.Name; }
        }

        public static String OSEdition
        {
            get { return OSInfo.Edition; }
        }

        public static String OSServicePack
        {
            get { return OSInfo.ServicePack; }
        }

        public static String OSVersionString
        {
            get { return OSInfo.VersionString; }
        }

        public static int OSBits
        {
            get { return OSInfo.Bits; }
        }

        public static extern string RuntimeArchitecture
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        /// <summary>
        /// Helper method to ditect .NET Core Version
        /// </summary>
        /// <see href="https://techblog.dorogin.com/how-to-detect-net-core-version-in-runtime-ecd65ad695be">Sergei Dorogin’s technical blog</see>
        /// <returns></returns>
        private static string GetNetCoreVersion()
        {
            var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
            var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
                return assemblyPath[netCoreAppIndex + 1];
            return null;
        }

        public static string FrameworkVersion
        {
            get
            {
                // ".NET Core 4.6.26515.07" => ".NET Core 2.1.0"
                var parts = RuntimeInformation.FrameworkDescription.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var i = 0;
                for (; i < parts.Length; i++)
                {
                    if (Char.IsDigit(parts[i][0]))
                    {
                        break;
                    }
                }
                var productName = String.Join(' ', parts, 0, i);
                return String.Join(' ', productName, " ", GetNetCoreVersion());
                #region Only on regular .NET Framework on Windows will support this code
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
                    {
                        int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                        if (true)
                        {
                            if (releaseKey >= 461808)
                            {
                                return "4.7.2";
                            }
                            if (releaseKey >= 461308)
                            {
                                return "4.7.1";
                            }
                            if (releaseKey >= 460798)
                            {
                                return "4.7";
                            }
                            if (releaseKey >= 394802)
                            {
                                return "4.6.2";
                            }
                            if (releaseKey >= 394254)
                            {
                                return "4.6.1";
                            }
                            if (releaseKey >= 393295)
                            {
                                return "4.6";
                            }
                            if (releaseKey >= 393273)
                            {
                                return "4.6 RC";
                            }
                            if ((releaseKey >= 379893))
                            {
                                return "4.5.2";
                            }
                            if ((releaseKey >= 378675))
                            {
                                return "4.5.1";
                            }
                            if ((releaseKey >= 378389))
                            {
                                return "4.5";
                            }

                            RegistryKey installed_versions = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
                            string[] version_names = installed_versions.GetSubKeyNames();
                            //version names start with 'v', eg, 'v3.5' which needs to be trimmed off before conversion
                            double Framework = Convert.ToDouble(version_names[version_names.Length - 1].Remove(0, 1), CultureInfo.InvariantCulture);
                            int SP = Convert.ToInt32(installed_versions.OpenSubKey(version_names[version_names.Length - 1]).GetValue("SP", 0));
                            String ver = Framework.ToString();

                            if (SP > 0)
                                ver += " SP" + SP.ToString();

                            return ver;
                        }
                    }
                }
                #endregion
            }
        }

        #region only on regular .NET Framework on Windows will support this code
        private static void GetVersionFromRegistry()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Opens the registry key for the .NET Framework entry. 
                using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                {
                    // As an alternative, if you know the computers you will query are running .NET Framework 4.5  
                    // or later, you can use: 
                    // using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,  
                    // RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                    foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                    {
                        if (versionKeyName.StartsWith("v"))
                        {

                            RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                            string name = (string)versionKey.GetValue("Version", "");
                            string sp = versionKey.GetValue("SP", "").ToString();
                            string install = versionKey.GetValue("Install", "").ToString();
                            if (install == "") //no install info, must be later.
                                Console.WriteLine(versionKeyName + "  " + name);
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    Console.WriteLine(versionKeyName + "  " + name + "  SP" + sp);
                                }

                            }
                            if (name != "")
                            {
                                continue;
                            }
                            foreach (string subKeyName in versionKey.GetSubKeyNames())
                            {
                                RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                                name = (string)subKey.GetValue("Version", "");
                                if (name != "")
                                    sp = subKey.GetValue("SP", "").ToString();
                                install = subKey.GetValue("Install", "").ToString();
                                if (install == "") //no install info, must be later.
                                    Console.WriteLine(versionKeyName + "  " + name);
                                else
                                {
                                    if (sp != "" && install == "1")
                                    {
                                        Console.WriteLine("  " + subKeyName + "  " + name + "  SP" + sp);
                                    }
                                    else if (install == "1")
                                    {
                                        Console.WriteLine("  " + subKeyName + "  " + name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        public static double FreePhysicalMemorySize
        {
            get
            {
                var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

                var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new {
                    FreePhysicalMemory = Double.Parse(mo["FreePhysicalMemory"].ToString())
                }).FirstOrDefault();

                if (memoryValues != null)
                {
                    return memoryValues.FreePhysicalMemory;
                }
                return -1;
            }
        }

        public static double TotalVisibleMemorySize
        {
            get
            {
                var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

                var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new {
                    TotalVisibleMemorySize = Double.Parse(mo["TotalVisibleMemorySize"].ToString())
                }).FirstOrDefault();

                if (memoryValues != null)
                {
                    return memoryValues.TotalVisibleMemorySize;
                }
                return -1;
            }
        }
    }
}
