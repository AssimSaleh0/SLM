namespace SLM.Core.Models
{
    public class UserDegreeProgram : BaseEntity
    {
        public int UserId { get; set; }
        public int DegreeProgramId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpectedGraduationDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public User User { get; set; } = null!;
        public DegreeProgram DegreeProgram { get; set; } = null!;
    }
}