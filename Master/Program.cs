using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Master";
        if (args.Length != 2)
        {
            Console.WriteLine("Provide 2 pipe names.");
            return;
        }
        // Pin to core 3 (bitmask=4)
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)4;
        var processor = new Processor(args[0], args[1]);
        processor.Run();
    }
}
