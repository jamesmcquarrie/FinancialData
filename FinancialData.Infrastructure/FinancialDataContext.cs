using FinancialData.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialData.Infrastructure;

public class FinancialDataContext : DbContext
{
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<MetaData> MetaData { get; set; }
    public DbSet<TimeSeries> TimeSeries { get; set; }

    public FinancialDataContext(DbContextOptions<FinancialDataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>()
            .HasOne(s => s.MetaData)
            .WithOne(m => m.Stock)
            .HasForeignKey<MetaData>(m => m.StockId)
            .IsRequired();

        modelBuilder.Entity<MetaData>()
            .Property(m => m.Symbol)
            .HasMaxLength(15);
        modelBuilder.Entity<MetaData>()
            .Property(m => m.Type)
            .HasMaxLength(30);
        modelBuilder.Entity<MetaData>()
            .Property(m => m.Currency)
            .HasMaxLength(15);
        modelBuilder.Entity<MetaData>()
            .Property(m => m.Exchange)
            .HasMaxLength(20);
        modelBuilder.Entity<MetaData>()
            .Property(m => m.ExchangeTimezone)
            .HasMaxLength(30);
        modelBuilder.Entity<MetaData>()
            .Property(m => m.Interval)
            .HasMaxLength(10);

        modelBuilder.Entity<MetaData>()
            .HasIndex(m => new { m.Symbol, m.Interval })
            .IsUnique();

        modelBuilder.Entity<TimeSeries>()
            .HasIndex(ts => new { ts.StockId, ts.Datetime })
            .IsUnique();
    }
}