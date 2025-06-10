using CodingAssignmentLib.Abstractions;
using System.Text.Json;

namespace CodingAssignmentLib;

public class JsonContentParser : IContentParser
{
    /// <summary>
    /// Parses a JSON array of objects with "Key" and "Value" fields into a collection of <see cref="Data"/>.
    /// <para>Supports optional null entries in the array, which will be ignored.</para>
    /// <para>Property names are matched case-sensitively; only properties with matching case will be recognized.</para>
    /// <para>If a JSON object is missing the "Key" or "Value" field, or if either field is explicitly null, it will be deserialized with a corresponding <c>null</c> value in the resulting <see cref="Data"/> instance.</para>
    /// </summary>
    /// <param name="content">The raw JSON string content to parse.</param>
    /// <returns>An enumerable of <see cref="Data"/> records.</returns>
    /// <exception cref="FormatException">Thrown when the JSON is invalid or cannot be parsed into the expected format.</exception>
    public IEnumerable<Data> Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Enumerable.Empty<Data>();
        }

        var result = new List<Data>();

        try
        {
            var jsonDatas = JsonSerializer.Deserialize<List<JsonData>>(content);
            if (jsonDatas != null && jsonDatas.Count > 0)
            {
                foreach (var jsonData in jsonDatas)
                {
                    if (jsonData == null)
                    {
                        continue; // Skip null entries in the array
                    }

                    result.Add(new Data(jsonData.Key ?? null!, jsonData.Value ?? null!));
                }
            }

            return result;
        }
        catch (JsonException ex)
        {
            throw new FormatException("Invalid JSON format.", ex);
        }
        catch (Exception ex)
        {
            throw new FormatException("An error occurred while parsing the JSON content.", ex);
        }
    }
}