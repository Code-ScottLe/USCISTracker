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

namespace USCISTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        #region Fields
        private ObservableCollection<Case> cases;
        private Case selectedCase;
        private string savedFileName = "USCISCasesJSON.json";
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
        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor for the view model
        /// </summary>
        public MainPageViewModel()
        {

            Cases = new ObservableCollection<Case>();

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
            //Create a new case with Case Factory
            Case testCase = new Case(receiptNumber, caseName);

            //In Case of empty case name.
            if (string.IsNullOrEmpty(caseName))
            {
                testCase.Name = $"Case #{Cases.Count + 1}";
            }

            //Create a session
            Session currentSession = new Session();
            await currentSession.ConnectAsync();
            await currentSession.SetReceiptNumberAsync(testCase.ReceiptNumber.ReceiptNumber);
            await currentSession.CheckCaseStatusAsync();
            string html = await currentSession.GetCurrentPageHTML();

            //Update the case with the respond HTML
            await testCase.UpdateFromHTMLAsync(html);         

            Cases.Add(testCase);

            //Back up
            await BackupCasesAsync();

        }

        /// <summary>
        /// Sync all the current tracking cases
        /// </summary>
        /// <returns></returns>
        public async Task SyncAllCaseAsync()
        {
            //If no case to check, don't bother.
            if(Cases.Count == 0)
            {
                return;
            }

            //create a session at USCIS for update.
            Session currentSession = new Session();
            await currentSession.ConnectAsync();

            if(currentSession.NavigateFailed == true)
            {
                throw new OperationCanceledException("Session WebView has Failed!");
            }

            //loop through the entire thing.
            for(int i = 0; i < Cases.Count; i++)
            {
                await currentSession.SetReceiptNumberAsync(Cases[i].ReceiptNumber.ReceiptNumber);
                await currentSession.CheckCaseStatusAsync();
                string html = await currentSession.GetCurrentPageHTML();

                await Cases[i].UpdateFromHTMLAsync(html);
            }

           
          
        }


        /// <summary>
        /// Back up all the tracking cases info into the JSON file
        /// </summary>
        /// <returns></returns>
        private async Task BackupCasesAsync()
        {
            if(Cases.Count == 0)
            {
                return;
            }

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
        /// 
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

        #endregion

        #region Template 10 Events Handlers and Commands
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
            }

            if(App.passThrough != null && (App.passThrough as Case) != null)
            {
                var detailCase = (App.passThrough as Case);

                if(detailCase.Name != SelectedCase.Name)
                {
                    SelectedCase.Name = detailCase.Name;
                }

                App.passThrough = null;
            }

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
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

