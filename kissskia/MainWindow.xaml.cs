using Microsoft.UI.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using WinUIEx;
using Microsoft.Graphics.Canvas.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kissskia
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        private IntPtr _hWnd = IntPtr.Zero;
        private Task _caretTask = null;
        private CancellationToken _taskToken;

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
            _hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            _taskToken = new CancellationToken();
            _caretTask = Task.Run(() => { FreshCaret(_taskToken); }, _taskToken);
        }

        private async void FreshCaret(CancellationToken token)
        {
            try
            {
                var c = Caret.Create(_hWnd, IntPtr.Zero, 10, 48);
                c = Caret.Show(_hWnd);
                int x = 0;
                while (!token.IsCancellationRequested)
                {
                    Caret.SetPos(x++, 0);
                    await Task.Delay(3000, token);
                }
            }catch(TaskCanceledException) {
                Caret.Destroy();
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

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            args.DrawingSession.DrawEllipse(155, 115, 80, 30, Microsoft.UI.Colors.Black, 3);
            args.DrawingSession.DrawText("Hello, Win2D World!", 100, 100, Microsoft.UI.Colors.Yellow);
        }
    }

    internal static class Caret
    {
        class UserSafeHanlde : SafeHandle
        {
            public UserSafeHanlde(IntPtr Handle) : base(IntPtr.Zero, true) 
            {
                SetHandle(Handle);
            }

            public override bool IsInvalid => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                return PInvoke.CloseHandle(new HANDLE(handle));
            }

        }
        public static bool Create(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight)
        {
            try
            {
                var bmp = new UserSafeHanlde(hBitmap);
                return PInvoke.CreateCaret(new HWND(hWnd), null, nWidth, nHeight);
            }
            catch (Exception ex)
            {
                // Let's pretend CreateCaret() is available on Linux
                if ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException))
                    return true;

                throw;
            }
        }

        public static bool Show(IntPtr hWnd)
        {
            try
            {
                return PInvoke.ShowCaret(new HWND(hWnd));
            }
            catch (Exception ex)
            {
                // Let's pretend ShowCaret() is available on Linux
                if ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException))
                    return true;

                throw;
            }
        }

        public static bool Destroy()
        {
            try
            {
                return PInvoke.DestroyCaret();
            }
            catch (Exception ex)
            {
                // Let's pretend DestroyCaret() is available on Linux
                if ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException))
                    return true;

                throw;
            }
        }

        public static bool SetPos(int X, int Y)
        {
            try
            {
                return PInvoke.SetCaretPos(X, Y);
            }
            catch (Exception ex)
            {
                // Let's pretend SetCaretPos() is available on Linux
                if ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException))
                    return true;

                throw;
            }
        }
    }
}
