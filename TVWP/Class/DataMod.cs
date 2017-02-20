using System;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using SharpDX;
using DX;
using System.IO;
using System.Diagnostics;

namespace TVWP.Class
{
    class DataMod:BindingMod,IDisposable
    {
        static string[] Mark = new string[] { "","独播","预告","专题","自制","VIP","用卷","付费","预约","直播中","直播结束"};
        public static void ReSize(object o, Size2F size)
        {
            BindingElement[] item = o as BindingElement[];
            float w = size.Width-6;
            float h = size.Height-6;
            item[0].UI.Size = new Size2F(w,h);
            item[0].RawLocation.X = 3;
            item[0].RawLocation.Y = 3;
            w -= 2;
            h -= 2;
            item[1].UI.Size = new Size2F(w,h);
            item[1].RawLocation.X = 4;
            item[1].RawLocation.Y = 4;
            float ch = h * 0.7f;
            item[2].RawLocation.X = 4;
            item[2].RawLocation.Y = ch;
            item[2].UI.Size = new Size2F(w, h - ch+6);
            item[3].UI.Size = new Size2F(w,32);
            item[3].RawLocation.X = 4;
            item[3].RawLocation.Y = ch;
            ch += 20;
            item[4].UI.Size = new Size2F(w,h-ch+6);
            item[4].RawLocation.X = 4;
            item[4].RawLocation.Y = ch;
            item[5].RawLocation.X = 4;
            item[5].RawLocation.Y = 4;
        }
        public static void ReSize_S(object o, Size2F size)
        {
            BindingElement[] item = o as BindingElement[];
            float w = size.Width - 6;
            float h = size.Height - 6;
            item[0].UI.Size = new Size2F(w, h);
            item[0].RawLocation.X = 3;
            item[0].RawLocation.Y = 3;
            w -= 2;
            h -= 2;
            float ch = h * 0.7f;
            item[1].UI.Size = new Size2F(w, ch-4);
            item[1].RawLocation.X = 4;
            item[1].RawLocation.Y = 4;
            
            item[2].RawLocation.X = 4;
            item[2].RawLocation.Y = ch;
            item[2].UI.Size = new Size2F(w, h - ch + 6);
            item[3].UI.Size = new Size2F(w, 18);
            item[3].RawLocation.X = 4;
            item[3].RawLocation.Y = ch;
            ch += 20;
            item[4].UI.Size = new Size2F(w, h - ch+6);
            item[4].RawLocation.X = 4;
            item[4].RawLocation.Y = ch;
            item[5].RawLocation.X = 4;
            item[5].RawLocation.Y = 4;
        }
        public static BindingElement[] GetTemplate()
        {
            BindingElement[] template = new BindingElement[6];
            Border bor = new Border();
            bor.Forground = new DX.Color(0.8f,0.2f,0.1f,1);
            template[0].UI = bor;
            UIImage img = new UIImage();
            template[1].UI = img;
            template[1].SetData = (o , d) => {
                var i = (d as DataMod).Img;
                if (i == null)
                {
                    (o as UIImage).d2dmap = null;
                    return;
                }
                (o as UIImage).d2dmap =i;
            };
            bor = new Border();
            bor.Background = new RawColor4(0f, 0f, 0f, 0.4f);
            template[2].UI = bor;
            TextBlock tb = new TextBlock();
            tb.Forground = new DX.Color(255,128,0,255);
            template[3].UI = tb;
            template[3].SetData = (o, d) => { (o as TextBlock).Text = (d as DataMod).Title; };
            tb = new TextBlock();
            tb.Forground = new RawColor4(1f,1f,1f,1f);
            template[4].UI = tb;
            tb.Warpping = SharpDX.DirectWrite.WordWrapping.Wrap;
            template[4].SetData = (o, d) => { (o as TextBlock).Text = (d as DataMod).Detail; };
            tb = new TextBlock();
            tb.Forground = new RawColor4(1,1,1,1);
            tb.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            tb.Size = new Size2F(40,18);
            template[5].UI = tb;
            template[5].SetData = SetMark;
            return template;
        }
        static void SetMark(object u,object mod)
        {
            TextBlock tb = u as TextBlock;
            DataMod dm = mod as DataMod;
            int c = dm.marktag;
            if(c<1)
            {
                tb.Text = null;
                tb.Background.A = 0;
                return;
            }
            tb.Text = Mark[c];
            tb.Background = new DX.Color(163,95,68,200);
        }
        public string href;
        public string src;
        Stream imgsteam;
        DX.BitmapSource bs;
        public SharpDX.Direct2D1.Bitmap Img;
        public string Title;
        public string Detail;
        //Stream markstream;
        //FormatConverter markfc;
        public int marktag;
        bool updating;
        ~DataMod()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (Img != null)
            {
                Img.Dispose();
                Img = null;
                bs.Converter.Dispose();
                bs.Decoder.Dispose();
                bs.FrameDecoder.Dispose();
            }
            if (imgsteam != null)
            { imgsteam.Dispose(); imgsteam = null; }
            GC.SuppressFinalize(this);
        }
        public async void UpdateImage()
        {
            if (updating)
                return;
            updating = true;
            imgsteam= await WebClass.Get(src);
            if(imgsteam==null)
            {
                updating = false;
                return;
            }
            bs = UIImage.CreateWicBitmap(imgsteam);
            Img = UIImage.CreateD2DBitmap(bs.Converter);
            updating = false;
            if (Parent != null)
            {
                Parent.Update = true;
                ThreadManage.UpdateUI = true;
            }
        }
    }
    class DataModA : BindingMod, IDisposable
    {
        public static void ReSize(object o, Size2F size)
        {
            BindingElement[] item = o as BindingElement[];
            float w = size.Width - 6;
            float h = size.Height - 6;
            item[0].UI.Size = new Size2F(w, h);
            item[0].RawLocation.X = 3;
            item[0].RawLocation.Y = 3;
            w -= 2;
            h -= 2;
            item[1].UI.Size = new Size2F(w, h);
            item[1].RawLocation.X = 4;
            item[1].RawLocation.Y = 4;
            float ch = h * 0.7f;
            item[2].RawLocation.X = 4;
            item[2].RawLocation.Y = ch;
            item[2].UI.Size = new Size2F(w, h - ch + 6);
        }
        public static BindingElement[] CreateDataMod()
        {
            BindingElement[] template = new BindingElement[3];
            Border bor = new Border();
            bor.Forground = new DX.Color(0.8f, 0.2f, 0.1f, 1);
            template[0].UI = bor;
            UIImage img = new UIImage();
            template[1].UI = img;
            template[1].SetData = (o, d) => {
                var dm = d as DataModA;
                var i = dm.Img;
                if (i == null)
                {
                    (o as UIImage).d2dmap = null;
                    dm.UpdateImag();
                    return;
                }
                (o as UIImage).d2dmap = i;
            };
            TextBlock tb = new TextBlock();
            tb.Forground = new DX.Color(255, 128, 0, 255);
            template[2].UI = tb;
            tb.EventCross = true;
            template[2].SetData = (o, d) => { (o as TextBlock).Text = (d as DataModA).Title; };
            tb.Background = new RawColor4(0.2f,0.2f,0.2f,0.6f);
            return template;
        }
        Stream imgsteam;
        DX.BitmapSource bs;
        public SharpDX.Direct2D1.Bitmap Img;
        public string Title;
        public string src;
        public string vid;
        ~DataModA()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (Img != null)
            {
                Img.Dispose();
                Img = null;
                bs.Converter.Dispose();
                bs.Decoder.Dispose();
                bs.FrameDecoder.Dispose();
            }
            if (imgsteam != null)
            { imgsteam.Dispose(); imgsteam = null; }
            GC.SuppressFinalize(this);
        }
        bool updating;
        public async void UpdateImag()
        {
            if (updating)
                return;
            updating = true;
            imgsteam = await WebClass.Get(src);
            if (imgsteam == null)
            {
                updating = false;
                return;
            }
            bs = UIImage.CreateWicBitmap(imgsteam);
            Img = UIImage.CreateD2DBitmap(bs.Converter);
            updating = false;
            if (Parent != null)
            {
                Parent.Update = true;
                ThreadManage.UpdateUI = true;
            }
        }
    }
}
