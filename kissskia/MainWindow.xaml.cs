using Microsoft.UI.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            var bytes = new byte[10 * 1024 * 1024];
            rand.NextBytes(bytes);

            Reader = new BinaryReader(new MemoryStream(bytes));
        }

        private async void FreshCaret(CancellationToken token)
        {
            try
            {
                await Task.Delay(500, token);
            } catch (TaskCanceledException) {
            }
        }

        private void SKXamlCanvas_PaintSurface(object sender, SkiaSharp.Views.Windows.SKPaintSurfaceEventArgs e)
        {
            var view = sender as SKXamlCanvas;
            DrawOverlayText(view, e.Surface.Canvas, view.CanvasSize, "canvas");
        }

        private void DrawOverlayText(FrameworkElement view, SKCanvas canvas, SKSize canvasSize, string backend)
        {
            const int TextOverlayPadding = 8;
            SKPaint textPaint = new()
            {
                TextSize = 16,
                IsAntialias = true,
            };

            canvas.Clear();

            SKFont textFont = new()
            {
                BaselineSnap = true,
            };

            // make sure no previous transforms still apply
            canvas.ResetMatrix();

            // get and apply the current scale
            var scale = canvasSize.Width / (float)view.ActualWidth;
            canvas.Scale(scale);

            var y = (float)view.ActualHeight - TextOverlayPadding;

            var text = $"Current scaling = {scale:0.0}x";
            canvas.DrawText(text, TextOverlayPadding, y, textFont, textPaint);

            y -= textPaint.TextSize + TextOverlayPadding;

            text = "SkiaSharp: " + SkiaSharpVersion.Native.ToString();
            canvas.DrawText(text, TextOverlayPadding, y, textPaint);

            y -= textPaint.TextSize + TextOverlayPadding;

            text = "HarfBuzzSharp: " + "v1.1.0";
            canvas.DrawText(text, TextOverlayPadding, y, textPaint);

            y -= textPaint.TextSize + TextOverlayPadding;

            text = "Backend: " + backend;
            canvas.DrawText(text, TextOverlayPadding, y, textPaint);
        }

    }
}
