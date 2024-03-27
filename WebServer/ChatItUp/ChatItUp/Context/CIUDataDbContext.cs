using System;
using System.Collections.Generic;
using ChatItUp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatItUp.Context;

public partial class CIUDataDbContext : DbContext
{
    public CIUDataDbContext()
    {
    }

    public CIUDataDbContext(DbContextOptions<CIUDataDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<ServerChannel> ServerChannels { get; set; }

    public virtual DbSet<ServerInviteCode> ServerInviteCodes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserServer> UserServers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json")
                                    .AddUserSecrets<CIUDataDbContext>(optional: true, reloadOnChange: true)
                                    .Build();

            var connectionString = configuration.GetConnectionString("CIUDataConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ChannelId).IsRequired(false);
            entity.Property(e => e.Body).HasColumnName("Body");
            entity.Property(e => e.SentOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime2(7)");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Image).HasColumnType("varbinary(max)"); ;
            entity.Property(e => e.DeletedOn).HasColumnType("datetime2(7)").IsRequired(false);
        });

        modelBuilder.Entity<ServerChannel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ServerInviteCode>(entity =>
        {
            entity.HasKey(e => new { e.InviteCode });
            entity.Property(e => e.ExpiresOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EmailAddress).HasMaxLength(255);
            entity.Property(e => e.DisplayName).HasMaxLength(16).IsRequired(false);
            entity.Property(e => e.Status).HasMaxLength(16).IsRequired(false);
        });

        modelBuilder.Entity<UserServer>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ServerId });

            entity.Property(e => e.JoinedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Server).WithMany(p => p.UserServers)
                .HasForeignKey(d => d.ServerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserServers_Server");

            entity.HasOne(d => d.User).WithMany(p => p.UserServers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserServers_Users");
        });

        modelBuilder.Entity<UserMessageStatus>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MessageId});

            entity.Property(e => e.ReadOn).HasColumnType("datetime");
            entity.Property(e => e.EditedOn).HasColumnType("datetime");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
