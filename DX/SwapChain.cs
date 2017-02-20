using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D1 = SharpDX.Direct2D1;
using Dxgi = SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Windows.UI.Xaml.Controls;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Input;
using Windows.UI.Input;
using SharpDX;
using Windows.UI.Xaml.Media.Imaging;
using SharpDX.WIC;
using System.IO;
using SharpDX.Direct2D1.Effects;
using System.Diagnostics;
using Windows.UI.ViewManagement;

namespace DX
{
    public struct Color
    {
        public float Alpha;
        public float Red;
        public float Green;
        public float Blue;
        public Color(float red, float green, float blue, float alpha)
        {
            Red = red;Green = green;Blue = blue; Alpha = alpha;
        }
        public Color(byte red, byte green, byte blue, byte alpha)
        {
            Red = (float)red/255; Green =(float) green/255; Blue =(float) blue/255; Alpha =(float) alpha/255;
        }
        public static implicit operator RawColor4(Color value)
        {
            return new RawColor4(value.Red, value.Green, value.Blue, value.Alpha);
        }
    }
    public sealed class SwapChain:SwapChainPanel
    {
        #region event
        static void PointerOperation(object sender,PointerRoutedEventArgs e)
        {
            PointerPoint p = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
            DX_Input.CopyPointer(p);
            if((sender as SwapChain).EventToSubThread)
            ThreadManage.AsyncDelegate(() => { (sender as SwapChain).CheckEvent(); });
            else (sender as SwapChain).CheckEvent();
        }
        static void CheckEvent(SwapChain swap)
        {
            var lui = swap.DX_Child;
            int c = lui.Count;
            if (c == 0)
                return;
            c--;
            int i;
            int t = c;
            for (i = c; i >= 0; i--)
            {
                UIElement u = lui[i];
                if (u.Visble)
                    if (UIElement.CheckEvent(u, DX_Input.Position))
                        if (!u.EventCross)
                        {
                            DX_Input.PointerHandled = true;
                            if (DX_Input.PointerUpdateKind != PointerUpdateKind.Other)
                                break;
                        }
            }
            if(DX_Input.IsInContact|DX_Input.PointerUpdateKind!=PointerUpdateKind.Other)
            ThreadManage.UpdateUI = true;
            if (DX_Input.MouseWheelDelta != 0)
                ThreadManage.UpdateUI = true;
        }
        #endregion

        public bool Change;
        /// <summary>
        /// delegate event check to sub thread
        /// </summary>
        public bool EventToSubThread = false;
        public List<UIElement> DX_Child;
        public RawColor4 background;
        SwapChainComponent com;
        D2D1.Bitmap BitmapBuffer;
        public SwapChain(double width,double height)
        {
            Width = width;
            Height = height;
            com = new SwapChainComponent();
            com.panel = this;
            com.dpi= DisplayInformation.GetForCurrentView().LogicalDpi;
            DX_Core.CreateD3DInstance(ref com);
            DX_Child = new List<UIElement>();
            this.PointerPressed += PointerOperation;
            this.PointerMoved += PointerOperation;
            this.PointerReleased +=PointerOperation;
            this.PointerEntered += PointerOperation;
            this.PointerExited += PointerOperation;
            this.PointerWheelChanged += PointerOperation;
            var bp = new D2D1.BitmapProperties1(
           new D2D1.PixelFormat(Dxgi.Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied),
           com.dpi, com.dpi, D2D1.BitmapOptions.Target);
            lock(DX_Core.D2D)
                BitmapBuffer = new D2D1.Bitmap1(DX_Core.D2D.d2dContext, new Size2((int)width, (int)height), bp);
            ThreadManage.StartLoop(this);
        }
        ~SwapChain()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (DX_Child != null)
            {
                DX_Child.Clear();
                GC.SuppressFinalize(DX_Child);
            }
            if (gaussian != null)
                gaussian.Dispose();
            BitmapBuffer.Dispose();
            DX_Core.DisposeSwapChain(ref com);
            GC.SuppressFinalize(this);
        }
        internal void DrawBuffer()
        {
            lock(DX_Child)
            lock(DX_Core.D2D)
            {
                var d2d = DX_Core.D2D;
                var context = d2d.d2dContext;
                if (gaussian == null)
                {
                    gaussian = new GaussianBlur(context);
                    gaussian.SetInput(0, BitmapBuffer, true);
                }
                context.Target = BitmapBuffer;
                context.BeginDraw();
                context.Clear(background);
                    for (int i = 0; i < DX_Child.Count; i++)
                    {
                        UIElement u = DX_Child[i];
                        if (u != null)
                            if (u.Visble)
                            {
                                if (u.Tag == UIElement.UITag.Panel)
                                {
                                    context.EndDraw();
                                    var p = (u as UIPanel);
                                    p.DrawUI(d2d, gaussian);
                                    context.Target = BitmapBuffer;
                                    context.BeginDraw();
                                    if (p.GaussianBack)
                                        DrawClipMap(d2d, p);
                                    else DrawMap(d2d, p);
                                }
                                else
                                if (u.UpdateUI != null)
                                    u.UpdateUI(d2d, u);
                            }
                    }
                    context.EndDraw();
            }
        }
        internal void UpdateUI()
        {
            lock(DX_Child)
            lock(DX_Core.D2D)
            {
                    var d2d = DX_Core.D2D;
                    var context = d2d.d2dContext;
                    context.Target = com.Bitmap;
                    context.BeginDraw();
                    context.Transform = Matrix3x2.Identity;
                    context.Clear(background);
                    context.DrawBitmap(BitmapBuffer, new RectangleF(0, 0, (int)Width, (int)Height), 1, D2D1.BitmapInterpolationMode.NearestNeighbor);
                    context.EndDraw();
                    com.swapChain.Present(0, 0);
                }
        }
        void CheckEvent()
        {
            int c = DX_Child.Count;
            if (c == 0)
                return;
            c--;
            lock(DX_Child)
            CheckEvent(this);
        }
        public void ReSize(double width,double height)
        {
            Width = width;
            Height = height;
            DX_Core.DisposeSwapChain(ref com);
            DX_Core.CreateD3DInstance(ref com);
            lock (DX_Core.D2D)
            {
                BitmapBuffer.Dispose();
                if (gaussian != null)
                {
                    gaussian.Dispose();
                    gaussian = null;
                }
                var bp = new D2D1.BitmapProperties1(
             new D2D1.PixelFormat(Dxgi.Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied),
             com.dpi, com.dpi, D2D1.BitmapOptions.Target);
                BitmapBuffer = new D2D1.Bitmap1(DX_Core.D2D.d2dContext, new Size2((int)width, (int)height), bp);
            }
        }
        GaussianBlur gaussian;
        void DrawMap(D2DEnviroment d2d, UIPanel port)
        {
            RawRectangleF rect;
            rect.Left = port.Location.X;
            rect.Top = port.Location.Y;
            rect.Right = port.Size.Width + rect.Left;
            rect.Bottom = port.Size.Height + rect.Top;
            var d2dcontext = d2d.d2dContext;
            d2dcontext.Transform = Matrix3x2.Identity;
            d2dcontext.DrawBitmap(port.bitmap, rect, 1, D2D1.BitmapInterpolationMode.NearestNeighbor);
        }
        void DrawClipMap(D2DEnviroment d2d,UIPanel port)
        {
            RawRectangleF rect = port.Rect;      
            RawRectangleF rc;
            rc.Left = rect.Left + 3;
            rc.Right = rect.Right - 3;
            rc.Top = rect.Top + 3;
            rc.Bottom = rect.Bottom - 3;

            var d2dcontext = d2d.d2dContext;
            d2dcontext.Transform = Matrix3x2.Identity;
            d2dcontext.PushAxisAlignedClip(rc, D2D1.AntialiasMode.Aliased);
            d2dcontext.Clear(background);
            d2dcontext.DrawBitmap(port.bitmap,rect,1,D2D1.BitmapInterpolationMode.NearestNeighbor);
            d2dcontext.PopAxisAlignedClip();
        }
    }
}
