using System;

namespace YellowDrawer.Storage.Common
{
    public interface IStorageFolder
    {
        string GetPath();
        string GetName();
        long GetSize();
        DateTime GetLastUpdated();
        IStorageFolder GetParent();
    }
}