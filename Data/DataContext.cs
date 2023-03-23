using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Entities;
using URLEntryMVC.Extensions;

namespace URLEntryMVC.Data;

public partial class DataContext : IdentityDbContext<ApplicationUserExtension>
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<CustomerTbl> CustomerTbls { get; set; }
    public virtual DbSet<PointCategory> PointCategories { get; set; }

    public virtual DbSet<PointEmail> PointEmails { get; set; }

    public virtual DbSet<UrlTbl> UrlTbls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CustomerTbl>(entity =>
        {
            entity.ToTable("CustomerTbl");
        });

        modelBuilder.Entity<PointCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("PointCategory");
        });

        modelBuilder.Entity<PointEmail>(entity =>
        {
            entity.HasKey(e => e.EmailId);

            entity.ToTable("PointEmail");

            entity.Property(e => e.EmailId).ValueGeneratedOnAdd();
            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.PointIdFk).HasColumnName("PointIdFK");

            entity.HasOne(d => d.PointIdFkNavigation).WithMany(p => p.PointEmails)
                .HasForeignKey(d => d.PointIdFk)
                .HasConstraintName("FK_PointEmail_UrlTbl");
        });

        modelBuilder.Entity<UrlTbl>(entity =>
        {
            entity.ToTable("UrlTbl");

            entity.Property(e => e.CustomerIdFk).HasColumnName("CustomerIdFK");

            entity.HasOne(d => d.CustomerIdFkNavigation).WithMany(p => p.UrlTbls)
                .HasForeignKey(d => d.CustomerIdFk)
                .HasConstraintName("FK_UrlTbl_CustomerTbl");

            entity.HasOne(d => d.PointCategoryIdFkNavigation).WithMany(p => p.UrlTbls)
                .HasForeignKey(d => d.PointCategoryIdFk)
                .HasConstraintName("FK_UrlTbl_PointCategory");
        });
    }

}
