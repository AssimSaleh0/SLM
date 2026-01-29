using SLM.Core.Enums;

namespace SLM.Core.Models
{
    public class CourseOffering : BaseEntity
    {
        public int CourseId { get; set; }
        public SemesterSeason Season { get; set; }
        public int Year { get; set; }
        public string? Instructor { get; set; }
        public int? MaxEnrollment { get; set; }
        public string? Location { get; set; }
        public string? Schedule { get; set; }

        // Navigation properties
        public Course Course { get; set; } = null!;
    }
}