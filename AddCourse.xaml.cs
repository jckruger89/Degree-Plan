using DegreePlan.Models;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using SQLite;
using System.Text.RegularExpressions;

namespace DegreePlan;

public partial class AddCourse : ContentPage
{
    private Term term;
    Regex regexName = new Regex(@"^[a-zA-Z\s]+$");
    Regex regexPhone = new Regex(@"^[\d-]+$");
    Regex regexEmail = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

    public AddCourse(Term term)
    {
        InitializeComponent();
        this.term = term;
        BindData();
        performanceName.Text = "";
        objectiveName.Text = "";
    }

    private void BindData()
    {
        startDatePicker.Date = term.StartDate;
        endDatePicker.Date = term.EndDate;
        performanceStartDatePicker.Date = term.StartDate;
        performanceEndDatePicker.Date = term.EndDate;
        objectiveStartDatePicker.Date = term.StartDate;
        objectiveEndDatePicker.Date = term.EndDate;
    }

    private void Save_Clicked(object sender, EventArgs e)
    {
        string selectedStatus = (string)picker.SelectedItem;
        bool hasCourseNotify = courseNotifyCheckbox.IsChecked;

        DateTime objectiveStartDate = objectiveStartDatePicker.Date;
        DateTime objectiveEndDate = objectiveEndDatePicker.Date;
        bool hasObjectiveAssessment = objectiveAssessmentCheckbox.IsChecked;
        bool hasObjectiveNotify = objectiveNotifyCheckbox.IsChecked;

        DateTime performanceStartDate = performanceStartDatePicker.Date;
        DateTime performanceEndDate = performanceEndDatePicker.Date;
        bool hasPerformanceAssessment = performanceAssessmentCheckbox.IsChecked;
        bool hasPerformanceNotify = performanceNotifyCheckbox.IsChecked;

        if (startDatePicker.Date < term.StartDate.Date || endDatePicker.Date > term.EndDate.Date)
        {
            DisplayAlert("Error", "Invalid course date range. Must be within term date range", "OK");
            return;
        }

        //////////////Work on this!
        if (hasObjectiveAssessment == false && !string.IsNullOrWhiteSpace(objectiveName.Text))
        {
            DisplayAlert("Error", "Must add/check objective assessment or remove objective assessment name.", "OK");
            return;
        }
        
        if (hasPerformanceAssessment == false && !string.IsNullOrWhiteSpace(performanceName.Text))
        {
            DisplayAlert("Error", "Must add/check performance assessment or remove performance assessment name.", "OK");
            return;
        }

        if (hasObjectiveAssessment == true)
        {
            if (objectiveStartDate < startDatePicker.Date || objectiveEndDate > endDatePicker.Date || objectiveStartDate > objectiveEndDate)
            {
                DisplayAlert("Error", "Invalid objective assessment date range. Assessments must be within current course date range.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(objectiveName.Text))
            {
                DisplayAlert("Error", "Must include an objective assessment name.", "OK");
                return;
            }
        }
                
        if (hasPerformanceAssessment == true)
        {
            if (performanceStartDate < startDatePicker.Date || performanceEndDate > endDatePicker.Date || performanceStartDate > performanceEndDate)
            {
                DisplayAlert("Error", "Invalid performance assessment date range. Assessments must be within current course date range.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(performanceName.Text))
            {
                DisplayAlert("Error", "Must include a performance assessment name.", "OK");
                return;
            }
        }
        
        if (hasObjectiveNotify == true)
        {
            if (hasObjectiveAssessment != true)
            {
                DisplayAlert("Error", "Objective Assessment must be added to enable its notifications.", "OK");
                return;
            }
        }
        
        if (hasPerformanceNotify == true)
        {
            if (hasPerformanceAssessment != true)
            {
                DisplayAlert("Error", "Performance Assessment must be added to enable its notifications.", "OK");
                return;
            }
        }

        if (string.IsNullOrWhiteSpace(courseNameEntry.Text))
        {
            DisplayAlert("Error", "Please enter a course name.", "OK");
            courseInstructorNameEntry.Text = string.Empty;
            return;
        }

        if (string.IsNullOrWhiteSpace(courseInstructorNameEntry.Text) || !regexName.IsMatch(courseInstructorNameEntry.Text))
        {
            DisplayAlert("Error", "Course Instructor name required. Must consist of letters and spaces only.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(courseInstructorPhoneEntry.Text) || !regexPhone.IsMatch(courseInstructorPhoneEntry.Text))
        {
            DisplayAlert("Error", "Course Instructor phone number required. Must consist of numbers and dashes ( - ) only.", "OK");
            courseInstructorPhoneEntry.Text = string.Empty;
            return;
        }

        if (string.IsNullOrWhiteSpace(courseInstructorEmailEntry.Text) || !regexEmail.IsMatch(courseInstructorEmailEntry.Text))
        {
            DisplayAlert("Error", "Course Instructor email required. Must be a valid email address.", "OK");
            courseInstructorEmailEntry.Text = string.Empty;
            return;
        }

        if (startDatePicker.Date >= endDatePicker.Date)
        {
            DisplayAlert("Error", "Start date must be before end date.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(selectedStatus))
        {
            DisplayAlert("Error", "A course status selection must be made.", "OK");
            return;
        }

        if (hasPerformanceAssessment == true && hasObjectiveAssessment == true && performanceName.Text == objectiveName.Text)
        {
            DisplayAlert("Error", "Assessment names must be unique.", "OK");
            return;
        }

        try
        {
            var courseCount = MainPage.database.Table<Course>().Count(course => course.TermID == term.ID);

            if (courseCount >= 6)
            {
                DisplayAlert("Error", "Maximum number of courses per term reached (6 courses)", "OK");
                return;
            }
          
                Course newCourse = new()
                {
                    Name = courseNameEntry.Text,
                    StartDate = startDatePicker.Date,
                    EndDate = endDatePicker.Date,
                    TermID = term.ID,
                    InstructorName = courseInstructorNameEntry.Text,
                    InstructorEmail = courseInstructorEmailEntry.Text,
                    InstructorPhone = courseInstructorPhoneEntry.Text,
                    Status = selectedStatus,
                    HasObjectiveAssessment = hasObjectiveAssessment,
                    ObjectiveAssessmentStartDate = hasObjectiveAssessment ? objectiveStartDate : DateTime.MinValue,
                    ObjectiveAssessmentEndDate = hasObjectiveAssessment ? objectiveEndDate : DateTime.MinValue,
                    HasPerformanceAssessment = hasPerformanceAssessment,
                    PerformanceAssessmentStartDate = hasPerformanceAssessment ? performanceStartDate : DateTime.MinValue,
                    PerformanceAssessmentEndDate = hasPerformanceAssessment ? performanceEndDate : DateTime.MinValue,
                    Notes = notesEntry.Text,
                    HasCourseNotify = hasCourseNotify,
                    HasObjectiveNotify = hasObjectiveNotify,
                    HasPerformanceNotify = hasPerformanceNotify,
                    PerformanceName = performanceName.Text,
                    ObjectiveName = objectiveName.Text
                };
                MainPage.database.Insert(newCourse);
                Navigation.PushAsync(new TermDetails(term));
        }
        catch (SQLite.SQLiteException ex)
        {
            DisplayAlert("Error", "Course name must be unique | " + ex.Message, "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert($"Error",ex.Message, "OK");
        }
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}

   