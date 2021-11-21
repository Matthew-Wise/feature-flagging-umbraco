namespace FeatureFlags.Core.Extensions
{
    using FeatureFlags.Core.NotificationHandlers;
    using Umbraco.Cms.Core.DependencyInjection;
    using Umbraco.Cms.Core.Notifications;

    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddFeatureFlagged(this IUmbracoBuilder builder)
        {
            builder.AddNotificationAsyncHandler<SendingContentNotification, FeatureContentSendingNotificationHandler>()
            .AddNotificationAsyncHandler<ContentPublishingNotification, FeatureContentPublishingNotificationHandler>();
            return builder;
        }
    }
}
