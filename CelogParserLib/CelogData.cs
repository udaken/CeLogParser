using System;
using CelogParserLib.Data;

namespace CelogParserLib
{
    public readonly struct CelogData
    {
        public readonly TimeSpan Timestamp;
        public readonly long ConsumedTime;
        public readonly CelogId Id;
        public readonly byte Cpu;
        //readonly ArraySegment<byte> Payload;
        public object? Data { get; }
        public ICelogThreadInfo? CurrentThread { get; }

        internal CelogData(TimeSpan timestamp, CelogId id, byte cpu, ArraySegment<byte> payload = default, ICelogThreadInfo? thread = null, object? data = default, long consumedTime = default)
        {
            Timestamp = timestamp;
            Id = id;
            Cpu = cpu;
            //Payload = payload;
            Data = data;
            CurrentThread = thread;
            ConsumedTime = consumedTime;
        }

        public override string ToString()
            => $"{Timestamp},{Id},CPU={Cpu} {Data?.ToString() ?? ""}";
    }
}
