using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace USCISTracker.Views
{
    public sealed partial class CaseEditorContentDialog : ContentDialog, INotifyPropertyChanged
    {
        public string receiptNumber;
        public string caseName;
        private Brush defaultBorderColor;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ReceiptNumber
        {
            get
            {
                return receiptNumber;
            }

            set
            {
                receiptNumber = value;
                OnPropertyChanged("ReceiptNumber");
            }
        }

        public string CaseName

        {
            get
            {
                return caseName;
            }

            set
            {
                caseName = value;
                OnPropertyChanged("CaseName");
            }
        }

        public CaseEditorContentDialog()
        {
            this.InitializeComponent();
            defaultBorderColor = ReceiptNumberTextBox.BorderBrush;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            receiptNumber = ReceiptNumberTextBox.Text;
            caseName = CaseNameTextBox.Text;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        /// <summary>
        /// Respond to the event that text of the receipt number text box is being changed. This will enable the save button when reached 13 characters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ReceiptNumberTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if ((sender as TextBox).Text.Length == 13)
            {
                IsPrimaryButtonEnabled = true;
            }

            else
            {
                IsPrimaryButtonEnabled = false;
            }
        }


        /// <summary>
        /// Respond to the vent that text of the receipt number text box has changed. Will warn user if the user mistyped the receipt number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiptNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length != 13)
            {
                (sender as TextBox).BorderBrush = new SolidColorBrush() { Color = Windows.UI.Colors.Red };
            }

            else
            {
                (sender as TextBox).BorderBrush = defaultBorderColor;
            }
        }


        /// <summary>
        /// Fire up the on propertychanged event
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
