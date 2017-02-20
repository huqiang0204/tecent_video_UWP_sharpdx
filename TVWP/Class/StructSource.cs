using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using SharpDX.Mathematics.Interop;
using DX;

namespace TVWP.Class
{
    interface SonPartialPage
    {
        void Create(UIPanel p,Thickness m);
        void UpdatePage(string data);
        void ReSize(Thickness m);
        void Hide();
        void Dispose();
    }
    interface Navigation
    {
        void Create(SwapChain p, RawRectangleF m);
        bool Back();
        void Hide();
        void Show();
        void ReSize(RawRectangleF m);
    }
    struct ImageContext
    {
        public string src;
        public string vid;
        public string title;
        public string detail;
        public int time;
    }
    struct VideoInfo
    {
        public int type;
        public int alltime;
        public int part;
        public int site;
        public string vid;
        public string tilte;
        public string href;
        public string vkey;
        public string[] sharp;
        public string[] cmd5;
    }
    struct CommentInfo
    {
        public string nick;
        public string content;
        public string url;
        public string time;
        public string m_id;//message id
        public string m_r_id;// message root id
        public string vip;
        public string u_id;//user id
        public string region;
        public string count;
        public int rid;
        public int approval;
        public int against;
        public int replay;
    }
    struct UpContent
    {
        public double width;
        public double height;
        public int type;
        public char[] text;
        public string content;
    }
    struct UpCommentInfo
    {
        public string nick;
        public string title;
        public string url;
        public string time;
        public string m_id;//message id
        public string m_r_id;// message root id
        public string vip;
        public string u_id;//user id
        public string region;
        public string count;
        public int rid;
        public int approval;
        public int against;
        public int replay;
        public int score;
        public List<UpContent> detail_s;
    }
    struct FilterOption
    {
        public string Content;
        public string Code;
    }
}
