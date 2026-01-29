namespace SLM.Core.DTOs.Academic
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? Department { get; set; }
        public int? Level { get; set; }
        public List<string> PrerequisiteCodes { get; set; } = new();
    }

    public class CreateCourseRequest
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? Department { get; set; }
        public int? Level { get; set; }
        public List<int> PrerequisiteCourseIds { get; set; } = new();
    }

    public class UpdateCourseRequest
    {
        public string? CourseName { get; set; }
        public string? Description { get; set; }
        public int? Credits { get; set; }
        public string? Department { get; set; }
        public int? Level { get; set; }
    }
}