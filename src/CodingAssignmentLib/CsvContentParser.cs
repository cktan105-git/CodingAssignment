using CodingAssignmentLib.Abstractions;

namespace CodingAssignmentLib;

public class CsvContentParser : IContentParser
{
    // TODO: To check if could use a library like CsvHelper or similar so to handle CSV parsing more robustly, e.g. following RFC 4180
    public IEnumerable<Data> Parse(string content)
    {
        return content.ReplaceLineEndings() // Quick fix: Normalize line endings to handle different OS formats
                      .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                      .Select(line =>
                      {
                          var items = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                          return new Data(items[0], items[1]);
                      });
    }
}