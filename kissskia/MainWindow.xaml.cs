using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kissskia
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        private Task _caretTask = null;
        private CancellationToken _taskToken;

        private BinaryReader _reader;

        public ICommand CopyCommand{ get; set; }

        public BinaryReader Reader
        {
            get { return _reader; }
            set { _reader = value; }
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
            _taskToken = new CancellationToken();
            _caretTask = Task.Run(() => { FreshCaret(_taskToken); }, _taskToken);

            var rand = new Random();

            // 10 MB of random data
            var bytes = new byte[1 * 1024 * 1024];
            rand.NextBytes(bytes);

            var fs = new FileStream("D:/temp/测试🌼/avrecoder-x64.mp4", FileMode.Open, FileAccess.Read);
            //Reader = new BinaryReader(new MemoryStream(bytes));
            Reader = new BinaryReader(fs);
            Root.RequestedTheme = ElementTheme.Light;

            CopyCommand = new RelayCommand(CopyExecuted, CopyCanExecute);
        }

        private void CopyExecuted(object sender)
        {
            Debugger.Log(0, "s", $"CopyExecuted\n");
        }
        private bool CopyCanExecute(object sender)
        {
            Debugger.Log(0, "s", $"CopyCanExecute\n");
            return true;
        }

        private async void FreshCaret(CancellationToken token)
        {
            try
            {
                await Task.Delay(500, token);
            } catch (TaskCanceledException) {
            }
        }
    }
}
