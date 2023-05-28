namespace DemoSite.Controllers.Search;

using DemoSite.Services;
using Microsoft.FeatureManagement;
using System;

public class SearchServiceFactory : ISearchServiceFactory
{
	private readonly IFeatureManagerSnapshot _featureManager;

	private readonly IServiceProvider _serviceProvider;

	public SearchServiceFactory(IFeatureManagerSnapshot featureManager, IServiceProvider serviceProvider)
	{
		_featureManager = featureManager;
		_serviceProvider = serviceProvider;
	}

	public async Task<ISearchService> GetSearchService()
	{
		if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.DanishSite)))
		{
			return _serviceProvider.GetRequiredService<ArticleSearchService>();
		}

		return _serviceProvider.GetRequiredService<SearchService>();
	}
}
