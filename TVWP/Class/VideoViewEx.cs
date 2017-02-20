using DX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using SharpDX;
namespace TVWP.Class
{
    class VideoPage : CharOperation,Navigation
    {
        #region
        struct Cover_info
        {
            public string title;
            public string s_title;
            public string cid;
            public string pic;
        }

        static char[] Key_m = "m.".ToCharArray();
        static string type;
        static string site;
        static Cover_info ci;
        static string cur_vid;
        #endregion

        static SwapChain parent;
        static UIViewPort panel;
        static StackPanel videolist;
        static UIImage pic;
        static RoundBorder titleback;
        static TextBlock title;
        static TextBlock detail;
        static ImageContext ic;
        static RawRectangleF margin;
        //static GridBox gb;
        static List<FilterOption> lep;
        static void CreateInfoPanel()
        {
            pic = new UIImage();
            titleback = new RoundBorder();
            titleback.FillBrush = BrushManage.GetLinearA();
          
            titleback.RoundRect.RadiusX = 8;
            titleback.RoundRect.RadiusY = 8;
            title = new TextBlock();
            title.Forground = new RawColor4(0.6f,0.9f,0.8f,1);
            title.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            title.PointerEntry = (o, e) =>
            {
                titleback.FillBrush = BrushManage.GetLinearB();
                titleback.Forground = new RawColor4(0.96f, 0.98f, 0.48f, 1);
                title.Forground = new RawColor4(1, 1, 1, 1);
                ThreadManage.UpdateUI = true;
            };
            title.PointerLeave = (o, e) =>
            {
                titleback.FillBrush = BrushManage.GetLinearA();
                titleback.Forground.A = 0;
                title.Forground = new RawColor4(0.6f, 0.9f, 0.8f, 1);
                ThreadManage.UpdateUI = true;
            };
            UIElement.SetClick(title,TitleClick);

            detail = new TextBlock();
            detail.Forground = new RawColor4(1,1,1,1);
            //gb = new GridBox();
            //gb.Forground = new RawColor4(0.8f, 0.8f, 0.8f, 1);
            //gb.BorderColor = new RawColor4(0, 0, 0, 1);
            //gb.ItemWidth = 100;
            //gb.ItemHeight = 18;
            //RoundBorder rb = new RoundBorder();
            //rb.RoundRect.RadiusX = 8;
            //rb.RoundRect.RadiusY = 8;
            //rb.Size = new Size2F(100, 20);
            //rb.FillBrush = BrushManage.GetLinearA();
            //gb.FillTemplate = rb;

            lock (parent.DX_Child)
            {
                parent.DX_Child.Add(pic);
                parent.DX_Child.Add(titleback);
                parent.DX_Child.Add(title);
                parent.DX_Child.Add(detail);
                //parent.DX_Child.Add(gb);
            }
        }
        static void TitleClick(UIElement u)
        {
            if (ic.vid == null)
                return;
            if (ic.vid == "")
                return;
            Player.Setvid(ic.vid);
            PageManageEx.CreateNewPage(PageTag.player);
        }
        static void ReSizeInfoPanel(RawRectangleF m)
        {  
            float w = m.Right - m.Left;
            float h = m.Bottom - m.Top;
            float dx;
            float ph;
            if (w < 280)
            {
                ph = w * 0.714f;
                pic.Size = new Size2F(w, ph);
                dx = 0;
            }
            else
            {
                pic.Size = new Size2F(280,200);
                dx = w - 280;
                dx *= 0.5f;
                ph = 200;
            }
            pic.Location = new Vector2(dx+m.Left, m.Top);
            m.Top += ph;
            title.Location = new Vector2(m.Left,m.Top);
            title.Size = new Size2F(140, 24);
            titleback.Size = new Size2F(140,24);
            titleback.Location = new Vector2(m.Left,m.Top);
            m.Top += 24;
            //float dh = 80000 / w;
            detail.Size = new Size2F(w, m.Bottom-m.Top);
            detail.Location = new Vector2(m.Left,m.Top);
            //m.Top += dh;
            //float gh = m.Bottom - m.Top;
            //gb.Size = new Size2F(w,gh-3);
            //gb.Location = new Vector2(m.Left,m.Top);
        }
        static async void LoadPic(string src)
        {
            UIImage.LoadSource(await WebClass.Get(src),pic);
        }
        static void CreatePage(SwapChain p,RawRectangleF m)
        {
            if (panel == null)
                panel = new UIViewPort();
            parent = p;
            lep = new List<FilterOption>();
            videolist = new StackPanel();
            videolist.ItemTemplate = DataModA.CreateDataMod();
            videolist.BorderColor = new RawColor4(0f,0f,0f,1);
            videolist.ItemClick= videolist.SelectChanged = (v) => {
                var sp= v as StackPanel;
                int c = sp.SelectedIndex;
                PlayControl((sp.Data[c] as DataModA).vid);
            };
            CreateInfoPanel();
            Resize(m);
            lock (p.DX_Child)
                p.DX_Child.Add(videolist);
        }
        static void Resize(RawRectangleF m)
        {
            if (margin.Left == m.Left)
                if (margin.Right == m.Right)
                    if (margin.Top == m.Top)
                        if (margin.Bottom == m.Bottom)
                            return;
            margin = m;
            float w = m.Right - m.Left;
            float h = m.Bottom - m.Top;
            videolist.Size = new Size2F(140,h);
            videolist.ItemWidth = 136;
            videolist.ItemHeight = 100;
            DataModA.ReSize(videolist.ItemTemplate,new Size2F(136,100));

            videolist.Location = new Vector2(m.Right-140,m.Top);
            m.Right -= 140;
            ReSizeInfoPanel(m);
        }
        public static void SetAddress(string url)
        {
            if(videolist!=null)
            {
                int c = videolist.Data.Count;
                for (int i = 0; i < c; i++)
                    (videolist.Data[i] as IDisposable).Dispose();
                videolist.Data.Clear();
            }
            char[] c_buff = url.ToCharArray();
            int t = FindCharArray(ref c_buff, ref Key_m, 0);
            if(t>-1)//mobile
            {
                type = "m";//mobile
                site = url;
                NetClass.TaskGet(site,AnalyzeM,site);
                return;
            }
            else//desktop
            {
                t = FindCharArray(ref c_buff, ref Key_x, 0);
                if (t < 0)
                {
                    int c = FindCharArray(ref c_buff, ref Key_qq, 0);
                    c--;
                    type = new string(FindCharArrayA(ref c_buff, '/', '/', ref c));
                    if (type == "live")
                    { site = url; goto label0; }
                    int l = c_buff.Length - 1;
                    int i;
                    for (i = l; i > -1; i--)
                    {
                        if (c_buff[i] == '/')
                            break;
                    }
                    string tag = new string(FindCharArrayA(ref c_buff, '/', '.', ref i));
                    site = "http://v.qq.com/x/" + type + "/" + tag + ".html";
                }
                else
                {
                    site = url;
                    t -= 2;
                    type = new string(FindCharArrayA(ref c_buff, '/', '/', ref t));
                }
            }
            label0:;
            NetClass.TaskGet(site,Analyze,site);
        }
        static void Analyze(string data)
        {
            if (data.Length < 4096)
            {
                char[] c_buff = data.ToCharArray();
                c_buff = DeleteChar(ref c_buff, '\\');
                int o = FindCharArray(ref c_buff, ref Key_refresh, 0);
                if (o < 0)
                {
                    o = FindCharArray(ref c_buff, ref Key_split, 0);
                    if (o < 0)
                        return;
                    o += 6;
                    site += new string(FindCharArrayA(ref c_buff, '\'', '\'', ref o));
                    NetClass.TaskGet(site, Analyze, site);
                    return;
                }
                o = FindCharArray(ref c_buff, ref Key_url, o);
                char[] tt;
                if (o < 0)
                {
                    o = FindCharArray(ref c_buff, ref Key_href, 0);
                    tt = FindCharArrayA(ref c_buff, '\"', '\"', ref o);
                }
                else
                {
                    o = FindCharArray(ref c_buff, 'h', o);
                    int e = FindCharArray(ref c_buff, '\'', '\"', o);
                    e -= o;
                    tt = CopyCharArry(ref c_buff, o, e);
                }
                if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                    SetAddress(new string(tt));
                else SetAddress("http:" + new string(tt));
                return;
            }

            char[] c = data.ToCharArray();
            c = DeleteChar(ref c,'\\');
            //if (type == "cover")
            //{
            //    lep.Clear();
            //    ParseData.GetEp_infoA(ref c, lep);
            //    gb.Data.Clear();
            //    for(int i=0;i<lep.Count;i++)
            //    {
            //        gb.Data.Add(lep[i].Content);
            //    }
            //    gb.Update = true;
            //}
            int s = GetCoverInfo(ref c);
            if (s < 0)
                s = 0;
            ic = ParseData.GetVideoInfo(ref c, s);
            title.Text = ic.title;
            detail.Text = ic.detail;
            LoadPic(ic.src);
            ParseData.GetListInfo(ref c, videolist.Data, s);
            if (type=="cover")
            {
                ParseData.GetCoverList(ref c,videolist.Data,s,50);
            }
            else
            {
                ParseData.GetPageList(ref c,videolist.Data);
            }
            s = FindCharArray(ref c, ref Key_player_figure, 0);
            if (s > 0)
            {
                s = FindCharArray(ref c, ref Key_src, s);
                char[] tt = FindCharArrayA(ref c, '\"', '\"', ref s);
                var src = "http:" + new string(tt);
            }
            videolist.Update = true;
            ThreadManage.UpdateUI = true;
        }
        static void AnalyzeM(string data)
        {
            char[] c = data.ToCharArray();
            c = DeleteChar(ref c, '\\');
            lep.Clear();
            ic = ParseData.Des_PlayPage(c,lep, videolist.Data);
            cur_vid = ic.vid;
            //gb.Data.Clear();
            //for (int i = 0; i < lep.Count; i++)
            //{
            //    gb.Data.Add(lep[i].Content);
            //}
            //gb.Update = true;
            videolist.Update = true;
            ThreadManage.UpdateUI = true;
        }
        static void UpdatePage()
        {

        }
     
        static int GetCoverInfo(ref char[] c_buff)
        {
            int s = FindCharArray(ref c_buff, ref Key_coverinfo, 0);
            if (s < 0)
            {
                ci.cid = null;
                return -1;
            }
            int t = FindCharArray(ref c_buff, ref Key_titleA, s);
            ci.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
            t++;
            ci.s_title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
            t = FindCharArray(ref c_buff, ref Key_id, t);
            ci.cid = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
            t = FindCharArray(ref c_buff, ref Key_pic, t);
            char[] c = FindCharArrayA(ref c_buff, '\"', '\"', ref t);
            if (c[0] != 'h')
                ci.pic = "http:" + new string(c);
            else ci.pic = new string(c);
            return t;
        }
        //static VideoAddress va;
        static void PlayControl(string vid)
        {
            Player.Setvid(vid);
            PageManageEx.CreateNewPage(PageTag.player);
        }
        static void Complete(string v)
        {

        }
        public bool Back()
        {
            Hide();
            return false;
        }
        public void Create(SwapChain p, RawRectangleF m)
        {
            CreatePage(p,m);
        }
        public void Show()
        {
            panel.Visble = true;
            videolist.Visble = true;
            pic.Visble = true;
            titleback.Visble = true;
            title.Visble = true;
            detail.Visble = true;
        }
        public void Hide()
        {
            panel.Visble = false;
            videolist.Visble = false;
            pic.Visble = false;
            titleback.Visble = false;
            title.Visble = false;
            detail.Visble = false;
        }
        public void ReSize(RawRectangleF m)
        {
            Resize(m);
        }
    }
}
