namespace DemoSite.Models.ViewModels;

using DemoSite.Models.Search;
using Microsoft.Extensions.Primitives;
using Umbraco.Cms.Core.Models.PublishedContent;

public class SearchViewModel : PublishedContentWrapped
{
	public SearchViewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
	{
	}

	public ISearchResults? Results { get; internal set; }
	public StringValues SearchQuery { get; internal set; }
}
