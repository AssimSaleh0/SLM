namespace SLM.Core.DTOs.Academic
{
    public class StudyPlanDto
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime PlannedStudyDate { get; set; }
        public int EstimatedHours { get; set; }
        public bool IsCompleted { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateStudyPlanRequest
    {
        public int AssignmentId { get; set; }
        public DateTime PlannedStudyDate { get; set; }
        public int EstimatedHours { get; set; }
        public string? Notes { get; set; }
    }

    public class GenerateStudyPlanRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DailyStudyHours { get; set; } = 4;
    }

    public class StudyPlanSummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalHours { get; set; }
        public List<StudyPlanDto> Plans { get; set; } = new();
    }
}