using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PRN221_FinalProject.Models
{
    public partial class PRN221FinalProjectContext : DbContext
    {
        public PRN221FinalProjectContext()
        {
        }

        public PRN221FinalProjectContext(DbContextOptions<PRN221FinalProjectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<Slot> Slots { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<Teacher> Teachers { get; set; } = null!;
        public virtual DbSet<TimeSlot> TimeSlots { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyCnn"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("Class");

                entity.Property(e => e.ClassName).HasMaxLength(50);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.RoomName).HasMaxLength(50);
            });

            modelBuilder.Entity<Slot>(entity =>
            {
                entity.ToTable("Slot");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Slots)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("FK_Slot_Class");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Slots)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_Slot_Room");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Slots)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("FK_Slot_Subject");

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.Slots)
                    .HasForeignKey(d => d.TeacherId)
                    .HasConstraintName("FK_Slot_Teacher");

                entity.HasOne(d => d.TimeSlot)
                    .WithMany(p => p.Slots)
                    .HasForeignKey(d => d.TimeSlotId)
                    .HasConstraintName("FK_Slot_TimeSlot");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");

                entity.Property(e => e.SubjectName).HasMaxLength(50);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teacher");

                entity.Property(e => e.TeacherName).HasMaxLength(50);
            });

            modelBuilder.Entity<TimeSlot>(entity =>
            {
                entity.ToTable("TimeSlot");

                entity.Property(e => e.TimeOfDay)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
