namespace SLM.Core.DTOs.Academic
{
    public class GradeDto
    {
        public int EnrollmentId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? Grade { get; set; }
        public decimal? GradePoint { get; set; }
        public string SemesterName { get; set; } = string.Empty;
    }

    public class SubmitGradeRequest
    {
        public int EnrollmentId { get; set; }
        public string Grade { get; set; } = string.Empty;
    }

    public class GPACalculationDto
    {
        public decimal CurrentSemesterGPA { get; set; }
        public decimal CumulativeGPA { get; set; }
        public int TotalCreditsCompleted { get; set; }
        public int TotalCreditsInProgress { get; set; }
        public List<SemesterGPADto> SemesterGPAs { get; set; } = new();
    }

    public class SemesterGPADto
    {
        public string SemesterName { get; set; } = string.Empty;
        public decimal GPA { get; set; }
        public int Credits { get; set; }
    }
}