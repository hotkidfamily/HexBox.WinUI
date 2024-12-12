using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HexBox.WinUI.Demo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        private Task _caretTask = null;
        private CancellationToken _taskToken;

        private BinaryReader _reader;
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

            var rand = new Random();

            // 10 MB of random data
            var bytes = new byte[1 * 1024 * 1024];
            rand.NextBytes(bytes);

            var fs = new FileStream("D:\\ÊÓÆµËØ²Ä²Ö¿â\\fragment-mp4-form-obs.mp4", FileMode.Open, FileAccess.Read);
            //var fs = new FileStream(@"C:\Users\admin\Desktop\crash\short-binary.dat", FileMode.Open, FileAccess.Read);
            //Reader = new BinaryReader(new MemoryStream(bytes));
            Reader = new BinaryReader(fs);
            Root.RequestedTheme = ElementTheme.Light;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
