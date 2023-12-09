using FinancialData.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialData.Infrastructure;

public class FinancialDataContext : DbContext
{
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Metadata> MetaData { get; set; }
    public DbSet<TimeSeries> TimeSeries { get; set; }

    public FinancialDataContext(DbContextOptions<FinancialDataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>()
            .HasOne(s => s.Metadata)
            .WithOne(m => m.Stock)
            .HasForeignKey<Metadata>(m => m.StockId)
            .IsRequired();

        modelBuilder.Entity<Metadata>()
            .Property(m => m.Symbol)
            .HasMaxLength(15);
        modelBuilder.Entity<Metadata>()
            .Property(m => m.Type)
            .HasMaxLength(30);
        modelBuilder.Entity<Metadata>()
            .Property(m => m.Currency)
            .HasMaxLength(15);
        modelBuilder.Entity<Metadata>()
            .Property(m => m.Exchange)
            .HasMaxLength(20);
        modelBuilder.Entity<Metadata>()
            .Property(m => m.ExchangeTimezone)
            .HasMaxLength(30);
        modelBuilder.Entity<Metadata>()
            .Property(m => m.Interval)
            .HasMaxLength(10);

        modelBuilder.Entity<Metadata>()
            .HasIndex(m => new { m.Symbol, m.Interval })
            .IsUnique();

        modelBuilder.Entity<TimeSeries>()
            .HasIndex(ts => new { ts.StockId, ts.Datetime })
            .IsUnique();
    }
}