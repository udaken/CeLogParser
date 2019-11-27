using CelogParserLib.Data;
using System.Collections.Generic;
using System.Linq;

namespace CelogParserLib
{
    public sealed class Celog
    {
        private readonly IReadOnlyList<ICelogThreadInfo> _threads;
        private readonly IReadOnlyList<ICelogProcessInfo> _processes;
        private readonly IReadOnlyList<ICelogEventInfo> _events;
        private readonly IReadOnlyList<ICelogMutexInfo> _mutexes;
        private readonly IReadOnlyList<ICelogSemInfo> _semaphores;
        private readonly IReadOnlyList<CelogData> _Timeline;
        private readonly IReadOnlyList<CelogWarningInfo> _Warnings;

        internal Celog(IReadOnlyList<ICelogThreadInfo> threads,
            IReadOnlyList<ICelogProcessInfo> processes,
            IReadOnlyList<ICelogEventInfo> events,
            IReadOnlyList<ICelogMutexInfo> mutexes,
            IReadOnlyList<ICelogSemInfo> semaphores,
            IReadOnlyList<CelogData> timeline,
            IReadOnlyList<CelogWarningInfo>? warnings, uint defaultQuantum)
        {
            _threads = threads;
            _processes = processes;
            _events = events;
            _mutexes = mutexes;
            _semaphores = semaphores;
            _Timeline = timeline;
            _Warnings = warnings ?? new List<CelogWarningInfo>();
            DefaultQuantum = defaultQuantum;
        }

        public ILookup<CeHandle, ICelogProcessInfo> Processes => _processes.ToLookup(item => item.Handle);

        public ILookup<CeHandle, ICelogThreadInfo> Threads => _threads.ToLookup(item => item.Handle);

        public IReadOnlyList<CelogData> Timeline
         => _Timeline;

        public uint DefaultQuantum { get; }

        public IReadOnlyList<CelogWarningInfo> Warnings => _Warnings;

        public IEnumerable<ICelogKernelObjectInfo> KernelObjects =>
            _threads.Cast<ICelogKernelObjectInfo>()
            .Concat(_processes)
            .Concat(_events)
            .Concat(_mutexes)
            .Concat(_semaphores)
            .OrderBy(item => item.CreatedAt);
    }
}
