using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Entities;

namespace URLEntryMVC.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<UrlTbl> UrlTbl { get; set; }
}

