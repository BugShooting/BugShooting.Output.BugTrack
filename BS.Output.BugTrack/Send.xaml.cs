using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace BS.Output.BugTrack
{
  partial class Send : Window
  {
 
    public Send(string url, int lastEntryID, string fileName)
    {
      InitializeComponent();
    
      Url.Text = url;
      NewEntry.IsChecked = true;
      EntryIDTextBox.Text = lastEntryID.ToString();
      FileNameTextBox.Text = fileName;
           
      EntryIDTextBox.TextChanged += ValidateData;
      FileNameTextBox.TextChanged += ValidateData;
      ValidateData(null, null);

    }

    public bool CreateNewEntry
    {
      get { return NewEntry.IsChecked.Value; }
    }
 
    public int EntryID
    {
      get { return Convert.ToInt32(EntryIDTextBox.Text); }
    }

    public string FileName
    {
      get { return FileNameTextBox.Text; }
    }

    private void EntryID_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
    }

    private void EntryIDTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      AttachToEntry.IsChecked = true;
      EntryIDTextBox.SelectAll();
    }

    private void Type_Changed(object sender, RoutedEventArgs e)
    {

      if (CreateNewEntry)
      {
        EntryIDTextBox.SetValue(Validation.RequiredProperty, false);
      }
      else { 
        EntryIDTextBox.SetValue(Validation.RequiredProperty, true);
        EntryIDTextBox.Focus();
      }

      ValidateData(null, null);
    }


    private void ValidateData(object sender, EventArgs e)
    {
      OK.IsEnabled = Validation.IsValid(EntryIDTextBox) &&
                     Validation.IsValid(FileNameTextBox);
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

  }

}
