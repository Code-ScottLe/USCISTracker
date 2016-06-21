﻿using System;
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
        /// Handler for the click event of the Hamburger utton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HamburgerNavButton_Click(object sender, RoutedEventArgs e)
        {
            MasterNavSplitView.IsPaneOpen = !MasterNavSplitView.IsPaneOpen;
        }
    }
}
