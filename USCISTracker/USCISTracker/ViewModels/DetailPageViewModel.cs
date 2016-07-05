using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using USCISTracker.Data;

namespace USCISTracker.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {

        #region Fields
        private ICase currentCase;
        #endregion

        #region Properties
        /// <summary>
        /// Current displayed case.
        /// </summary>
        public ICase CurrentCase
        {
            get
            {
                return currentCase;
            }

            set
            {
                currentCase = value;
                RaisePropertyChanged("CurrentCase");
            }
        }
        #endregion

        #region Events

        #endregion

        #region Constructors

        public DetailPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
               // Value = "Designtime value";
            }
        }


        #endregion
        //private string _Value = "Default";
        //public string Value { get { return _Value; } set { Set(ref _Value, value); } }


        #region Methods
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();

            CurrentCase = parameter as ICase;
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                //suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }
       
        #endregion
    }
}

