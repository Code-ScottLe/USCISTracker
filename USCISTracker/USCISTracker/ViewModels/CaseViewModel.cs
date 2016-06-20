using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USCISTracker.API;
using USCISTracker.Data;


namespace USCISTracker.ViewModels
{
    public class CaseViewModel : INotifyPropertyChanged
    {

        #region Fields
        private ObservableCollection<ICase> cases;
        #endregion

        #region Property
        public ObservableCollection<ICase> Cases
        {
            get
            {
                return cases;
            }
        }
        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// PUblic Constructors
        /// </summary>
        public CaseViewModel()
        {

        }
        #endregion

        #region Methods


        /// <summary>
        /// Helper method to fire the Property Changed
        /// </summary>
        /// <param name="propertyName">the name of the changed property</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #endregion

    }
}
