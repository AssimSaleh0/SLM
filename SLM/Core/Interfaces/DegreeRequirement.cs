namespace SLM.Core.Models
{
    public class DegreeRequirement : BaseEntity
    {
        public int DegreeProgramId { get; set; }
        public int CourseId { get; set; }
        public bool IsRequired { get; set; } = true;
        public bool IsElective { get; set; } = false;
        public int? RecommendedSemester { get; set; }

        // Navigation properties
        public DegreeProgram DegreeProgram { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}