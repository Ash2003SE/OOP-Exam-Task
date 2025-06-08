using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Shared;

public class Worker
{
    private readonly string pipeName;
    private readonly string directoryPath;
    private readonly List<WordEntry> indexData = new();

    public Worker(string pipe, string path)
    {
        pipeName = pipe;
        directoryPath = path;
    }

    public void Run()
    {
        var readThread = new Thread(ReadFiles);
        var sendThread = new Thread(SendData);
        readThread.Start();
        readThread.Join();
        sendThread.Start();
        sendThread.Join();
    }

    private void ReadFiles()
    {
        if (!Directory.Exists(directoryPath))
        {
            Console.Error.WriteLine($"Directory not found: {directoryPath}");
            return;
        }
        var files = Directory.GetFiles(directoryPath, "*.txt");
        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            var words = content
                .Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => new string(w.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant())
                .Where(w => w.Length > 0);

            var groups = words.GroupBy(w => w).Select(g => new WordEntry
            {
                FileName = Path.GetFileName(file),
                Word = g.Key,
                Count = g.Count()
            });
            indexData.AddRange(groups);
        }
    }

    private void SendData()
    {
        using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
        pipe.Connect();
        using var writer = new StreamWriter(pipe) { AutoFlush = true };
        var json = PipeUtils.Serialize(indexData);
        writer.WriteLine(json);
    }
}
