using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX
{
   public class ComboBox
    {
        public TextBlock Content { get; private set; }
        public TextBlock Pop { get; private set; }
        public ListBox List { get; private set; }
        public Action<ComboBox> SelectChanged;
        bool up;
        public bool Up {get { return up; } set {
                up = value;
                if (up)
                    Pop.Text =((char)0xe0a0).ToString();
                else Pop.Text = ((char)0xe0a1).ToString();
            } }
        public ComboBox()
        {
            Pop = new TextBlock();
            Pop.Forground = new RawColor4(0,0,0,1);
            Pop.Size = new Size2F(20,20);
            Pop.FontStyle = TextBlock.FontName.SegoeUISymbol;
            Pop.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            Pop.Text = ((char)0xe0a1).ToString();
            UIElement.SetClick(Pop,Click);
            Content = new TextBlock();
            Content.Forground = new RawColor4(0,0,0,1);
            Content.Alignment = SharpDX.DirectWrite.TextAlignment.Center;
            List = new ListBox();
            List.Visble = false;
            List.Forground = new RawColor4(0,0,0,1);
            List.Context = this;
            List.SelectChanged = (o) =>
            {
                ListBox lb = o as ListBox;
                int c = lb.SelectedIndex;
                if (c < 0)
                {
                    Content.Text = "";
                    return;
                }
                Content.Text = lb.Data[c];
                if (SelectChanged != null)
                    SelectChanged(lb.Context as ComboBox);
            };
        }
        Size2F size;
        public Size2F Size { get { return size; } set {
                size = value;
                float w = size.Width-20;
                if (w < 0)
                    w = 0;
                Content.Size = new Size2F(w,20);
                Pop.Location = new Vector2(location.X+w,location.Y);
            } }
        Vector2 location;
        public Vector2 Location { get { return location; } set {
                location = value;
                Content.Location = location;
                float w = Content.Size.Width;
                Pop.Location = new Vector2(location.X+w,location.Y);
            } }
        void Click(UIElement u)
        {
            if (List.Visble)
            {
                List.Visble = false;
                if(up)
                    Pop.Text = ((char)0xe0a0).ToString();
                else Pop.Text = ((char)0xe0a1).ToString();
            }
            else
            {
                if (up)
                {
                    List.Location = new Vector2(location.X, location.Y - List.Size.Height);
                    Pop.Text = ((char)0xe0a1).ToString();
                }
                else
                {
                    List.Location = new Vector2(location.X, location.Y + size.Height);
                    Pop.Text = ((char)0xe0a0).ToString();
                }
                List.Visble = true;
            }
        }
        List<UIElement> parent;
        public void SetParent(List<UIElement> p)
        {
            parent = p;
            parent.Add(Content);
            parent.Add(Pop);
            parent.Add(List);
        }
        public void BreakParent()
        {
            parent.Remove(Content);
            parent.Remove(Pop);
            parent.Remove(List);
            parent = null;
        }
        public bool Visble { set {
                Content.Visble = value;
                Pop.Visble = value;
                if (!value)
                    List.Visble = false;
            } }
        ~ComboBox()
        {
            Dispose();
        }
        public void Dispose()
        {
            BreakParent();
            Content.Dispose();
            Pop.Dispose();
            List.Dispose();
        }
    }
}
