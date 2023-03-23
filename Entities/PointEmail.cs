namespace URLEntryMVC.Entities;

public partial class PointEmail
{
    public int EmailId { get; set; }

    public string? Email { get; set; }

    public int? PointIdFk { get; set; }

    public virtual UrlTbl? PointIdFkNavigation { get; set; }
}
