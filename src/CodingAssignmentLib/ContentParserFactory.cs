using CodingAssignmentLib.Abstractions;

namespace CodingAssignmentLib;

public static class ContentParserFactory
{
    public static IContentParser GetParser(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentException("File extension must not be null or empty.", nameof(extension));
        }
        
        return extension.ToLower() switch
        {
            ".csv" => new CsvContentParser(),
            ".json" => new JsonContentParser(),
            ".xml" => new XmlContentParser(),
            _ => throw new NotSupportedException($"Unsupported file extension: {extension}")
        };
    }
}