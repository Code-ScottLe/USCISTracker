using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace USCISTracker.Views
{
    public sealed partial class SettingsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;

        public SettingsPage()
        {
            InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var index = int.Parse(_SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
            MyPivot.SelectedIndex = index;
        }


        /// <summary>
        /// Event handler for the DeleteCacheButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteCacheButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //Re-Confirm if user want to delete
            ContentDialog dialog = new ContentDialog()
            {
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Delete",
                SecondaryButtonText = "Cancel",
                Title = "Are you sure that you want to clear the cache?"           
            };

            //show the content
            var result = await dialog.ShowAsync();

            if(result == ContentDialogResult.Primary)
            {
                //clear the cache.
                await ViewModel.SettingsPartViewModel.ClearCacheAsync();
            }
        }
    }
}
