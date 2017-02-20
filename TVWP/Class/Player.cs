using DX;
using SharpDX.Mathematics.Interop;
using System;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using SharpDX;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;

namespace TVWP.Class
{
    class Player:Component,Navigation
    {
        struct PlayStateBar
        {
            public DX.Border border;
            public DX.ComboBox Sharp;
            public DX.ComboBox Site;
            public Ellipse ellipse;
            public DX.TextBlock play;
            public RoundBorder progressA;
            public RoundBorder progressB;
            public RoundBorder progressC;
            public RoundBorder sharpback;
            public RoundBorder siteback;
            public DX.Border Back;
        }
        #region variable
        struct StateInfo
        {
            public Canvas parent;
            public int play_part;
            public string vid;
        }
        static VideoInfo vic;
        static StateInfo current;
        static MediaElement me;
        static PlayStateBar PSB = new PlayStateBar();
        static DisplayRequest display;
        static double screenW, screenH;
        static VideoAddress va;
        static int Sharp = 720;
        static DispatcherTimer dt;
        #endregion

        #region UI
        static void CreateBar(SwapChain p)
        {
            if (PSB.border != null)
            {
                PSB.border.Visble = true;
                PSB.ellipse.Visble = true;
                PSB.play.Visble = true;
                PSB.Sharp.Visble = true;
                PSB.Site.Visble = true;
                PSB.progressA.Visble = true;
                PSB.progressB.Visble = true;
                PSB.progressC.Visble = true;
                PSB.sharpback.Visble = true;
                PSB.siteback.Visble = true;
                PSB.Back.Visble = true;
                return;
            }
            PSB.border = new DX.Border();
            PSB.border.Background = new RawColor4(0.2f, 0.2f, 0.2f, 0.4f);
            PSB.play = new DX.TextBlock();
            PSB.play.FontStyle = DX.TextBlock.FontName.SegoeUISymbol;
            PSB.play.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            PSB.play.Text = ((char)0xe103).ToString();//e103
            PSB.play.FontSize = 30;
            PSB.play.Size = new Size2F(40, 40);
            PSB.play.EventCross = true;
            PSB.play.SurfaceBrush = BrushManage.GetRadia_A();

            PSB.progressA = new RoundBorder();
            PSB.progressA.RoundRect.RadiusX = 2.5f;
            PSB.progressA.RoundRect.RadiusY = 2.5f;
            PSB.progressA.FillBrush = BrushManage.GetLinearA();
            PSB.progressB = new RoundBorder();
            PSB.progressB.RoundRect.RadiusX = 2.5f;
            PSB.progressB.RoundRect.RadiusY = 2.5f;
            PSB.progressB.FillBrush = BrushManage.GetLinearG();
            PSB.progressC = new RoundBorder();
            PSB.progressC.RoundRect.RadiusX = 2.5f;
            PSB.progressC.RoundRect.RadiusY = 2.5f;
            PSB.progressC.FillBrush = BrushManage.GetLinearR();
            DX.UIElement.SetClick(PSB.progressA,ProgressChange);
            DX.UIElement.SetClick(PSB.progressB, ProgressChange);
            DX.UIElement.SetClick(PSB.progressC, ProgressChange);

            PSB.ellipse = new Ellipse();
            PSB.ellipse.ellipse.RadiusX = 20;
            PSB.ellipse.ellipse.RadiusY = 20;
            PSB.ellipse.FillBrush = BrushManage.GetRadia_C();
            PSB.ellipse.PointerEntry = (o, e) => {
                PSB.play.SurfaceBrush = BrushManage.GetRadia_B();
                PSB.ellipse.FillBrush = BrushManage.GetRadia_D();
                ThreadManage.UpdateUI = true;
            };
            PSB.ellipse.PointerLeave = (o, e) => {
                PSB.play.SurfaceBrush = BrushManage.GetRadia_A();
                PSB.ellipse.FillBrush = BrushManage.GetRadia_C();
                ThreadManage.UpdateUI = true;
            };
            DX.UIElement.SetClick(PSB.ellipse, PlayOrPause);

            PSB.Sharp = new DX.ComboBox();
            PSB.Sharp.Size = new Size2F(100, 20);
            PSB.Sharp.List.Size = new Size2F(100,72);
            PSB.Sharp.Up = true;
            PSB.Sharp.Content.Forground = new RawColor4(0, 1, 0, 1);
            PSB.Sharp.List.Forground = new RawColor4(1,1,1,0.8f);
            PSB.Sharp.List.Background = new RawColor4(0.2f,0.2f,0.2f,0.5f);
            PSB.Sharp.Pop.Forground = new DX.Color(251,255,139,255);
            PSB.Sharp.SelectChanged = (o) => {
                int c = (o as DX.ComboBox).List.SelectedIndex;
                SharpChanged(c);
            };
            PSB.Site = new DX.ComboBox();
            PSB.Site.Size = new Size2F(100, 20);
            PSB.Site.List.Size = new Size2F(100,72);
            PSB.Site.List.Forground = new RawColor4(1, 1, 1, 0.8f);
            PSB.Site.Up = true;
            PSB.Site.Content.Forground = new RawColor4(1, 0, 0, 1);
            PSB.Site.List.Background = new RawColor4(0.2f, 0.2f, 0.2f, 0.5f);
            PSB.Site.Pop.Forground = new DX.Color(251, 255, 139, 255);
            PSB.Site.SelectChanged = (o) => {
                int c = (o as DX.ComboBox).List.SelectedIndex;
                SiteChanged(c);
            };
            PSB.sharpback = new RoundBorder();
            PSB.sharpback.RoundRect.RadiusX = 6;
            PSB.sharpback.RoundRect.RadiusY = 6;
            PSB.sharpback.Size = new Size2F(100,20);
            PSB.sharpback.FillBrush = BrushManage.GetLinearA();
            PSB.siteback = new RoundBorder();
            PSB.siteback.RoundRect.RadiusX = 6;
            PSB.siteback.RoundRect.RadiusY = 6;
            PSB.siteback.Size = new Size2F(100,20);
            PSB.siteback.FillBrush = BrushManage.GetLinearA();

            PSB.Back = new DX.Border();
            DX.UIElement.SetClick(PSB.Back,HideOrShow);
            lock (p.DX_Child)
            {
                p.DX_Child.Add(PSB.Back);
                p.DX_Child.Add(PSB.border);
                p.DX_Child.Add(PSB.ellipse);
                p.DX_Child.Add(PSB.play);
                p.DX_Child.Add(PSB.progressA);
                p.DX_Child.Add(PSB.progressB);
                p.DX_Child.Add(PSB.progressC);
                p.DX_Child.Add(PSB.sharpback);
                p.DX_Child.Add(PSB.siteback);
                PSB.Sharp.SetParent(p.DX_Child);
                PSB.Site.SetParent(p.DX_Child);
            }
        }
        static void HideOrShow(DX.UIElement u)
        {
            if(PSB.border.Visble)
            {
                PSB.border.Visble = false;
                PSB.ellipse.Visble = false;
                PSB.play.Visble = false;
                PSB.Sharp.Visble = false;
                PSB.Site.Visble = false;
                PSB.progressA.Visble = false;
                PSB.progressB.Visble = false;
                PSB.progressC.Visble = false;
                PSB.sharpback.Visble = false;
                PSB.siteback.Visble = false;
            }
            else
            {
                PSB.border.Visble = true;
                PSB.ellipse.Visble = true;
                PSB.play.Visble = true;
                PSB.Sharp.Visble = true;
                PSB.Site.Visble = true;
                PSB.progressA.Visble = true;
                PSB.progressB.Visble = true;
                PSB.progressC.Visble = true;
                PSB.sharpback.Visble = true;
                PSB.siteback.Visble = true;
            }
        }
        static void ResizeBar(RawRectangleF m)
        {
            float w = m.Right - m.Left;
            float dx = w * 0.5f - 20;
            float top = m.Bottom - 50;
            PSB.Back.Size = new Size2F(w,m.Bottom-m.Top);
            PSB.border.Location = new Vector2(m.Left,top);
            PSB.border.Size = new Size2F(w,50);
            top += 2;
            PSB.progressA.Location = new Vector2(m.Left,top);
            PSB.progressA.Size = new Size2F(w,6);
            PSB.progressB.Location = new Vector2(m.Left, top);
            PSB.progressC.Location = new Vector2(m.Left, top);
            top += 8;
            PSB.ellipse.Location = new Vector2(dx, top);
            PSB.play.Location = new Vector2(dx,top);
            top += 5;
            dx = m.Left;
            PSB.Sharp.Location = new Vector2(dx,top);
            PSB.sharpback.Location = new Vector2(dx,top);
            dx += 120;
            PSB.Site.Location = new Vector2(dx,top);
            PSB.siteback.Location = new Vector2(dx,top);
        }
        static void Create(Canvas parent)
        {
            current.parent = parent;
            if (me != null)
            {
                me.Visibility = Visibility.Visible;
                display.RequestActive();
#if phone
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
#endif
                return;
            }
            screenW = Window.Current.Bounds.Width;
            screenH = Window.Current.Bounds.Height;
#if phone
            if(screenH>screenW)
            {
                double t = screenH;
                screenH = screenW;
                screenW = t;
            }
#endif
            me = new MediaElement();
            me.Stretch = Stretch.Uniform;
            me.MediaEnded += PlayEnd;
            display = new DisplayRequest();
            display.RequestActive();
            parent.Children.Insert(0,me);
#if phone
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
#endif
            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0,0, 300);
            dt.Tick += (o, e) => { Refresh(); };
        }
        static void PlayOrPause(DX.UIElement u)
        {
            if (me.CurrentState == MediaElementState.Playing)
            {
                PSB.play.Text = ((char)0xe102).ToString();
                me.Pause();
            }
            else
            {
                PSB.play.Text = ((char)0xe103).ToString();
                me.Play();
            }
            ThreadManage.UpdateUI = true;
        }
      
        public static void Resize()
        {
            screenW = Window.Current.Bounds.Width;
            screenH = Window.Current.Bounds.Height;
#if phone
            if (screenH > screenW)
            {
                double t = screenH;
                screenH = screenW;
                screenW = t;
            }
#endif
            me.Width = screenW;
            me.Height = screenH;
            ResizeBar(new RawRectangleF(0,0,(float)screenW,(float)screenH));
        }
        static void ProgressChange(DX.UIElement u)
        {
            X = DX_Input.Position.X;
            Jump();
        }
        static void Jump()
        {
            double tl = X / screenW;
            tl *= vic.alltime;
            int s = (int)tl;
            int p = s / 300;
            if (p >= vic.part)
                p--;
            s -= p * 300;
            int h = s / 3600;
            int t = s % 3600;
            int m = t / 60;
            s = t % 60;
            if (p != current.play_part)
            {
                current.play_part = p;
                me.Stop();
                PlayChange();
            }
            me.Position = new TimeSpan(0, h, m, s);
        }
        
        static void PlayEnd(object o, RoutedEventArgs e)
        {
            current.play_part++;
            if (current.play_part < vic.part)
                PlayChange();
        }
        static void Dispose()
        {
            display.RequestRelease();
#if phone
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
#endif
        }
        #endregion

        #region control
        public static void Setvid(string vid)
        {
            current.vid = vid;
            current.play_part = 0;
            if (va == null)
                va = new VideoAddress();
            va.SetVid(vid,Sharp,PlayStart);
        }
        static void PlayStart(string vid)
        {
            vic = new VideoInfo();
            va.GetAddress(ref vic);
            string str;
            if (vic.type == 1)
                str = vic.href;
            else str = vic.href + "1" + vic.vkey;
            me.Source = new Uri(str);
            me.Play();
            PSB.Sharp.List.Data.Clear();
            for (int i = 0; i < vic.sharp.Length; i++)
                PSB.Sharp.List.Data.Add(vic.sharp[i]);
            PSB.Site.List.Data.Clear();
            int c = vic.site;
            for (int i = 0; i < c; i++)
                PSB.Site.List.Data.Add("站点 "+i.ToString());
            PSB.Site.Content.Text = "站点 0";
            dt.Start();
        }
        static void PlayChange()
        {
            string str;
            if (vic.type == 1)
                str = vic.href;
            else
                str = vic.href + (current.play_part + 1).ToString() + vic.vkey;
            me.Source = new Uri(str);
            me.Play();
        }
        static void Refresh()
        {
            TimeSpan ts = me.Position;
            float s = ts.Minutes * 60 + ts.Seconds;
            s += current.play_part * 300;
            s /= vic.alltime;
            s *= (float)screenW;
            PSB.progressC.Size = new Size2F(s,6);
            double b = me.BufferingProgress;
            ts = me.NaturalDuration.TimeSpan;
            s = ts.Minutes * 60 + ts.Seconds;
            s *= (float)b;
            s += current.play_part * 300;
            s /= vic.alltime;
            s *= (float)screenW;
            PSB.progressB.Size = new Size2F(s, 6);
            if (b < 1)
            {
                b *= 100;
                int c = (int)b;
            }
            ThreadManage.UpdateUI = true;
        }
        static void SharpChanged(int index)
        {
            if (index < 0)
                return;
            va.ChangeSharpA(index,(s)=> {
                va.GetAddress(ref vic);
                string str;
                if (vic.type == 1)
                    str = vic.href;
                else  str = vic.href + (current.play_part+1).ToString() + vic.vkey;
                me.Source = new Uri(str);
            });
            char[] c = vic.sharp[index].ToCharArray();
            Sharp = CharOperation.CharToInt(ref c);
        }
        static void SiteChanged(int index)
        {
            if (index < 0)
                return;
            va.ChangeSite(index ,ref vic);
            string str;
            if (vic.type == 1)
                str = vic.href;
            else str = vic.href + (current.play_part + 1).ToString() + vic.vkey;
            me.Source = new Uri(str);
        }
        #endregion

        #region interface
        public void Create(SwapChain p, RawRectangleF m)
        {
            Create(App.Main);
            CreateBar(p);
            Resize();
            ThreadManage.UpdateUI = true;
        }
        public void Hide()
        {
            PSB.border.Visble = false;
            PSB.ellipse.Visble = false;
            PSB.play.Visble = false;
            PSB.Sharp.Visble = false;
            PSB.Site.Visble = false;
            PSB.progressA.Visble = false;
            PSB.progressB.Visble = false;
            PSB.progressC.Visble = false;
            PSB.sharpback.Visble = false;
            PSB.siteback.Visble = false;
            PSB.Back.Visble = false;
            me.Stop();
            me.Visibility = Visibility.Collapsed;
            dt.Stop();
        }
        public void Show()
        {
            PSB.border.Visble = true;
            PSB.ellipse.Visble = true;
            PSB.play.Visble = true;
            PSB.Sharp.Visble = true;
            PSB.Site.Visble = true;
            PSB.progressA.Visble = true;
            PSB.progressB.Visble = true;
            PSB.progressC.Visble = true;
            PSB.sharpback.Visble = true;
            PSB.siteback.Visble = true;
            PSB.Back.Visble = true;
            me.Visibility = Visibility.Visible;
        }
        public bool Back()
        {
            Dispose();
            Hide();
            return false;
        }
        public void ReSize(RawRectangleF m)
        {
            Resize();
        }
        #endregion
    }
}