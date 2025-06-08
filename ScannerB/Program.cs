using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Scanner B";

        if (args.Length == 0)
        {
            Console.WriteLine("Provide directory path.");
            return;
        }

        // Set processor affinity only on Windows or Linux
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)2; // Pin to core 2
        }

        var worker = new Worker("agent2", args[0]);
        worker.Run();
    }
}
