using System;
using USCISTracker.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using USCISTracker.Data;
using System.Threading.Tasks;

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
                ViewModel.SelectedCase = e.ClickedItem as Case;
                ViewModel.GotoDetailsPage();
            }

            else
            {
                ViewModel.SelectedCase = e.ClickedItem as Case;
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
                //Disable Sync and add button
                AddNewCaseAppBarButton.IsEnabled = false;
                CheckCaseStatusAppBarButton.IsEnabled = false;

                //Turn on the Progress Ring
                MasterProgressRing.IsActive = true;

                await ViewModel.AddNewCaseAsync(dialog.receiptNumber, dialog.caseName);


                //Enable Sync and add button
                AddNewCaseAppBarButton.IsEnabled = true;
                CheckCaseStatusAppBarButton.IsEnabled = true;

                //Turn off the Progress Ring
                MasterProgressRing.IsActive = false;
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


        /// <summary>
        /// Click event handler for the sync all button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckCaseStatusAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //Disable the button
            (sender as AppBarButton).IsEnabled = false;

            //Set ring to active
            MasterProgressRing.IsActive = true;

            //List item unclickable during update
            CasesListView.IsItemClickEnabled = false;

            await ViewModel.SyncAllCaseAsync();

            //re-enable the button
            (sender as AppBarButton).IsEnabled = true;

            //List item clickable
            CasesListView.IsItemClickEnabled = true;

            //SEt ring to inactive
            MasterProgressRing.IsActive = false;
        }



        /// <summary>
        /// Page Loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainPageActual_Loaded(object sender, RoutedEventArgs e)
        {

            //disable button to indicate loading
            AddNewCaseAppBarButton.IsEnabled = false;
            CheckCaseStatusAppBarButton.IsEnabled = false;

            if(ViewModel != null && ViewModel.IsInitialized == false)
            {
                //try to load the cases
                await ViewModel.LoadBackupCasesAsync();

                //Done loading switch it back
                ViewModel.IsInitialized = true;
            }
          
            //reload
            AddNewCaseAppBarButton.IsEnabled = true;
            CheckCaseStatusAppBarButton.IsEnabled = true;
        }
    }
}
