using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVWP.Class
{
    class ParseData:CharOperation
    {
        #region navigation
        public static bool Analyze_Nav(ref char[] c_buff, List<DX.BindingMod> lid, int count,int start)
        {
            int s = FindCharArray(ref c_buff, ref Key_figures_list, start);
            if (s < 0)
                return false;
            int i = 0, e;
            s = FindCharArray(ref c_buff, ref Key_list_itemA, s);
            if (s > 0)
                for (i = 0; i < count; i++)
                {
                    e = FindCharArray(ref c_buff, ref Key_list_itemA, s);
                    if (e < 0)
                    {
                        e = c_buff.Length;
                        lid.Add(GetItemDataA(ref c_buff, s, e));
                        i++;
                        break;
                    }
                    lid.Add(GetItemDataA(ref c_buff, s, e));
                    s = e;
                }
            return true;
        }
        static DataMod GetItemDataA(ref char[] c_buff, int s, int e)
        {
            DataMod im = new DataMod();
            s = FindCharArray(ref c_buff, ref Key_href, s);
            string str = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            im.href = str;
            s = FindCharArray(ref c_buff, ref Key_lazyload, s);
            char[] t = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
            if (FindCharArray(ref t, ref Key_http, 0) > -1)
                im.src = new string(t);
            else im.src = "http:" + new string(t);
            s = FindCharArray(ref c_buff, ref Key_alt, s);
            im.Title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));

            int ae = FindCharArray(ref c_buff, ref Key_a_e, s);
            int ss = FindCharArray(ref c_buff, ref Key_mask, s, ae);
            string mask = "";
            if (ss > 0)
                mask = new string(FindCharArrayA(ref c_buff, '>', '<', ref ss)) + "\r\n";
            ss = FindCharArray(ref c_buff, ref Key_mark, s, ae);
            if (ss > 0)
            {
                ss += 2;
                int a = ss;
                t = FindCharArrayA(ref c_buff,'_','.' , ref a);
                im.marktag = CharToInt(ref t);
            }

            int d = FindCharArray(ref c_buff, ref Key_figure_desc, s, e);
            if (d > 0)
            {
                char[] tt = FindCharArrayA(ref c_buff, '>', '<', ref d);
                tt = DeleteChar(ref tt, (char)9, '\r', '\n');
                str = new string(tt);
                int end = FindCharArray(ref c_buff, ref Key_div_e, d);
                int o = d;
                for (int c = 0; c < 4; c++)
                {
                    o = FindCharArray(ref c_buff, ref Key_title, o, end);
                    if (o < 0)
                        break;
                    str += " " + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref o));
                }
                s = d;
            }
            else str = "";
            d = FindCharArray(ref c_buff, ref Key_info_inner, s, e);
            if (d > 0)
                str = str + "\r\n播放数:" + new string(FindCharArrayA(ref c_buff, '>', '<', ref d));
            im.Detail = mask + str;
            return im;
        }
        public static int GetFilterOption(ref char[] c_buff,List<FilterOption>[] lfo_buff,ref string[] Title)
        {
            int s = FindCharArray(ref c_buff, ref Key_mod_filter_box, 0);
            int i;
            for(i=0;i<8;i++)
            {
                int t = FindCharArray(ref c_buff,ref Key_label,s);
                if (t < 0)
                    break;
                s = t;
                char[] tt = FindCharArrayA(ref c_buff, '>', '<', ref s);
                string str= new string(tt);
                Title[i] = str.Replace("&nbsp;",String.Empty);
                int e = FindCharArray(ref c_buff, ref Key_div_e, s);
                if (lfo_buff[i] != null)
                    lfo_buff[i].Clear();
                else lfo_buff[i] = new List<FilterOption>();
                var lfo = lfo_buff[i];
                FilterOption fo;
                for(int c=0;c<20;c++)
                {
                    t =  FindCharArray(ref c_buff,ref Key_a,s,e);
                    if (t < 0)
                        break;
                    s = t;
                    char[] temp = FindCharArrayA(ref c_buff,'\"','\"',ref s);
                    int ts= FallFindCharArray(ref temp,';',temp.Length-1);
                    ts++;
                    temp = CopyCharArry(ref temp,ts,temp.Length-ts);
                    fo.Code = new string(temp);
                    fo.Content = new string(FindCharArrayA(ref c_buff,'>','<',ref s));
                    lfo.Add(fo);
                }
            }
            if (s < 0)
                s = 0;
            for(int c=i;c<8;c++)
                if(lfo_buff[c]!=null)
                {
                    lfo_buff[c].Clear();
                    lfo_buff[c] = null;
                    Title[c] = null;
                }
            return s;
        }
        #endregion

        #region videoview
        public static void GetListInfo(ref char[] c_buff, List<DX.BindingMod> data, int s)
        {
            int t = FindCharArray(ref c_buff, ref Key_listinfo, s);
            if (t < 0)
                return;
            int e = FindCharArray(ref c_buff, ref Key_listinfoE, t);
            t = FindCharArray(ref c_buff, ref Key_data, t);
            t = FindCharArray(ref c_buff, ref Key_vidB, t, e);
            while (t > 0)
            {
                DataModA ei = new DataModA();
                ei.vid = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                t = FindCharArray(ref c_buff, ref Key_duration, t);
                t++;
                ei.src = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                t = FindCharArray(ref c_buff, ref Key_titleA, t);
                t++;
                ei.Title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                t = FindCharArray(ref c_buff, ref Key_preview, t);
                char[] c = FindCharArrayA(ref c_buff, '\"', '\"', ref t);
                if (c[0] != 'h')
                    ei.src = "http:" + new string(c);
                else ei.src = new string(c);
                t = FindCharArray(ref c_buff, ref Key_vidB, t, e);
                data.Add(ei);
            }
        }
        public static void GetCoverList(ref char[] c_buff, List<DX.BindingMod> lic, int start, int ec)
        {
            int e = FindCharArray(ref c_buff, ref Key_listinfoE, start);
            if (e < 0)
                e = c_buff.Length;
            int s = start;
            s = FindCharArray(ref c_buff, ref Key_data, s);
            s = FindCharArray(ref c_buff, ref Key_vidB, s, e);
            int index = ec;
            int count = 0;
            while (s > 0)
            {
                DataModA ic = new DataModA();
                ic.vid = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_titleA, s);
                s++;
                ic.Title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_preview, s);
                if (s < 0)
                    break;
                char[] t = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                if (FindCharArray(ref t, ref Key_http, 0) > -1)
                    ic.src = new string(t);
                else ic.src = "http:" + new string(t);
                if (count >= index)
                    lic.Add(ic);
                s = FindCharArray(ref c_buff, ref Key_vidB, s, e);
                count++;
            }
        }
        public static void GetPageList(ref char[] c_buff, List<DX.BindingMod> lic)
        {
            int s = FindCharArray(ref c_buff, ref Key_mod_playlist, 0);
            if (s < 0)
                return;
            s = FindCharArray(ref c_buff, ref Key_list_itemA, s);
            int e = FindCharArray(ref c_buff, ref Key_ul_e, s);
            int c;
            while (s > 0)
            {
                DataModA ic = new DataModA();
                c = FindCharArray(ref c_buff, ref Key_href, s);
                char[] href = FindCharArrayA(ref c_buff, '\"', '\"', ref c);
                int a = FindCharArray(ref href, ref Key_vidA, 0);
                if (a > 0)
                    ic.vid = new string(CopyCharArry(ref href, a, href.Length - a));
                else goto label0;
                //ic.href = "http://v.qq.com" + new string(href);
                c = FindCharArray(ref c_buff, ref Key_src, c);
                char[] t = FindCharArrayA(ref c_buff, '\"', '\"', ref c);
                if (FindCharArray(ref t, ref Key_http, 0) > -1)
                    ic.src = new string(t);
                else ic.src = "http:" + new string(t);
                c = FindCharArray(ref c_buff, ref Key_alt, c);
                ic.Title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref c));
                lic.Add(ic);
                label0:;
                s = FindCharArray(ref c_buff, ref Key_list_itemA, c);
                if (s > e)
                    break;
            }
        }
        public static int GetEp_infoA(ref char[] c_buff, List<FilterOption> lep)
        {
            int s = 0;
            s = FindCharArray(ref c_buff, ref Key_mod_episode, s);
            if (s < 0)
                return 0;
            int e = FindCharArray(ref c_buff, ref Key_mod_playlist, s);
            FilterOption ep = new FilterOption();
            do
            {
                s = FindCharArray(ref c_buff, ref Key_curvid, s);
                if (s > e || s < 0)
                    break;
                ep.Code = new string(FindCharArrayA(ref c_buff, '\'', '\'', ref s));
                s = FindCharArray(ref c_buff, ref Key_titleA, s);
                ep.Content = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                lep.Add(ep);
            } while (s < e);
            return e;
        }
        public static ImageContext GetVideoInfo(ref char[] c_buff, int s)
        {
            ImageContext ic = new ImageContext();
            s = FindCharArray(ref c_buff, ref Key_interactionCount, s);
            s += 4;
            string text = "播放数:" + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            s = FindCharArray(ref c_buff, ref Key_datePublished, s);
            s += 4;
            text += "  发布时间：" + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            ic.detail = text;
            int d = FindCharArray(ref c_buff, ref Key_description, 0);
            if (d > 0)
            {
                d = FindCharArray(ref c_buff, ref Key_content, d);
                d++;
                ic.detail += "\r\n" + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref d));
            }
            s = FindCharArray(ref c_buff, ref Key_img, s);
            d = FindCharArray(ref c_buff, ref Key_content, s);
            ic.src += new string(FindCharArrayA(ref c_buff, '\"', '\"', ref d));
            s = FindCharArray(ref c_buff, ref Key_videoinfo, s);
            if (s < 0)
                return ic;
            s = FindCharArray(ref c_buff, ref Key_titleA, s);
            ic.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            s++;
            s = FindCharArray(ref c_buff, ref Key_duration, s);
            s++;
            text = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            ic.time = Convert.ToInt32(text);
            s = FindCharArray(ref c_buff, ref Key_vidB, s);
            s++;
            ic.vid = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            return ic;
        }
        #endregion

        //public static void Analyze_Home(string data, int start, int end, int s ,ref Area[] area)
        //{
        //    char[] c_buff = data.ToCharArray();
        //    c_buff = DeleteChar(ref c_buff, '\\');
        //    int st = s;
        //    for (int i = start; i < end; i++)
        //    {
        //        char[] tmp = nav_all[i].title[0].ToCharArray();
        //        int ts = FindCharArray(ref c_buff, ref tmp, st);
        //        if (ts > 0)
        //        {
        //            ItemDataA[] icb = area[i].data;
        //            int l = nav_all[i].count;
        //            s = ts;
        //            for (int d = 0; d < l; d++)
        //            {
        //                s = FindCharArray(ref c_buff, ref Key_list_item, s);
        //                s = FindCharArray(ref c_buff, ref Key_href, s);
        //                char[] tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
        //                string aa;
        //                if (FindCharArray(ref tt, ref Key_http, 0) > -1)
        //                    aa = new string(tt);
        //                else aa = "http:" + new string(tt);
        //                icb[d].href = aa;

        //                s = FindCharArray(ref c_buff, ref Key_title, s);
        //                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
        //                icb[d].title = new string(tt);

        //                s = FindCharArray(ref c_buff, ref Key_src, s);
        //                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
        //                if (FindCharArray(ref tt, ref Key_about, 0) > -1)
        //                    break;
        //                if (FindCharArray(ref tt, ref Key_http, 0) > -1)
        //                    aa = new string(tt);
        //                else aa = "http:" + new string(tt);
        //                icb[d].src = aa;
        //            }
        //        }
        //    }
        //}

        static char[] Key_up = "up\"".ToCharArray();
        static char[] Key_rep = "orireplynum\"".ToCharArray();
        static char[] Key_poke = "poke\"".ToCharArray();
        static char[] Key_score = "score\"".ToCharArray();
        public static int AnalyComment(ref char[] c_buff,List<CommentInfo> lci)
        {
            int s = 0, ss = lci.Count;
            CommentInfo ci = new CommentInfo();
            int i, rid = ss;
            for (i = 0; i < 20; i++)
            {
                s = FindCharArray(ref c_buff, ref Key_js_id, s);
                if (s < 0)
                    break;
                ci.m_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                int r = FindCharArray(ref c_buff, ref Key_js_rootid, s, s + 20);
                if (r > 0)
                {
                    ci.m_r_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref r));
                    ci.rid = rid;
                    s = r;
                }
                else { ci.m_r_id = null; rid = i + ss; };
                s = FindCharArray(ref c_buff, ref Key_js_timeD, s);
                char[] tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.time = GetString16A(ref tt);
                s = FindCharArray(ref c_buff, ref Key_js_content, s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.content = GetString16(ref tt);

                s = FindCharArray(ref c_buff,ref Key_up,s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.approval = CharToInt(ref tt);
                string str ="("+ new string(tt)+")赞 (";
                s = FindCharArray(ref c_buff, ref Key_rep, s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.replay = CharToInt(ref tt);
                string rs = new string(tt);

                s = FindCharArray(ref c_buff, ref Key_js_userid, s);
                ci.u_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_poke, s);
                tt = FindCharArrayA(ref c_buff, ':', ',', ref s);
                ci.against = CharToInt(ref tt);
                str += new string(tt) + ")反对 (" + rs + ")回复";
                ci.count = str;
                s = FindCharArray(ref c_buff, ref Key_js_nick, s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.nick = GetString16(ref tt);
                s = FindCharArray(ref c_buff, ref Key_js_head, s);
                int t = s;
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref t);
                if(tt!=null)
                   ci.url = new string(DeleteChar(ref tt, '\\'));
                s = FindCharArray(ref c_buff, ref Key_js_vip, s);
                ci.vip = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_js_region, s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.region = GetString16(ref tt);
                lci.Add(ci);
            }
            return i;
        }
        public static int AnalyUpComment(ref char[] c_buff,List<UpCommentInfo> luc)
        {
            int s = 0, ss = luc.Count;
            UpCommentInfo ci = new UpCommentInfo();
            int i, rid = ss;
            for (i = 0; i < 10; i++)
            {
                s = FindCharArray(ref c_buff, ref Key_js_id, s);//id
                if (s < 0)
                    break;
                ci.m_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                int r = FindCharArray(ref c_buff, ref Key_js_rootid, s, s + 20);//rootid
                if (r > 0)
                {
                    ci.m_r_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref r));
                    ci.rid = rid;
                    s = r;
                }
                else { ci.m_r_id = null; rid = i + ss; };

                s = FindCharArray(ref c_buff, ref Key_js_userid, s);//userid
                ci.u_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));

                s = FindCharArray(ref c_buff, ref Key_up, s);//up
                char[] tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.approval = CharToInt(ref tt);
                string str = "(" + new string(tt) + ")赞 (";

                s = FindCharArray(ref c_buff, ref Key_poke, s);//poke
                tt = FindCharArrayA(ref c_buff, ':', ',', ref s);
                ci.against = CharToInt(ref tt);
                str += new string(tt) + ")反对 (";

                s = FindCharArray(ref c_buff, ref Key_rep, s);//rep
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.replay = CharToInt(ref tt);
                str += new string(tt) + ")回复";
                ci.count = str;

                s = FindCharArray(ref c_buff,ref Key_titleB,s);//title
                tt = FindCharArrayA(ref c_buff,'\"','\"',ref s);
                ci.title = GetString16A(ref tt);

                s = FindCharArray(ref c_buff,':',s);
                tt= FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                s = FindCharArray(ref c_buff, ref Key_content, s);//content
                ci.detail_s = new List<UpContent>();
                int o= GetUpCom_Content(ref c_buff,s,ci.detail_s);
                if (o > 0)
                    s = o;
                if(ci.detail_s.Count==0)
                {
                    UpContent u = new UpContent();
                    tt = GetCharArray16A(ref tt);
                    u.text = tt;
                    u.type = 't';
                    ci.detail_s.Add(u);
                }

                s = FindCharArray(ref c_buff, ref Key_js_timeD, s);//timeDifference
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.time = GetString16A(ref tt);
        
                s = FindCharArray(ref c_buff, ref Key_js_nick, s);//nick
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.nick = GetString16(ref tt);

                s = FindCharArray(ref c_buff, ref Key_js_head, s);//head
                int t = s;
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref t);
                if (tt != null)
                    ci.url = new string(DeleteChar(ref tt, '\\'));

                s = FindCharArray(ref c_buff, ref Key_js_region, s);//region
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.region = GetString16(ref tt);

                s = FindCharArray(ref c_buff, ref Key_js_vip, s);//viptype
                ci.vip = new string(FindCharArrayA(ref c_buff, ':', ',', ref s));

                s = FindCharArray(ref c_buff,ref Key_score,s);//score
                tt = FindCharArrayA(ref c_buff,':',',',ref s);
                ci.score = CharToInt(ref tt);

                luc.Add(ci);
            }
            return i;
        }
        static char[] Key_p_e = "/p>\"".ToCharArray();
        static char[] Key_data_width = "data-width".ToCharArray();
        static int GetUpCom_Content(ref char[] c_buff, int s ,List<UpContent> luc)
        {
            int e = FindCharArray(ref c_buff,ref Key_p_e,s);
            UpContent uc = new UpContent();
            for(int i=0;i<40;i++)
            {
                char[] tt = FindCharArrayA(ref c_buff,'>','<',ref s);
                if (s > e - 10)
                    return e;
                if(tt!=null)
                {
                    uc.text= GetCharArray16A(ref tt);
                    uc.type = 't';
                    uc.content = null;
                    luc.Add(uc);
                }
                s++;
                if(c_buff[s]=='i')//<img
                {
                    tt=FindCharArrayA(ref c_buff,'\"','\"',ref s);
                    tt = DeleteChar(ref tt,'\\');
                    if (tt[0] != 'h')
                        uc.content = "http:" + new string(tt);
                    else uc.content = new string(tt);
                    uc.type = 'i';
                    s= FindCharArray(ref c_buff, ref Key_data_width, s);
                    tt= FindCharArrayA(ref c_buff,'\"','\"',ref s);
                    uc.width = CharToInt(ref tt);
                    s++;
                    tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                    uc.height = CharToInt(ref tt);
                    luc.Add(uc);
                }
            }
            return e;
        }

        static char[] Key_mod_video_info = "mod_video_info".ToCharArray();
        static char[] Key_video_tit = "video_tit".ToCharArray();
        static char[] Key_video_types = "video_types".ToCharArray();
        static char[] Key_intro_line = "intro_line".ToCharArray();
        static char[] Key_mod_sideslip_episodes = "mod_sideslip_episodes".ToCharArray();
        static char[] Key_liA = "<li".ToCharArray();
        static char[] Key_i = "<i".ToCharArray();
        static char[] Key_video = "'$video'".ToCharArray();
        static char[] Key_videos = "'$videos'".ToCharArray();
        static char[] Key_clips = "'$clips'".ToCharArray();
        static char[] Key_m_e = ");".ToCharArray();
        public static ImageContext Des_PlayPage(char[] c_buff,List<FilterOption> lep,List<DX.BindingMod> lid)
        {
            c_buff = DeleteChar(ref c_buff,'\\');
            ImageContext ic = new ImageContext();
            int s = FindCharArray(ref c_buff,ref Key_mod_video_info,0);
            if (s < 0)
                return ic;
            int e = FindCharArray(ref c_buff,ref Key_section_e,s);
            int t = FindCharArray(ref c_buff,ref Key_video_tit,s);
            string str="";
            if(t>0)
            {
                ic.title = new string(FindCharArrayA(ref c_buff,'>','<',ref t ))+"\r\n";
                s = t;
            }
            t = FindCharArray(ref c_buff, ref Key_video_types, s);
            if (t > 0)
            {
                str += new string(FindCharArrayA(ref c_buff, '>', '<', ref t))+ "\r\n";
                s = t;
            }
            for(int i=0;i<3;i++)
            {
                t = FindCharArray(ref c_buff, ref Key_intro_line, s);
                if (t > 0)
                {
                    str += new string(FindCharArrayA(ref c_buff, '>', '<', ref t))+ "\r\n";
                    s = t;
                }
                else break;
            }
            ic.detail = str;
            t = FindCharArray(ref c_buff,ref Key_mod_sideslip_episodes,s);
            if(t>0)
            {
                FilterOption ep = new FilterOption();
                e = FindCharArray(ref c_buff, ref Key_section_e, t);
                int c = 1;
                while(t>0)
                {
                    t = FindCharArray(ref c_buff,ref Key_li,s,e);
                    if (t < 0)
                        break;
                    t = FindCharArray(ref c_buff,ref Key_vidA,t);
                    ep.Code = new string(FindCharArrayA(ref c_buff,'\"','\"',ref t));
                    int le = FindCharArray(ref c_buff,ref Key_a_e,t);
                    t = FindCharArray(ref c_buff,ref Key_i,t,le);
                    if (t > 0)
                        ep.Content =c.ToString()+ new string(FindCharArrayA(ref c_buff, '>', '<', ref t));
                    else ep.Content = c.ToString();
                    c++;
                    s = le;
                    lep.Add(ep);
                }
            }
            t = FindCharArray(ref c_buff,ref Key_video,s);
            if(t>0)
            {
                t = FindCharArray(ref c_buff,ref Key_vidB,t);
                ic.vid = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                t = FindCharArray(ref c_buff,ref Key_pic,t);
                t = FindCharArray(ref c_buff,':',t);
                ic.src = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                s = t;
            }
            t = FindCharArray(ref c_buff,ref Key_clips,s);
            if(t>0)
            {
                DataModA ida = new DataModA();
                e = FindCharArray(ref c_buff,ref Key_m_e,t);
                while (t > 0)
                {
                    t = FindCharArray(ref c_buff, ref Key_vidB, s, e);
                    if (t < 0)
                        break;
                    if (e - s < 200)
                        break;
                    ida.vid = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                    t = FindCharArray(ref c_buff, ref Key_titleA, t);
                    t++;
                    ida.Title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                    t = FindCharArray(ref c_buff, ref Key_pic, t);
                    t = FindCharArray(ref c_buff, ':', t);
                    ida.src = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                    int le = FindCharArray(ref c_buff, '}', t);
                    s = le;
                    le = FindCharArray(ref c_buff, ref Key_duration, t, s);
                    if (le > 0)
                    {
                        le++; //ida.href = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref le));
                    }
                    //else ida.href = null;
                    lid.Add(ida);
                }
            }
            return ic;
        }

        static void GetMark(ref char[] buff,int s,ref string[] tag)
        {
            int ec = FindCharArray(ref buff, ref Key_span_e, s);
            for (int i = 0; i < 3; i++)
            {
                s = FindCharArray(ref buff, '<', s);
                s++;
                char[] p = FindCharArrayA(ref buff, '\"', '\"', s, ec);
                if (p == null)
                    break;
                char[] c = FindCharArrayA(ref buff, '>', '<', s, ec);
                if (c != null)
                {
                    switch (p[2])
                    {
                        case 's'://mask
                            tag[2] = new string(c);
                            break;
                        case 'r'://mark
                            if (p[5] == 'i')
                                tag[1] = new string(c);
                            else tag[0] = new string(c);
                            break;
                    }
                }
            }
        }

        static char[] Key_posterPic = "posterPic\"".ToCharArray();
        static char[] Key_videoCategory = "videoCategory".ToCharArray();
        static char[] Key_publishDate = "publishDate".ToCharArray();
        static char[] Key_webPlayUrl = "webPlayUrl".ToCharArray();
        public static int Search_ex(char[] buff,List<DX.BindingMod> lia)
        {
            int s = 0 ;
            string detail;
            int i;
            for( i=0;i<15;i++)
            {
                DataMod im = new DataMod();
                s = FindCharArray(ref buff,ref Key_idA,s);
                if (s < 0)
                    break;
                detail = "";
                s = FindCharArray(ref buff,ref Key_posterPic,s);
                im.src = new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                detail +="播放量:" +new string(FindCharArrayA(ref buff,':',',',ref s));
                s = FindCharArray(ref buff,ref Key_duration,s);
                detail+="\r\n时长:"+ new string(FindCharArrayA(ref buff, ':', ',', ref s));
                s = FindCharArray(ref buff,ref Key_titleA,s);
                s++;
                char[] t = FindCharArrayA(ref buff, '\"', '\"', ref s);
                im.Title = GetString16A(ref t);
                s = FindCharArray(ref buff,ref Key_videoCategory,s);
                s++;
                detail+="S\r\n分类:"+ new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                s = FindCharArray(ref buff ,':',s);
                t = FindCharArrayA(ref buff, '\"', '\"', ref s);
                if(t!=null)
                   detail += "\r\n区域:" + GetString16A(ref t);
                s = FindCharArray(ref buff, ':', s);
                detail += "\r\n" + new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                s = FindCharArray(ref buff,ref Key_webPlayUrl,s);
                s++;
                im.href = new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                s = FindCharArray(ref buff,ref Key_publishDate,s);
                s++;
                detail += "\r\n" + new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                im.Detail = detail;
                lia.Add(im);
            }
            return i;
        }
    }
}
