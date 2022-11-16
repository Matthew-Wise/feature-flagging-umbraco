using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers;

using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.FeatureManagement.Mvc;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

[FeatureGate(Features.BetaUser)]
public class BetaTemplateController : RenderController
{
	public BetaTemplateController(ILogger<BetaTemplateController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine, umbracoContextAccessor)
	{
	}
	
	[FeatureGate(Features.BetaUser)]
	public IActionResult BetaTemplate(BetaTemplate model)
	{
		return View(model);
	}
}