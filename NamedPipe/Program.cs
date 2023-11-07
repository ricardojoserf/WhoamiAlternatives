using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;


namespace NamedPipe
{
    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr CreateNamedPipe(string lpName, uint dwOpenMode, uint dwPipeMode, uint nMaxInstances, uint nOutBufferSize, uint nInBufferSize, uint nDefaultTimeOut, IntPtr lpSecurityAttributes);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)] public static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, System.Threading.ThreadStart lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("advapi32.dll")] public static extern IntPtr NpGetUserName(IntPtr hNamedPipe, IntPtr Username, uint UsernameLength);
        [DllImport("kernel32.dll")] static extern bool ConnectNamedPipe(IntPtr hNamedPipe, IntPtr lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)] static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)] public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("kernel32.dll", SetLastError = true)] unsafe static extern int WriteFile(IntPtr handle, char* buffer, int numBytesToWrite, out uint numBytesWritten, Boolean lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)] unsafe static extern int ReadFile(IntPtr hFile, IntPtr lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)] static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)] static extern IntPtr GetProcessHeap();
        [DllImport("ntdll.dll", SetLastError = true)] static extern IntPtr RtlAllocateHeap(IntPtr HeapHandle, int Flags, int Size);
        [DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)] static extern bool DisconnectNamedPipe(IntPtr hNamedPipe);

        static uint PIPE_ACCESS_DUPLEX = 0x00000003;
        static uint PIPE_TYPE_MESSAGE = 0x00000004;
        static uint PIPE_READMODE_MESSAGE = 0x00000002;
        static uint PIPE_WAIT = 0x00000000;
        static uint PIPE_UNLIMITED_INSTANCES = 255;
        static uint GENERIC_READ = 0x80000000;
        static uint GENERIC_WRITE = 0x40000000;
        static uint OPEN_EXISTING = 3;
        static int test_string_size = 5;
        static uint MAX_USERNAME_LENGTH = 256;

        public static void writeFile()
        {
            // Open handle to named pipe
            IntPtr hPipe = CreateFile("\\\\.\\pipe\\LOCAL\\usernamepipe", GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            Console.WriteLine("[+] Named pipe (CreateFile): \t0x{0}", hPipe);

            // WriteFile
            try
            {
                string str = "Test";
                unsafe
                {
                    fixed (char* ptr = str)
                    {
                        uint numBytes = 0;
                        int res = WriteFile(hPipe, ptr, test_string_size * 2, out numBytes, false);
                        Console.WriteLine("[+] WriteFile response code: \t{0}", res);
                        Console.WriteLine("[+] Number of written bytes: \t{0}", numBytes);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[-] Failure calling WriteFile, try again.");
                System.Environment.Exit(-1);
            }

            // Close handle
            CloseHandle(hPipe);
        }


        // Source (C++): https://pastebin.com/raw/ZsReS7k4
        static void Main(string[] args)
        {
            // CreateNamedPipe
            IntPtr hNamedPipe = CreateNamedPipe("\\\\.\\pipe\\LOCAL\\usernamepipe", PIPE_ACCESS_DUPLEX, PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE | PIPE_WAIT, PIPE_UNLIMITED_INSTANCES, 256, 256, 0, IntPtr.Zero);
            Console.WriteLine("[+] Named pipe handle: \t\t0x{0}", hNamedPipe.ToString("X"));

            // CreateThread
            IntPtr hThread = CreateThread(IntPtr.Zero, 0, new ThreadStart(Program.writeFile), hNamedPipe, 0, IntPtr.Zero);
            Console.WriteLine("[+] CreateThread handle: \t0x{0}", hThread.ToString("X"));

            // ConnectNamedPipe
            bool connected = ConnectNamedPipe(hNamedPipe, IntPtr.Zero);
            Console.WriteLine("[+] Named Pipe connected: \t{0}", connected);

            // WaitForSingleObject
            WaitForSingleObject(hThread, 1000);

            // Allocate memory with correct size + ReadFile
            IntPtr Handle = GetProcessHeap();
            int HEAP_ZERO_MEMORY = 0x00000008;
            IntPtr allocated_address = RtlAllocateHeap(Handle, HEAP_ZERO_MEMORY, test_string_size * 2);
            uint numBytes = 0;
            int res = ReadFile(hNamedPipe, allocated_address, (uint)test_string_size * 2, out numBytes, IntPtr.Zero);
            Console.WriteLine("[+] ReadFile response code: \t{0}", res);
            Console.WriteLine("[+] Number of read bytes: \t{0}", numBytes);
            Console.WriteLine("[+] Allocated Address:\t\t0x{0}", allocated_address.ToString("X"));

            // ReadFile - Result
            byte[] data = new byte[test_string_size * 2];
            ReadProcessMemory(Process.GetCurrentProcess().Handle, allocated_address, data, data.Length, out _);
            String environment_vars = Encoding.Unicode.GetString(data);
            Console.WriteLine("[+] String from named pipe: \t{0}", environment_vars);

            // Allocate memory with correct size
            //Handle = GetProcessHeap();
            allocated_address = RtlAllocateHeap(Handle, HEAP_ZERO_MEMORY, (int)MAX_USERNAME_LENGTH);
            NpGetUserName(hNamedPipe, allocated_address, MAX_USERNAME_LENGTH);
            Console.WriteLine("[+] UserName Pointer: \t\t0x{0}", allocated_address.ToString("X"));

            // ReadFile - Result
            data = new byte[(int)MAX_USERNAME_LENGTH * 2];
            ReadProcessMemory(Process.GetCurrentProcess().Handle, allocated_address, data, data.Length, out _);
            String username = Encoding.Unicode.GetString(data);
            int index = username.IndexOf("\x00\x00");
            username = username.Substring(0, index);
            Console.WriteLine("[+] Result:\n{0}", username);

            // Close handle
            DisconnectNamedPipe(hNamedPipe);
            CloseHandle(hNamedPipe);
        }
    }
}