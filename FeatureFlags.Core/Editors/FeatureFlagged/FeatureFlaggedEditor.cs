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
        
        private readonly ILocalizedTextService _localizedTextService;

        private readonly IIOHelper _ioHelper;

        public FeatureFlaggedEditor(
            IDataValueEditorFactory dataValueEditorFactory,
            IIOHelper ioHelper,
            IFeatureManager featureManager,
            ILocalizedTextService localizedTextService,
            EditorType type = EditorType.PropertyValue)
            : base(dataValueEditorFactory, type)
        {
            _featureManager = featureManager;
            _localizedTextService = localizedTextService;
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() 
            => new FeatureFlaggedConfigurationEditor(_ioHelper, _featureManager, _localizedTextService);
    }
}