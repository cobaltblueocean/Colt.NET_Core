using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Utility
{
    public static class MappingStrings
    {
        /// <summary>
        /// Name of the disk drive from which the Windows operating system starts.
        /// </summary>
        public const string BootDevice = "BootDevice";
        public const string BuildNumber = "BuildNumber";
        public const string BuildType = "BuildType";
        public const string Caption = "Caption";
        public const string CodeSet = "CodeSet";
        public const string CountryCode = "CountryCode";
        public const string CreationClassName = "CreationClassName";
        public const string CSCreationClassName = "CSCreationClassName";
        public const string CSDVersion = "CSDVersion";
        public const string CSName = "CSName";
        public const string CurrentTimeZone = "CurrentTimeZone";
        public const string DataExecutionPrevention_32BitApplications = "DataExecutionPrevention_32BitApplications";
        public const string DataExecutionPrevention_Available = "DataExecutionPrevention_Available";
        public const string DataExecutionPrevention_Drivers = "DataExecutionPrevention_Drivers";
        public const string DataExecutionPrevention_SupportPolicy = "DataExecutionPrevention_SupportPolicy";
        public const string Debug = "Debug";
        public const string Description = "Description";
        public const string Distributed = "Distributed";
        public const string EncryptionLevel = "EncryptionLevel";
        public const string ForegroundApplicationBoost = "ForegroundApplicationBoost";
        public const string FreePhysicalMemory = "FreePhysicalMemory";
        public const string FreeSpaceInPagingFiles = "FreeSpaceInPagingFiles";
        public const string FreeVirtualMemory = "FreeVirtualMemory";
        public const string InstallDate = "InstallDate";
        public const string LargeSystemCache = "LargeSystemCache";
        public const string LastBootUpTime = "LastBootUpTime";
        public const string LocalDateTime = "LocalDateTime";
        public const string Locale = "Locale";
        public const string Manufacturer = "Manufacturer";
        public const string MaxNumberOfProcesses = "MaxNumberOfProcesses";
        public const string MaxProcessMemorySize = "MaxProcessMemorySize";
        public const string MUILanguages = "MUILanguages";
        public const string Name = "Name";
        public const string NumberOfLicensedUsers = "NumberOfLicensedUsers";
        public const string NumberOfProcesses = "NumberOfProcesses";
        public const string NumberOfUsers = "NumberOfUsers";
        public const string OperatingSystemSKU = "OperatingSystemSKU";
        public const string Organization = "Organization";
        public const string OSArchitecture = "OSArchitecture";
        public const string OSLanguage = "OSLanguage";
        public const string OSProductSuite = "OSProductSuite";
        public const string OSType = "OSType";
        public const string OtherTypeDescription = "OtherTypeDescription";
        public const string PAEEnabled = "PAEEnabled";
        public const string PlusProductID = "PlusProductID";
        public const string PlusVersionNumber = "PlusVersionNumber";
        public const string PortableOperatingSystem = "PortableOperatingSystem";
        public const string Primary = "Primary";
        public const string Product = "ProductType";
        public const string QuantumLength = "QuantumLength";
        public const string Quantum = "QuantumType";
        public const string RegisteredUser = "RegisteredUser";
        public const string SerialNumber = "SerialNumber";
        public const string ServicePackMajorVersion = "ServicePackMajorVersion";
        public const string ServicePackMinorVersion = "ServicePackMinorVersion";
        public const string SizeStoredInPagingFiles = "SizeStoredInPagingFiles";
        public const string Status = "Status";
        public const string SuiteMask = "SuiteMask";
        public const string SystemDevice = "SystemDevice";
        public const string SystemDirectory = "SystemDirectory";
        public const string SystemDrive = "SystemDrive";
        public const string TotalSwapSpaceSize = "TotalSwapSpaceSize";
        public const string TotalVirtualMemorySize = "TotalVirtualMemorySize";
        public const string TotalVisibleMemorySize = "TotalVisibleMemorySize";
        public const string Version = "Version";
        public const string WindowsDirectory = "WindowsDirectory";

        public enum DataExecutionPrevention_SupportPolicyType
        {
            AlwaysOff = 0,
            AlwaysOn = 1,
            OptIn = 2,
            OptOut = 3
        }

        public enum EncryptionLevelType
        {
            FourtyBit = 0,
            OneHundredTwentyEightBit = 1,
            nBit = 2
        }

        public enum ForegroundApplicationBoostType
        {
            None = 0,
            Minimum = 1,
            Maximum = 2
        }

        public enum LargeSystemCacheType
        {
            OptimizeForApplications = 0,
            OptimizeForSystemPerformance = 1
        }

        public enum OperatingSystemSKUType
        {
            PRODUCT_UNDEFINED = 0,
            PRODUCT_ULTIMATE = 1,
            PRODUCT_HOME_BASIC = 2,
            PRODUCT_HOME_PREMIUM = 3,
            PRODUCT_ENTERPRISE = 4,
            PRODUCT_BUSINESS = 6,
            PRODUCT_STANDARD_SERVER = 7,
            PRODUCT_DATACENTER_SERVER = 8,
            PRODUCT_SMALLBUSINESS_SERVER = 9,
            PRODUCT_ENTERPRISE_SERVER = 10,
            PRODUCT_STARTER = 1,
            PRODUCT_DATACENTER_SERVER_CORE = 12,
            PRODUCT_STANDARD_SERVER_CORE = 13,
            PRODUCT_ENTERPRISE_SERVER_CORE = 14,
            PRODUCT_WEB_SERVER = 17,
            PRODUCT_HOME_SERVER = 19,
            PRODUCT_STORAGE_EXPRESS_SERVER = 20,
            PRODUCT_STORAGE_STANDARD_SERVER = 21,
            PRODUCT_STORAGE_WORKGROUP_SERVER = 22,
            PRODUCT_STORAGE_ENTERPRISE_SERVER = 23,
            PRODUCT_SERVER_FOR_SMALLBUSINESS = 24,
            PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 25,
            PRODUCT_ENTERPRISE_N = 27,
            PRODUCT_ULTIMATE_N = 28,
            PRODUCT_WEB_SERVER_CORE = 29,
            PRODUCT_STANDARD_SERVER_V = 36,
            PRODUCT_DATACENTER_SERVER_V = 37,
            PRODUCT_ENTERPRISE_SERVER_V = 38,
            PRODUCT_DATACENTER_SERVER_CORE_V = 39,
            PRODUCT_STANDARD_SERVER_CORE_V = 40,
            PRODUCT_ENTERPRISE_SERVER_CORE_V = 41,
            PRODUCT_HYPERV = 42,
            PRODUCT_STORAGE_EXPRESS_SERVER_CORE = 43,
            PRODUCT_STORAGE_STANDARD_SERVER_CORE = 44,
            PRODUCT_STORAGE_WORKGROUP_SERVER_CORE = 45,
            PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE = 46,
            PRODUCT_SB_SOLUTION_SERVER = 50,
            PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE = 63,
            PRODUCT_CLUSTER_SERVER_V = 64,
            PRODUCT_CORE_ARM = 97,
            PRODUCT_CORE = 101,
            PRODUCT_PROFESSIONAL_WMC = 103,
            PRODUCT_MOBILE_CORE = 104,
            PRODUCT_IOTUAP = 123,
            PRODUCT_DATACENTER_NANO_SERVER = 143,
            PRODUCT_STANDARD_NANO_SERVER = 144,
            PRODUCT_DATACENTER_WS_SERVER_CORE = 147,
            PRODUCT_STANDARD_WS_SERVER_CORE = 148,
        }

        public enum OperatingSystemType
        {
            Unknown = 0,
            Other = 1,
            MACOS = 2,
            ATTUNIX = 3,
            DGUX = 4,
            DECNT = 5,
            Digital_Unix = 6,
            OpenVMS = 7,
            HPUX = 8,
            AIX = 9,
            MVS = 10,
            OS400 = 11,
            OS_2 = 12,
            JavaVM = 13,
            MSDOS = 14,
            WIN3x = 15,
            WIN95 = 16,
            WIN98 = 17,
            WINNT = 18,
            WINCE = 19,
            NCR3000 = 20,
            NetWare = 21,
            OSF = 22,
            DC_OS = 23,
            Reliant_UNIX = 24,
            SCO_UnixWare = 25,
            SCO_OpenServer = 26,
            Sequent = 27,
            IRIX = 28,
            Solaris = 29,
            SunOS = 30,
            U6000 = 31,
            ASERIES = 32,
            TandemNSK = 33,
            TandemNT = 34,
            BS2000 = 35,
            LINUX = 36,
            Lynx = 37,
            XENIX = 38,
            VM_ESA = 39,
            Interactive_UNIX = 40,
            BSDUNIX = 41,
            FreeBSD = 42,
            NetBSD = 43,
            GNU_Hurd = 44,
            OS9 = 45,
            MACH_Kernel = 46,
            Inferno = 47,
            QNX = 48,
            EPOC = 49,
            IxWorks = 50,
            VxWorks = 51,
            MiNT = 52,
            BeOS = 53,
            HP_MPE = 54,
            NextStep = 55,
            PalmPilot = 56,
            Rhapsody = 57,
            Windows2000 = 58,
            Dedicated = 59,
            OS_390 = 60,
            VSE = 61,
            TPF = 62,
        }

        public enum ProductType
        {
            WorkStation = 0,
            DomainController = 1,
            Server = 2,
        }

        public enum QuantumLengthType
        {
            Unknown = 0,
            OneTick = 1,
            TwoTicks = 2
        }

        public enum QuantumType
        {
            Unknown = 0,
            Fixed = 1,
            Variable = 2,
        }

    }
}
