﻿namespace CelogParserLib
{
    public sealed class CelogWarningInfo
    {
        internal CelogWarningInfo(string message)
        {
            Message = message;
        }
        public string Message { get; }
        public override string ToString() => Message;
    }
}
