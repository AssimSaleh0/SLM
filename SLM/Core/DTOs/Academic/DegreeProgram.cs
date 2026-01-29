namespace SLM.Core.DTOs.Academic
{
    public class DegreeProgramDto
    {
        public int Id { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Description { get; set; }
        public int TotalCreditsRequired { get; set; }
        public int RequiredCourses { get; set; }
        public int ElectiveCourses { get; set; }
    }

    public class CreateDegreeProgramRequest
    {
        public string ProgramName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Description { get; set; }
        public int TotalCreditsRequired { get; set; }
    }

    public class DegreeProgressDto
    {
        public DegreeProgramDto Program { get; set; } = null!;
        public int CompletedCredits { get; set; }
        public int RemainingCredits { get; set; }
        public decimal ProgressPercentage { get; set; }
        public List<CourseDto> CompletedCourses { get; set; } = new();
        public List<CourseDto> InProgressCourses { get; set; } = new();
        public List<CourseDto> RemainingRequiredCourses { get; set; } = new();
    }
}