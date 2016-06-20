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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace USCISTracker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MasterDetailPage : Page
    {
        public MasterDetailPage()
        {
            this.InitializeComponent();
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {

        }

        private async void AddCaseAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.TestAsync();
        }


        /// <summary>
        /// Event handler for the event when an item on the master listview is choosen/clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //get the clicked item
            var choosenCase = e.ClickedItem as Data.ICase;

            if(AdaptiveStates.CurrentState == NarrowState)
            {
                //Narrow state, transit to the detail page
            }

            else
            {
                //Show the item on the detail section
                DetailContentPresenter.ContentTransitions.Clear();
                DetailContentPresenter.ContentTransitions.Add(new EntranceThemeTransition());
            }
        }
    }
}
