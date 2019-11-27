﻿using DWORD = System.UInt32;
using static CelogParserLib.Interop;

namespace CelogParserLib
{
    public enum CelogBootTimeAction : DWORD
    {
        LAUNCHING_FS = CEL_BOOT_TIME.BOOT_TIME_LAUNCHING_FS,
        FS_INITED = CEL_BOOT_TIME.BOOT_TIME_FS_INITED,
        FS_OBJ_STORE_INITIALIZED = CEL_BOOT_TIME.BOOT_TIME_FS_OBJ_STORE_INITIALIZED,
        FS_FILES_INITIALIZED = CEL_BOOT_TIME.BOOT_TIME_FS_FILES_INITIALIZED,
        FS_REG_INITIALIZED = CEL_BOOT_TIME.BOOT_TIME_FS_REG_INITIALIZED,
        FS_DB_INITIALIZED = CEL_BOOT_TIME.BOOT_TIME_FS_DB_INITIALIZED,
        FS_LAUNCH = CEL_BOOT_TIME.BOOT_TIME_FS_LAUNCH,
        DEV_ACTIVATE = CEL_BOOT_TIME.BOOT_TIME_DEV_ACTIVATE,
        EV_FINISHED = CEL_BOOT_TIME.BOOT_TIME_DEV_FINISHED,
        GWES_FINISHED = CEL_BOOT_TIME.BOOT_TIME_GWES_FINISHED,
        SYSTEM_STARTED = CEL_BOOT_TIME.BOOT_TIME_SYSTEM_STARTED,
        START_DELAYED_WORK = CEL_BOOT_TIME.BOOT_TIME_START_DELAYED_WORK,
    }
}