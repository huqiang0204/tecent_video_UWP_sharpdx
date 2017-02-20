using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using DX;
namespace TVWP.Class
{
    class CharOperation
    {
        #region function
        public static char[] CharInsertSpace(ref char[] source, int row,int count)
        {
            int c = source.Length;
            int l= c + row * count;
            char[] buff = new char[l];
            int t = 0;
            int s = 0;
            int r = 0;
            for(int i=0;i<c;i++)
            {
                if(r<count)
                {
                    if(t<row)
                    {
                        for(r=0;r<count;r++)
                        {
                            buff[s] = ' ';
                            s++;
                        }
                        t++;
                    }
                }
                if(source[i]=='\r')
                {
                    buff[s] = '\r';
                    s++;
                    buff[s] = '\n';
                    i ++;
                    r = 0;
                }else
                {
                    buff[s] = source[i];
                    s++;
                }
            }
            return buff;
        }
        public static char[] CharInsert(ref char[] source, char[] target, int index)
        {
            int x = source.Length;
            int y = target.Length;
            int l = x + y;
            char[] temp = new char[l];
            for (int i = 0; i < index; i++)
                temp[i] = source[i];
            int s = index;
            for (int i = 0; i < y; i++)
            {
                temp[s] = target[i];
                s++;
            }
            for (int i = s; i < l; i++)
            {
                temp[i] = source[index];
                index++;
            }
            return temp;
        }
        public static int FindCharArray(ref char[] source, ref char[] content, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (content[0] == source[i])
                {
                    int t = i;
                    t++;
                    if (t >= source.Length)
                        return -1;
                    for (int c = 1; c < content.Length; c++)
                    {
                        if (content[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            return -1;
                    }
                    return t;
                }
                label1:;
            }
            return -1;
        }
        public static int FindCharArray(ref char[] source, ref char[] content, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                if (content[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < content.Length; c++)
                    {
                        if (content[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= end)
                            return -1;
                    }
                    return t;
                }
                label1:;
            }
            return -1;
        }
        public static int FindCharArray(ref char[] source, ref char[] content, int start, char end)
        {
            for (int i = start; i < end; i++)
            {
                if (source[i] == end)
                    return -1;
                if (content[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < content.Length; c++)
                    {
                        if (source[t] == end)
                            return -1;
                        if (content[c] != source[t])
                            goto label1;
                        t++;
                    }
                    return t;
                }
                label1:;
            }
            return -1;
        }
        public static char[] FindCharArray(ref char[] source, ref char[] sc, ref char[] ec, int index)
        {
            return FindCharArrayA(ref source, ref sc, ref ec, ref index);
        }
        public static int FindCharArray(ref char[] source, char c1, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (source[i] == c1)
                    return i;
            }
            return -1;
        }
        public static int FindCharArray(ref char[] source, char c1, char c2, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (source[i] == c1 | source[i] == c2)
                    return i;
            }
            return -1;
        }
        public static char[] FindCharArrayA(ref char[] source, char sc, char ec, ref int index)
        {
            //if (index < 0)
            //    index = 0;
            char temp = sc;
            bool o = false;
            int s = 0;
            for (int i = index; i < source.Length; i++)
            {
                if (temp == source[i])
                {
                    if (o)
                    {
                        int l = i - s;
                        if (l <= 0)
                        { index = i; return null; }
                        char[] cc = new char[l];
                        for (int c = 0; c < l; c++)
                        { cc[c] = source[s]; s++; }
                        index = i;
                        return cc;
                    }
                    else { o = true; s = i; s++; temp = ec; }
                }
            }
            return null;
        }
        public static char[] FindCharArrayA(ref char[] source, char sc, char ec, int start, int end)
        {
            //if (start < 0)
            //    start = 0;
            char temp = sc;
            bool o = false;
            int s = 0;
            for (int i = start; i < end; i++)
            {
                if (temp == source[i])
                {
                    if (o)
                    {
                        int l = i - s;
                        if (l <= 0)
                            return null;
                        char[] cc = new char[l];
                        for (int c = 0; c < l; c++)
                        { cc[c] = source[s]; s++; }
                        return cc;
                    }
                    else { o = true; s = i; s++; temp = ec; }
                }
            }
            return null;
        }
        public static char[] FindCharArrayA(ref char[] source, ref char[] sc, ref char[] ec, ref int index)
        {
            if (index < 0)
                index = 0;
            char[] temp = sc;
            bool o = false;
            int s = 0;
            for (int i = index; i < source.Length; i++)
            {
                if (temp[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < temp.Length; c++)
                    {
                        if (temp[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            return null;
                    }
                    if (o)
                    {
                        int l = t - s - 1;
                        if (l <= 0)
                            return null;
                        temp = new char[l];
                        for (int c = 0; c < l; c++)
                        { temp[c] = source[s]; s++; }
                        index = t;
                        return temp;
                    }
                    else { o = true; s = t; i = t; temp = ec; }
                }
                label1:;
            }
            return null;
        }
        public static char[] CopyCharArry(ref char[] source, int index, int count)
        {
            char[] temp = new char[count];
            for (int i = 0; i < count; i++)
            {
                temp[i] = source[index];
                index++;
            }
            return temp;
        }
        public static int FallFindCharArray(ref char[] source, char sc, int index)
        {
            for (int i = index; i > 0; i--)
            {
                if (source[i] == sc)
                    return i;
            }
            return -1;
        }
        public static int FallFindCharArray(ref char[] source, ref char[] sc, int index)
        {
            for (int i = index; i > 0; i--)
            {
                if (sc[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < sc.Length; c++)
                    {
                        if (sc[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            goto label1;
                    }
                    return i;
                }
                label1:;
            }
            return -1;
        }
        public static char[] FallFindCharArray(ref char[] source, char sc, char ec, ref int index)
        {
            bool o = false;
            int end = 0;
            for (int i = index; i > 0; i--)
            {
                if (source[i] == ec)
                {
                    if (o)
                    {
                        i++;
                        int l = end - i;
                        char[] temp = new char[l];
                        for (int c = 0; c < l; c++)
                        {
                            temp[c] = source[i];
                            i++;
                        }
                        return temp;
                    }
                    else
                    {
                        end = i; o = true; ec = sc;
                    }
                }
            }
            return null;
        }
        public static char[] FallFindCharArray(ref char[] source, ref char[] sc, ref char[] ec, int index)
        {
            return FallFindCharArrayA(ref source, ref sc, ref ec, ref index);
        }
        public static char[] FallFindCharArrayA(ref char[] source, ref char[] sc, ref char[] ec, ref int index)
        {
            char[] temp = sc;
            bool o = false;
            int s = 0;
            for (int i = index; i > 0; i--)
            {
                if (temp[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < temp.Length; c++)
                    {
                        if (temp[c] != source[t])
                            goto label1;
                        t++;
                        if (i >= source.Length)
                            goto label1;
                    }
                    if (o)
                    {
                        int l = t - s - 1;
                        temp = new char[l];
                        for (int c = 0; c < l; c++)
                        { temp[c] = source[s]; s++; }
                        index = i;
                        return temp;
                    }
                    else { o = true; s = t; temp = ec; }
                }
                label1:;
            }
            return null;
        }
        public static string GetString16(ref char[] source)
        {
            if (source == null)
                return "";
            int len = source.Length;
            len /= 6;
            char[] temp = new char[len];
            int t = 2;
            for (int i = 0; i < len; i++)
            {
                int c = (int)source[t];
                if (c > 58)
                    c -= 87;
                else
                    c &= 15;
                c <<= 12;
                t++;
                int d = (int)source[t];
                if (d > 58)
                    d -= 87;
                else
                    d &= 15;
                d <<= 8;
                c |= d;
                t++;
                d = (int)source[t];
                if (d > 58)
                    d -= 87;
                else
                    d &= 15;
                d <<= 4;
                c |= d;
                t++;
                d = (int)source[t];
                if (d > 58)
                    d -= 87;
                else
                    d &= 15;
                c |= d;
                t++;
                t += 2;
                temp[i] = (char)c;
            }
            return new string(temp);
        }
        public static string GetString16A(ref char[] source)
        {
            int len = source.Length;
            char[] temp = new char[len];
            int t = 0, s = 0;
            while (t < len)
            {
                if (source[t] == '\\')
                    t++;
                if (source[t] == 'u')
                {
                    t++;
                    int c = (int)source[t];
                    if (c > 58)
                        c -= 87;
                    else
                        c &= 15;
                    c <<= 12;
                    t++;
                    int d = (int)source[t];
                    if (d > 58)
                        d -= 87;
                    else
                        d &= 15;
                    d <<= 8;
                    c |= d;
                    t++;
                    d = (int)source[t];
                    if (d > 58)
                        d -= 87;
                    else
                        d &= 15;
                    d <<= 4;
                    c |= d;
                    t++;
                    d = (int)source[t];
                    if (d > 58)
                        d -= 87;
                    else
                        d &= 15;
                    c |= d;
                    t ++;
                    temp[s] = (char)c;
                    s++;
                }
                else
                {
                    label0:;
                    temp[s] = source[t];
                    s++;
                    t++;
                    if (t >= len)
                        break;
                    if (source[t] != '\\')
                        goto label0;
                    t++;
                }
            }
            char[] temp2 = new char[s];
            for (int i = 0; i < s; i++)
                temp2[i] = temp[i];
            return new string(temp2);
        }
        public static char[] DeleteChar(ref char[] source, char c)
        {
            int len = source.Length;
            char[] temp = new char[len];
            int s = 0;
            for (int i = 0; i < len; i++)
            {
                if (source[i] != c)
                {
                    temp[s] = source[i];
                    s++;
                }
            }
            char[] temp2 = new char[s];
            for (int i = 0; i < s; i++)
                temp2[i] = temp[i];
            return temp2;
        }
        public static char[] DeleteChar(ref char[] source, params char[] c)
        {
            int len = source.Length;
            int l = c.Length;
            char[] temp = new char[len];
            int s = 0;
            for (int i = 0; i < len; i++)
            {
                for (int t = 0; t < l; t++)
                {
                    if (source[i] == c[t])
                        goto label1;
                }
                temp[s] = source[i];
                s++;
                label1:;
            }
            char[] temp2 = new char[s];
            for (int i = 0; i < s; i++)
                temp2[i] = temp[i];
            return temp2;
        }
        public static int CharToInt(ref char[] source)
        {
            int c;
            if (source.Length < 10)
                c = source.Length;
            else c = 10;
            int r = 0;
            for (int i = 0; i < c; i++)
            {
                int t = source[i];
                if (t > 57 || t < 48)
                    return r;
                t -= 48;
                r *= 10;
                r += t;
            }
            return r;
        }
        public static int FindCharCount(ref char[] source, char sc, int s)
        {
            int count = 0;
            for (int i = s; i < source.Length; i++)
            {
                if (source[i] == sc)
                    count++;
            }
            return count;
        }
        public static int FindCharArrayCount(ref char[] source, ref char[] sc, int s)
        {
            int count = 0;
            for (int i = s; i < source.Length; i++)
            {
                if (source[i] == sc[0])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < sc.Length; c++)
                    {
                        if (sc[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            return count;
                    }
                    count++;
                }
                label1:;
            }
            return count;
        }
        public static int FindCharArrayCount(ref char[] source, ref char[] sc, int s, int e)
        {
            int count = 0;
            for (int i = s; i < e; i++)
            {
                if (source[i] == sc[0])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < sc.Length; c++)
                    {
                        if (sc[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= e)
                            return count;
                    }
                    count++;
                }
                label1:;
            }
            return count;
        }
        public static char[] CharWarp(char[] source,int count,out int row)
        {
            int len = source.Length;
            char[] buff = new char[len+256];
            int max = 0;
            int c = 0;
            row = 0;
            for(int i=0;i<len;i++)
            {
                if (source[i] == '\r')
                {
                    c = 0;
                    buff[max] = '\r';
                    max++;
                    buff[max] = '\n';
                    max++;
                    i += 2;
                    row++;
                }
                else
                {
                    buff[max] = source[i];
                    max++;
                    c++;
                    if(c>=count)
                    {
                        c = 0;
                        buff[max] = '\r';
                        max++;
                        buff[max] = '\n';
                        max++;
                        row++;
                    }
                }
            }
            char[] temp = new char[max];
            for (int i = 0; i < max; i++)
                temp[i] = buff[i];
            return temp;
        }
        public static char[] CharWarp(char[] source,int count,int row,int count2,out int allrow)
        {
            allrow = 0;
            int len = source.Length;
            char[] buff = new char[len + 256];
            int max = 0;
            int c = 0;
            int c2 = count;
            for (int i = 0; i < len; i++)
            {
                if (source[i] == '\r')
                {
                    c = 0;
                    buff[max] = '\r';
                    max++;
                    buff[max] = '\n';
                    max++;
                    i += 2;
                    allrow++;
                    if (allrow == row)
                        c2 = count2;
                }
                else
                {
                    buff[max] = source[i];
                    max++;
                    c++;
                    if (c >= c2)
                    {
                        c = 0;
                        buff[max] = '\r';
                        max++;
                        buff[max] = '\n';
                        max++;
                        allrow++;
                        if (allrow == row)
                            c2 = count2;
                    }
                }
            }
            char[] temp = new char[max];
            for (int i = 0; i < max; i++)
                temp[i] = buff[i];
            return temp;
        }
        public static char[] GetCharArray16A(ref char[] source)
        {
            int len = source.Length;
            char[] temp = new char[len];
            int t = 0, s = 0;
            while (t < len)
            {
                if (source[t] == '\\')
                    t++;
                if (source[t] == 'u')
                {
                    t++;
                    int c = (int)source[t];
                    if (c > 58)
                        c -= 87;
                    else
                        c &= 15;
                    c <<= 12;
                    t++;
                    int d = (int)source[t];
                    if (d > 58)
                        d -= 87;
                    else
                        d &= 15;
                    d <<= 8;
                    c |= d;
                    t++;
                    d = (int)source[t];
                    if (d > 58)
                        d -= 87;
                    else
                        d &= 15;
                    d <<= 4;
                    c |= d;
                    t++;
                    d = (int)source[t];
                    if (d > 58)
                        d -= 87;
                    else
                        d &= 15;
                    c |= d;
                    t += 2;
                    temp[s] = (char)c;
                    s++;
                }
                else
                {
                    label0:;
                    temp[s] = source[t];
                    s++;
                    t++;
                    if (t >= len)
                        break;
                    if (source[t] != '\\')
                        goto label0;
                    t++;
                }
            }
            char[] temp2 = new char[s];
            for (int i = 0; i < s; i++)
                temp2[i] = temp[i];
            return temp2;
        }
        #endregion

        #region keyword
        public static char[] Key_interactionCount = "interactionCount".ToCharArray();
        public static char[] Key_datePublished = "datePublished".ToCharArray();
        public static char[] Key_varietyDate = "varietyDate".ToCharArray();
        public static char[] Key_title = "title=".ToCharArray();
        public static char[] Key_titleA = "title".ToCharArray();
        public static char[] Key_titleB = "\"title\"".ToCharArray();
        public static char[] Key_pic = "pic".ToCharArray();
        public static char[] Key_less = "<".ToCharArray();
        public static char[] Key_list_item = "list_item".ToCharArray();
        public static char[] Key_list_itemA = "\"list_item\"".ToCharArray();
        public static char[] Key_href = "href=".ToCharArray();
        public static char[] Key_quote = "\"".ToCharArray();
        public static char[] Key_src = "src=".ToCharArray();
        public static char[] Key_http = "http".ToCharArray();
        public static char[] Key_content = "content".ToCharArray();
        public static char[] Key_count = "Count".ToCharArray();
        public static char[] Key_data = "data".ToCharArray();
        public static char[] Key_date = "date".ToCharArray();
        public static char[] Key_img = "image".ToCharArray();
        public static char[] Key_x = ".com/x/".ToCharArray();
        public static char[] Key_com = ".com".ToCharArray();
        public static char[] Key_refresh = "refresh".ToCharArray();
        public static char[] Key_url = "url=".ToCharArray();
        public static char[] Key_urlA = "\"url\"".ToCharArray();
        public static char[] Key_quote_s = "'".ToCharArray();
        public static char[] Key_curvid = "curVid ==".ToCharArray();
        public static char[] Key_id = "id".ToCharArray();
        public static char[] Key_idA = "\"id\"".ToCharArray();
        public static char[] Key_left_brace = "(".ToCharArray();
        public static char[] Key_right_brace = ")".ToCharArray();
        public static char[] Key_P = "P)".ToCharArray();
        public static char[] Key_fc = "\"fc\"".ToCharArray();
        public static char[] Key_fn = "\"fn\"".ToCharArray();
        public static char[] Key_key = "<key>".ToCharArray();
        public static char[] Key_fvkey = "\"fvkey\"".ToCharArray();
        public static char[] Key_fmt = "<fmt>".ToCharArray();
        public static char[] Key_name = "\"name\"".ToCharArray();
        public static char[] Key_filename = "<filename>".ToCharArray();
        public static char[] Key_dtc = "<dtc>".ToCharArray();
        public static char[] Key_coverinfo = "COVER_INFO".ToCharArray();
        public static char[] Key_videoinfo = "VIDEO_INFO".ToCharArray();
        public static char[] Key_listinfo = "LIST_INFO".ToCharArray();
        public static char[] Key_listinfoE = "}}}".ToCharArray();
        public static char[] Key_vid = "vid:".ToCharArray();
        public static char[] Key_vidA = "vid=".ToCharArray();
        public static char[] Key_vidB = "vid\"".ToCharArray();
        public static char[] Key_slash = "//".ToCharArray();
        public static char[] Key_about = "about:".ToCharArray();
        public static char[] Key_comment_id = "comment_id\":".ToCharArray();
        public static char[] Key_return = "retnum\"".ToCharArray();
        public static char[] Key_fristid = "first\"".ToCharArray();
        public static char[] Key_lastid = "last\"".ToCharArray();
        public static char[] Key_js_id = "\"id\"".ToCharArray();
        public static char[] Key_js_rootid = "rootid\"".ToCharArray();
        public static char[] Key_js_content = "content\"".ToCharArray();
        public static char[] Key_js_time = "time\"".ToCharArray();
        public static char[] Key_js_timeD = "timeDifference\"".ToCharArray();
        public static char[] Key_js_userid = "userid\"".ToCharArray();
        public static char[] Key_js_useridA = "userid".ToCharArray();
        public static char[] Key_js_nick = "nick\"".ToCharArray();
        public static char[] Key_js_head = "head\"".ToCharArray();
        public static char[] Key_js_vip = "viptype\"".ToCharArray();
        public static char[] Key_js_region = "region\"".ToCharArray();
        public static char[] Key_equal = "=".ToCharArray();
        public static char[] Key_and = "&".ToCharArray();
        #endregion

        #region Keyword
        public static char[] Key_duration = "duration".ToCharArray();
        public static char[] Key_tl = "tl=".ToCharArray();
        public static char[] Key_description = "description".ToCharArray();
        public static char[] Key_mod_episode = "mod_episode".ToCharArray();
        public static char[] Key_mod_playlist = "mod_playlist".ToCharArray();
        public static char[] Key_result_item = "result_item".ToCharArray();
        public static char[] Key_player_figure = "player_figure".ToCharArray();
        public static char[] Key_playlist = "_playlist".ToCharArray();
        public static char[] Key_figures_list = "figures_list".ToCharArray();
        public static char[] Key_alt = "alt=".ToCharArray();
        public static char[] Key_mod_box_series = "mod_box_series".ToCharArray();
        public static char[] Key_mod_box_stage = "mod_box_stage".ToCharArray();
        public static char[] Key_mod_video_list = "mod_video_list".ToCharArray();
        public static char[] Key_mod_item = "mod_item\"".ToCharArray();
        public static char[] Key_a = "<a".ToCharArray();
        public static char[] Key_a_e = "</a".ToCharArray();
        public static char[] Key_em = "<em".ToCharArray();
        public static char[] Key_desc_text = "desc_text".ToCharArray();
        public static char[] Key_replace = "replace".ToCharArray();
        public static char[] Key_replaceA = "url.replace".ToCharArray();
        public static char[] Key_keyid = "keyid".ToCharArray();
        public static char[] Key_fs = "\"fs\"".ToCharArray();
        public static char[] Key_td = "\"td\"".ToCharArray();
        public static char[] Key_ti = "\"ti\"".ToCharArray();
        public static char[] Key_cmd5 = "\"cmd5\"".ToCharArray();
        public static char[] Key_info_inner = "info_inner".ToCharArray();
        public static char[] Key_list_item_hover = "list_item_hover".ToCharArray();
        public static char[] Key_figure_desc = "figure_desc".ToCharArray();
        public static char[] Key_lazyload = "lazyload=".ToCharArray();
        public static char[] Key_mod_filter_box = "mod_filter_box".ToCharArray();
        public static char[] Key_filter_content = "filter_content".ToCharArray();
        public static char[] Key_label = "label".ToCharArray();
        public static char[] Key_item_toggle = "item_toggle".ToCharArray();
        public static char[] Key_boss = "_boss".ToCharArray();
        public static char[] Key_index = "index".ToCharArray();
        public static char[] Key_type = "type".ToCharArray();
        public static char[] Key_li = "<li>".ToCharArray();
        public static char[] Key_qq = "qq.com".ToCharArray();
        public static char[] Key_preview = "preview\"".ToCharArray();
        public static char[] Key_ul_e = "</ul>".ToCharArray();
        public static char[] Key_split = "split".ToCharArray();
        public static char[] Key_site_container = "site_container".ToCharArray();
        public static char[] Key_c_over = "<!--".ToCharArray();
        #endregion

        #region Keyword
        public static char[] Key_weekline_title = "weekline_title".ToCharArray();
        public static char[] Key_curPlaysrc = "curPlaysrc".ToCharArray();
        public static char[] Key_datavid = "data-vid".ToCharArray();
        public static char[] Key_dispatch = "dispatch".ToCharArray();
        public static char[] Key_span = "<span".ToCharArray();
        public static char[] Key_span_e = "</span".ToCharArray();
        public static char[] Key_div = "<div".ToCharArray();
        public static char[] Key_div_e = "</div>".ToCharArray();
        public static char[] Key_p = "<p".ToCharArray();
        public static char[] Key_section = "<section".ToCharArray();
        public static char[] Key_section_e = "</section".ToCharArray();
        public static char[] Key_h2 = "<h2".ToCharArray();
        public static char[] Key_h2_e = "</h2".ToCharArray();
        public static char[] Key_mask = "mask_txt".ToCharArray();
        public static char[] Key_mark = "mark_v".ToCharArray();
        public static char[] Key_markA = "mark_".ToCharArray();
        public static char[] Key_leaf_id = "leaf_id\"".ToCharArray();
        public static char[] Key_now = "now\"".ToCharArray();
        #endregion
    }
    class Component
    {
        #region  global set
        protected const double minX = 140;
        public const int presstime = 1800000;
        public const double pressoffset = 30;
        public static double PixRratio { get; set; }
        public static int language { get; set; }
        public static float screenX, screenY;
        protected static double X { get; set; }
        protected static double Y { get; set; }
        protected static double OffsetX { get; set; }
        protected static double OffsetY { get; set; }
        #endregion
    }
}