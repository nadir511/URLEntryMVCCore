using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Entities;

namespace URLEntryMVC.Data;

public partial class DataContext : IdentityDbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    

    //public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    //public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    //public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    //public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    //public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    //public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
    //public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
    public virtual DbSet<CustomerTbl> CustomerTbls { get; set; }

    public virtual DbSet<UrlTbl> UrlTbls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.Entity<AspNetRole>(entity =>
        //{
        //    entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
        //        .IsUnique()
        //        .HasFilter("([NormalizedName] IS NOT NULL)");

        //    entity.Property(e => e.Name).HasMaxLength(256);
        //    entity.Property(e => e.NormalizedName).HasMaxLength(256);
        //});

        //modelBuilder.Entity<AspNetRoleClaim>(entity =>
        //{
        //    entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

        //    entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        //});

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.ToTable("AspNetUser");
            entity.HasNoKey();
            //entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            //entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
            //    .IsUnique()
            //    .HasFilter("([NormalizedUserName] IS NOT NULL)");

            //entity.Property(e => e.Email).HasMaxLength(256);
            //entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            //entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            //entity.Property(e => e.UserName).HasMaxLength(256);

            //entity.HasMany(d => d.Roles).WithMany(p => p.Users)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "AspNetUserRole",
            //        r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
            //        l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
            //        j =>
            //        {
            //            j.HasKey("UserId", "RoleId");
            //            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
            //        });
        });

        //modelBuilder.Entity<AspNetUserClaim>(entity =>
        //{
        //    entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

        //    entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        //});

        //modelBuilder.Entity<AspNetUserLogin>(entity =>
        //{
        //    entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

        //    entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

        //    entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        //});

        //modelBuilder.Entity<AspNetUserToken>(entity =>
        //{
        //    entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

        //    entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        //});
        //modelBuilder.Entity<AspNetUserRole>(entity =>
        //{
        //    entity.HasKey(e => new { e.UserId, e.RoleId });
        //    entity.HasOne(d => d.Role).WithMany(p => p.FK_AspNetRoleUser).HasForeignKey(d => d.RoleId);
        //    entity.HasOne(d => d.User).WithMany(p => p.FK_AspNetUserRole).HasForeignKey(d => d.UserId);
        //});
        modelBuilder.Entity<CustomerTbl>(entity =>
        {
            entity.ToTable("CustomerTbl");
        });

        modelBuilder.Entity<UrlTbl>(entity =>
        {
            entity.ToTable("UrlTbl");

            entity.Property(e => e.CustomerIdFk).HasColumnName("CustomerIdFK");

            entity.HasOne(d => d.CustomerIdFkNavigation).WithMany(p => p.UrlTbls)
                .HasForeignKey(d => d.CustomerIdFk)
                .HasConstraintName("FK_UrlTbl_CustomerTbl");
        });
        //OnModelCreatingPartial(modelBuilder);
    }

}
