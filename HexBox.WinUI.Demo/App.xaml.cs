using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
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
            var systemTheme = _uisettings.GetColorValue(UIColorType.Background) == Colors.Black ? ApplicationTheme.Dark : ApplicationTheme.Light;

            if (AppSettings.LocalSettings.TryGetUintValue("theme", out var theme))
            {
                systemTheme = theme == 0 ? systemTheme : theme == 1 ? ApplicationTheme.Light : ApplicationTheme.Dark;
            }

            Application.Current.RequestedTheme = systemTheme;

            _queue = DispatcherQueue.GetForCurrentThread();
            if (AppSettings.LocalSettings.TryGetStringValue("Language", out var lang))
            {
                Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = lang;
            }
        }

        private void Settings_ColorValuesChanged(UISettings sender, object args)
        {
            var systemTheme = _uisettings.GetColorValue(UIColorType.Background) == Colors.Black ? ApplicationTheme.Dark : ApplicationTheme.Light;

            _queue.TryEnqueue(() =>
            {
                if (AppSettings.LocalSettings.TryGetUintValue("theme", out var theme))
                {
                    if (theme == 0)
                    {
                        if (_window?.Content is FrameworkElement b)
                        {
                            b.RequestedTheme = systemTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
                        }
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
    }
}