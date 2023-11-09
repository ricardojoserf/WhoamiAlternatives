using System;
using System.Runtime.InteropServices;

namespace ADSystemInfoExample
{
    internal class Program
    {
        [DllImport("ole32.dll", EntryPoint = "CoCreateInstance", CallingConvention = CallingConvention.StdCall)] static extern UInt32 CoCreateInstance([In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, IntPtr pUnkOuter, UInt32 dwClsContext, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, out ADSystemInfo rReturnedComObject);

        static void Main(string[] args)
        {
            Guid CLSID_ADSystemInfo = new Guid("50b6327f-afd1-11d2-9cb9-0000f87a369e");
            UInt32 CLSCTX_INPROC_SERVER = 1;
            Guid IID_IADsADSystemInfo = new Guid("5BB11929-AFD1-11D2-9CB9-0000F87A369E");

            UInt32 res = CoCreateInstance(CLSID_ADSystemInfo, IntPtr.Zero, CLSCTX_INPROC_SERVER, IID_IADsADSystemInfo, out ADSystemInfo rReturnedComObject);
            if (res != 0)
            {
                Console.WriteLine("[-] Failure calling CoCreateInstance.");
                System.Environment.Exit(-1);
            }
            try
            {
                Console.WriteLine("[+] Result:\n", rReturnedComObject.UserName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[-] Error calling get_Username(), check you are in a domain joined machine. Error: {0}", e.ToString());
                System.Environment.Exit(-1);
            }
        }
    }
}