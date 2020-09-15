using Microsoft.EntityFrameworkCore;

namespace GuessTheNumber.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public virtual DbSet<NumbersData> NumbersDataRecords { get; set; }
        public virtual DbSet<GuessedNumber> GuessedNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NumbersData>(entity =>
            {
                entity.Property(e => e.Id);
                entity.Property(e => e.PredefinedNumber);
                entity.Property(e => e.UsedAttempts);
            });

            modelBuilder.Entity<GuessedNumber>(entity =>
            {
                entity.Property(e => e.Id);
                entity.Property(e => e.Number);
                entity.Property(e => e.NumbersDataId);

                entity.HasOne(d => d.NumbersData)
                    .WithMany(p => p.GuessedNumbers)
                    .HasForeignKey(d => d.NumbersDataId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Sites_Companies");
            });
        }
    }
}

    