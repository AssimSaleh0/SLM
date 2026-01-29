namespace SLM.Core.Models
{
    public class Assignment : BaseEntity
    {
        public int EnrollmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? MaxPoints { get; set; }
        public decimal? EarnedPoints { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string? AssignmentType { get; set; }

        // Navigation properties
        public Enrollment Enrollment { get; set; } = null!;
    }
}