namespace DemoSite.Controllers.Search;

using DemoSite.Services;

public interface ISearchServiceFactory
{
	Task<ISearchService> GetSearchService();
}