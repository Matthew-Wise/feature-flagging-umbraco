namespace DemoSite.Components;

using Umbraco.Cms.Core.Models.PublishedContent;

public class PageHeaderViewModel
{
	public string? Name { get; set; }

	public string? Title { get; set; }

	public string? Subtitle { get; set; }

	public bool HasSubtitle => !string.IsNullOrWhiteSpace(Subtitle);

	public IPublishedContent? BackgroundImage { get; set; }

	public bool HasBackgroundImage => BackgroundImage != null;

	public string? AuthorName { get; set; }

	public bool HasAuthor => !string.IsNullOrWhiteSpace(AuthorName);

	public DateTime? ArticleDate { get; set; }

	public bool IsArticle => ArticleDate.HasValue;

}