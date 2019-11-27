using static CelogParserLib.Interop;

namespace CelogParserLib
{
    public enum CelogMapFlushType : ushort
    {
        Invalid = CEL_MAPFLUSH_TYPE.CEL_INVALID_MAPFLUSH,
        FlushMapSimple = CEL_MAPFLUSH_TYPE.CEL_FlushMapSimple,
        FlushMapAtomic = CEL_MAPFLUSH_TYPE.CEL_FlushMapAtomic,
        ValidateFile = CEL_MAPFLUSH_TYPE.CEL_ValidateFile,
        FlushMapGather = CEL_MAPFLUSH_TYPE.CEL_FlushMapGather,
    }
}
