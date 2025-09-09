using LocationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocationTracker.Infrastructure.Data;
public class TrackingDbContext : DbContext
{
    public TrackingDbContext(DbContextOptions<TrackingDbContext> options) : base(options) { }
    
    public DbSet<LocationRecord> LocationRecords { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocationRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DeviceId);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}