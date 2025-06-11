using CodingAssignmentLib.Abstractions;
using System.IO.Abstractions;

namespace CodingAssignmentLib.Services;

public class SearchService : ISearchService
{
    private readonly IFileSystem _fileSystem;

    public SearchService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public List<string> Search(string baseDirectory, string searchKey)
    {
        if (string.IsNullOrWhiteSpace(searchKey))
        {
            return new List<string> { "Search key cannot be empty." };
        }

        if (!_fileSystem.Directory.Exists(baseDirectory))
        {
            return new List<string> { "Data directory does not exist." };
        }

        var fileUtility = new FileUtility(_fileSystem);
        var files = _fileSystem.Directory.GetFiles(baseDirectory, "*.*", SearchOption.AllDirectories);
        var results = new List<(Data data, string path)>();

        // Dictionary to hold parsers for different file extensions
        Dictionary<string, IContentParser> parsers = new Dictionary<string, IContentParser>();

        foreach (var file in files)
        {
            // Get the relative path of the file based on the base directory, including the parent directory
            var relativePath = _fileSystem.Path.GetRelativePath(Directory.GetParent(baseDirectory)!.FullName, file);
            
            var extension = fileUtility.GetExtension(file);

            // Try to get the parser for the file extension, or create it if it doesn't exist
            if (!parsers.TryGetValue(extension, out var parser))
            {
                try
                {
                    // Use the factory to get the parser for the file extension
                    parser = ContentParserFactory.GetParser(extension);
                    parsers[extension] = parser;
                }
                catch
                {
                    // If the parser is not supported, skip this file
                    continue;
                }
            }

            try
            {
                var dataList = parser.Parse(fileUtility.GetContent(file));
                foreach (var data in dataList)
                {
                    if (data.Key != null && data.Key.StartsWith(searchKey, StringComparison.OrdinalIgnoreCase))
                    {
                        results.Add((data, relativePath));
                    }
                }
            }
            catch
            {
                // TODO: Check if to log the error or handle it as needed, but continue processing other files
                continue;
            }
        }

        if (results.Count == 0)
        {
            return new List<string> { "No matching data found." };
        }

        return results.Select(r => $"Key:{r.data.Key} Value:{r.data.Value} FileName:{r.path}").ToList();
    }
}
