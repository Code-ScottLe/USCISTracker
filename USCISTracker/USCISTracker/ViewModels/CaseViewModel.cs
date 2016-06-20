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
        private ICase selectedItem;
        #endregion

        #region Property

        /// <summary>
        /// The list of all currently tracked case.
        /// </summary>
        public ObservableCollection<ICase> Cases
        {
            get
            {
                return cases;
            }
        }

        /// <summary>
        /// Current selected item
        /// </summary>
        public ICase SelectedItem
        {
            get
            {
                return selectedItem;
            }

            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
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
            cases = new ObservableCollection<ICase>();
            
        }
        #endregion

        #region Methods

        public async Task TestAsync()
        {
            Session mySession = new Session();
            await mySession.ConnectAsync();

            //Receipt
            ICaseReceiptNumber receipt = new CaseReceiptNumber("YSC1690058904");

            await mySession.SetReceiptNumberAsync(receipt.ReceiptNumber);

            await mySession.CheckCaseStatusAsync();

            var html = await mySession.GetCurrentPageHTML();

            Case myCase = await Case.GenerateFromHTMLAsync(html, receipt);

            cases.Add(myCase);
        }

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
