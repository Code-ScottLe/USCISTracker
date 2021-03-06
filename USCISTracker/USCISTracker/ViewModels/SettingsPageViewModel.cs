using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.SettingsService;
using Windows.UI.Xaml;
using Windows.Storage;
using USCISTracker.Services.BackgroundServices;
using Windows.ApplicationModel.Background;
using System.Text;

namespace USCISTracker.ViewModels
{
    /// <summary>
    /// View Model for the Setting Page as a whole
    /// </summary>
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    /// <summary>
    /// View Model for the Pivot Item : Setting
    /// </summary>
    public class SettingsPartViewModel : ViewModelBase
    {

        #region Template10Crap
        Services.SettingsServices.SettingsService _settings;

        public SettingsPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
            }
        }

        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }

        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged(); }
        }

        private string _BusyText = "Please wait...";
        public string BusyText
        {
            get { return _BusyText; }
            set
            {
                Set(ref _BusyText, value);
                _ShowBusyCommand.RaiseCanExecuteChanged();
            }
        }

        DelegateCommand _ShowBusyCommand;
        public DelegateCommand ShowBusyCommand
            => _ShowBusyCommand ?? (_ShowBusyCommand = new DelegateCommand(async () =>
            {
                Views.Busy.SetBusy(true, _BusyText);
                await Task.Delay(5000);
                Views.Busy.SetBusy(false);
            }, () => !string.IsNullOrEmpty(BusyText)));

        #endregion

        /// <summary>
        /// Clear the local cases cache.
        /// </summary>
        /// <returns></returns>
        public async Task ClearCacheAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile cacheFile = await localFolder.TryGetItemAsync("USCISCasesJSON.json") as StorageFile;

            if(cacheFile != null)
            {
                await cacheFile.DeleteAsync();
            }
        }

        /// <summary>
        /// Report Exception via Email.
        /// </summary>
        /// <param name="e"></param>
        public async void ErrorReport(Exception e)
        {
            //Create the email to be sent.
            Windows.ApplicationModel.Email.EmailMessage emailMessage = new Windows.ApplicationModel.Email.EmailMessage();

            //Get the current app version
            var packageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            string version = $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";

            //Build the body.
            StringBuilder emailMessageBodyBuilder = new StringBuilder();
            emailMessageBodyBuilder.AppendLine($"Exception Type: {e.GetType().FullName}");
            emailMessageBodyBuilder.AppendLine($"Message: {e.Message}");
            emailMessageBodyBuilder.AppendLine($"App version: {version}");
            emailMessageBodyBuilder.AppendLine($"Detail: {e.ToString()}");

            //set the body:       
            emailMessage.Body = emailMessageBodyBuilder.ToString();

            //format the subject of the email (for inbox filtering)
            emailMessage.Subject = $"[UCS][v{version}]{e.GetType().FullName} ";

            //set the sender.
            emailMessage.To.Add(new Windows.ApplicationModel.Email.EmailRecipient("code.scottle@outlook.com"));

            //send it to the default mail application.
            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);

        }




    }


    /// <summary>
    /// ViewModel for the Pivot Item : About
    /// </summary>
    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public Uri RateMe => new Uri("http://aka.ms/template10");
    }
}

