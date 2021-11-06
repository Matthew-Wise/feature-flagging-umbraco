namespace FeatureFlags.Core.Editors.FeatureFlagged
{
    using System.Collections.Generic;

    using Microsoft.FeatureManagement;

    public class FeatureFlaggedConfiguration
    {
        public RequirementType Requirement { get; set; }

        public List<string> Features { get; set; }

        public int DataType { get; set; }
    }
}