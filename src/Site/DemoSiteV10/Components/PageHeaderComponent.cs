namespace DemoSite.Components;

using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

[ViewComponent(Name = "PageHeader")]
public class PageHeaderComponent : ViewComponent
{
	public IViewComponentResult Invoke(IPublishedContent content)
	{
		var model = new PageHeaderViewModel
		{
			Name = content.Name
		};

		if (content is IHeaderControls header)
		{
			model.Title = header.Title;
			model.Subtitle = header.Subtitle;
		}

		if(content is IMainImageControls mainImage)
		{
			model.BackgroundImage = mainImage.MainImage;
		}

		if (content is Article article)
		{
			model.ArticleDate = article.ArticleDate;
		}

		return View(model);
	}
}
