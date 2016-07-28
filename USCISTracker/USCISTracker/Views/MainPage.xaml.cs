using System;
using USCISTracker.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using USCISTracker.Data;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

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
            var res = await dialog.ShowAsync();

            //Check if we actually have changes.

            if(res == ContentDialogResult.Primary)
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
            DetailCaseNameTextBox.IsReadOnly = false;
            DetailCaseNameTextBox.Focus(FocusState.Pointer);
        }


        /// <summary>
        /// Click event handler for the sync all button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckCaseStatusAppBarButton_Click(object sender, RoutedEventArgs a)
        {
            //Disable the button
            (sender as AppBarButton).IsEnabled = false;

            //Set ring to active
            MasterProgressRing.IsActive = true;

            //List item unclickable during update
            CasesListView.IsItemClickEnabled = false;

            try
            {
                await ViewModel.SyncAllCaseAsync();
            }
            
            catch (Exception e)
            {
               ContentDialog contentDialog = new Windows.UI.Xaml.Controls.ContentDialog();
                contentDialog.Title = "Whoops! Something is wrong :(";
                contentDialog.Content = $"{e.ToString()}";
                contentDialog.PrimaryButtonText = "Send Crash Report";
                contentDialog.SecondaryButtonText = "Dismiss";
                contentDialog.FullSizeDesired = true;

                var result = await contentDialog.ShowAsync();

                if(result == ContentDialogResult.Primary)
                {
                    //Get the current app version
                    var packageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
                    string version = $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";

                    string mailingUrl = $"code.scottle@outlook.com?subject=[UT][v{version}]{e.GetType().FullName}&body= ExceptionType: {e.GetType().FullName}\n Message: {e.Message}\n App Version: {version}\n Details: {e.ToString()}";

                    Uri mailingUri = new Uri($"mailto:{mailingUrl}");

                    await Windows.System.Launcher.LaunchUriAsync(mailingUri);
                    await contentDialog.ShowAsync();
                }
            }

            finally
            {
                //re-enable the button
                (sender as AppBarButton).IsEnabled = true;

                //List item clickable
                CasesListView.IsItemClickEnabled = true;

                //SEt ring to inactive
                MasterProgressRing.IsActive = false;
            }
            
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


        /// <summary>
        /// Event handler on holding event of the ListView's Item's Grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }


        /// <summary>
        /// Event handler on righttapped event of the ListView's Item's Grid. Only respond to non-touch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if(e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
        }


        /// <summary>
        /// Event handler for the delete case flyout button click. Delete the current selected case.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCaseFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            //Take the receipt number
            var receipt = ((sender as Button).DataContext as Case).ReceiptNumber;

            //Delete it
            ViewModel.DeleteCase(receipt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EditCaseFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            //Get the Case Receipt Number and Name
            string CaseName = ((sender as Button).DataContext as Case).Name;
            string CaseReceiptNumber = ((sender as Button).DataContext as Case).ReceiptNumber.ReceiptNumber;

            //Call the editor
            CaseEditorContentDialog editor = new CaseEditorContentDialog() {
                caseName = CaseName,
                receiptNumber = CaseReceiptNumber
            };

            var result = await editor.ShowAsync();

            //check if saved
            if(result == ContentDialogResult.Primary)
            {
                //Check name change
                if (((sender as Button).DataContext as Case).Name != editor.CaseName)
                {
                    ((sender as Button).DataContext as Case).Name = editor.CaseName;
                }

                //Check if receipt number change, trigger refresh.
                if (((sender as Button).DataContext as Case).ReceiptNumber.ReceiptNumber != editor.ReceiptNumber)
                {
                    ((sender as Button).DataContext as Case).ReceiptNumber = new CaseReceiptNumber(editor.ReceiptNumber);

                    await ViewModel.SyncCaseStatusAsync(((sender as Button).DataContext as Case));

                    //force save data if refresh
                    await ViewModel.BackupCasesAsync();
                }

                
            }
        }
    }
}
