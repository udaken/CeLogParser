using System;

namespace CelogParserLib.Data
{
    public interface ICelogKernelObjectInfo : ICelogInfo
    {
        CeHandle Handle { get; }

        bool IsPsudoObject { get; }
        string Name { get; }
        TimeSpan CreatedAt { get; }
        TimeSpan? DeletedAt { get; }
    }

}
