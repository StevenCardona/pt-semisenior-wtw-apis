using Microsoft.EntityFrameworkCore;
using Models.Tasks;
using Models.Users;

namespace Repository.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Mail)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(e => e.Mail)
                .IsUnique();

            entity.Property(e => e.Rol)
                .IsRequired()
                .HasConversion(
                    v => ToRoleString(v),
                    v => FromRoleString(v))
                .HasMaxLength(20);

            entity.Property(e => e.CreatedBy)
                .IsRequired();

            entity.Property(e => e.CreatedDate)
                .IsRequired();

            entity.Property(e => e.UpdatedBy);

            entity.Property(e => e.UpdatedDate);

            entity.HasMany(e => e.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            entity.HasData(
                new User
                {
                    Id = 1,
                    Name = "wtw",
                    Mail = "wtw@wtw.com",
                    Rol = UserRole.Admin,
                    CreatedBy = 1,
                    CreatedDate = seedDate
                },
                new User
                {
                    Id = 2,
                    Name = "steven",
                    Mail = "steven@wtw.com",
                    Rol = UserRole.User,
                    CreatedBy = 1,
                    CreatedDate = seedDate
                });
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion(
                    v => ToStatusString(v),
                    v => FromStatusString(v))
                .HasMaxLength(20);

            entity.Property(e => e.UserId)
                .IsRequired();

            entity.Property(e => e.AdditionalInfo)
                .HasColumnType("nvarchar(max)");

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Tasks_AdditionalInfo_IsJson",
                "[AdditionalInfo] IS NULL OR ISJSON([AdditionalInfo]) = 1"));

            entity.Property(e => e.CreatedBy)
                .IsRequired();

            entity.Property(e => e.CreatedDate)
                .IsRequired();

            entity.Property(e => e.UpdatedBy);

            entity.Property(e => e.UpdatedDate);

            entity.HasIndex(e => e.UserId);

            entity.HasIndex(e => new { e.UserId, e.Status });
        });
    }

    private static string ToRoleString(UserRole role) => role switch
    {
        UserRole.Admin => "admin",
        UserRole.User => "user",
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Rol no válido.")
    };

    private static UserRole FromRoleString(string value) => value switch
    {
        "admin" => UserRole.Admin,
        "user" => UserRole.User,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Rol no válido.")
    };

    private static string ToStatusString(TaskItemStatus status) => status switch
    {
        TaskItemStatus.Pending => "pending",
        TaskItemStatus.InProgress => "in_progress",
        TaskItemStatus.Done => "done",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Estado no válido.")
    };

    private static TaskItemStatus FromStatusString(string value) => value switch
    {
        "pending" => TaskItemStatus.Pending,
        "in_progress" => TaskItemStatus.InProgress,
        "done" => TaskItemStatus.Done,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Estado no válido.")
    };
}
