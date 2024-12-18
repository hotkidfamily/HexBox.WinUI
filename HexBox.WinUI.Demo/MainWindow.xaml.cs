using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
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
        //private Task _caretTask = null;
        //private CancellationToken _taskToken;

        private BinaryReader _reader;

        public event PropertyChangedEventHandler? PropertyChanged;

        public BinaryReader Reader
        {
            get { return _reader; }
            set {
                _reader = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            this.InitializeComponent();
            this.MaxWidth = 1920;
            this.MaxHeight = 1080;
            this.Move(640, 1280);
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);
            this.Width = 1280;
            this.Height = 720;

            Root.RequestedTheme = ElementTheme.Light;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new() 
            { 

            };
            IntPtr hwnd = this.GetWindowHandle();

            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

            var file = await filePicker.PickSingleFileAsync();

            if (file == null)
                return;
            else
            {
                var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
                Reader = new BinaryReader(fs);
                HexViewer.DataSource = Reader;
                Color[] cols = [Colors.DeepSkyBlue, Colors.Aquamarine, Colors.DarkSalmon];
                int offset = 0x256;
                for(int i = 0; i < 3 ; i++)
                {
                    HexBox.HighlightedRegion r = new();
                    r.Start = offset+i*16;
                    r.Length = 16;
                    r.Color = new SolidColorBrush() { Color = cols[i] };
                    HexViewer.HighlightedRegions.Add(r);
                }

                HexViewer.Offset = offset;
            }
        }
    }
}
