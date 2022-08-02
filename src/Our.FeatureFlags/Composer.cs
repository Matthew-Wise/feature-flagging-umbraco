namespace Our.FeatureFlags;

using Our.FeatureFlags.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

internal class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddOurFeatureFlags();

        builder.ManifestFilters().Append<FeatureFlagsManifestFilter>();
    }
}
