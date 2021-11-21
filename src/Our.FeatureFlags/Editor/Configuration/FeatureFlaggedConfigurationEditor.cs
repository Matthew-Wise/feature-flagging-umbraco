using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.FeatureManagement;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Our.FeatureFlags.Editor.Configuration
{
    public sealed class FeatureFlaggedConfigurationEditor : ConfigurationEditor<FeatureFlaggedConfiguration>
    {
        private readonly IDataTypeService _dataTypeService;

        public FeatureFlaggedConfigurationEditor(
            IFeatureManager featureManager,
            IDataTypeService dataTypeService,
            IIOHelper ioHelper) : base(ioHelper)
        {
            _dataTypeService = dataTypeService;
            Task.Run(async () => await ConfigureFeaturesField(featureManager)).GetAwaiter().GetResult();
            ConfigureRequirementField();
            ConfigureDataTypeField();
        }

        private void ConfigureDataTypeField()
        {
            Field(nameof(FeatureFlaggedConfiguration.DataType)).Config = new Dictionary<string, object>
                                    {
                                        {"multiPicker", false},
                                        {"entityType", "DataType"},
                                        {"type", Constants.Applications.Settings},
                                        {"treeAlias", Constants.Trees.DataTypes},
                                        {"idType", "id"}
                                    };
        }

        private async Task ConfigureFeaturesField(IFeatureManager featureManager)
        {
            var features = new List<object>();
            await foreach (var featureName in featureManager.GetFeatureNamesAsync())
            {
                features.Add(new { value = featureName, label = featureName });
            }

            Field(nameof(FeatureFlaggedConfiguration.Features)).Config = new Dictionary<string, object> { { "prevalues", features } };
        }

        private void ConfigureRequirementField()
        {
            var requirementOptions = new List<object>(2) {
             new { value = RequirementType.All, label = RequirementType.All.ToString() },
             new { value = RequirementType.Any, label = RequirementType.Any.ToString() }
            };

            Field(nameof(FeatureFlaggedConfiguration.Requirement)).Config = new Dictionary<string, object> { { "prevalues", requirementOptions } };
        }

        public override FeatureFlaggedConfiguration FromConfigurationEditor(IDictionary<string, object> editorValues, FeatureFlaggedConfiguration configuration)
        {
            foreach ((var key, var value) in editorValues)
            {
                switch (key)
                {
                    case nameof(FeatureFlaggedConfiguration.DataType):
                        if (int.TryParse(value.ToString(), out var dataTypeId))
                        {
                            var dataType = _dataTypeService.GetDataType(dataTypeId);
                            if (dataType != null)
                            {
                                editorValues[key] = dataType.Key.ToString();                                
                            }
                        }                        
                        else if (UdiParser.TryParse<GuidUdi>(value.ToString(), out var udi))
                        {
                            editorValues[key] = udi.Guid;
                        }
                        else
                        {
                            throw new InvalidOperationException($"{nameof(FeatureFlaggedConfiguration.DataType)} is required");
                        }
                        continue;
                    default:
                        continue;
                };
            }
            return base.FromConfigurationEditor(editorValues, configuration);
        }

        public override Dictionary<string, object> ToConfigurationEditor(FeatureFlaggedConfiguration configuration)
        {
            var config = base.ToConfigurationEditor(configuration);

            if(configuration.DataType == Guid.Empty)
            {
                config[nameof(FeatureFlaggedConfiguration.DataType)] = null;
            }
            else {
                var dataType = _dataTypeService.GetDataType(configuration.DataType);
                config[nameof(FeatureFlaggedConfiguration.DataType)] = GuidUdi.Create(Umbraco.Cms.Core.Constants.UdiEntityType.DataType, dataType.Key).ToString();
            }

            return config;
        }
    }
}