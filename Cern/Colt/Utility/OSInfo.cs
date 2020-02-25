using System;
using System.Management;

namespace Cern.Colt
{
    /// <summary>
    /// Provides detailed information about the host operating system.
    /// </summary>
    public static class OSInfo
    {
        #region BITS
        /// <summary>
        /// Determines if the current application is 32 or 64-bit.
        /// </summary>
        public static int Bits
        {
            get
            {
                return IntPtr.Size * 8;
            }
        }
        #endregion BITS

        #region EDITION
        private static string s_Edition;
        /// <summary>
        /// Gets the edition of the operating system running on this computer.
        /// </summary>
        public static string Edition
        {
            get
            {
                if (s_Edition != null)
                    return s_Edition;  //***** RETURN *****//

                string edition = "";
                byte productType = 0;
                short suiteMask = 0;
                int ed = 0;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    ManagementObjectCollection information = searcher.Get();
                    if (information != null)
                    {
                        foreach (ManagementObject obj in information)
                        {
                            productType = (Byte)(obj["ProductType"]);
                            suiteMask = (short)(obj["SuiteMask"]);
                            ed = (int)(obj["OperatingSystemSKU"]);
                        }
                    }
                }

                OperatingSystem osVersion = Environment.OSVersion;
                int majorVersion = osVersion.Version.Major;
                int minorVersion = osVersion.Version.Minor;

                #region VERSION 4
                if (majorVersion == 4)
                {
                    if (productType == VER_NT_WORKSTATION)
                    {
                        // Windows NT 4.0 Workstation
                        edition = "Workstation";
                    }
                    else if (productType == VER_NT_SERVER)
                    {
                        if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                        {
                            // Windows NT 4.0 Server Enterprise
                            edition = "Enterprise Server";
                        }
                        else
                        {
                            // Windows NT 4.0 Server
                            edition = "Standard Server";
                        }
                    }
                }
                #endregion VERSION 4

                #region VERSION 5
                else if (majorVersion == 5)
                {
                    if (productType == VER_NT_WORKSTATION)
                    {
                        if ((suiteMask & VER_SUITE_PERSONAL) != 0)
                        {
                            // Windows XP Home Edition
                            edition = "Home";
                        }
                        else
                        {
                            // Windows XP / Windows 2000 Professional
                            edition = "Professional";
                        }
                    }
                    else if (productType == VER_NT_SERVER)
                    {
                        if (minorVersion == 0)
                        {
                            if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                            {
                                // Windows 2000 Datacenter Server
                                edition = "Datacenter Server";
                            }
                            else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                // Windows 2000 Advanced Server
                                edition = "Advanced Server";
                            }
                            else
                            {
                                // Windows 2000 Server
                                edition = "Server";
                            }
                        }
                        else
                        {
                            if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                            {
                                // Windows Server 2003 Datacenter Edition
                                edition = "Datacenter";
                            }
                            else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                // Windows Server 2003 Enterprise Edition
                                edition = "Enterprise";
                            }
                            else if ((suiteMask & VER_SUITE_BLADE) != 0)
                            {
                                // Windows Server 2003 Web Edition
                                edition = "Web Edition";
                            }
                            else
                            {
                                // Windows Server 2003 Standard Edition
                                edition = "Standard";
                            }
                        }
                    }
                }
                #endregion VERSION 5

                #region VERSION 6 or higher
                else if (majorVersion >= 6)
                {
                    switch (ed)
                    {
                        case PRODUCT_BUSINESS:
                            edition = "Business";
                            break;
                        case PRODUCT_BUSINESS_N:
                            edition = "Business N";
                            break;
                        case PRODUCT_CLUSTER_SERVER:
                            edition = "HPC Edition";
                            break;
                        case PRODUCT_CLUSTER_SERVER_V:
                            edition = "Server Hyper Core V";
                            break;
                        case PRODUCT_CORE:
                            edition = "Windows 10 Home";
                            break;
                        case PRODUCT_CORE_COUNTRYSPECIFIC:
                            edition = "Windows 10 Home China";
                            break;
                        case PRODUCT_CORE_N:
                            edition = "Windows 10 Home N";
                            break;
                        case PRODUCT_CORE_SINGLELANGUAGE:
                            edition = "Windows 10 Home Single Language";
                            break;
                        case PRODUCT_DATACENTER_EVALUATION_SERVER:
                            edition = "Server Datacenter (evaluation installation)";
                            break;
                        case PRODUCT_DATACENTER_SERVER:
                            edition = "Server Datacenter (full installation)";
                            break;
                        case PRODUCT_DATACENTER_SERVER_CORE:
                            edition = "Server Datacenter (core installation)";
                            break;
                        case PRODUCT_DATACENTER_SERVER_CORE_V:
                            edition = "Server Datacenter without Hyper-V (core installation)";
                            break;
                        case PRODUCT_DATACENTER_SERVER_V:
                            edition = "Server Datacenter without Hyper-V (full installation)";
                            break;
                        case PRODUCT_EDUCATION:
                            edition = "Windows 10 Education";
                            break;
                        case PRODUCT_EDUCATION_N:
                            edition = "Windows 10 Education N";
                            break;
                        case PRODUCT_ENTERPRISE:
                            edition = "Windows 10 Enterprise";
                            break;
                        case PRODUCT_ENTERPRISE_E:
                            edition = "Windows 10 Enterprise E";
                            break;
                        case PRODUCT_ENTERPRISE_EVALUATION:
                            edition = "Windows 10 Enterprise Evaluation";
                            break;
                        case PRODUCT_ENTERPRISE_N:
                            edition = "Windows 10 Enterprise N";
                            break;
                        case PRODUCT_ENTERPRISE_N_EVALUATION:
                            edition = "Windows 10 Enterprise N Evaluation";
                            break;
                        case PRODUCT_ENTERPRISE_S:
                            edition = "Windows 10 Enterprise 2015 LTSB";
                            break;
                        case PRODUCT_ENTERPRISE_S_EVALUATION:
                            edition = "Windows 10 Enterprise 2015 LTSB Evaluation";
                            break;
                        case PRODUCT_ENTERPRISE_S_N:
                            edition = "Windows 10 Enterprise 2015 LTSB N";
                            break;
                        case PRODUCT_ENTERPRISE_S_N_EVALUATION:
                            edition = "Windows 10 Enterprise 2015 LTSB N Evaluation";
                            break;
                        case PRODUCT_ENTERPRISE_SERVER:
                            edition = "Server Enterprise (full installation)";
                            break;
                        case PRODUCT_ENTERPRISE_SERVER_CORE:
                            edition = "Server Enterprise (core installation)";
                            break;
                        case PRODUCT_ENTERPRISE_SERVER_CORE_V:
                            edition = "Server Enterprise without Hyper-V (core installation)";
                            break;
                        case PRODUCT_ENTERPRISE_SERVER_IA64:
                            edition = "Server Enterprise for Itanium-based Systems";
                            break;
                        case PRODUCT_ENTERPRISE_SERVER_V:
                            edition = "Server Enterprise without Hyper-V (full installation)";
                            break;
                        case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL:
                            edition = "Windows Essential Server Solution Additional";
                            break;
                        case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC:
                            edition = "Windows Essential Server Solution Additional SVC";
                            break;
                        case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT:
                            edition = "Windows Essential Server Solution Management";
                            break;
                        case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC:
                            edition = "Windows Essential Server Solution Management SVC";
                            break;
                        case PRODUCT_HOME_BASIC:
                            edition = "Home Basic";
                            break;
                        case PRODUCT_HOME_BASIC_E:
                            edition = "Not supported";
                            break;
                        case PRODUCT_HOME_BASIC_N:
                            edition = "Home Basic N";
                            break;
                        case PRODUCT_HOME_PREMIUM:
                            edition = "Home Premium";
                            break;
                        case PRODUCT_HOME_PREMIUM_E:
                            edition = "Not supported";
                            break;
                        case PRODUCT_HOME_PREMIUM_N:
                            edition = "Home Premium N";
                            break;
                        case PRODUCT_HOME_PREMIUM_SERVER:
                            edition = "Windows Home Server 2011";
                            break;
                        case PRODUCT_HOME_SERVER:
                            edition = "Windows Storage Server 2008 R2 Essentials";
                            break;
                        case PRODUCT_HYPERV:
                            edition = "Microsoft Hyper-V Server";
                            break;
                        case PRODUCT_IOTUAP:
                            edition = "Windows 10 IoT Core";
                            break;
                        case PRODUCT_IOTUAPCOMMERCIAL:
                            edition = "Windows 10 IoT Core Commercial";
                            break;
                        case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                            edition = "Windows Essential Business Server Management Server";
                            break;
                        case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                            edition = "Windows Essential Business Server Messaging Server";
                            break;
                        case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                            edition = "Windows Essential Business Server Security Server";
                            break;
                        case PRODUCT_MOBILE_CORE:
                            edition = "Windows 10 Mobile";
                            break;
                        case PRODUCT_MOBILE_ENTERPRISE:
                            edition = "Windows 10 Mobile Enterprise";
                            break;
                        case PRODUCT_MULTIPOINT_PREMIUM_SERVER:
                            edition = "Windows MultiPoint Server Premium (full installation)";
                            break;
                        case PRODUCT_MULTIPOINT_STANDARD_SERVER:
                            edition = "Windows MultiPoint Server Standard (full installation)";
                            break;
                        case PRODUCT_PRO_WORKSTATION:
                            edition = "Windows 10 Pro for Workstations";
                            break;
                        case PRODUCT_PRO_WORKSTATION_N:
                            edition = "Windows 10 Pro for Workstations N";
                            break;
                        case PRODUCT_PROFESSIONAL:
                            edition = "Windows 10 Pro";
                            break;
                        case PRODUCT_PROFESSIONAL_E:
                            edition = "Not supported";
                            break;
                        case PRODUCT_PROFESSIONAL_N:
                            edition = "Windows 10 Pro N";
                            break;
                        case PRODUCT_PROFESSIONAL_WMC:
                            edition = "Professional with Media Center";
                            break;
                        case PRODUCT_SB_SOLUTION_SERVER:
                            edition = "Windows Small Business Server 2011 Essentials";
                            break;
                        case PRODUCT_SB_SOLUTION_SERVER_EM:
                            edition = "Server For SB Solutions EM";
                            break;
                        case PRODUCT_SERVER_FOR_SB_SOLUTIONS:
                            edition = "Server For SB Solutions";
                            break;
                        case PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM:
                            edition = "Server For SB Solutions EM";
                            break;
                        case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                            edition = "Windows Server 2008 for Windows Essential Server Solutions";
                            break;
                        case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                            edition = "Windows Server 2008 without Hyper-V for Windows Essential Server Solutions";
                            break;
                        case PRODUCT_SERVER_FOUNDATION:
                            edition = "Server Foundation";
                            break;
                        case PRODUCT_SMALLBUSINESS_SERVER:
                            edition = "Windows Small Business Server";
                            break;
                        case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM:
                            edition = "Small Business Server Premium";
                            break;
                        case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE:
                            edition = "Small Business Server Premium (core installation)";
                            break;
                        case PRODUCT_SOLUTION_EMBEDDEDSERVER:
                            edition = "Windows MultiPoint Server";
                            break;
                        case PRODUCT_STANDARD_EVALUATION_SERVER:
                            edition = "Server Standard (evaluation installation)";
                            break;
                        case PRODUCT_STANDARD_SERVER:
                            edition = "Server Standard";
                            break;
                        case PRODUCT_STANDARD_SERVER_CORE:
                            edition = "Server Standard (core installation)";
                            break;
                        case PRODUCT_STANDARD_SERVER_CORE_V:
                            edition = "Server Standard without Hyper-V (core installation)";
                            break;
                        case PRODUCT_STANDARD_SERVER_V:
                            edition = "Server Standard without Hyper-V";
                            break;
                        case PRODUCT_STANDARD_SERVER_SOLUTIONS:
                            edition = "Server Solutions Premium";
                            break;
                        case PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE:
                            edition = "Server Solutions Premium (core installation)";
                            break;
                        case PRODUCT_STARTER:
                            edition = "Starter";
                            break;
                        case PRODUCT_STARTER_E:
                            edition = "Not supported";
                            break;
                        case PRODUCT_STARTER_N:
                            edition = "Starter N";
                            break;
                        case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                            edition = "Storage Server Enterprise";
                            break;
                        case PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE:
                            edition = "Storage Server Enterprise (core installation)";
                            break;
                        case PRODUCT_STORAGE_EXPRESS_SERVER:
                            edition = "Storage Server Express";
                            break;
                        case PRODUCT_STORAGE_EXPRESS_SERVER_CORE:
                            edition = "Storage Server Express (core installation)";
                            break;
                        case PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER:
                            edition = "Storage Server Standard (evaluation installation)";
                            break;
                        case PRODUCT_STORAGE_STANDARD_SERVER:
                            edition = "Storage Server Standard";
                            break;
                        case PRODUCT_STORAGE_STANDARD_SERVER_CORE:
                            edition = "Storage Server Standard (core installation)";
                            break;
                        case PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER:
                            edition = "Storage Server Workgroup (evaluation installation)";
                            break;
                        case PRODUCT_STORAGE_WORKGROUP_SERVER:
                            edition = "Storage Server Workgroup";
                            break;
                        case PRODUCT_STORAGE_WORKGROUP_SERVER_CORE:
                            edition = "Storage Server Workgroup (core installation)";
                            break;
                        case PRODUCT_ULTIMATE:
                            edition = "Ultimate";
                            break;
                        case PRODUCT_ULTIMATE_E:
                            edition = "Not supported";
                            break;
                        case PRODUCT_ULTIMATE_N:
                            edition = "Ultimate N";
                            break;
                        case PRODUCT_UNDEFINED:
                            edition = "An unknown product";
                            break;
                        case PRODUCT_WEB_SERVER:
                            edition = "Web Server (full installation)";
                            break;
                        case PRODUCT_WEB_SERVER_CORE:
                            edition = "Web Server (core installation)";
                            break;
                    }
                }

                #endregion VERSION 6

                s_Edition = edition;
                return edition;
            }
        }
        #endregion EDITION

        #region NAME
        private static string s_Name;
        /// <summary>
        /// Gets the name of the operating system running on this computer.
        /// </summary>
        public static string Name
        {
            get
            {
                if (s_Name != null)
                    return s_Name;  //***** RETURN *****//

                string name = "unknown";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    ManagementObjectCollection information = searcher.Get();
                    if (information != null)
                    {
                        foreach (ManagementObject obj in information)
                        {
                            name = obj["Caption"].ToString();
                        }
                    }
                    name = name.Replace("NT 5.1.2600", "XP");
                    name = name.Replace("NT 5.2.3790", "Server 2003");
                }


                s_Name = name;
                return name;
            }
        }
        #endregion NAME

        #region PINVOKE
        //#region GET
        //#region PRODUCT INFO
        //[DllImport("Kernel32.dll")]
        //internal static extern bool GetProductInfo(
        //    int osMajorVersion,
        //    int osMinorVersion,
        //    int spMajorVersion,
        //    int spMinorVersion,
        //    out int edition);
        //#endregion PRODUCT INFO

        //#region VERSION
        //[DllImport("kernel32.dll")]
        //private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);
        //#endregion VERSION
        //#endregion GET

        //#region OSVERSIONINFOEX
        //[StructLayout(LayoutKind.Sequential)]
        //private struct OSVERSIONINFOEX
        //{
        //    public int dwOSVersionInfoSize;
        //    public int dwMajorVersion;
        //    public int dwMinorVersion;
        //    public int dwBuildNumber;
        //    public int dwPlatformId;
        //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        //    public string szCSDVersion;
        //    public short wServicePackMajor;
        //    public short wServicePackMinor;
        //    public short wSuiteMask;
        //    public byte wProductType;
        //    public byte wReserved;
        //}
        //#endregion OSVERSIONINFOEX

        #region PRODUCT
        private const int PRODUCT_BUSINESS = 0x00000006;
        private const int PRODUCT_BUSINESS_N = 0x00000010;
        private const int PRODUCT_CLUSTER_SERVER = 0x00000012;
        private const int PRODUCT_CLUSTER_SERVER_V = 0x00000040;
        private const int PRODUCT_CORE = 0x00000065;
        private const int PRODUCT_CORE_COUNTRYSPECIFIC = 0x00000063;
        private const int PRODUCT_CORE_N = 0x00000062;
        private const int PRODUCT_CORE_SINGLELANGUAGE = 0x00000064;
        private const int PRODUCT_DATACENTER_EVALUATION_SERVER = 0x00000050;
        private const int PRODUCT_DATACENTER_SERVER = 0x00000008;
        private const int PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C;
        private const int PRODUCT_DATACENTER_SERVER_CORE_V = 0x00000027;
        private const int PRODUCT_DATACENTER_SERVER_V = 0x00000025;
        private const int PRODUCT_EDUCATION = 0x00000079;
        private const int PRODUCT_EDUCATION_N = 0x0000007A;
        private const int PRODUCT_ENTERPRISE = 0x00000004;
        private const int PRODUCT_ENTERPRISE_E = 0x00000046;
        private const int PRODUCT_ENTERPRISE_EVALUATION = 0x00000048;
        private const int PRODUCT_ENTERPRISE_N = 0x0000001B;
        private const int PRODUCT_ENTERPRISE_N_EVALUATION = 0x00000054;
        private const int PRODUCT_ENTERPRISE_S = 0x0000007D;
        private const int PRODUCT_ENTERPRISE_S_EVALUATION = 0x00000081;
        private const int PRODUCT_ENTERPRISE_S_N = 0x0000007E;
        private const int PRODUCT_ENTERPRISE_S_N_EVALUATION = 0x00000082;
        private const int PRODUCT_ENTERPRISE_SERVER = 0x0000000A;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;
        private const int PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F;
        private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL = 0x0000003C;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC = 0x0000003E;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT = 0x0000003B;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC = 0x0000003D;
        private const int PRODUCT_HOME_BASIC = 0x00000002;
        private const int PRODUCT_HOME_BASIC_E = 0x00000043;
        private const int PRODUCT_HOME_BASIC_N = 0x00000005;
        private const int PRODUCT_HOME_PREMIUM = 0x00000003;
        private const int PRODUCT_HOME_PREMIUM_E = 0x00000044;
        private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;
        private const int PRODUCT_HOME_PREMIUM_SERVER = 0x00000022;
        private const int PRODUCT_HOME_SERVER = 0x00000013;
        private const int PRODUCT_HYPERV = 0x0000002A;
        private const int PRODUCT_IOTUAP = 0x0000007B;
        private const int PRODUCT_IOTUAPCOMMERCIAL = 0x00000083;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F;
        private const int PRODUCT_MOBILE_CORE = 0x00000068;
        private const int PRODUCT_MOBILE_ENTERPRISE = 0x00000085;
        private const int PRODUCT_MULTIPOINT_PREMIUM_SERVER = 0x0000004D;
        private const int PRODUCT_MULTIPOINT_STANDARD_SERVER = 0x0000004C;
        private const int PRODUCT_PRO_WORKSTATION = 0x000000A1;
        private const int PRODUCT_PRO_WORKSTATION_N = 0x000000A2;
        private const int PRODUCT_PROFESSIONAL = 0x00000030;
        private const int PRODUCT_PROFESSIONAL_E = 0x00000045;
        private const int PRODUCT_PROFESSIONAL_N = 0x00000031;
        private const int PRODUCT_PROFESSIONAL_WMC = 0x00000067;
        private const int PRODUCT_SB_SOLUTION_SERVER = 0x00000032;
        private const int PRODUCT_SB_SOLUTION_SERVER_EM = 0x00000036;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS = 0x00000033;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM = 0x00000037;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;
        private const int PRODUCT_SERVER_FOUNDATION = 0x00000021;
        private const int PRODUCT_SMALLBUSINESS_SERVER = 0x00000009;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE = 0x0000003F;
        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER = 0x00000038;
        private const int PRODUCT_STANDARD_EVALUATION_SERVER = 0x0000004F;
        private const int PRODUCT_STANDARD_SERVER = 0x00000007;
        private const int PRODUCT_STANDARD_SERVER_CORE = 0x0000000D;
        private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;
        private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS = 0x00000034;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE = 0x00000035;
        private const int PRODUCT_STARTER = 0x0000000B;
        private const int PRODUCT_STARTER_E = 0x00000042;
        private const int PRODUCT_STARTER_N = 0x0000002F;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE = 0x0000002E;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER_CORE = 0x0000002B;
        private const int PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER = 0x00000060;
        private const int PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015;
        private const int PRODUCT_STORAGE_STANDARD_SERVER_CORE = 0x0000002C;
        private const int PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER = 0x0000005F;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER_CORE = 0x0000002D;
        private const int PRODUCT_ULTIMATE = 0x00000001;
        private const int PRODUCT_ULTIMATE_E = 0x00000047;
        private const int PRODUCT_ULTIMATE_N = 0x0000001C;
        private const int PRODUCT_UNDEFINED = 0x00000000;
        private const int PRODUCT_WEB_SERVER = 0x00000011;
        private const int PRODUCT_WEB_SERVER_CORE = 0x0000001D;

        #endregion PRODUCT

        #region VERSIONS
        private const int VER_NT_WORKSTATION = 1;
        private const int VER_NT_DOMAIN_CONTROLLER = 2;
        private const int VER_NT_SERVER = 3;
        private const int VER_SUITE_SMALLBUSINESS = 1;
        private const int VER_SUITE_ENTERPRISE = 2;
        private const int VER_SUITE_TERMINAL = 16;
        private const int VER_SUITE_DATACENTER = 128;
        private const int VER_SUITE_SINGLEUSERTS = 256;
        private const int VER_SUITE_PERSONAL = 512;
        private const int VER_SUITE_BLADE = 1024;
        #endregion VERSIONS
        #endregion PINVOKE

        #region SERVICE PACK

        private static string s_ServicePack;

        /// <summary>
        /// Gets the service pack information of the operating system running on this computer.
        /// </summary>
        public static string ServicePack
        {
            get
            {

                if (s_ServicePack != null)
                    return s_ServicePack;  //***** RETURN *****//

                s_ServicePack = String.Empty;

                int servicePackMajor = 0;
                int servicePackMinor = 0;

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    ManagementObjectCollection information = searcher.Get();
                    if (information != null)
                    {
                        foreach (ManagementObject obj in information)
                        {
                            servicePackMajor = (int)obj["ServicePackMajorVersion"];
                            servicePackMajor = (int)obj["ServicePackMinorVersion"];
                        }
                    }
                }

                if (servicePackMajor != 0)
                    s_ServicePack = servicePackMajor + "." + servicePackMinor;

                return s_ServicePack;
            }
        }
        #endregion SERVICE PACK

        #region VERSION
        #region BUILD
        /// <summary>
        /// Gets the build version number of the operating system running on this computer.
        /// </summary>
        public static int BuildVersion
        {
            get
            {
                return Environment.OSVersion.Version.Build;
            }
        }
        #endregion BUILD

        #region FULL
        #region STRING
        /// <summary>
        /// Gets the full version string of the operating system running on this computer.
        /// </summary>
        public static string VersionString
        {
            get
            {
                return Environment.OSVersion.Version.ToString();
            }
        }
        #endregion STRING

        #region VERSION
        /// <summary>
        /// Gets the full version of the operating system running on this computer.
        /// </summary>
        public static Version Version
        {
            get
            {
                return Environment.OSVersion.Version;
            }
        }
        #endregion VERSION
        #endregion FULL

        #region MAJOR
        /// <summary>
        /// Gets the major version number of the operating system running on this computer.
        /// </summary>
        public static int MajorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Major;
            }
        }
        #endregion MAJOR

        #region MINOR
        /// <summary>
        /// Gets the minor version number of the operating system running on this computer.
        /// </summary>
        public static int MinorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Minor;
            }
        }
        #endregion MINOR

        #region REVISION
        /// <summary>
        /// Gets the revision version number of the operating system running on this computer.
        /// </summary>
        public static int RevisionVersion
        {
            get
            {
                return Environment.OSVersion.Version.Revision;
            }
        }
        #endregion REVISION
        #endregion VERSION
    }
}
