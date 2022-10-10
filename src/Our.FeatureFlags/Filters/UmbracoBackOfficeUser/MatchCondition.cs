namespace Our.FeatureFlags.Filters.UmbracoBackOfficeUser;

using Microsoft.FeatureManagement;

internal sealed class MatchCondition
{

    /// <inheritdoc cref="RequirementType"/>
    public RequirementType Match { get; set; } = RequirementType.Any;

    public string[] Values { get; set; } = Array.Empty<string>();

    public bool IsMatch(IEnumerable<string> valuesToMatch)
    {
        if(Values.Any() == false)
        {
            return true;
        }

        return Match switch
        {
            RequirementType.All => Values.All(v => valuesToMatch.Contains(v, StringComparer.InvariantCultureIgnoreCase)),
            RequirementType.Any => Values.Any(v => valuesToMatch.Contains(v, StringComparer.InvariantCultureIgnoreCase)),
            _ => false
        };
    }

    public bool IsMatch(string email) => IsMatch(new[] { email });
}