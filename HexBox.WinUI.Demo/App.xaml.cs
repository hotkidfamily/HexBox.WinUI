using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HexBox.WinUI.Demo
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            _uisettings = new();
            _uisettings.ColorValuesChanged += Settings_ColorValuesChanged;
            var systemTheme = _uisettings.GetColorValue(UIColorType.Background).ToString().Equals("#FFFFFFFF") ? ApplicationTheme.Light : ApplicationTheme.Dark;

            // packaged 
            if(IsPackaged())
            {
                _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                var userTheme = _localSettings.Values["themes"] as string;
                if (userTheme != null)
                {
                    App.Current.RequestedTheme = userTheme == "light" ? ApplicationTheme.Light : ApplicationTheme.Dark;
                }
            }

            App.Current.RequestedTheme = systemTheme;

            _queue = DispatcherQueue.GetForCurrentThread();
        }

        private bool IsPackaged()
        {
            try
            {
                var package = Package.Current;
                return package != null;
            }
            catch
            {
                return false;
            }
        }
        
        private void Settings_ColorValuesChanged(UISettings sender, object args)
        {
            var currentTheme = _uisettings.GetColorValue(UIColorType.Background).ToString().Equals("#FFFFFFFF") ? ApplicationTheme.Light : ApplicationTheme.Dark;

            _queue.TryEnqueue(() =>
            {
                if (currentTheme == ApplicationTheme.Dark)
                {
                    if (_window?.Content is FrameworkElement b)
                    {
                        b.RequestedTheme =  ElementTheme.Dark;
                    }
                }
                else
                {
                    if (_window?.Content is FrameworkElement b)
                    {
                        b.RequestedTheme =  ElementTheme.Light;
                    }
                }
            });
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }

        private Window? _window;
        private UISettings _uisettings;
        private DispatcherQueue _queue;
        Windows.Storage.ApplicationDataContainer? _localSettings;
    }
}
