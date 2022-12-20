namespace Our.FeatureFlags.Filters.UmbracoDomain;

using Microsoft.FeatureManagement;

public class UmbracoDomainFilterSettings
{
	public RequirementType RequirementType { get; set; } = RequirementType.Any;

	public string[] Domains { get; set; } = Array.Empty<string>();
}