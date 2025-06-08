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

        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)4;
        var master = new Processor(args[0], args[1]);
        master.Run();
    }
}

