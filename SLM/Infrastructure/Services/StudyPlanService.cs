using SLM.Core.DTOs.Academic;
using SLM.Core.Interfaces;
using SLM.Core.Models;

namespace SLM.Infrastructure.Services
{
    public class StudyPlanService : IStudyPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudyPlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StudyPlanDto> CreateStudyPlanAsync(int userId, CreateStudyPlanRequest request)
        {
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(request.AssignmentId);
            if (assignment == null)
            {
                throw new InvalidOperationException("Assignment not found");
            }

            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == assignment.EnrollmentId && e.UserId == userId);

            if (enrollment == null)
            {
                throw new InvalidOperationException("Enrollment not found");
            }

            var studyPlan = new StudyPlan
            {
                UserId = userId,
                AssignmentId = request.AssignmentId,
                PlannedStudyDate = request.PlannedStudyDate,
                EstimatedHours = request.EstimatedHours,
                Notes = request.Notes,
                IsCompleted = false
            };

            await _unitOfWork.Repository<StudyPlan>().AddAsync(studyPlan);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(studyPlan);
        }

        public async Task<List<StudyPlanDto>> GetUserStudyPlansAsync(int userId)
        {
            var studyPlans = await _unitOfWork.Repository<StudyPlan>()
                .FindAsync(sp => sp.UserId == userId);

            var dtos = new List<StudyPlanDto>();
            foreach (var plan in studyPlans.OrderBy(sp => sp.PlannedStudyDate))
            {
                dtos.Add(await MapToDtoAsync(plan));
            }

            return dtos;
        }

        public async Task<List<StudyPlanDto>> GetStudyPlansByDateAsync(int userId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var studyPlans = await _unitOfWork.Repository<StudyPlan>()
                .FindAsync(sp => sp.UserId == userId &&
                                sp.PlannedStudyDate >= startDate &&
                                sp.PlannedStudyDate < endDate);

            var dtos = new List<StudyPlanDto>();
            foreach (var plan in studyPlans.OrderBy(sp => sp.PlannedStudyDate))
            {
                dtos.Add(await MapToDtoAsync(plan));
            }

            return dtos;
        }

        public async Task<List<StudyPlanSummaryDto>> GetWeeklyStudyPlanAsync(int userId, DateTime startDate)
        {
            var endDate = startDate.AddDays(7);

            var studyPlans = await _unitOfWork.Repository<StudyPlan>()
                .FindAsync(sp => sp.UserId == userId &&
                                sp.PlannedStudyDate >= startDate &&
                                sp.PlannedStudyDate < endDate);

            var groupedPlans = studyPlans
                .GroupBy(sp => sp.PlannedStudyDate.Date)
                .OrderBy(g => g.Key);

            var summaries = new List<StudyPlanSummaryDto>();

            foreach (var group in groupedPlans)
            {
                var planDtos = new List<StudyPlanDto>();
                foreach (var plan in group)
                {
                    planDtos.Add(await MapToDtoAsync(plan));
                }

                summaries.Add(new StudyPlanSummaryDto
                {
                    Date = group.Key,
                    TotalHours = group.Sum(sp => sp.EstimatedHours),
                    Plans = planDtos
                });
            }

            return summaries;
        }

        public async Task<List<StudyPlanDto>> GenerateStudyPlanAsync(int userId, GenerateStudyPlanRequest request)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsInProgress);

            var enrollmentIds = enrollments.Select(e => e.Id).ToList();

            var assignments = await _unitOfWork.Repository<Assignment>()
                .FindAsync(a => enrollmentIds.Contains(a.EnrollmentId) &&
                               !a.IsCompleted &&
                               a.DueDate >= request.StartDate &&
                               a.DueDate <= request.EndDate);

            var orderedAssignments = assignments.OrderBy(a => a.DueDate).ToList();

            var generatedPlans = new List<StudyPlanDto>();
            var currentDate = request.StartDate;

            foreach (var assignment in orderedAssignments)
            {
                var daysUntilDue = (assignment.DueDate - currentDate).Days;
                var sessionsNeeded = Math.Max(1, daysUntilDue / 2);
                var hoursPerSession = Math.Max(1, request.DailyStudyHours / sessionsNeeded);

                for (int i = 0; i < sessionsNeeded && currentDate < assignment.DueDate; i++)
                {
                    var studyPlan = new StudyPlan
                    {
                        UserId = userId,
                        AssignmentId = assignment.Id,
                        PlannedStudyDate = currentDate.AddDays(i),
                        EstimatedHours = hoursPerSession,
                        Notes = $"Auto-generated study session {i + 1} of {sessionsNeeded}",
                        IsCompleted = false
                    };

                    await _unitOfWork.Repository<StudyPlan>().AddAsync(studyPlan);
                    generatedPlans.Add(await MapToDtoAsync(studyPlan));
                }

                currentDate = currentDate.AddDays(sessionsNeeded);
            }

            await _unitOfWork.SaveChangesAsync();

            return generatedPlans;
        }

        public async Task<bool> UpdateStudyPlanAsync(int studyPlanId, int userId, CreateStudyPlanRequest request)
        {
            var studyPlan = await _unitOfWork.Repository<StudyPlan>()
                .FirstOrDefaultAsync(sp => sp.Id == studyPlanId && sp.UserId == userId);

            if (studyPlan == null)
            {
                return false;
            }

            studyPlan.PlannedStudyDate = request.PlannedStudyDate;
            studyPlan.EstimatedHours = request.EstimatedHours;
            studyPlan.Notes = request.Notes;

            await _unitOfWork.Repository<StudyPlan>().UpdateAsync(studyPlan);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkStudyPlanCompletedAsync(int studyPlanId, int userId)
        {
            var studyPlan = await _unitOfWork.Repository<StudyPlan>()
                .FirstOrDefaultAsync(sp => sp.Id == studyPlanId && sp.UserId == userId);

            if (studyPlan == null)
            {
                return false;
            }

            studyPlan.IsCompleted = true;

            await _unitOfWork.Repository<StudyPlan>().UpdateAsync(studyPlan);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteStudyPlanAsync(int studyPlanId, int userId)
        {
            var studyPlan = await _unitOfWork.Repository<StudyPlan>()
                .FirstOrDefaultAsync(sp => sp.Id == studyPlanId && sp.UserId == userId);

            if (studyPlan == null)
            {
                return false;
            }

            await _unitOfWork.Repository<StudyPlan>().DeleteAsync(studyPlan);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<StudyPlanDto> MapToDtoAsync(StudyPlan studyPlan)
        {
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(studyPlan.AssignmentId);
            var enrollment = assignment != null
                ? await _unitOfWork.Repository<Enrollment>().GetByIdAsync(assignment.EnrollmentId)
                : null;
            var course = enrollment != null
                ? await _unitOfWork.Repository<Course>().GetByIdAsync(enrollment.CourseId)
                : null;

            return new StudyPlanDto
            {
                Id = studyPlan.Id,
                AssignmentId = studyPlan.AssignmentId,
                AssignmentTitle = assignment?.Title ?? "",
                CourseCode = course?.CourseCode ?? "",
                PlannedStudyDate = studyPlan.PlannedStudyDate,
                EstimatedHours = studyPlan.EstimatedHours,
                IsCompleted = studyPlan.IsCompleted,
                Notes = studyPlan.Notes
            };
        }
    }
}