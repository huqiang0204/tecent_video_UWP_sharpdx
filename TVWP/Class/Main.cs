using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Windows.Foundation;
using Windows.Globalization;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX;
using DX;

namespace TVWP.Class
{
    enum PageTag : int
    {
        none,main,partial,page_m, nav,search,videopage,player,playerEx,all
    }
    class Main:Component
    {

        #region main
        static Navigation Nav { get; set; }

        static SwapChain Swap;
        //static Border bk_ground;
        public static void Initial(SwapChain swap)
        {
            Swap = swap;
            PageManageEx.Initial(swap);
            PageManageEx.CreateNewPage(PageTag.main);   
        }
        #endregion
    }

   public class PageManageEx
    {
        static Navigation[] nav_buff;
        static Navigation[] ln;
        static Navigation current;
        static int point;
        static SwapChain parent;
        static void CreateNewPageA(PageTag tag)
        {
            switch (tag)
            {
                case PageTag.main:
                    nav_buff[(int)PageTag.main] = new MainEx();
                    break;
                case PageTag.partial:
                    //nav_buff[(int)PageTag.partial] = new PartialNav();
                    break;
                case PageTag.page_m:
                    //nav_buff[(int)PageTag.page_m] = new PageNav_m();
                    break;
                case PageTag.nav:

                    break;
                case PageTag.search:

                    break;
                case PageTag.videopage:
                    nav_buff[(int)PageTag.videopage] = new VideoPage();
                    break;
                case PageTag.player:
                    nav_buff[(int)PageTag.player] = new Player();
                    break;
                    //case PageTag.playerEx:
                    //    nav_buff[(int)PageTag.playerEx] = new PlayerEx();
                    //    break;
            }
        }

        public static void Initial(SwapChain p)
        {
            nav_buff = new Navigation[(int)PageTag.all];
            ln = new Navigation[8];
            parent = p;
            point = -1;
        }
        internal static void CreateNewPage(PageTag tag)
        {
            point++;
            if (current != null)
                current.Hide();
            int index = (int)tag;
            if (nav_buff[index] == null)
                CreateNewPageA(tag);
            current = nav_buff[index];
            ln[point] = current;
#if phone
            float x=0;
            float y=0;
            if (Component.screenX > Component.screenY)
                x += 45;
            else y += 23;
            current.Create(parent, new RawRectangleF(x, y, (float)Component.screenX, (float)Component.screenY));
#else
            current.Create(parent, new RawRectangleF(0, 0, (float)Component.screenX, (float)Component.screenY));
#endif
            ThreadManage.UpdateUI = true;
        }
        public static bool Back()
        {
            if (current.Back())
                return true; 
            else
            {
                point--;
                if (point < 0)
                    return true;
                current = ln[point];
                current.Show();
                ReSize();
                return false;
            }
        }
        public static void ReSize()
        {
#if phone
            float x = 0;
            float y = 0;
            if (Component.screenX > Component.screenY)
                x += 45;
            else y += 23;
            current.ReSize(new RawRectangleF(x, y, (float)Component.screenX, (float)Component.screenY));
#else
              current.ReSize(new RawRectangleF(0, 0, (float)Component.screenX, (float)Component.screenY));
#endif
            ThreadManage.UpdateUI = true;
        }
    }

    class MainEx : Component,Navigation
    {

        #region part
        public void Create(SwapChain p, RawRectangleF m)
        {
            m.Top += 36;
            NavPage.Create(p, m);
            ThreadManage.MissionToMain(()=> {
#if phone
                float y = 0;
                if (Component.screenX < Component.screenY)
                   y += 23;
                InputText.Create(App.Main, new Thickness(screenX - 240, y, screenX, 0));
#else
                InputText.Create(App.Main, new Thickness(screenX - 240, 0, screenX, 0));
#endif
            });
        }
        public bool Back()
        {
            Hide();
            return true;
        }
        public void Hide()
        {
            NavPage.Hide();
            ThreadManage.MissionToMain(() => {
                InputText.Hide();
            });
        }
        public void Show()
        {
            NavPage.Show();
            ThreadManage.MissionToMain(() => {
                InputText.Show();
            });
        }
        public void ReSize(RawRectangleF m)
        {
            m.Top += 36;
            NavPage.ReSize(m);
#if phone
            float y = 0;
            if (Component.screenX < Component.screenY)
                y += 23;
            InputText.ReDock(new Thickness(screenX - 240, y, screenX, 0));
#else
            InputText.Create(App.Main, new Thickness(screenX - 240, 0, screenX, 0));
#endif
        }
#endregion

    }
}
