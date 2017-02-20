using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;

namespace TVWP.Class
{
    class PageControl
    {
        public static Canvas main_canv { get; set; }
        static Canvas se_canv;
        static ScrollViewer scrollv;
        public static Color font_color { get; set; }
        public static Double font_size { get; set; }
        public static Color bk_color { get; set; }
        static ButtonBase[] buff_button;
        static TextBase[] buff_text;
        static ImageBase[] buff_img;
        static BitmapImage bi;
        static double screenX,screenY;
        public static void UseDefSet()
        {
            font_color = Color.FromArgb(255, 0, 0, 0);
            bk_color = Color.FromArgb(128, 128, 128, 128);
            font_size = 24;
            main_canv.Background =new SolidColorBrush( bk_color);
        }
        public static void Initial()
        {
            buff_button = new ButtonBase[64];
            buff_text = new TextBase[256];
            buff_img = new ImageBase[256];
            screenX = Window.Current.Bounds.Width;
            screenY = Window.Current.Bounds.Height;
            
            scrollv = new ScrollViewer();
            scrollv.Height = screenY - 40;
            main_canv.Children.Add(scrollv);
            Thickness tk = new Thickness();
            tk.Top = 40;
            tk.Right = screenX;
            scrollv.Margin = tk;
            se_canv = new Canvas();
            se_canv.Width = screenX;
            se_canv.Height = screenY;
            scrollv.Content = se_canv;
           
        }

        #region button control
        public static int CreateButton(ref ButtonProperty target)
        {
            int i;
            for (i = 0; i < 64; i++)
            {
                if (!buff_button[i].reg)
                {
                    buff_button[i].reg = true;
                    break;
                }
                if (buff_button[i].restore)
                {
                    buff_button[i].restore = false;
                    buff_button[i].button.Visibility = Visibility.Visible;
                    buff_button[i].button.Click -= buff_button[i].bp.click;
                    goto label1;
                }
            }
            Button button = new Button();
            buff_button[i].button = button;
            label1:;
            buff_button[i].bp = target;
            button = buff_button[i].button;
            target.margin.Left = target.x;
           // bp.margin.Right = screenX - bp.x - bp.Width;
            target.margin.Top = target.y;
            target.margin.Bottom = screenY - target.y - target.Width;
            button.Margin = target.margin;
            button.Content = target.text;
            if (target.angle > 0)
            {
                RotateTransform rtf = new RotateTransform();
                rtf.Angle = target.angle;
                button.RenderTransform = rtf;
            }
            if (target.color.A > 0)
            {
                button.Foreground = new SolidColorBrush(target.color);
            }
            else
            {
                button.Foreground = new SolidColorBrush(font_color);
            }
            if(target.bk_color.A>0)
            {
                button.Background = new SolidColorBrush(target.bk_color);
            }
            if (target.size > 0)
                button.FontSize = target.size;
            if (target.click != null)
               button.Click += target.click;
            button.DataContext = target.data;
            main_canv.Children.Add(button);
            return i + 256;
        }
        public static void RecyleButton(int id)
        {
            if (id < 255)
                return;
            id -= 256;
            buff_button[id].restore = true;
            buff_button[id].button.Visibility = Visibility.Collapsed;
        }
        public static void CreateButton(List<ButtonProperty> target,ref int[] id)
        {
            for (int i = 0; i < target.Count; i++)
            {
                ButtonProperty temp = target[i];
                id[i] = CreateButton(ref temp);
            }
        }
        public static void CreateButton(ref ButtonProperty[] target, ref int[] id)
        {
            for (int i = 0; i < target.Length; i++)
            {
                id[i] = CreateButton(ref target[i]);
            }
        }
        public static void FlashButton()
        {
            for(int i=0;i<buff_button.Length;i++)
            {
                if(buff_button[i].reg)
                    if(!buff_button[i].restore)
                    {
                       
                    }
            }
        }
        #endregion

        #region text control
        public static int CreateText(ref TextProperty target)
        {
            int i;
            for( i=0;i<256;i++)
            {
                if(!buff_text[i].reg)
                {
                    buff_text[i].reg = true;
                    break;
                }
                if(buff_text[i].restore)
                {
                    buff_text[i].restore = false;
                    buff_text[i].t_b.Visibility = Visibility.Visible;
                    goto label1;
                }
            }
            TextBlock tb = new TextBlock();
            buff_text[i].t_b = tb;
            label1:;
            buff_text[i].tp = target;
            
            tb = buff_text[i].t_b;
            tb.Margin = target.margin;
            tb.Text = target.text;
            if(target.angle>0)
            {
                RotateTransform rtf = new RotateTransform();
                rtf.Angle = target.angle;
                tb.RenderTransform = rtf;
            }
            if (target.color.A > 0)
            {
                tb.Foreground =new  SolidColorBrush(target.color);
            }
            else
            {
                tb.Foreground = new SolidColorBrush(font_color);
            }
            if (target.size > 0)
                tb.FontSize = target.size;
            //main_grid.Children.Add(tb);
            if (target.margin.Top > se_canv.Height)
                se_canv.Height = target.margin.Top + 50;
            se_canv.Children.Add(tb);
            return i+256;
        }
        public static void RecyleText(int id)
        {
            if (id < 255)
                return;
            id -=256;
            buff_text[id].restore = true;
            buff_text[id].t_b.Visibility = Visibility.Collapsed;
        }
        public static void ReSetText(int id,string text)
        {
            if (id < 255)
                return;
            id -= 256;
            buff_text[id].tp.text = text;
        }
        public static void CreateText(List<TextProperty> target,ref int[] id)
        {
            for (int i = 0; i < target.Count; i++)
            {
                TextProperty temp = target[i];
                id[i] = CreateText(ref temp);
            }
        }
        public static void CreateText(ref TextProperty[] target, ref int[] id)
        {
            for (int i = 0; i < target.Length; i++)
            {
                id[i] = CreateText(ref target[i]);
            }
        }
        #endregion

        #region img control
        public static int CreateImg(ref ImageProperty target)
        {
            int i;
            for (i = 0; i < 256; i++)
            {
                if (!buff_img[i].reg)
                {
                    buff_img[i].reg = true;
                    break;
                }
                if (buff_img[i].restore)
                {
                    buff_img[i].restore = false;
                    buff_img[i].img.Visibility = Visibility.Visible;
                    goto label1;
                }
            }
            Image img = new Image();
            buff_img[i].img = img;
            label1:;
            if (buff_img[i].can == null)
                buff_img[i].can = new Canvas();
            Canvas can = buff_img[i].can;
            Thickness tk = new Thickness();
            tk.Left = target.x;
            tk.Top = target.y;
            can.Margin = tk;
            buff_img[i].ip = target;
            img = buff_img[i].img;
            if (target.angle>0)
            {
                RotateTransform rtf = new RotateTransform();
                rtf.Angle = target.angle;
                img.RenderTransform = rtf;
            }
            BitmapImage bi = new BitmapImage();
            bi.UriSource = new Uri("ms-appx:///resource/Play.png");//
            img.Source = bi;
            can.Children.Add(img);
            se_canv.Children.Add(can);
            return i+256;
        }
        public static void RecyleImg(int id)
        {
            if (id < 255)
                return;
            id -= 256;
            buff_img[id].restore = true;
            buff_img[id].img.Visibility = Visibility.Collapsed;
        }
        public static void ReSetImg(int id,ImageSource img)
        {
            if (id < 255)
                return;
            id -= 256;
            buff_img[id].img.Source = img;
        }
        public static void CreateImg(List<ImageProperty> target, ref int[] id)
        {
            for (int i = 0; i < target.Count; i++)
            {
                ImageProperty temp = target[i];
                id[i] = CreateImg(ref temp);
            }
        }
        public static void CreateImg(ref ImageProperty[] target, ref int[] id)
        {
            for (int i = 0; i < target.Length; i++)
            {
                id[i] = CreateImg(ref target[i]);
            }
        }
        #endregion

        #region
        public static void test()
        {
           
        }
        #endregion
    }
}
