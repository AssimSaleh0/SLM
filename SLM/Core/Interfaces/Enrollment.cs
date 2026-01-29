namespace SLM.Core.Models
{
    public class Enrollment : BaseEntity
    {
        public int UserId { get; set; }
        public int SemesterId { get; set; }
        public int CourseId { get; set; }
        public string? Grade { get; set; }
        public decimal? GradePoint { get; set; }
        public bool IsCompleted { get; set; } = false;
        public bool IsInProgress { get; set; } = false;
        public bool IsPlanned { get; set; } = false;

        // Navigation properties
        public User User { get; set; } = null!;
        public Semester Semester { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}