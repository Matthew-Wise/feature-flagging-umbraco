using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using Our.FeatureFlags.Editor;
using Our.FeatureFlags.Editor.Configuration;
using Our.FeatureFlags.Extensions;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace Our.FeatureFlags.NotificationHandlers
{
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
                foreach (var tab in variant.Tabs)
                {
                    tab.Properties = tab.Properties.Where(prop =>
                    {
                        if (prop.PropertyEditor.Alias.InvariantEquals(FeatureFlaggedEditor.AliasValue) == true &&
                            prop.Config.TryGetValue("__ffconfig", out var tmp) == true &&
                            tmp is FeatureFlaggedConfiguration config)
                        {
                            prop.Config.Remove("__ffconfig");

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
            }
        }
    }
}