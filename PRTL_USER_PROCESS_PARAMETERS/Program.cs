using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PRTL_USER_PROCESS_PARAMETERS
{
    internal class Program
    {
        [DllImport("ntdll.dll", SetLastError = true)] static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION pbi, uint processInformationLength, ref uint returnLength);
        [DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        private struct PROCESS_BASIC_INFORMATION { public uint ExitStatus; public IntPtr PebBaseAddress; public UIntPtr AffinityMask; public int BasePriority; public UIntPtr UniqueProcessId; public UIntPtr InheritedFromUniqueProcessId; }


        static void GetEnv(int processparameters_offset, int environmentsize_offset, int environment_offset)
        {
            IntPtr hProcess = Process.GetCurrentProcess().Handle;
            PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
            uint temp = 0;
            NtQueryInformationProcess(hProcess, 0x0, ref pbi, (uint)(IntPtr.Size * 6), ref temp);
            IntPtr PebBaseAddress = (IntPtr)(pbi.PebBaseAddress);
            Console.WriteLine("[+] PEB base:                  \t0x{0}", PebBaseAddress.ToString("X"));

            IntPtr processparameters_pointer = (IntPtr)(pbi.PebBaseAddress + processparameters_offset);
            IntPtr processparameters = Marshal.ReadIntPtr(processparameters_pointer);
            Console.WriteLine("[+] ProcessParameters Address: \t0x{0}", processparameters.ToString("X"));

            // Reference: https://www.geoffchappell.com/studies/windows/km/ntoskrnl/inc/api/pebteb/rtl_user_process_parameters.htm
            IntPtr environment_size_pointer = processparameters + environmentsize_offset;
            IntPtr environment_size = Marshal.ReadIntPtr(environment_size_pointer);
            Console.WriteLine("[+] Environment Size:          \t{0}", environment_size);

            IntPtr environment_pointer = processparameters + environment_offset;
            IntPtr environment_start = Marshal.ReadIntPtr(environment_pointer);
            Console.WriteLine("[+] Environment Address:       \t0x{0}", environment_start.ToString("X"));
            IntPtr environment_end = environment_start + (int)environment_size;

            Console.WriteLine("[+] Result:");
            byte[] data = new byte[(int)environment_size];
            ReadProcessMemory(hProcess, environment_start, data, data.Length, out _);   
            String environment_vars = Encoding.Unicode.GetString(data);
            int found = environment_vars.IndexOf("USERNAME=");
            String rest_String = environment_vars.Substring(found);
            int found2 = rest_String.IndexOf("=");
            int found3 = rest_String.IndexOf("\x00");
            found3 -= found2;
            rest_String = rest_String.Substring(found2 + 1, found3 - 1);
            Console.WriteLine(rest_String);
        }


        static void Main(string[] args)
        {
            if (Environment.Is64BitProcess)
            {
                Console.WriteLine("[+] 64 bits process");
                GetEnv(0x20, 0x3F0, 0x80);
            }
            else
            {
                Console.WriteLine("[+] 32 bits process");
                GetEnv(0x10, 0x0290, 0x48);
            }
        }
    }
}