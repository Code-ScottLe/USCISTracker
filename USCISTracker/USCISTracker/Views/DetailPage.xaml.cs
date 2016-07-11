using USCISTracker.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace USCISTracker.Views
{
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void EditCaseNameButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CaseNameTextBox.IsReadOnly = false;
            CaseNameTextBox.Focus(Windows.UI.Xaml.FocusState.Pointer);
        }
    }
}

