namespace Our.FeatureFlags.Filters.UmbracoBackOfficeUser;

using Microsoft.FeatureManagement;

internal sealed class UmbracoBackOfficeUserFilterSettings
{
    /// <inheritdoc cref="RequirementType"/>
    public RequirementType Match { get; set; } = RequirementType.Any;

    /// <summary>
    /// Groups to check the user had to enable the flag
    /// </summary>
    public MatchCondition Groups { get; set; } = new();

    /// <summary>
    /// Email addresses to check the user had to enable the flag
    /// </summary>
    public MatchCondition EmailAddresses { get; set; } = new();
}