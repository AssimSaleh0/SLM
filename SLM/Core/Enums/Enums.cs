namespace SLM.Core.Enums
{
    public enum Enums
    {
        Student = 1,
        AcademicAdvisor = 2,
        CareerCounselor = 3,
        Administrator = 4
    }

    public enum ApplicationStatus
    {
        Applied = 1,
        Interviewed = 2,
        OfferReceived = 3,
        Rejected = 4,
        Accepted = 5,
        Withdrawn = 6
    }

    public enum TransactionType
    {
        Expense = 1,
        Income = 2
    }

    public enum BillStatus
    {
        Pending = 1,
        Paid = 2,
        Overdue = 3
    }

    public enum NotificationType
    {
        BudgetOverage = 1,
        Deadline = 2,
        JobOffer = 3,
        EventReminder = 4,
        BillPayment = 5,
        General = 6
    }

    public enum EventType
    {
        StudentClub = 1,
        StudyGroup = 2,
        SocialGathering = 3,
        Career = 4,
        Academic = 5
    }

    public enum TaskStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum SemesterSeason
    {
        Fall = 1,
        Winter = 2,
        Spring = 3,
        Summer = 4
    }
}