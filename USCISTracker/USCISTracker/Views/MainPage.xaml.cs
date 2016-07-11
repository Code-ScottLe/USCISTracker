using System;
using USCISTracker.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using USCISTracker.Data;

namespace USCISTracker.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        /// <summary>
        /// Event Handler for the Item Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CasesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Check whether if we are in narrow state or normal/extend state
            if (AdaptiveVisualStateGroup.CurrentState == VisualStateNarrow)
            {
                //Narrow state, we navigate to the detail page.
                ViewModel.SelectedCase = e.ClickedItem as ICase;
                ViewModel.GotoDetailsPage();
            }

            else
            {
                ViewModel.SelectedCase = e.ClickedItem as ICase;
            }
        }


        /// <summary>
        /// Click event handler for the Add A Newcase App bar Button. Add a new case to track.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddNewCaseAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //Add a new case on the content dialog
            CaseCreatorContentDialog dialog = new CaseCreatorContentDialog();
            await dialog.ShowAsync();

            //Check if we actually have changes.

            if(dialog.isCancelled != true)
            {
                await ViewModel.AddNewCaseAsync(dialog.receiptNumber, dialog.caseName);
            }

            
        }

        /// <summary>
        /// Click event handler for the edit the name button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCaseNameButton_Click(object sender, RoutedEventArgs e)
        {
            CaseNameTextBox.IsReadOnly = false;
            CaseNameTextBox.Focus(FocusState.Pointer);
        }
    }
}
