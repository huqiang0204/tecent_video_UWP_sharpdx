using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TVWP.Class
{
    class NetClass
    {
        static HttpClient hc;
        static HttpRequestMessage hrm;
        public static async void TaskGet(string url,Action<string> callback)
        {
            if (hc == null)
                hc = new HttpClient();
            try
            {
                HttpRequestMessage hrm = new HttpRequestMessage();
                hrm.Method = HttpMethod.Get;
                hrm.RequestUri = new Uri(url);
                int t = DateTime.Now.Millisecond;
                HttpResponseMessage o = await hc.SendAsync(hrm);
                //Debug.WriteLine(t);
                string str;
                if (o.StatusCode == HttpStatusCode.NotFound)
                    return;
                if (o.StatusCode == HttpStatusCode.Forbidden)
                {
                    o = await hc.SendAsync(hrm);
                }
                str = await o.Content.ReadAsStringAsync();
                callback(str);
            }catch (Exception ex)
            {
                throw (ex);
            }
            
        }
        public static async Task<Stream> Get(string url)
        {
            if (hc == null)
                hc = new HttpClient();
            try
            {
                if(hrm==null)
                   hrm = new HttpRequestMessage();
                hrm.Method = HttpMethod.Get;
                hrm.RequestUri = new Uri(url);
                int t = DateTime.Now.Millisecond;
                HttpResponseMessage o = await hc.SendAsync(hrm);
                if (o.StatusCode == HttpStatusCode.NotFound)
                    return null;
                if (o.StatusCode == HttpStatusCode.Forbidden)
                {
                    o = await hc.SendAsync(hrm);
                }
                return await o.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public static async void TaskGet(string url, Action<Stream> callback)
        {
            if (hc == null)
                hc = new HttpClient();
            try
            {
                HttpRequestMessage hrm = new HttpRequestMessage();
                hrm.Method = HttpMethod.Get;
                hrm.RequestUri = new Uri(url);
                int t = DateTime.Now.Millisecond;
                HttpResponseMessage o = await hc.SendAsync(hrm);
                //Debug.WriteLine(t);
                Stream str;
                if (o.StatusCode == HttpStatusCode.NotFound)
                    return;
                if (o.StatusCode == HttpStatusCode.Forbidden)
                {
                    o = await hc.SendAsync(hrm);
                }
                str = await o.Content.ReadAsStreamAsync();
                callback(str);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public static async void TaskGet(string url, Action<string> callback,string refer)
        {
            if (hc == null)
                hc = new HttpClient();
            try
            {
                HttpRequestMessage hrm = new HttpRequestMessage();
                hc.DefaultRequestHeaders.Referrer = new Uri(refer);
                hrm.Method = HttpMethod.Get;
                hrm.RequestUri = new Uri(url);
                int t = DateTime.Now.Millisecond;
                HttpResponseMessage o = await hc.SendAsync(hrm);
                //Debug.WriteLine(t);
                string str;
                if (o.StatusCode == HttpStatusCode.NotFound)
                    return;
                if (o.StatusCode == HttpStatusCode.Forbidden)
                {
                    o = await hc.SendAsync(hrm);
                }
                byte[] b = await o.Content.ReadAsByteArrayAsync();
                str = Encoding.UTF8.GetString(b);
                callback(str);
            }catch (Exception ex)
            {
                throw (ex);
            }
            
        }
        //public static async Task<byte[]> GetA(string url)
        //{
        //    Uri u = new Uri(url);
        //    if (hc == null)
        //        hc = new HttpClient();
        //    HttpRequestMessage hrm = new HttpRequestMessage();
        //    hrm.Method = HttpMethod.Get;
        //    hrm.RequestUri = u;
        //    HttpResponseMessage o = await hc.SendAsync(hrm);
        //    byte[] b= await o.Content.ReadAsByteArrayAsync();
        //    if (b.Length> 0)
        //        return b;
        //    string path= hrm.RequestUri.LocalPath;
        //    path=  WebClass.GetResults(path);
        //    url += "&guid=" + path;
        //    hrm.RequestUri = new Uri(url);
        //    o = await hc.SendAsync(hrm);
        //    b = await o.Content.ReadAsByteArrayAsync();
        //    return b;
        //    //return await hc.GetByteArrayAsync(hrm.RequestUri);
        //}
        public static async Task<string> Post(string url,string content, string refer)
        {
            if (hc == null)
                hc = new HttpClient();
            hc.DefaultRequestHeaders.Referrer =new Uri(refer);
            StringContent sc= new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage hrm= await hc.PostAsync(url,sc);
            return await hrm.Content.ReadAsStringAsync();
        }
    }
}
