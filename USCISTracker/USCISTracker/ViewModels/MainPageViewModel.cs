using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors

        #endregion

        #region Methods

        /// <summary>
        /// Helper method to fire the Property Changed
        /// </summary>
        /// <param name="propertyName">the name of the changed property</param>
        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
