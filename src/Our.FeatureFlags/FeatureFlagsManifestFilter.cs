namespace Our.FeatureFlags;

using Umbraco.Cms.Core.Manifest;

internal class FeatureFlagsManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests) => manifests.Add(new PackageManifest
    {
        PackageName = "Our.FeatureFlags",
        Version = typeof(FeatureFlagsManifestFilter).Assembly.GetName().Version?.ToString(3) ?? string.Empty,
    });
}