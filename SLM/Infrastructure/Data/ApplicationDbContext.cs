using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SLM.Core.Models;

namespace SLM.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Core tables
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Academic tables
        public DbSet<Course> Courses { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<DegreeProgram> DegreePrograms { get; set; }
        public DbSet<DegreeRequirement> DegreeRequirements { get; set; }
        public DbSet<UserDegreeProgram> UserDegreePrograms { get; set; }
        public DbSet<CourseOffering> CourseOfferings { get; set; }
        public DbSet<StudyPlan> StudyPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.StudentId).IsUnique();
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.PasswordHash)
                    .IsRequired();
                
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);
                
                entity.Property(e => e.StudentId)
                    .HasMaxLength(50);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.HasQueryFilter(e => !e.IsDeleted);

                // Seed default roles
                entity.HasData(
                    new Role { Id = 1, Name = "Student", Description = "Student user", CreatedAt = DateTime.UtcNow },
                    new Role { Id = 2, Name = "AcademicAdvisor", Description = "Academic advisor", CreatedAt = DateTime.UtcNow },
                    new Role { Id = 3, Name = "CareerCounselor", Description = "Career counselor", CreatedAt = DateTime.UtcNow },
                    new Role { Id = 4, Name = "Administrator", Description = "System administrator", CreatedAt = DateTime.UtcNow }
                );
            });

            // UserRole configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Roles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedAt);
                
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(1000);
                
                entity.Property(e => e.ActionUrl)
                    .HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // NotificationPreference configuration
            modelBuilder.Entity<NotificationPreference>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();
                
                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<NotificationPreference>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // AuditLog configuration
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.EntityName, e.EntityId });
                
                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.EntityName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.IpAddress)
                    .HasMaxLength(45);
                
                entity.Property(e => e.UserAgent)
                    .HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Course configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CourseCode).IsUnique();
                
                entity.Property(e => e.CourseCode)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.Property(e => e.CourseName)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.Department)
                    .HasMaxLength(100);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // CoursePrerequisite configuration
            modelBuilder.Entity<CoursePrerequisite>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.CourseId, e.PrerequisiteCourseId }).IsUnique();
                
                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Prerequisites)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.PrerequisiteCourse)
                    .WithMany(c => c.IsPrerequisiteFor)
                    .HasForeignKey(e => e.PrerequisiteCourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Semester configuration
            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.Season, e.Year }).IsUnique();
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Enrollment configuration
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.SemesterId, e.CourseId }).IsUnique();
                
                entity.Property(e => e.Grade)
                    .HasMaxLength(5);
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Semester)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(e => e.SemesterId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Assignment configuration
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.EnrollmentId);
                entity.HasIndex(e => e.DueDate);
                
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.AssignmentType)
                    .HasMaxLength(50);
                
                entity.HasOne(e => e.Enrollment)
                    .WithMany(en => en.Assignments)
                    .HasForeignKey(e => e.EnrollmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // DegreeProgram configuration
            modelBuilder.Entity<DegreeProgram>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProgramName).IsUnique();
                
                entity.Property(e => e.ProgramName)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.Department)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // DegreeRequirement configuration
            modelBuilder.Entity<DegreeRequirement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.DegreeProgramId, e.CourseId }).IsUnique();
                
                entity.HasOne(e => e.DegreeProgram)
                    .WithMany(dp => dp.Requirements)
                    .HasForeignKey(e => e.DegreeProgramId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Course)
                    .WithMany(c => c.DegreeRequirements)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // UserDegreeProgram configuration
            modelBuilder.Entity<UserDegreeProgram>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.DegreeProgramId });
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.DegreeProgram)
                    .WithMany(dp => dp.UserDegreePrograms)
                    .HasForeignKey(e => e.DegreeProgramId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // CourseOffering configuration
            modelBuilder.Entity<CourseOffering>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.CourseId, e.Season, e.Year });
                
                entity.Property(e => e.Instructor)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Location)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Schedule)
                    .HasMaxLength(255);
                
                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Offerings)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // StudyPlan configuration
            modelBuilder.Entity<StudyPlan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.PlannedStudyDate });
                
                entity.Property(e => e.Notes)
                    .HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Assignment)
                    .WithMany()
                    .HasForeignKey(e => e.AssignmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added || 
                    e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        //override for Microsoft.EntityFrameworkCore.Migrations.PendingModelChangesWarning warning on "Update-Database" command

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

    }
}