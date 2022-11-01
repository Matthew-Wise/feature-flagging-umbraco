namespace DemoSite.Controllers.Search;

using DemoSite.Models.ViewModels;
using DemoSite.Services;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

public class SearchController : RenderController
{
	private readonly ISearchServiceFactory _searchServiceFactory;

	private readonly IPublishedValueFallback _publishedValueFallback;
	public SearchController(ILogger<SearchController> logger, 
		ICompositeViewEngine compositeViewEngine, 
		IUmbracoContextAccessor umbracoContextAccessor,
		ISearchServiceFactory searchServiceFactory, 
		IPublishedValueFallback publishedValueFallback) : base(logger, compositeViewEngine, umbracoContextAccessor)
	{
		_searchServiceFactory = searchServiceFactory;
		_publishedValueFallback = publishedValueFallback;
	}

	public async Task<IActionResult> Search()
	{
		if(CurrentPage == null)
		{
			return NotFound();
		}

		var searchQuery = Request.Query["q"];
		var searchService = await _searchServiceFactory.GetSearchService();
		var results = searchService.GetSearchResults(searchQuery);

		var model = new SearchViewModel(CurrentPage, _publishedValueFallback)
		{
			Results = results,
			SearchQuery = searchQuery
		};
		return View(model);
	}
}
