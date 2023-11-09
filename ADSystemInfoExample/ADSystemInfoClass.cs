using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ADSystemInfoExample
{
    //[ComImport]
    [Guid("50B6327F-AFD1-11D2-9CB9-0000F87A369E")]
    // [ClassInterface(0)]
    [TypeLibType(2)]
    public class ADSystemInfoClass : IADsADSystemInfo, ADSystemInfo
    {
        [DispId(2)]
        public virtual extern string UserName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(2)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(3)]
        public virtual extern string ComputerName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(4)]
        public virtual extern string SiteName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(5)]
        public virtual extern string DomainShortName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(5)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(6)]
        public virtual extern string DomainDNSName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(6)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(7)]
        public virtual extern string ForestDNSName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(7)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(8)]
        public virtual extern string PDCRoleOwner
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(8)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(9)]
        public virtual extern string SchemaRoleOwner
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(9)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(10)]
        public virtual extern bool IsNativeMode
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(10)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern ADSystemInfoClass();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(11)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern string GetAnyDCName();

        string IADsADSystemInfo.GetAnyDCName()
        {
            //ILSpy generated this explicit interface implementation from .override directive in GetAnyDCName
            return this.GetAnyDCName();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(12)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern string GetDCSiteName([In][MarshalAs(UnmanagedType.BStr)] string szServer);

        string IADsADSystemInfo.GetDCSiteName([In][MarshalAs(UnmanagedType.BStr)] string szServer)
        {
            //ILSpy generated this explicit interface implementation from .override directive in GetDCSiteName
            return this.GetDCSiteName(szServer);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(13)]
        public virtual extern void RefreshSchemaCache();

        void IADsADSystemInfo.RefreshSchemaCache()
        {
            //ILSpy generated this explicit interface implementation from .override directive in RefreshSchemaCache
            this.RefreshSchemaCache();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(14)]
        [return: MarshalAs(UnmanagedType.Struct)]
        public virtual extern object GetTrees();

        object IADsADSystemInfo.GetTrees()
        {
            //ILSpy generated this explicit interface implementation from .override directive in GetTrees
            return this.GetTrees();
        }
    }
}