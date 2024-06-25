using DegreePlan.Models;
using Plugin.LocalNotification;
using System.Text.RegularExpressions;


namespace DegreePlan;

public partial class EditCourse : ContentPage
{
    Course course;
    Term term;
    Regex regexName = new Regex(@"^[a-zA-Z\s]+$");
    Regex regexPhone = new Regex(@"^[\d-]+$");
    Regex regexEmail = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

    public EditCourse(Course course, Term term)
    {
        this.term = term;
        InitializeComponent();
        this.course = course;
        BindData();
    }

    private void BindData()
    {
        courseNameEntry.Text = course.Name;
        startDatePicker.Date = course.StartDate;
        endDatePicker.Date = course.EndDate;
        objectiveStartDatePicker.Date = course.ObjectiveAssessmentStartDate;
        objectiveEndDatePicker.Date = course.ObjectiveAssessmentEndDate;
        performanceStartDatePicker.Date = course.PerformanceAssessmentStartDate;
        performanceEndDatePicker.Date = course.PerformanceAssessmentEndDate;
        courseInstructorEmailEntry.Text = course.InstructorEmail;
        courseInstructorNameEntry.Text = course.InstructorName;
        courseInstructorPhoneEntry.Text = course.InstructorPhone;
        string[] pickerItems = picker.ItemsSource.Cast<string>().ToArray();
        int selectedIndex = Array.IndexOf(pickerItems, course.Status);
        picker.SelectedIndex = selectedIndex;
        notesEntry.Text = course.Notes;
        performanceAssessmentCheckbox.IsChecked = course.HasPerformanceAssessment;
        objectiveAssessmentCheckbox.IsChecked = course.HasObjectiveAssessment;
        courseNotifyCheckbox.IsChecked = course.HasCourseNotify;
        objectiveNotifyCheckbox.IsChecked = course.HasObjectiveNotify;
        performanceNotifyCheckbox.IsChecked = course.HasPerformanceNotify;
        performanceName.Text = course.PerformanceName;
        objectiveName.Text = course.ObjectiveName;
    }

    private void Save_Clicked(object sender, EventArgs e)
    {
        string selectedStatus = (string)picker.SelectedItem;

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

        if (hasObjectiveAssessment == false && objectiveName.Text != string.Empty)
        {
            DisplayAlert("Error", "Must add/check objective assessment or remove objective assessment name.", "OK");
            return;
        }

        if (hasPerformanceAssessment == false && performanceName.Text != string.Empty)
        {
            DisplayAlert("Error", "Must add/check performance assessment or remove performance assessment name.", "OK");
            return;
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

        if (hasObjectiveAssessment == false && string.IsNullOrWhiteSpace(objectiveName.Text))
        {
            course.ObjectiveName = string.Empty;
            objectiveStartDatePicker.Date = DateTime.MinValue;
            objectiveEndDatePicker.Date = DateTime.MinValue;
        }

        if (hasPerformanceAssessment == false && string.IsNullOrWhiteSpace(performanceName.Text))
        {
            course.PerformanceName = string.Empty;
            performanceStartDatePicker.Date = DateTime.MinValue;
            performanceEndDatePicker.Date = DateTime.MinValue;
        }

        if (hasPerformanceAssessment == true && hasObjectiveAssessment == true && performanceName.Text == objectiveName.Text)
        {
            DisplayAlert("Error", "Assessment names must be unique.", "OK");
            return;
        }

        try
        {
            course.Name = courseNameEntry.Text;
            course.StartDate = startDatePicker.Date;
            course.EndDate = endDatePicker.Date;
            course.PerformanceAssessmentStartDate = performanceStartDatePicker.Date;
            course.PerformanceAssessmentEndDate = performanceEndDatePicker.Date;
            course.ObjectiveAssessmentStartDate = objectiveStartDatePicker.Date;
            course.ObjectiveAssessmentEndDate = objectiveEndDatePicker.Date;
            course.InstructorPhone = courseInstructorPhoneEntry.Text;
            course.InstructorEmail = courseInstructorEmailEntry.Text;
            course.InstructorName = courseInstructorNameEntry.Text;
            course.Status = (string)picker.SelectedItem;
            course.HasObjectiveAssessment = objectiveAssessmentCheckbox.IsChecked;
            course.HasPerformanceAssessment = performanceAssessmentCheckbox.IsChecked;
            course.Notes = notesEntry.Text;
            course.HasCourseNotify = courseNotifyCheckbox.IsChecked;
            course.HasObjectiveNotify = objectiveNotifyCheckbox.IsChecked;
            course.HasPerformanceNotify = performanceNotifyCheckbox.IsChecked;
            course.PerformanceName = performanceName.Text;
            course.ObjectiveName = objectiveName.Text;
            MainPage.database.Update(course);
           
            Navigation.PushAsync(new TermDetails(term));
        }
        catch (SQLite.SQLiteException ex)
        {
            DisplayAlert("Error", "Course name must be unique | " + ex.Message, "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert($"Error", ex.Message, "OK");
        }
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}