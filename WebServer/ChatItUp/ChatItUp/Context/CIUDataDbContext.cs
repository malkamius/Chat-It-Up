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

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageAttachment> MessageAttachments { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<ServerInviteCode> ServerInviteCodes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivePrivateChannel> UserActivePrivateChannels { get; set; }

    public virtual DbSet<UserMessageStatus> UserMessageStatuses { get; set; }

    public virtual DbSet<UserServer> UserServers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=CIUDataConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Channel>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasMany(d => d.Users).WithMany(p => p.Channels)
                .UsingEntity<Dictionary<string, object>>(
                    "PrivateChannelMember",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PrivateChannelMembers_Users"),
                    l => l.HasOne<Channel>().WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PrivateChannelMembers_Channels"),
                    j =>
                    {
                        j.HasKey("ChannelId", "UserId");
                        j.ToTable("PrivateChannelMembers");
                    });
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.FriendUserId });
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.SentOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<MessageAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId);

            entity.Property(e => e.AttachmentId).ValueGeneratedNever();
            entity.Property(e => e.AttachmentName).HasMaxLength(256);
            entity.Property(e => e.AttachmentPath).HasMaxLength(1024);

            entity.HasOne(d => d.Message).WithMany(p => p.MessageAttachments)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageAttachments_Message");
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<ServerInviteCode>(entity =>
        {
            entity.HasKey(e => e.InviteCode).HasName("PK_ServerInviteCodes_1");

            entity.Property(e => e.InviteCode).ValueGeneratedNever();
            entity.Property(e => e.ExpiresOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DisplayName).HasMaxLength(16);
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.Status)
                .HasMaxLength(16)
                .HasDefaultValue("Offline");
        });

        modelBuilder.Entity<UserActivePrivateChannel>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ChannelId });

            entity.HasOne(d => d.Channel).WithMany(p => p.UserActivePrivateChannels)
                .HasForeignKey(d => d.ChannelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserActivePrivateChannels_Channels");

            entity.HasOne(d => d.User).WithMany(p => p.UserActivePrivateChannels)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserActivePrivateChannels_Users");
        });

        modelBuilder.Entity<UserMessageStatus>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MessageId });

            entity.ToTable("UserMessageStatus");

            entity.Property(e => e.EditedOn).HasColumnType("datetime");
            entity.Property(e => e.ReadOn).HasColumnType("datetime");
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
