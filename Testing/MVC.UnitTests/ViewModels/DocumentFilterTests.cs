using MVC.ViewModels;
using System;
using Xunit;

namespace MVC.UnitTests.ViewModels
{
  public class DocumentFilterTests
  {
    [Fact]
    [Trait("Category", "DocumentFilter")]
    public void EmptyFilter_Returns_4_Slashes()
    {
      string expected = "----";
      DokumentFilter filter = new();
      string actual = filter.ToString();
      Assert.Equal(expected, actual);
    }

    [Fact]
    [Trait("Category", "DocumentFilter")]
    public void Four_Slashes_Is_EmptyFilter()
    {
      string filterString = "----";
      DokumentFilter filter = DokumentFilter.FromString(filterString);
      Assert.True(filter.IsEmpty());
    }

    [Fact]
    [Trait("Category", "DocumentFilter")]
    public void InvalidDate_Throws_FormatException()
    {
      string filterString = "-15.12.2015-15.13.2021--";
      Assert.Throws<FormatException>(() => DokumentFilter.FromString(filterString));      
    }

    [Trait("Category", "DocumentFilter")]
    [Theory]    
    [InlineData("1-15.12.2015-15.12.2021-300")]
    [InlineData("1-2-3-4-5-6")]
    public void Not_4_Slashes_IsEmptyFilter(string filterString)
    {      
      DokumentFilter filter = DokumentFilter.FromString(filterString);
      Assert.True(filter.IsEmpty());
    }
    
    [Trait("Category", "DocumentFilter")]
    [Theory]
    [InlineData("1-15.12.2015-07.05.2021-300-500", 1, 300.0, 500.0, "Irrelevant name", 15, 12, 2015, 7, 5, 2021)]
    [InlineData("1-15.12.2015-07.05.2021-25,5-2,5", 1, 25.5, 2.5, "Irrelevant name", 15, 12, 2015, 7, 5, 2021)]
    [InlineData("-15.12.2015-07.05.2021--", null, null, null, "Irrelevant name", 15, 12, 2015, 7, 5, 2021)]
    public void FilterString_Is_WellFormed(string expected, int? partnerId, 
      double? fromAmount, double? toAmount, 
      string partnerName, 
      int fromDate, int fromMonth, int fromYear,
      int toDate, int toMonth, int toYear
    )
    {
      DokumentFilter filter = new DokumentFilter();
      filter.IdPartnera = partnerId;
      filter.NazPartnera = partnerName;
      filter.IznosOd = (decimal?) fromAmount;
      filter.IznosDo = (decimal?) toAmount;
      
      filter.DatumOd = new DateTime(fromYear, fromMonth, fromDate);
      filter.DatumDo = new DateTime(toYear, toMonth, toDate);
      string filterString = filter.ToString();      
      Assert.Equal(expected, filterString);
    }
  }
}
