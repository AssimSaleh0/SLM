using SLM.Core.Enums;

namespace SLM.Core.Models
{
    public class Course : BaseEntity
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? Department { get; set; }
        public int? Level { get; set; }

        // Navigation properties
        public ICollection<CoursePrerequisite> Prerequisites { get; set; } = new List<CoursePrerequisite>();
        public ICollection<CoursePrerequisite> IsPrerequisiteFor { get; set; } = new List<CoursePrerequisite>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<CourseOffering> Offerings { get; set; } = new List<CourseOffering>();
        public ICollection<DegreeRequirement> DegreeRequirements { get; set; } = new List<DegreeRequirement>();
    }
}