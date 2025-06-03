using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using MySqlAuthAPI.Data.Entities;

namespace MySqlAuthAPI.Data
{
    public partial class MySqlDbContext : IdentityDbContext<Aspnetuser>
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Aspnetuser> Aspnetusers { get; set; }

        public virtual DbSet<UserMaster> UserMasters { get; set; }

        public virtual DbSet<LoginSession> LoginSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Aspnetuser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("aspnetusers");

                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

                entity.Property(e => e.AccessFailedCount).HasColumnType("int(11)");
                entity.Property(e => e.ConcurrencyStamp).HasDefaultValueSql("'NULL'");
                entity.Property(e => e.Email)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("'NULL'");
                entity.Property(e => e.LockoutEnd)
                    .HasMaxLength(6)
                    .HasDefaultValueSql("'NULL'");
                entity.Property(e => e.NormalizedEmail)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("'NULL'");
                entity.Property(e => e.NormalizedUserName)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("'NULL'");
                entity.Property(e => e.PasswordHash).HasDefaultValueSql("'NULL'");
                entity.Property(e => e.PhoneNumber).HasDefaultValueSql("'NULL'");
                entity.Property(e => e.SecurityStamp).HasDefaultValueSql("'NULL'");
                entity.Property(e => e.UserName)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("aspnetroles");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("aspnetuserroles");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("aspnetuserclaims");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("aspnetroleclaims");
            });

            modelBuilder.Entity<LoginSession>(entity =>
            {
                entity.HasKey(e => e.SessionId).HasName("PRIMARY");

                entity.ToTable("loginsessions");
            });

            modelBuilder.Entity<UserMaster>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("usermaster");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
