using System.IO;
using System.IO.Pipes;
using System.Threading;
using Shared;

public class Worker
{
    private readonly string pipeName;
    private readonly string directoryPath;
    private List<WordEntry> indexData = new();

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
        foreach (var file in Directory.GetFiles(directoryPath, "*.txt"))
        {
            var content = File.ReadAllText(file);
            var wordGroups = content.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(w => w)
                .Select(g => new WordEntry
                {
                    FileName = Path.GetFileName(file),
                    Word = g.Key,
                    Count = g.Count()
                });

            indexData.AddRange(wordGroups);
        }
    }

    private void SendData()
    {
        using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
        pipe.Connect();
        using var writer = new StreamWriter(pipe) { AutoFlush = true };
        string json = PipeUtils.Serialize(indexData);
        writer.WriteLine(json);
    }
}
