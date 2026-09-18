namespace Central.Common.Lib.Types;

public abstract class PagedQueryBase : IPagedQuery
{
  public int Page { get; set; }
  public int Results { get; set; }
  public string OrderBy { get; set; } = String.Empty;
  public string SortOrder { get; set; } = String.Empty;
}
