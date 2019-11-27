using System;
using static CelogParserLib.Interop;

namespace CelogParserLib
{
    public enum CelogId : ushort
    {

        AppProfilerHit = CELID_APP_PROFILER_HIT,

        BootTime = CELID_BOOT_TIME,

        CacheAllocview = CELID_CACHE_ALLOCVIEW,

        CacheFlushview = CELID_CACHE_FLUSHVIEW,

        CacheFreeview = CELID_CACHE_FREEVIEW,

        CallcapEnter = CELID_CALLCAP_ENTER,

        CallcapExit = CELID_CALLCAP_EXIT,

        Callstack = CELID_CALLSTACK,

        CallstackEx = CELID_CALLSTACK_EX,

        CapOverhead = CELID_CAP_OVERHEAD,

        Ceperf = CELID_CEPERF,

        CsDelete = CELID_CS_DELETE,

        CsEnter = CELID_CS_ENTER,

        CsInit = CELID_CS_INIT,

        CsLeave = CELID_CS_LEAVE,

        DataLoss = CELID_DATA_LOSS,

        DebugMsg = CELID_DEBUG_MSG,

        DebugTrap = CELID_DEBUG_TRAP,

        [Obsolete]
        EventClose = CELID_EVENT_CLOSE,

        EventCreate = CELID_EVENT_CREATE,

        EventDelete = CELID_EVENT_DELETE,

        EventPulse = CELID_EVENT_PULSE,

        EventReset = CELID_EVENT_RESET,

        EventSet = CELID_EVENT_SET,

        EventlogChannelMsg = CELID_EVENTLOG_CHANNEL_MSG,

        EventlogWppHeader = CELID_EVENTLOG_WPP_HEADER,

        EventlogWppMsg = CELID_EVENTLOG_WPP_MSG,

        ExtraModuleInfo = CELID_EXTRA_MODULE_INFO,

        ExtraProcessInfo = CELID_EXTRA_PROCESS_INFO,

        FastcapEnter = CELID_FASTCAP_ENTER,

        FastcapExit = CELID_FASTCAP_EXIT,

        Flagged = CELID_FLAGGED,

        Gdi = CELID_GDI,

        Gwes = CELID_GWES,

        GwesMax = CELID_GWES_MAX,

        HeapAlloc = CELID_HEAP_ALLOC,

        HeapCreate = CELID_HEAP_CREATE,

        HeapDestroy = CELID_HEAP_DESTROY,

        HeapFree = CELID_HEAP_FREE,

        HeapRealloc = CELID_HEAP_REALLOC,

        Interrupts = CELID_INTERRUPTS,

        KcallEnter = CELID_KCALL_ENTER,

        KcallLeave = CELID_KCALL_LEAVE,

        LogMarker = CELID_LOG_MARKER,

        LowmemSignalled = CELID_LOWMEM_SIGNALLED,

        [Obsolete]
        MapfileCreate = CELID_MAPFILE_CREATE,

        MapfileCreateEx = CELID_MAPFILE_CREATE_EX,

        MapfileDestroy = CELID_MAPFILE_DESTROY,

        [Obsolete]
        MapfileFlush = CELID_MAPFILE_FLUSH,

        MapfileViewClose = CELID_MAPFILE_VIEW_CLOSE,

        MapfileViewFlush = CELID_MAPFILE_VIEW_FLUSH,

        [Obsolete]
        MapfileViewOpen = CELID_MAPFILE_VIEW_OPEN,

        MapfileViewOpenEx = CELID_MAPFILE_VIEW_OPEN_EX,

        Max = CELID_MAX,

        MemtrackBaseline = CELID_MEMTRACK_BASELINE,

        MemtrackDetachp = CELID_MEMTRACK_DETACHP,

        ModuleFailedLoad = CELID_MODULE_FAILED_LOAD,

        ModuleFree = CELID_MODULE_FREE,

        ModuleLoad = CELID_MODULE_LOAD,

        ModuleReferences = CELID_MODULE_REFERENCES,

        MonteCarloHit = CELID_MONTECARLO_HIT,

        [Obsolete]
        MutexClose = CELID_MUTEX_CLOSE,

        MutexCreate = CELID_MUTEX_CREATE,

        MutexDelete = CELID_MUTEX_DELETE,

        MutexRelease = CELID_MUTEX_RELEASE,

        OemprofilerHit = CELID_OEMPROFILER_HIT,

        [Obsolete]
        ProcessClose = CELID_PROCESS_CLOSE,

        ProcessCreate = CELID_PROCESS_CREATE,

        ProcessDelete = CELID_PROCESS_DELETE,

        ProcessTerminate = CELID_PROCESS_TERMINATE,

        ProfilerMark = CELID_PROFILER_MARK,

        ProfilerName = CELID_PROFILER_NAME,

        ProfilerStart = CELID_PROFILER_START,

        ProfilerStop = CELID_PROFILER_STOP,

        RawChar = CELID_RAW_CHAR,

        RawDouble = CELID_RAW_DOUBLE,

        RawFloat = CELID_RAW_FLOAT,

        RawLong = CELID_RAW_LONG,

        RawShort = CELID_RAW_SHORT,

        RawUchar = CELID_RAW_UCHAR,

        RawUlong = CELID_RAW_ULONG,

        RawUshort = CELID_RAW_USHORT,

        RawWchar = CELID_RAW_WCHAR,

        Rdp = CELID_RDP,

        [Obsolete]
        SemaphoreClose = CELID_SEM_CLOSE,

        SemaphoreCreate = CELID_SEM_CREATE,

        SemaphoreDelete = CELID_SEM_DELETE,

        SemaphoreRelease = CELID_SEM_RELEASE,

        Sleep = CELID_SLEEP,

        SyncEnd = CELID_SYNC_END,

        SystemInvert = CELID_SYSTEM_INVERT,

        [Obsolete]
        SystemPage = CELID_SYSTEM_PAGE,

        SystemPageIn = CELID_SYSTEM_PAGE_IN,

        SystemPageOut = CELID_SYSTEM_PAGE_OUT,

        SystemTlb = CELID_SYSTEM_TLB,

        [Obsolete]
        ThreadClose = CELID_THREAD_CLOSE,

        ThreadCreate = CELID_THREAD_CREATE,

        ThreadDelete = CELID_THREAD_DELETE,

        ThreadMigrate = CELID_THREAD_MIGRATE,

        ThreadPriority = CELID_THREAD_PRIORITY,

        ThreadQuantum = CELID_THREAD_QUANTUM,

        ThreadQuantumexpire = CELID_THREAD_QUANTUMEXPIRE,

        ThreadResume = CELID_THREAD_RESUME,

        ThreadSupend = CELID_THREAD_SUSPEND,

        ThreadSwitch = CELID_THREAD_SWITCH,

        ThreadTerminate = CELID_THREAD_TERMINATE,

        TimerStart = CELID_TIMER_START,

        TimerStop = CELID_TIMER_STOP,

        User = CELID_USER,

        [Obsolete]
        VirtualAlloc = CELID_VIRTUAL_ALLOC,

        VirtualAllocEx = CELID_VIRTUAL_ALLOC_EX,

        [Obsolete]
        VirtualCopy = CELID_VIRTUAL_COPY,

        VirtualCopyEx = CELID_VIRTUAL_COPY_EX,

        [Obsolete]
        VirtualFree = CELID_VIRTUAL_FREE,

        VirtualFreeEx = CELID_VIRTUAL_FREE_EX,

        WaitMulti = CELID_WAIT_MULTI,

    }
}
