namespace DemoSite.FeatureManagement.SendingContentNotification;

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Notifications;

public class SendingContentNotificationContext : ISendingContentNotificationContext
{

	public SendingContentNotificationContext(SendingContentNotification notification, IUser? user)
	{
		Notification = notification;
		User = user;
	}

	public SendingContentNotification Notification { get; set; }
	public IUser? User { get; set; }
}

public interface ISendingContentNotificationContext
{
	SendingContentNotification Notification { get; set; }
	IUser? User { get; set; }
}
