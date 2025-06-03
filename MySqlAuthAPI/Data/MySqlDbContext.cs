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

        public virtual DbSet<Aspnetrole> Aspnetroles { get; set; }

        public virtual DbSet<Aspnetroleclaim> Aspnetroleclaims { get; set; }

        public virtual DbSet<Aspnetuser> Aspnetusers { get; set; }

        public virtual DbSet<Aspnetuserclaim> Aspnetuserclaims { get; set; }

        public virtual DbSet<Aspnetuserlogin> Aspnetuserlogins { get; set; }

        public virtual DbSet<Aspnetusertoken> Aspnetusertokens { get; set; }

        public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

        public virtual DbSet<UserMaster> UserMasters { get; set; }

        public virtual DbSet<LoginSession> LoginSessions { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseMySQL(_configuration.GetConnectionString("Database"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Aspnetrole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("aspnetroles");

                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

                entity.Property(e => e.ConcurrencyStamp).HasDefaultValueSql("'NULL'");
                entity.Property(e => e.Name)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("'NULL'");
                entity.Property(e => e.NormalizedName)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Aspnetroleclaim>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("aspnetroleclaims");

                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.Id).HasColumnType("int(11)");
                entity.Property(e => e.ClaimType).HasDefaultValueSql("'NULL'");
                entity.Property(e => e.ClaimValue).HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.Role).WithMany(p => p.Aspnetroleclaims)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_AspNetRoleClaims_AspNetRoles_RoleId");
            });

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

                entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "Aspnetuserrole",
                        r => r.HasOne<Aspnetrole>().WithMany()
                            .HasForeignKey("RoleId")
                            .HasConstraintName("FK_AspNetUserRoles_AspNetRoles_RoleId"),
                        l => l.HasOne<Aspnetuser>().WithMany()
                            .HasForeignKey("UserId")
                            .HasConstraintName("FK_AspNetUserRoles_AspNetUsers_UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PRIMARY");
                            j.ToTable("aspnetuserroles");
                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<Aspnetuserclaim>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("aspnetuserclaims");

                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");
                entity.Property(e => e.ClaimType).HasDefaultValueSql("'NULL'");
                entity.Property(e => e.ClaimValue).HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserclaims)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_AspNetUserClaims_AspNetUsers_UserId");
            });

            modelBuilder.Entity<Aspnetuserlogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PRIMARY");

                entity.ToTable("aspnetuserlogins");

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.ProviderDisplayName).HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserlogins)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_AspNetUserLogins_AspNetUsers_UserId");
            });

            modelBuilder.Entity<Aspnetusertoken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PRIMARY");

                entity.ToTable("aspnetusertokens");

                entity.Property(e => e.Value).HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.User).WithMany(p => p.Aspnetusertokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_AspNetUserTokens_AspNetUsers_UserId");
            });

            modelBuilder.Entity<Efmigrationshistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

                entity.ToTable("__efmigrationshistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);
                entity.Property(e => e.ProductVersion).HasMaxLength(32);
            });

            modelBuilder.Entity<LoginSession>(entity =>
            {
                entity.HasKey(e => e.SessionId).HasName("PRIMARY");

                entity.ToTable("LoginSessions");
            });

            modelBuilder.Entity<UserMaster>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("UserMaster");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
