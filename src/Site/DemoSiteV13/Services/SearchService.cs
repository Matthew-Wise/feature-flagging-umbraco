namespace DemoSite.Services;

using DemoSite.Models.Search;
using System.Globalization;
using Umbraco.Cms.Core;

public sealed class SearchService : ISearchService
{
	private readonly IPublishedContentQuery publishedContentQuery;

	public SearchService(IPublishedContentQuery publishedContentQuery)
	{
		this.publishedContentQuery = publishedContentQuery;
	}

	public ISearchResults GetSearchResults(string searchTerm)
	{
		if (string.IsNullOrWhiteSpace(searchTerm))
		{
			return new SearchResults();
		}
		
		var results = publishedContentQuery
			.Search(searchTerm, 0, 10, out long totalRecords, 
				CultureInfo.CurrentUICulture.Name);
		return new SearchResults
		{
			TotalRecords = totalRecords,
			Content = results
		};
	}
}
