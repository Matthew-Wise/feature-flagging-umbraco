namespace DemoSite.Services;

using DemoSite.Models.Search;

public interface ISearchService
{
	ISearchResults GetSearchResults(string searchTerm);
}