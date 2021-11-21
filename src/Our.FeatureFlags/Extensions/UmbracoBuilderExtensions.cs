namespace Our.FeatureFlags.Extensions
{
    using Our.FeatureFlags.NotificationHandlers;
    using Umbraco.Cms.Core.DependencyInjection;
    using Umbraco.Cms.Core.Notifications;

    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddOurFeatureFlags(this IUmbracoBuilder builder)
        {
            //Umbraco's helper method checks to see if NotificationAsyncHandler is already registered if so skips it.
            builder.AddNotificationAsyncHandler<SendingContentNotification, FeatureContentSendingNotificationHandler>()
            .AddNotificationAsyncHandler<ContentPublishingNotification, FeatureContentPublishingNotificationHandler>();
            return builder;
        }
    }
}
