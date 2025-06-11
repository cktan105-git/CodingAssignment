using System.Collections.Generic;

namespace CodingAssignmentLib.Abstractions;

public interface ISearchService
{
    List<string> Search(string baseDirectory, string searchKey);
}