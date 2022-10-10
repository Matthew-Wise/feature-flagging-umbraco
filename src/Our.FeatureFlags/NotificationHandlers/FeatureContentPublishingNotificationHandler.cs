namespace Our.FeatureFlags.NotificationHandlers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using Our.FeatureFlags.Editor;
using Our.FeatureFlags.Editor.Configuration;
using Our.FeatureFlags.Extensions;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

public class FeatureContentPublishingNotificationHandler : INotificationAsyncHandler<ContentPublishingNotification>
{
    private readonly IFeatureManager _featureManager;

    private readonly IDataTypeService _dataTypeService;

    public FeatureContentPublishingNotificationHandler(IFeatureManager featureManager, IDataTypeService dataTypeService)
    {
        _featureManager = featureManager;
        _dataTypeService = dataTypeService;
    }

    public async Task HandleAsync(ContentPublishingNotification notification, CancellationToken cancellationToken)
    {
        var enabledFeatures = await _featureManager.GetEnabledFeatures();

        foreach (var entity in notification.PublishedEntities)
        {
            foreach (var prop in entity.Properties)
            {
                if (prop.PropertyType.PropertyEditorAlias.InvariantEquals(FeatureFlaggedEditor.AliasValue) == false)
                {
                    continue;
                }

                var dataType = _dataTypeService.GetDataType(prop.PropertyType.DataTypeId);
                var config = ConfigurationEditor.ConfigurationAs<FeatureFlaggedConfiguration>(dataType?.Configuration);

                var enabled = config?.Requirement switch
                {
                    RequirementType.Any => enabledFeatures.ContainsAny(config.Features),
                    RequirementType.All => enabledFeatures.ContainsAll(config.Features),
                    _ => throw new InvalidOperationException($"Configured requirement for property ({prop.PropertyType.Alias}) had no matching {nameof(RequirementType)} "),
                };

                if (enabled == false)
                {
	                prop.PropertyType.ValidationRegExp = null;
                    prop.PropertyType.Mandatory = false;
                }
            }
        }
    }
}