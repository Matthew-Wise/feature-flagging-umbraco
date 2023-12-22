namespace DemoSite.Models.Search;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

public class SearchResults : ISearchResults
{
	public long TotalRecords { get; set; }

	public IEnumerable<PublishedSearchResult>? Content { get; set; }
}
