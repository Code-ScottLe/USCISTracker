using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace USCISTracker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            API.Session mySession = null;


            mySession = new API.Session();

            await mySession.ConnectAsync();


            await mySession.SetReceiptNumberAsync("YSC1690058904");

            await mySession.CheckCaseStatusAsync();

            string stuffs = await mySession.GetCurrentPageHTML();

            Data.CaseReceiptNumber receipt = new Data.CaseReceiptNumber("YSC1690058904");

            await Data.Case.GenerateFromHTMLAsync(stuffs, receipt);

        }
    }
}
