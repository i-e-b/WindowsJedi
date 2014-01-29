namespace WindowsJedi.WinApis.Data
{
    internal enum WindowStack : uint
    {
        BelowTarget = 2, //GW_HWNDNEXT
        AboveTarget = 3  //GW_HWNDPREV
    }
}