namespace DemoSite.Backoffice;

using Microsoft.FeatureManagement;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

public class MenuRenderingNotificationHandler : INotificationHandler<SendingAllowedChildrenNotification>
{
	private readonly IFeatureManager _featureManager;

	public MenuRenderingNotificationHandler(IFeatureManager featureManager)
	{
		_featureManager = featureManager;
	}

	public async void Handle(SendingAllowedChildrenNotification notification)
	{
		if (await _featureManager.IsEnabledAsync(Features.BetaTemplate))
		{
			return;
		}
		
		notification.Children = notification.Children.Where(c => c.Alias != BetaTemplate.ModelTypeAlias);
	}
}