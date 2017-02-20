using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace TVWP.Class
{
    //main thread
    class InputText
    {
        static TextBox tb;
        static string content;
        public static void Create(Canvas p,Thickness margin)
        {
            if (tb != null)
            {
                tb.Visibility = Visibility.Visible;
                tb.Margin = margin;
                return;
            }
            tb = new TextBox();
            tb.Margin = margin;
            tb.Width = 240;
            tb.Height = 24;
            tb.KeyDown += KeyDown;
            p.Children.Add(tb);
        }
        static void KeyDown(Object sender, KeyRoutedEventArgs e)
        {
            if(e.Key==Windows.System.VirtualKey.Enter)
            {
                content = tb.Text;
                DX.ThreadManage.AsyncDelegate(()=>NavPage.Find(content));
            }
        }
        public static void ReDock(Thickness margin)
        {
            tb.Margin = margin;
        }
        public static void Hide()
        {
            tb.Visibility = Visibility.Collapsed;
        }
        public static void Show()
        {
            tb.Visibility = Visibility.Visible;
        }
    }
}
