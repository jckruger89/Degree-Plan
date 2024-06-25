using DegreePlan.Models;
using Plugin.LocalNotification;
namespace DegreePlan;

public partial class TermDetails : ContentPage
{
    private Term term;
   
    public TermDetails(Term term)
    {
        InitializeComponent();
        this.term = term;
        TermNameLabel.Text = term.Name + " :  " + term.DateRange.ToString();
        LoadCourses();
        LoadAssessmentNotifications();
    }

    private void LoadAssessmentNotifications()
    {
        var courses = MainPage.database.Table<Course>().Where(course => course.TermID == term.ID);
        foreach (var course in courses)
        {
            //Performance Notifications------------------------------------------------------------------------------------------
            if (course.HasPerformanceNotify == true && course.HasPerformanceAssessment)
            {
                if (course.PerformanceAssessmentStartDate.Date == DateTime.Today.Date)
                {
                    var random = new Random();
                    var notifyId = random.Next(1000);

                    var request = new NotificationRequest
                    {
                        Title = "Performance Assessment Alert",
                        NotificationId = notifyId,
                        Description = $"Performance Assessment for class: {course.Name} starts today!",
                        CategoryType = NotificationCategoryType.Reminder,
                        Schedule = new NotificationRequestSchedule()
                        {
                            NotifyTime = DateTime.Now,
                        }
                    };
                    LocalNotificationCenter.Current.Show(request);
                }
                if (course.PerformanceAssessmentEndDate.Date == DateTime.Today.Date)
                {
                    var random = new Random();
                    var notifyId = random.Next(1000);

                    var request = new NotificationRequest
                    {
                        Title = "Performance Assessment Alert",
                        NotificationId = notifyId,
                        Description = $"Performance Assessment for class: {course.Name} ends today!",
                        CategoryType = NotificationCategoryType.Reminder,
                        Schedule = new NotificationRequestSchedule()
                        {
                            NotifyTime = DateTime.Now,
                        }
                    };
                    LocalNotificationCenter.Current.Show(request);
                }
            }

            //Objective Notifications------------------------------------------------------------------------------------------
            if (course.HasObjectiveNotify == true && course.HasObjectiveAssessment)
            {
                if (course.ObjectiveAssessmentStartDate.Date == DateTime.Today.Date)
                {
                    var random = new Random();
                    var notifyId = random.Next(1000);

                    var request = new NotificationRequest
                    {
                        Title = "Objective Assessment Alert",
                        NotificationId = notifyId,
                        Description = $"Objective Assessment for class: {course.Name} starts today!",
                        CategoryType = NotificationCategoryType.Reminder,
                        Schedule = new NotificationRequestSchedule()
                        {
                            NotifyTime = DateTime.Now,
                        }
                    };
                    LocalNotificationCenter.Current.Show(request);
                }
                if (course.ObjectiveAssessmentEndDate.Date == DateTime.Today.Date)
                {
                    var random = new Random();
                    var notifyId = random.Next(1000);

                    var request = new NotificationRequest
                    {
                        Title = "Objective Assessment Alert",
                        NotificationId = notifyId,
                        Description = $"Objective Assessment for class: {course.Name} ends today!",
                        CategoryType = NotificationCategoryType.Reminder,
                        Schedule = new NotificationRequestSchedule()
                        {
                            NotifyTime = DateTime.Now,
                        }
                    };
                    LocalNotificationCenter.Current.Show(request);
                }
            }
        }
    }

    private void LoadCourses()
    {
        var courses = MainPage.database.Table<Course>().Where(course => course.TermID == term.ID).ToList();
        courseListView.ItemsSource = courses;
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
        AddCourse addCourse = new(this.term);
        await Navigation.PushAsync(addCourse);
    }

    private async void Edit_Clicked(object sender, EventArgs e)
    {
        if (courseListView.SelectedItem != null)
        {
            Course selectedCourse = (Course)courseListView.SelectedItem;
            await Navigation.PushAsync(new EditCourse(selectedCourse, this.term));
        }
        else
        {
            return;
        }
    }

    private void Home_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        if (courseListView.SelectedItem != null)
        {
            bool answer = await DisplayAlert("Delete Course", "Are you sure you want to delete this course?", "Yes", "No");
            if (!answer)
                return;
            Course selectedCourse = (Course)courseListView.SelectedItem;
            MainPage.database.Delete(selectedCourse);
            LoadCourses();
        }
        else
        {
            return;
        }
    }

    private async void CourseDetails_Clicked(object sender, EventArgs e)
    {
        if (courseListView.SelectedItem != null)
        {
            Course selectedCourse = (Course)courseListView.SelectedItem;
            await Navigation.PushAsync(new CourseDetails(selectedCourse, this.term));
        }
        else
        {
            return;
        }
    }
}