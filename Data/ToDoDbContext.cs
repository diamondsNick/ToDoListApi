using Microsoft.EntityFrameworkCore;

namespace ToDoListApi.Data
{
    public class ToDoDbContext : DbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
        {
        }

        public DbSet<Entities.Page> Pages { get; set; }
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.Assignment> Assignments { get; set; }
        public DbSet<Entities.Status> Statuses { get; set; }
        public DbSet<Entities.AssigmentPage> AssigmentPages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Page>(entity =>
            { 
                entity.HasKey(e => e.Id)
                .HasName("PK_Pages");

                entity.ToTable("Pages");

                entity.HasOne(e => e.Author)
                    .WithMany(u => u.Pages)
                    .HasForeignKey(e => e.UserId)
                    .HasConstraintName("FK_User_Pages");

                entity.Property(a => a.CreationDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("GETUTCDATE()");
            });
                

            modelBuilder.Entity<Entities.User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id)
                    .HasName("PK_Users");

                entity.HasIndex(e => e.Login)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Login");

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Login");
            });

            modelBuilder.Entity<Entities.Status>(entity =>
            {
                entity.ToTable("Statuses");
                entity.HasKey(e => e.Id)
                    .HasName("PK_Status");

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Status_Name");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Name");
            });

            modelBuilder.Entity<Entities.Assignment>(entity =>
            {
                entity.ToTable("Assignments");
                entity.HasKey(e => e.Id)
                    .HasName("PK_Assignment");

                entity.Property(e => e.Text)
                    .HasMaxLength(500);

                entity.HasOne(e => e.CurStatus)
                    .WithMany(s => s.Assignments)
                    .HasForeignKey(e => e.StatusId)
                    .HasConstraintName("FK_Assignment_Status");

                entity.Property(a => a.CreationDate)
                    .HasColumnType("datetime2(0)");
                entity.Property(a => a.CompletionDate)
                    .HasColumnType("datetime2(0)");
                entity.Property(a => a.CompletionDeadline)
                    .HasColumnType("datetime2(0)");

                entity.HasCheckConstraint(
                    "CK_Assignment_CreationDate",
                    "CreationDate >= '2025-01-01'");
            });

            modelBuilder.Entity<Entities.AssigmentPage>(entity =>
            {
                entity.ToTable("AssignmentsPages");
                entity.HasKey(e => new { e.AssigmentId, e.PageId })
                    .HasName("PK_Page_Assignment");

                entity.HasOne(e => e.Assignment)
                    .WithMany(a => a.AssigmentsPages)
                    .HasConstraintName("FK_Assignment_AssignmentPage");

                entity.HasOne(e => e.Page)
                    .WithMany(p => p.AssigmentsPages)
                    .HasConstraintName("FK_Page_AssignmentPage");
            });
        }

    }
}
