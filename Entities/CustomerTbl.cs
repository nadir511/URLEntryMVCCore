using System;
using System.Collections.Generic;

namespace URLEntryMVC.Entities;

public partial class CustomerTbl
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? Address { get; set; }

    public string? ContactNumber { get; set; }

    public byte[] CustomerPic { get; set; } = null!;

    public virtual ICollection<UrlTbl> UrlTbls { get; } = new List<UrlTbl>();
}
