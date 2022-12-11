using MVC_EN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace MVC_EN.TagHelpers;

/// <summary>
/// Tag helper use to create pagelink using element pager
/// The elemebt will be rendered as div
/// <example>
/// Usage example
/// ```
/// <pager page-info="@Model.PagingInfo" page-action="Index" page-title="Enter the page" class="float-end">
/// </pager>
/// ```
/// In order to use it, add `@addTagHelper *, MVC-EN` in *_ViewImports.cshtml* 
/// and include *gotopage.js*
/// Note, MVC-EN is project (assembly) name, and MVC_EN is namespace
/// </example>
/// </summary>  
[HtmlTargetElement(Attributes="page-info")]
public class PagerTagHelper : TagHelper
{

  private readonly IUrlHelperFactory urlHelperFactory;  
  private readonly AppSettings appData;   
  public PagerTagHelper(IUrlHelperFactory helperFactory, IOptionsSnapshot<AppSettings> options)
  {      
    urlHelperFactory = helperFactory;
    appData = options.Value;
  }

  [ViewContext]
  [HtmlAttributeNotBound]
  public ViewContext ViewContext { get; set; }

  /// <summary>
  /// Serialized string containing information about current page, and total number of pages
  /// </summary>
  public PagingInfo PageInfo { get; set; }

  /// <summary>
  /// Serialized string containing current filter
  /// </summary>
  public IPageFilter PageFilter { get; set; }

  /// <summary>
  /// Action for which link should be created
  /// </summary>
  public string PageAction { get; set; }

  /// <summary>
  /// Tooltip for the textbox that is used to enter desired page
  /// </summary>
  public string PageTitle { get; set; }

  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
    output.TagName = "nav";      
    int offset = appData.PageOffset;      
    TagBuilder paginationList = new TagBuilder("ul");
    paginationList.AddCssClass("pagination");      

    if (PageInfo.CurrentPage - offset > 1) //create list item for the first page
    {
      var tag = BuildListItemForPage(1, "1..");
      paginationList.InnerHtml.AppendHtml(tag);
    }

    for (int i = Math.Max(1, PageInfo.CurrentPage - offset);
             i <= Math.Min(PageInfo.TotalPages, PageInfo.CurrentPage + offset);
             i++)
    {
      var tag = i == PageInfo.CurrentPage ? BuildListItemForCurrentPage(i) : BuildListItemForPage(i);        
      paginationList.InnerHtml.AppendHtml(tag);
    }

    if (PageInfo.CurrentPage + offset < PageInfo.TotalPages) //create list item for the last page
    {
      var tag = BuildListItemForPage(PageInfo.TotalPages, ".. " + PageInfo.TotalPages);
      paginationList.InnerHtml.AppendHtml(tag);
    }

    output.Content.AppendHtml(paginationList);     
  }

  /// <summary>
  /// Create tag for the i-th page with *i* as text
  /// <seealso cref="BuildListItemForPage(int, string)"/>
  /// </summary>
  /// <param name="i">page number</param>
  /// <returns>TagBuilder with the link</returns>
  private TagBuilder BuildListItemForPage(int i)
  {
    return BuildListItemForPage(i, i.ToString());
  }

  /// <summary>
  ///  Create tag for the i-th page with *text* as text
  /// </summary>
  /// <param name="i">page number</param>
  /// <param name="text">text to display</param>
  /// <returns>TagBuilder with the link</returns>
  private TagBuilder BuildListItemForPage(int i, string text)
  {
    IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);      

    TagBuilder a = new TagBuilder("a");
    a.InnerHtml.Append(text);
    a.Attributes["href"] = urlHelper.Action(PageAction, new
    {
      page = i,
      sort = PageInfo.Sort,
      ascending = PageInfo.Ascending,
      filter = PageFilter?.ToString()
    });
    a.AddCssClass("page-link");

    TagBuilder li = new TagBuilder("li");
    li.AddCssClass("page-item");
    li.InnerHtml.AppendHtml(a);
    return li;
  }

  /// <summary>
  /// Create textbox to show current page, and to allow entering desired page
  /// </summary>
  /// <param name="page">Current page</param>
  /// <returns></returns>
  private TagBuilder BuildListItemForCurrentPage(int page)
  {
    IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
    TagBuilder input = new TagBuilder("input");     
    input.Attributes["type"] = "text";
    input.Attributes["value"] = page.ToString();
    input.Attributes["data-current"] = page.ToString();
    input.Attributes["data-min"] = "1";
    input.Attributes["data-max"] = PageInfo.TotalPages.ToString();
    input.Attributes["data-url"] = urlHelper.Action(PageAction, new
    {
      page = -1,
      sort = PageInfo.Sort,
      ascending = PageInfo.Ascending,
      filter = PageFilter?.ToString()
    });
    input.AddCssClass("page-link");
    input.AddCssClass("pagebox");//so that can be styled at the application level

    if (!string.IsNullOrWhiteSpace(PageTitle))
    {
      input.Attributes["title"] = PageTitle;
    }

    TagBuilder li = new TagBuilder("li");
    li.AddCssClass("page-item active");
    li.InnerHtml.AppendHtml(input);

    return li;
  }

}
