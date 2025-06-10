using CodingAssignmentLib;
using CodingAssignmentLib.Abstractions;

namespace CodingAssignmentTests;

// TODO: Confirm if need to add more tests for CsvContentParser
public class CsvContentParserTests
{
    private CsvContentParser _sut = null!;
    
    [SetUp]
    public void Setup()
    {
        _sut = new CsvContentParser();
    }

    [Test]
    public void Parse_ReturnsData()
    {
        var content = "a,b" + Environment.NewLine + "c,d" + Environment.NewLine;
        var dataList = _sut.Parse(content).ToList();
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b"),
            new("c", "d")
        }).AsCollection);
    }
}