namespace DemoSite.NotificationHandlers;

using DemoSite.FeatureManagement.SendingContentNotification;
using Microsoft.FeatureManagement;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;

public class ContentSendingNotificationHandler : INotificationAsyncHandler<SendingContentNotification>
{
	private readonly IFeatureManager _featureManager;
	private readonly IBackOfficeSecurityAccessor backOfficeSecurityAccessor;

	public ContentSendingNotificationHandler(IFeatureManager featureManager, IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
	{
		_featureManager = featureManager;
		this.backOfficeSecurityAccessor = backOfficeSecurityAccessor;
	}
	public async Task HandleAsync(SendingContentNotification notification, CancellationToken cancellationToken)
	{
		SendingContentNotificationContext context = new(notification, backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser);
		
		if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.RestrictPublish), context))
		{
			var filtered = notification.Content.AllowedActions?.ToList() ?? new List<string>();
			filtered.Remove(ActionPublish.ActionLetter.ToString());
			notification.Content.AllowedActions = filtered;
		}

		if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.HideURLsAndPreview), context))
		{
			notification.Content.Urls = null;
			notification.Content.AllowPreview = false;
		}
	}
}
