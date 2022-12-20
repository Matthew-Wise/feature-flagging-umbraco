namespace Our.FeatureFlags.Filters.UmbracoBackOfficeUser;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using System.Security.Claims;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

public sealed class UmbracoBackOfficeUserFilter : IFeatureFilter
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	//TODO:  Can we use the service provider in the http context?
    private readonly IServiceProvider _serviceProvider;

    public UmbracoBackOfficeUserFilter(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        this._serviceProvider = serviceProvider;
    }

    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        // We are a singleton but we need scoped information
        using IServiceScope scope = _serviceProvider.CreateScope();

        var settings = context.Parameters.Get<UmbracoBackOfficeUserFilterSettings>();
        var backOfficeUser = BackofficeUser(scope.ServiceProvider.GetService<IOptionsSnapshot<CookieAuthenticationOptions>>());

        if (backOfficeUser == null || settings == null)
        {
            return Task.FromResult(false);
        }

        var email = backOfficeUser.GetEmail();
        var groups = backOfficeUser.GetRoles();

        if (string.IsNullOrWhiteSpace(email) || groups.Any() != true)
        {
            return Task.FromResult(false);
        }

        return settings.Match switch
        {
            RequirementType.All => Task.FromResult(settings.EmailAddresses.IsMatch(email) && settings.Groups.IsMatch(groups)),
            RequirementType.Any => Task.FromResult(settings.EmailAddresses.IsMatch(email) || settings.Groups.IsMatch(groups)),
            _ => Task.FromResult(false),
        };

    }

    /// <summary>
    /// Gets the backoffice user from the cookie
    /// https://our.umbraco.com/forum/umbraco-9/106857-how-do-i-determine-if-a-backoffice-user-is-logged-in-from-a-razor-view#comment-334423
    /// </summary>
    private ClaimsIdentity? BackofficeUser(IOptionsSnapshot<CookieAuthenticationOptions>? cookieOptionsSnapshot)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || cookieOptionsSnapshot == null)
        {
            return null;
        }

        var cookieOptions = cookieOptionsSnapshot.Get(Constants.Security.BackOfficeAuthenticationType);
        string? backOfficeCookie = string.IsNullOrWhiteSpace(cookieOptions.Cookie.Name) ? null : httpContext.Request.Cookies[cookieOptions.Cookie.Name];

        if (string.IsNullOrEmpty(backOfficeCookie))
        {
            return null;
        }

        var unprotected = cookieOptions.TicketDataFormat.Unprotect(backOfficeCookie);
        var backOfficeIdentity = unprotected?.Principal.GetUmbracoIdentity();

        backOfficeIdentity?.VerifyBackOfficeIdentity(out backOfficeIdentity);

        return backOfficeIdentity;

    }
}
