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
        private Case currentCase;
        private bool isNameChanged = false;
        #endregion

        #region Properties
        /// <summary>
        /// Current displayed case.
        /// </summary>
        public Case CurrentCase
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

        public bool IsNameChanged
        {
            get
            {
                return isNameChanged;
            }

            set
            {
                isNameChanged = value;
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

            //Fake Receipt Number
            //CurrentCase = CaseFactory.GetCase("YSC1600000000");
        }


        #endregion
        //private string _Value = "Default";
        //public string Value { get { return _Value; } set { Set(ref _Value, value); } }


        #region Methods
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
           
            CurrentCase = (Case)parameter;

            //(CurrentCase as Case).Details = caseme.Details;
            //(CurrentCase as Case).FormType = caseme.FormType;
            //(CurrentCase as Case).LastCaseUpdate = caseme.LastCaseUpdate;
            //(CurrentCase as Case).LastRefresh = caseme.LastRefresh;
            //(CurrentCase as Case).Name = caseme.Name;
            //(CurrentCase as Case).ReceiptNumber = caseme.ReceiptNumber;
            //(CurrentCase as Case).Status = caseme.Status;

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
            App.passThrough = CurrentCase;
            args.Cancel = false;
            await Task.CompletedTask;
        }
       
        #endregion
    }
}

