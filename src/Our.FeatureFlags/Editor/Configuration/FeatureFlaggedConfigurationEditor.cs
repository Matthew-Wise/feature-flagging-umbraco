namespace Our.FeatureFlags.Editor.Configuration;

using Microsoft.FeatureManagement;
using Our.FeatureFlags.Extensions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

public sealed class FeatureFlaggedConfigurationEditor : ConfigurationEditor<FeatureFlaggedConfiguration>
{
    private readonly IDataTypeService _dataTypeService;
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IShortStringHelper _shortStringHelper;

    public FeatureFlaggedConfigurationEditor(
        IDataTypeService dataTypeService,
        IFeatureManager featureManager,
        IIOHelper ioHelper,
        IEditorConfigurationParser editorConfigurationParser,
        PropertyEditorCollection propertyEditors,
        IShortStringHelper shortStringHelper)
        : base(ioHelper, editorConfigurationParser)
    {
        _dataTypeService = dataTypeService;
        _propertyEditors = propertyEditors;
        _shortStringHelper = shortStringHelper;

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
            features.Add(new { value = featureName, label = featureName.SplitPascalCasing(_shortStringHelper) });
        }

        Field(nameof(FeatureFlaggedConfiguration.Features)).Config = new Dictionary<string, object> { { "prevalues", features } };
    }

    private void ConfigureRequirementField()
    {
        var requirementOptions = new List<object>(2)
        {
            new { value = RequirementType.All, label = RequirementType.All.ToString() },
            new { value = RequirementType.Any, label = RequirementType.Any.ToString() }
        };

        Field(nameof(FeatureFlaggedConfiguration.Requirement)).Config = new Dictionary<string, object> { { "prevalues", requirementOptions } };
    }

    public override FeatureFlaggedConfiguration? FromConfigurationEditor(IDictionary<string, object?>? editorValues, FeatureFlaggedConfiguration? configuration)
    {
        if (editorValues != null)
        {
            foreach ((string key, object? value) in editorValues)
            {
                switch (key)
                {
                    case nameof(FeatureFlaggedConfiguration.DataType):
                        if (int.TryParse(value?.ToString(), out var dataTypeId))
                        {
                            var dataType = _dataTypeService.GetDataType(dataTypeId);
                            if (dataType != null)
                            {
                                editorValues[key] = dataType.Key.ToString();
                            }
                        }
                        else if (UdiParser.TryParse<GuidUdi>(value?.ToString(), out var udi))
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
                }
            }
        }

        return base.FromConfigurationEditor(editorValues, configuration);
    }

    public override Dictionary<string, object> ToConfigurationEditor(FeatureFlaggedConfiguration? configuration)
    {
        var config = base.ToConfigurationEditor(configuration);

        if (configuration == null)
        {
            return config;
        }

        if (configuration.DataType == Guid.Empty)
        {
            config[nameof(FeatureFlaggedConfiguration.DataType)] = string.Empty;
        }
        else
        {
            var dataType = _dataTypeService.GetDataType(configuration.DataType);
            if (dataType != null)
            {
                config[nameof(FeatureFlaggedConfiguration.DataType)] = Udi.Create(Constants.UdiEntityType.DataType, dataType.Key).ToString();
            }
        }

        return config;
    }

    public override IDictionary<string, object> ToValueEditor(object? configuration)
    {
	    if (configuration is not FeatureFlaggedConfiguration config)
	    {
		    return base.ToValueEditor(configuration);
	    }

	    var dataType = _dataTypeService.GetDataType(config.DataType);
        if (dataType != null && _propertyEditors.TryGet(dataType.EditorAlias, out var dataEditor))
        {
	        var config2 = dataEditor.GetConfigurationEditor().ToValueEditor(dataType.Configuration);
	        if (config2 != null)
	        {
		        if (config2.ContainsKey("__ffconfig") == false)
		        {
			        config2.Add("__ffconfig", config);
		        }

		        return config2;
	        }
        }

        return base.ToValueEditor(configuration);
    }
}