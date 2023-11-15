using System.Runtime.InteropServices;
using EverythingSharp.Enums;

namespace EverythingSharp;

public abstract class Base : IDisposable
{
    public abstract void Dispose();

    public abstract IEnumerable<EverythingResult> Search(string query, int maxResults = -1, int offset = -1,
        Sort sort = Sort.NameAscending, RequestFlags requestFlags = RequestFlags.FullPathAndFileName);

    public abstract uint IncrementRunCount(EverythingResult result);

    public static Base? Get()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return null;

        return IntPtr.Size == 8 ? new Everything64() : new Everything();
    }
}