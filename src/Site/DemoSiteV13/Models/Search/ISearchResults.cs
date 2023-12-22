namespace DemoSite.Models.Search;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

public interface ISearchResults
{
	long TotalRecords { get; set; }
	IEnumerable<PublishedSearchResult>? Content { get; set; }
}
