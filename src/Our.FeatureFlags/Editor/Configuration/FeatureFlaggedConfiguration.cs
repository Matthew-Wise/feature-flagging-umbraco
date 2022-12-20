namespace Our.FeatureFlags.Editor.Configuration;

using System;
using System.Collections.Generic;
using Microsoft.FeatureManagement;
using Umbraco.Cms.Core.PropertyEditors;

public class FeatureFlaggedConfiguration
{
    public FeatureFlaggedConfiguration()
    {
        Requirement = RequirementType.All;
        Features = new List<string>();
    }

    [ConfigurationField(key: nameof(Requirement),
       name: nameof(Requirement),
       view: "radiobuttonlist",
       Description = "Controls whether 'All' or 'Any' feature in a list of features should be enabled to render the editor")]
    public RequirementType Requirement { get; set; }

    [ConfigurationField(key: nameof(Features),
      name: nameof(Features),
      view: "checkboxlist",
      Description = "Features which control if this editor should be rendered")]
    public List<string> Features { get; set; }

    [ConfigurationField(key: nameof(DataType),
      name: "Data Type",
      view: "treepicker",
      Description = "The data type to feature flag")]
    public Guid DataType { get; set; }
}