#pragma warning disable 1006

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnsafeAndSpanExtension;
using BOOL = System.Int32;
using BYTE = System.Byte;
using CONTEXT = CelogParserLib.ARM.CONTEXT;
using DWORD = System.UInt32;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;
using HANDLE = CelogParserLib.CeHandle;
using LPBYTE = CelogParserLib.CePtr;
using LPVOID = CelogParserLib.CePtr;
using UCHAR = System.Byte;
using ULONG_PTR = System.UInt32;
using ULONGLONG = System.UInt64;
using USHORT = System.UInt16;
using WCHAR = System.Char;
using WORD = System.UInt16;

namespace CelogParserLib
{
    [StructLayout(LayoutKind.Sequential, Size = 8 * sizeof(uint))]
    public struct FixedLengthUInt32Array8
    {
        public UInt32 First;
    }
    [StructLayout(LayoutKind.Sequential, Size = 16 * sizeof(uint))]
    public struct FixedLengthUInt32Array15
    {
        public UInt32 First;
    }
    [StructLayout(LayoutKind.Sequential, Size = 64 * sizeof(uint))]
    public struct FixedLengthUInt32Array64
    {
        public UInt32 First;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    unsafe partial struct EXCEPTION_RECORD
    {
        public DWORD ExceptionCode;
        public DWORD ExceptionFlags;
        public /*EXCEPTION_RECORD*/ LPVOID ExceptionRecord;
        public LPVOID ExceptionAddress;
        public DWORD NumberParameters;

        private const int EXCEPTION_NONCONTINUABLE = 0x1;
        public const int EXCEPTION_MAXIMUM_PARAMETERS = 15;

        public FixedLengthUInt32Array15 ExceptionInformation;

        static EXCEPTION_RECORD() => System.Diagnostics.Debug.Assert(sizeof(EXCEPTION_RECORD) == 80);
    };

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    partial struct EVENT_DESCRIPTOR
    {
        public USHORT Id;
        public UCHAR Version;
        public UCHAR Channel;
        public UCHAR Level;
        public UCHAR Opcode;
        public USHORT Task;
        public ULONGLONG Keyword;
        static unsafe EVENT_DESCRIPTOR() => System.Diagnostics.Debug.Assert(sizeof(EVENT_DESCRIPTOR) == 16);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles")]
    static unsafe partial class Interop
    {
        private static Span<TElem> GetTrailArray<TFrom, TElem>(ref TFrom self, int bufferSize)
            where TElem : unmanaged
            where TFrom : unmanaged
        {
            if (bufferSize < sizeof(TFrom))
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            int arrayLength = (bufferSize - sizeof(TFrom)) / sizeof(TElem);
            return AltMemoryMarshal.CreateSpan(ref Unsafe.As<TFrom, TElem>(ref Unsafe.Add(ref self, 1)), arrayLength);
        }

        private static ReadOnlySpan<TElem> GetTrailReadOnlyArray<TFrom, TElem>(in TFrom self, int bufferSize)
            where TElem : unmanaged
            where TFrom : unmanaged
        {
            if (bufferSize < sizeof(TFrom))
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            int arrayLength = (bufferSize - sizeof(TFrom)) / sizeof(TElem);
            return ReadOnlyRefUnsafe.CreateReadOnlySpan(in ReadOnlyRefUnsafe.As<TFrom, TElem>(in ReadOnlyRefUnsafe.Add(in self, 1)), arrayLength);
        }

        public const int MAX_PATH = 260;
        #region Common header for all event data

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_HEADER
        {
            internal CEL_HEADER(uint value) => _Value = value;

            readonly DWORD _Value;
            public readonly ushort Length => (ushort)(_Value & (CEL_HEADER_LENGTH_MASK | CEL_HEADER_CPU_MASK));
            public readonly ushort ID => (ushort)((_Value & CEL_HEADER_ID_MASK) >> 16);
            // DWORD   Reserved : 1;
            public readonly bool fFileTime => (_Value & 0x40000000) != 0;
            public readonly bool fTimeStamp => (_Value & CEL_HEADER_TIMESTAMP) != 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_HEADER_V3
        {
            internal CEL_HEADER_V3(uint value) => _Value = value;

            readonly DWORD _Value;
            public readonly ushort Length => (ushort)(_Value & CEL_HEADER_LENGTH_MASK);
            public readonly byte CPU => (byte)((_Value & CEL_HEADER_CPU_MASK) >> 12);
            public readonly ushort ID => (ushort)((_Value & CEL_HEADER_ID_MASK) >> 16);
            public readonly bool fFileTime => (_Value & 0x40000000) != 0;
            //DWORD Reserved : 1;
            public readonly bool fTimeStamp => (_Value & CEL_HEADER_TIMESTAMP) != 0;
        }

        public const uint CEL_HEADER_TIMESTAMP = 0x80000000;
        public const uint CEL_HEADER_CPU_MASK = 0x0000F000;
        public const uint CEL_HEADER_LENGTH_MASK = 0x00000FFF;
        public const uint CEL_HEADER_ID_MASK = 0x3FFF0000;
        #endregion

        #region Critical Section
        public const int CELID_CS_INIT = 11;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_CRITSEC_INIT
        {
            public HANDLE hCS;
        }

        public const int CELID_CS_DELETE = 12;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_CRITSEC_DELETE
        {
            public HANDLE hCS;
        }

        public const int CELID_CS_ENTER = 1;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_CRITSEC_ENTER
        {
            public HANDLE hCS;
            public HANDLE hOwnerThread;
        }

        public const int CELID_CS_LEAVE = 2;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_CRITSEC_LEAVE
        {
            public HANDLE hCS;
            public HANDLE hOwnerThread;
        }
        #endregion

        #region Events
        public const int CELID_EVENT_CREATE = 3;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EVENT_CREATE
        {
            public HANDLE hEvent;
            public readonly bool fManual => (dwReserved & 1 << 0) != 0;
            public readonly bool fInitialState => (dwReserved & 1 << 1) != 0;
            public readonly bool fCreate => (dwReserved & 1 << 2) != 0;

            readonly DWORD dwReserved;

            //WCHAR   szName[0];
        }

        public static ReadOnlySpan<WCHAR> GetName(this in CEL_EVENT_CREATE self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_EVENT_CREATE, WCHAR>(in self, bufferSize);

        public const int CELID_EVENT_SET = 4;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EVENT_SET
        {
            public HANDLE hEvent;
        }

        public const int CELID_EVENT_RESET = 5;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EVENT_RESET
        {
            public HANDLE hEvent;
        }

        public const int CELID_EVENT_PULSE = 6;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EVENT_PULSE
        {
            public HANDLE hEvent;
        }

        public const int CELID_EVENT_DELETE = 8;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EVENT_DELETE
        {
            public HANDLE hEvent;
        }
        #endregion

        #region WaitForSingleObject / WaitForMultipleObjects
        public const int CELID_WAIT_MULTI = 9;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_WAIT_MULTI
        {
            public DWORD dwTimeout;
            public readonly bool fWaitAll => (dwReserved & 1 << 0) != 0;
            readonly DWORD dwReserved;
            //HANDLE  hHandles[0];
        }
        public static ReadOnlySpan<HANDLE> GetHandles(this in CEL_WAIT_MULTI self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_WAIT_MULTI, HANDLE>(in self, bufferSize);
        #endregion

        #region Sleep
        public const int CELID_SLEEP = 10;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_SLEEP
        {
            public DWORD dwTimeout;
        }
        #endregion

        #region Semaphores
        public const int CELID_SEM_CREATE = 15;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_SEM_CREATE
        {
            public HANDLE hSem;
            public DWORD dwInitCount;
            public DWORD dwMaxCount;
            //WCHAR   szName[0];
        }
        public static ReadOnlySpan<WCHAR> GetName(this in CEL_SEM_CREATE self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_SEM_CREATE, WCHAR>(in self, bufferSize);

        public const int CELID_SEM_RELEASE = 16;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_SEM_RELEASE
        {
            public HANDLE hSem;
            public DWORD dwReleaseCount;
            public DWORD dwPreviousCount;
        }

        public const int CELID_SEM_DELETE = 18;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_SEM_DELETE
        {
            public HANDLE hSem;
        }
        #endregion

        #region Heap
        public const int CELID_HEAP_CREATE = 25;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_HEAP_CREATE
        {
            public DWORD dwOptions;
            public DWORD dwInitSize;
            public DWORD dwMaxSize;
            public HANDLE hHeap;
            public DWORD dwTID;
            public DWORD dwPID;
        }

        public const int CELID_HEAP_ALLOC = 26;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_HEAP_ALLOC
        {
            public HANDLE hHeap;
            public DWORD dwFlags;
            public DWORD dwBytes;
            public DWORD lpMem;
            public DWORD dwTID;
            public DWORD dwPID;
            public DWORD dwCallerPID;
            //DWORD   adwStackTrace[0];
        }
        public static ReadOnlySpan<DWORD> GetStackTrace(this in CEL_HEAP_ALLOC self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_HEAP_ALLOC, DWORD>(in self, bufferSize);

        public const int CELID_HEAP_REALLOC = 27;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_HEAP_REALLOC
        {
            public HANDLE hHeap;
            public DWORD dwFlags;
            public DWORD dwBytes;
            public DWORD lpMemOld;
            public DWORD lpMem;
            public DWORD dwTID;
            public DWORD dwPID;
            public DWORD dwCallerPID;
            //DWORD   adwStackTrace[0];
        }
        public static ReadOnlySpan<DWORD> GetStackTrace(this in CEL_HEAP_REALLOC self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_HEAP_REALLOC, DWORD>(in self, bufferSize);

        public const int CELID_HEAP_FREE = 28;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_HEAP_FREE
        {
            public HANDLE hHeap;
            public DWORD dwFlags;
            public DWORD lpMem;
            public DWORD dwTID;
            public DWORD dwPID;
            public DWORD dwCallerPID;
            //DWORD   adwStackTrace[0];
        }
        public static ReadOnlySpan<DWORD> GetStackTrace(this in CEL_HEAP_FREE self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_HEAP_FREE, DWORD>(in self, bufferSize);

        public const int CELID_HEAP_DESTROY = 29;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_HEAP_DESTROY
        {
            public HANDLE hHeap;
            public DWORD dwTID;
            public DWORD dwPID;
        }
        #endregion

        #region Virtual Memory
        public const int CELID_VIRTUAL_ALLOC_EX = 32;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_VIRTUAL_ALLOC_EX
        {
            public HANDLE hProcess;
            public DWORD dwResult;
            public DWORD dwAddress;
            public DWORD dwSize;
            public DWORD dwType;
            public DWORD dwProtect;
        }
        public const int CELID_VIRTUAL_COPY_EX = 33;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_VIRTUAL_COPY_EX
        {
            public HANDLE hDestProc;
            public DWORD dwDest;
            public HANDLE hSrcProc;
            public DWORD dwSource;
            public DWORD dwSize;
            public DWORD dwProtect;
        }

        public const int CELID_VIRTUAL_FREE_EX = 34;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_VIRTUAL_FREE_EX
        {
            public HANDLE hProcess;
            public DWORD dwAddress;
            public DWORD dwSize;
            public DWORD dwType;
        }
        #endregion

        #region  Paging and memory-mapped files

        public const int CELID_SYSTEM_PAGE_IN = 111;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_SYSTEM_PAGE_IN
        {
            public DWORD dwAddress;
            public readonly bool fReadWrite => (dwReserved & 1 << 0) != 0;
            public readonly bool fEndPageIn => (dwReserved & 1 << 1) != 0;
            public readonly bool fSuccess => (dwReserved & 1 << 2) != 0;
            readonly DWORD dwReserved;
            public HANDLE hProcess;
        }

        public const int CELID_SYSTEM_PAGE_OUT = 79;

        public const int CELID_MAPFILE_CREATE_EX = 112;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MAPFILE_CREATE_EX
        {
            public HANDLE hMap;
            public DWORD flProtect;
            public DWORD dwMapFlags;
            public DWORD dwMaxSizeHigh;
            public DWORD dwMaxSizeLow;
            //WCHAR   szName[0];  
        }
        public static ReadOnlySpan<WCHAR> GetName(this in CEL_MAPFILE_CREATE_EX self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_MAPFILE_CREATE_EX, WCHAR>(in self, bufferSize);

        public const int CELID_MAPFILE_DESTROY = 39;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MAPFILE_DESTROY
        {
            public HANDLE hMap;
        }
        public const int CELID_MAPFILE_VIEW_OPEN_EX = 113;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MAPFILE_VIEW_OPEN_EX
        {
            public HANDLE hMap;
            public HANDLE hProcess;
            public DWORD dwDesiredAccess;
            public DWORD dwFileOffsetHigh;
            public DWORD dwFileOffsetLow;
            public DWORD dwLen;
            public LPVOID lpBaseAddress;
        }

        public const int CELID_MAPFILE_VIEW_CLOSE = 41;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MAPFILE_VIEW_CLOSE
        {
            public HANDLE hProcess;
            public LPVOID lpBaseAddress;
        }

        public const int CELID_MAPFILE_VIEW_FLUSH = 114;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MAPFILE_VIEW_FLUSH
        {
            public HANDLE hProcess;
            public LPVOID lpBaseAddress;
            public DWORD dwLen;
            public DWORD dwNumPages;
            public WORD wFlushFlags;
        }

        // wFlushFlags 
        public const WORD CEL_MAPFLUSH_FLAGMASK = 0xFFF0;
        public const WORD CEL_MAPFLUSH_TYPEMASK = 0x000F;
        public enum CEL_MAPFLUSH_TYPE
        {
            CEL_INVALID_MAPFLUSH = 0,
            CEL_FlushMapSimple = 1,
            CEL_FlushMapAtomic = 2,
            CEL_ValidateFile = 3,
            CEL_FlushMapGather = 4,

            CEL_NUM_MAPFLUSH_TYPES
        }
        public const ushort CEL_MAPFLUSH_BEGIN = 0x00000010;
        public const ushort CEL_MAPFLUSH_FULLDISCARD = 0x00000020;
        public const ushort CEL_MAPFLUSH_NOWRITEOUT = 0x00000040;

        public const int CELID_CACHE_ALLOCVIEW = 115;
        public const int CELID_CACHE_FREEVIEW = 116;
        public const int CELID_CACHE_FLUSHVIEW = 117;
        #endregion

        #region Scheduler
        public const int CELID_THREAD_SWITCH = 45;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_THREAD_SWITCH
        {
            public HANDLE hThread;
        }

        public const int CELID_THREAD_MIGRATE = 46;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_THREAD_MIGRATE
        {
            public HANDLE hProcess;
            public DWORD dwReserved;
        }

        public const int CELID_THREAD_CREATE = 47;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_THREAD_CREATE
        {
            public HANDLE hThread;
            public HANDLE hProcess;
            public HANDLE hModule;
            public DWORD dwStartAddr;
            public int nPriority;
            //WCHAR   szName[0];         
        }
        public static ReadOnlySpan<WCHAR> GetName(this in CEL_THREAD_CREATE self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_THREAD_CREATE, WCHAR>(in self, bufferSize);

        public const int CELID_THREAD_TERMINATE = 49;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_THREAD_TERMINATE
        {
            public HANDLE hThread;
        }

        public const int CELID_THREAD_DELETE = 50;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_THREAD_DELETE
        {
            public HANDLE hThread;
        }

        public const int CELID_PROCESS_CREATE = 51;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_PROCESS_CREATE
        {
            public HANDLE hProcess;
            public DWORD dwVMBase;
            //WCHAR   szName[0];                  
        }
        public static ReadOnlySpan<WCHAR> GetName(this in CEL_PROCESS_CREATE self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_PROCESS_CREATE, WCHAR>(in self, bufferSize);

        public const int CELID_PROCESS_TERMINATE = 53;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_PROCESS_TERMINATE
        {
            public HANDLE hProcess;
        }

        public const int CELID_PROCESS_DELETE = 54;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_PROCESS_DELETE
        {
            public HANDLE hProcess;
        }

        public const int CELID_THREAD_SUSPEND = 55;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_THREAD_SUSPEND
        {
            public HANDLE hThread;
        }

        public const int CELID_THREAD_RESUME = 56;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_THREAD_RESUME
        {
            public HANDLE hThread;
        }

        public const int CELID_THREAD_QUANTUMEXPIRE = 57;

        public const int CELID_EXTRA_PROCESS_INFO = 58;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EXTRA_PROCESS_INFO
        {
            public HANDLE hProcess;
            public DWORD dwCodeBase;
            public DWORD dwVMLen;
            public DWORD dwOID;
            //WCHAR   szFullPath[0];
        }
        public static ReadOnlySpan<WCHAR> GetFullPath(this in CEL_EXTRA_PROCESS_INFO self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_EXTRA_PROCESS_INFO, WCHAR>(in self, bufferSize);

        public const int CELID_SYSTEM_INVERT = 82;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_SYSTEM_INVERT
        {
            public HANDLE hThread;
            public int nPriority;
        }

        public const int CELID_THREAD_PRIORITY = 83;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_THREAD_PRIORITY
        {
            public HANDLE hThread;
            public int nPriority;
        }

        public const int CELID_THREAD_QUANTUM = 84;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_THREAD_QUANTUM
        {
            public HANDLE hThread;
            public DWORD dwQuantum;
        }
        #endregion

        #region Modules / Loader
        public const int CELID_MODULE_LOAD = 85;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MODULE_LOAD
        {
            public HANDLE hProcess;
            public HANDLE hModule;
            public DWORD dwBase;
            //WCHAR   szName[0];
        }
        public static ReadOnlySpan<WCHAR> GetName(this in CEL_MODULE_LOAD self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_MODULE_LOAD, WCHAR>(in self, bufferSize);

        public const int CELID_MODULE_FREE = 86;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MODULE_FREE
        {
            public HANDLE hProcess;
            public HANDLE hModule;
        }

        public const int CELID_MODULE_FAILED_LOAD = 119;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MODULE_FAILED_LOAD
        {
            public DWORD dwProcessId;
            public DWORD dwFlags;
            public DWORD dwError;
            public fixed WCHAR szName[MAX_PATH];
        }
        public static Span<WCHAR> GetName(this ref CEL_MODULE_FAILED_LOAD self)
            => AltMemoryMarshal.CreateSpan(ref self.szName[0], MAX_PATH);

        public const uint CEL_MODULE_FLAG_KERNEL = ((DWORD)1);
        public const uint CEL_MODULE_FLAG_DATAONLY = ((DWORD)2);

        public const int CELID_EXTRA_MODULE_INFO = 98;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EXTRA_MODULE_INFO
        {
            public HANDLE hModule;
            public DWORD dwVMLen;
            public CelogModuleFlag dwModuleFlags;
            public DWORD dwOID;
            //WCHAR   szFullPath[0];     
        }
        public static ReadOnlySpan<WCHAR> GetFullPath(this in CEL_EXTRA_MODULE_INFO self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_EXTRA_MODULE_INFO, WCHAR>(in self, bufferSize);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_PROCESS_REFCOUNT
        {
            public HANDLE hProcess;
            public DWORD dwRefCount;
        }

        public const int CELID_MODULE_REFERENCES = 97;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MODULE_REFERENCES
        {
            public HANDLE hModule;
            public CEL_PROCESS_REFCOUNT @ref/*[1]*/;
        }
        public static ReadOnlySpan<CEL_PROCESS_REFCOUNT> GetRefs(this in CEL_MODULE_REFERENCES self, int bufferSize)
        {
            if (bufferSize < sizeof(CEL_MODULE_REFERENCES))
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            int arrayLength = (bufferSize - sizeof(HANDLE)) / sizeof(CEL_PROCESS_REFCOUNT);
            return AltMemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(self).@ref, arrayLength);
        }
        #endregion

        #region Mutexes
        public const int CELID_MUTEX_CREATE = 60;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MUTEX_CREATE
        {
            public HANDLE hMutex;
            //WCHAR   szName[0];
        }
        public static ReadOnlySpan<WCHAR> GetName(this in CEL_MUTEX_CREATE self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_MUTEX_CREATE, WCHAR>(in self, bufferSize);

        public const int CELID_MUTEX_RELEASE = 61;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MUTEX_RELEASE
        {
            public HANDLE hMutex;
        }

        public const int CELID_MUTEX_DELETE = 63;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MUTEX_DELETE
        {
            public HANDLE hMutex;
        }
        #endregion

        #region Data types for logging raw data

        public const int CELID_RAW_LONG = 70;
        public const int CELID_RAW_ULONG = 71;
        public const int CELID_RAW_SHORT = 72;
        public const int CELID_RAW_USHORT = 73;
        public const int CELID_RAW_WCHAR = 74;
        public const int CELID_RAW_CHAR = 75;
        public const int CELID_RAW_UCHAR = 76;
        public const int CELID_RAW_FLOAT = 77;
        public const int CELID_RAW_DOUBLE = 78;

        #endregion

        #region Miscellaneous
        public const int CELID_SYSTEM_TLB = 80;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_SYSTEM_TLB
        {
            public DWORD dwCount;
        }

        public const int CELID_INTERRUPTS = 87;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_INT_DATA
        {
            public DWORD dwTimeStamp;
            public WORD wSysIntr;
            public WORD wNestingLevel;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_INTERRUPTS
        {
            public DWORD dwDiscarded;
            //CEL_INT_DATA IntData[0];
        }
        public static ReadOnlySpan<CEL_INT_DATA> GetIntData(this in CEL_INTERRUPTS self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_INTERRUPTS, CEL_INT_DATA>(in self, bufferSize);

        public const int CELID_KCALL_ENTER = 88;
        public const int CELID_KCALL_LEAVE = 89;

        public const int CELID_FLAGGED = 90;

        public const int CELID_CALLSTACK = 91;
        public const int CELID_CALLSTACK_EX = 95;

        public const int CELID_CEPERF = 92;

        public const int CELID_TIMER_START = 93;
        public const int CELID_TIMER_STOP = 94;

        public const int CELID_LOWMEM_SIGNALLED = 96;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_LOWMEM_DATA
        {
            public int pageFreeCount;
            public int cpNeed;
            public int cpLowThreshold;
            public int cpCriticalThreshold;
            public int cpLowBlockSize;
            public int cpCriticalBlockSize;
        }
        #endregion

        #region Memtrack
        public const int CELID_MEMTRACK_DETACHP = 99;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MEMTRACK_DETACHP { public CEL_PROCESS_TERMINATE _; }

        public const int CELID_MEMTRACK_BASELINE = 102;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_MEMTRACK_BASELINE
        {
            public DWORD dwReserved;
        }
        #endregion

        #region  Boot time

        public const int CELID_BOOT_TIME = 103;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_BOOT_TIME
        {
            public CelogBootTimeAction dwAction;
            internal const int BOOT_TIME_LAUNCHING_FS = 01;
            internal const int BOOT_TIME_FS_INITED = 10;
            internal const int BOOT_TIME_FS_OBJ_STORE_INITIALIZED = 11;
            internal const int BOOT_TIME_FS_FILES_INITIALIZED = 12;
            internal const int BOOT_TIME_FS_REG_INITIALIZED = 13;
            internal const int BOOT_TIME_FS_DB_INITIALIZED = 14;
            internal const int BOOT_TIME_FS_LAUNCH = 15;
            internal const int BOOT_TIME_DEV_ACTIVATE = 20;
            internal const int BOOT_TIME_DEV_FINISHED = 21;
            internal const int BOOT_TIME_GWES_FINISHED = 30;
            internal const int BOOT_TIME_SYSTEM_STARTED = 40;
            internal const int BOOT_TIME_START_DELAYED_WORK = 50;
            //WCHAR szName[0];
        }
        public static ReadOnlySpan<WCHAR> GetName(this in CEL_BOOT_TIME self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_BOOT_TIME, WCHAR>(in self, bufferSize);

        #endregion

        #region CELog GDI calls
        public const int CELID_GDI = 104;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_GDI_INFO
        {
            public CelogGdiOp dwGDIOp;
            public const int CEL_GDI_AddFontResource = 0;
            public const int CEL_GDI_PatBlt = 1;
            public const int CEL_GDI_BitBlt = 2;
            public const int CEL_GDI_CombineRgn = 3;
            public const int CEL_GDI_CreateBitmap = 4;
            public const int CEL_GDI_CreateCompatibleBitmap = 5;
            public const int CEL_GDI_CreateCompatibleDC = 6;
            public const int CEL_GDI_CreateDIBPatternBrushPt = 7;
            public const int CEL_GDI_CreateFontIndirectW = 8;
            public const int CEL_GDI_CreateRectRgnIndirect = 9;
            public const int CEL_GDI_CreatePenIndirect = 10;
            public const int CEL_GDI_CreateSolidBrush = 11;
            public const int CEL_GDI_DeleteDC = 12;
            public const int CEL_GDI_DeleteObject = 13;
            public const int CEL_GDI_DrawEdge = 14;
            public const int CEL_GDI_DrawFocusRect = 15;
            public const int CEL_GDI_DrawTextW = 16;
            public const int CEL_GDI_Ellipse = 17;
            public const int CEL_GDI_EnumFontFamiliesW = 18;
            public const int CEL_GDI_EnumFontsW = 19;
            public const int CEL_GDI_ExcludeClipRect = 20;
            public const int CEL_GDI_ExtTextOutW = 21;
            public const int CEL_GDI_SetTextAlign = 22;
            public const int CEL_GDI_GetTextAlign = 23;
            public const int CEL_GDI_FillRect = 24;
            public const int CEL_GDI_GetBkColor = 25;
            public const int CEL_GDI_GetBkMode = 26;
            public const int CEL_GDI_GetClipRgn = 27;
            public const int CEL_GDI_GetClipBox = 28;
            public const int CEL_GDI_GetCurrentObject = 29;
            public const int CEL_GDI_GetDeviceCaps = 30;
            public const int CEL_GDI_GetNearestColor = 31;
            public const int CEL_GDI_GetObjectW = 32;
            public const int CEL_GDI_GetObjectType = 33;
            public const int CEL_GDI_GetPixel = 34;
            public const int CEL_GDI_GetRegionData = 35;
            public const int CEL_GDI_GetSysColorBrush = 36;
            public const int CEL_GDI_GetRgnBox = 37;
            public const int CEL_GDI_GetStockObject = 38;
            public const int CEL_GDI_GetTextColor = 39;
            public const int CEL_GDI_GetTextExtentExPointW = 40;
            public const int CEL_GDI_GetTextFaceW = 41;
            public const int CEL_GDI_GetTextMetricsW = 42;
            public const int CEL_GDI_GetCharWidth32 = 43;
            public const int CEL_GDI_IntersectClipRect = 44;
            public const int CEL_GDI_MaskBlt = 45;
            public const int CEL_GDI_OffsetRgn = 46;
            public const int CEL_GDI_MoveToEx = 47;
            public const int CEL_GDI_LineTo = 48;
            public const int CEL_GDI_GetCurrentPositionEx = 49;
            public const int CEL_GDI_Polygon = 50;
            public const int CEL_GDI_Polyline = 51;
            public const int CEL_GDI_PtInRegion = 52;
            public const int CEL_GDI_Rectangle = 53;
            public const int CEL_GDI_RectInRegion = 54;
            public const int CEL_GDI_RemoveFontResourceW = 55;
            public const int CEL_GDI_RestoreDC = 56;
            public const int CEL_GDI_RoundRect = 57;
            public const int CEL_GDI_SaveDC = 58;
            public const int CEL_GDI_SelectClipRgn = 59;
            public const int CEL_GDI_SelectObject = 60;
            public const int CEL_GDI_SetBkColor = 61;
            public const int CEL_GDI_SetBkMode = 62;
            public const int CEL_GDI_SetBrushOrgEx = 63;
            public const int CEL_GDI_SetPixel = 64;
            public const int CEL_GDI_SetTextColor = 65;
            public const int CEL_GDI_StretchBlt = 66;
            public const int CEL_GDI_StretchDIBits = 67;
            public const int CEL_GDI_CloseEnhMetaFile = 68;
            public const int CEL_GDI_CreateEnhMetaFileW = 69;
            public const int CEL_GDI_DeleteEnhMetaFile = 70;
            public const int CEL_GDI_PlayEnhMetaFile = 71;
            public const int CEL_GDI_CreatePalette = 72;
            public const int CEL_GDI_SelectPalette = 73;
            public const int CEL_GDI_RealizePalette = 74;
            public const int CEL_GDI_GetPaletteEntries = 75;
            public const int CEL_GDI_SetPaletteEntries = 76;
            public const int CEL_GDI_GetSystemPaletteEntries = 77;
            public const int CEL_GDI_GetNearestPaletteIndex = 78;
            public const int CEL_GDI_GetDIBColorTable = 79;
            public const int CEL_GDI_SetDIBColorTable = 80;
            public const int CEL_GDI_CreatePen = 81;
            public const int CEL_GDI_StartDocW = 82;
            public const int CEL_GDI_EndDoc = 83;
            public const int CEL_GDI_StartPage = 84;
            public const int CEL_GDI_EndPage = 85;
            public const int CEL_GDI_AbortDoc = 86;
            public const int CEL_GDI_SetAbortProc = 87;
            public const int CEL_GDI_CreateDCW = 88;
            public const int CEL_GDI_CreateRectRgn = 89;
            public const int CEL_GDI_ExtCreateRegion = 90;
            public const int CEL_GDI_FillRgn = 91;
            public const int CEL_GDI_SetROP2 = 92;
            public const int CEL_GDI_RectVisible = 93;
            public const int CEL_GDI_SetRectRgn = 94;
            public const int CEL_GDI_CreatePatternBrush = 95;
            public const int CEL_GDI_CreateBitmapFromPointer = 96;
            public const int CEL_GDI_SetViewportOrgEx = 97;
            public const int CEL_GDI_TransparentImage = 98;
            public const int CEL_GDI_TranslateCharsetInfo = 99;
            public const int CEL_GDI_ExtEscape = 100;
            public const int CEL_GDI_SetDIBitsToDevice = 101;
            public const int CEL_GDI_GradientFill = 102;
            public const int CEL_GDI_InvertRect = 103;
            public const int CEL_GDI_GetCharABCWidths = 104;
            public const int CEL_GDI_GetStretchBltMode = 105;
            public const int CEL_GDI_SetStretchBltMode = 106;
            public const int CEL_GDI_SetLayout = 107;
            public const int CEL_GDI_GetLayout = 108;
            public const int CEL_GDI_BitmapEscape = 109;

            public DWORD dwEntryTime;
            public DWORD dwContext;
            public DWORD dwContext2;
            public DWORD dwContext3;
            public DWORD dwContext4;
        }
        #endregion

        #region CELog RDP Info
        public const int CELID_RDP = 105;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_RDP_INFO
        {
            public BYTE bOrderType;
            public BYTE bOrder;
            public DWORD dwTimeSpent;
        }
        #endregion

        #region Kernel Profiler Events

        public const int CELID_PROFILER_START = 106;
        public const int CELID_PROFILER_STOP = 107;
        public const int CELID_PROFILER_MARK = 125;
        public const int CELID_PROFILER_NAME = 126;

        public const int CELID_MONTECARLO_HIT = 108;
        public const int CELID_OEMPROFILER_HIT = 109;
        public const int CELID_APP_PROFILER_HIT = 118;

        public const int CELID_CAP_OVERHEAD = 120;
        public const int CELID_CALLCAP_ENTER = 121;
        public const int CELID_CALLCAP_EXIT = 122;
        public const int CELID_FASTCAP_ENTER = 123;
        public const int CELID_FASTCAP_EXIT = 124;
        #endregion

        #region   Kernel Debug Events

        public const int CELID_DEBUG_MSG = 69;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_DEBUG_MSG
        {
            public DWORD pid;
            public DWORD tid;
            //WCHAR szMessage[0];
        }
        public static ReadOnlySpan<WCHAR> GetMessage(this in CEL_DEBUG_MSG self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_DEBUG_MSG, WCHAR>(in self, bufferSize);

        public const int CELID_DEBUG_TRAP = 110;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_DEBUG_TRAP
        {
            public WORD wFlags;

            public const int CEL_DEBUG_TRAP_SECONDCHANCE = 0x0001;

            public WORD cbStackTraceOffset;
            public EXCEPTION_RECORD er;
            public CONTEXT context;
            //DWORD adwStackTrace[0];
        }
        public static ReadOnlySpan<DWORD> GetStackTrace(this in CEL_DEBUG_TRAP self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_DEBUG_TRAP, DWORD>(in self, bufferSize);
        #endregion

        #region  GWES Events

        // Reserved range of events for GWES to log
        public const int CELID_GWES = 200;
        public const int CELID_GWES_MAX = 299;
        #endregion

        #region Eventlog Events

        public const int CELID_EVENTLOG_CHANNEL_MSG = 300;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EVENTLOG_CHANNEL_MSG
        {
            public Guid ProviderID;
            public DWORD CPUType;
            public EVENT_DESCRIPTOR EventDescriptor;
            public DWORD cbUserData;
            public DWORD eUserDataCount;
            public DWORD SequenceNumber;
            public DWORD AlignmentPadding;
            public fixed BYTE UserData[4];
        }

        public const int CELID_EVENTLOG_WPP_HEADER = 301;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EVENTLOG_WPP_CLOSE
        {
            public long StopTime;
        }

        public const int CELID_EVENTLOG_WPP_MSG = 302;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_EVENTLOG_WPP_MSG
        {
            public DWORD cMissedEvents;
            public FILETIME KernelTime;
            public FILETIME UserTime;
        }
        #endregion

        #region CELog control info
        public const int CELID_DATA_LOSS = 150;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_DATA_LOSS
        {
            public DWORD dwBytes;
        }

        public const int CELID_SYNC_END = 0x1FFE;

        public const int CELID_LOG_MARKER = 0x1FFF;

        // Marker for CELOG_VERSION=1 & 2
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_LOG_MARKER
        {
            public DWORD dwFrequency;
            public DWORD dwDefaultQuantum;
            public DWORD dwVersion;
        }

        // Marker for CELOG_VERSION=3
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct CEL_LOG_MARKER_V3
        {
            public DWORD dwFrequency;
            public DWORD dwDefaultQuantum;
            public DWORD dwVersion;
            public DWORD dwNumCPUs;
        }

        public const int CELID_USER = 0x2000;
        public const int CELID_MAX = 0x3FFF;
        #endregion

        #region Deprecated events
        [Obsolete]
        public const int CELID_EVENT_CLOSE = 7;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_EVENT_CLOSE
        {
            public HANDLE hEvent;
        }

        [Obsolete]
        public const int CELID_SEM_CLOSE = 17;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_SEM_CLOSE
        {
            public HANDLE hSem;
        }

        [Obsolete]
        public const int CELID_THREAD_CLOSE = 48;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_THREAD_CLOSE
        {
            public HANDLE hThread;
        }

        [Obsolete]
        public const int CELID_PROCESS_CLOSE = 52;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_PROCESS_CLOSE
        {
            public HANDLE hProcess;
        }

        [Obsolete]
        public const int CELID_MUTEX_CLOSE = 62;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_MUTEX_CLOSE
        {
            public HANDLE hMutex;
        }

        [Obsolete]
        public const int CELID_VIRTUAL_ALLOC = 35;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_VIRTUAL_ALLOC
        {
            public DWORD dwResult;
            public DWORD dwAddress;
            public DWORD dwSize;
            public DWORD dwType;
            public DWORD dwProtect;
            //BYTE    bReserved[0];
        }

        [Obsolete]
        public const int CELID_VIRTUAL_COPY = 36;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_VIRTUAL_COPY
        {
            public DWORD dwDest;
            public DWORD dwSource;
            public DWORD dwSize;
            public DWORD dwProtect;
        }

        [Obsolete]
        public const int CELID_VIRTUAL_FREE = 37;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_VIRTUAL_FREE
        {
            public DWORD dwAddress;
            public DWORD dwSize;
            public DWORD dwType;
            //BYTE    bReserved[0];
        }

        [Obsolete]
        public const int CELID_SYSTEM_PAGE = 81;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_SYSTEM_PAGE
        {
            public DWORD dwAddress;
            public readonly DWORD fReadWrite => dwReserved & 1 << 0;
            public readonly DWORD fEndPageIn => dwReserved & 1 << 1;
            public readonly DWORD fSuccess => dwReserved & 1 << 2;
            public readonly DWORD dwReserved;
        }

        [Obsolete]
        public const int CELID_MAPFILE_CREATE = 38;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_MAPFILE_CREATE
        {
            public HANDLE hMap;
            public DWORD flProtect;
            public DWORD dwMapFlags;
            public DWORD dwMaxSize;
            //WCHAR   szName[0];
        }

        [Obsolete]
        public static ReadOnlySpan<WCHAR> GetName(this in CEL_MAPFILE_CREATE self, int bufferSize)
            => GetTrailReadOnlyArray<CEL_MAPFILE_CREATE, WCHAR>(in self, bufferSize);

        [Obsolete]
        public const int CELID_MAPFILE_VIEW_OPEN = 40;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_MAPFILE_VIEW_OPEN
        {
            public HANDLE hMap;
            public HANDLE hProcess;
            public DWORD dwDesiredAccess;
            public DWORD dwFileOffset;
            public DWORD dwLen;
            public LPVOID lpBaseAddress;
        }

        [Obsolete]
        public const int CELID_MAPFILE_FLUSH = 42;
        [StructLayout(LayoutKind.Sequential, Pack = 4), Obsolete]
        public partial struct CEL_MAPFILE_FLUSH
        {
            public LPVOID lpBaseAddress;
            public DWORD dwLen;
            public WORD wFlushFlags;
            public WORD wNumPages;
        }
        #endregion

        #region Shared data access
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct MAPHEADER_V1
        {
            public DWORD dwBufSize;
            public LPBYTE pWrite;
            public LPBYTE pRead;
            public BOOL fSetEvent;
            readonly BYTE bReserved;
            public DWORD dwLostBytes;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct MAPHEADER_V2
        {
            public DWORD dwBufSize;
            readonly LPBYTE pWrite;
            readonly LPBYTE pRead;
            public BOOL fSetEvent;
            readonly BYTE bReserved;
            public DWORD dwLostBytes;

            public DWORD dwVersion;
            public DWORD dwBufferStart;
            public DWORD dwWriteOffset;

            public DWORD dwReadOffset;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public partial struct MAPHEADER_V3
        {
            public DWORD dwBufSize;
            readonly LPBYTE pWrite;
            readonly LPBYTE pRead;
            public BOOL fSetEvent;
            readonly BYTE bReserved;
            public DWORD dwLostBytes;

            public DWORD dwVersion;
            public DWORD dwBufferStart;
            public DWORD dwWriteOffset;

            public DWORD dwReadOffset;

            public DWORD Signature;
            public BOOL IsLocked;
            public DWORD dwSyncBufferStart;
            public DWORD dwSyncBufferSize;
            public DWORD dwSyncWriteOffset;
            public DWORD dwSyncReadOffset;
            public DWORD dwIntBufferStart;
            public DWORD dwIntBufferSize;
            public DWORD dwIntWriteOffset;
            public DWORD dwIntReadOffset;

        }
        #endregion
    }
}
