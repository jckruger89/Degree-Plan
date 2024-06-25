using DegreePlan.Models;
namespace DegreePlan;

public partial class CourseDetails : ContentPage
{
    private Term term;
    public CourseDetails(Course course, Term term)
    {
        InitializeComponent();
        BindingContext = course;
        this.term = term;

        if (course.HasPerformanceAssessment)
        {
            _ = $"{course.PerformanceAssessmentDateRange}";
                _ = $"{course.PerformanceName}";
        }
        if (course.HasObjectiveAssessment)
        {
            _ = $"{course.ObjectiveAssessmentDateRange}";
            _ = $"{course.ObjectiveName}";
        }
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void EditCourse_Clicked(object sender, EventArgs e)
    {
        Course course = (Course)BindingContext;
        await Navigation.PushAsync(new EditCourse(course, term));
    }

    private async void Share_Clicked(object sender, EventArgs e)
    {
        Course course = (Course)BindingContext;

        await ShareNotes(course.Notes);
    }

    public async Task ShareNotes(string notes)
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = notes,
            Title = "Share Notes"
        });
    }
}