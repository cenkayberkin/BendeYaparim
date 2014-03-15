using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using BendeYaparim.Web.DAL;

namespace BendeYaparim.Web.Models
{
    public class BendeyaparimContext : DbContext, IDisposable
    {
        public DbSet<User> Users { get; set; }
        public DbSet<JobSeek> JobSeeks { get; set; }
        public DbSet<JobOffer> JobOffers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Message> Messages { get; set; }

        public BendeyaparimContext()
            : base("BendeyaparimDB3")
        {
            //System.Data.Entity.Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=.\SQLSERVERR2;Initial Catalog=BendeyaparimLastDB;Integrated Security=True");
            System.Data.Entity.Database.SetInitializer(new BendeyaparimDbInitializer());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            GC.SuppressFinalize(this);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobSeek>().Map(e =>
            {
                e.MapInheritedProperties();
                e.ToTable("JobSeeks");
            });
            modelBuilder.Entity<JobOffer>().Map(e =>
            {
                e.MapInheritedProperties();
                e.ToTable("JobOffers");
            });

            modelBuilder.Entity<User>()
              .HasMany<Role>(r => r.Roles)
              .WithMany(u => u.Users)
              .Map(m => m.ToTable("UserRoles")
                 .MapLeftKey("UserId")
                 .MapRightKey("RoleId"));

            modelBuilder.Entity<Message>()
               .HasRequired(a => a.From)
               .WithMany()
               .HasForeignKey(u => u.FromUserId);

            modelBuilder.Entity<Message>()
                        .HasRequired(a => a.To)
                        .WithMany()
                        .HasForeignKey(u => u.ToUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().Property(a => a.CreatedDate).HasColumnType("datetime2");
            modelBuilder.Entity<User>().Property(a => a.LastLockedOutDate).HasColumnType("datetime2");
            modelBuilder.Entity<User>().Property(a => a.LastModifiedDate).HasColumnType("datetime2");
            modelBuilder.Entity<User>().Property(a => a.LastLoginDate).HasColumnType("datetime2");
            modelBuilder.Entity<User>().Property(a => a.NewPasswordRequested).HasColumnType("datetime2");
            modelBuilder.Entity<User>().Property(a => a.NewEmailRequested).HasColumnType("datetime2");

            modelBuilder.Entity<JobSeek>().Property(a => a.CreateDate).HasColumnType("datetime2");
            modelBuilder.Entity<JobOffer>().Property(a => a.CreateDate).HasColumnType("datetime2");
            modelBuilder.Entity<Message>().Property(a => a.SentAt).HasColumnType("datetime2");

        }
    }



}