namespace URLEntryMVC.Entities;

public partial class PointCategory
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<UrlTbl> UrlTbls { get; } = new List<UrlTbl>();
}
