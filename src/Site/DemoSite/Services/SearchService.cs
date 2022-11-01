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
		var results = publishedContentQuery.Search(searchTerm, 0, 10, out var totalRecords, CultureInfo.CurrentUICulture.Name, Constants.UmbracoIndexes.ExternalIndexName);
		return new SearchResults
		{
			TotalRecords = totalRecords,
			Content = results
		};
	}
}
