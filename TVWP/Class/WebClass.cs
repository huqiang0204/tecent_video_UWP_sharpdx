using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Networking.Connectivity;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace TVWP.Class
{
    class WebClass:Component
    {
        #region main
        static HttpClient hc;

        public static void Initial()
        {
            //if ( NetworkInformation.GetInternetConnectionProfile().IsWlanConnectionProfile)
            //{
            //    //wifi
            //}
            hc = new HttpClient();
            hc.DefaultRequestHeaders.UserAgent.Add(new Windows.Web.Http.Headers.HttpProductInfoHeaderValue(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36"));
            //TaskGet("http://v.qq.com/sports/",(s)=> { Debug.WriteLine(s); });
            //TaskGet("http://data.video.qq.com/fcgi-bin/data?tid=1&idlist=6y3sjdvehyjk59p", (s) => { Debug.WriteLine(s); }, "http://v.qq.com/sports/");
            //TaskGet("http://m.v.qq.com/index.html", (s) => { Scroll_m.Test(s.ToCharArray()); });
            //SetCookie();
            
        }
        public static async Task<string> GetResults(string url,string refer)
        {
            //url += "&otype=json";
            hc.DefaultRequestHeaders.Referer = new Uri(refer);
            IBuffer ib = await hc.GetBufferAsync(new Uri(url));
            var dr = DataReader.FromBuffer(ib);
            byte[] buff = new byte[ib.Length];
            dr.ReadBytes(buff);
            return Encoding.UTF8.GetString(buff);
        }
        public static async Task<string> Post(string url,string content)
        {
            var cc=  await hc.PostAsync(new Uri( url), new HttpStringContent(content, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));//"application/x-www-form-urlencoded"
            return await  cc.Content.ReadAsStringAsync();
        }
        public static async Task<string> Post(string url, string content,string refer)
        {
            hc.DefaultRequestHeaders.Referer = new Uri(refer);
            var cc = await hc.PostAsync(new Uri(url), new HttpStringContent(content, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));//"application/x-www-form-urlencoded"
            return await cc.Content.ReadAsStringAsync();
        }
        #endregion

        #region ex
        public static async void TaskGet(string url,Action<string> t)
        {
            byte[] buff= { };
            try
            {
                IBuffer ib = await hc.GetBufferAsync(new Uri(url));
                var dr = DataReader.FromBuffer(ib);
                buff = new byte[ib.Length];
                dr.ReadBytes(buff);
#if !DEBUG
                t(Encoding.UTF8.GetString(buff));
#endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
#if DEBUG
            t(Encoding.UTF8.GetString(buff));
#endif
        }
        public static async void TaskGet(string url, Action<string,int> t,int tag)
        {
            byte[] buff = { };
            try
            {
                IBuffer ib = await hc.GetBufferAsync(new Uri(url));
                var dr = DataReader.FromBuffer(ib);
                buff = new byte[ib.Length];
                dr.ReadBytes(buff);
#if !DEBUG
                t(Encoding.UTF8.GetString(buff),tag);
#endif
            }
            catch (Exception ex)
            {
                throw (ex);
            }
#if DEBUG
            t(Encoding.UTF8.GetString(buff),tag);
#endif
        }
        public static async void TaskGet(string url, Action<string> t, string refer)
        {
            byte[] buff = { };
            try
            {
                hc.DefaultRequestHeaders.Referer = new Uri(refer);
                IBuffer ib = await hc.GetBufferAsync(new Uri(url));
                var dr = DataReader.FromBuffer(ib);
                buff = new byte[ib.Length];
                dr.ReadBytes(buff);
#if !DEBUG
                t(Encoding.UTF8.GetString(buff));
#endif
            }
            catch (Exception ex)
            {
                throw (ex);
            }
#if DEBUG
            t(Encoding.UTF8.GetString(buff));
#endif
        }
        public static async void TaskPost(string url, Action<string> t, string content)
        {
            try
            {
                var cc = await hc.PostAsync(new Uri(url), new HttpStringContent(content,
                    Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));
                t(await cc.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static async void TaskPost(string url,Action<string> t,string content,string refer)
        {
            try
            {
                hc.DefaultRequestHeaders.Referer = new Uri(refer);
                var cc = await hc.PostAsync(new Uri(url), new HttpStringContent(content,
                    Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));
                t( await cc.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static void SetCookie()
        {
            HttpBaseProtocolFilter hb = new HttpBaseProtocolFilter();
            HttpCookieCollection t = hb.CookieManager.GetCookies(new Uri("http://v.qq.com/x/movielist/"));
            string c = "";
            int a = t.Count;
            for (int i = 0; i < a; i++)
            {
                HttpCookie item = t[i];
                if (i == a - 1)
                    c += item.Name + "=" + item.Value;
                else c += item.Name + "=" + item.Value + ";";
            }
            if (c == "")
                return;
            hc.DefaultRequestHeaders.Remove("Cookie");
            hc.DefaultRequestHeaders.Add("Cookie", c);
        }
        public static bool GetLogin()
        {
            HttpBaseProtocolFilter hb = new HttpBaseProtocolFilter();
            HttpCookieCollection t = hb.CookieManager.GetCookies(new Uri("http://v.qq.com/x/movielist/"));
            string c = "";
            int a = t.Count;
            for (int i = 0; i < a; i++)
            {
                HttpCookie item = t[i];
                if (i == a - 1)
                    c += item.Name + "=" + item.Value;
                else c += item.Name + "=" + item.Value + ";";
                if (item.Name == "encuin")
                    return true;
            }
            return false;
        }
        public static async void TaskGetA(string url, Action<string> t)
        {
            byte[] buff = { };
            try
            {
                IBuffer ib = await hc.GetBufferAsync(new Uri(url));
                
                var dr = DataReader.FromBuffer(ib);
                buff = new byte[ib.Length];
                dr.ReadBytes(buff);
#if !DEBUG
                t(Encoding.UTF8.GetString(buff));
#endif
            }
            catch (Exception ex)
            {
                throw (ex);
            }
#if DEBUG
            t(Encoding.UTF8.GetString(buff));
#endif
        }
        #endregion

        public static async Task<Stream> Get(string url)
        {
            try
            {
                IBuffer ib = await hc.GetBufferAsync(new Uri(url));
                return WindowsRuntimeBufferExtensions.AsStream(ib);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}