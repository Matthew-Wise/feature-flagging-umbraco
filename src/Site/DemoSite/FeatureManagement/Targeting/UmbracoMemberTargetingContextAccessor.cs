namespace DemoSite.FeatureManagement.Targeting;

using Microsoft.FeatureManagement.FeatureFilters;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Security;

public class UmbracoMemberTargetingContextAccessor : ITargetingContextAccessor
{
	private const string TargetingContextLookup = "UmbracoContextTargetingContextAccessor.TargetingContext";

	private readonly IHttpContextAccessor _httpContextAccessor;

	public UmbracoMemberTargetingContextAccessor(
		IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}


	public async ValueTask<TargetingContext> GetContextAsync() => await GetContextAsync(default);
	public async ValueTask<TargetingContext> GetContextAsync(CancellationToken cancellationToken = default)
	{
		HttpContext httpContext = _httpContextAccessor.HttpContext!;

		if (httpContext.Items.TryGetValue(TargetingContextLookup, out object? value) == true && value != null)
		{
			return (TargetingContext)value;
		}

		var memberManager = httpContext.RequestServices.GetRequiredService<IMemberManager>();

		var member = await memberManager.GetCurrentMemberAsync();
		IList<string> groups = new List<string>(0);

		if (member != null)
		{
			groups = await memberManager.GetRolesAsync(member);
		}

		var targetingContext = new TargetingContext
		{
			UserId = member?.Name ?? string.Empty,
			Groups = groups
		};

		
		httpContext.Items[TargetingContextLookup] = targetingContext;

		return targetingContext;
	}
}
