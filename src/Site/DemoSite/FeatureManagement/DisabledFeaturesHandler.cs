namespace DemoSite.FeatureManagement;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.FeatureManagement.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DisabledFeaturesHandler : IDisabledFeaturesHandler
{
	public Task HandleDisabledFeatures(IEnumerable<string> features, ActionExecutingContext context)
	{
		context.Result = new ForbidResult(); // generate a 403
		return Task.CompletedTask;
	}
}
