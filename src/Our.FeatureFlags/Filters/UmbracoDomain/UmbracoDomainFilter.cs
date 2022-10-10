namespace Our.FeatureFlags.Filters.UmbracoDomain;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

public sealed class UmbracoDomainFilter : IFeatureFilter
{
	private IHttpContextAccessor HttpContextAccessor { get; }


	public UmbracoDomainFilter(IHttpContextAccessor httpContextAccessor)
	{
		HttpContextAccessor = httpContextAccessor;
	}

	public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
	{
		var settings = context.Parameters.Get<UmbracoDomainFilterSettings>();

		if (settings?.Domains.Any() != true)
		{
			return Task.FromResult(false);
		}
		
		var serviceProvider = HttpContextAccessor.HttpContext?.RequestServices;
		if (serviceProvider == null)
		{
			return Task.FromResult(false);
		}
		
		using var umbracoContext = serviceProvider
			.GetRequiredService<IUmbracoContextAccessor>().GetRequiredUmbracoContext();

		if (umbracoContext.IsFrontEndUmbracoRequest() != true)
		{
			return Task.FromResult(false);
		}

		var content = umbracoContext.PublishedRequest?.PublishedContent;

		if (content == null)
		{
			return Task.FromResult(false);
		}

		List<Domain>? domains = umbracoContext.Domains?.GetAssigned(content.Id, true).ToList();
		if (domains?.Any() != true)
		{
			return Task.FromResult(false);
		}

		var siteDomainMapper = serviceProvider.GetRequiredService<ISiteDomainMapper>();

		var currentDomain = DomainUtilities.SelectDomain(
			domains,
			umbracoContext.CleanedUmbracoUrl,
			umbracoContext.PublishedRequest?.Culture,
			umbracoContext.Domains?.DefaultCulture,
			siteDomainMapper.MapDomain);
		
		if (currentDomain == null)
		{
			return Task.FromResult(false);
		}
		
		return Task.FromResult(settings.Domains.Contains(currentDomain.Uri));
	}
}
