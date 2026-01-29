namespace SLM.Core.Models
{
    public class DegreeProgram : BaseEntity
    {
        public string ProgramName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Description { get; set; }
        public int TotalCreditsRequired { get; set; }

        // Navigation properties
        public ICollection<DegreeRequirement> Requirements { get; set; } = new List<DegreeRequirement>();
        public ICollection<UserDegreeProgram> UserDegreePrograms { get; set; } = new List<UserDegreeProgram>();
    }
}