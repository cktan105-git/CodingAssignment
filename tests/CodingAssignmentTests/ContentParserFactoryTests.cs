using CodingAssignmentLib;
using CodingAssignmentLib.Abstractions;

namespace CodingAssignmentTests;

public class ContentParserFactoryTests
{
    [TestCase(".csv", typeof(CsvContentParser))]
    [TestCase(".CSV", typeof(CsvContentParser))]
    [TestCase(".json", typeof(JsonContentParser))]
    [TestCase(".JSON", typeof(JsonContentParser))]
    [TestCase(".xml", typeof(XmlContentParser))]
    [TestCase(".XML", typeof(XmlContentParser))]
    public void GetParser_ReturnsCorrectParser_ForSupportedExtensions(string extension, Type expectedType)
    {
        // Act
        var parser = ContentParserFactory.GetParser(extension);

        // Assert
        Assert.That(parser, Is.Not.Null);
        Assert.That(parser, Is.InstanceOf(expectedType));
    }

    [TestCase(".txt")]
    [TestCase(".yaml")]
    [TestCase(".md")]
    public void GetParser_ThrowsNotSupportedException_ForUnsupportedExtensions(string extension)
    {
        // Act & Assert
        var ex = Assert.Throws<NotSupportedException>(() => ContentParserFactory.GetParser(extension));
        Assert.That(ex!.Message, Does.Contain("Unsupported file extension"));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void GetParser_ThrowsArgumentException_ForEmptyOrWhitespaceExtensions(string extension)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => ContentParserFactory.GetParser(extension));
        Assert.That(ex!.Message, Does.Contain("File extension must not be null or empty."));
    }

    [Test]
    public void GetParser_ThrowsArgumentException_ForNullExtension()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => ContentParserFactory.GetParser(null!));
        Assert.That(ex!.Message, Does.Contain("File extension must not be null or empty."));
    }
}