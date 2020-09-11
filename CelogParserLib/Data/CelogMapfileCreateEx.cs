using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogMapfileCreateEx : ICelogInfo
    {
        internal CelogMapfileCreateEx(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MAPFILE_CREATE_EX>();
            Handle = data.hMap;
            Protect = data.flProtect;
            MapFlags = data.dwMapFlags;
            MaxSize = ((ulong)data.dwMaxSizeHigh << 32) | data.dwMaxSizeLow;
            Name = data.GetName(buffer.Length).ToString();
        }

        public CeHandle Handle { get; }
        public uint Protect { get; }
        public uint MapFlags { get; }
        public ulong MaxSize { get; }
        public string Name { get; }

        public bool IsPsudoObject => false;
        public IReadOnlyList<CeHandle> ContainsHadles => new[] { Handle };
    }
}
