using System.Collections.Generic;
using System.Text.Json;

namespace Shared
{
    public class WordEntry
    {
        public string FileName { get; set; }
        public string Word { get; set; }
        public int Count { get; set; }
    }

    public static class PipeUtils
    {
        public static string Serialize(List<WordEntry> entries)
        {
            return JsonSerializer.Serialize(entries);
        }

        public static List<WordEntry> Deserialize(string json)
        {
            return JsonSerializer.Deserialize<List<WordEntry>>(json) ?? new();
        }
    }
}
