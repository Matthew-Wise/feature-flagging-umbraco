namespace Our.FeatureFlags.Editor;

using System.Collections.Generic;
using Microsoft.FeatureManagement;
using Our.FeatureFlags.Editor.Configuration;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

public class FeatureFlaggedEditor : IDataEditor
{
    public const string AliasValue = "Our.FeatureFlags.FeatureFlagged";

    private readonly IDataTypeService _dataTypeService;
    private readonly IFeatureManager _featureManager;
    private readonly IIOHelper _ioHelper;
    private readonly IEditorConfigurationParser _editorConfigurationParser;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILocalizedTextService _localizedTextService;
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IShortStringHelper _shortStringHelper;

    public FeatureFlaggedEditor(
        IDataTypeService dataTypeService,
        IFeatureManager featureManager,
        IIOHelper ioHelper,
        IEditorConfigurationParser editorConfigurationParser,
        IJsonSerializer jsonSerializer,
        ILocalizedTextService localizedTextService,
        PropertyEditorCollection propertyEditors,
        IShortStringHelper shortStringHelper)
    {
        _dataTypeService = dataTypeService;
        _featureManager = featureManager;
        _ioHelper = ioHelper;
        _editorConfigurationParser = editorConfigurationParser;
        _jsonSerializer = jsonSerializer;
        _localizedTextService = localizedTextService;
        _propertyEditors = propertyEditors;
        _shortStringHelper = shortStringHelper;            
    }

    public string Alias => AliasValue;

    public EditorType Type => EditorType.PropertyValue;

    public string Name => "Feature Flagged";

    public string Icon => "icon-code";

    public string Group => Constants.PropertyEditors.Groups.RichContent;

    public bool IsDeprecated => false;

    public IDictionary<string, object>? DefaultConfiguration => default;

    public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

    public IConfigurationEditor GetConfigurationEditor() => new FeatureFlaggedConfigurationEditor(
        _dataTypeService,
        _featureManager,
        _ioHelper,
        _editorConfigurationParser,
        _propertyEditors,
        _shortStringHelper);

    public IDataValueEditor GetValueEditor()
    {
        return new DataValueEditor(
             _localizedTextService,
             _shortStringHelper,
             _jsonSerializer)
        {
            ValueType = ValueTypes.Json,
            View = "readonlyvalue",
        };
    }

    public IDataValueEditor GetValueEditor(object? configuration)
    {
        if (configuration is FeatureFlaggedConfiguration config)
        {
            var dataType = _dataTypeService.GetDataType(config.DataType);
            if (dataType != null && _propertyEditors.TryGet(dataType.EditorAlias, out var dataEditor) == true)
            {
                return dataEditor.GetValueEditor(dataType.Configuration);
            }
        }

        return GetValueEditor();
    }
}