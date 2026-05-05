using System.Text;
using System.Text.Json;

namespace Parser;

public static class Export
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static void Render(
        OutputFormat format,
        IList<KeyValuePair<string, int>> wordCounts,
        TranscriptAnalysis analysis)
    {
        switch (format)
        {
            case OutputFormat.Print:
                Console.Write(BuildPrint(wordCounts, analysis));
                break;
            case OutputFormat.Markdown:
                WriteFile("output.md", BuildMarkdown(wordCounts, analysis));
                break;
            case OutputFormat.Json:
                WriteFile("output.json", BuildJson(wordCounts, analysis));
                break;
            case OutputFormat.Plaintext:
                WriteFile("output.txt", BuildPlaintext(wordCounts, analysis));
                break;
        }
    }

    private static void WriteFile(string path, string content)
    {
        File.WriteAllText(path, content);
        Console.WriteLine($"Wrote {path}");
    }

    private static string BuildPrint(IList<KeyValuePair<string, int>> wordCounts, TranscriptAnalysis a)
    {
        var sb = new StringBuilder();
        var max = wordCounts.Count > 0 ? wordCounts[0].Value : 0;
        var longest = wordCounts.Count > 0 ? wordCounts.Max(kv => kv.Key.Length) : 0;
        foreach (var (word, count) in wordCounts)
        {
            var bars = max > 0 ? (int)Math.Round((double)count / max * Constants.BarWidth) : 0;
            sb.AppendLine($"{word.PadRight(longest)}  {new string('#', bars)} ({count})");
        }
        sb.AppendLine();
        sb.AppendLine("--- Analysis ---");
        sb.AppendLine($"Summary: {a.QuickSummary}");
        sb.AppendLine();
        sb.AppendLine("Highlights:");
        foreach (var bullet in a.BulletPointHighlights)
        {
            sb.AppendLine($"  - {bullet}");
        }
        sb.AppendLine();
        sb.AppendLine($"Sentiment: {a.SentimentAnalysis}");
        sb.AppendLine();
        sb.AppendLine($"Keywords: {string.Join(", ", a.Keywords)}");
        return sb.ToString();
    }

    private static string BuildMarkdown(IList<KeyValuePair<string, int>> wordCounts, TranscriptAnalysis a)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Transcript Analysis");
        sb.AppendLine();
        sb.AppendLine("## Summary");
        sb.AppendLine(a.QuickSummary);
        sb.AppendLine();
        sb.AppendLine("## Highlights");
        foreach (var bullet in a.BulletPointHighlights)
        {
            sb.AppendLine($"- {bullet}");
        }
        sb.AppendLine();
        sb.AppendLine("## Sentiment");
        sb.AppendLine(a.SentimentAnalysis);
        sb.AppendLine();
        sb.AppendLine("## Keywords");
        sb.AppendLine(string.Join(", ", a.Keywords));
        sb.AppendLine();
        sb.AppendLine("## Word Frequencies");
        sb.AppendLine();
        sb.AppendLine("| Word | Count |");
        sb.AppendLine("| --- | --- |");
        foreach (var (word, count) in wordCounts)
        {
            sb.AppendLine($"| {word} | {count} |");
        }
        return sb.ToString();
    }

    private static string BuildJson(IList<KeyValuePair<string, int>> wordCounts, TranscriptAnalysis a)
    {
        var dto = new
        {
            wordFrequencies = wordCounts.ToDictionary(kv => kv.Key, kv => kv.Value),
            analysis = a,
        };
        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    private static string BuildPlaintext(IList<KeyValuePair<string, int>> wordCounts, TranscriptAnalysis a)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Word Frequencies:");
        foreach (var (word, count) in wordCounts)
        {
            sb.AppendLine($"{word}: {count}");
        }
        sb.AppendLine();
        sb.AppendLine("Summary:");
        sb.AppendLine(a.QuickSummary);
        sb.AppendLine();
        sb.AppendLine("Highlights:");
        foreach (var bullet in a.BulletPointHighlights)
        {
            sb.AppendLine($"- {bullet}");
        }
        sb.AppendLine();
        sb.AppendLine("Sentiment:");
        sb.AppendLine(a.SentimentAnalysis);
        sb.AppendLine();
        sb.AppendLine("Keywords:");
        sb.AppendLine(string.Join(", ", a.Keywords));
        return sb.ToString();
    }
}
