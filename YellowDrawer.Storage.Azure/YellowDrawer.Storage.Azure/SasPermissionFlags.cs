using System;

namespace YellowDrawer.Azure
{
    [Flags]
    public enum SasPermissionFlags
    {
        None = 0,
        Read = 1,
        Write = 2,
        Delete = 4,
        List = 8,
    }
}