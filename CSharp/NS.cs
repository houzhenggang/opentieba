using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace Opentieba
{
    /// <summary>
    /// JSON
    /// </summary>
    public static class JSON
    {
        /// <summary>
        /// 解析JSON
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns>JObject对象</returns>
        public static JObject parse(String json) {
            return JsonConvert.DeserializeObject<JObject>(json);
        }
    }
    /// <summary>
    /// opentieba核心类
    /// </summary>
    static class _
    {
        /// <summary>
        /// [内部调用]是否开启调试模式。
        /// </summary>
        public const bool __debug__=false;
        /// <summary>
        /// [内部调用]发包
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="post">post</param>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static String sendHttp(String url, String post, String cookie)
        {
            WebClient wclient = new WebClient();
            wclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            if (cookie.Length > 0) {
                wclient.Headers.Add("Cookie", cookie);
            }
            try
            {
                return wclient.UploadString(url, "POST", post);
            }
            catch (WebException)
            {
                throw;
            }
        }
        /// <summary>
        /// [内部调用]编码URL
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static String encodeURIComponent(String st)
        {
            return HttpUtility.UrlEncode(st);
        }
        /// <summary>
        /// [内部调用]编码base64
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>base64</returns>
        public static String base64enc(String str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
    }
    class ASCIIComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            byte[] a_ = Encoding.ASCII.GetBytes(a);
            byte[] b_ = Encoding.ASCII.GetBytes(b);
            if (a_.Length < b_.Length) Array.Resize<byte>(ref a_, b_.Length);
            if (b_.Length < a_.Length) Array.Resize<byte>(ref b_, a_.Length);
            for (int i = 0; i < a_.Length; ++i)
            {
                if (a_[i] < b_[i]) return -1;
                if (a_[i] > b_[i]) return 1;
            }
            return 0;
        }
    }
    public static class _stbapi {
        public static readonly String versions = "dab421ab4377f5c7a8cccc320c7851021f5049d8";
        public static String sendTieba(String fpath, String post, String bduss) {
            String pbdata="BDUSS="+_.encodeURIComponent(bduss)+"&_client_id=a0112ba8-b146-45c5-bb18-5b9fdde4917b&_client_type=4&_client_version=1.3.3&_phone_imei="+
                "05-00-54-20-06-00-01-00-04-00-9C-35-01-00-26-28-02-00-24-14-09-00-32-53&net_type=3";
            if(post.Length>0){
                pbdata+="&"+post;
            }
            String sign=_stbapi.toSign(pbdata);
            pbdata+="&sign="+sign;
            return _.sendHttp("http://c.tieba.baidu.com" + fpath, pbdata, "");
        }
        /// <summary>
        /// 此函数实现感谢 <a href="http://www.baidu.com/p/877120274">@877120274</a> (面包)
        /// </summary>
        /// <param name="str">POST数据</param>
        /// <returns>sign值</returns>
        public static String toSign(String str){
            string[] buffer = str.Split('&');
            Array.Sort(buffer, new ASCIIComparer());
            var md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] md5Result = md5CSP.ComputeHash(Encoding.UTF8.GetBytes(HttpUtility.UrlDecode(string.Join("", buffer)) + "tiebaclient!!!"));
            string ret = "";
            foreach (byte i in md5Result)
            {
                ret += i.ToString("x2");
            }
            return ret.ToUpper();
        }
    }
    public interface TiebaResult
    {
    }
    public class EntryResult : TiebaResult
    {
    }
    /// <summary>
    /// 出错时拖出。
    /// </summary>
    public class TiebaField : Exception
    {
        /// <summary>
        /// 结果
        /// </summary>
        public readonly TiebaResult tbres;
        /// <summary>
        /// 错误码
        /// </summary>
        public readonly int error_code;
        /// <summary>
        /// 错误文本描述
        /// </summary>
        public readonly String error_msg;
        public TiebaField(TiebaResult tr, int errcode, String errmsg)
            : base("错误" + errcode + "：" + errmsg)
        {
            tbres = tr;
            error_code = errcode;
            error_msg = errmsg;
        }
    }
}
