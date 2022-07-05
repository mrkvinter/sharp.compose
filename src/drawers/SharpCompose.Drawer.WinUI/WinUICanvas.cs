using Microsoft.Graphics.Canvas.UI.Xaml;
using SharpCompose.Drawer.Core;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas;
using SharpCompose.Drawer.Core.Utilities;

namespace SharpCompose.WinUI
{
    public class WinUICanvas : ICanvas
    {
        private readonly CanvasControl canvasControl;

        public IntSize Size { get; set; }

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

        public IGraphics StartGraphics()
        {
            var winUIGraphics = new WinUIGraphics(canvasControl);
            graphics.Add(winUIGraphics);

            return winUIGraphics;
        }
    }
}