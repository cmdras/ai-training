using System.Text.Json;
using OpenAI.Chat;

namespace Parser;

public record TranscriptAnalysis(
    string QuickSummary,
    string[] BulletPointHighlights,
    string SentimentAnalysis,
    string[] Keywords);

public static class Llm
{
    private const string Model = "gpt-4o-mini";
    private const string EnvVarName = "OPENAI_API_KEY";

    private const string SystemPrompt =
        "You analyze meeting/conversation transcripts. " +
        "quickSummary: 2-3 sentence overview. " +
        "bulletPointHighlights: 3-7 concise bullets of key points. " +
        "sentimentAnalysis: short paragraph describing the overall tone and sentiment. " +
        "keywords: select 5-15 keywords drawn from the provided list of top frequent words. " +
        "Pick the ones most representative of the transcript's topics and ignore generic filler. " +
        "You may keep them as single words or combine adjacent ones into short phrases when that better captures the topic.";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static readonly BinaryData Schema = BinaryData.FromBytes("""
    {
      "type": "object",
      "properties": {
        "quickSummary": { "type": "string" },
        "bulletPointHighlights": {
          "type": "array",
          "items": { "type": "string" }
        },
        "sentimentAnalysis": { "type": "string" },
        "keywords": {
          "type": "array",
          "items": { "type": "string" }
        }
      },
      "required": ["quickSummary", "bulletPointHighlights", "sentimentAnalysis", "keywords"],
      "additionalProperties": false
    }
    """u8.ToArray());

    public static async Task<TranscriptAnalysis> AnalyzeAsync(
        string transcript,
        IEnumerable<KeyValuePair<string, int>> topWords)
    {
        var apiKey = Environment.GetEnvironmentVariable(EnvVarName)
            ?? throw new InvalidOperationException($"Environment variable '{EnvVarName}' is not set.");

        var client = new ChatClient(Model, apiKey);

        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "transcript_analysis",
                jsonSchema: Schema,
                jsonSchemaIsStrict: true),
        };

        var topWordsList = string.Join(", ", topWords.Select(kv => $"{kv.Key} ({kv.Value})"));
        var userContent =
            $"Top frequent words (word and count, already filtered for stopwords):\n{topWordsList}\n\n" +
            $"Transcript:\n{transcript}";

        ChatCompletion completion = await client.CompleteChatAsync(
            [new SystemChatMessage(SystemPrompt), new UserChatMessage(userContent)],
            options);

        var json = completion.Content[0].Text;
        return JsonSerializer.Deserialize<TranscriptAnalysis>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to parse analysis JSON.");
    }
}
