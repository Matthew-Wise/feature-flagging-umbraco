namespace FeatureFlags.Core.Editors.FeatureFlagged
{
    using FeatureFlags.Core.Constants;

    using Microsoft.FeatureManagement;
    using Umbraco.Cms.Core.IO;
    using Umbraco.Cms.Core.PropertyEditors;
    using Umbraco.Cms.Core.Services;

    [DataEditor(
        alias: DataTypes.FeatureFlagged.Alias,
        name: "Feature Flagged",
        view: "~/App_Plugins/FeatureFlagged/featureFlagged.html",
        ValueType = ValueTypes.Json,
        Group = "Rich Content",
        Icon = "icon-code")]
    public class FeatureFlaggedEditor : DataEditor
    {
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
            => new FeatureFlaggedConfigurationEditor(_featureManager,_dataTypeService, _iOHelper);
    }
}