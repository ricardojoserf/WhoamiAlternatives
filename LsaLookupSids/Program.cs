using System;
using System.Runtime.InteropServices;


namespace LsaLookupSids
{
    internal class Program
    {
        [DllImport("ntdll.dll", SetLastError = true)] static extern int NtOpenProcessToken(int ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
        [DllImport("ntdll.dll", SetLastError = true)] static extern int NtQueryInformationToken(IntPtr TokenHandle, uint TokenInformationClass, IntPtr TokenInformation, int TokenInformationLength, out int ReturnLength);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)] static extern IntPtr GetProcessHeap();
        [DllImport("ntdll.dll", SetLastError = true)] static extern IntPtr RtlAllocateHeap(IntPtr HeapHandle, int Flags, int Size);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] static extern uint LsaOpenPolicy(LSA_UNICODE_STRING[] SystemName, ref LSA_OBJECT_ATTRIBUTES ObjectAttributes, uint AccessMask, out IntPtr PolicyHandle);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] internal static extern uint LsaLookupSids(IntPtr PolicyHandle, int count, IntPtr buffer, out IntPtr domainList, out IntPtr nameList);

        [StructLayout(LayoutKind.Sequential)] struct LSA_OBJECT_ATTRIBUTES { internal int Length; internal IntPtr RootDirectory; internal IntPtr ObjectName; internal int Attributes; internal IntPtr SecurityDescriptor; internal IntPtr SecurityQualityOfService; }
        [StructLayout(LayoutKind.Sequential)] struct LSA_TRANSLATED_NAME { internal uint Use; internal LSA_UNICODE_STRING Name; internal IntPtr DomainIndex; }
        [StructLayout(LayoutKind.Sequential)] struct LSA_UNICODE_STRING { internal ushort Length; internal ushort MaximumLength; [MarshalAs(UnmanagedType.LPWStr)] internal string Buffer; }


        static void Main(string[] args)
        {
            // NtOpenProcessToken
            uint TOKEN_QUERY = 0x0008;
            int current_process = -1;
            int Status = NtOpenProcessToken(current_process, TOKEN_QUERY, out IntPtr hToken);
            if (Status != 0)
            {
                Console.WriteLine("[-] Error calling NtOpenProcessToken");
                System.Environment.Exit(-1);
            }
            Console.WriteLine("[+] hToken handle: \t\t0x{0}", hToken.ToString("X"));

            // NtQueryInformationToken
            uint tokenUser = 1;
            IntPtr TokenInformation = IntPtr.Zero;
            int TokenInformationLength = 0;
            Status = NtQueryInformationToken(hToken, tokenUser, TokenInformation, TokenInformationLength, out TokenInformationLength);

            // Allocate memory with correct size
            IntPtr Handle = GetProcessHeap();
            int HEAP_ZERO_MEMORY = 0x00000008;
            IntPtr allocated_address = RtlAllocateHeap(Handle, HEAP_ZERO_MEMORY, TokenInformationLength);
            Console.WriteLine("[+] TokenInformation Address:\t0x{0}", allocated_address.ToString("X"));

            // NtQueryInformationToken - Right size
            Status = NtQueryInformationToken(hToken, tokenUser, allocated_address, TokenInformationLength, out TokenInformationLength);
            if (Status != 0)
            {
                Console.WriteLine("[-] Error calling NtQueryInformationToken");
                System.Environment.Exit(-1);
            }

            // LsaOpenPolicy
            LSA_OBJECT_ATTRIBUTES lsaAttr;
            lsaAttr.RootDirectory = IntPtr.Zero;
            lsaAttr.ObjectName = IntPtr.Zero;
            lsaAttr.Attributes = 0;
            lsaAttr.SecurityDescriptor = IntPtr.Zero;
            lsaAttr.SecurityQualityOfService = IntPtr.Zero;
            lsaAttr.Length = Marshal.SizeOf(typeof(LSA_OBJECT_ATTRIBUTES));
            IntPtr hHandle = IntPtr.Zero;
            uint POLICY_LOOKUP_NAMES = 0x00000800;
            uint ret = LsaOpenPolicy(null, ref lsaAttr, POLICY_LOOKUP_NAMES, out hHandle);
            if (ret != 0)
            {
                Console.WriteLine("[-] Error calling LsaOpenPolicy");
                System.Environment.Exit(-1);
            }

            // LsaLookupSids
            ret = LsaLookupSids(hHandle, 1, allocated_address, out IntPtr ptrDomainList, out IntPtr ptrNameList);
            if (ret != 0)
            {
                Console.WriteLine("[-] Error calling LsaLookupSids");
                System.Environment.Exit(-1);
            }

            // Result
            LSA_TRANSLATED_NAME trans_name = (LSA_TRANSLATED_NAME)Marshal.PtrToStructure(ptrNameList, typeof(LSA_TRANSLATED_NAME));
            Console.WriteLine("[+] Result:\n{0}", trans_name.Name.Buffer);
        }
    }
}