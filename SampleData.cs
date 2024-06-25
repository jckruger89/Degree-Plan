using DegreePlan.Models;
using SQLite;
using System.Security.Cryptography.X509Certificates;

namespace DegreePlan;

public static class SampleData //Class is for testing purposes only
{
    static SampleData()
    {

    }
    public static void Sample()
    {
        try
        {
            Term term = new Term
            {
                Name = "Term 1",
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 6, 30)
            };

            MainPage.database.Insert(term);

            Course course = new Course
            {
                TermID = term.ID,
                Name = "Mobile Applications",
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 6, 30),
                HasObjectiveAssessment = true,
                HasPerformanceAssessment = true,
                PerformanceName = "Test 2",
                ObjectiveName = "Test 1",
                ObjectiveAssessmentStartDate = new DateTime(2024, 2, 1),
                ObjectiveAssessmentEndDate = new DateTime(2024, 4, 1),
                PerformanceAssessmentStartDate = new DateTime(2024, 4, 1),
                PerformanceAssessmentEndDate = new DateTime(2024, 6, 1),
                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                Status = "In-Progress",
                Notes = "Course details..."
            };

            MainPage.database.Insert(course);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}