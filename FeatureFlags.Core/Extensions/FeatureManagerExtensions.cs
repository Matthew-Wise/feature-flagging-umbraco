namespace FeatureFlags.Core.Extensions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.FeatureManagement;

    public static class FeatureManagerExtensions
    {
        public static async Task<IList<string>> GetEnabledFeatures(this IFeatureManager featureManager)
        {
            var enabledFeatures = new List<string>();

            await foreach (var featureName in featureManager.GetFeatureNamesAsync())
            {
                if (await featureManager.IsEnabledAsync(featureName))
                {
                    enabledFeatures.Add(featureName);
                }
            }

            return enabledFeatures;
        }
    }
}