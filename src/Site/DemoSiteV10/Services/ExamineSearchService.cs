namespace DemoSite.Services;

using DemoSite.Models.Search;
using Examine;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Web.Common.PublishedModels;
using ISearchResults = Models.Search.ISearchResults;

public sealed class ArticleSearchService : ISearchService
{

	private readonly IExamineManager _examineManager;
	private readonly IPublishedSnapshotAccessor _publishedSnapshot;

	public ArticleSearchService(IExamineManager examineManager, IPublishedSnapshotAccessor publishedSnapshot)
	{
		_examineManager = examineManager;
		_publishedSnapshot = publishedSnapshot;
	}

	public ISearchResults GetSearchResults(string searchTerm)
	{
		if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out var index) || string.IsNullOrWhiteSpace(searchTerm))
		{
			return new SearchResults();
		}

		var results = index
		.Searcher
		.CreateQuery("content")
		.ManagedQuery(searchTerm)
		.And()
		.NodeTypeAlias(Article.ModelTypeAlias)		
		.Execute(new Examine.Search.QueryOptions(0, 10));

		return new SearchResults
		{
			TotalRecords = results.TotalItemCount,
			Content = results.ToPublishedSearchResults(_publishedSnapshot.GetRequiredPublishedSnapshot().Content)
		};
	}
}
