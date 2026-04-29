using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public partial class DbTestContext : DbContext
{
    public DbTestContext()
    {
    }

    public DbTestContext(DbContextOptions<DbTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usertask> Usertasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=192.168.88.196;Port=5432;Username=postgres;Password=password;Database=db_test");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usertask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usertask_pk");

            entity.ToTable("usertask");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Dtcreate).HasColumnName("dtcreate");
            entity.Property(e => e.Dtupdate).HasColumnName("dtupdate");
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
