namespace SLM.Core.DTOs.Academic
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? MaxPoints { get; set; }
        public decimal? EarnedPoints { get; set; }
        public bool IsCompleted { get; set; }
        public string? AssignmentType { get; set; }
        public int DaysUntilDue { get; set; }
    }

    public class CreateAssignmentRequest
    {
        public int EnrollmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? MaxPoints { get; set; }
        public string? AssignmentType { get; set; }
    }

    public class UpdateAssignmentRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? MaxPoints { get; set; }
        public decimal? EarnedPoints { get; set; }
        public bool? IsCompleted { get; set; }
        public string? AssignmentType { get; set; }
    }
}