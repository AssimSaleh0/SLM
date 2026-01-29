using SLM.Core.Enums;

namespace SLM.Core.DTOs.Academic
{
    public class SemesterDto
    {
        public int Id { get; set; }
        public SemesterSeason Season { get; set; }
        public int Year { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsCompleted { get; set; }
        public decimal? GPA { get; set; }
        public int TotalCredits { get; set; }
        public int EnrolledCourses { get; set; }
        public List<EnrollmentDto> Enrollments { get; set; } = new();
    }

    public class CreateSemesterRequest
    {
        public SemesterSeason Season { get; set; }
        public int Year { get; set; }
    }

    public class UpdateSemesterRequest
    {
        public bool? IsCurrent { get; set; }
        public bool? IsCompleted { get; set; }
    }
}