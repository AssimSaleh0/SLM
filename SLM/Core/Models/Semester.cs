using SLM.Core.Enums;

namespace SLM.Core.Models
{
    public class Semester : BaseEntity
    {
        public int UserId { get; set; }
        public SemesterSeason Season { get; set; }
        public int Year { get; set; }
        public bool IsCurrent { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        public decimal? GPA { get; set; }
        public int TotalCredits { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}