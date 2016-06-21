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
using USCISTracker.ViewModels;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace USCISTracker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        //Dependencies stuffs
        private static DependencyProperty caseProperty = DependencyProperty.Register("Case", typeof(CaseViewModel), typeof(DetailPage), new PropertyMetadata(null));

        public static DependencyProperty CaseProperty
        {
            get
            {
                return caseProperty;
            }
        }

        public CaseViewModel Case
        {
            get
            {
                return GetValue(caseProperty) as CaseViewModel;
            }

            set
            {
                SetValue(caseProperty, value);
            }
        }

        public DetailPage()
        {
            this.InitializeComponent();

            //Register the back button
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += SettingPage_BackRequested;
        }

        /// <summary>
        /// Event handler for the back pressed button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            //Get the current frame that is being displayed in the current window.
            Frame currentFrame = Window.Current.Content as Frame;

            if (currentFrame == null)
            {
                //something wrong
                return;
            }

            //Handled property indicate if the back button has been handled or not.
            //We only handle it here if it is not already handled (usually by the system)
            if (currentFrame.CanGoBack == true && e.Handled == false)
            {
                //set the event as already being handled
                e.Handled = true;

                currentFrame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            mainStackPanel.DataContext = e.Parameter as CaseViewModel;

            Binding stuffs = new Binding();
            stuffs.ElementName = "SelectedItem";
            stuffs.Path = new PropertyPath("Status");

            DetailedCaseStatus.SetBinding(TextBlock.TextProperty, stuffs);

        }
    }


}
