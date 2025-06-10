using CodingAssignmentLib;
using CodingAssignmentLib.Abstractions;
using System.Xml;
using NUnit.Framework;

namespace CodingAssignmentTests;

public class XmlContentParserTests
{
    private XmlContentParser _sut = null!;

    [SetUp]
    public void Setup()
    {
        _sut = new XmlContentParser();
    }

    [Test]
    public void Parse_ReturnsData()
    {
        // Arrange
        var content = """
        <Datas>
            <Data>
                <Key>a</Key>
                <Value>b</Value>
            </Data>
            <Data>
                <Key>c</Key>
                <Value>d</Value>
            </Data>
        </Datas>
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
    public void Parse_ReturnsEmpty_ForNoDataElements()
    {
        // Arrange
        var content = "<Datas></Datas>";

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
    public void Parse_HandlesMissingKeyField()
    {
        // Arrange
        var content = """
        <Datas>
            <Data>
                <Key>a</Key>
                <Value>b</Value>
            </Data>
            <Data>
                <Value>d</Value>
            </Data>
        </Datas>
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b"),
            new(null!, "d"),
        }).AsCollection);
    }

    [Test]
    public void Parse_HandlesMissingValueField()
    {
        // Arrange
        var content = """
        <Datas>
            <Data>
                <Key>a</Key>
                <Value>b</Value>
            </Data>
            <Data>
                <Key>c</Key>
            </Data>
        </Datas>
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
    public void Parse_HandlesExtraFields()
    {
        // Arrange
        var content = """
        <Datas>
            <Data>
                <Key>a</Key>
                <Value>b</Value>
                <Extra>123</Extra>
            </Data>
        </Datas>
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
    public void Parse_HandlesWhitespaceOnlyContent()
    {
        // Arrange
        var content = "   \n\t  ";

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.Empty);
    }

    [Test]
    public void Parse_HandlesEmptyKeyElement()
    {
        // Arrange
        var content = """
        <Datas>
            <Data>
                <Key></Key>
                <Value>b</Value>
            </Data>
            <Data>
                <Key> </Key>
                <Value>d</Value>
            </Data>
            <Data>
                <Key>   </Key>
                <Value>f</Value>
            </Data>
            <Data>
                <Key/>
                <Value>h</Value>
            </Data>
        </Datas>
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("", "b"),
            new("", "d"),
            new("", "f"),
            new("", "h")
        }).AsCollection);
    }

    [Test]
    public void Parse_HandlesEmptyValueElement()
    {
        // Arrange
        var content = """
        <Datas>
            <Data>
                <Key>a</Key>
                <Value></Value>
            </Data>
            <Data>
                <Key>c</Key>
                <Value> </Value>
            </Data>
            <Data>
                <Key>e</Key>
                <Value>   </Value>
            </Data>
            <Data>
                <Key>g</Key>
                <Value/>
            </Data>
        </Datas>
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", ""),
            new("c", ""),
            new("e", ""),
            new("g", ""),
        }).AsCollection);
    }

    [Test]
    public void Parse_IgnoresCaseMismatchTags()
    {
        // Arrange
        var content = """
        <Datas>
            <Data>
                <key>a</key>
                <value>b</value>
            </Data>
            <Data>
                <Key>c</Key>
                <value>d</value>
            </Data>
            <Data>
                <key>e</key>
                <Value>f</Value>
            </Data>
        </Datas>
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new(null!, null!),
            new("c", null!),
            new(null!, "f")
        }).AsCollection);
    }

    [Test]
    public void Parse_HandlesMultipleDataWithSomeMissingKeyOrValue()
    {
        // Arrange
        var content = """
        <Datas>
            <Data>
                <Key>a</Key>
                <Value>b</Value>
            </Data>
            <Data>
                <Value>c</Value>
            </Data>
            <Data>
                <Key>d</Key>
            </Data>
            <Data>
            </Data>
        </Datas>
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b"),
            new(null!, "c"),
            new("d", null!),
            new(null!, null!)
        }).AsCollection);
    }

    [Test]
    public void Parse_IgnoresAttributesOnElements()
    {
        // Arrange
        var content = """
        <Datas>
            <Data id="1">
                <Key>a</Key>
                <Value>b</Value>
            </Data>
        </Datas>
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
    public void Parse_IgnoresComments()
    {
        // Arrange
        var content = """
        <?xml version="1.0"?>
        <!-- This is a comment -->
        <Datas>
            <Data>
                <Key>a</Key>
                <Value>b</Value>
            </Data>
            <!-- Another comment -->
            <Data>
                <Key>c</Key>
                <Value>d</Value>
            </Data>
        </Datas>
        """;

        // Act
        var dataList = _sut.Parse(content).ToList();

        // Assert
        Assert.That(dataList, Is.EqualTo(new List<Data>
        {
            new("a", "b"),
            new("c", "d"),
        }).AsCollection);
    }
   
    [Test]
    public void Parse_ThrowsFormatException_OnMalformedXml()
    {
        // Arrange
        var malformedContent = "<Datas><Data><Key>a</Key><Value>b</Value>"; // missing closing tags

        // Act & Assert
        Assert.Throws<FormatException>(() => _sut.Parse(malformedContent).ToList());
    }

    [Test]
    public void Parse_ThrowsFormatException_OnNonXmlInput()
    {
        // Arrange
        var content = "not xml at all";

        // Act & Assert
        Assert.Throws<FormatException>(() => _sut.Parse(content).ToList());
    }

    [Test]
    public void Parse_ThrowsFormatException_WhenRootIsNotDatas()
    {
        // Arrange
        var content = """
        <Root>
            <Data>
                <Key>a</Key>
                <Value>b</Value>
            </Data>
        </Root>
        """;

        // Act & Assert
        Assert.Throws<FormatException>(() => _sut.Parse(content).ToList());
    }

    [Test]
    public void Parse_ThrowsFormatException_OnExtraNestedElementsInside()
    {
        // Arrange
        var content = """
        <Datas>
            <Data>
                <Key>a</Key>
                <Value>b</Value>
            </Data>
            <Data>
                <Key>c</Key>
                <Value>
                    <Inner>should be ignored</Inner>
                    d
                </Value>
            </Data>
        </Datas>
        """;

        // Act & Assert
        Assert.Throws<FormatException>(() => _sut.Parse(content).ToList());
    }
}