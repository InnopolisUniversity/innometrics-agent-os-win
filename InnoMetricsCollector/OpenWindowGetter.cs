﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using InnoMetricsCollector.Profiler;
using log4net;
using HWND = System.IntPtr;

namespace InnoMetricsCollector
{
    internal class OpenWindowGetter
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly int LogicalProcessors = Environment.ProcessorCount;

        private static List<LUID> GpuLuids;
        //static LUID GpuLuid = new Adapter("\\\\?\\PCI#VEN_8086&DEV_0416&SUBSYS_397817AA&REV_06#3&11583659&0&10#{1ca05180-a699-450a-9a0c-de4fbe3ddd89}").Luid;

        private static bool _isInitialized;
        private static IOPerfCounter _perfIO;
        private static NetworkInterfacePerfCounter _perfNet;

        /// <summary>
        ///     Fetches all video devices available.
        /// </summary>
        private static void Initialize()
        {
            GpuLuids = new List<LUID>();
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (var entry in searcher.Get())
            {
                var gpuLuid = new Adapter("\\\\?\\" + entry["PNPDeviceID"].ToString().Replace("\\", "#") +
                                          "#{1ca05180-a699-450a-9a0c-de4fbe3ddd89}").Luid;
                GpuLuids.Add(gpuLuid);
            }

            _perfIO = new IOPerfCounter();
            _perfNet = new NetworkInterfacePerfCounter();

            searcher = new ManagementObjectSearcher("Select * From Win32_NetworkAdapter");
            foreach (var entry in searcher.Get())
                if (entry.Properties["NetConnectionID"].Value != null)
                {
                    // Windows inconsistency.. again
                    _perfNet.Initialize(entry.Properties["Description"].Value.ToString().
                            Replace("\\", "_").
                            Replace("/", "_").
                            Replace(")", "]").
                            Replace("(", "[").
                            Replace("#", "_"),
                        NetworkInterfacePerfCounter.NetworkPerfType.BytesSent);
                    break;
                }

            _isInitialized = true;
        }

        private static IEnumerable<HWND> FindWindows()
        {
            var res = new List<HWND>();

            EnumWindows(delegate(HWND wnd, HWND param)
            {
                //if (IsWindowVisible(wnd)) 
                res.Add(wnd);
                return true;
            }, HWND.Zero);

            return res;
        }

        private static void CheckNtStatus(NtStatus status)
        {
            if (status != NtStatus.Success) throw new Exception("Failed NtStatus");
        }

        private static unsafe TimeSpan GetGpuTime(IntPtr processHandle)
        {
            var sz = Environment.Is64BitOperatingSystem ? 808 : 800;
            foreach (var adapter in GpuLuids)
            {
                var foo = stackalloc byte[sz];
                *(int*) foo = 6;
                *(LUID*) (foo + 4) = adapter;
                *(IntPtr*) (foo + (Environment.Is64BitOperatingSystem ? 16 : 12)) = processHandle;
                var s = D3DKMTQueryStatistics(foo);
                if (s == NtStatus.Success)
                    return new TimeSpan(*(long*) (foo + (Environment.Is64BitOperatingSystem ? 24 : 16)));
                //Console.WriteLine("HERE");
            }

            return TimeSpan.Zero;
        }

        public static IDictionary<HWND, WindowInfo> GetOpenedWindowsProcessesInfo()
        {
            // TODO: this needs redesign; it queries duplicate values from the same processes
            
            var shellWindow = GetShellWindow();
            var processes = FindWindows()
                .Where(window => window != shellWindow)
                .Select(w =>
                {
                    GetWindowThreadProcessId(w, out var pid);
                    var wi = GetWindowInfoWithoutProcess(w);
                    var p = Process.GetProcessById(pid);
                    ProcessInfo pi;
                    try
                    {
                        pi = GetProcessInfo(p);
                    }
                    catch (Exception e)
                    {
                        pi = null;
                    }

                    return new {w, wi, p, pi};
                })
                .Where(p => p.pi != null)
                .ToArray();

            Thread.Sleep(1000);
            
            var now = DateTime.Now;
            var cpuTimes = processes.Select(p => p.p.TotalProcessorTime);
            var gpuTimes = processes.Select(p => GetGpuTime(p.p.Handle));


            foreach (var (p, time) in processes.Zip(cpuTimes, (p, t) => (p, t)))
            {
                var dt = now - p.pi.MeasurementTime;
                var dpt = time - p.pi.TotalProcessorTime;
                p.pi.CpuUsage = dpt.TotalSeconds / dt.TotalSeconds * 100 / LogicalProcessors;
            }

            foreach (var (p, time) in processes.Zip(gpuTimes, (p, t) => (p, t)))
            {
                var dt = now - p.pi.MeasurementTime;
                var dpt = time - p.pi.TotalGpuTime;
                p.pi.GpuUsage = dpt.TotalSeconds / dt.TotalSeconds * 100 / LogicalProcessors;
            }

            foreach (var p in processes) p.wi.ProcessInfo = p.pi;

            return processes
                .ToDictionary(t => t.w, t => t.wi);
        }

        // What the battery stuff is even doing here?!
        public static IDictionary<string, object> GetBatteryStatus()
        {
            var BatteryStatus = new Dictionary<string, object>();

            var query = new ObjectQuery("Select * FROM Win32_Battery");
            var searcher = new ManagementObjectSearcher(query);

            var collection = searcher.Get();

            foreach (var mo in collection)
            foreach (var property in mo.Properties)
                BatteryStatus.Add(property.Name, property.Value);
            //Console.WriteLine("Property {0}: Value is {1}", property.Name, property.Value);

            return BatteryStatus;
        }

        public static WindowInfo GetWindowInfo(HWND hWnd)
        {
            var res = GetWindowInfoWithoutProcess(hWnd);
            GetWindowThreadProcessId(hWnd, out var pid);
            res.ProcessInfo = GetProcessInfo(pid);
            return res;
        }

        private static WindowInfo GetWindowInfoWithoutProcess(HWND hWnd)
        {
            var windowTitle = "";
            var length = GetWindowTextLength(hWnd);
            if (length != 0)
            {
                // TODO: fix race condition
                var builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);
                windowTitle = builder.ToString();
            }


            return new WindowInfo
            {
                WindowTitle = windowTitle,
                IsForeground = GetForegroundWindow() == hWnd
            };
        }

        private static ProcessInfo GetProcessInfo(int pid)
        {
            return GetProcessInfo(Process.GetProcessById(pid));
        }

        private static ProcessInfo GetProcessInfo(Process process)
        {
            if (!_isInitialized) Initialize();


            // Does not fill cpu usage information, as it is faster to do in parallel for many processes
            var mainModule = process.MainModule;

            // TODO: probably not a cheap operation
            //var cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
            //cpuCounter.NextValue();
            //var cpuUsage = cpuCounter.NextValue();
            var cpuUsage = 0;

            _perfIO.Initialize(process, IOPerfCounter.IOPerfType.DataRate);

            Console.WriteLine(_perfIO.Category);
            //double testb = _perfNet.Pop(NetworkInterfacePerfCounter.NetworkPerfType.BytesSent);
            return new ProcessInfo
            {
                ProcessName = process.ProcessName,
                ProcessId = process.Id,
                MainModuleName = mainModule.ModuleName,
                MainModulePath = mainModule.FileName,
                MainModuleDescription = mainModule.FileVersionInfo.FileDescription,

                StartTime = process.StartTime,
                TotalProcessorTime = process.TotalProcessorTime,
                UserProcessorTime = process.UserProcessorTime,

                TotalGpuTime = GetGpuTime(process.Handle),

                MeasurementTime = DateTime.Now,

                RamVirtualSize = (int) (process.VirtualMemorySize64 / 1024 / 1024), // B -> MiB
                RamWorkingSetSize = (int) (process.WorkingSet64 / 1024 / 1024),

                CpuUsage = cpuUsage,
                IOUsage = _perfIO.Pop(IOPerfCounter.IOPerfType.DataRate),
                NetworkUsage = _perfNet.Pop(NetworkInterfacePerfCounter.NetworkPerfType.BytesSent)
            };
        }

        private class Adapter : IDisposable
        {
            private uint _handle;

            public Adapter(string name)
            {
                var n = new D3DKMT_OPENADAPTERFROMDEVICENAME {pDeviceName = name};

                CheckNtStatus(D3DKMTOpenAdapterFromDeviceName(ref n));

                _handle = n.handle;
                Luid = n.luid;
            }

            public LUID Luid { get; }

            public void Dispose()
            {
                ReleaseUnmanagedResources();
                GC.SuppressFinalize(this);
            }

            private void ReleaseUnmanagedResources()
            {
                if (_handle != 0)
                {
                    var close = new D3DKMT_CLOSEADAPTER {handle = _handle};
                    _handle = 0;
                    CheckNtStatus(D3DKMTCloseAdapter(ref close));
                }
            }

            ~Adapter()
            {
                ReleaseUnmanagedResources();
            }
        }

        public class ProcessInfo
        {
            public string ProcessName { get; set; }
            public int ProcessId { get; set; }
            public string MainModuleName { get; set; }
            public string MainModulePath { get; set; }
            public string MainModuleDescription { get; set; }
            public DateTime StartTime { get; set; }
            public TimeSpan TotalProcessorTime { get; set; }
            public TimeSpan UserProcessorTime { get; set; }
            public TimeSpan TotalGpuTime { get; set; }
            public DateTime MeasurementTime { get; set; }
            public int RamWorkingSetSize { get; set; }
            public int RamVirtualSize { get; set; }
            public double CpuUsage { get; set; }
            public double GpuUsage { get; set; }

            public double IOUsage { get; set; }
            public double NetworkUsage { get; set; }
        }

        public class WindowInfo
        {
            public string WindowTitle { get; set; }
            public bool IsForeground { get; set; }

            public ProcessInfo ProcessInfo { get; set; }
        }

        // TODO: move those into a separate class

        #region "Import USER32.dll"

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public enum NtStatus : uint
        {
            // Success
            Success = 0x00000000,
            Wait0 = 0x00000000,
            Wait1 = 0x00000001,
            Wait2 = 0x00000002,
            Wait3 = 0x00000003,
            Wait63 = 0x0000003f,
            Abandoned = 0x00000080,
            AbandonedWait0 = 0x00000080,
            AbandonedWait1 = 0x00000081,
            AbandonedWait2 = 0x00000082,
            AbandonedWait3 = 0x00000083,
            AbandonedWait63 = 0x000000bf,
            UserApc = 0x000000c0,
            KernelApc = 0x00000100,
            Alerted = 0x00000101,
            Timeout = 0x00000102,
            Pending = 0x00000103,
            Reparse = 0x00000104,
            MoreEntries = 0x00000105,
            NotAllAssigned = 0x00000106,
            SomeNotMapped = 0x00000107,
            OpLockBreakInProgress = 0x00000108,
            VolumeMounted = 0x00000109,
            RxActCommitted = 0x0000010a,
            NotifyCleanup = 0x0000010b,
            NotifyEnumDir = 0x0000010c,
            NoQuotasForAccount = 0x0000010d,
            PrimaryTransportConnectFailed = 0x0000010e,
            PageFaultTransition = 0x00000110,
            PageFaultDemandZero = 0x00000111,
            PageFaultCopyOnWrite = 0x00000112,
            PageFaultGuardPage = 0x00000113,
            PageFaultPagingFile = 0x00000114,
            CrashDump = 0x00000116,
            ReparseObject = 0x00000118,
            NothingToTerminate = 0x00000122,
            ProcessNotInJob = 0x00000123,
            ProcessInJob = 0x00000124,
            ProcessCloned = 0x00000129,
            FileLockedWithOnlyReaders = 0x0000012a,
            FileLockedWithWriters = 0x0000012b,

            // Informational
            Informational = 0x40000000,
            ObjectNameExists = 0x40000000,
            ThreadWasSuspended = 0x40000001,
            WorkingSetLimitRange = 0x40000002,
            ImageNotAtBase = 0x40000003,
            RegistryRecovered = 0x40000009,

            // Warning
            Warning = 0x80000000,
            GuardPageViolation = 0x80000001,
            DatatypeMisalignment = 0x80000002,
            Breakpoint = 0x80000003,
            SingleStep = 0x80000004,
            BufferOverflow = 0x80000005,
            NoMoreFiles = 0x80000006,
            HandlesClosed = 0x8000000a,
            PartialCopy = 0x8000000d,
            DeviceBusy = 0x80000011,
            InvalidEaName = 0x80000013,
            EaListInconsistent = 0x80000014,
            NoMoreEntries = 0x8000001a,
            LongJump = 0x80000026,
            DllMightBeInsecure = 0x8000002b,

            // Error
            Error = 0xc0000000,
            Unsuccessful = 0xc0000001,
            NotImplemented = 0xc0000002,
            InvalidInfoClass = 0xc0000003,
            InfoLengthMismatch = 0xc0000004,
            AccessViolation = 0xc0000005,
            InPageError = 0xc0000006,
            PagefileQuota = 0xc0000007,
            InvalidHandle = 0xc0000008,
            BadInitialStack = 0xc0000009,
            BadInitialPc = 0xc000000a,
            InvalidCid = 0xc000000b,
            TimerNotCanceled = 0xc000000c,
            InvalidParameter = 0xc000000d,
            NoSuchDevice = 0xc000000e,
            NoSuchFile = 0xc000000f,
            InvalidDeviceRequest = 0xc0000010,
            EndOfFile = 0xc0000011,
            WrongVolume = 0xc0000012,
            NoMediaInDevice = 0xc0000013,
            NoMemory = 0xc0000017,
            NotMappedView = 0xc0000019,
            UnableToFreeVm = 0xc000001a,
            UnableToDeleteSection = 0xc000001b,
            IllegalInstruction = 0xc000001d,
            AlreadyCommitted = 0xc0000021,
            AccessDenied = 0xc0000022,
            BufferTooSmall = 0xc0000023,
            ObjectTypeMismatch = 0xc0000024,
            NonContinuableException = 0xc0000025,
            BadStack = 0xc0000028,
            NotLocked = 0xc000002a,
            NotCommitted = 0xc000002d,
            InvalidParameterMix = 0xc0000030,
            ObjectNameInvalid = 0xc0000033,
            ObjectNameNotFound = 0xc0000034,
            ObjectNameCollision = 0xc0000035,
            ObjectPathInvalid = 0xc0000039,
            ObjectPathNotFound = 0xc000003a,
            ObjectPathSyntaxBad = 0xc000003b,
            DataOverrun = 0xc000003c,
            DataLate = 0xc000003d,
            DataError = 0xc000003e,
            CrcError = 0xc000003f,
            SectionTooBig = 0xc0000040,
            PortConnectionRefused = 0xc0000041,
            InvalidPortHandle = 0xc0000042,
            SharingViolation = 0xc0000043,
            QuotaExceeded = 0xc0000044,
            InvalidPageProtection = 0xc0000045,
            MutantNotOwned = 0xc0000046,
            SemaphoreLimitExceeded = 0xc0000047,
            PortAlreadySet = 0xc0000048,
            SectionNotImage = 0xc0000049,
            SuspendCountExceeded = 0xc000004a,
            ThreadIsTerminating = 0xc000004b,
            BadWorkingSetLimit = 0xc000004c,
            IncompatibleFileMap = 0xc000004d,
            SectionProtection = 0xc000004e,
            EasNotSupported = 0xc000004f,
            EaTooLarge = 0xc0000050,
            NonExistentEaEntry = 0xc0000051,
            NoEasOnFile = 0xc0000052,
            EaCorruptError = 0xc0000053,
            FileLockConflict = 0xc0000054,
            LockNotGranted = 0xc0000055,
            DeletePending = 0xc0000056,
            CtlFileNotSupported = 0xc0000057,
            UnknownRevision = 0xc0000058,
            RevisionMismatch = 0xc0000059,
            InvalidOwner = 0xc000005a,
            InvalidPrimaryGroup = 0xc000005b,
            NoImpersonationToken = 0xc000005c,
            CantDisableMandatory = 0xc000005d,
            NoLogonServers = 0xc000005e,
            NoSuchLogonSession = 0xc000005f,
            NoSuchPrivilege = 0xc0000060,
            PrivilegeNotHeld = 0xc0000061,
            InvalidAccountName = 0xc0000062,
            UserExists = 0xc0000063,
            NoSuchUser = 0xc0000064,
            GroupExists = 0xc0000065,
            NoSuchGroup = 0xc0000066,
            MemberInGroup = 0xc0000067,
            MemberNotInGroup = 0xc0000068,
            LastAdmin = 0xc0000069,
            WrongPassword = 0xc000006a,
            IllFormedPassword = 0xc000006b,
            PasswordRestriction = 0xc000006c,
            LogonFailure = 0xc000006d,
            AccountRestriction = 0xc000006e,
            InvalidLogonHours = 0xc000006f,
            InvalidWorkstation = 0xc0000070,
            PasswordExpired = 0xc0000071,
            AccountDisabled = 0xc0000072,
            NoneMapped = 0xc0000073,
            TooManyLuidsRequested = 0xc0000074,
            LuidsExhausted = 0xc0000075,
            InvalidSubAuthority = 0xc0000076,
            InvalidAcl = 0xc0000077,
            InvalidSid = 0xc0000078,
            InvalidSecurityDescr = 0xc0000079,
            ProcedureNotFound = 0xc000007a,
            InvalidImageFormat = 0xc000007b,
            NoToken = 0xc000007c,
            BadInheritanceAcl = 0xc000007d,
            RangeNotLocked = 0xc000007e,
            DiskFull = 0xc000007f,
            ServerDisabled = 0xc0000080,
            ServerNotDisabled = 0xc0000081,
            TooManyGuidsRequested = 0xc0000082,
            GuidsExhausted = 0xc0000083,
            InvalidIdAuthority = 0xc0000084,
            AgentsExhausted = 0xc0000085,
            InvalidVolumeLabel = 0xc0000086,
            SectionNotExtended = 0xc0000087,
            NotMappedData = 0xc0000088,
            ResourceDataNotFound = 0xc0000089,
            ResourceTypeNotFound = 0xc000008a,
            ResourceNameNotFound = 0xc000008b,
            ArrayBoundsExceeded = 0xc000008c,
            FloatDenormalOperand = 0xc000008d,
            FloatDivideByZero = 0xc000008e,
            FloatInexactResult = 0xc000008f,
            FloatInvalidOperation = 0xc0000090,
            FloatOverflow = 0xc0000091,
            FloatStackCheck = 0xc0000092,
            FloatUnderflow = 0xc0000093,
            IntegerDivideByZero = 0xc0000094,
            IntegerOverflow = 0xc0000095,
            PrivilegedInstruction = 0xc0000096,
            TooManyPagingFiles = 0xc0000097,
            FileInvalid = 0xc0000098,
            InstanceNotAvailable = 0xc00000ab,
            PipeNotAvailable = 0xc00000ac,
            InvalidPipeState = 0xc00000ad,
            PipeBusy = 0xc00000ae,
            IllegalFunction = 0xc00000af,
            PipeDisconnected = 0xc00000b0,
            PipeClosing = 0xc00000b1,
            PipeConnected = 0xc00000b2,
            PipeListening = 0xc00000b3,
            InvalidReadMode = 0xc00000b4,
            IoTimeout = 0xc00000b5,
            FileForcedClosed = 0xc00000b6,
            ProfilingNotStarted = 0xc00000b7,
            ProfilingNotStopped = 0xc00000b8,
            NotSameDevice = 0xc00000d4,
            FileRenamed = 0xc00000d5,
            CantWait = 0xc00000d8,
            PipeEmpty = 0xc00000d9,
            CantTerminateSelf = 0xc00000db,
            InternalError = 0xc00000e5,
            InvalidParameter1 = 0xc00000ef,
            InvalidParameter2 = 0xc00000f0,
            InvalidParameter3 = 0xc00000f1,
            InvalidParameter4 = 0xc00000f2,
            InvalidParameter5 = 0xc00000f3,
            InvalidParameter6 = 0xc00000f4,
            InvalidParameter7 = 0xc00000f5,
            InvalidParameter8 = 0xc00000f6,
            InvalidParameter9 = 0xc00000f7,
            InvalidParameter10 = 0xc00000f8,
            InvalidParameter11 = 0xc00000f9,
            InvalidParameter12 = 0xc00000fa,
            MappedFileSizeZero = 0xc000011e,
            TooManyOpenedFiles = 0xc000011f,
            Cancelled = 0xc0000120,
            CannotDelete = 0xc0000121,
            InvalidComputerName = 0xc0000122,
            FileDeleted = 0xc0000123,
            SpecialAccount = 0xc0000124,
            SpecialGroup = 0xc0000125,
            SpecialUser = 0xc0000126,
            MembersPrimaryGroup = 0xc0000127,
            FileClosed = 0xc0000128,
            TooManyThreads = 0xc0000129,
            ThreadNotInProcess = 0xc000012a,
            TokenAlreadyInUse = 0xc000012b,
            PagefileQuotaExceeded = 0xc000012c,
            CommitmentLimit = 0xc000012d,
            InvalidImageLeFormat = 0xc000012e,
            InvalidImageNotMz = 0xc000012f,
            InvalidImageProtect = 0xc0000130,
            InvalidImageWin16 = 0xc0000131,
            LogonServer = 0xc0000132,
            DifferenceAtDc = 0xc0000133,
            SynchronizationRequired = 0xc0000134,
            DllNotFound = 0xc0000135,
            IoPrivilegeFailed = 0xc0000137,
            OrdinalNotFound = 0xc0000138,
            EntryPointNotFound = 0xc0000139,
            ControlCExit = 0xc000013a,
            PortNotSet = 0xc0000353,
            DebuggerInactive = 0xc0000354,
            CallbackBypass = 0xc0000503,
            PortClosed = 0xc0000700,
            MessageLost = 0xc0000701,
            InvalidMessage = 0xc0000702,
            RequestCanceled = 0xc0000703,
            RecursiveDispatch = 0xc0000704,
            LpcReceiveBufferExpected = 0xc0000705,
            LpcInvalidConnectionUsage = 0xc0000706,
            LpcRequestsNotAllowed = 0xc0000707,
            ResourceInUse = 0xc0000708,
            ProcessIsProtected = 0xc0000712,
            VolumeDirty = 0xc0000806,
            FileCheckedOut = 0xc0000901,
            CheckOutRequired = 0xc0000902,
            BadFileType = 0xc0000903,
            FileTooLarge = 0xc0000904,
            FormsAuthRequired = 0xc0000905,
            VirusInfected = 0xc0000906,
            VirusDeleted = 0xc0000907,
            TransactionalConflict = 0xc0190001,
            InvalidTransaction = 0xc0190002,
            TransactionNotActive = 0xc0190003,
            TmInitializationFailed = 0xc0190004,
            RmNotActive = 0xc0190005,
            RmMetadataCorrupt = 0xc0190006,
            TransactionNotJoined = 0xc0190007,
            DirectoryNotRm = 0xc0190008,
            CouldNotResizeLog = 0xc0190009,
            TransactionsUnsupportedRemote = 0xc019000a,
            LogResizeInvalidSize = 0xc019000b,
            RemoteFileVersionMismatch = 0xc019000c,
            CrmProtocolAlreadyExists = 0xc019000f,
            TransactionPropagationFailed = 0xc0190010,
            CrmProtocolNotFound = 0xc0190011,
            TransactionSuperiorExists = 0xc0190012,
            TransactionRequestNotValid = 0xc0190013,
            TransactionNotRequested = 0xc0190014,
            TransactionAlreadyAborted = 0xc0190015,
            TransactionAlreadyCommitted = 0xc0190016,
            TransactionInvalidMarshallBuffer = 0xc0190017,
            CurrentTransactionNotValid = 0xc0190018,
            LogGrowthFailed = 0xc0190019,
            ObjectNoLongerExists = 0xc0190021,
            StreamMiniversionNotFound = 0xc0190022,
            StreamMiniversionNotValid = 0xc0190023,
            MiniversionInaccessibleFromSpecifiedTransaction = 0xc0190024,
            CantOpenMiniversionWithModifyIntent = 0xc0190025,
            CantCreateMoreStreamMiniversions = 0xc0190026,
            HandleNoLongerValid = 0xc0190028,
            NoTxfMetadata = 0xc0190029,
            LogCorruptionDetected = 0xc0190030,
            CantRecoverWithHandleOpen = 0xc0190031,
            RmDisconnected = 0xc0190032,
            EnlistmentNotSuperior = 0xc0190033,
            RecoveryNotNeeded = 0xc0190034,
            RmAlreadyStarted = 0xc0190035,
            FileIdentityNotPersistent = 0xc0190036,
            CantBreakTransactionalDependency = 0xc0190037,
            CantCrossRmBoundary = 0xc0190038,
            TxfDirNotEmpty = 0xc0190039,
            IndoubtTransactionsExist = 0xc019003a,
            TmVolatile = 0xc019003b,
            RollbackTimerExpired = 0xc019003c,
            TxfAttributeCorrupt = 0xc019003d,
            EfsNotAllowedInTransaction = 0xc019003e,
            TransactionalOpenNotAllowed = 0xc019003f,
            TransactedMappingUnsupportedRemote = 0xc0190040,
            TxfMetadataAlreadyPresent = 0xc0190041,
            TransactionScopeCallbacksNotSet = 0xc0190042,
            TransactionRequiredPromotion = 0xc0190043,
            CannotExecuteFileInTransaction = 0xc0190044,
            TransactionsNotFrozen = 0xc0190045,

            MaximumNtStatus = 0xffffffff
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public readonly uint LowPart;
            public readonly int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct D3DKMT_OPENADAPTERFROMDEVICENAME
        {
            [MarshalAs(UnmanagedType.LPWStr)] public string pDeviceName;
            public readonly uint handle;
            public readonly LUID luid;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct D3DKMT_CLOSEADAPTER
        {
            public uint handle;
        }

        [DllImport("gdi32.dll")]
        private static extern NtStatus D3DKMTOpenAdapterFromDeviceName(ref D3DKMT_OPENADAPTERFROMDEVICENAME name);

        [DllImport("gdi32.dll")]
        private static extern NtStatus D3DKMTCloseAdapter(ref D3DKMT_CLOSEADAPTER adapter);

        [DllImport("gdi32.dll")]
        private static extern unsafe NtStatus D3DKMTQueryStatistics(byte* aaa);

        #endregion
    }
}