using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using USCISTracker.API;
using USCISTracker.Data;
using System.ComponentModel;
using Newtonsoft.Json;
using Windows.Storage;
using USCISTracker.Services.TileServices;
using Windows.UI.Notifications;
using USCISTracker.Services.BackgroundServices;
using USCISTracker.Configurations;
using Windows.ApplicationModel.Background;

namespace USCISTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        #region Fields
        private ObservableCollection<Case> cases;
        private Case selectedCase;
        private string savedFileName = "USCISCasesJSON.json";
        private bool isCaseUpdating = false;
        #endregion

        #region Properties

        /// <summary>
        /// Collection of cases that the user is current tracking
        /// </summary>
        public ObservableCollection<Case> Cases
        {
            get
            {
                return cases;
            }

            private set
            {
                cases = value;
                RaisePropertyChanged("Cases");
            }
        }

        /// <summary>
        /// Current selected case
        /// </summary>
        public Case SelectedCase
        {
            get
            {
                return selectedCase;
            }

            set
            {
                selectedCase = value;
                RaisePropertyChanged("SelectedCase");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsCaseUpdating
        {
            get
            {
                return isCaseUpdating;
            }

            set
            {
                isCaseUpdating = value;
                RaisePropertyChanged("IsCaseUpdating");
            }
        }


        /// <summary>
        /// Set if the viewModel was initialized or not. Default is False.
        /// </summary>
        public bool IsInitialized
        {
            get;set;
        }

        /// <summary>
        /// The counter for the amount of cases that were updated when run the all case updates.
        /// </summary>
        private int CasesUpdatedCounter
        {
            get;set;
        }
        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor for the view model
        /// </summary>
        public MainPageViewModel()
        {

            Cases = new ObservableCollection<Case>();
            IsInitialized = false;

            SelectedCase = new Case();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
            }
        }

        #endregion


        #region Methods


        /// <summary>
        /// Create a case and get it status 
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="caseName"></param>
        public async Task AddNewCaseAsync(string receiptNumber, string caseName ="")
        {
            IsCaseUpdating = true;

            //Create a new case with Case Factory
            Case testCase = new Case(receiptNumber, caseName);

            //In Case of empty case name.
            if (string.IsNullOrEmpty(caseName))
            {
                testCase.Name = $"Case #{Cases.Count + 1}";
            }

            //Check for status

            await SyncCaseStatusAsync(testCase);

            Cases.Add(testCase);

            //Back up
            await BackupCasesAsync();

            IsCaseUpdating = false;


           
        }

        /// <summary>
        /// Sync all the current tracking cases
        /// </summary>
        /// <returns></returns>
        public async Task SyncAllCaseAsync(int overrideCounter = 0)
        {
            //If no case to check, don't bother.
            if(Cases.Count == 0)
            {
                return;
            }

            IsCaseUpdating = true;


            //create a session at USCIS for update.
            Session currentSession = new Session();

            for (int i = overrideCounter; i < Cases.Count; i++)
            {

                await SyncCaseStatusAsync(Cases[i], currentSession);
                CasesUpdatedCounter = i;

            }


            UpdateSelectedCase();
            

            IsCaseUpdating = false;

        }



        /// <summary>
        /// Check and update individual case
        /// </summary>
        /// <param name="checkingCase"> current case to check for status</param>
        /// <param name="currentSession"> connected sesion if we have one</param>
        /// <returns></returns>
        public async Task<Case> SyncCaseStatusAsync(Case checkingCase, Session currentSession = null)
        {
            //Check if we have to create session
            Session localSession = currentSession;
            if(localSession == null)
            {
                localSession = new Session();
            }

            //Check 
            string html = await localSession.CheckCaseStatusAsync(checkingCase.ReceiptNumber.ReceiptNumber);
            await checkingCase.UpdateFromHTMLAsync(html);

            return checkingCase;

        }

        /// <summary>
        /// Back up all the tracking cases info into the JSON file
        /// </summary>
        /// <returns></returns>
        public async Task BackupCasesAsync()
        {

            //serialize the collection
            string json = await Task.Run<string>(() => JsonConvert.SerializeObject(Cases, Formatting.Indented));

            //Get the local folder.
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            //Get the file, if not, create it.
            StorageFile localCaseFile = await localFolder.TryGetItemAsync(savedFileName) as StorageFile;

            if(localCaseFile == null)
            {
                localCaseFile = await localFolder.CreateFileAsync(savedFileName);
            }

            //Write to the file
            await FileIO.WriteTextAsync(localCaseFile, json);

        }


        /// <summary>
        /// We load all the cases from the backup JSON
        /// </summary>
        /// <returns></returns>
        public async Task LoadBackupCasesAsync()
        {
            //Get the local folder.
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            //Get the file, if not, create it.
            StorageFile localCaseFile = await localFolder.TryGetItemAsync(savedFileName) as StorageFile;

            if(localCaseFile == null)
            {
                return;
            }

            else
            {
                //read from the file and deserialize it
                string json = await FileIO.ReadTextAsync(localCaseFile);

                Cases = await Task.Run<ObservableCollection<Case>>(() => JsonConvert.DeserializeObject<ObservableCollection<Case>>(json));
            }
        }


        /// <summary>
        /// Delete the given case on the receipt Number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public void DeleteCase(CaseReceiptNumber receiptNumber)
        {
            //find the case in the list
            var toBeDeletedCase = Cases.Where(n => n.ReceiptNumber.ReceiptNumber == receiptNumber.ReceiptNumber).Select(n => n).FirstOrDefault();

            if(toBeDeletedCase != null)
            {
                Cases.Remove(toBeDeletedCase);
            }
        }


        /// <summary>
        /// Update the main tile with case status with most recent updated cases (5)
        /// </summary>
        private void UpdateTile()
        {
            List<Case> tempCaseList = Cases.ToList();

            //Remove all the irrelavent case
            while (tempCaseList.Count > 5)
            {
                var older = tempCaseList.Min((s) => { return s.LastRefresh.ToFileTime(); });

                var olderCase = tempCaseList.Where(n => n.LastRefresh.ToFileTime() == older).Select(n => n).FirstOrDefault();
                tempCaseList.Remove(olderCase);
            }

            //Create the tile update
            List<NotificationsExtensions.Tiles.TileContent> tileUpdates = new List<NotificationsExtensions.Tiles.TileContent>();

            foreach(Case individualCase in tempCaseList)
            {
                tileUpdates.Add(TileService.CreateAdaptiveMainTileContent(individualCase));
            }

            //Call the updater
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();

            //Enable the queue
            updater.EnableNotificationQueue(true);

            //remove old 
            updater.Clear();

            //push it.
            foreach( var tileUpdate in tileUpdates)
            {
                updater.Update(new TileNotification(tileUpdate.GetXml()));
            }


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="args"></param>
        private async void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            //Reload the current cases
            await LoadBackupCasesAsync();
            UpdateSelectedCase();
        }


        /// <summary>
        /// Update the selected case to reflect the change in the detail view
        /// </summary>
        /// <returns></returns>
        private void UpdateSelectedCase()
        {
            if(SelectedCase.ReceiptNumber != null)
            {
                //Get the updated version
                var updatedCase = Cases.Where(n => n.ReceiptNumber.ReceiptNumber == SelectedCase.ReceiptNumber.ReceiptNumber).Select(n => n).FirstOrDefault();
                SelectedCase = updatedCase;
            }
            
        }


        #endregion

        #region Template 10 Events Handlers and Commands
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //register the handler if we have the background task.
            if (BackgroundService.IsTaskRegistered(BackgroundTasksConfiguration.CaseUpdateBackgroundTaskName) == true)
            {
                BackgroundService.GetBackgroundTask(BackgroundTasksConfiguration.CaseUpdateBackgroundTaskName).Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
            }

            if (suspensionState.Any())
            {
                //Load up if we previously suspensded
                await LoadBackupCasesAsync();

                //Check if we previously syncing cases.
                if(suspensionState.ContainsKey("CasesUpdatedCounter") == true)
                {
                    CasesUpdatedCounter = (int)suspensionState["CasesUpdatedCounter"];

                    //resume checking.
                    IsCaseUpdating = true;

                    await SyncAllCaseAsync(CasesUpdatedCounter);
                }
            }

            else
            {
                if (App.passThrough != null && (App.passThrough as Case) != null)
                {
                    var detailCase = (App.passThrough as Case);

                    if (detailCase.Name != SelectedCase.Name)
                    {
                        SelectedCase.Name = detailCase.Name;
                    }

                    App.passThrough = null;
                }
              
            }

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                //Save the update case index on the list if we are running the update
                if (IsCaseUpdating == true)
                {
                    suspensionState["CasesUpdatedCounter"] = CasesUpdatedCounter;
                }

                //Save what we have.
                await BackupCasesAsync();

                //refresh tile
                await Task.Run(() => { UpdateTile(); });

            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }


        /// <summary>
        /// Using navigation service to get to the Detail page of the selected case.
        /// </summary>
        public void GotoDetailsPage()
        {
            NavigationService.Navigate(typeof(Views.DetailPage), SelectedCase, null);
        }
            

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        #endregion
    }
}

