using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HexBox.WinUI.Demo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx, INotifyPropertyChanged
    {
        private DispatcherQueue _queue;
        private Task _queryTask;
        private CancellationTokenSource _tokenSource;

        private BinaryReader _reader = default!;

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _EnforceMode = false;

        public bool EnforceMode
        {
            get { return _EnforceMode; }
            set
            {
                if (_EnforceMode != value)
                {
                    _EnforceMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public BinaryReader Reader
        {
            get { return _reader; }
            set
            {
                _reader = value;
                OnPropertyChanged();
            }
        }

        private int _columns = 32;

        public int Columns
        {
            get { return _columns; }
            set
            {
                if (_columns != value)
                {
                    _columns = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            this.InitializeComponent();
            this.SetTaskBarIcon(Icon.FromFile("Assets/hexagons_32.ico"));
            this.MaxWidth = 1920;
            this.MaxHeight = 1080;
            //this.Move(640, 1280);
            //this.CenterOnScreen();
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);
            this.Width = 1280;
            this.Height = 720;
            _tokenSource = new();
            _queue = DispatcherQueue.GetForCurrentThread();
            _queryTask = Task.Run(() => _queryFocus(_tokenSource.Token, _queue), _tokenSource.Token);

            _queue.ShutdownStarting += _queue_ShutdownStarting;

            if (AppSettings.LocalSettings.TryGetUintValue("theme", out var theme))
            {
                foreach (ComboBoxItem item in ThemeBox.Items)
                {
                    if (item.Tag.ToString() == theme.ToString())
                    {
                        ThemeBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (AppSettings.LocalSettings.TryGetStringValue("Language", out var lang))
            {
                foreach (ComboBoxItem item in LanguageBox.Items)
                {
                    if (item.Tag.ToString() == lang.ToString())
                    {
                        LanguageBox.SelectedItem = item;
                        break;
                    }
                }
            }
            ThemeBox.SelectionChanged += Theme_SelectionChanged;
            LanguageBox.SelectionChanged += LanguageBox_SelectionChanged;
        }

        private void _queue_ShutdownStarting(DispatcherQueue sender, DispatcherQueueShutdownStartingEventArgs args)
        {
            _tokenSource.Cancel();

            _queryTask.Wait();
        }

        private async Task _queryFocus(CancellationToken token, DispatcherQueue queue)
        {
            while (!token.IsCancellationRequested)
            {
                queue.TryEnqueue(() =>
                    {
                        if (Root.IsLoaded)
                        {
                            var focusedElement = FocusManager.GetFocusedElement(Root.XamlRoot);

                            FindBox.Text = $"Focused: {focusedElement}";
                        }
                    }
                );

                try
                {
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new()
            {
                SuggestedStartLocation= PickerLocationId.PicturesLibrary,
            };
            filePicker.FileTypeFilter.Add("*");

            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, this.GetWindowHandle());

            var file = await filePicker.PickSingleFileAsync();

            if (file == null)
                return;
            else
            {
                /*sample 1 using filestream as data provider */
                var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
                Reader = new BinaryReader(fs);
                /* sample 2, using MemoryStream as data provider */
                /*
                var bytes = new byte[1024];
                var rd = new Random();
                rd.NextBytes(bytes);
                var ms = new MemoryStream(bytes, 0, bytes.Length);
                Reader = new BinaryReader(ms);
                */
                HexViewer.DataSource = Reader;
                HexViewer.HighlightedRegions.Clear();
                List<HexBox.HighlightedRegion> HighlightedRegions = [];
                Color[] cols = [Colors.DeepSkyBlue, Colors.Aquamarine, Colors.DarkSalmon];
                int offset = 0x2f2;
                for (int i = 0; i < 3; i++)
                {
                    HexBox.HighlightedRegion r = new()
                    {
                        Start = offset + 3*i*29 + i,
                        Length = 3*29,
                        Color = new SolidColorBrush() { Color = cols[i] }
                    };
                    HighlightedRegions.Add(r);
                }
                HexViewer.HighlightedRegions = HighlightedRegions;
                HexViewer.ScrollToOffset(offset);
            }
        }

        private void Theme_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if ((ComboBox)sender == ThemeBox)
            {
                if (ThemeBox.SelectedItem is ComboBoxItem c)
                {
                    uint idx = uint.Parse(c.Tag?.ToString());
                    if (Content is FrameworkElement a)
                        a.RequestedTheme  = idx == 0 ? ElementTheme.Default : idx == 1 ? ElementTheme.Light : ElementTheme.Dark;
                    AppSettings.LocalSettings.Values["theme"] = idx;
                    AppSettings.LocalSettings.Save();
                }
            }
        }

        private void LanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBox)sender == LanguageBox)
            {
                if (LanguageBox.SelectedItem is ComboBoxItem c)
                {
                    var lang = c.Tag.ToString();
                    AppSettings.LocalSettings.Values["Language"] = lang;
                    AppSettings.LocalSettings.Save();
                }
            }
        }

        private void ColumnsSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if ((Slider)sender == ColumnsSlider)
            {
                int cols = (int)e.NewValue;
                _queue?.TryEnqueue(() =>
                {
                    Columns = cols;
                });
            }
        }
    }
}