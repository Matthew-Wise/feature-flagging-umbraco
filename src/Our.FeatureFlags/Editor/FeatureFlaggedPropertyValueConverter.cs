namespace Our.FeatureFlags.Editor;

using Microsoft.FeatureManagement;
using Our.FeatureFlags.Editor.Configuration;
using Our.FeatureFlags.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

public class FeatureFlaggedPropertyValueConverter : PropertyValueConverterBase
{
    private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;
    private readonly IDataTypeService _dataTypeService;
    private readonly IFeatureManager _featureManager;

    public FeatureFlaggedPropertyValueConverter(
        IPublishedContentTypeFactory publishedContentTypeFactory,
        IDataTypeService dataTypeService,
        IFeatureManager featureManager)
    {
        _publishedContentTypeFactory = publishedContentTypeFactory;
        _dataTypeService = dataTypeService;
        _featureManager = featureManager;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType)
        => propertyType.EditorAlias.InvariantEquals(FeatureFlaggedEditor.AliasValue);

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
    {
        var innerProperty = GetInnerPropertyType(propertyType, out var config);
        var enabledFeatures = Task.Run(async () => await _featureManager.GetEnabledFeatures()).GetAwaiter().GetResult();
        var propertyEnabled = config?.Requirement switch
        {
            RequirementType.All => enabledFeatures.ContainsAll(config.Features),
            RequirementType.Any => enabledFeatures.ContainsAny(config.Features),
            _ => throw new InvalidOperationException($"RequirementType for property {propertyType.Alias} was not handled"),
        };

        return innerProperty.ConvertInterToObject(owner, referenceCacheLevel, propertyEnabled ? inter : null, preview);
    }

    public override object? ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
        => GetInnerPropertyType(propertyType, out _).ConvertInterToXPath(owner, referenceCacheLevel, inter, preview);

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
        => GetInnerPropertyType(propertyType, out _).ConvertSourceToInter(owner, source, preview);

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        => GetInnerPropertyType(propertyType, out _).CacheLevel;

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        => GetInnerPropertyType(propertyType, out _).ModelClrType;

    private IPublishedPropertyType GetInnerPropertyType(IPublishedPropertyType propertyType, out FeatureFlaggedConfiguration? config)
    {
        config = ConfigurationEditor.ConfigurationAs<FeatureFlaggedConfiguration>(propertyType.DataType.Configuration);

        if (config != null)
        {
            var dataType = _dataTypeService.GetDataType(config.DataType);
            if (dataType != null && propertyType.ContentType != null)
            {
                return _publishedContentTypeFactory.CreatePropertyType(
                    propertyType.ContentType,
                    propertyType.Alias,
                    dataType.Id,
                    ContentVariation.Nothing);
            }
        }

        throw new InvalidOperationException($"Property not configured for feature flagged data type: {propertyType.DataType.Id}");
    }
}
