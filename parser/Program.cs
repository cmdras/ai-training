using System.Text.RegularExpressions;
using Parser;

var parsed = ArgParse.Parse(args);
var content = ReadTranscript(parsed.Path);

var frequencies = CountWords(content);
var ordered = frequencies
    .Where(kv => kv.Value >= parsed.MinCount && kv.Key.Length > 1 && !Constants.Blacklist.Contains(kv.Key))
    .OrderByDescending(kv => kv.Value)
    .ToList();

var analysis = await Llm.AnalyzeAsync(content, ordered);
Export.Render(parsed.Format, ordered, analysis);

static string ReadTranscript(string path)
{
    return File.ReadAllText(path);
}

static Dictionary<string, int> CountWords(string text)
{
    var counts = new Dictionary<string, int>();
    foreach (Match match in WordRegex().Matches(text))
    {
        var word = match.Value.Split('\'')[0].ToLowerInvariant();
        if (word.Length == 0) continue;
        counts[word] = counts.GetValueOrDefault(word) + 1;
    }
    return counts;
}

partial class Program
{
    [GeneratedRegex(@"[\w']+")]
    private static partial Regex WordRegex();
}
