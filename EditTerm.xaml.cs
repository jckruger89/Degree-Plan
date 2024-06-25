using DegreePlan.Models;
using System;
using Microsoft.Maui.Controls;
using SQLite;
using Plugin.LocalNotification;


namespace DegreePlan;

public partial class EditTerm : ContentPage
{
    private Term term;

    public EditTerm(Term term)
    {
        InitializeComponent();
        this.term = term;
        BindData();
    }

    private void BindData()
    {
        termNameEntry.Text = term.Name;
        startDatePicker.Date = term.StartDate;
        endDatePicker.Date = term.EndDate;
    }

    private void Save_Clicked(object sender, EventArgs e)
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
            term.Name = termNameEntry.Text;
            term.StartDate = startDatePicker.Date;
            term.EndDate = endDatePicker.Date;
            MainPage.database.Update(term);
            Navigation.PushAsync(new MainPage());
        }
        catch (SQLite.SQLiteException ex)
        {
            DisplayAlert("Error", "Term name must be unique | " + ex.Message, "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error",ex.Message, "OK");
        }
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}