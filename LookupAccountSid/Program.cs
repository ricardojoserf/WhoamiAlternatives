using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;


namespace LookupAccountSid
{

    internal class Program
    {
        [DllImport("ntdll.dll", SetLastError = true)] static extern int NtOpenProcessToken(int ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
        [DllImport("ntdll.dll", SetLastError = true)] static extern int NtQueryInformationToken(IntPtr TokenHandle, uint TokenInformationClass, IntPtr TokenInformation, int TokenInformationLength, out int ReturnLength);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)] static extern IntPtr GetProcessHeap();
        [DllImport("ntdll.dll", SetLastError = true)] static extern IntPtr RtlAllocateHeap(IntPtr HeapHandle, int Flags, int Size);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)] static extern bool ConvertSidToStringSid(IntPtr pSID, out IntPtr ptrSid);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)] static extern bool LookupAccountSid(string lpSystemName, [MarshalAs(UnmanagedType.LPArray)] byte[] Sid, StringBuilder lpName, ref uint cchName, StringBuilder ReferencedDomainName, ref uint cchReferencedDomainName, out uint peUse);
      
        public struct TOKEN_USER { public SID_AND_ATTRIBUTES User; }
        public struct SID_AND_ATTRIBUTES { public IntPtr Sid; public int Attributes; }


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
            if (Status != 0) {
                Console.WriteLine("[-] Error calling NtQueryInformationToken");
                System.Environment.Exit(-1);
            }

            // Marshal result bytes as TOKEN_USER struct
            TOKEN_USER TokenUser = (TOKEN_USER)Marshal.PtrToStructure(allocated_address, typeof(TOKEN_USER));

            // ConvertSidToStringSid
            IntPtr pstr = IntPtr.Zero;
            Boolean ok = ConvertSidToStringSid(TokenUser.User.Sid, out pstr);
            string sidstr = Marshal.PtrToStringAuto(pstr);
            Console.WriteLine("[+] SID (String version):\n{0}", sidstr);

            // LookupAccountSid
            StringBuilder name = new StringBuilder();
            uint cchName = (uint)name.Capacity;
            StringBuilder referencedDomainName = new StringBuilder();
            uint cchReferencedDomainName = (uint)referencedDomainName.Capacity;
            var sid = new SecurityIdentifier(sidstr);
            byte[] byteSid = new byte[sid.BinaryLength];
            sid.GetBinaryForm(byteSid, 0);
            LookupAccountSid(null, byteSid, name, ref cchName, referencedDomainName, ref cchReferencedDomainName, out uint sidUse);
            Console.WriteLine("[+] Result:\n{1}\\{2}", sidUse, referencedDomainName.ToString(), name.ToString());
        }
    }
}
