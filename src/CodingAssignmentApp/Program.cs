// See https://aka.ms/new-console-template for more information

using CodingAssignmentLib;
using CodingAssignmentLib.Abstractions;
using System.IO.Abstractions;

Console.WriteLine("Coding Assignment!");

do
{
    Console.WriteLine("\n---------------------------------------\n");
    Console.WriteLine("Choose an option from the following list:");
    Console.WriteLine("\t1 - Display");
    Console.WriteLine("\t2 - Search");
    Console.WriteLine("\t3 - Exit");

    switch (Console.ReadLine())
    {
        case "1":
            Display();
            break;
        case "2":
            Search();
            break;
        case "3":
            return;
        default:
            return;
    }
} while (true);


void Display()
{
    Console.WriteLine("Enter the name of the file to display its content:");

    var fileName = Console.ReadLine()!;
    var fileSystem = new FileSystem();

    // Check to ensure the file exists before attempting to read it.
    if (!fileSystem.File.Exists(fileName))
    {
        Console.WriteLine("File does not exist.");
        return;
    }

    try
    {
        var fileUtility = new FileUtility(fileSystem);
        
        // Factory to create the appropriate parser based on the file extension.
        var parser = TryGetParser(fileUtility.GetExtension(fileName), true);
        if (parser == null)
        {
            return;
        }
        var dataList = parser.Parse(fileUtility.GetContent(fileName));

        Console.WriteLine("Data:");
        foreach (var data in dataList)
        {
            Console.WriteLine($"Key:{data.Key} Value:{data.Value}");
        }
    }
    catch (NotSupportedException ex)
    {
        Console.WriteLine(ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
    }

}

void Search()
{
    Console.WriteLine("Enter the key to search.");
    var searchKey = Console.ReadLine()!;

    if (string.IsNullOrWhiteSpace(searchKey))
    {
        Console.WriteLine("Search key cannot be empty.");
        return;
    }

    try
    {
        var fileSystem = new FileSystem();
        var fileUtility = new FileUtility(fileSystem);

        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "data");
        if (!fileSystem.Directory.Exists(basePath))
        {
            Console.WriteLine("Data directory does not exist.");
            return;
        }
        var files = fileSystem.Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories);
                              
        var results = new List<(Data data, string path)>();

        Dictionary<string, IContentParser> parsers = new Dictionary<string, IContentParser>();

        foreach (var file in files)
        {
            var extension = fileUtility.GetExtension(file);
            if (!parsers.TryGetValue(extension, out var parser))
            {
                parser = TryGetParser(extension, false);
                if (parser == null)
                {
                    continue;
                }
                parsers[extension] = parser;
            }

            var dataList = parser.Parse(fileUtility.GetContent(file));

            foreach (var data in dataList)
            {
                if (data.Key != null && data.Key.StartsWith(searchKey, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add((data, file));
                }
            }
        }

        foreach (var (data, path) in results)
        {
            Console.WriteLine($"Key:{data.Key} Value:{data.Value} FileName:{path}");
        }

        if (results.Count == 0)
        {
            Console.WriteLine("No matching data found.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while searching: {ex.Message}");
    }
}

/// <summary>
/// Retrieves a content parser based on the file extension.
/// </summary>
/// <param name="extension">The file extension to determine the parser.</param>
/// <param name="displayErrorMessage">Whether to display an error message if the parser cannot be created.</param>
/// <returns>An instance of <see cref="IContentParser"/> or null if the parser cannot be created.</returns>
IContentParser TryGetParser(string extension, bool displayErrorMessage = false)
{
    try
    {
        return ContentParserFactory.GetParser(extension);
    }
    catch (Exception ex)
    {
        if (displayErrorMessage)
        {
            Console.WriteLine(ex.Message);
        }

        return null!;
    }
}