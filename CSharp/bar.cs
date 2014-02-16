using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Opentieba
{
    public class goodclassflyItem
    {
        public readonly String kw;
        public readonly String name;
        public readonly Int16 id;
        public goodclassflyItem(String kw, String name, Int16 id)
        {
            this.kw = kw;
            this.name = name;
            this.id = id;
        }
        public List<basethread> listThread()
        {
            return null;
        }
    }
    public class SeeBarField : TiebaField
    {
        public readonly String kw;
        public SeeBarField(String kw, int errcode, String errmsg)
            : base(new EntryResult(), errcode, errmsg)
        {
            this.kw = kw;
        }
    }
    public class baseBar
    {
        /// <summary>
        /// 吧名称
        /// </summary>
        public readonly String kw;
        /// <summary>
        /// 贴吧fid
        /// </summary>
        public readonly long fid;
        public baseBar(String kw, long fid)
        {
            this.kw = kw;
            this.fid = fid;
        }
    }
    /// <summary>
    /// 表示一个吧。
    /// </summary>
    public class bar
    {
        /// <summary>
        /// 吧名称
        /// </summary>
        public readonly String kw;
        /// <summary>
        /// 贴吧fid
        /// </summary>
        public readonly long fid;
        /// <summary>
        /// 此吧大吧主的user数组
        /// </summary>
        public readonly List<user> Managers = new List<user>();
        protected readonly JObject barinfo;
        public readonly long maxPage;
        public readonly List<goodclassflyItem> gdclasses = new List<goodclassflyItem>();
        /// <summary>
        /// 根据吧名称构建一个“吧对象”
        /// </summary>
        /// <param name="kw">吧名</param>
        public bar(String kw)
        {
            this.kw = kw;
            String jon = _stbapi.sendTieba("/c/f/frs/page", "kw=" + _.encodeURIComponent(kw) +
                "&is_good=0&pn=1", "");
            barinfo = JSON.parse(jon);
            if (barinfo["error_code"].Value<int>() != 0)
            {
                throw new SeeBarField(kw, barinfo["error_code"].Value<int>(), barinfo["error_msg"].Value<String>());
            }
            maxPage = barinfo["page"]["total_page"].Value<long>();
            JEnumerable<JToken> fromgr = barinfo["forum"]["managers"].Children();
            foreach (JToken jt in fromgr)
            {
                Managers.Add(new user(jt["id"].Value<long>(), jt["name"].Value<String>()));
            }
            JEnumerable<JToken> frgc = barinfo["forum"]["good_classify"].Children();
            foreach (JToken jt in frgc)
            {
                gdclasses.Add(new goodclassflyItem(kw, jt["class_name"].Value<String>(),
                    jt["class_id"].Value<Int16>()));
            }
            fid = barinfo["forum"]["id"].Value<long>();
        }
        /// <summary>
        /// 列出该吧所有主题
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns>一个basethread组成的List组</returns>
        public List<basethread> listThreads(long page)
        {
            String jon = _stbapi.sendTieba("/c/f/frs/page", "kw=" + _.encodeURIComponent(kw) +
                "&is_good=0&pn=" + page, "");
            JEnumerable<JToken> threadlist = JSON.parse(jon)["thread_list"].Children();
            List<basethread> lt = new List<basethread>();
            foreach (JToken jt in threadlist)
            {
                lt.Add(new basethread(jt["tid"].Value<long>(), jt["title"].Value<String>(),
                    jt["reply_num"].Value<long>(), jt["last_time_int"].Value<long>(), (jt["is_top"].Value<Int16>() == 1 ? true : false),
                    (jt["is_good"].Value<Int16>() == 1 ? true : false), new userWithPic(jt["author"]["id"].Value<int>(),
                        jt["author"]["name"].Value<String>(), jt["author"]["portrait"].Value<String>()), kw));
            }
            return lt;
        }
    }
}
