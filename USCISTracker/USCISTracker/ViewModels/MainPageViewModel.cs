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

namespace USCISTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        #region Fields
        private ObservableCollection<ICase> cases;
        #endregion

        #region Properties

        /// <summary>
        /// Collection of cases that the user is current tracking
        /// </summary>
        public ObservableCollection<ICase> Cases
        {
            get
            {
                return cases;
            }

            private set
            {
                cases = value;
            }
        }
        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor for the view model
        /// </summary>
        public MainPageViewModel()
        {

            Cases = new ObservableCollection<ICase>();

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
            Session currentSession = new Session();
            await currentSession.ConnectAsync();
            await currentSession.SetReceiptNumberAsync(receiptNumber);
            await currentSession.CheckCaseStatusAsync();
            string html = await currentSession.GetCurrentPageHTML();

            ICase testCase = await Case.GenerateFromHTMLAsync(html, new CaseReceiptNumber(receiptNumber));
            testCase.LastRefresh = DateTime.Now;

            if (string.IsNullOrEmpty(caseName))
            {
                testCase.Name = $"Case #{Cases.Count + 1}";
            }

            else
            {
                testCase.Name = caseName;
            }
            

            Cases.Add(testCase);
        }


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

                string tempname = Cases[i].Name;
                Cases[i] = await Case.GenerateFromHTMLAsync(html, Cases[i].ReceiptNumber);
                Cases[i].LastRefresh = DateTime.Now;
                Cases[i].Name = tempname;
            }
        }

        #endregion

        #region Template 10 Events Handlers and Commands
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
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

        public void GotoDetailsPage(ICase selectedCase) =>
            NavigationService.Navigate(typeof(Views.DetailPage), selectedCase, null);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        #endregion
    }
}

