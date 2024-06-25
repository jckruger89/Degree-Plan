using SQLite;
using SQLitePCL;
using Microsoft.Maui.Controls;
using System;
using DegreePlan.Models;
using System.IO;
using Plugin.LocalNotification;


namespace DegreePlan
{
    public partial class MainPage : ContentPage
    {
        public static SQLiteConnection database;

        public MainPage()
        {
            InitializeComponent();
            InitializeDatabase();
            SampleData.Sample(); //For testing purposes C6, remove after testing
            LoadTerms();
            LoadCourseNotifications();
        }

        private static void InitializeDatabase()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "degree.db");
            database = new SQLiteConnection(dbPath);
            database.CreateTable<Term>();
            database.CreateTable<Course>();
        }

        private void LoadCourseNotifications()
        {
            var courses = database.Table<Course>();
            foreach (var course in courses)
            {
                //Course Notifications------------------------------------------------------------------------------------------
                if (course.HasCourseNotify == true)
                {
                    if (course.StartDate.Date == DateTime.Today.Date)
                    {
                        var random = new Random();
                        var notifyId = random.Next(1000);

                        var request = new NotificationRequest
                        {
                            Title = "Course Alert",
                            NotificationId = notifyId,
                            Description = $"{course.Name} starts today!",
                            CategoryType = NotificationCategoryType.Reminder,
                            Schedule = new NotificationRequestSchedule()
                            {
                                NotifyTime = DateTime.Now
                            }
                        };
                        LocalNotificationCenter.Current.Show(request);
                    }
                    if (course.EndDate.Date == DateTime.Today.Date)
                    {
                        var random = new Random();
                        var notifyId = random.Next(1000);

                        var request = new NotificationRequest
                        {
                            Title = "Course Alert",
                            NotificationId = notifyId,
                            Description = $"{course.Name} ends today!",
                            CategoryType = NotificationCategoryType.Reminder,
                            Schedule = new NotificationRequestSchedule()
                            {
                                NotifyTime = DateTime.Now
                            }
                        };
                        LocalNotificationCenter.Current.Show(request);
                    }
                }
            }
        }

        private void LoadTerms()
        {
            var terms = database.Table<Term>().ToList();
            termListView.ItemsSource = terms;
        }

        private void AddTerm_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(termNameEntry.Text))
            {
                DisplayAlert("Error", "Please enter a term name.", "OK");
                return;
            }

            if (startDatePicker.Date >= endDatePicker.Date)
            {
                DisplayAlert("Error", "Start date must be before end date.", "OK");
                return;
            }
            try
            {
                Term newTerm = new()
                {
                    Name = termNameEntry.Text,
                    StartDate = startDatePicker.Date,
                    EndDate = endDatePicker.Date
                };
                database.Insert(newTerm);
                LoadTerms();
            }

            catch (SQLite.SQLiteException ex)
            {
                DisplayAlert("Error", "Term name must be unique | " + ex.Message, "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }

        }

        private async void EditTerm_Clicked(object sender, EventArgs e)
        {
            if (termListView.SelectedItem != null)
            {
                Term selectedTerm = (Term)termListView.SelectedItem;
                await Navigation.PushAsync(new EditTerm(selectedTerm));
            }
            else
            {
                return;
            }
        }

        private async void DeleteTerm_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (termListView.SelectedItem != null)
                {
                    bool answer = await DisplayAlert("Delete Term", "Are you sure you want to delete this term?", "Yes", "No");
                    if (!answer)
                        return;
                    Term selectedTerm = (Term)termListView.SelectedItem;
                    var courses = database.Table<Course>().Where(course => course.TermID == selectedTerm.ID).ToList();
                    if (courses.Count == 0)
                    {
                        database.Delete(selectedTerm);
                        LoadTerms();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Cannot delete: Term must not contain any courses", "OK");
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void ViewTermDetails_Clicked(object sender, EventArgs e)
        {
            if (termListView.SelectedItem != null)
            {
                Term selectedTerm = (Term)termListView.SelectedItem;
                await Navigation.PushAsync(new TermDetails(selectedTerm));
            }
            else
            {
                return;
            }
        }

    }
}
