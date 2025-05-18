namespace BF1ModTools.Native;

public static partial class Win32
{
    [LibraryImport("psapi.dll")]
    public static partial int EmptyWorkingSet(IntPtr hwProc);
}