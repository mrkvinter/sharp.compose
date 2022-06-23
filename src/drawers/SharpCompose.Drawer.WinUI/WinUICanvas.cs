using Microsoft.Graphics.Canvas.UI.Xaml;
using SharpCompose.Drawer.Core;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

namespace SharpCompose.WinUI
{
    public class WinUICanvas : ICanvas
    {
        private readonly CanvasControl canvasControl;

        public (int w, int h) Size { get; set; }

        private readonly List<WinUIGraphics> graphics = new();
        private CanvasDrawingSession canvasDrawingSession;

        public WinUICanvas(CanvasControl canvasControl)
        {
            this.canvasControl = canvasControl;
        }

        public void Draw()
        {
        }

        public void StartDraw(CanvasDrawingSession canvasDrawingSession)
        {
            this.canvasDrawingSession = canvasDrawingSession;
        }

        public void DrawGraphics(int x, int y, IGraphics graphics)
        {

            using var canvasCommandList = new CanvasCommandList(canvasDrawingSession);
            var canvas = canvasCommandList.CreateDrawingSession();
            ((WinUIGraphics)graphics).Draw(canvas);
            ((WinUIGraphics)graphics).Pop();
            canvasDrawingSession.DrawImage(canvasCommandList);
        }

        public (int w, int h) MeasureText(string text, double emSize, Font font)
        {
            var format = new CanvasTextFormat
            {
                FontSize = (float)emSize,
                FontFamily = font.FontFamily,
            };
            using var canvasTextLayout = new CanvasTextLayout(canvasControl, text, format, float.MaxValue, float.MaxValue);

            return ((int)canvasTextLayout.LayoutBounds.Width, (int)canvasTextLayout.LayoutBounds.Height);
        }

        public IGraphics StartGraphics()
        {
            var winUIGraphics = new WinUIGraphics(canvasControl);
            graphics.Add(winUIGraphics);

            return winUIGraphics;
        }
    }
}