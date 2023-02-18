using System;
using System.Collections.Generic;

namespace URLEntryMVC.Entities;

public partial class UrlTbl
{
    public int Id { get; set; }

    public string? UrlLink { get; set; }
    public string? UrlLinkForCustomer { get; set; }

    public string? DomainLink { get; set; }

    public int? TotalClicks { get; set; }

    public int? CustomerIdFk { get; set; }

    public virtual CustomerTbl? CustomerIdFkNavigation { get; set; }
}
