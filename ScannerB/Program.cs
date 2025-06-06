using System.Diagnostics;

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

        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)2;
        var worker = new Worker("agent2", args[0]);
        worker.Run();
    }
}

