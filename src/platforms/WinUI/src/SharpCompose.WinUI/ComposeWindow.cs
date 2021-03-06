using System;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using SharpCompose.Base;
using SharpCompose.Base.Input;
using SharpCompose.Drawer.Core.Images;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.WinUI
{
    public abstract class ComposeWindow : Window, IInputHandler
    {
        private WinUICanvas composeCanvas;
        private readonly DispatcherTimer timer;
        private readonly CanvasControl canvas;
        private bool init;

        public event Action MouseDown;
        public event Action MouseUp;
        public event Action<int, int> MouseMove;
        public event Action<KeyCode> KeyDown;
        public event Action<string> OnTextInput;

        public (int x, int y) MousePosition { get; private set; }

        protected ComposeWindow()
        {
            canvas = new CanvasControl();
            Content = canvas;

            canvas.PointerMoved += (_, args) =>
            {
                var pointer = args.GetCurrentPoint(canvas);
                MousePosition = ((int) pointer.Position.X, (int) pointer.Position.Y);
                MouseMove?.Invoke((int) pointer.Position.X, (int) pointer.Position.Y);
            };

            canvas.PointerPressed += (_, _) => MouseDown?.Invoke();
            canvas.PointerReleased += (_, _) => MouseUp?.Invoke();
            canvas.KeyDown += (_, arg) => KeyDown?.Invoke((KeyCode) arg.Key);

            canvas.CreateResources += Canvas_CreateResources;

            SizeChanged += (_, _) => Composer.Recompose();

            timer = new DispatcherTimer();
            timer.Tick += Tick;
            timer.Interval = TimeSpan.FromMilliseconds(1);
        }

        [Composable]
        protected abstract void SetContent();

        protected virtual Task LoadResources() => Task.CompletedTask;

        private void Tick(object sender, object e)
        {
            if (Composer.Instance.RecomposingAsk && Bounds.Width > 0 && Bounds.Height > 0)
            {
                composeCanvas.Size = new IntSize((int) Bounds.Width, (int) Bounds.Height);
                Composer.Compose(this, SetContent);
                Composer.Layout();
                canvas.Invalidate();
            }
        }

        private void Canvas_CreateResources(CanvasControl sender,
            Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            async Task Load()
            {
                await LoadResources();
                Composer.Recompose();
                timer.Start();
            }

            if (init)
                return;

            init = true;
            args.TrackAsyncAction(Load().AsAsyncAction());

            composeCanvas = new WinUICanvas(canvas);
            Composer.Instance.Init(composeCanvas);

            canvas.Draw += (_, drawEventArgs) =>
            {
                composeCanvas.StartDraw(drawEventArgs.DrawingSession);
                Composer.Draw();
            };
        }

        protected async Task<IImage> LoadSvgImage(string filePath)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{filePath}"));
            var fileText = await FileIO.ReadTextAsync(file);
            var svgImage = new WinUIVectorImage(canvas, fileText);

            return svgImage;
        }

        public void SetCursor(Cursor cursor)
        {
            if (CoreWindow?.PointerCursor != null)
                CoreWindow.PointerCursor = cursor switch
                {
                    Cursor.Default => new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0),
                    Cursor.Pointer => new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0),
                    Cursor.Text => new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.IBeam, 0),
                    _ => throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null)
                };
        }
    }
}