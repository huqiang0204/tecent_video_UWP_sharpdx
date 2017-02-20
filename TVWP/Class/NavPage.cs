using System;
using System.Collections.Generic;
using DX;
using SharpDX.Mathematics.Interop;
using System.Diagnostics;
using SharpDX;

namespace TVWP.Class
{
    class NavPage
    {
        struct Nav_DataA
        {
            public string[] title;
            public string href;
            public Nav_DataA(string[] t, string h) { title = t; href = h; }
        }
        #region sort filter
        static string[] order_4 = new string[] { "热播", "Hot" };
        static string[] order_5 = new string[] { "最新", "new" };
        static string[] order_6 = new string[] { "评分", "Score" };
        static string[] order_value = new string[] { "?sort=4", "?sort=5", "?sort=6" };
        #endregion

        #region class rs
        static string[] movie = new string[] { "电影", "Movie" };
        static string[] tv = new string[] { "电视剧", "TV" };
        static string[] variety = new string[] { "综艺", "Variety" };
        static string[] animation = new string[] { "动漫", "Animation" };
        static string[] children = new string[] { "少儿", "Children" };
        static string[] mv = new string[] { "MV", "MV" };
        static string[] docoment = new string[] { "纪录片", "Docoment" };
        static string[] news = new string[] { "新闻", "News" };
        static string[] entertainment = new string[] { "娱乐", "Entertainment" };
        static string[] sports = new string[] { "体育", "Sports" };
        static string[] games = new string[] { "游戏", "Games" };
        static string[] fun = new string[] { "搞笑", "Fun" };

        public static string[] type = new string[] { "分类", "Type" };
        public static string[] sort = new string[] { "排序", "Sort" };
        #endregion

        #region href rs
        static string movie_href = "http://v.qq.com/x/list/movie";// /?sort=4 &offset=20|40.....
        static string tv_href = "http://v.qq.com/x/list/tv";
        static string variety_href = "http://v.qq.com/x/list/variety";
        static string animation_href = "http://v.qq.com/x/list/cartoon";
        static string children_href = "http://v.qq.com/x/list/children";
        static string mv_href = "http://v.qq.com/x/list/music";
        static string docoment_href = "http://v.qq.com/x/list/doco";
        static string news_href = "http://v.qq.com/x/list/news";
        static string entertainment_href = "http://v.qq.com/x/list/ent";
        static string sports_href = "http://v.qq.com/x/list/sports";
        static string games_href = "http://v.qq.com/x/list/games";
        static string fun_href = "http://v.qq.com/x/list/fun";


        static Nav_DataA[] nav_data = new Nav_DataA[] {new Nav_DataA(movie,movie_href),new Nav_DataA(tv,tv_href),
        new Nav_DataA(variety,variety_href),new Nav_DataA(animation,animation_href),new Nav_DataA(children,children_href),
        new Nav_DataA(docoment,docoment_href),new Nav_DataA(mv,mv_href),new Nav_DataA(news,news_href),
        new Nav_DataA(entertainment,entertainment_href),new Nav_DataA(sports,sports_href),new Nav_DataA(games,games_href),
        new Nav_DataA(fun,fun_href)};
        #endregion

        #region main
        const int itemcount = 20;
        
        static int nav_index;
        static string filter = "";
        static char[] c_buff;
        static bool create, load;
        static RawRectangleF margin;
        static GridPanel sp;
        static ListBox nav;
        static SwapChain Parent;
        public static void Create(SwapChain p, RawRectangleF m)//parent,margin
        {
            if (sp != null)
            {
                ReSize(m);
                return;
            }
            SetNav(0);
            Parent = p;

            nav = new ListBox();
            nav.Hrizon = true;
            int c = nav_data.Length;
            for (int i = 0; i < c; i++)
            {
                nav.Data.Add(nav_data[i].title[0]);
            }
            nav.SelectedIndex = 0;
            nav.ItemWidth = 60;
            nav.Forground = new RawColor4(0.8f, 0.8f, 0.8f, 1);
            nav.BorderColor = new RawColor4(0, 0, 0, 1);
            nav.Background = new RawColor4(0.6f, 0.4725f, 0.4725f, 0.8f);

            RoundBorder rb = new RoundBorder();
            rb.RoundRect.RadiusX = 8;
            rb.RoundRect.RadiusY = 8;
            rb.Size = new Size2F(60, 20);

            rb.FillBrush = BrushManage.GetLinearA();
            nav.FillTemplate = rb;
            nav.SelectChanged = (o) => {
                var lb = o as ListBox;
                if (lb.SelectedIndex < 0)
                    return;
                if (lb.SelectedIndex >= 12)
                    return;
                FilterPanel.Visble = false;
                for (int ss = 0; ss < sp.Data.Count; ss++)
                    (sp.Data[ss] as IDisposable).Dispose();
                sp.Data.Clear();
                part = 0;
                TextBar[1].Text = "1";
                SetNav(lb.SelectedIndex);
            };
            nav.ItemClick = (o) => {
                ShowOrHideFilter();
            };

            FilterPanel = new UIViewPort();
            FilterPanel.Visble = false;
            FilterPanel.GaussianBack = true;
            FilterPanel.Background = new RawColor4(0, 0.1f, 0.2f, 0.4f);

            sp = new GridPanel();
            sp.ItemTemplate = DataMod.GetTemplate();
            sp.BorderColor = new RawColor4(0, 0, 0, 1);
            create = true;
            sp.ItemClick = (o,e) => {
                if (sp.ClickIndex < 0)
                    return;
                string href = (sp.Data[sp.ClickIndex] as DataMod).href;
                if(href!=null)
                VideoPage.SetAddress(href);
                PageManageEx.CreateNewPage(PageTag.videopage);
            };
            lock (p.DX_Child)
            {
                p.DX_Child.Add(sp);
                p.DX_Child.Add(nav);
                p.DX_Child.Add(FilterPanel);
            }
            CreateBar(p);
            ReSize(m);
            if (load)
                Analyze();
        }
        static List<FilterOption>[] filter_buff;
        static string[] OptionTitle;
        static ListBox[] FilterList;
        static TextBlock[] FilterTitle;
        static UIViewPort FilterPanel;
        static void Analyze()
        {
            if (filter_buff == null)
            {
                filter_buff = new List<FilterOption>[8];
                OptionTitle = new string[8];
                FilterList = new ListBox[8];
                FilterTitle = new TextBlock[8];
            }
            int s = ParseData.GetFilterOption(ref c_buff,filter_buff,ref OptionTitle);
            ParseData.Analyze_Nav(ref c_buff, sp.Data , itemcount, s);
            
            sp.Update = true;
            sp.Visble = true;
            UpdateFilter();
            int c = sp.Data.Count;
            for (int i = 0; i < c; i++)
                (sp.Data[i] as DataMod).UpdateImage();
        }

        static void SetNav(int index)
        {
            if (loading)
                return;
            loading = true;
            if (sp!=null)
               if(sp.Data!=null)
                {
                    if (index < 6)
                        ReSizeItemMod(0);
                    else ReSizeItemMod(1);
                }
            nav_index = index;
            if (index >= 0)
                WebClass.TaskGet(nav_data[nav_index].href + GetFilter(), AnalyzeEx);
        }
        static void AnalyzeEx(string data)
        {
            char[] buff = data.ToCharArray();
            c_buff = CharOperation.DeleteChar(ref buff, '\\');
            //Debug.WriteLine(new string(c_buff));
            if (create)
                Analyze();
            else load = true;
            loading = false;
        }

        static bool FilterShow = false;
        static void ShowOrHideFilter()
        {
            if (FilterShow)
            {
                FilterShow = false;
                FilterPanel.Visble = false;
            } else
            {
                FilterShow = true;
                FilterPanel.Visble = true;
            }
        }
        static void UpdateFilter()
        {
            Vector2 location= new Vector2(4,4);
            Vector2 location2 = new Vector2(60,0);
            for(int i=0;i<8;i++)
            {
                if(OptionTitle[i]!=null)
                {
                    if (FilterTitle[i] == null)
                    {
                        var tb = new TextBlock();
                        tb.Forground = new DX.Color(255, 128, 0, 255);
                        tb.Size = new Size2F(80,20);
                        tb.Location = location;
                        FilterTitle[i] = tb;
                        FilterPanel.DX_Child.Add(tb);
                    }else FilterTitle[i].Visble = true;
                    FilterTitle[i].Text = OptionTitle[i];
                    if (FilterList[i] == null)
                    {
                        var lb = new ListBox();
                        lb.Size = new Size2F(margin.Right-margin.Left-70, 30);
                        lb.Hrizon = true;
                        lb.Location = location2;
                        lb.SelectedIndex = 0;
                        lb.ItemWidth = 70;
                        lb.Forground = new DX.Color(255,255,193,255);
                        lb.Context = i;
                        RoundBorder rb = new RoundBorder();
                        rb.RoundRect.RadiusX = 8;
                        rb.RoundRect.RadiusY = 8;
                        rb.Size = new Size2F(70, 20);
                        rb.FillBrush = BrushManage.GetLinearA();
                        lb.FillTemplate = rb;
                        FilterList[i] = lb;
                        lb.SelectedIndex = 0;
                        lb.SelectChanged = SelectChanged;
                        FilterPanel.DX_Child.Add(lb);
                    }else FilterList[i].Visble = true;
                    FilterList[i].Data.Clear();
                    int c = filter_buff[i].Count;
                    for(int t=0;t<c;t++)
                    {
                        FilterList[i].Data.Add(filter_buff[i][t].Content);
                    }
                }
                else
                {
                    if (FilterTitle[i] != null)
                        FilterTitle[i].Visble = false;
                    if (FilterList[i] != null)
                        FilterList[i].Visble = false;
                }
                location.Y += 24;
                location2.Y += 24;
            }
            ThreadManage.UpdateUI = true;
        }
        static string GetFilter()
        {
            if (OptionTitle == null)
                return "";
            filter = "?offset="+(part*20).ToString();
            for (int i = 0; i < 8; i++)
            {
               if(OptionTitle[i]!=null)
                {
                    int c = FilterList[i].SelectedIndex;
                    if (c>0)
                    {
                        filter +="&"+ filter_buff[i][c].Code;
                    }
                }
            }
            return filter;
        }
        static float ResizeFilter(float w)
        {
            int i;
            for(i=0;i<8;i++)
            {
                if(OptionTitle[i]==null)
                    break;
                if (FilterList[i] != null)
                    FilterList[i].Size = new Size2F(w - 60,24);
            }
            return (float) i*24;
        }
        public static void ReSize(RawRectangleF m)
        {
            if (margin.Left == m.Left)
                if (margin.Right == m.Right)
                    if (margin.Top == m.Top)
                        if (margin.Bottom == m.Bottom)
                            return;
            margin = m;
            float w = m.Right - m.Left;
            float h = m.Bottom - m.Top;
            if (w != nav.Size.Width)
                nav.Size = new Size2F(w, 28);
            nav.Location = new Vector2(m.Left, m.Top);
            m.Top += 24;
            m.Top += 4;
            h = m.Bottom - m.Top - 36;
            sp.Size = new Size2F(w, h);
            sp.Location = new Vector2(m.Left, m.Top);
            FilterPanel.Size = new Size2F(w, 200);//8*24
            FilterPanel.Location = new Vector2(m.Left, m.Top);
            if (nav_index < 6)
                ReSizeItemMod(0);
            else ReSizeItemMod(1);
            ReDockBar(m);
            if (FilterList != null)
                ResizeFilter(w);
            if(about!=null)
            if (about.Visble)
                ReDockAbout(margin);
        }
        static void ReSizeItemMod(int type)
        {
            float w = margin.Right - margin.Left;
            float iw = (w - 4) / 140;
            int t = (int)iw;
            iw = (w - 4) / t;
            sp.ItemWidth = (int)iw;
            iw *= 1.5f;
            sp.ItemHeight = (int)iw;
            if(type==0)
            DataMod.ReSize(sp.ItemTemplate, new Size2F(sp.ItemWidth, sp.ItemHeight));
            else DataMod.ReSize_S(sp.ItemTemplate, new Size2F(sp.ItemWidth, sp.ItemHeight));
        }
        static void SelectChanged(DX.UIElement u)
        {
            ListBox lb = u as ListBox;
            int index = lb.SelectedIndex;
            if (index <1)
                return;
            for (int ss = 0; ss < sp.Data.Count; ss++)
                (sp.Data[ss] as IDisposable).Dispose();
            sp.Data.Clear();
            SetNav(nav_index);
        }
        #endregion

        #region search
        static string address = "http://m.v.qq.com/search.html?act=0&keyWord=";// + content;
        static int index;
        static int part;
        static bool loading, over;
        static string content;
        public static void Find(string c)
        {
            if (loading)
                return;
            loading = true;
            nav_index = 13;
            nav.SelectedIndex = -1;
            nav.Update = true;
            FilterPanel.Visble = false;
            for (int ss = 0; ss < sp.Data.Count; ss++)
                (sp.Data[ss] as IDisposable).Dispose();
            sp.Data.Clear();
            ReSizeItemMod(2);
            content = c;
            over = false;
            string url = "http://node.video.qq.com/x/cgi/msearch?contextValue=last_end=%3D" +
                index.ToString() + "%26areaId%3D101&keyWord=";
            url += Uri.EscapeUriString(c);
            url += "&contextType=2&callback=jsonp" + part.ToString();
            string str = address + c;
            str = Uri.EscapeUriString(str);
            NetClass.TaskGet(url, Analyze, str);
        }
        static void Analyze(string data)
        {
            int c = ParseData.Search_ex(data.ToCharArray(), sp.Data);
            if (c < 14)
                over = true;
            loading = false;
            sp.Update = true;
            sp.Visble = true;
            UpdateFilter();
            for (int i = 0; i < c; i++)
                (sp.Data[i] as DataMod).UpdateImage();
        }
        static void FindMore()
        {
            if (over)
                return;
            loading = true;
            index += 15;
            part++;
            for (int ss = 0; ss < sp.Data.Count; ss++)
                (sp.Data[ss] as IDisposable).Dispose();
            sp.Data.Clear();
            ReSizeItemMod(2);
            over = false;
            string url = "http://node.video.qq.com/x/cgi/msearch?contextValue=last_end=%3D" +
                index.ToString() + "%26areaId%3D101&keyWord=";
            url += Uri.EscapeUriString(content);
            url += "&contextType=2&callback=jsonp" + part.ToString();
            string str = address + content;
            str = Uri.EscapeUriString(str);
            NetClass.TaskGet(url, Analyze, str);
        }
        #endregion

        #region bar
        static TextBlock[] TextBar = new TextBlock[4];
        static void CreateBar(SwapChain parent)
        {
            TextBlock tb = new TextBlock();
            tb.FontStyle = TextBlock.FontName.SegoeUISymbol;
            tb.Text =((char)0xe0d5).ToString();
            tb.SurfaceBrush = BrushManage.GetRadia_A();
            tb.Size = new Size2F(32,32);
            tb.FontSize = 24;
            tb.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            tb.PointerEntry = PointerEntry;
            tb.PointerLeave = PointerLeave;
            UIElement.SetClick(tb,Click);
            TextBar[0] = tb;

            tb = new TextBlock();
            tb.Text = "1";
            tb.Forground = new RawColor4(1,1,1,1);
            tb.Size = new Size2F(48,32);
            tb.FontSize = 24;
            tb.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            TextBar[1] = tb;

            tb = new TextBlock();
            tb.FontStyle = TextBlock.FontName.SegoeUISymbol;
            tb.Text = ((char)0xe0ae).ToString();
            tb.SurfaceBrush = BrushManage.GetRadia_A();
            tb.Size = new Size2F(32, 32);
            tb.FontSize = 24;
            tb.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            tb.PointerEntry = PointerEntry;
            tb.PointerLeave = PointerLeave;
            UIElement.SetClick(tb,Click);
            TextBar[2] = tb;

            tb = new TextBlock();
            tb.FontStyle = TextBlock.FontName.SegoeUISymbol;
            tb.Text = ((char)0xe181).ToString();
            tb.SurfaceBrush = BrushManage.GetRadia_A();
            tb.Size = new Size2F(32, 32);
            tb.FontSize = 24;
            tb.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            tb.PointerEntry = PointerEntry;
            tb.PointerLeave = PointerLeave;
            UIElement.SetClick(tb,ShowAbout);
            TextBar[3] = tb;
            lock (parent.DX_Child)
            {
                parent.DX_Child.Add(TextBar[0]);
                parent.DX_Child.Add(TextBar[1]);
                parent.DX_Child.Add(TextBar[2]);
                parent.DX_Child.Add(TextBar[3]);
            }
        }
        static void ReDockBar(RawRectangleF m)
        {
            float top = m.Bottom - 36;
            float dx = (m.Right - m.Left)*0.5f-56;
            TextBar[0].Location = new Vector2(dx,top);
            dx += 32;
            TextBar[1].Location = new Vector2(dx,top);
            dx += 48;
            TextBar[2].Location = new Vector2(dx,top);
            TextBar[3].Location = new Vector2(m.Left,top);
        }
        static void Click(DX.UIElement u)
        {
            if (u == TextBar[0])
            {
                if (part < 1)
                    return;
                else part--;
            }
            else
            {
                part++;
            }
            TextBar[1].Text = (part + 1).ToString();
            if (nav_index == 13)
            {
                if (over)
                    return;
                for (int ss = 0; ss < sp.Data.Count; ss++)
                    (sp.Data[ss] as IDisposable).Dispose();
                sp.Data.Clear();
                FindMore();
            }
            else
            {
                for (int ss = 0; ss < sp.Data.Count; ss++)
                    (sp.Data[ss] as IDisposable).Dispose();
                sp.Data.Clear();
                SetNav(nav_index);
            }
        }
        static void PointerEntry(DX.UIElement u, Vector2 dot)
        {
            (u as TextBlock).SurfaceBrush = BrushManage.GetRadia_B();
            ThreadManage.UpdateUI = true;
        }
        static void PointerLeave(DX.UIElement u, Vector2 dot)
        {
            (u as TextBlock).SurfaceBrush = BrushManage.GetRadia_A();
            ThreadManage.UpdateUI = true;
        }
        static void ShowAbout(DX.UIElement u)
        {
            if(about==null)
            {
                CreateAbout(Parent);
                ReDockAbout(margin);
                ThreadManage.UpdateUI = true;
            }else if(about.Visble)
            {
                about.Visble = false;
                declare.Visble = false;
            }else
            {
                ReDockAbout(margin);
                about.Visble = true;
                declare.Visble = true;
                ThreadManage.UpdateUI = true;
            }
        }
        #endregion

        public static void Hide()
        {
            sp.Visble = false;
            nav.Visble = false;
            FilterPanel.Visble = false;
            TextBar[0].Visble = false;
            TextBar[1].Visble = false;
            TextBar[2].Visble = false;
            TextBar[3].Visble = false;
            if (about != null)
            {
                about.Visble = false;
                declare.Visble = false;
            }
        }
        public static void Show()
        {
            sp.Visble = true;
            nav.Visble = true;
            TextBar[0].Visble = true;
            TextBar[1].Visble = true;
            TextBar[2].Visble = true;
            TextBar[3].Visble = true;
        }

        #region about
        static string aouth = "想找份工作\r\n 1.会unity monogame c# sharpdx \r\n2.熟悉2d图形碰撞算法,unity着色器"+
            "\r\n3.习惯多线程交互委托\r\n4.易语言及内联汇编\r\nhuqiang1990@outlook.com\r\nhttps://github.com/huqiang0204"
            +"\r\n只有初中学历（全部自学包括英语）\r\n招聘网站有点难（需要学历）\r\n在此自荐"
            + "\r\n本次更新采用sharpdx绘制\r\n实现毛玻璃效果\r\nUI允许多线程绘制和访问\r\n鼠标手势";
        static UIViewPort about;
        static TextBlock declare;
        static TextBlock close;
        static void CreateAbout(SwapChain p)
        {
            if(about!=null)
            {
                about.Visble = true;
                return;
            }
            about = new UIViewPort();
            about.Size = new Size2F(240,240);
            about.GaussianBack = true;
            about.Background = new RawColor4(0f,0.1f,0.2f,0.4f);
            declare = new TextBlock();
            declare.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            declare.Size = new Size2F(232,232);
            declare.Location = new Vector2(3,3);
            declare.Text = aouth;
            declare.Forground = new RawColor4(1,1,1,1);
            declare.EventCross = true;
            //about.DX_Child.Add(declare);
            close = new TextBlock();
            close.FontStyle = TextBlock.FontName.SegoeUISymbol;
            close.Text = ((char)0xe0a4).ToString();
            close.SurfaceBrush = BrushManage.GetRadia_A();
            close.Size = new Size2F(20, 20);
            close.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            close.PointerEntry = (o,e)=> { close.SurfaceBrush = BrushManage.GetRadia_R();ThreadManage.UpdateUI = true; };
            close.PointerLeave = PointerLeave;
            close.Location = new Vector2(217,3);
            about.DX_Child.Add(close);
            UIElement.SetClick(close, (o)=> { about.Visble = false;
                declare.Visble = false;
                ThreadManage.UpdateUI = true; });
            lock (p.DX_Child)
            {
                p.DX_Child.Add(about);
                p.DX_Child.Add(declare);
            }
        }
        static void ReDockAbout(RawRectangleF m)
        {
            float w = m.Right - m.Left;
            w -= 240;
            w *= 0.5f;
            float h = m.Bottom - m.Top;
            h -= 240;
            h *= 0.5f;
            about.Location = new Vector2(w,h);
            w += 3;
            h += 3;
            declare.Location = new Vector2(w,h);
        }
        #endregion
    }
}
