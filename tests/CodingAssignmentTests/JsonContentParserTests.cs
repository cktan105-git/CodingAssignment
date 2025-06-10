using CodingAssignmentLib;
using CodingAssignmentLib.Abstractions;

namespace CodingAssignmentTests;

public class JsonContentParserTests
{
    private JsonContentParser _sut = null!;

    [SetUp]
    public void Setup()
    {
        _sut = new JsonContentParser();
    }

    [Test]
    public void Parse_ReturnsData()
    {
        // Arrange
        var content = """
        [
            { "Key": "a", "Value": "b" },
            { "Key": "c", "Value": "d" }
        ]
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b"),
            new("c", "d")
        }).AsCollection);
    }

    [Test]
    public void Parse_ReturnsEmpty_ForEmptyArray()
    {
        // Arrange
        var content = "[]";

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.Empty);
    }

    [Test]
    public void Parse_ReturnsEmpty_ForEmptyContent()
    {
        // Arrange
        var content = "";

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.Empty);
    }


    [Test]
    public void Parse_HandlesMissingValueField()
    {
        // Arrange
        var content = """
        [
            { "Key": "a", "Value": "b" },
            { "Key": "c" }
        ]
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b"),
            new("c", null!)
        }).AsCollection);
    }

    [Test]
    public void Parse_HandlesMissingKeyField()
    {
        // Arrange
        var content = """
        [
            { "Key": "a", "Value": "b" },
            { "Value": "d" }
        ]
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b"),
            new(null!, "d")
        }).AsCollection);
    }

    [Test]
    public void Parse_HandlesExtraFields()
    {
        // Arrange
        var content = """
        [
            { "Key": "a", "Value": "b", "Extra": 123 }
        ]
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b")
        }).AsCollection);
    }

    [Test]
    public void Parse_HandlesNullElementsInArray()
    {
        // Arrange
        var content = """
        [
            { "Key": "a", "Value": "b" },
            null,
            { "Key": "c", "Value": "d" }
        ]
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b"),
            new("c", "d")
        }).AsCollection);
    }

    [Test]
    public void Parse_HandlesCompletelyEmptyObject()
    {
        // Arrange
        var content = """
        [
            { "Key": "a", "Value": "b" },
            {},
            { "Key": "c", "Value": "d" }
        ]
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b"),
            new(null!, null!),
            new("c", "d")
        }).AsCollection);
    }

    [Test]
    public void Parse_HandlesExplicitNullKeyAndValue()
    {
        // Arrange
        var content = """
        [
            { "Key": null, "Value": null }
        ]
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new(null!, null!)
        }).AsCollection);
    }

    [Test]
    public void Parse_IgnoresAllNulls()
    {
        // Arrange
        var content = "[null, null, null]";

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.Empty);
    }

    [Test]
    public void Parse_IgnoresCaseMismatchProperties()
    {
        // Arrange
        var content = """
        [
            { "key": "a", "value": "b" },
            { "KEY": "c", "VaLuE": "d" },
            { "Key": "e", "VaLuE": "f" },
            { "KEY": "g", "Value": "h" }
        ]
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new(null!, null!),
            new(null!, null!),
            new("e", null!),
            new(null!, "h")
        }).AsCollection);
    }

    [Test]
    public void Parse_IgnoresWhitespaceOnlyContent()
    {
        // Arrange
        var content = "   \n\t  ";

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.Empty);
    }

    [Test]
    public void Parse_ThrowsFormatException_OnMalformedJson()
    {
        // Arrange
        var malformedContent = "[{ \"Key\": \"a\", \"Value\": \"b\" "; // missing closing } and ]

        // Act & Assert
        Assert.Throws<FormatException>(() => _sut.Parse(malformedContent));
    }

    [Test]
    public void Parse_ThrowsFormatException_OnNestedObjectInsteadOfFlatFields()
    {
        // Arrange
        var content = """
        [
            { "Key": "a", "Value": { "Nested": "data" } }
        ]
        """;

        // Act & Assert
        Assert.Throws<FormatException>(() => _sut.Parse(content).ToList());
    }

    [Test]
    public void Parse_ThrowsFormatException_OnNonArrayInput()
    {
        // Arrange
        var content = """
        { "Key": "a", "Value": "b" }
        """;

        // Act & Assert
        Assert.Throws<FormatException>(() => _sut.Parse(content).ToList());
    }
}