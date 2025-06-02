using Authentication.Data.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data;

public partial class AuthDbContext : IdentityDbContext<AspNetUser>
{
    protected readonly IConfiguration _configuration;

    public AuthDbContext()
    {
        
    }

    public AuthDbContext(DbContextOptions<AuthDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<LoginSession> LoginSessions { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<IdentityUser>(entity =>
        //{
        //    entity.ToTable("AspNetUser");
        //    entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
        //        .IsUnique()
        //        .HasFilter("([NormalizedUserName] IS NOT NULL)");
        //});

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex1")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<LoginSession>(entity =>
        {
            entity.HasKey(e => e.SessionId);

            entity.HasIndex(e => e.UserId, "IX_LoginSessions_UserId").IsUnique();

            entity.Property(e => e.LoginFrom)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.SessionName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Validity)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.LoginSession).HasForeignKey<LoginSession>(d => d.UserId);
        });

        modelBuilder.Entity<UserMaster>(entity =>
        {
            entity.ToTable("UserMaster");

            entity.HasIndex(e => e.UserId, "IX_UserMaster_UserId").IsUnique();

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(d => d.User).WithOne(p => p.UserMaster).HasForeignKey<UserMaster>(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
