namespace DemoSite.Controllers.Search;

using DemoSite.Services;
using Microsoft.FeatureManagement;
using System;

public class SearchServiceFactory : ISearchServiceFactory
{
	private readonly IFeatureManager _featureManager;

	private readonly IServiceProvider _serviceProvider;

	public SearchServiceFactory(IFeatureManager featureManager, IServiceProvider serviceProvider)
	{
		_featureManager = featureManager;
		_serviceProvider = serviceProvider;
	}

	public async Task<ISearchService> GetSearchService()
	{
		if (await _featureManager.IsEnabledAsync(Features.DanishSite))
		{
			return _serviceProvider.GetRequiredService<ArticleSearchService>();
		}

		return _serviceProvider.GetRequiredService<SearchService>();
	}
}
