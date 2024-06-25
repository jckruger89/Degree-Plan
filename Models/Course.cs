using SQLite;
using Plugin.LocalNotification;

namespace DegreePlan.Models
{
    [Table("Course")]
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int TermID { get; set; }

        [Unique]
        public string? Name { get; set; }
        public string? PerformanceName { get; set; }
        public string? ObjectiveName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DateRange => $"{StartDate.ToShortDateString()} : {EndDate.ToShortDateString()}";
        public string? InstructorName { get; set; }
        public string? InstructorPhone { get; set; }
        public string? InstructorEmail { get; set; }
        public string Status { get; set; } = "In-Progress";
        public string Notes { get; set; } = "Notes...";

        public bool HasObjectiveAssessment { get; set; }
        public DateTime ObjectiveAssessmentStartDate { get; set; }
        public DateTime ObjectiveAssessmentEndDate { get; set; }
        public string ObjectiveAssessmentDateRange =>
            $"{ObjectiveAssessmentStartDate.ToShortDateString()} : {ObjectiveAssessmentEndDate.ToShortDateString()}";

        public bool HasPerformanceAssessment { get; set; }
        public DateTime PerformanceAssessmentStartDate { get; set; }
        public DateTime PerformanceAssessmentEndDate { get; set; }
        public string PerformanceAssessmentDateRange =>
            $"{PerformanceAssessmentStartDate.ToShortDateString()} : {PerformanceAssessmentEndDate.ToShortDateString()}";

        public bool HasObjectiveNotify { get; set; }
        public bool HasPerformanceNotify { get; set; }
        public bool HasCourseNotify { get; set; }
    }
}