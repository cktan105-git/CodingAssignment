// See https://aka.ms/new-console-template for more information

using CodingAssignmentLib;
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
        var parser = ContentParserFactory.GetParser(fileUtility.GetExtension(fileName));
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
}