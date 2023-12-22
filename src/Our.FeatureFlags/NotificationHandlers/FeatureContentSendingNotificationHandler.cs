namespace Our.FeatureFlags.NotificationHandlers;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using Our.FeatureFlags.Editor;
using Our.FeatureFlags.Editor.Configuration;
using Our.FeatureFlags.Extensions;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

public class FeatureContentSendingNotificationHandler : INotificationAsyncHandler<SendingContentNotification>
{
    private readonly IFeatureManager _featureManager;

    public FeatureContentSendingNotificationHandler(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    public async Task HandleAsync(SendingContentNotification notification, CancellationToken cancellationToken)
    {
        var enabledFeatures = await _featureManager.GetEnabledFeatures();

        foreach (var variant in notification.Content.Variants)
        {
			var tabsWithNoProperties = new List<Tab<ContentPropertyDisplay>>();
            foreach (var tab in variant.Tabs)
            {
                if(tab.Properties == null || !tab.Properties.Any())
                {
					tabsWithNoProperties.Add(tab);
                    continue;
                }

                tab.Properties = tab.Properties.Where(prop =>
                {
                    if (prop.PropertyEditor?.Alias.InvariantEquals(FeatureFlaggedEditor.AliasValue) == true &&
                        prop.ConfigNullable?.TryGetValue("__ffconfig", out var tmp) == true &&
                        tmp is FeatureFlaggedConfiguration config)
                    {
                        prop.ConfigNullable.Remove("__ffconfig");

                        var enabled = config.Requirement switch
                        {
                            RequirementType.Any => enabledFeatures.ContainsAny(config.Features),
                            RequirementType.All => enabledFeatures.ContainsAll(config.Features),
                            _ => false,
                        };

                        if (enabled == false)
                        {
                            return false;
                        }
                    }

                    return true;

                }).ToList();

                if (tab.Properties.Any() == false)
                {
                    tab.Type = string.Empty;
                }
            }

			foreach(var tab in tabsWithNoProperties)
			{
				if (string.IsNullOrWhiteSpace(tab.Alias))
				{
					continue;
				}

				var groups = variant.Tabs.Where(t => t.Alias?.StartsWith(tab.Alias) == true && tab.Type != "tab");
				if(groups.All(g => string.IsNullOrWhiteSpace(g.Type)))
				{
					tab.Type = string.Empty;
				}
			}
			
        }
    }
}