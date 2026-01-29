namespace SLM.Core.Models
{
    public class CoursePrerequisite : BaseEntity
    {
        public int CourseId { get; set; }
        public int PrerequisiteCourseId { get; set; }
        public bool IsRequired { get; set; } = true;

        // Navigation properties
        public Course Course { get; set; } = null!;
        public Course PrerequisiteCourse { get; set; } = null!;
    }
}