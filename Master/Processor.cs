using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Shared;

public class Processor
{
    private readonly string pipe1, pipe2;
    private readonly List<WordEntry> allData = new();
    private readonly object lockObj = new();

    public Processor(string p1, string p2)
    {
        pipe1 = p1;
        pipe2 = p2;
    }

    public void Run()
    {
        var t1 = new Thread(() => Listen(pipe1));
        var t2 = new Thread(() => Listen(pipe2));
        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();
        DisplayResult();
    }

    private void Listen(string pipeName)
    {
        using var server = new NamedPipeServerStream(pipeName, PipeDirection.In);
        Console.WriteLine($"Waiting for '{pipeName}'...");
        server.WaitForConnection();
        using var reader = new StreamReader(server);
        var json = reader.ReadLine() ?? "[]";
        var entries = PipeUtils.Deserialize(json);
        lock (lockObj)
        {
            allData.AddRange(entries);
        }
        Console.WriteLine($"Received from '{pipeName}'.");
    }

    private void DisplayResult()
    {
        var result = allData
            .GroupBy(x => new { x.FileName, x.Word })
            .Select(g => new
            {
                g.Key.FileName,
                g.Key.Word,
                Count = g.Sum(e => e.Count)
            })
            .OrderBy(x => x.FileName)
            .ThenBy(x => x.Word);

        Console.WriteLine("\nConsolidated Word Index:");
        foreach (var entry in result)
            Console.WriteLine($"{entry.FileName}:{entry.Word}:{entry.Count}");
    }
}
