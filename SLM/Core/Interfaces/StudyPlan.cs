namespace SLM.Core.Models
{
    public class StudyPlan : BaseEntity
    {
        public int UserId { get; set; }
        public int AssignmentId { get; set; }
        public DateTime PlannedStudyDate { get; set; }
        public int EstimatedHours { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string? Notes { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Assignment Assignment { get; set; } = null!;
    }
}