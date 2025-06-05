using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Scanner A";
        if (args.Length == 0)
        {
            Console.WriteLine("Provide directory path.");
            return;
        }

        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)1;
        var worker = new Worker("agent1", args[0]);
        worker.Run();
    }
}
