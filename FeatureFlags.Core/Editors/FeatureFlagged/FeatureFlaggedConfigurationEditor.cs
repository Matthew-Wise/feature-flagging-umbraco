namespace FeatureFlags.Core.Editors.FeatureFlagged
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    using Microsoft.FeatureManagement;

    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.IO;
    using Umbraco.Cms.Core.PropertyEditors;
    using Umbraco.Cms.Core.PropertyEditors.Validators;
    using Umbraco.Cms.Core.Services;

    internal sealed class FeatureFlaggedConfigurationEditor : ConfigurationEditor<FeatureFlaggedConfiguration>
    {
        private readonly ILocalizedTextService _localizedTextService;

        public FeatureFlaggedConfigurationEditor(
            IIOHelper ioHelper,
            IFeatureManager featureManager,
            ILocalizedTextService localizedTextService) : base(ioHelper)
        {
            _localizedTextService = localizedTextService;
            CreateFeaturesSetting(featureManager).GetAwaiter().GetResult();
            CreateRequirementSetting();
            CreateDataTypePickerSetting();
        }

        private void CreateDataTypePickerSetting()
        {
            Fields.Add(new ConfigurationField
                       {
                           Key = nameof(FeatureFlaggedConfiguration.DataType),
                           Name = "Data type",
                           View = "treepicker",
                           Description = "The data type to feature flag",
                           Validators = { new RequiredValidator(_localizedTextService) },
                           Config = new Dictionary<string, object>
                                    {
                                        {"multiPicker", false},
                                        {"entityType", "DataType"},
                                        {"type", Constants.Applications.Settings},
                                        {"treeAlias", Constants.Trees.DataTypes},
                                        {"idType", "id"}
                                    }
                       });
        }

        private async Task CreateFeaturesSetting(IFeatureManager featureManager)
        {
            var features = new List<object>();
            await foreach (var featureName in featureManager.GetFeatureNamesAsync())
            {
                features.Add(new { value = featureName, label = featureName });
            }

            Fields.Add(new ConfigurationField
            {
                Key = nameof(FeatureFlaggedConfiguration.Features),
                Name = "Features",
                View = "checkboxlist",
                PropertyType = typeof(List<string>),
                Description = "Features which control if this editor should be rendered",
                Validators = { new RequiredValidator(_localizedTextService) },
                Config = new Dictionary<string, object> { { "prevalues", features } },
                HideLabel = false
            });
        }

        private void CreateRequirementSetting()
        {
            var requirementOptions = new List<object>(2) {
             new { value = RequirementType.All, label = RequirementType.All.ToString() },
             new { value = RequirementType.Any, label = RequirementType.Any.ToString() }
            };

            Fields.Add(new ConfigurationField
            {
                Key = nameof(FeatureFlaggedConfiguration.Requirement),
                Name = "Requirement",
                View = "radiobuttonlist",
                Description = "Controls whether 'All' or 'Any' feature in a list of features should be enabled to render the editor",
                Config = new Dictionary<string, object> { { "prevalues", requirementOptions } }
            });

            DefaultConfiguration[nameof(FeatureFlaggedConfiguration.Requirement)] = RequirementType.All;
        }
    }
}