using CodingAssignmentLib.Abstractions;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CodingAssignmentLib;

public class XmlContentParser : IContentParser
{
    /// <summary>
    /// Parses the provided XML content, extracting the <c>&lt;Key&gt;</c> and <c>&lt;Value&gt;</c> from each <c>&lt;Data&gt;</c> element.
    /// <para>Element matching is case-sensitive; only elements with matching case will be recognized.</para>
    /// <para>If a <c>&lt;Key&gt;</c> or <c>&lt;Value&gt;</c> is missing from a <c>&lt;Data&gt;</c> element, the corresponding property in the resulting <see cref="Data"/> instance will be set to <c>null</c>.</para>
    /// <para>If a <c>&lt;Key&gt;</c> or <c>&lt;Value&gt;</c> is present but empty or whitespace (e.g., <c>&lt;Key/&gt;</c> or <c>&lt;Key&gt;&lt;/Key&gt;</c> or <c>&lt;Key&gt;   &lt;/Key&gt;</c>), the corresponding property will be set to an empty string (<c>""</c>).</para>
    /// </summary>
    /// <param name="content">The XML content to parse.</param>
    /// <returns>A collection of <see cref="Data"/> objects.</returns>
    /// <exception cref="FormatException">Thrown when the input XML is invalid or malformed.</exception>
    public IEnumerable<Data> Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Enumerable.Empty<Data>();
        }

        var result = new List<Data>();

        try
        {
            using (StringReader reader = new StringReader(content))
            {
                XmlDatas? xmlDatas = new XmlSerializer(typeof(XmlDatas)).Deserialize(reader) as XmlDatas;
                if (xmlDatas?.DataList != null && xmlDatas.DataList.Count > 0)
                {
                    foreach (var xmlData in xmlDatas.DataList)
                    {
                        result.Add(new Data(xmlData.Key ?? null!, xmlData.Value ?? null!));
                    }
                }
            }

            return result;
        }
        catch (InvalidOperationException ex) when (ex.InnerException is System.Xml.XmlException)
        {
            throw new FormatException("Invalid XML format.", ex);
        }
        catch (Exception ex)
        {
            throw new FormatException("An error occurred while parsing the XML content.", ex);
        }
    }
}