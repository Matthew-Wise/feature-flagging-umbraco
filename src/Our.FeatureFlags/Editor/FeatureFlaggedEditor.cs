using Microsoft.FeatureManagement;
using Our.Umbraco.FeatureFlags.Editor.Configuration;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.FeatureFlags.Editor
{
    [DataEditor(
        alias: AliasValue,
        name: "Feature Flagged",
        view: "~/App_Plugins/Our.Umbraco.FeatureFlags/editor.html",
        ValueType = ValueTypes.Json,
        Group = "Rich Content",
        Icon = "icon-code")]
    public class FeatureFlaggedEditor : DataEditor
    {
        public const string AliasValue = "Our.FeatureFlags.FeatureFlagged";

        private readonly IFeatureManager _featureManager;

        private readonly IIOHelper _iOHelper;
        private readonly IDataTypeService _dataTypeService;

        public FeatureFlaggedEditor(
            IDataValueEditorFactory dataValueEditorFactory,
            IFeatureManager featureManager,
            IIOHelper iOHelper,
            IDataTypeService dataTypeService,
            EditorType type = EditorType.PropertyValue)
            : base(dataValueEditorFactory, type)
        {
            _featureManager = featureManager;
            _iOHelper = iOHelper;
            _dataTypeService = dataTypeService;
        }

        protected override IConfigurationEditor CreateConfigurationEditor()
            => new FeatureFlaggedConfigurationEditor(_featureManager, _dataTypeService, _iOHelper);
    }
}