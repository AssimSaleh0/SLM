namespace SLM.Core.DTOs.Academic
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int SemesterId { get; set; }
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? Grade { get; set; }
        public decimal? GradePoint { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsPlanned { get; set; }
    }

    public class CreateEnrollmentRequest
    {
        public int SemesterId { get; set; }
        public int CourseId { get; set; }
        public bool IsPlanned { get; set; } = false;
    }

    public class UpdateEnrollmentRequest
    {
        public string? Grade { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsInProgress { get; set; }
    }
}