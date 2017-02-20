using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVWP.Class
{
    class VideoAddress:CharOperation
    {
        struct SharpItem
        {
            public int sharpA;
            public string sharp;
            public string fid;
            public string pid;
            public string fmt;
            public string vkey;
            //public string[] vkeys;
        }
        struct VideoInfoA
        {
            public int type;
            public int alltime;
            public int fregment;
            public string vid;
            public string fn;
            public string title;
            public SharpItem[] vi;
            public string[] http;
            public string[] cmd5;
        }
        string vid;
        VideoInfoA via;
        int defsharp;
        int sharpindex;
        int siteindex;
        Action<string> done;
        string parament;
        public void SetVid(string v,int sharp,Action<string> callback)//sharp
        {
            vid = v;
            defsharp = sharp;
            done = callback;
            siteindex = 0;
            string str;
            switch (sharp)
            {
                case 270:
                    str = "sd";
                    break;
                case 360:
                    str = "hd";
                    break;
                default:
                    str = "shd";
                    break;
            }
            parament = "otype=json&defn=" + str + "&vids=" + vid;
            GetVideoInfo(parament,vid, SetVideoInfo);
        }
        void SetVideoInfo(VideoInfoA a)
        {
            if (a.http.Length == 0)
            {
                //Timer.Stop();
                //Main.Notify("解析失败", 5000);
                return;
            }
            via = a;
            SharpItem[] vi = via.vi;
            ChangeSharp(defsharp);
        }
        public void ChangeSharpA(int index, Action<string> callback)
        {
            done = callback;
            if (index < 0)
                index = 0;
            if (index >= via.vi.Length)
                index = via.vi.Length - 1;
            sharpindex = index;
            if (via.vi[sharpindex].vkey == null)
                GetVideoKey(via.vid, via.fn, via.vi[sharpindex], (vk,p) =>
                {
                    via.vi[sharpindex].vkey = vk;
                    if (done != null)
                        done(vid);
                }, 1);
            else done(vid);
        }
        void ChangeSharp(int sharp)
        {
            defsharp = sharp;
            SharpItem[] vi = via.vi;
            int c = vi.Length;
            int s = 270;
            for (int i = 0; i < c; i++)
            {
                int t = vi[i].sharpA;
                if (t == defsharp)
                {
                    sharpindex = i;
                    break;
                }
                else if (t > s & t < defsharp)
                {
                    s = t;
                    sharpindex = i;
                }
            }
            if (via.vi[sharpindex].vkey == null)
                GetVideoKey(via.vid, via.fn, via.vi[sharpindex], (vk,p) =>
                {
                    via.vi[sharpindex].vkey = vk;
                    if (done != null)
                        done(vid);
                }, 1);
            else done(vid);
        }
        public void GetAddress(ref VideoInfo vic)
        {
            vic.part = via.fregment;
            vic.alltime = via.alltime;
            SharpItem vi = via.vi[sharpindex];
            vic.type = via.type;
            if (via.type == 1)
            {
                vic.href = via.http[siteindex] + via.fn + ".mp4?type=mp4&fmt=auto&vkey=" + vi.vkey
                         + "";// + vi.fmt;
            }
            else
            {
                vic.href = via.http[siteindex] + via.fn + vi.pid;
                vic.vkey = ".mp4?sdtfrom=v1001&type=mp4&fmt=auto&vkey=" + vi.vkey;//+ vi.fmt;//href + part +vkey
            }
            int c = via.vi.Length;
            vic.sharp = new string[c];
            for (int i = 0; i < c; i++)
                vic.sharp[i] = via.vi[i].sharp;
            vic.site = via.http.Length;
            vic.tilte = via.title;
            vic.vid = vid;
            vic.cmd5 = via.cmd5;
        }
        public void ChangeSite(int index,ref VideoInfo vic)
        {
            siteindex = index;
            SharpItem vi = via.vi[sharpindex];
            vic.type = via.type;
            if (via.type == 1)
            {
                vic.href = via.http[index] + via.fn + ".mp4?vkey=" + vi.vkey
                         + "&type=mp4&fmt=" + vi.fmt;
            }
            else
            {
                vic.href = via.http[index] + via.fn + vi.pid;
                vic.vkey = ".mp4?vkey=" + vi.vkey + "&type=mp4&fmt=" + vi.fmt;//href + part +vkey
            }
        }
        public async Task<string> GetPartVkey(int part)
        {
            SharpItem vi = via.vi[sharpindex];
            string str = "platform=11&otype=xml&vids=" + vid + "&format=" + vi.fid + "&filename=" + via.fn + vi.pid;
            str += part.ToString() + ".mp4";
            str = await WebClass.Post("http://vv.video.qq.com/getkey", str,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20161114");
            //Debug.WriteLine(str);
            char[] tc = str.ToCharArray();
            int ccc = 0;
            tc = FindCharArrayA(ref tc, ref Key_key, ref Key_less, ref ccc);
            if (tc != null)
                return new string(tc);
            else return null;
        }

        static async void GetVideoInfo(string p,string vid, Action<VideoInfoA> callback)
        {
            string s = await WebClass.Post("http://vv.video.qq.com/getinfo",p,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20161114");
            char[] c = s.ToCharArray();
            c = DeleteChar(ref c, '\\');
            //Debug.WriteLine(new string(c));
            VideoInfoA vi = new VideoInfoA();
            vi.vid = vid;
            vi.http = GetHttp(ref c);
            GetVI(ref c, ref vi);
            if(vi.fregment>0)
            {
                string[] md5 = new string[vi.fregment];
                GetCmd5(ref c,ref md5);
                vi.cmd5 = md5;
            }
            callback(vi);
        }
        static string[] GetHttp(ref char[] source)
        {
            string[] ss = new string[6];
            int c = 0;
            int i = 0;
            label0:;
            c = FindCharArray(ref source, ref Key_urlA, c);
            if (c < 0)
            {
                string[] temp = new string[i];
                for (int p = 0; p < i; p++)
                    temp[p] = ss[p];
                return temp;
            }
            char[] tt = FindCharArrayA(ref source, '\"', '\"', ref c);
            int t = FindCharArray(ref tt, '/', 0);
            t = (int)tt[t + 2];
            if (t > 57 || t < 48)
            {
                if(FindCharArray(ref tt,ref Key_dispatch,0)>0)
                {
                    string str;
                    if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                        str = new string(tt);
                    else str = "http:" + new string(tt);
                    ss[i] = ss[0];
                    ss[0] = str;
                    i++;
                }
                goto label0;
            }
            if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                ss[i] = new string(tt);
            else ss[i] = "http:" + new string(tt);
            i++;
            goto label0;
        }
        static void GetCmd5(ref char[] source,ref string[] str)
        {
            int c = str.Length;
            int s= 0;
            for(int i=0;i<c;i++)
            {
                s = FindCharArray(ref source, ref Key_cmd5, s);
                if (s < 0)
                    return;
                s++;
                str[i] = new string(FindCharArrayA(ref source,'\"','\"',ref s));
            }
        }
        static void GetVI(ref char[] source, ref VideoInfoA vi)
        {
            int t = FindCharArray(ref source, ref Key_td, 0);
            char[] tt = FindCharArrayA(ref source, '\"', '\"', ref t);
            vi.alltime = CharToInt(ref tt);
            t= FindCharArray(ref source, ref Key_ti, t);
            tt = FindCharArrayA(ref source, '\"', '\"', ref t);
            vi.title = new string(tt);
            t = FindCharArray(ref source, ref Key_fc, 0);
            tt = FindCharArrayA(ref source, ':', ',', ref t);
            vi.fregment = CharToInt(ref tt);
            int s = 0;
            int a = FindCharArray(ref source, ref Key_fn, 0);
            char[] fn = FindCharArrayA(ref source, '\"', '.', ref a);
            vi.fn = new string(fn);
            a++;
            if (source[a] != 'p')
                vi.type = 1;
            else vi.type = 0;

            SharpItem[] temp = new SharpItem[6];
            int c = s = 0;
            for (int i = 0; i < 6; i++)
            {
                tt = FindCharArrayA(ref source, '(', ')', ref s);
                if (tt == null)
                    break;
                SharpItem VI = new SharpItem();
                VI.sharp = new string(tt);
                VI.sharpA = CharToInt(ref tt);
                s = FindCharArray(ref source, ref Key_idA, s);
                if (s < 0)
                    break;
                tt = FindCharArrayA(ref source, ':', ',', ref s);
                if (tt != null)
                {
                    if (tt.Length > 2)
                    {
                        s = FindCharArray(ref source, ref Key_name, s);
                        VI.fmt = new string(FindCharArrayA(ref source, '\"', '\"', ref s));
                        string str = new string(CopyCharArry(ref tt, tt.Length - 3, 3));
                        VI.fid = "10" + str;
                        VI.pid = ".p" + str + ".";
                        temp[c] = VI;
                    }
                    else
                    {
                        s = FindCharArray(ref source, ref Key_name, s);
                        VI.fmt = new string(FindCharArrayA(ref source, '\"', '\"', ref s));
                        a = FindCharArray(ref source, ref Key_fvkey, a);
                        VI.vkey = new string(FindCharArrayA(ref source, '\"', '\"', ref a));
                        temp[c] = VI;
                    }
                    c++;
                }
            }
            SharpItem[] r = new SharpItem[c];
            for (int d = 0; d < c; d++)
                r[d] = temp[d];
            vi.vi = r;
            return;
        }
        static async void GetVideoKey(string vid, string fn, SharpItem vi, Action<string ,int > callback, int part)
        {
            string str = "platform=11&otype=xml&vids=" + vid + "&format=" + vi.fid + "&filename=" + fn + vi.pid;
            str += part.ToString() + ".mp4";
            str = await WebClass.Post("http://vv.video.qq.com/getkey", str,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20161114");
            char[] tc = str.ToCharArray();
            int ccc = 0;
            tc = FindCharArrayA(ref tc, ref Key_key, ref Key_less, ref ccc);
            if (tc != null)
            {
                str = new string(tc);
                callback(str,part);
            }
            else callback(null,part);
        }    
        static async void GetVideoFregment(string vid, string fn, SharpItem vi, Action<int> callback)
        {
            string str = "platform=11&otype=xml&vids=" + vid + "&format=" + vi.fid + "&filename=" + fn + vi.pid;
            int c = 2;
            l:;
            string cc = str + c.ToString() + ".mp4";
            cc = await WebClass.Post("http://vv.video.qq.com/getkey", cc,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20160819");
            char[] tc = cc.ToCharArray();
            //Debug.WriteLine(new string(DeleteChar(ref tc, '/')));
            if (FindCharArray(ref tc, ref Key_key, 0) > 0)
            { c++; goto l; }
            c--;
            callback(c);
        }     
    }
}
