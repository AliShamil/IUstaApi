using IUstaApi.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IUstaApi.Data
{
    public class UstaDbContext : IdentityDbContext<AppUser>
    {
        public UstaDbContext(DbContextOptions options) : base(options){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }
        //    protected override void OnModelCreating(ModelBuilder modelBuilder)
        //    {
        //        modelBuilder.Entity<Rating>()
        //.HasOne(r => r.Worker)
        //.WithMany(w => w.Ratings)
        //.HasForeignKey(r => r.WorkerId);

        //        modelBuilder.Entity<Rating>()
        //            .HasOne(r => r.Customer)
        //            .WithMany(c => c.Ratings)
        //            .HasForeignKey(r => r.CustomerId);
        //    }


        public DbSet<Category> Categories { get; set; }
        public DbSet<WorkerCategory> WorkerCategories { get; set; }
        public DbSet<WorkRequest> WorkRequests { get; set; }
    }
}
