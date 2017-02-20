using System;
using System.Collections.Generic;
using Wic = SharpDX.WIC;
using D2D1 = SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.DirectWrite;
using SharpDX;
using System.IO;
using Windows.Storage.Streams;
using System.Diagnostics;

namespace DX
{

    public enum PointerTag:int
    {
        LeftButton,RgihtButton,MiddleButton,Wheel
    }
    public enum PointerStatus : int
    {
        None,
        Leave=1,
        Released=2,
        Entry=4,
        Pressed=8,
        Move=16,
        MouseWheel=32
    }
    internal delegate void DrawUI(D2DEnviroment d2d, UIElement e);
    internal delegate bool DotInBounds(UIElement ui,Vector2 dot);
    public class UIElement:IDisposable
    {
        public const long PressTime = 1800000;
        static void ClickEventPress(UIElement u, Vector2 dot)
        {
            u.OperationTime = DX_Input.EventTicks;
        }
        static void ClickEventRelease(UIElement u, Vector2 dot)
        {
            long t = DX_Input.EventTicks;
            if (t - u.OperationTime < PressTime)
            {
                if (u.Click != null)
                    u.Click(u);
            }
            u.OperationTime = t;
        }
        public static void SetClick(UIElement u, Action<UIElement> click)
        {
            u.PointerPress = ClickEventPress;
            u.PointerRelease = ClickEventRelease;
            u.Click = click;
        }

        #region EventCheck
        protected static bool CheckRect(UIElement u,Vector2 dot)
        {
            if (u.Angle == 0)
            {
                float x = dot.X;
                float y = dot.Y;
                if (x > u.Rect.Left & x < u.Rect.Right)
                    if (y > u.Rect.Top & y < u.Rect.Bottom)
                        return true;
                return false;
            }
            if (u.vertex == null)
                return false;
            return MathF.DotToPolygon(u.vertex, dot);
        }
        //protected static bool CheckRoundRect(UIElement u,Vector2 dot)
        //{
        //    //检测大四边形
        //    //检测四个角小四边形
        //    //检测四个角椭圆
        //    RoundBorder rb = u as RoundBorder;
        //    if(CheckRect(u,dot))
        //    {
        //        if(MathF.DotToPolygon(rb.vertex3,dot))
        //        {
        //            if (MathF.DotToEllipse(rb.vertex2[0], dot, rb.roundrect.RadiusX, rb.roundrect.RadiusY))
        //                return true;
        //            return false;
        //        }
        //        else if(MathF.DotToPolygon(rb.vertex4, dot))
        //        {
        //            if (MathF.DotToEllipse(rb.vertex2[1], dot, rb.roundrect.RadiusX, rb.roundrect.RadiusY))
        //                return true;
        //            return false;
        //        }
        //        else if (MathF.DotToPolygon(rb.vertex5, dot))
        //        {
        //            if (MathF.DotToEllipse(rb.vertex2[2], dot, rb.roundrect.RadiusX, rb.roundrect.RadiusY))
        //                return true;
        //            return false;
        //        }
        //        else if (MathF.DotToPolygon(rb.vertex6, dot))
        //        {
        //            if (MathF.DotToEllipse(rb.vertex2[3], dot, rb.roundrect.RadiusX, rb.roundrect.RadiusY))
        //                return true;
        //            return false;
        //        }
        //        return true;
        //    }
        //    return false;
        //}
        protected static bool CheckEllipse(UIElement u,Vector2 dot)
        {
            Ellipse ell = u as Ellipse;
            float rx = ell.ellipse.RadiusX;
            float ry = ell.ellipse.RadiusY;
            if (rx==ry )
            {
                float x = ell.ellipse.Point.X - dot.X;
                float y = ell.ellipse.Point.Y - dot.Y;
                return x * x + y * y <= rx * rx;
            }
            return MathF.DotToEllipse(ell.ellipse.Point, dot, rx, ry,ell.Angle);
        }
        internal static void EventAnalysis(UIElement u, Vector2 dot)
        {
            if(DX_Input.PointerUpdateKind==Windows.UI.Input.PointerUpdateKind.Other)
            {
                if (DX_Input.MouseWheelDelta != 0)
                    if (u.MouseWheel != null)
                        u.MouseWheel(u, dot);
                if(DX_Input.IsMoved)
                {
                    if (u.PointStatus < PointerStatus.Entry)
                    {
                        if (u.PointerEntry != null)
                            u.PointerEntry(u,dot);
                        u.PointStatus = PointerStatus.Entry;
                        return;
                    }
                    if (u.PointerMove != null)
                        u.PointerMove(u, dot);
                    u.PointStatus = PointerStatus.Move;
                }
                return;
            }
            if((DX_Input.PointerUpdateKind&Windows.UI.Input.PointerUpdateKind.LeftButtonPressed)>0)
            {
                if (u.PointerPress != null)
                    u.PointerPress(u, dot);
                u.PointStatus = PointerStatus.Pressed;
                return;
            }
            if(u.PointStatus==PointerStatus.Pressed)
            {
                if (u.PointerRelease != null)
                    u.PointerRelease(u, dot);
                u.PointStatus = PointerStatus.Released;
            }
        }
        internal static bool CheckEvent(UIElement u, Vector2 dot)
        {
            if (u.CheckDot != null)
                if (u.CheckDot(u, dot))
                {
                    if (u.PointerAnalysis != null)
                        u.PointerAnalysis(u, dot);
                    else if(!DX_Input.PointerHandled)
                        EventAnalysis(u, dot);
                    return true;
                }
                else
                {
                    if(u.PointStatus>=PointerStatus.Entry)
                    {
                        if (u.PointerLeave != null)
                            u.PointerLeave(u,dot);
                        u.PointStatus = PointerStatus.Leave;
                    }
                }
            return false;
        }
        #endregion

        #region DrawUI Ex
        internal static void DrawText(D2DEnviroment d2d, UIElement u)
        {
            TextBlock tb = u as TextBlock;
            if (tb.Text == null)
                return;
            var render = d2d.d2dContext;
            if (u.Angle == 0)
                render.Transform = Matrix3x2.Identity;
            else render.Transform = tb.Matri;
            if (u.Background.A > 0)
            {
                d2d.ColorBrush.Color = u.Background;
                render.FillRectangle(u.Rect, d2d.ColorBrush);
            }
            D2D1.Brush br;
            if (tb.SurfaceBrush == null)
            {
                d2d.ColorBrush.Color = tb.Forground;
                br = d2d.ColorBrush;
            }
            else {
                br = tb.SurfaceBrush;
                (br as IGradientBrush).SetRect(tb.Rect);
            }
            TextFormat tf = TextBlock.GetFormat(tb.FontStyle,tb.FontSize);
            tf.WordWrapping = tb.Warpping;
            tf.TextAlignment = tb.Alignment;
            render.DrawText(tb.Text, tf, u.Rect,br, (D2D1.DrawTextOptions)2);
        }
        internal static void DrawLine(D2DEnviroment d2d, UIElement u)
        {
            Line l = u as Line;
            D2D1.Brush br;
            if (l.SurfaceBrush == null)
            {
                d2d.ColorBrush.Color = u.Forground;
                br = d2d.ColorBrush;
            }
            else br = l.SurfaceBrush;
            var render = d2d.d2dContext;
            render.Transform = Matrix3x2.Identity;
            if (l.Style == null)
                render.DrawLine(l.Start, l.End, br, l.Storke);
            else
                render.DrawLine(l.Start, l.End, br, l.Storke, l.Style);
        }
        internal static void DrawRect(D2DEnviroment d2d, UIElement u)
        {
            Border bor = u as Border;
            D2D1.Brush br;
            if (bor.SurfaceBrush == null)
            {
                d2d.ColorBrush.Color = u.Forground;
                br = d2d.ColorBrush;
            }
            else br = bor.SurfaceBrush;
            var render = d2d.d2dContext;
            if (u.Angle != 0)
                render.Transform = bor.Matri;
            else render.Transform = Matrix3x2.Identity;
            if (bor.Style == null)
                render.DrawRectangle(bor.Rect, br, bor.Stroke);
            else
                render.DrawRectangle(bor.Rect, br, bor.Stroke, bor.Style);
            if (u.FillBrush != null)
            {
                (u.FillBrush as IGradientBrush).SetRect(u.Rect);
                render.FillRectangle(u.Rect, u.FillBrush);
            }
            else if (u.FillImage != null)
            {
                if (u.FillImage.d2dmap == null)
                    return;
                if (d2d.ImgBrush == null)
                {
                    d2d.ImgBrush = new D2D1.ImageBrush(render, u.FillImage.d2dmap, new D2D1.ImageBrushProperties()
                    {
                        ExtendModeX = D2D1.ExtendMode.Wrap,
                        ExtendModeY = D2D1.ExtendMode.Wrap,
                        InterpolationMode = D2D1.InterpolationMode.HighQualityCubic
                    });
                }
                else
                    d2d.ImgBrush.Image = u.FillImage.d2dmap;
                var s = u.FillImage.Bitmap.Size;
                d2d.ImgBrush.SourceRectangle = new RawRectangleF(0, 0, s.Width, s.Height);
                render.FillRectangle(u.Rect, d2d.ImgBrush);
            }
            else if (u.Background.A > 0)
            {
                d2d.ColorBrush.Color = u.Background;
                render.FillRectangle(u.Rect, d2d.ColorBrush);
            }
        }
        internal static void DrawRoundRect(D2DEnviroment d2d, UIElement u)
        {
            RoundBorder bor = u as RoundBorder;
            D2D1.Brush br;
            if (bor.SurfaceBrush == null)
            {
                d2d.ColorBrush.Color = bor.Forground;
                br = d2d.ColorBrush;
            }
            else br = bor.SurfaceBrush;
            var render = d2d.d2dContext;
            if (u.Angle != 0)
                render.Transform = bor.Matri;
            else render.Transform = Matrix3x2.Identity;
            if(u.Forground.A>0)
            {
                if (bor.Style == null)
                    render.DrawRoundedRectangle(bor.RoundRect, br, bor.Stroke);
                else
                    render.DrawRoundedRectangle(bor.RoundRect, br, bor.Stroke, bor.Style);
            }
            if (u.FillBrush != null)
            {
                (u.FillBrush as IGradientBrush).SetRect(bor.RoundRect.Rect);
                render.FillRoundedRectangle(bor.RoundRect, u.FillBrush);
            }
            else if (u.FillImage != null)
            {
                if (u.FillImage.d2dmap == null)
                    return;
                if (d2d.ImgBrush == null)
                {
                    d2d.ImgBrush = new D2D1.ImageBrush(render, u.FillImage.d2dmap, new D2D1.ImageBrushProperties()
                    {
                        ExtendModeX = D2D1.ExtendMode.Wrap,
                        ExtendModeY = D2D1.ExtendMode.Wrap,
                        InterpolationMode = D2D1.InterpolationMode.HighQualityCubic
                    });
                }
                else
                    d2d.ImgBrush.Image = u.FillImage.d2dmap;
                var s = u.FillImage.Bitmap.Size;
                d2d.ImgBrush.SourceRectangle = new RawRectangleF(0, 0, s.Width, s.Height);
                render.FillRoundedRectangle(bor.RoundRect, d2d.ImgBrush);
            }
            else if (u.Background.A > 0)
            {
                d2d.ColorBrush.Color = u.Background;
                render.FillRoundedRectangle(bor.RoundRect, d2d.ColorBrush);
            }
        }
        internal static void DrawImage(D2DEnviroment d2d, UIElement u)
        {
            UIImage img = u as UIImage;
            if (img.d2dmap == null)
                return;
            var render = d2d.d2dContext;
            if (u.Angle != 0)
                render.Transform = u.Matri;
            else render.Transform = Matrix3x2.Identity;
            render.DrawBitmap(img.d2dmap, img.Rect, img.opacity, D2D1.BitmapInterpolationMode.NearestNeighbor);
        }
        internal static void DrawCloneImage(D2DEnviroment d2d, UIElement u)
        {
            CloneImage cimg = u as CloneImage;
            UIImage img = cimg.img;
            if (img.d2dmap == null)
                return;
            var render = d2d.d2dContext;
            if (u.Angle != 0)
                render.Transform = u.Matri;
            else render.Transform = Matrix3x2.Identity;
            render.DrawBitmap(img.d2dmap, cimg.Rect, cimg.opacity, D2D1.BitmapInterpolationMode.NearestNeighbor);
        }
        internal static void DrawEllipse(D2DEnviroment d2d, UIElement u)
        {
            Ellipse ell = u as Ellipse;
            var render = d2d.d2dContext;
            if (u.Angle != 0)
                render.Transform = ell.Matri;
            else render.Transform = Matrix3x2.Identity;
            D2D1.Brush br;
            if (u.SurfaceBrush == null)
            {
                d2d.ColorBrush.Color = ell.Forground;
                br = d2d.ColorBrush;
            }
            else br = u.SurfaceBrush;
            render.DrawEllipse(ell.ellipse, br, ell.Stroke);
            if(u.FillBrush!=null)
            {
                RawRectangleF rect = new RawRectangleF();
                rect.Left = ell.ellipse.Point.X - ell.ellipse.RadiusX;
                rect.Right = ell.ellipse.Point.X + ell.ellipse.RadiusX;
                rect.Top = ell.ellipse.Point.Y - ell.ellipse.RadiusY;
                rect.Bottom = ell.ellipse.Point.Y + ell.ellipse.RadiusY;
                (u.FillBrush as IGradientBrush).SetRect(rect);
                render.FillEllipse(ell.ellipse, u.FillBrush);
            }
            else if(u.FillImage!=null)
            {
                if (u.FillImage.d2dmap == null)
                    return;
                if (d2d.ImgBrush == null)
                {
                    d2d.ImgBrush = new D2D1.ImageBrush(render, u.FillImage.d2dmap, new D2D1.ImageBrushProperties()
                    {
                        ExtendModeX = D2D1.ExtendMode.Wrap,
                        ExtendModeY = D2D1.ExtendMode.Wrap,
                        InterpolationMode = D2D1.InterpolationMode.HighQualityCubic
                    });
                } 
                else
                    d2d.ImgBrush.Image = u.FillImage.d2dmap;
                var s = u.FillImage.Bitmap.Size;
                d2d.ImgBrush.SourceRectangle = new RawRectangleF(0,0,s.Width,s.Height);
                render.FillEllipse(ell.ellipse, d2d.ImgBrush);
            }
            else if (u.Background.A > 0)
            {
                d2d.ColorBrush.Color = u.Background;
                render.FillEllipse(ell.ellipse, d2d.ColorBrush);
            }
        }
        internal static void DrawGeometry(D2DEnviroment d2d, UIElement u)
        {
            Geometry geo = u as Geometry;
            var render = d2d.d2dContext;
            if (u.Angle != 0)
                render.Transform = geo.Matri;
            else render.Transform = Matrix3x2.Identity;
            D2D1.Brush br;
            if (geo.SurfaceBrush == null)
            {
                d2d.ColorBrush.Color = geo.Forground;
                br = d2d.ColorBrush;
            }
            else br = geo.SurfaceBrush;
            render.DrawGeometry(geo.geometry, br);
            if (u.FillBrush != null)
                render.FillGeometry(geo.geometry, u.FillBrush);
            else if (u.FillImage != null)
            {
                if (u.FillImage.d2dmap == null)
                    return;
                if (d2d.ImgBrush == null)
                {
                    d2d.ImgBrush = new D2D1.ImageBrush(render, u.FillImage.d2dmap, new D2D1.ImageBrushProperties()
                    {
                        ExtendModeX = D2D1.ExtendMode.Wrap,
                        ExtendModeY = D2D1.ExtendMode.Wrap,
                        InterpolationMode = D2D1.InterpolationMode.HighQualityCubic
                    });
                }
                else
                    d2d.ImgBrush.Image = u.FillImage.d2dmap;
                var s = u.FillImage.Bitmap.Size;
                d2d.ImgBrush.SourceRectangle = new RawRectangleF(0, 0, s.Width, s.Height);
                render.FillGeometry(geo.geometry, d2d.ImgBrush);
            }
            else if (u.Background.A > 0)
            {
                d2d.ColorBrush.Color = u.Background;
                render.FillGeometry(geo.geometry, d2d.ColorBrush);
            }
        }
        #endregion

        #region property
        public enum UITag { Line, Text,  Border,RoundBorder, Image,CloneImage, Ellipse,Geometry,
        Panel}
        public UITag Tag { get; protected set; }
        public bool EventCross;
        public PointerStatus PointStatus;
        internal Action<UIElement, Vector2> PointerAnalysis;
        public Action<UIElement, Vector2> MouseWheel;
        public Action<UIElement, Vector2> PointerEntry;
        public Action<UIElement, Vector2> PointerMove;
        public Action<UIElement, Vector2> PointerLeave;
        public Action<UIElement, Vector2> PointerPress;
        public Action<UIElement, Vector2> PointerRelease;
        Action<UIElement> Click;
        internal DrawUI UpdateUI;
        internal DotInBounds CheckDot;
        public RawRectangleF Rect;
        public object Context;
        public float Angle;
        protected long OperationTime;
        internal Vector2[] vertex;
        internal Matrix3x2 Matri;
        internal Vector2 Center;
        internal RawVector2 Origin;
        public void Rotation()
        {
            float x = Rect.Left;
            float x1 =Rect.Right;
            float y = Rect.Top;
            float y1 =Rect.Bottom;
            vertex = new Vector2[4];
            vertex[0].X = x;
            vertex[0].Y = y;
            vertex[1].X = x1;
            vertex[1].Y = y;
            vertex[2].X = x1;
            vertex[2].Y = y1;
            vertex[3].X = x;
            vertex[3].Y = y1;
            float w = x1 - x;
            float h = y1 - y;
            Center = new Vector2(x + 0.5f * w, y+h+5);
            vertex = MathF.RotatePoint2(vertex,Center, 360-Angle);
            float a = Angle * 0.01666666f;
            Center = new Vector2(x + 0.5f * w, y + 0.5f * h);
            Matri = Matrix3x2.Rotation(a, Center);
            Origin = Center;
        }
        internal object DataContext;
        public bool Visble=true;
        public D2D1.Brush SurfaceBrush;
        public D2D1.Brush FillBrush;
        public UIImage FillImage;
        public RawColor4 Background;
        public RawColor4 Forground;
        public virtual void Dispose() {
            GC.SuppressFinalize(this);
        }
        protected Size2F size;
        public virtual Size2F Size { get { return size; }set {
                size = value;
                Rect.Left = location.X;
                Rect.Right = location.X + size.Width;
                Rect.Top = location.Y;
                Rect.Bottom = location.Y + size.Height;
            } }
        protected Vector2 location;
        public virtual Vector2 Location { get { return location; } set {
                location = value;
                Rect.Left = location.X ;
                Rect.Right = location.X + size.Width;
                Rect.Top = location.Y;
                Rect.Bottom = location.Y + size.Height;
            } }
        #endregion

    }
    public sealed class TextBlock : UIElement
    {
        public enum FontName
        {
            新宋体, SegoeUISymbol
        }
        static TextFormat[] textformat=new TextFormat[73];
        static TextFormat[] uiformat=new TextFormat[73];
        internal static TextFormat GetFormat(FontName name, int size)
        {
            if(name ==FontName.新宋体)
            {
                if(textformat[size]==null)
                    textformat[size] = new TextFormat(fac, "新宋体", size);
                return textformat[size];
            }
            else
            {
                if (uiformat[size] == null)
                    uiformat[size] = new TextFormat(fac, "Segoe UI Symbol",size);
                return uiformat[size];
            }
        }
        public const int defsize = 14;
        static Factory fac;
        public TextBlock()
        {
            Tag = UITag.Text;
            EventCross = true;
            Angle = 0;
            fontsize = defsize;
            UpdateUI = DrawText;
            CheckDot = CheckRect;
            if(fac==null)
            {
                fac = new Factory();
                textformat[fontsize] = new TextFormat(fac, "新宋体", defsize);
                textformat[fontsize].TextAlignment = TextAlignment.Center;
            }
        }
        public FontName FontStyle { get; set; }
        public string Text { get; set; }
        public TextAlignment Alignment;
        public WordWrapping Warpping;
        public Color4 PointerDockColor;
        public float Width { get; set; }
        public float Height { get; set; }
        int fontsize;
        public int FontSize { get { return fontsize; } set {
               fontsize = value;
                if (fontsize > 72)
                    fontsize=72;
                if (fontsize < 1)
                    fontsize = 1;
            } }    
        ~TextBlock()
        {
            Dispose();
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
    public sealed class Line:UIElement
    {
        public Line()
        {
            Tag = UITag.Line;
            UpdateUI = DrawLine;
            CheckDot = CheckRect;
        }
        ~Line()
        {
            Dispose();
        }
        public override void Dispose()
        {
            if(Style!=null)
            {
                Style.Dispose();
                Style = null;
            }
            base.Dispose();
        }
        public RawVector2 Start { get; set; }
        public RawVector2 End { get; set; }
        public Vector2 StartOffset;
        public Vector2 EndOffset;
        public float Storke;
        public D2D1.StrokeStyle Style;
        public override Vector2 Location
        {
           get { return location; }
            set { location = value;
                Start = location + StartOffset;
                End = location + EndOffset;
            }
        }
    }
    public sealed class Border:UIElement
    {
        public Border()
        {
            Tag = UITag.Border;
            UpdateUI = DrawRect;
            CheckDot = CheckRect;
        }
        ~Border()
        {
            if (Style != null)
                Style.Dispose();
        }
        public float Stroke=1;
        public D2D1.StrokeStyle Style;
        public override void Dispose()
        {
            if (Style != null)
            {
                Style.Dispose();
                Style = null;
            }
            base.Dispose();
        }
    }
    public struct BitmapSource
    {
        public Wic.BitmapDecoder Decoder;
        public Wic.BitmapFrameDecode FrameDecoder;
        public Wic.FormatConverter Converter;
    }
    public sealed class UIImage:UIElement
    {
        public async static void LoadLocalSource(string path, UIImage img)
        {
            Uri u = new Uri(path);
            RandomAccessStreamReference rass = RandomAccessStreamReference.CreateFromUri(u);
            IRandomAccessStream ir = await rass.OpenReadAsync();
            Stream s = ir.AsStream();
            LoadSource(s, img);
            img.iras = ir;
            
        }
        public static BitmapSource CreateWicBitmap(Stream str)
        {
            BitmapSource bs = new BitmapSource();
            Wic.ImagingFactory fac = ThreadResource.GetWicFactory();
            bs.Decoder = new Wic.BitmapDecoder(fac, str, Wic.DecodeOptions.CacheOnLoad);
            bs.FrameDecoder = bs.Decoder.GetFrame(0);
            bs.Converter = new Wic.FormatConverter(fac);
            bs.Converter.Initialize(bs.FrameDecoder, Wic.PixelFormat.Format32bppPRGBA);
            return bs;
        }
        public static D2D1.Bitmap CreateD2DBitmap(Wic.FormatConverter fconv)
        {
            D2D1.Bitmap map;
            lock (DX_Core.D2D)
            map = D2D1.Bitmap.FromWicBitmap(DX_Core.D2D.d2dContext,fconv);
            return map;
        }
        public static void LoadSource(Stream str, UIImage img)
        {
            Wic.ImagingFactory fac = ThreadResource.GetWicFactory();
            var d = new Wic.BitmapDecoder(fac, str, Wic.DecodeOptions.CacheOnLoad);
            if (img.data != null)
                ClearResource(img);
            img.data = str;
            img.Decoder = d;
            var frame = d.GetFrame(0);
            img.FrameDecoder = frame;
            var fconv = new Wic.FormatConverter(fac);
            fconv.Initialize(frame, Wic.PixelFormat.Format32bppPRGBA);
            img.Bitmap = fconv;
            img.Mapsize.Left = 0;
            img.Mapsize.Top = 0;
            img.Mapsize.Right = img.Bitmap.Size.Width;
            img.Mapsize.Bottom = img.Bitmap.Size.Height;
            lock(DX_Core.D2D)
            img.d2dmap = D2D1.Bitmap.FromWicBitmap(DX_Core.D2D.d2dContext, img.Bitmap);
        }
        public static void ClearResource(UIImage img)
        {
            if (img.d2dmap != null)
            {
                img.d2dmap.Dispose();
                img.d2dmap = null;
            }
            if(img.Bitmap!=null)
            {
                img.Bitmap.Dispose();
                img.Bitmap = null;
            }
            if(img.FrameDecoder!=null)
            {
                img.FrameDecoder.Dispose();
                img.FrameDecoder = null;
            }
            if(img.Decoder!=null)
            {
                img.Decoder.Dispose();
                img.Decoder = null;
            }
            if (img.data!=null)
            {
                img.data.Dispose();
                img.data = null;
            }
            if(img.iras!=null)
            {
                img.iras.Dispose();
                img.iras = null;
            }
        }
        public UIImage()
        {
            Tag = UITag.Image;
            opacity = 1;
            UpdateUI = DrawImage;
            CheckDot = CheckRect;
        }
        ~UIImage()
        {
            Dispose();
        }
        public override void Dispose()
        {
            ClearResource(this);
            GC.SuppressFinalize(this);
        }
        internal int d2dmapPoint = -1;
        IRandomAccessStream iras;
        Stream data;
        internal RawRectangleF Mapsize;
        Wic.BitmapDecoder Decoder;
        Wic.BitmapFrameDecode FrameDecoder;
        public Wic.FormatConverter Bitmap;
        public D2D1.Bitmap d2dmap;
        public float opacity;
    }
    public sealed class Ellipse : UIElement
    {
        public Ellipse()
        {
            Tag = UITag.Ellipse;
            Stroke = 1;
            UpdateUI = DrawEllipse;
            CheckDot = CheckEllipse;
        }
        ~Ellipse()
        { Dispose(); }
        public override void Dispose()
        {
            if (Style != null)
            {
                Style.Dispose();
                Style = null;
            }
            base.Dispose();
        }
        public Color4 PointerDockColor;
        public D2D1.Ellipse ellipse;
        public float Stroke;
        public D2D1.StrokeStyle Style;
        public override Size2F Size
        {
            get  { return size; }
            set
            {
                size = value;
                ellipse.RadiusX = size.Width*0.5f;
                ellipse.RadiusY = size.Height * 0.5f;
            }
        }
        public override Vector2 Location
        {
            get { return location; }
            set
            {
                location = value;
                ellipse.Point.X = location.X+ellipse.RadiusX;
                ellipse.Point.Y = location.Y+ellipse.RadiusY;
            }
        }
    }
    public sealed class Geometry : UIElement
    {
        public Geometry()
        {
            Tag = UITag.Geometry;
            UpdateUI = DrawGeometry;
        }
        ~Geometry()
        {
            Dispose();
        }
        public D2D1.ImageBrush Brush;
        public Color4 PointerDockColor;
        public D2D1.Geometry geometry;
        public float Stroke;
        public D2D1.StrokeStyle Style;
        public override void Dispose()
        {
            if (Style != null)
            {
                Style.Dispose();
                Style = null;
            }
            base.Dispose();
        }
    }
    public sealed class RoundBorder:UIElement
    {
        public RoundBorder()
        {
            Tag = UITag.RoundBorder;
            UpdateUI = DrawRoundRect;
            CheckDot = CheckRect;
        }
        ~RoundBorder()
        {
            if (Style != null)
                Style.Dispose();
        }
        public float Stroke=1;
        public D2D1.StrokeStyle Style;
        public D2D1.RoundedRectangle RoundRect;
        public override void Dispose()
        {
            if (Style != null)
            {
                Style.Dispose();
                Style = null;
            }
            base.Dispose();
        }
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
                RoundRect.Rect = Rect;
            }
        }
        public override Vector2 Location
        {
            get { return location; }
            set {
                location = value;
                RoundRect.Rect.Left = location.X ;
                RoundRect.Rect.Right = location.X + size.Width;
                RoundRect.Rect.Top = location.Y ;
                RoundRect.Rect.Bottom = location.Y + size.Height;
                Rect = RoundRect.Rect;
            }
        }
        //internal Vector2[] vertex2;
        //internal Vector2[] vertex3;
        //internal Vector2[] vertex4;
        //internal Vector2[] vertex5;
        //internal Vector2[] vertex6;
        //public void RotationRound()
        //{
        //    float x = Rect.Left;
        //    float x1 = Rect.Right;
        //    float y = Rect.Top;
        //    float y1 = Rect.Bottom;
        //    float ax = x + roundrect.RadiusY;
        //    float ax1 = x1 - roundrect.RadiusX;
        //    float ay = y + roundrect.RadiusY;
        //    float ay1 = y1 - roundrect.RadiusY;
        //    float a = 360 - Angle;
        //    float w = x1 - x;
        //    float h = y1 - y;
        //    Center = new Vector2(x + 0.5f * w, y + h + 5);
        //    Matri = Matrix3x2.Rotation(a, Center);
        //    Origin = Center;
        //    Vector2 dot1 = MathF.RotatePoint2(new Vector2(x, y), Center, a);
        //    Vector2 dot2 = MathF.RotatePoint2(new Vector2(x1, y), Center, a);
        //    Vector2 dot3 = MathF.RotatePoint2(new Vector2(x1, y1), Center, a);
        //    Vector2 dot4 = MathF.RotatePoint2(new Vector2(x, y1), Center, a);
        //    Vector2 dot5 = MathF.RotatePoint2(new Vector2(ax, ay), Center, a);
        //    Vector2 dot6 = MathF.RotatePoint2(new Vector2(ax1, ay), Center, a);
        //    Vector2 dot7 = MathF.RotatePoint2(new Vector2(ax1, ay1), Center, a);
        //    Vector2 dot8 = MathF.RotatePoint2(new Vector2(ax, ay1), Center, a);
        //    vertex = new Vector2[4];
        //    vertex[0].X = x;
        //    vertex[0].Y = y;
        //    vertex[1].X = x1;
        //    vertex[1].Y = y;
        //    vertex[2].X = x1;
        //    vertex[2].Y = y1;
        //    vertex[3].X = x;
        //    vertex[3].Y = y1;

        //    vertex2 = new Vector2[4];
        //    vertex2[0].X = ax;
        //    vertex2[0].Y = ay;
        //    vertex2[1].X = ax1;
        //    vertex2[1].Y = ay;
        //    vertex2[2].X = ax1;
        //    vertex2[2].Y = ay1;
        //    vertex2[3].X = ax;
        //    vertex2[3].Y = ay1;

        //    vertex3 = new Vector2[4];//left top
        //    vertex3[0].X = x;
        //    vertex3[0].Y = y;
        //    vertex3[1].X = ax;
        //    vertex3[1].Y = y;
        //    vertex3[2].X = ax;
        //    vertex3[2].Y = ay;
        //    vertex3[3].X = x;
        //    vertex3[3].Y = ay;

        //    vertex4 = new Vector2[4];//right top
        //    vertex4[0].X = ax1;
        //    vertex4[0].Y = y;
        //    vertex4[1].X = x1;
        //    vertex4[1].Y = y;
        //    vertex4[2].X = x1;
        //    vertex4[2].Y = ay;
        //    vertex4[3].X = ax1;
        //    vertex4[3].Y = ay;

        //    vertex5 = new Vector2[4];//right bottom
        //    vertex5[0].X = ax1;
        //    vertex5[0].Y = ay1;
        //    vertex5[1].X = x1;
        //    vertex5[1].Y = ay1;
        //    vertex5[2].X = x1;
        //    vertex5[2].Y = y1;
        //    vertex5[3].X = ax1;
        //    vertex5[3].Y = y1;

        //    vertex6 = new Vector2[4];//left bottom
        //    vertex6[0].X = x;
        //    vertex6[0].Y = ay1;
        //    vertex6[1].X = ax;
        //    vertex6[1].Y = ay1;
        //    vertex6[2].X = ax;
        //    vertex6[2].Y = y1;
        //    vertex6[3].X = x;
        //    vertex6[3].Y = y1;
        //}
    }
    public sealed class CloneImage:UIElement
    {
        internal UIImage img;
        public float opacity;
        public CloneImage(UIImage i)
        {
            Tag = UITag.CloneImage;
            img = i;
            UpdateUI = DrawCloneImage;
        }
        ~CloneImage()
        {
            base.Dispose();
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }

}
