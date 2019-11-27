using System;
using System.Runtime.InteropServices;
using UnsafeAndSpanExtension;
using ULONG = System.UInt32;

namespace CelogParserLib
{
    namespace ARM
    {
        public enum ProcessorFeature : uint
        {
            V4 = 0x80000001,
            V5 = 0x80000002,
            V6 = 0x80000003,
            V7 = 0x80000004,
            THUMB = 0x80000005,
            JAZELLE = 0x80000006,
            DSP = 0x80000007,
            MOVE_CP = 0x80000008,
            VFP_HARDWARE = 0x80000009,
            MPU = 0x8000000A,
            WRITE_BUFFER = 0x8000000B,
            MBX = 0x8000000C,
            L2CACHE = 0x8000000D,
            PHYSICALLY_TAGGED_CACHE = 0x8000000E,
            VFP_SINGLE_PRECISION = 0x8000000F,
            VFP_DOUBLE_PRECISION = 0x80000010,
            ITCM = 0x80000011,
            DTCM = 0x80000012,
            UNIFIED_CACHE = 0x80000013,
            WRITE_BACK_CACHE = 0x80000014,
            CACHE_CAN_BE_LOCKED_DOWN = 0x80000015,
            L2CACHE_MEMORY_MAPPED = 0x80000016,
            L2CACHE_COPROC = 0x80000017,
            THUMB2 = 0x80000018,
            T2EE = 0x80000019,
            VFP_V3 = 0x8000001A,
            NEON = 0x8000001B,
            UNALIGNED_ACCESS = 0x8000001C,
            VFP_SUPPORT = 0x8000001D,
            VFP_V1 = 0x8000001E,
            VFP_V2 = 0x8000001F,
            VFP_ALL_ROUNDING_MODES = 0x80000020,
            VFP_SHORT_VECTORS = 0x80000021,
            VFP_SQUARE_ROOT = 0x80000022,
            VFP_DIVIDE = 0x80000023,
            VFP_FP_EXCEPTIONS = 0x80000024,
            VFP_EXTENDED_REGISTERS = 0x80000025,
            VFP_HALF_PRECISION = 0x80000026,
            NEON_HALF_PRECISION = 0x80000027,
            NEON_SINGLE_PRECISION = 0x80000028,
            NEON_LOAD_STORE = 0x80000029,
            VFP_DENORMALS = 0x8000002A,
            INTEL_XSCALE = 0x80010001,
            INTEL_PMU = 0x80010002,
            INTEL_WMMX = 0x80010003,
        }

        [Flags]
        public enum ContextFlags : ULONG
        {
            Arm = 0x0000040,
            Control = (Arm | 0x00000001),
            Integer = (Arm | 0x00000002),
            FloatingPoint = (Arm | 0x00000004),
            ExtendedFloat = (Arm | 0x00000008),

            Full = (Control | Integer | FloatingPoint),
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public unsafe partial struct CONTEXT
        {
            public ContextFlags ContextFlags;
            public ULONG R0;
            public ULONG R1;
            public ULONG R2;
            public ULONG R3;
            public ULONG R4;
            public ULONG R5;
            public ULONG R6;
            public ULONG R7;
            public ULONG R8;
            public ULONG R9;
            public ULONG R10;
            public ULONG R11;
            public ULONG R12;

            public ULONG Sp;
            public ULONG Lr;
            public ULONG Pc;
            public ULONG Psr;

            internal const int NUM_VFP_REGS_COMMON = 32;
            internal const int NUM_VFP_REGS = 64;
            internal const int NUM_EXTRA_CONTROL_REGS = 8;

            public ULONG Fpscr;
            public ULONG FpExc;
            //public fixed ULONG S[NUM_VFP_REGS];
            public FixedLengthUInt32Array64 S;
            //public fixed ULONG FpExtra[NUM_EXTRA_CONTROL_REGS];
            public FixedLengthUInt32Array8 FpExtra;

            public static int Size => sizeof(CONTEXT);

            static CONTEXT() => System.Diagnostics.Debug.Assert(Size == 368);
        }
        public static class Extention
        {
            public static ReadOnlySpan<ULONG> GetFpExtra(ref CONTEXT context)
             => AltMemoryMarshal.CreateReadOnlySpan(ref context.FpExtra.First, CONTEXT.NUM_EXTRA_CONTROL_REGS);
        }
    }
}
