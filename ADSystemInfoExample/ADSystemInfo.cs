using ADSystemInfoExample;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ADSystemInfoExample
{
    [ComImport]
    [Guid("5BB11929-AFD1-11D2-9CB9-0000F87A369E")]
    [CoClass(typeof(ADSystemInfoClass))]
    public interface ADSystemInfo : IADsADSystemInfo
    {
    }


    // INTERFACE
    [ComImport]
    [TypeLibType(4160)]
    [Guid("5BB11929-AFD1-11D2-9CB9-0000F87A369E")]
    public interface IADsADSystemInfo
    {
        [DispId(2)]
        string UserName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(2)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(3)]
        string ComputerName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(4)]
        string SiteName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(5)]
        string DomainShortName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(5)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(6)]
        string DomainDNSName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(6)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(7)]
        string ForestDNSName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(7)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(8)]
        string PDCRoleOwner
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(8)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(9)]
        string SchemaRoleOwner
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(9)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(10)]
        bool IsNativeMode
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(10)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(11)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetAnyDCName();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(12)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDCSiteName([In][MarshalAs(UnmanagedType.BStr)] string szServer);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(13)]
        void RefreshSchemaCache();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(14)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetTrees();
    }
}