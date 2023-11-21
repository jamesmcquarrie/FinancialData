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
            .HasForeignKey<Metadata>();

        //modelBuilder.Entity<MetaData>()
        //    .Property(m => m.Symbol)
        //    .HasMaxLength(10);
        //modelBuilder.Entity<MetaData>()
        //    .Property(m => m.Type)
        //    .HasMaxLength(30);
        //modelBuilder.Entity<MetaData>()
        //    .Property(m => m.Currency)
        //    .HasMaxLength(10);
        //modelBuilder.Entity<MetaData>()
        //    .Property(m => m.Exchange)
        //    .HasMaxLength(20);
        //modelBuilder.Entity<MetaData>()
        //    .Property(m => m.ExchangeTimeZone)
        //    .HasMaxLength(30);
        //modelBuilder.Entity<MetaData>()
        //    .Property(m => m.Interval)
        //    .HasMaxLength(10);

        //modelBuilder.Entity<TimeSeries>()
        //    .Property(t => t.DateTime)
        //    .HasMaxLength(15);
        //modelBuilder.Entity<TimeSeries>()
        //    .Property(t => t.High)
        //    .HasMaxLength(15);
        //modelBuilder.Entity<TimeSeries>()
        //    .Property(t => t.Low)
        //    .HasMaxLength(15);
        //modelBuilder.Entity<TimeSeries>()
        //    .Property(t => t.Open)
        //    .HasMaxLength(15);
        //modelBuilder.Entity<TimeSeries>()
        //    .Property(t => t.Close)
        //    .HasMaxLength(15);
        //modelBuilder.Entity<TimeSeries>()
        //    .Property(t => t.Volume)
        //    .HasMaxLength(15);
    }
}