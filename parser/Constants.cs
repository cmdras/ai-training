namespace Parser;

public static class Constants
{
    public const string DefaultTranscriptPath = "transcript.txt";
    public const int DefaultMinCount = 5;
    public const int BarWidth = 50;

    public static readonly HashSet<string> Blacklist = new(StringComparer.OrdinalIgnoreCase)
    {
        "the", "a", "an", "and", "or", "but", "if", "then", "else",
        "to", "of", "in", "on", "at", "by", "for", "with", "from", "as",
        "is", "are", "was", "were", "be", "been", "being", "am",
        "do", "does", "did", "have", "has", "had",
        "i", "you", "he", "she", "it", "we", "they", "me", "him", "her", "us", "them",
        "my", "your", "his", "its", "our", "their",
        "this", "that", "these", "those",
        "so", "not", "no", "yes", "just", "really", "very", "too",
        "what", "when", "where", "who", "why", "how",
        "can", "could", "would", "should", "will", "shall", "may", "might", "must",
        "there", "here", "than", "about", "out", "up", "down", "into",
    };
}
