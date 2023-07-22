using System;
using System.Collections.Generic;

namespace URLEntryMVC.Entities;

public partial class UrlTbl
{
    public int Id { get; set; }

    public string? UrlLink { get; set; }

    public string? DomainLink { get; set; }

    public int? TotalClicks { get; set; }

    public int? CustomerIdFk { get; set; }

    public string? CustomerPointName { get; set; }
    public string? ManagementName { get; set; }

    public string? CustomerNotes { get; set; }

    public int? PointCategoryIdFk { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }
    public bool ? SaveInLibrary { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime? UpdationDate { get; set; }

    public virtual CustomerTbl? CustomerIdFkNavigation { get; set; }

    public virtual PointCategory? PointCategoryIdFkNavigation { get; set; }

    public virtual ICollection<PointEmail> PointEmails { get; } = new List<PointEmail>();
}
