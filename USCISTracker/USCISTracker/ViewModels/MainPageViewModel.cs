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
        public MainPageViewModel()
        {

            Cases = new ObservableCollection<ICase>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
            }
        }

        #endregion


        #region Methods


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

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), null);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);


        #endregion
    }
}

