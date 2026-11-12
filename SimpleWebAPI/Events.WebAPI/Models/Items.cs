namespace Events.WebAPI.Models;

/// <summary>
/// Contains requested items (based on filter, paging and sorting criteria),
/// and the number of total items satisfying the filter
/// (or count of all items if no filter is present)
/// </summary>
/// <typeparam name="T"></typeparam>
public class Items<T>
{
  public List<T>? Data { get; set; }
  public int Count { get; set; }
}
