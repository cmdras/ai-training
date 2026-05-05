using System.CommandLine;

namespace Parser;

public enum OutputFormat { Print, Markdown, Json, Plaintext }

public record ParsedArgs(string Path, int MinCount, OutputFormat Format);

public static class ArgParse
{
    public static ParsedArgs Parse(string[] args)
    {
        var pathArg = new Argument<string>(
            name: "path",
            getDefaultValue: () => Constants.DefaultTranscriptPath,
            description: "Path to the transcript file");

        var minCountOpt = new Option<int>(
            aliases: new[] { "--min-count", "-m" },
            getDefaultValue: () => Constants.DefaultMinCount,
            description: "Minimum word count threshold");

        var formatOpt = new Option<OutputFormat>(
            aliases: new[] { "--format", "-f" },
            getDefaultValue: () => OutputFormat.Print,
            description: "Output format: print | markdown | json | plaintext");

        var root = new RootCommand("Transcript word-frequency analyzer with LLM-assisted summary")
        {
            pathArg,
            minCountOpt,
            formatOpt,
        };

        var result = root.Parse(args);
        if (result.Errors.Count > 0)
        {
            foreach (var err in result.Errors)
            {
                Console.Error.WriteLine(err.Message);
            }
            Environment.Exit(1);
        }

        return new ParsedArgs(
            result.GetValueForArgument(pathArg),
            result.GetValueForOption(minCountOpt),
            result.GetValueForOption(formatOpt));
    }
}
