using System;
using USCISTracker.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using USCISTracker.Data;

namespace USCISTracker.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        /// <summary>
        /// Event Handler for the Item Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CasesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Check whether if we are in narrow state or normal/extend state
            if (AdaptiveVisualStateGroup.CurrentState == VisualStateNarrow)
            {
                //Narrow state, we navigate to the detail page.
                ViewModel.GotoDetailsPage(CasesListView.SelectedItem as ICase);
            }
        }
    }
}
