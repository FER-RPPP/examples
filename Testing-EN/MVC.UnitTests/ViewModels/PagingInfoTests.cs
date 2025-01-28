using MVC_EN.ViewModels;

namespace MVC.UnitTests.ViewModels
{
  public class PagingInfoTests
  {
    [Fact]
    [Trait("Category", "PagingInfo")]
    public void NonEmptyItems_Takes_AtLeast_OnePage()
    {
      PagingInfo pagingInfo = new()
      {
        ItemsPerPage = 10,
        TotalItems = 7
      };
      Assert.True(pagingInfo.TotalPages > 0);
    }

    [Fact]
    [Trait("Category", "PagingInfo")]
    public void ZeroItemsPerPage_Throws_DivideByZeroException()
    {
      PagingInfo pagingInfo = new();
      Assert.Throws<DivideByZeroException>(() => pagingInfo.TotalPages);      
    }

    [Trait("Category", "PagingInfo")]
    [Theory]    
    [InlineData(13, 5, 3)]
    [InlineData(14, 5, 3)]
    [InlineData(15, 5, 3)]
    [InlineData(16, 5, 4)]   
    public void RoundingTests(int totalItems, int itemsPerPage, int expectedPages)
    {
      PagingInfo pagingInfo = new()
      {
        TotalItems = totalItems,
        ItemsPerPage = itemsPerPage
      };
      Assert.Equal(expectedPages, pagingInfo.TotalPages); 
    }
  }
}
