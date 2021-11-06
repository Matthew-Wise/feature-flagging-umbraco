namespace FeatureFlags.Core.NotificationHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FeatureFlags.Core.Constants;
    using FeatureFlags.Core.Editors.FeatureFlagged;
    using FeatureFlags.Core.Extensions;

    using Microsoft.FeatureManagement;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    using Umbraco.Cms.Core.Events;
    using Umbraco.Cms.Core.IO;
    using Umbraco.Cms.Core.Models.ContentEditing;
    using Umbraco.Cms.Core.Notifications;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Extensions;

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
                    if (prop.PropertyEditor.Alias != DataTypes.FeatureFlagged.Alias)
                    {
                        tabHasProperties = true;
                        continue;
                    }

                    ConfigureFeatureProperty(prop, enabledFeatures);
                }

                if (!tabHasProperties)
                {
                    tab.Type = string.Empty;
                }
            }
        }

        private void ConfigureFeatureProperty(ContentPropertyDisplay prop, IEnumerable<string> enabledFeatures)
        {

            var propFeatures = prop.Config[nameof(FeatureFlaggedConfiguration.Features)] as List<string>;
            if (propFeatures?.Any() == false)
            {
                return;
            }

            if (!Enum.TryParse(prop.Config[nameof(FeatureFlaggedConfiguration.Requirement)].ToString(), out RequirementType requirement))
            {
                return;
            }

            if (!int.TryParse(prop.Config[nameof(FeatureFlaggedConfiguration.DataType)].ToString(), out var dataTypeId))
            {
                throw new InvalidOperationException($"{nameof(FeatureFlaggedConfiguration.DataType)} could not be converted to an int");
            }
            
            var enabled = requirement switch
            {
                RequirementType.Any => enabledFeatures.ContainsAny(propFeatures),
                RequirementType.All => enabledFeatures.ContainsAll(propFeatures),
                _ => throw new InvalidOperationException($"Configured requirement ({requirement}) had no matching {nameof(RequirementType)} "),
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
            }
            
            var dataType = _dataTypeService.GetDataType(dataTypeId);
            var dataTypeModel = dataType.Editor.GetValueEditor(dataType.Configuration);

            var serializer = JsonSerializer.CreateDefault();
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            

            //TODO: FIX VIEW ROUTING
            prop.Config["dataTypeSettings"] = JObject.FromObject(new
                                              {
                                                  view = $"views/propertyeditors/{dataTypeModel.View}/{dataTypeModel.View}.html",
                                                  config = dataType.Configuration,
                                                  validation = prop.Validation,
                                                  label = prop.Label
                                              }, serializer);
        }
    }
}