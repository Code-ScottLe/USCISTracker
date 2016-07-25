using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Background;
using USCISTracker.Services.BackgroundServices;

namespace USCISTracker.Views
{
    public sealed partial class SettingsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;

        public SettingsPage()
        {
            InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var index = int.Parse(_SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
            MyPivot.SelectedIndex = index;


            //check if we have register background task
            EnableBackgroundUpdateToggleSwitch.IsOn = BackgroundService.IsTaskRegistered("CaseUpdateBackgroundTask");

            //register the handler if we have the background task.
            if(BackgroundService.IsTaskRegistered("CaseUpdateBackgroundTask") == true)
            {
                BackgroundService.GetBackgroundTask("CaseUpdateBackgroundTask").Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
            }
        }


        /// <summary>
        /// Event handler for the DeleteCacheButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteCacheButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //Re-Confirm if user want to delete
            ContentDialog dialog = new ContentDialog()
            {
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Delete",
                SecondaryButtonText = "Cancel",
                Title = "Are you sure that you want to clear the cache?"           
            };

            //show the content
            var result = await dialog.ShowAsync();

            if(result == ContentDialogResult.Primary)
            {
                //clear the cache.
                await ViewModel.SettingsPartViewModel.ClearCacheAsync();
            }
        }


        /// <summary>
        /// Event Handler for the toggled background task register.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EnableBackgroundUpdateToggleSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if((sender as ToggleSwitch).IsOn == true)
            {
                if (BackgroundService.IsTaskRegistered("CaseUpdateBackgroundTask") == false)
                {
                    //Register it.
                    var backgroundTask = await BackgroundService.RegisterBackgroundTask("CaseUpdateBackgroundTask", "USCISTracker.Background.CaseUpdateBackgroundTask",
                        new TimeTrigger(15, false), new SystemCondition(SystemConditionType.InternetAvailable));

                    //hook up complete handler
                    backgroundTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                }
            }

            else
            {
                BackgroundService.UnregisterBackgroundTask("CaseUpdateBackgroundTask");
            }

            UpdateUIAsync();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="args"></param>
        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            UpdateUIAsync();
        }


        /// <summary>
        /// Enable upadting the UI from non-UI thread
        /// </summary>
        private async void UpdateUIAsync()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                //get the setting
                var setting = Windows.Storage.ApplicationData.Current.LocalSettings;

                //get the last update info from the setting if we have it.
                object lastTaskRun = null;

                if(setting.Values.TryGetValue("CaseUpdateBackgroundTask", out lastTaskRun) == true)
                {
                    BackgroundTaskLastRunTextBox.Text = lastTaskRun as string;
                }
                
            });
        }
    }
}
