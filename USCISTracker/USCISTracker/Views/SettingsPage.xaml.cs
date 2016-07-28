using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Background;
using USCISTracker.Services.BackgroundServices;
using USCISTracker.Configurations;

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


        /// <summary>
        /// Event handler for the event of the setting is on the current screen.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var index = int.Parse(_SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
            MyPivot.SelectedIndex = index;


            //check if we have register background task
            EnableBackgroundUpdateToggleSwitch.IsOn = BackgroundService.IsTaskRegistered(BackgroundTasksConfiguration.CaseUpdateBackgroundTaskName);

            //register the handler if we have the background task.
            if(BackgroundService.IsTaskRegistered(BackgroundTasksConfiguration.CaseUpdateBackgroundTaskName) == true)
            {
                BackgroundService.GetBackgroundTask(BackgroundTasksConfiguration.CaseUpdateBackgroundTaskName).Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
            }

            if(BackgroundService.IsTaskRegistered(BackgroundTasksConfiguration.ServiceCompletedBackgroundTaskName) == true)
            {
                BackgroundService.GetBackgroundTask(BackgroundTasksConfiguration.ServiceCompletedBackgroundTaskName).Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
            }

            if (BackgroundService.IsTaskRegistered(BackgroundTasksConfiguration.TileUpdateBackgroundTaskName) == true)
            {
                BackgroundService.GetBackgroundTask(BackgroundTasksConfiguration.TileUpdateBackgroundTaskName).Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
            }

            //Update the text box if we have result from previous running background task
            UpdateUIAsync();
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
            //On = register
            if((sender as ToggleSwitch).IsOn == true)
            {
                if (BackgroundService.IsTaskRegistered(BackgroundTasksConfiguration.CaseUpdateBackgroundTaskName) == false)
                {
                    //Register it.
                    var backgroundTask = await BackgroundService.RegisterBackgroundTask(BackgroundTasksConfiguration.CaseUpdateBackgroundTaskName, 
                        BackgroundTasksConfiguration.CaseUpdateBackgroundTaskEntryPoint,
                        new TimeTrigger(BackgroundTasksConfiguration.TimeUpdateInterval, false), new SystemCondition(SystemConditionType.InternetAvailable));

                    //hook up complete handler
                    backgroundTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                }

                if(BackgroundService.IsTaskRegistered(BackgroundTasksConfiguration.TileUpdateBackgroundTaskName) == false)
                {
                    //Register tile update
                    var backgroundTask = await BackgroundService.RegisterBackgroundTask(BackgroundTasksConfiguration.TileUpdateBackgroundTaskName,
                        BackgroundTasksConfiguration.TileUpdateBackgroundTaskEntryPoint,
                        new TimeTrigger(BackgroundTasksConfiguration.TimeUpdateInterval, false));

                    //Hook up complete handler
                    backgroundTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                }

                //If we now have the background task, also enable background task on event of app update to make sure everything is still being registered
                if(BackgroundService.IsTaskRegistered(BackgroundTasksConfiguration.ServiceCompletedBackgroundTaskName) == false)
                {
                    var backgroundTask = await BackgroundService.RegisterBackgroundTask(BackgroundTasksConfiguration.ServiceCompletedBackgroundTaskName,
                        BackgroundTasksConfiguration.ServiceCompletedBackgroundTaskEntryPoint,
                        new SystemTrigger(SystemTriggerType.ServicingComplete, false));

                    //hoop up complete handler
                    backgroundTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                }
            }

            //Off = unregister
            else
            {
                BackgroundService.UnregisterBackgroundTask(BackgroundTasksConfiguration.CaseUpdateBackgroundTaskName);
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
#if DEBUG
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                //get the setting
                var setting = Windows.Storage.ApplicationData.Current.LocalSettings;

                //get the last update info from the setting if we have it.
                object lastTaskRun = null;

                if(setting.Values.TryGetValue(BackgroundTasksConfiguration.CaseUpdateBackgroundTaskName, out lastTaskRun) == true)
                {
                    BackgroundTaskLastRunTextBox.Text = lastTaskRun as string;
                }

                //get the last app update if we have it
                object lastAppUpdate = null;

                if(setting.Values.TryGetValue(BackgroundTasksConfiguration.ServiceCompletedBackgroundTaskName, out lastAppUpdate) == true)
                {
                    LastAppUpdateTextBox.Text = lastAppUpdate as string;
                }

                //Get the tile update
                object lastTileUpdate = null;
                if(setting.Values.TryGetValue(BackgroundTasksConfiguration.TileUpdateBackgroundTaskName, out lastTileUpdate) == true)
                {
                    LastTileUpdateTextBox.Text = lastTileUpdate as string;
                }
                
            });

#endif
        }
    }
}
