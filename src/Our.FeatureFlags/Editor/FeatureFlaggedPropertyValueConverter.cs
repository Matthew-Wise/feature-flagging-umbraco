using System;
using System.Threading.Tasks;

using Microsoft.FeatureManagement;
using Our.FeatureFlags.Extensions;
using Our.FeatureFlags.Editor.Configuration;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Our.FeatureFlags.Editor
{
    public class FeatureFlaggedPropertyValueConverter : PropertyValueConverterBase
    {
        private readonly PropertyValueConverterCollection _propertyValueConverters;
        private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;
        private readonly IPublishedModelFactory _publishedModelFactory;
        private readonly IDataTypeService _dataTypeService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IFeatureManager _featureManager;

        public FeatureFlaggedPropertyValueConverter(
            PropertyValueConverterCollection propertyValueConverters,
            IPublishedContentTypeFactory publishedContentTypeFactory,
            IPublishedModelFactory publishedModelFactory,
            IDataTypeService dataTypeService,
            IShortStringHelper shortStringHelper,
            IFeatureManager featureManager)
        {
            _propertyValueConverters = propertyValueConverters;
            _publishedContentTypeFactory = publishedContentTypeFactory;
            _publishedModelFactory = publishedModelFactory;
            _dataTypeService = dataTypeService;
            _shortStringHelper = shortStringHelper;
            _featureManager = featureManager;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
            => propertyType.EditorAlias == FeatureFlaggedEditor.AliasValue;

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var innerProperty = GetInnerPropertyType(propertyType, out var config);
            var enablebledFeatures = Task.Run(async () => await _featureManager.GetEnabledFeatures()).GetAwaiter().GetResult();
            var propertyEnabled = config.Requirement switch
            {
                RequirementType.All => enablebledFeatures.ContainsAll(config.Features),
                RequirementType.Any => enablebledFeatures.ContainsAny(config.Features),
                _ => throw new InvalidOperationException($"RequirementType {config.Features} was not handled"),
            };
            return innerProperty.ConvertInterToObject(owner, referenceCacheLevel, propertyEnabled ? inter : null, preview);
        }

        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
            => GetInnerPropertyType(propertyType, out _).ConvertInterToXPath(owner, referenceCacheLevel, inter, preview);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
            => GetInnerPropertyType(propertyType, out _).ConvertSourceToInter(owner, source, preview);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
            => GetInnerPropertyType(propertyType, out _).CacheLevel;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
            => GetInnerPropertyType(propertyType, out _).ClrType;


        private PublishedPropertyType GetInnerPropertyType(IPublishedPropertyType propertyType, out FeatureFlaggedConfiguration config)
        {
            config = ConfigurationEditor.ConfigurationAs<FeatureFlaggedConfiguration>(propertyType.DataType.Configuration);
            var dataType = _dataTypeService.GetDataType(config.DataType);
            if (dataType == null)
            {
                throw new InvalidOperationException($"DataType configured for feature flagged data type {propertyType.DataType.Id}");
            }

            return new PublishedPropertyType(
                propertyType.ContentType,
                new PropertyType(_shortStringHelper, dataType, propertyType.Alias),
                _propertyValueConverters,
                _publishedModelFactory,
                _publishedContentTypeFactory);
        }
    }
}
