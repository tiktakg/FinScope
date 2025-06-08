using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FinScope.Enitys;

namespace FinScope.Context;

public partial class FinScopeDbContext : DbContext
{
    public FinScopeDbContext()
    {
    }

    public FinScopeDbContext(DbContextOptions<FinScopeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MarketIndex> MarketIndexes { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<PortfolioAsset> PortfolioAssets { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SectorAllocation> SectorAllocations { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=TikTak;Initial Catalog=FinScopeDB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MarketIndex>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MarketIn__3214EC0789EEF13D");

            entity.Property(e => e.Change).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ChangePercent).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Value).HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3214EC0763518231");

            entity.Property(e => e.Source).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<PortfolioAsset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Portfoli__3214EC077DBB2B9D");

            entity.Property(e => e.AvgPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CurrentPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Profit)
                .HasComputedColumnSql("(([CurrentPrice]-[AvgPrice])*[Quantity])", true)
                .HasColumnType("decimal(38, 8)");
            entity.Property(e => e.ProfitPercent)
                .HasComputedColumnSql("(case when [AvgPrice]>(0) then (([CurrentPrice]-[AvgPrice])/[AvgPrice])*(100) else (0) end)", true)
                .HasColumnType("decimal(38, 15)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Value)
                .HasComputedColumnSql("([Quantity]*[CurrentPrice])", true)
                .HasColumnType("decimal(37, 8)");

            entity.HasOne(d => d.Stock).WithMany(p => p.PortfolioAssets)
                .HasForeignKey(d => d.StockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Portfolio__Stock__46E78A0C");

            entity.HasOne(d => d.User).WithMany(p => p.PortfolioAssets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Portfolio__UserI__45F365D3");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07FC036A12");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F62920AF23").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<SectorAllocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SectorAl__3214EC07D59C16D9");

            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Sector).HasMaxLength(100);
            entity.Property(e => e.Value).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.User).WithMany(p => p.SectorAllocations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SectorAll__UserI__52593CB8");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Stocks__3214EC074474C8F0");

            entity.HasIndex(e => e.Symbol, "UQ__Stocks__B7CC3F016CCC3830").IsUnique();

            entity.Property(e => e.Change).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ChangePercent).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Sector).HasMaxLength(100);
            entity.Property(e => e.Symbol).HasMaxLength(10);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC07AC2FC08D");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Total)
                .HasComputedColumnSql("([Quantity]*[Price])", true)
                .HasColumnType("decimal(29, 4)");
            entity.Property(e => e.Type).HasMaxLength(10);

            entity.HasOne(d => d.Stock).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.StockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Stock__4BAC3F29");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__UserI__4AB81AF0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC076D3665F9");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4EA842FF7").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053425233AB2").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__UserRoles__RoleI__403A8C7D"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserRoles__UserI__3F466844"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760ADA8C93321");
                        j.ToTable("UserRoles");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
