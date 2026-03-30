using SLM.Core.Enums;

namespace SLM.Core.DTOs.Financial
{
    public class BillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public BillStatus Status { get; set; }
        public bool IsRecurring { get; set; }
        public int? RecurringDayOfMonth { get; set; }
        public string? Description { get; set; }
        public DateTime? PaidDate { get; set; }
        public int DaysUntilDue { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class CreateBillRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsRecurring { get; set; }
        public int? RecurringDayOfMonth { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateBillRequest
    {
        public string? Name { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public BillStatus? Status { get; set; }
        public bool? IsRecurring { get; set; }
        public int? RecurringDayOfMonth { get; set; }
        public string? Description { get; set; }
    }

    public class BillSummaryDto
    {
        public decimal TotalUpcoming { get; set; }
        public decimal TotalOverdue { get; set; }
        public int UpcomingCount { get; set; }
        public int OverdueCount { get; set; }
        public List<BillDto> UpcomingBills { get; set; } = new();
        public List<BillDto> OverdueBills { get; set; } = new();
    }
}