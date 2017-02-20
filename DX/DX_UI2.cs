using System;
using System.Collections.Generic;
using Wic = SharpDX.WIC;
using D2D1 = SharpDX.Direct2D1;
using Dxgi = SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.DirectWrite;
using SharpDX;
using System.IO;
using Windows.Storage.Streams;
using System.Diagnostics;
using Windows.UI.Input;
using SharpDX.Direct2D1.Effects;

namespace DX
{
    public enum PanelTag
    {
        ListBox,ViewPort,StackPanel,GridPanel,GridBox
    }
    public class UIPanel:UIElement
    {
        public PanelTag Tag2 { get; protected set; }
        public UIPanel()
        {
            Tag = UITag.Panel;
        }
        internal D2D1.Bitmap bitmap;
        public override Size2F Size
        {
            get { return size; }
            set
            {
                size = value;
                if (size.Width < 0)
                    size.Width = 0;
                if (size.Height < 0)
                    size.Height = 0;
                Rect.Left = location.X;
                Rect.Right = location.X + size.Width;
                Rect.Top = location.Y;
                Rect.Bottom = location.Y + size.Height;
                lock (DX_Core.D2D)
                {
                    if (bitmap != null)
                        bitmap.Dispose();
                    var bp = new D2D1.BitmapProperties1(
                  new D2D1.PixelFormat(Dxgi.Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied),
                  DX_Core.dpi, DX_Core.dpi, D2D1.BitmapOptions.Target);
                    Size2 s = new Size2((int)size.Width, (int)size.Height);
                    bitmap = new D2D1.Bitmap1(DX_Core.D2D.d2dContext, s, bp);
                }
                Rect = new RawRectangleF(location.X,location.Y,location.X+size.Width,location.Y+size.Height);
                Update = true;
            }
        }
        public bool GaussianBack;
        public bool Update = true;
        public RawRectangleF SourceRect;
        internal virtual void DrawUI(D2DEnviroment d2d, D2D1.Effect effect) { }
    }
    public sealed class ListBox:UIPanel
    {
        static void PointerPressA(UIElement u, Vector2 dot)
        {
            ListBox lb = u as ListBox;
            lb.OperationTime = DX_Input.EventTicks;
            lb.SlideTime = DX_Input.EventTime;
            lb.OriginPoiter = DX_Input.Position;
            lb.Velocities = 0;
        }
        static void PointerReleaseA(UIElement u, Vector2 dot)
        {
            ListBox lb = u as ListBox;
            float x = DX_Input.Position.X - lb.OriginPoiter.X;
            float y= DX_Input.Position.Y - lb.OriginPoiter.Y;
            if(x>-10& x<10)
                if(y>-10&y<10)
                {
                    long t = DX_Input.EventTicks;
                    if (t - lb.OperationTime < PressTime)
                    {
                        dot -= lb.Location;
                        int o;
                        if (lb.Hrizon)
                        {
                            int ds = (int)dot.X;
                            o = lb.Offset;
                            o += ds;
                            o /= lb.ItemWidth;
                            if (o >= lb.Data.Count)
                                o = -1;
                        }
                        else
                        {
                            int ds = (int)dot.Y;
                            o = lb.Offset;
                            o += ds;
                            o /= lb.ItemHeight;
                            if (o >= lb.Data.Count)
                                o = -1;
                        }
                        if(lb.SelectedIndex!=o)
                        {
                            lb.SelectedIndex = o;
                            if (lb.SelectChanged != null)
                                lb.SelectChanged(u);
                        }
                        else if (lb.ItemClick != null)
                                lb.ItemClick(u);
                        
                    }
                    lb.OperationTime = t;
                    lb.Update = true;
                    return;
                }
            if(lb.Velocities1<0)
            {
                if (lb.Velocities > lb.Velocities1)
                    lb.Velocities = lb.Velocities1;
            }
            else
            {
                if (lb.Velocities < lb.Velocities1)
                    lb.Velocities = lb.Velocities1;
            }
        }
        static void PointerMoveA(UIElement u, Vector2 dot)
        {
            if(DX_Input.IsInContact)
            {
                ListBox lb = u as ListBox;
                lb.SlideTime = DX_Input.EventTime;
                lb.Velocities = lb.Velocities1;
                int os = lb.Offset;
                if (lb.Hrizon)
                {
                    lb.Velocities1 = DX_Input.VelocitiesX;
                    os -=(int) DX_Input.Motion.X;
                }
                else
                {
                    lb.Velocities1 = DX_Input.VelocitiesY;
                    os -= (int)DX_Input.Motion.Y;
                }
                lb.SetOffset(os);
            }
        }
        static void PointerLeaveA(UIElement u, Vector2 dot)
        {
            if(DX_Input.IsInContact)
            {
                ListBox lb = u as ListBox;
                lb.SlideTime = DX_Input.EventTime;
                lb.Velocities = lb.Velocities1;
                int os = lb.Offset;
                if (lb.Hrizon)
                {
                    lb.Velocities1 = DX_Input.VelocitiesX;
                    os -= (int)DX_Input.Motion.X;
                }
                else
                {
                    lb.Velocities1 = DX_Input.VelocitiesY;
                    os -= (int)DX_Input.Motion.Y;
                }
                lb.SetOffset(os);
            }
        }
        static void MouseWheelA(UIElement u,Vector2 dot)
        {
            ListBox lb = u as ListBox;
            int os = lb.Offset;
            os -= DX_Input.MouseWheelDelta;
            lb.SetOffset(os);
        }

        Vector2 OriginPoiter;
        float Velocities = 0;
        float Velocities1 = 0;
        int SlideTime;
        public float DecayRate = 0.998f;
        public UIElement BackGroundTemplate;
        public UIElement FillTemplate;
        public TextBlock ItemTemplate { get; private set; }
        public List<string> Data;
        public Action<UIElement> SelectChanged;
        public Action<UIElement> ItemClick;
        public bool Hrizon;
        public int ItemWidth=60;
        public int ItemHeight=20;
        public float BorderStrok=1;
        public RawColor4 BorderColor;
        public RawColor4 SelectFillColor;
        public RawColor4 SelectTextColor = new RawColor4(1,1,1,1);
        public int Offset { get; private set; }
        public int SelectedIndex { get; set; }
        public ListBox()
        {
            Tag = UITag.Panel;
            Tag2 = PanelTag.ListBox;
            Data = new List<string>();
            ItemTemplate = new TextBlock();
            ItemTemplate.Alignment = TextAlignment.Center;
            ItemHeight = 24;
            PointerPress = PointerPressA;
            PointerRelease = PointerReleaseA;
            PointerMove = PointerMoveA;
            PointerLeave = PointerLeaveA;
            MouseWheel = MouseWheelA;
            CheckDot = CheckRect;
            SelectedIndex = -1;
        }
        ~ListBox()
        {
            Dispose();
        }
        public void SetOffset(int offset)
        {
            if (offset < 0)
                offset = 0;
            int len;
            if (Hrizon)
            {
                len = ItemWidth * Data.Count;
                if (len < size.Width)
                    return;
                if (offset + Size.Width > len)
                    offset = len - (int)Size.Width;
            }
            else
            {
                len = ItemHeight * Data.Count;
                if (len <= size.Height)
                    return;
                if (offset + Size.Height > len)
                    offset = len - (int)Size.Height;
            }
            Offset = offset;
            if (Visble)
            {
                ThreadManage.UpdateUI = true;
                Update = true;
            }
        }
        public override void Dispose()
        {
            ItemTemplate.Dispose();
            GC.SuppressFinalize(this);
        }
        internal override void DrawUI(D2DEnviroment d2d, D2D1.Effect effect)
        {
            if (bitmap == null)
                return;
            if (Velocities != 0)
                DuringSlide();
            if (!Update)
                if (!GaussianBack)
                    return;
            D2D1.DeviceContext context = d2d.d2dContext;
            context.Target = bitmap;
            context.BeginDraw();
            context.Transform = Matrix3x2.Identity;
            context.Clear(Background);
            if (GaussianBack)
                if (effect != null)
                {
                    context.DrawImage(effect, new RawVector2(0 - Location.X, 0 - Location.Y));
                    if (Background.A > 0)
                    {
                        d2d.ColorBrush.Color = Background;
                        context.FillRectangle(new RawRectangleF(0,0,size.Width,size.Height), d2d.ColorBrush);
                    }
                }
            DrawData(d2d);
            if (BackGroundTemplate != null)
            {
                if (BackGroundTemplate.UpdateUI != null)
                    BackGroundTemplate.UpdateUI(d2d, BackGroundTemplate);
            }
            else if(BorderColor.A>0)
            {
                d2d.ColorBrush.Color = BorderColor;
                context.DrawRectangle(new RawRectangleF(0, 0, Size.Width, Size.Height), d2d.ColorBrush, BorderStrok);
            }
            context.EndDraw();
            Update = false;
        }
        void DrawData(D2DEnviroment d2d)
        {
            RawRectangleF rect;
            if (Hrizon)
            {
                if (ItemWidth <= 0)
                    return;
                TextFormat tf = TextBlock.GetFormat(ItemTemplate.FontStyle,ItemTemplate.FontSize);
                tf.TextAlignment = ItemTemplate.Alignment;
                int s = Offset;
                int drawindex=Offset/ItemWidth;
                int r = Offset % ItemWidth;
                int os = drawindex * ItemWidth;
                os = -r;
                int len = (int)Size.Width;
                rect.Left = os+5;
                rect.Top = 5;
                rect.Right = os + ItemWidth;
                rect.Bottom = Size.Height;
                var render = d2d.d2dContext;
                for(int i=drawindex;i<Data.Count;i++)
                {
                    if (SelectedIndex == i)
                    {
                        if (FillTemplate != null)
                        {
                            FillTemplate.Location = new Vector2(rect.Left, rect.Top);
                            if (FillTemplate.UpdateUI != null)
                                FillTemplate.UpdateUI(d2d, FillTemplate);
                        }
                        else
                        {
                            d2d.ColorBrush.Color = SelectFillColor;
                            render.FillRectangle(rect, d2d.ColorBrush);
                        }
                        d2d.ColorBrush.Color = SelectTextColor;
                    }
                    else d2d.ColorBrush.Color = Forground;
                    render.DrawText(Data[i], tf, rect,
             d2d.ColorBrush, (D2D1.DrawTextOptions)6);
                    os += ItemWidth;
                    if (os >= len)
                        break;
                    rect.Left = os;
                    rect.Right += ItemWidth;
                }
            }
            else
            {
                if (ItemHeight <= 0)
                    return;
                TextFormat tf = TextBlock.GetFormat(ItemTemplate.FontStyle, ItemTemplate.FontSize);
                tf.TextAlignment = ItemTemplate.Alignment;
                int s = Offset;
                int drawindex = Offset / ItemHeight;
                int r = Offset % ItemHeight;
                int os = drawindex * ItemHeight;
                os = -r;
                int len = (int)Size.Height;
                rect.Left = 5;
                rect.Top = os+5;
                rect.Right = Size.Width-3;
                rect.Bottom = os+ItemHeight;
                var render = d2d.d2dContext;
                for (int i = drawindex; i < Data.Count; i++)
                {
                    if (SelectedIndex == i)
                    {
                        if (FillTemplate != null)
                        {
                            FillTemplate.Location = new Vector2(rect.Left,rect.Top);
                            if (FillTemplate.UpdateUI != null)
                                FillTemplate.UpdateUI(d2d, FillTemplate);
                        }
                        else
                        {
                            d2d.ColorBrush.Color = SelectFillColor;
                            render.FillRectangle(rect, d2d.ColorBrush);
                        }
                        d2d.ColorBrush.Color = SelectTextColor;
                    }
                    else d2d.ColorBrush.Color = Forground;
                    render.DrawText(Data[i], tf, rect,
             d2d.ColorBrush, (D2D1.DrawTextOptions)6);
                    os += ItemHeight;
                    if (os >= len)
                        break;
                    rect.Top = os;
                    rect.Bottom += ItemHeight;
                }
            }
        } 
        void DuringSlide()
        {
            if (Velocities > 0)
            {
                if (Velocities < 0.005f)
                { Velocities = 0; return; }
            }
            else
            {
                if (Velocities > -0.005f)
                { Velocities = 0; return; }
            }
            int t = DateTime.Now.Millisecond;
            int c = t - SlideTime;
            SlideTime = t;
            if (c < 0)
                c += 1000;
            int a = c;
            float v = DecayRate;
            while (c>1)
            {
                v *= v;
                c >>= 1;
            }
            if (c > 0)
                v *= DecayRate;
            float v1 = (DecayRate + v)*0.5f;
            float os = Offset- Velocities* v1 * a;
            Velocities *= v;
            SetOffset((int)os);
        }
    }
    public sealed class UIViewPort : UIPanel
    {
        public override Size2F Size
        {
            get { return size; }
            set
            {
                size = value;
                Rect.Left = location.X;
                Rect.Right = location.X + size.Width;
                Rect.Top = location.Y;
                Rect.Bottom = location.Y + size.Height;
                lock(DX_Core.D2D)
                {
                    if (bitmap != null)
                        bitmap.Dispose();
                    var bp = new D2D1.BitmapProperties1(
                  new D2D1.PixelFormat(Dxgi.Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied),
                  DX_Core.dpi, DX_Core.dpi, D2D1.BitmapOptions.Target);
                    Size2 s = new Size2((int)size.Width, (int)size.Height);
                    bitmap = new D2D1.Bitmap1(DX_Core.D2D.d2dContext, s, bp);
                    if (gaussian != null)
                    {
                        gaussian.Dispose();
                        gaussian = null;
                    }
                    gaussian = new GaussianBlur(DX_Core.D2D.d2dContext);
                    gaussian.SetInput(0, bitmap, true);
                }
                Rect = new RawRectangleF(location.X, location.Y, location.X + size.Width, location.Y + size.Height);
                Update = true;
            }
        }
        static void PointerOperation(object sender,Vector2 Position)
        {
            var v = sender as UIViewPort;
            var lui = v.DX_Child;
            int c = lui.Count;
            if (c == 0)
                return ;
            c--;
            int i;
            int t = c;
            var dot = Position - v.Location;
            for (i = c; i >= 0; i--)
            {
                UIElement u = lui[i];
                if( CheckEvent(u, dot))
                {
                    DX_Input.PointerHandled = true;
                    if (DX_Input.PointerUpdateKind != PointerUpdateKind.Other)
                        break;
                }
            }
        }
        public List<UIElement> DX_Child;
        public UIViewPort()
        {
            Tag = UITag.Panel;
            Tag2 = PanelTag.ViewPort;
            DX_Child = new List<UIElement>();
            CheckDot = CheckRect;
            PointerAnalysis = PointerOperation;
            PointerPress = PointerOperation;
            PointerRelease = PointerOperation;
            PointerEntry = PointerOperation;
            PointerMove = PointerOperation;
            PointerLeave = PointerOperation;
        }
        ~UIViewPort()
        {
            Dispose();
        }
        GaussianBlur gaussian;
        public override void Dispose()
        {
            int c = DX_Child.Count;
            for (int i = 0; i < c; i++)
                DX_Child[i].Dispose();
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            GC.SuppressFinalize(this);
        }
        internal override void DrawUI(D2DEnviroment d2d, D2D1.Effect effect)
        {
            if (bitmap == null)
                return;
            if (!Update)
                if (!GaussianBack)
                    return;
            D2D1.DeviceContext context = d2d.d2dContext;
            context.Target = bitmap;
            context.BeginDraw();
            context.Transform = Matrix3x2.Identity;
            context.Clear(Background);
            if (GaussianBack)
                if (effect != null)
                {
                    context.DrawImage(effect, new RawVector2(0 - Location.X, 0 - Location.Y));
                    if(Background.A>0)
                    {
                        d2d.ColorBrush.Color = Background;
                        context.FillRectangle(new RawRectangleF(0, 0, size.Width, size.Height), d2d.ColorBrush);
                    }
                }
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
                            context.Target = bitmap;
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
            Update = false;
        }
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
        void DrawClipMap(D2DEnviroment d2d, UIPanel port)
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
            d2dcontext.Clear(Background);
            d2dcontext.DrawBitmap(port.bitmap, rect, 1, D2D1.BitmapInterpolationMode.NearestNeighbor);
            d2dcontext.PopAxisAlignedClip();
        }
    }
    public sealed class StackPanel:UIPanel
    {
        static void PointerPressA(UIElement u, Vector2 dot)
        {
            StackPanel lb = u as StackPanel;
            lb.OperationTime = DX_Input.EventTicks;
            lb.SlideTime = DX_Input.EventTime;
            lb.OriginPoiter = DX_Input.Position;
            lb.Velocities = 0;
        }
        static void PointerReleaseA(UIElement u, Vector2 dot)
        {
            StackPanel lb = u as StackPanel;
            float x = DX_Input.Position.X - lb.OriginPoiter.X;
            float y = DX_Input.Position.Y - lb.OriginPoiter.Y;
            if (x > -10 & x < 10)
                if (y > -10 & y < 10)
                {
                    long t = DX_Input.EventTicks;
                    if (t - lb.OperationTime < PressTime)
                    {
                        dot -= lb.Location;
                        int o = lb.Offset; ;
                        if (lb.Hrizon)
                        {
                            int ds = (int)dot.X;
                            o += ds;
                            o /= lb.ItemWidth;
                            if (o >= lb.Data.Count)
                                o = -1;
                        }
                        else
                        {
                            int ds = (int)dot.Y;
                            o += ds;
                            o /= lb.ItemHeight;
                            if (o >= lb.Data.Count)
                                o = -1;
                        }
                        if (lb.SelectedIndex != o)
                        {
                            lb.SelectedIndex = o;
                            if (lb.SelectChanged != null)
                                lb.SelectChanged(u);
                        }
                        else if (lb.ItemClick != null)
                            lb.ItemClick(u);
                    }
                    lb.OperationTime = t;
                    return;
                }
            if (lb.Velocities1 < 0)
            {
                if (lb.Velocities > lb.Velocities1)
                    lb.Velocities = lb.Velocities1;
            }
            else
            {
                if (lb.Velocities < lb.Velocities1)
                    lb.Velocities = lb.Velocities1;
            }
        }
        static void PointerMoveA(UIElement u, Vector2 dot)
        {
            if (DX_Input.IsInContact)
            {
                StackPanel lb = u as StackPanel;
                lb.SlideTime = DX_Input.EventTime;
                lb.Velocities = lb.Velocities1;
                int os = lb.Offset;
                if (lb.Hrizon)
                {
                    lb.Velocities1 = DX_Input.VelocitiesX;
                    os -= (int)DX_Input.Motion.X;
                }
                else
                {
                    lb.Velocities1 = DX_Input.VelocitiesY;
                    os -= (int)DX_Input.Motion.Y;
                }
                if (lb.SetOffset(os))
                    if (lb.ViewChanged != null)
                        lb.ViewChanged(lb);
            }
        }
        static void PointerLeaveA(UIElement u, Vector2 dot)
        {
            if (DX_Input.IsInContact)
            {
                StackPanel lb = u as StackPanel;
                lb.SlideTime = DX_Input.EventTime;
                lb.Velocities = lb.Velocities1;
                int os = lb.Offset;
                if (lb.Hrizon)
                {
                    lb.Velocities1 = DX_Input.VelocitiesX;
                    os -= (int)DX_Input.Motion.X;
                }
                else
                {
                    lb.Velocities1 = DX_Input.VelocitiesY;
                    os -= (int)DX_Input.Motion.Y;
                }
                lb.SetOffset(os);
            }
        }
        static void MouseWheelA(UIElement u, Vector2 dot)
        {
            StackPanel lb = u as StackPanel;
            int os = lb.Offset;
            os -= DX_Input.MouseWheelDelta;
            if(lb.SetOffset(os))
            {
                if (lb.ViewChanged != null)
                    lb.ViewChanged(lb);
            }
        }

        public List<BindingMod> Data;
        public BindingElement[] ItemTemplate;
        public StackPanel()
        {
            Tag = UITag.Panel;
            Tag2 = PanelTag.StackPanel;
            Data = new List<BindingMod>();
            PointerPress = PointerPressA;
            PointerRelease = PointerReleaseA;
            PointerMove = PointerMoveA;
            PointerLeave = PointerLeaveA;
            MouseWheel = MouseWheelA;
            CheckDot = CheckRect;
        }
        ~StackPanel()
        {
            Dispose();
        }
        Vector2 OriginPoiter;
        float Velocities = 0;
        float Velocities1 = 0;
        int SlideTime;
        public float DecayRate = 0.998f;
        public UIElement BackGroundTemplate;
        public UIElement FillTemplate;
        public Action<UIElement> SelectChanged;
        public Action<UIElement> ItemClick;
        public Action<UIElement> ViewChanged;
        public bool Hrizon;
        public int ItemWidth=60;
        public int ItemHeight=60;
        public float BorderStrok=1;
        public RawColor4 BackColor;
        public RawColor4 BorderColor;
        public int Offset { get; private set; }
        public int SelectedIndex { get; set; }
        public bool SetOffset(int offset)
        {
            if (offset < 0)
                offset = 0;
            int len;
            if (Hrizon)
            {
                len = ItemWidth * Data.Count;
                if (len < size.Width)
                    return false;
                if (offset + Size.Width > len)
                    offset = len - (int)Size.Width;
            }
            else
            {
                len = ItemHeight * Data.Count;
                if (len <= size.Height)
                    return false;
                if (offset + Size.Height > len)
                    offset = len - (int)Size.Height;
            }
            Offset = offset;
            if (Visble)
            {
                ThreadManage.UpdateUI = true;
                Update = true;
            }
            return true;
        }
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        internal override void DrawUI(D2DEnviroment d2d, D2D1.Effect effect)
        {
            if (ItemTemplate == null)
                return;
            if (Velocities != 0)
                DuringSlide();
            if (!Update)
                if (!GaussianBack)
                    return;
            if (bitmap == null)
                return;
            int s = Offset;
            int drawindex = Offset / ItemHeight;
            int r = Offset % ItemHeight;
            int os = drawindex * ItemHeight;
            os = -r;
            D2D1.DeviceContext context = d2d.d2dContext;
            context.Target = bitmap;
            context.BeginDraw();
            context.Transform = Matrix3x2.Identity;
            context.Clear(Background);
            if (GaussianBack)
                if (effect != null)
                {
                    context.DrawImage(effect, new RawVector2(0 - Location.X, 0 - Location.Y));
                    if (Background.A > 0)
                    {
                        d2d.ColorBrush.Color = Background;
                        context.FillRectangle(new RawRectangleF(0, 0, size.Width, size.Height), d2d.ColorBrush);
                    }
                }
            Vector2 offset;
            if(Hrizon)
            {
                offset.X = os+2;
                offset.Y = 2;
                for (int i = drawindex; i < Data.Count; i++)
                {
                    DrawItem(d2d, Data[i], offset);
                    offset.Y += ItemHeight;
                    if (offset.Y >= Size.Height)
                        break;
                }
            }
            else
            {
                offset.X = 2;
                offset.Y = os + 2;
                for (int i = drawindex; i < Data.Count; i++)
                {
                    DrawItem(d2d, Data[i], offset);
                    offset.Y += ItemHeight;
                    if (offset.Y >= Size.Height)
                        break;
                }
            }
            context.Transform = Matrix3x2.Identity;
            if (BackGroundTemplate != null)
            {
                if (BackGroundTemplate.UpdateUI != null)
                    BackGroundTemplate.UpdateUI(d2d, BackGroundTemplate);
            }
            else if(BorderColor.A>0)
            {
                d2d.ColorBrush.Color = BorderColor;
                context.DrawRectangle(new RawRectangleF(0, 0, Size.Width , Size.Height ), d2d.ColorBrush, BorderStrok);
            }
            context.EndDraw();
            Update = false;
        }
        void DrawItem(D2DEnviroment d2d ,BindingMod item,Vector2 offset)
        {
            var e = ItemTemplate;
            int c = e.Length;
            item.Parent = this;
            for (int i = 0; i < c; i++)
            {
                var u = e[i].UI as UIElement;
                if (e[i].SetData != null)
                    e[i].SetData(u, item);
                u.Location = offset + e[i].RawLocation;
                if (u.UpdateUI != null)
                    u.UpdateUI(d2d, u);
            }
        }
        void DuringSlide()
        {
            if (Velocities > 0)
            {
                if (Velocities < 0.005f)
                { Velocities = 0; return; }
            }
            else
            {
                if (Velocities > -0.005f)
                { Velocities = 0; return; }
            }
            int t = DateTime.Now.Millisecond;
            int c = t - SlideTime;
            SlideTime = t;
            if (c < 0)
                c += 1000;
            int a = c;
            float v = DecayRate;
            while (c > 1)
            {
                v *= v;
                c >>= 1;
            }
            if (c > 0)
                v *= DecayRate;
            float v1 = (DecayRate + v) * 0.5f;
            float os = Offset - Velocities * v1 * a;
            Velocities *= v;
            if (SetOffset((int)os))
            {
                if (ViewChanged != null)
                    ViewChanged(this);
            }
            else Velocities = 0;
        }
    }
    public sealed class GridPanel : UIPanel
    {
        static void CheckEvent(GridPanel gp, Vector2 dot)
        {
            if (gp.EventIndex < 0)
                return;
            int c = gp.ItemTemplate.Length;
            c--;
            for (int i = c; i >= 0; i--)
            {
                UIElement u = gp.ItemTemplate[i].UI;
                if (CheckEvent(u, dot))
                    if (!u.EventCross)
                        break;
            }
        }
        static void PointerPressA(UIElement u, Vector2 dot)
        {
            GridPanel lb = u as GridPanel;
            lb.OperationTime = DX_Input.EventTicks;
            lb.SlideTime = DX_Input.EventTime;
            lb.OriginPoiter = DX_Input.Position;
            lb.Velocities = 0;
            dot -= lb.location;
        }
        static void PointerReleaseA(UIElement u, Vector2 dot)
        {
            GridPanel lb = u as GridPanel;
            float x = DX_Input.Position.X - lb.OriginPoiter.X;
            float y = DX_Input.Position.Y - lb.OriginPoiter.Y;
            if (x > -10 & x < 10)
                if (y > -10 & y < 10)
                {
                    long t = DX_Input.EventTicks;
                    if (t - lb.OperationTime < PressTime)
                    {
                        dot -= lb.Location;
                        int dx = (int)dot.X;
                        int r = dx / lb.ItemWidth;
                        int dy = (int)dot.Y;
                        int o = lb.Offset;
                        o += dy;
                        o /= lb.ItemHeight;
                        int c = (int)lb.size.Width / lb.ItemWidth;
                        o *= c;
                        o += r;
                        if (o >= lb.Data.Count)
                            o = -1;
                        lb.ClickIndex = o;
                        if (lb.ItemClick != null)
                            lb.ItemClick(u,dot);
                    }
                    lb.OperationTime = t;
                    dot -= lb.location;
                    return;
                }
            if (lb.Velocities1 < 0)
            {
                if (lb.Velocities > lb.Velocities1)
                    lb.Velocities = lb.Velocities1;
            }
            else
            {
                if (lb.Velocities < lb.Velocities1)
                    lb.Velocities = lb.Velocities1;
            }
        }
        static void PointerMoveA(UIElement u, Vector2 dot)
        {
            GridPanel lb = u as GridPanel;
            if (DX_Input.IsInContact)
            {
                lb.SlideTime = DX_Input.EventTime;
                lb.Velocities = lb.Velocities1;
                int os = lb.Offset;
                lb.Velocities1 = DX_Input.VelocitiesY;
                os -= (int)DX_Input.Motion.Y;
                if( lb.SetOffset(os))
                {
                    if (lb.ViewChanged != null)
                        lb.ViewChanged(lb);
                }
            }
            dot -= lb.location;
        }
        static void PointerLeaveA(UIElement u, Vector2 dot)
        {
            GridPanel lb = u as GridPanel;
            if (DX_Input.IsInContact)
            {
                lb.SlideTime = DX_Input.EventTime;
                lb.Velocities = lb.Velocities1;
                int os = lb.Offset;
                lb.Velocities1 = DX_Input.VelocitiesY;
                os -= (int)DX_Input.Motion.Y;
                lb.SetOffset(os);
            }
            dot -= lb.location;
        }
        static void MouseWheelA(UIElement u, Vector2 dot)
        {
            GridPanel lb = u as GridPanel;
            int os = lb.Offset;
            os -= DX_Input.MouseWheelDelta;
            if(lb.SetOffset(os))
            {
                if (lb.ViewChanged != null)
                    lb.ViewChanged(lb);
            }
        }

        public List<BindingMod> Data;
        public BindingElement[] ItemTemplate;
        public GridPanel()
        {
            Tag = UITag.Panel;
            Tag2 = PanelTag.StackPanel;
            Data = new List<BindingMod>();
            PointerPress = PointerPressA;
            PointerRelease = PointerReleaseA;
            PointerMove = PointerMoveA;
            PointerLeave = PointerLeaveA;
            MouseWheel = MouseWheelA;
            CheckDot = CheckRect;
            EventIndex = -1;
        }
        ~GridPanel()
        {
            Dispose();
        }
        Vector2 OriginPoiter;
        float Velocities = 0;
        float Velocities1 = 0;
        int SlideTime;
        public float DecayRate = 0.998f;
        public UIElement BackGroundTemplate;
        public UIElement FillTemplate;
        public Action<UIElement,Vector2> ItemClick;
        public Action<UIElement> ViewChanged;
        public int ItemWidth=60;
        public int ItemHeight=60;
        public float BorderStrok=1;
        public RawColor4 BackColor;
        public RawColor4 BorderColor;
        public Action<GridPanel> Scrolling;
        public int Offset { get; private set; }
        public int EventIndex { get; private set; }
        public int ClickIndex { get; private set; }
        public int Length { get; private set; }
        public bool SetOffset(int offset)
        {
            ThreadManage.UpdateUI = true;
            Update = true;
            if (offset < 0)
            { offset = 0; return false; }
            int all = Data.Count;
            int c = (int)size.Width / ItemWidth;
            int l = all / c;
            if (all % c > 0)
                l++;
            Length = ItemHeight * l;
            if (Length <= size.Height)
                return false;
            if (offset + Size.Height > Length)
                offset = Length - (int)Size.Height;
            Offset = offset;
            return true;
        }
        public override void Dispose()
        {
            bitmap.Dispose();
            for (int i = 0; i < Data.Count; i++)
            {
                (Data[i] as IDisposable).Dispose();
            }
            GC.SuppressFinalize(this);
        }
        internal override void DrawUI(D2DEnviroment d2d, D2D1.Effect effect)
        {
            if (ItemTemplate == null)
                return;
            if (Velocities != 0)
                DuringSlide();
            if (!Update)
                if (!GaussianBack)
                    return;
            if (bitmap == null)
                return;
            int s = Offset;
            int drawindex = Offset / ItemHeight;
            int count = (int)size.Width / ItemWidth;
            drawindex *= count;
            int r = Offset % ItemHeight;
            int os = drawindex * ItemHeight;
            os = -r;
            D2D1.DeviceContext context = d2d.d2dContext;
            context.Target = bitmap;
            context.BeginDraw();
            context.Transform = Matrix3x2.Identity;
            context.Clear(Background);
            if (GaussianBack)
                if (effect != null)
                {
                    context.DrawImage(effect, new RawVector2(0 - Location.X, 0 - Location.Y));
                    if (Background.A > 0)
                    {
                        d2d.ColorBrush.Color = Background;
                        context.FillRectangle(new RawRectangleF(0, 0, size.Width, size.Height), d2d.ColorBrush);
                    }
                }
            Vector2 offset;
            offset.X = 2;
            offset.Y = os + 2;
            for (int i = drawindex; i < Data.Count; i++)
            {
                DrawItem(d2d, Data[i], offset);
                offset.X += ItemWidth;
                if (offset.X + ItemWidth > Size.Width)
                {
                    offset.X = 2;
                    offset.Y += ItemHeight;
                    if (offset.Y >= Size.Height)
                        break;
                }
            }
            context.Transform = Matrix3x2.Identity;
            if (BackGroundTemplate != null)
            {
                if (BackGroundTemplate.UpdateUI != null)
                    BackGroundTemplate.UpdateUI(d2d, BackGroundTemplate);
            }
            else if (BorderColor.A > 0)
            {
                d2d.ColorBrush.Color = BorderColor;
                context.DrawRectangle(new RawRectangleF(0, 0, Size.Width, Size.Height), d2d.ColorBrush, BorderStrok);
            }
            context.EndDraw();
            Update = false;
        }
        void DrawItem(D2DEnviroment d2d, BindingMod item, Vector2 offset)
        {
            var e = ItemTemplate;
            int c = e.Length;
            item.Parent = this;
            for (int i = 0; i < c; i++)
            {
                
                var u = e[i].UI as UIElement;
                if (e[i].SetData != null)
                    e[i].SetData(u, item);
                u.Location = offset + e[i].RawLocation;
                if (u.UpdateUI != null)
                    u.UpdateUI(d2d, u);
            }
        }
        void DuringSlide()
        {
            if (Velocities > 0)
            {
                if (Velocities < 0.005f)
                { Velocities = 0; return; }
            }
            else
            {
                if (Velocities > -0.005f)
                { Velocities = 0; return; }
            }
            int t = DateTime.Now.Millisecond;
            int c = t - SlideTime;
            SlideTime = t;
            if (c < 0)
                c += 1000;
            int a = c;
            float v = DecayRate;
            while (c > 1)
            {
                v *= v;
                c >>= 1;
            }
            if (c > 0)
                v *= DecayRate;
            float v1 = (DecayRate + v) * 0.5f;
            float os = Offset - Velocities * v1 * a;
            Velocities *= v;
            if( SetOffset((int)os))
            {
                if (ViewChanged != null)
                    ViewChanged(this);
            }
            else
            {
                Velocities = 0;
            }
        }
    }
    public sealed class GridBox : UIPanel
    {
        static void PointerPressA(UIElement u, Vector2 dot)
        {
            GridBox lb = u as GridBox;
            lb.OperationTime = DX_Input.EventTicks;
            lb.SlideTime = DX_Input.EventTime;
            lb.OriginPoiter = DX_Input.Position;
            lb.Velocities = 0;
        }
        static void PointerReleaseA(UIElement u, Vector2 dot)
        {
            GridBox lb = u as GridBox;
            float x = DX_Input.Position.X - lb.OriginPoiter.X;
            float y = DX_Input.Position.Y - lb.OriginPoiter.Y;
            if (x > -10 & x < 10)
                if (y > -10 & y < 10)
                {
                    long t = DX_Input.EventTicks;
                    if (t - lb.OperationTime < PressTime)
                    {
                        dot -= lb.Location;
                        int dx = (int)dot.X;
                        int r = dx / lb.ItemWidth;
                        int dy = (int)dot.Y;
                        int o = lb.Offset;
                        o += dy;
                        o /= lb.ItemHeight;
                        int c = (int)lb.size.Width / lb.ItemWidth;
                        o *= c;
                        o += r;
                        if (o >= lb.Data.Count)
                            o = -1;
                        if (lb.SelectedIndex != o)
                        {
                            lb.SelectedIndex = o;
                            if (lb.SelectChanged != null)
                                lb.SelectChanged(u);
                        }
                        else if (lb.ItemClick != null)
                            lb.ItemClick(u);
                    }
                    lb.OperationTime = t;
                    lb.Update = true;
                    return;
                }
            if (lb.Velocities1 < 0)
            {
                if (lb.Velocities > lb.Velocities1)
                    lb.Velocities = lb.Velocities1;
            }
            else
            {
                if (lb.Velocities < lb.Velocities1)
                    lb.Velocities = lb.Velocities1;
            }
        }
        static void PointerMoveA(UIElement u, Vector2 dot)
        {
            if (DX_Input.IsInContact)
            {
                GridBox lb = u as GridBox;
                lb.SlideTime = DX_Input.EventTime;
                lb.Velocities = lb.Velocities1;
                int os = lb.Offset;
                lb.Velocities1 = DX_Input.VelocitiesY;
                os -= (int)DX_Input.Motion.Y;
                lb.SetOffset(os);
            }
        }
        static void PointerLeaveA(UIElement u, Vector2 dot)
        {
            if (DX_Input.IsInContact)
            {
                GridBox lb = u as GridBox;
                lb.SlideTime = DX_Input.EventTime;
                lb.Velocities = lb.Velocities1;
                int os = lb.Offset;
                lb.Velocities1 = DX_Input.VelocitiesY;
                os -= (int)DX_Input.Motion.Y;
                lb.SetOffset(os);
            }
        }
        static void MouseWheelA(UIElement u, Vector2 dot)
        {
            GridBox lb = u as GridBox;
            int os = lb.Offset;
            os -= DX_Input.MouseWheelDelta;
            lb.SetOffset(os);
        }

        Vector2 OriginPoiter;
        float Velocities = 0;
        float Velocities1 = 0;
        int SlideTime;
        public float DecayRate = 0.998f;
        public UIElement BackGroundTemplate;
        public UIElement FillTemplate;
        public TextBlock ItemTemplate { get; private set; }
        public List<string> Data;
        public Action<UIElement> SelectChanged;
        public Action<UIElement> ItemClick;
        public int ItemWidth = 60;
        public int ItemHeight = 20;
        public float BorderStrok = 1;
        public RawColor4 BorderColor;
        public RawColor4 SelectFillColor;
        public RawColor4 SelectTextColor = new RawColor4(1, 1, 1, 1);
        public int Offset { get; private set; }
        public int SelectedIndex { get; set; }
        public int Length { get; private set; }
        public GridBox()
        {
            Tag = UITag.Panel;
            Tag2 = PanelTag.GridBox;
            Data = new List<string>();
            ItemTemplate = new TextBlock();
            ItemTemplate.Alignment = TextAlignment.Center;
            ItemHeight = 24;
            PointerPress = PointerPressA;
            PointerRelease = PointerReleaseA;
            PointerMove = PointerMoveA;
            PointerLeave = PointerLeaveA;
            MouseWheel = MouseWheelA;
            CheckDot = CheckRect;
            SelectedIndex = -1;
        }
        ~GridBox()
        {
            Dispose();
        }
        public void SetOffset(int offset)
        {
            if (offset < 0)
                offset = 0;
            int all = Data.Count;
            int c = (int)size.Width / ItemWidth;
            int l = all / c;
            if (all % c > 0)
                l++;
            Length = ItemHeight * l;
            if (Length <= size.Height)
                return;
            if (offset + Size.Height > Length)
                offset = Length - (int)Size.Height;
            Offset = offset;
            Update = true;
            ThreadManage.UpdateUI = true;
        }
        public override void Dispose()
        {
            ItemTemplate.Dispose();
            GC.SuppressFinalize(this);
        }
        internal override void DrawUI(D2DEnviroment d2d, D2D1.Effect effect)
        {
            if (Velocities != 0)
                DuringSlide();
            if (!Update)
                if (!GaussianBack)
                    return;
            if (bitmap == null)
                return;
            D2D1.DeviceContext context = d2d.d2dContext;
            context.Target = bitmap;
            context.BeginDraw();
            context.Transform = Matrix3x2.Identity;
            context.Clear(Background);
            if (GaussianBack)
                if (effect != null)
                {
                    context.DrawImage(effect, new RawVector2(0 - Location.X, 0 - Location.Y));
                    if (Background.A > 0)
                    {
                        d2d.ColorBrush.Color = Background;
                        context.FillRectangle(new RawRectangleF(0, 0, size.Width, size.Height), d2d.ColorBrush);
                    }
                }
            DrawData(d2d);
            if (BackGroundTemplate != null)
            {
                if (BackGroundTemplate.UpdateUI != null)
                    BackGroundTemplate.UpdateUI(d2d, BackGroundTemplate);
            }
            else if (BorderColor.A > 0)
            {
                d2d.ColorBrush.Color = BorderColor;
                context.DrawRectangle(new RawRectangleF(0, 0, Size.Width, Size.Height), d2d.ColorBrush, BorderStrok);
            }
            context.EndDraw();
            Update = false;
        }
        void DrawData(D2DEnviroment d2d)
        {
            RawRectangleF rect;
            if (ItemHeight <= 0)
                return;
            TextFormat tf = TextBlock.GetFormat(ItemTemplate.FontStyle, ItemTemplate.FontSize);
            tf.TextAlignment = ItemTemplate.Alignment;
            int s = Offset;
            int drawindex = Offset / ItemHeight;
            int count = (int)size.Width / ItemWidth;
            drawindex *= count;
            int r = Offset % ItemHeight;
            int os = drawindex * ItemHeight;
            os = -r;
            int len = (int)Size.Height;
            float x = 2;
            float y = os + 2;
            rect.Left = 2;
            rect.Top = os + 2;
            rect.Right = ItemWidth + 2; ;
            rect.Bottom = os + ItemHeight +2;
            var render = d2d.d2dContext;
            for (int i = drawindex; i < Data.Count; i++)
            {
                if (SelectedIndex == i)
                {
                    if (FillTemplate != null)
                    {
                        FillTemplate.Location = new Vector2(rect.Left, rect.Top);
                        if (FillTemplate.UpdateUI != null)
                            FillTemplate.UpdateUI(d2d, FillTemplate);
                    }
                    else
                    {
                        d2d.ColorBrush.Color = SelectFillColor;
                        render.FillRectangle(rect, d2d.ColorBrush);
                    }
                    d2d.ColorBrush.Color = SelectTextColor;
                }
                else d2d.ColorBrush.Color = Forground;
                render.DrawText(Data[i], tf, rect, d2d.ColorBrush, (D2D1.DrawTextOptions)2);
                x += ItemWidth;
                if (x + ItemWidth > Size.Width)
                {
                    x = 2;
                    y += ItemHeight;
                    if (y>= Size.Height)
                        break;
                    rect.Top = y;
                    rect.Bottom =y + ItemHeight;
                }
                rect.Left = x;
                rect.Right = x + ItemWidth;
            }
        }
        void DuringSlide()
        {
            if (Velocities > 0)
            {
                if (Velocities < 0.005f)
                { Velocities = 0; return; }
            }
            else
            {
                if (Velocities > -0.005f)
                { Velocities = 0; return; }
            }
            int t = DateTime.Now.Millisecond;
            int c = t - SlideTime;
            SlideTime = t;
            if (c < 0)
                c += 1000;
            int a = c;
            float v = DecayRate;
            while (c > 1)
            {
                v *= v;
                c >>= 1;
            }
            if (c > 0)
                v *= DecayRate;
            float v1 = (DecayRate + v) * 0.5f;
            float os = Offset - Velocities * v1 * a;
            Velocities *= v;
            SetOffset((int)os);
        }
    }
}
