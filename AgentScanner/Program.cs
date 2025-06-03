using System;
using System.Diagnostics;
using System.Threading;

namespace AgentScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            // Expected args: [0] = <directoryPath>, [1] = <pipeName>
            //
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: AgentScanner.exe <directoryPath> <pipeName>");
                return;
            }

            string directoryPath = args[0];
            string pipeName = args[1];

            // 1) Pin this process to a specific core
            //    (e.g. force to Core 2; adapt as needed).
            var proc = Process.GetCurrentProcess();
            proc.ProcessorAffinity = new IntPtr(1 << 2); 
            // (On a 0-based index, 1<<2 is core #2. Change the shift if you want a different core.)

            Console.WriteLine($"[AgentScanner:{pipeName}] Running on CPU core {GetCurrentCore()}");
            Console.WriteLine($"[AgentScanner:{pipeName}] Scanning directory: {directoryPath}");
            Console.WriteLine($"[AgentScanner:{pipeName}] Will send to pipe: {pipeName}");

            // 2) Create a shared data‐structure to hold the word index
            //    (e.g. ConcurrentDictionary<string, Dictionary<string,int>>)
            var indexer = new WordIndexer();
            var fileScannerThread = new Thread(() =>
            {
                var scanner = new FileScannerThread(directoryPath, indexer);
                scanner.Run();
            })
            { IsBackground = false }; // keep alive until completion

            var senderThread = new Thread(() =>
            {
                var sender = new SenderThread(pipeName, indexer);
                sender.Run();
            })
            { IsBackground = false };

            fileScannerThread.Start();
        
            fileScannerThread.Join();

            senderThread.Start();
            senderThread.Join();

            Console.WriteLine($"[AgentScanner:{pipeName}] Done. Press ENTER to exit.");
            Console.ReadLine();
        }

        // Helper to display current core (approximate; Windows doesn't expose exact core,
        // but we can read process affinity mask, or just print a static message)
        static int GetCurrentCore()
        {
            // This is a simplification. In real code, you might p/invoke GetCurrentProcessorNumber().
            return 2; 
        }
    }
}
