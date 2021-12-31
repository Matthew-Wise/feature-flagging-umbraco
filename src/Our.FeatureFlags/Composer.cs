using Our.FeatureFlags.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.FeatureFlags
{
    internal class Composer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddOurFeatureFlags();
        }
    }
}
