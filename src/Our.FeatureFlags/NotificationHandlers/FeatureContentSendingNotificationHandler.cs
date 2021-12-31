using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.FeatureManagement;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Our.Umbraco.FeatureFlags.Extensions;
using Our.Umbraco.FeatureFlags.Editor;
using Our.Umbraco.FeatureFlags.Editor.Configuration;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Our.Umbraco.FeatureFlags.NotificationHandlers
{
    public class FeatureContentSendingNotificationHandler : INotificationAsyncHandler<SendingContentNotification>
    {
        private readonly IFeatureManager _featureManager;

        private readonly IDataTypeService _dataTypeService;

        public FeatureContentSendingNotificationHandler(IFeatureManager featureManager, IDataTypeService dataTypeService)
        {
            _featureManager = featureManager;
            _dataTypeService = dataTypeService;
        }
        public async Task HandleAsync(SendingContentNotification notification, CancellationToken cancellationToken)
        {

            var enabledFeatures = await _featureManager.GetEnabledFeatures();

            foreach (var tab in notification.Content.Variants.SelectMany(v => v.Tabs))
            {
                var tabHasProperties = false;
                foreach (var prop in tab.Properties)
                {
                    if (prop.PropertyEditor.Alias != FeatureFlaggedEditor.AliasValue)
                    {
                        tabHasProperties = true;
                        continue;
                    }

                    if (ConfigureFeatureProperty(prop, enabledFeatures))
                    {
                        tabHasProperties = true;
                    }
                }

                if (!tabHasProperties)
                {
                    tab.Type = string.Empty;
                }
            }
        }

        private bool ConfigureFeatureProperty(ContentPropertyDisplay prop, IEnumerable<string> enabledFeatures)
        {
            var config = prop.PropertyEditor.GetConfigurationEditor().FromConfigurationEditor(prop.Config, null) as FeatureFlaggedConfiguration;
            var enabled = config.Requirement switch
            {
                RequirementType.Any => enabledFeatures.ContainsAny(config.Features),
                RequirementType.All => enabledFeatures.ContainsAll(config.Features),
                _ => throw new InvalidOperationException($"Configured requirement ({config.Requirement}) had no matching {nameof(RequirementType)}"),
            };

            if (prop.Config.Keys.Contains("featureEnabled"))
            {
                prop.Config["featureEnabled"] = enabled;
            }
            else
            {
                prop.Config.Add("featureEnabled", enabled);
            }

            if (enabled == false)
            {
                prop.Readonly = true;
                prop.HideLabel = true;
                prop.Validation = new PropertyTypeValidation { Mandatory = false, Pattern = string.Empty };
                return enabled;
            }
            
            var dataType = _dataTypeService.GetDataType(config.DataType);

            var dataTypeModel = dataType.Editor.GetValueEditor(dataType.Configuration);
                var serializer = JsonSerializer.CreateDefault();
                serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();

                prop.Config["dataTypeSettings"] = JObject.FromObject(new
                {
                    view = dataTypeModel.View,
                    config = dataType.Editor.GetConfigurationEditor().ToValueEditor(dataType.Configuration),
                    validation = prop.Validation,
                    label = prop.Label
                }, serializer);

            return enabled;
        }
    }
}