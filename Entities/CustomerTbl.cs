﻿using System;
using System.Collections.Generic;

namespace URLEntryMVC.Entities;

public partial class CustomerTbl
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? Address { get; set; }

    public string? ContactNumber { get; set; }
    public string? CustomerEmail { get; set; }
    public byte[] CustomerPic { get; set; } = null!;
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Twitter { get; set; }
    public string? LinkedIn { get; set; }
    public string? TikTok { get; set; }
    public string? Youtube { get; set; }
    public string? Snapchat { get; set; }

    public virtual ICollection<UrlTbl> UrlTbls { get; } = new List<UrlTbl>();
}
