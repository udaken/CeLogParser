using System;
namespace CelogParserLib
{
    [Flags]
    public enum CelogModuleFlag : uint
    {
        None,
        Kernel = Interop.CEL_MODULE_FLAG_KERNEL,
        Dataonly = Interop.CEL_MODULE_FLAG_DATAONLY,
    }
}
