namespace DemoSite.FeatureManagement.SendingContentNotification.Filters;

using DemoSite.FeatureManagement.SendingContentNotification;
using Microsoft.FeatureManagement;
using System.Threading.Tasks;

[FilterAlias("SendingContentNotification")]
public class SendingContentNotificationFilter : IContextualFeatureFilter<SendingContentNotificationContext>
{
	public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext featureFilterContext, 
		SendingContentNotificationContext appContext)
	{
		
		var settings = featureFilterContext.Parameters.Get<Settings>();
		var result = settings.DocumentTypes.Contains(appContext.Notification.Content.ContentTypeAlias);

		return Task.FromResult(result);
	}

	public class Settings
	{
		public string[] DocumentTypes { get; set; } = Array.Empty<string>();
	}
}
