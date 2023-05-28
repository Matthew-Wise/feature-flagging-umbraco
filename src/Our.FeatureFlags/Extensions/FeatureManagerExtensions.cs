namespace Our.FeatureFlags.Extensions;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.FeatureManagement;

public static class FeatureManagerExtensions
{
	//TODO: This FAILS when using contextual flags!
    public static async Task<IList<string>> GetEnabledFeatures(this IFeatureManager featureManager)
    {
        var enabledFeatures = new List<string>();

        await foreach (var featureName in featureManager.GetFeatureNamesAsync())
        {
			try
			{
				if (await featureManager.IsEnabledAsync(featureName))
				{
					enabledFeatures.Add(featureName);
				}
			}
			catch { }
        }

        return enabledFeatures;
    }
}