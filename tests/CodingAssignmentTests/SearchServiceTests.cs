using CodingAssignmentLib.Services;
using System.IO.Abstractions.TestingHelpers;

namespace CodingAssignmentTests;

[TestFixture]
public class SearchServiceTests
{
    private MockFileSystem _fileSystem = null!;
    private SearchService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"C:\project\data\file1.csv", new MockFileData("hello1,value1\nfoo,value2") },
            { @"C:\project\data\sub\file2.csv", new MockFileData("hello2,value3\nbar,value4") },
            { @"C:\project\data\file3.json", new MockFileData("[{\"Key\": \"hello3\", \"Value\": \"value5\"}]") },
            { @"C:\project\data\file4.xml", new MockFileData("<Datas><Data><Key>hello4</Key><Value>value6</Value></Data></Datas>") }
        });

        _sut = new SearchService(_fileSystem);
    }

    [Test]
    public void Search_ReturnsMatchingKeysAcrossFiles()
    {
        // Act
        var results = _sut.Search(@"C:\project\data", "hello");

        // Assert
        Assert.That(results, Has.Count.EqualTo(4));
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(r => r.Contains(@"Key:hello1 Value:value1 FileName:data\file1.csv")));
            Assert.That(results.Any(r => r.Contains(@"Key:hello2 Value:value3 FileName:data\sub\file2.csv")));
            Assert.That(results.Any(r => r.Contains(@"Key:hello3 Value:value5 FileName:data\file3.json")));
            Assert.That(results.Any(r => r.Contains(@"Key:hello4 Value:value6 FileName:data\file4.xml")));
        });
    }

    [Test]
    public void Search_IsCaseInsensitive()
    {
        // Act
        var results = _sut.Search(@"C:\project\data", "HELLO");

        // Assert - should find the same matches regardless of case
        Assert.That(results, Has.Count.EqualTo(4));
    }

    [Test]
    public void Search_ReturnsMultipleMatchesFromSingleFile()
    {
        // Arrange - add a file with multiple matching keys
        _fileSystem.AddFile(@"C:\project\data\file6.csv", new MockFileData("hello6,value7\nhello7,value8"));

        // Act
        var results = _sut.Search(@"C:\project\data", "hello");

        // Assert - should find 6 matches now
        Assert.That(results, Has.Count.EqualTo(6));
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(r => r.Contains(@"Key:hello6 Value:value7 FileName:data\file6.csv")));
            Assert.That(results.Any(r => r.Contains(@"Key:hello7 Value:value8 FileName:data\file6.csv")));
        });
    }

    [Test]
    public void Search_DoesNotReturnPartialMatches()
    {
        // Arrange
        _fileSystem.AddFile(@"C:\project\data\file7.csv", new MockFileData("ahello,value8\nbhello,value9"));

        // Act
        var results = _sut.Search(@"C:\project\data", "hello");

        // Assert - should not match 'ahello' or 'bhello' since they do not start with 'hello'
        Assert.That(results.Any(r => r.Contains("ahello") || r.Contains("bhello")), Is.False);
    }

    [Test]
    public void Search_HandlesMissingValues()
    {
        // Arrange - add files with missing values
        _fileSystem.AddFile(@"C:\project\data\incomplete.json", new MockFileData("[{\"Key\": \"keyonly2\" },{ \"Value\": \"valueonly2\"}]"));
        _fileSystem.AddFile(@"C:\project\data\incomplete.xml", new MockFileData("<Datas><Data><Key>keyonly3</Key></Data><Data><Value>valueonly3</Value></Data></Datas>"));

        // Act
        var results = _sut.Search(@"C:\project\data", "keyonly");

        // Assert - should not crash
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        { 
            Assert.That(results.Any(r => r.Contains(@"Key:keyonly2 Value: FileName:data\incomplete.json")));
            Assert.That(results.Any(r => r.Contains(@"Key:keyonly3 Value: FileName:data\incomplete.xml")));
        });
    }

    [Test]
    public void Search_ReturnsNoMatchMessage_WhenNoKeysFound()
    {
        // Act
        var results = _sut.Search(@"C:\project\data", "xyz");

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0], Is.EqualTo("No matching data found."));
    }

    [Test]
    public void Search_ReturnsError_WhenDirectoryMissing()
    {
        // Act
        var results = _sut.Search(@"C:\project\missing", "hello");

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0], Is.EqualTo("Data directory does not exist."));
    }

    [TestCase("")]
    [TestCase("  ")]
    public void Search_ReturnsError_WhenSearchKeyIsEmpty(string searchKey)
    {
        // Act
        var results = _sut.Search(@"C:\project\data", searchKey);

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0], Is.EqualTo("Search key cannot be empty."));
    }

    [Test]
    public void Search_HandlesFilesWithNoData()
    {
        // Arrange - simulate a file with no data (empty content)
        _fileSystem.AddFile(@"C:\project\data\empty.csv", new MockFileData(""));

        // Act
        var results = _sut.Search(@"C:\project\data", "hello");

        // Assert - should still only find the 4 valid keys, ignoring the empty file
        Assert.That(results, Has.Count.EqualTo(4));
    }

    [Test]
    public void Search_IgnoresFilesWithUnsupportedExtensions()
    {
        // Arrange - simulate a file with an unsupported extension
        _fileSystem.AddFile(@"C:\project\data\file4.unsupported", new MockFileData("hello1,value1\nfoo,value2"));

        // Act
        var results = _sut.Search(@"C:\project\data", "hello");

        // Assert - should still only find the 4 valid keys, ignoring the unsupported file
        Assert.That(results, Has.Count.EqualTo(4));
    }

    [Test]
    public void Search_SkipsFilesWithParserException()
    {
        // Arrange - simulate a file with a supported extension but invalid content that causes parser to throw
        _fileSystem.AddFile(@"C:\project\data\file5.json", new MockFileData("not,a,valid,json"));

        // Act
        var results = _sut.Search(@"C:\project\data", "hello");

        // Assert - should still only find the 4 valid keys, ignoring the problematic file
        Assert.That(results, Has.Count.EqualTo(4));
    }
}
